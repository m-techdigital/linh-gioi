"""Native Krita source-repair documents; run author_job(job, output) in Scripter.

After editing and saving a new KRA revision, use
export_revision(job, native_path, selected_native_sha256, new_output_directory).
The document must retain locked BODY_REFERENCE and REFERENCE_BODY clones.
Legacy documents without display references must be migrated explicitly; this
exporter does not silently rebuild them or treat raw body hashes as fit approval.

No runtime pack/promotion is performed. Inputs stay immutable. The source poses
are already registered: identity masks are editable mapping records, not a claim
that this adapter authors mesh deformation or missing garment regions.
"""
from __future__ import annotations

import hashlib
import json
import math
import struct
from xml.etree import ElementTree as ET
from pathlib import Path

from compose_lgo_pose_review_loadout import SLOTS, COMPLETE_GARMENT_LAYER_PROFILE

POSES = ('idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck')
CANVAS = (1024, 1536)


def fingerprint(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def validate_job(job, output):
    """Fail before any write; usable from shell without embedded Krita/Pillow."""
    output = Path(output).resolve()
    if output.exists():
        raise FileExistsError('Use a new output directory: ' + str(output))
    if (job.get('slot') not in SLOTS or job.get('gender') not in ('male', 'female')
            or not job.get('fitFamily') or not job.get('candidateId')):
        raise ValueError('Missing item/slot/body identity')
    if (job.get('sourceCanvas') != list(CANVAS)
            or job.get('sourceSpaceProfile') != 'lgo_character_canvas_1024x1536_v1'
            or job.get('jumpPivotSource') != [512, 820]
            or job.get('layerOrderProfile') != COMPLETE_GARMENT_LAYER_PROFILE):
        raise ValueError('Unsupported canvas/registration/layer profile')
    if set(job.get('poses', {})) != set(POSES):
        raise ValueError('Exactly six authority poses are required')
    if not job.get('designReferences') or not job.get('sourceStatus'):
        raise ValueError('Design provenance and source review status are required')
    material = job.get('variantB', {})
    if set(material) != {'h', 's', 'v'} or any(
            type(v) not in (int, float) or not math.isfinite(v) or not -180 <= v <= 180
            for v in material.values()):
        raise ValueError('Explicit finite HSV material parameters are required')
    mappings = job.get('rigidPoseTransforms')
    if mappings is not None:
        if job['slot'] != 'class_accessory' or set(mappings) != set(POSES):
            raise ValueError('Rigid authoring mapping requires accessory and all six poses')
        for mapping in mappings.values():
            if set(mapping) != {'sourceAnchor', 'targetAnchor', 'angleDegrees'}:
                raise ValueError('Rigid mapping cannot scale, shear or deform')
            for key in ('sourceAnchor', 'targetAnchor'):
                point = mapping[key]
                if len(point) != 2 or any(type(v) not in (int, float) or not math.isfinite(v)
                                         or not 0 <= v <= limit for v, limit in zip(point, CANVAS)):
                    raise ValueError('Rigid anchor must be on the source canvas')
            angle = mapping['angleDegrees']
            if type(angle) not in (int, float) or not math.isfinite(angle) or not -180 <= angle <= 180:
                raise ValueError('Invalid rigid angle')
    refs = [(ref, False) for ref in job['designReferences']]
    for pose in job['poses'].values():
        if set(pose) != {'body', 'front', 'back'}:
            raise ValueError('Only body reference and front/back component roles are supported')
        for role, ref in pose.items():
            if 'emptyReason' in ref and (role == 'body' or not ref['emptyReason'].strip()):
                raise ValueError('Empty component needs an explicit occlusion reason')
            refs.append((ref, True))
    for ref, registered in refs:
        path = Path(ref['path']).resolve()
        if output == path or output in path.parents:
            raise ValueError('Output contains a locked input: ' + str(path))
        if fingerprint(path) != ref.get('sha256'):
            raise ValueError('Source fingerprint changed: ' + str(path))
        if registered:
            with path.open('rb') as file:
                header = file.read(24)
            if (header[:8] != b'\x89PNG\r\n\x1a\n' or header[12:16] != b'IHDR'
                    or struct.unpack('>II', header[16:24]) != CANVAS):
                raise ValueError('Source must use canonical PNG canvas: ' + str(path))
    return output


def rigid_transform_xml(identity_xml, mapping):
    """Serialize authored rigid anchors to Krita's own identity-mask XML."""
    root = ET.fromstring(identity_xml)
    for tag, key in (('originalCenter', 'sourceAnchor'), ('transformedCenter', 'targetAnchor')):
        node = root.find('.//' + tag)
        for attribute, value in zip(('x', 'y'), mapping[key]):
            node.set(attribute, str(value))
    root.find('.//aZ').set('value', str(math.radians(mapping['angleDegrees'])))
    return ET.tostring(root, encoding='unicode')


def author_job(job, output):
    """Create A/B clones, save/reopen KRA, export exact canvas via real Krita API.

    This stages repair inputs, including rejected artwork, without laundering
    their status. Alpha below anatomy may already be absent in legacy inputs;
    native_source_completeness remains unverified until an artist repairs it.
    """
    output = validate_job(job, output)
    from krita import Krita, InfoObject, Selection
    from PyQt5.QtCore import QRect
    from PyQt5.QtGui import QImage

    app = Krita.instance()
    previous_batchmode = app.batchmode()
    width, height = CANVAS
    output.mkdir(parents=True)
    (output / 'input-job.json').write_text(json.dumps(job, ensure_ascii=False, indent=2) + '\n')
    report = {'status': 'AUTHORING_DIAGNOSTIC_ONLY', 'runtimeEligible': False,
              'toolVersion': app.version(), 'candidateId': job['candidateId'],
              'sourceStatus': job['sourceStatus'], 'inputJobSha256': fingerprint(output / 'input-job.json'),
              'nativeSourceCompleteness': 'UNVERIFIED_REPAIR_REQUIRED',
              'mapping': job.get('rigidPoseTransforms', 'identity from registered sources; no mesh/warp authored'),
              'records': []}
    document = None

    def image_bytes(image):
        image = image.convertToFormat(QImage.Format_ARGB32)
        data = image.constBits()
        data.setsize(image.byteCount())
        return bytes(data)

    def import_node(doc, parent, name, ref):
        image = QImage(ref['path'])
        if image.isNull() or (image.width(), image.height()) != CANVAS:
            raise ValueError('Krita image read/canvas failed: ' + ref['path'])
        data = image_bytes(image)
        if not any(data[3::4]) and not ref.get('emptyReason'):
            raise ValueError('Undeclared empty component: ' + name)
        node = doc.createNode(name, 'paintlayer')
        if not parent.addChildNode(node, None) or not node.setPixelData(data, 0, 0, width, height):
            raise RuntimeError('Krita could not import layer: ' + name)
        node.setLocked(True)
        return node, hashlib.sha256(data).hexdigest()

    try:
        app.setBatchmode(True)
        # Each item is one document. Six registered sources remain editable;
        # A and B share those exact sources, including future source repairs.
        document = app.createDocument(width, height, job['candidateId'], 'RGBA', 'U8', '', 120.0)
        document.setBatchmode(True)
        document.setAutosave(False)
        root = document.rootNode()
        sources = document.createGroupLayer('SOURCES_LOCKED_REPAIR_INPUTS')
        root.addChildNode(sources, None)
        source_pixels = {}
        clones = []
        clone_sources = {}
        source_cache = {}
        for pose in POSES:
            values = job['poses'][pose]
            body, raw_hash = import_node(document, sources, 'BODY_REFERENCE/' + pose, values['body'])
            body.setVisible(False)
            source_pixels[body.name()] = raw_hash
            for component in ('back', 'front'):
                if component == 'front':
                    reference = document.createCloneLayer('REFERENCE_BODY/' + pose, body)
                    root.addChildNode(reference, None)
                    reference.setLocked(True)
                    reference.setVisible(pose == 'idle')
                key = (values[component]['path'], values[component]['sha256'])
                if key not in source_cache:
                    source, raw_hash = import_node(document, sources, 'SOURCE/' + pose + '/' + component, values[component])
                    source_cache[key] = source
                    source_pixels[source.name()] = raw_hash
                source = source_cache[key]
                for variant in ('A', 'B'):
                    name = variant + '/' + pose + '/' + component
                    clone = document.createCloneLayer(name, source)
                    root.addChildNode(clone, None)
                    transform = document.createTransformMask('REGISTRATION_IDENTITY_NOT_FIT_APPROVAL')
                    clone.addChildNode(transform, None)
                    if job.get('rigidPoseTransforms'):
                        xml = rigid_transform_xml(transform.toXML(), job['rigidPoseTransforms'][pose])
                        if not transform.fromXML(xml):
                            raise RuntimeError('Krita rejected rigid transform XML: ' + name)
                    if variant == 'B':
                        material = app.filter('hsvadjustment')
                        config = material.configuration()
                        for key, value in job['variantB'].items(): config.setProperty(key, value)
                        material.setConfiguration(config)
                        selection = Selection()
                        selection.select(0, 0, width, height, 255)
                        adjustment = document.createFilterMask('B_SURFACE_VARIANT_NOT_LV10', material, selection)
                        clone.addChildNode(adjustment, None)
                    clone.setVisible(variant == 'A' and pose == 'idle')
                    clones.append(name)
                    clone_sources[name] = source.name()
        sources.setVisible(False)
        sources.setLocked(True)
        document.refreshProjection()
        document.waitForDone()
        native = output / 'authoring.kra'
        if not document.saveAs(str(native)):
            raise RuntimeError('Krita native save failed')
        document.close()
        document = app.openDocument(str(native))
        if document is None:
            raise RuntimeError('Krita native reopen failed')
        document.setBatchmode(True)
        document.waitForDone()
        if (document.width(), document.height()) != CANVAS:
            raise ValueError('Native canvas drift')
        for name, expected in source_pixels.items():
            node = document.nodeByName(name)
            if node is None or not node.locked():
                raise ValueError('Locked source lost on reopen: ' + name)
            actual = hashlib.sha256(bytes(node.pixelData(0, 0, width, height))).hexdigest()
            if actual != expected:
                raise ValueError('Native source pixel drift: ' + name)
        variant_a_alpha = {}
        for name in clones:
            node = document.nodeByName(name)
            variant, pose, component = name.split('/')
            if node is None or node.type() != 'clonelayer':
                raise ValueError('Native clone lost: ' + name)
            source = node.sourceNode()
            if source is None or source.name() != clone_sources[name]:
                raise ValueError('Native clone source mismatch: ' + name)
            node.setVisible(True)
            document.refreshProjection()
            document.waitForDone()
            path = output / variant / pose / (component + '.png')
            path.parent.mkdir(parents=True, exist_ok=True)
            node.save(str(path), document.xRes(), document.yRes(), InfoObject(), QRect(0, 0, width, height))
            image = QImage(str(path))
            if image.isNull() or (image.width(), image.height()) != CANVAS:
                raise ValueError('Export lost source canvas: ' + name)
            exported = image_bytes(image)
            projection = bytes(node.projectionPixelData(0, 0, width, height))
            if exported != projection:
                raise ValueError('Export differs from reopened layer projection: ' + name)
            source_data = bytes(source.pixelData(0, 0, width, height))
            if variant == 'A' and not job.get('rigidPoseTransforms') and exported != source_data:
                raise ValueError('Identity mapping changed source pixels: ' + name)
            if variant == 'A':
                variant_a_alpha[(pose, component)] = exported[3::4]
            if variant == 'B' and exported[3::4] != variant_a_alpha[(pose, component)]:
                raise ValueError('Surface variant changed garment alpha: ' + name)
            report['records'].append({'variant': variant, 'pose': pose, 'component': component,
                                      'source': str(path), 'sourceSha256': fingerprint(path),
                                      'sourceCanvasRect': [0, 0, width, height],
                                      'cloneSource': source.name(), 'alphaPreserved': True})
            node.setVisible(False)
        report['nativeSha256'] = fingerprint(native)
        # Recheck external authorities after the entire operation.
        for values in job['poses'].values():
            for ref in values.values():
                if fingerprint(ref['path']) != ref['sha256']:
                    raise ValueError('Locked external input changed during export')
        report['technicalRoundtrip'] = 'PASS'
    except Exception as error:
        report['technicalRoundtrip'] = 'FAIL'
        report['error'] = str(error)
        raise
    finally:
        app.setBatchmode(previous_batchmode)
        if document is not None:
            document.setModified(False)
            document.close()
        (output / 'authoring-report.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    return report


def validate_revision(job, native, expected_sha256, output):
    """Pin the artist-selected revision; never overwrite an existing artifact."""
    output = validate_job(job, output)
    native = Path(native).resolve()
    if native.suffix.lower() != '.kra':
        raise ValueError('Revision must be a KRA document')
    if fingerprint(native) != expected_sha256:
        raise ValueError('Native fingerprint changed since revision selection')
    if output == native or output in native.parents:
        raise ValueError('Output contains the native input')
    return native, output


def validate_rigid_mask(xml):
    root = ET.fromstring(xml)
    data = root.find('data')
    if data is None or data.get('mode') != '0':
        raise ValueError('Rigid accessory cannot use warp/perspective mode')
    for tag, expected in dict(scaleX=1, scaleY=1, shearX=0, shearY=0, aX=0, aY=0).items():
        node = root.find('.//' + tag)
        if node is None or float(node.get('value', 'nan')) != expected:
            raise ValueError('Rigid accessory changed ' + tag)
    perspective = root.find('.//flattenedPerspectiveTransform')
    for row in range(1, 4):
        for col in range(1, 4):
            expected = 1 if row == col else 0
            if perspective is None or float(perspective.get(f'm{row}{col}', 'nan')) != expected:
                raise ValueError('Rigid accessory has perspective deformation')


def export_revision(job, native, expected_sha256, output):
    """Reopen an edited KRA, preserve body authority, export A/B shared layers.

    Garment pixels and masks may be repaired in the native document. This is
    diagnostic export only: no anatomy/fit approval or runtime ingestion.
    """
    native, output = validate_revision(job, native, expected_sha256, output)
    from krita import Krita, InfoObject
    from PyQt5.QtCore import QRect
    from PyQt5.QtGui import QImage
    app = Krita.instance()
    previous = app.batchmode()
    document = None
    output.mkdir(parents=True)
    (output / 'input-job.json').write_text(json.dumps(job, ensure_ascii=False, indent=2) + '\n')
    report = {'status': 'AUTHORING_DIAGNOSTIC_ONLY', 'runtimeEligible': False,
              'inputJobSha256': fingerprint(output / 'input-job.json'),
              'sourceStatus': job['sourceStatus'], 'candidateId': job['candidateId'],
              'bodyAnatomyStatus': 'SOURCE_ANATOMY_REVIEW_REQUIRED',
              'nativeSourceCompleteness': 'UNVERIFIED_REPAIR_REQUIRED',
              'native': str(native), 'nativeSha256': expected_sha256,
              'toolVersion': app.version(), 'records': []}

    def pixels(image):
        image = image.convertToFormat(QImage.Format_ARGB32)
        data = image.constBits()
        data.setsize(image.byteCount())
        return bytes(data)

    try:
        app.setBatchmode(True)
        document = app.openDocument(str(native))
        if document is None:
            raise ValueError('Native reopen failed')
        document.setBatchmode(True)
        document.waitForDone()
        if (document.width(), document.height()) != CANVAS:
            raise ValueError('Native canvas drift')
        # Check every body before exporting any garment. Locked alone is not proof.
        for pose in POSES:
            body = document.nodeByName('BODY_REFERENCE/' + pose)
            expected = QImage(job['poses'][pose]['body']['path'])
            if body is None or not body.locked() or body.type() != 'paintlayer':
                raise ValueError('Body reference missing or unlocked: ' + pose)
            if bytes(body.pixelData(0, 0, *CANVAS)) != pixels(expected):
                raise ValueError('Body reference pixels changed: ' + pose)
            reference = document.nodeByName('REFERENCE_BODY/' + pose)
            if (reference is None or reference.type() != 'clonelayer' or not reference.locked()
                    or reference.sourceNode() is None
                    or reference.sourceNode().uniqueId() != body.uniqueId()
                    or body.childNodes() or reference.childNodes()):
                raise ValueError('Body display reference changed: ' + pose)
            reference.setVisible(True)
            document.refreshProjection()
            document.waitForDone()
            if bytes(reference.projectionPixelData(0, 0, *CANVAS)) != pixels(expected):
                raise ValueError('Body display projection changed: ' + pose)
            reference.setVisible(False)
        source_names = {}
        source_ids = {}
        for pose in POSES:
            for component in ('back', 'front'):
                ref = job['poses'][pose][component]
                key = (ref['path'], ref['sha256'])
                expected_name = source_names.setdefault(key, 'SOURCE/' + pose + '/' + component)
                shared_source = None
                alpha_a = None
                for variant in ('A', 'B'):
                    name = variant + '/' + pose + '/' + component
                    node = document.nodeByName(name)
                    if node is None or node.type() != 'clonelayer':
                        raise ValueError('Native clone missing: ' + name)
                    source = node.sourceNode()
                    if source is None or source.name() != expected_name or source.type() != 'paintlayer':
                        raise ValueError('Garment clone source changed: ' + name)
                    if shared_source is not None and source.uniqueId() != shared_source:
                        raise ValueError('A/B no longer share garment source: ' + name)
                    shared_source = source.uniqueId()
                    if source_ids.setdefault(key, shared_source) != shared_source:
                        raise ValueError('Shared garment master changed across poses: ' + name)
                    if job.get('rigidPoseTransforms'):
                        if source.childNodes():
                            raise ValueError('Rigid source has unverified modifiers: ' + name)
                        for child in node.childNodes():
                            if child.type() == 'transformmask':
                                validate_rigid_mask(child.toXML())
                            elif child.type() != 'filtermask':
                                raise ValueError('Rigid clone has unverified modifier: ' + name)
                    node.setVisible(True)
                    document.refreshProjection()
                    document.waitForDone()
                    path = output / variant / pose / (component + '.png')
                    path.parent.mkdir(parents=True, exist_ok=True)
                    node.save(str(path), document.xRes(), document.yRes(), InfoObject(), QRect(0, 0, *CANVAS))
                    image = QImage(str(path))
                    if image.isNull() or (image.width(), image.height()) != CANVAS:
                        raise ValueError('Export canvas drift: ' + name)
                    data = pixels(image)
                    if data != bytes(node.projectionPixelData(0, 0, *CANVAS)):
                        raise ValueError('Export differs from reopened projection: ' + name)
                    alpha = data[3::4]
                    if not any(alpha) and not ref.get('emptyReason'):
                        raise ValueError('Required garment became empty: ' + name)
                    if variant == 'A':
                        alpha_a = alpha
                    elif alpha != alpha_a:
                        raise ValueError('Surface variant changed garment alpha: ' + name)
                    report['records'].append({'variant': variant, 'pose': pose,
                        'component': component, 'source': str(path),
                        'sourceSha256': fingerprint(path), 'sourceCanvasRect': [0, 0, *CANVAS],
                        'cloneSource': expected_name, 'alphaPreserved': True})
                    node.setVisible(False)
        if fingerprint(native) != expected_sha256:
            raise ValueError('Native changed during export')
        for pose in job['poses'].values():
            for ref in pose.values():
                if fingerprint(ref['path']) != ref['sha256']:
                    raise ValueError('External source changed during export')
        report['technicalRoundtrip'] = 'PASS'
    except Exception as error:
        report['technicalRoundtrip'] = 'FAIL'
        report['error'] = str(error)
        raise
    finally:
        if document is not None:
            document.setModified(False)
            document.close()
        app.setBatchmode(previous)
        (output / 'revision-export-report.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    return report
