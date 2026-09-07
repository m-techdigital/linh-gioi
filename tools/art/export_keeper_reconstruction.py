"""Export the whole keeper remake from its editable Blender source (run with Blender)."""
import argparse
import hashlib
import json
import sys
from pathlib import Path

import bpy

parser = argparse.ArgumentParser()
parser.add_argument('--source', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
rig = bpy.data.objects['Armature']
mesh = bpy.data.objects['Keeper Macro Reconstruction']
for bone in rig.pose.bones:
    bone.matrix_basis.identity()
bpy.context.view_layer.update()
mesh.data.calc_loop_triangles()
assert len(rig.data.bones) == 65
assert len(mesh.data.loop_triangles) > 0
assert len(mesh.data.materials) == 2
assert len(mesh.data.uv_layers) == 1
assert all(1 <= len(v.groups) <= 4 and abs(sum(g.weight for g in v.groups) - 1) < 1e-5
           for v in mesh.data.vertices)
args.output.mkdir(parents=True, exist_ok=True)
for name in ['KeeperReconstructionAlbedo', 'KeeperReconstructionFace']:
    image = bpy.data.images[name]
    # Review the native authoring atlas; platform reduction is deferred until fidelity is approved.
    native_size = (1024, 512) if name.endswith('Face') else (4096, 2048)
    assert tuple(image.size) == native_size, (name, list(image.size))
    image.filepath_raw = str((args.output / (name + '.png')).resolve())
    image.file_format = 'PNG'
    image.save()
# Semantic material sections transport module ownership through FBX without guessing from bone weights or height.
catalog = json.loads((Path(__file__).resolve().parents[2] / 'client/art-source/appearance-slots.json').read_text())
parts = mesh.data.attributes.get('appearance_slot')
assert parts is not None and parts.domain == 'FACE', 'Label every face with its wardrobe slot before export'
part_ids = [entry.value for entry in parts.data]
material_ids = [polygon.material_index for polygon in mesh.data.polygons]
assert all(str(part) in catalog for part in part_ids), 'Unknown or unassigned wardrobe slot'
source_materials = list(mesh.data.materials)
sections = sorted(set(zip(part_ids, material_ids)))
mesh.data.materials.clear()
for part, surface in sections:
    material = source_materials[surface].copy()
    material.name = 'LGO_' + catalog[str(part)]['key'] + ('_Face' if surface == 1 else '_Body')
    mesh.data.materials.append(material)
section_indices = {section: index for index, section in enumerate(sections)}
for polygon in mesh.data.polygons:
    polygon.material_index = section_indices[(part_ids[polygon.index], material_ids[polygon.index])]
bpy.ops.object.select_all(action='DESELECT')
mesh.select_set(True)
rig.select_set(True)
bpy.context.view_layer.objects.active = mesh
fbx = args.output / 'GateKeeper.fbx'
bpy.ops.export_scene.fbx(filepath=str(fbx.resolve()), use_selection=True,
                        object_types={'ARMATURE', 'MESH'}, add_leaf_bones=False,
                        bake_anim=False, axis_forward='-Z', axis_up='Y', path_mode='STRIP')
receipt = {
    'source_sha256': hashlib.sha256(args.source.read_bytes()).hexdigest(),
    'fbx_sha256': hashlib.sha256(fbx.read_bytes()).hexdigest(),
    'triangles': len(mesh.data.loop_triangles), 'vertices': len(mesh.data.vertices),
    'bones': len(rig.data.bones), 'materials': 2, 'authoring_material_sections': len(sections),
    'slots': [catalog[str(part)]['key'] for part in sorted(set(part_ids))],
    'texture_sizes': {'albedo': list(bpy.data.images['KeeperReconstructionAlbedo'].size), 'face': list(bpy.data.images['KeeperReconstructionFace'].size)},
    'runtime_verified': False,
}
(args.output / 'export-receipt.json').write_text(json.dumps(receipt, indent=2) + '\n')
print('LGO_KEEPER_RECONSTRUCTION_EXPORTED ' + json.dumps(receipt), flush=True)
