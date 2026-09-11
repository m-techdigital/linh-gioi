#!/usr/bin/env python3
"""Render an entire packed outfit and its slot removals before a Unity build.

Uses the runtime manifest's atlas rectangles, world bounds and sorting orders.
This is an idle asset diagnostic, never animation or Player PASS evidence.
"""

import argparse
import json
import math
from pathlib import Path

from PIL import Image, ImageChops, ImageDraw, ImageStat
from pack_lgo_vo_lv1_map_avatar import registration_errors


def compare_bind_pose(reference: Image.Image, equipped: Image.Image) -> dict:
    """Compare one fixed outfit in the same pose/space, excluding UI and ground.

    This is a rejection gate, not proof of art approval or moving joint coverage.
    Never normalize each silhouette to its own bounding box before comparing.
    """
    if reference.size != equipped.size:
        raise ValueError("Bind comparisons must share a canvas")
    masks = [im.getchannel("A").point(lambda a: 255 if a >= 32 else 0)
             for im in (reference, equipped)]
    counts = [mask.histogram()[255] for mask in masks]
    if not all(counts):
        raise ValueError("Empty reference or equipped silhouette")
    union = ImageChops.lighter(*masks)
    intersection = ImageChops.multiply(*masks).histogram()[255]
    iou = intersection / union.histogram()[255]
    area_ratio = counts[1] / counts[0]
    difference = ImageChops.difference(reference.convert("RGB"), equipped.convert("RGB"))
    color_error = sum(ImageStat.Stat(difference, mask=union).mean) / 3
    return {"silhouetteIoU": round(iou, 5), "areaRatio": round(area_ratio, 5),
            "meanRgbError": round(color_error, 3),
            "passed": iou >= .90 and .90 <= area_ratio <= 1.10 and color_error <= 22,
            "limits": {"minimumIoU": .90, "areaRatio": [.90, 1.10], "maximumMeanRgbError": 22}}


def render_pack(pack_dir: Path, output: Path, level: int = 1) -> dict:
    manifest = json.loads((pack_dir / "manifest.json").read_text())
    if level not in manifest["levels"]:
        raise ValueError(f"Level {level} is absent from the pack")
    output.mkdir(parents=True, exist_ok=False)
    atlases = {a["id"]: Image.open(pack_dir / a["file"]).convert("RGBA")
               for a in manifest["atlases"]}
    slots = manifest["slots"]
    ppu, ground, height = 200, 365, 400
    visible = manifest["rigParts"] + [p for p in manifest["equipmentComponents"] + manifest["parts"]
                                    if p["level"] == level]
    extent = max(abs(p["dx"]) + p["worldW"] / 2 for p in visible)
    width = max(260, math.ceil(extent * ppu) * 2 + 24)

    def composite(entries):
        canvas = Image.new("RGBA", (width, height))
        for part in sorted(entries, key=lambda p: p["order"]):
            atlas = atlases[part["atlas"]]
            x, y, w, h = (part[k] for k in ("x", "y", "w", "h"))
            if min(x, y) < 0 or min(w, h) <= 0 or x + w > atlas.width or y + h > atlas.height:
                raise ValueError(f"Invalid atlas rectangle: {part['id']}")
            sprite = atlas.crop((x, atlas.height - y - h, x + w, atlas.height - y))
            sprite = sprite.resize((max(1, round(part["worldW"] * ppu)),
                                    max(1, round(part["worldH"] * ppu))), Image.Resampling.LANCZOS)
            left = round(width / 2 + (part["dx"] - part["worldW"] / 2) * ppu)
            top = round(ground - (part["dy"] + part["worldH"] / 2) * ppu)
            canvas.alpha_composite(sprite, (left, top))
        return canvas

    def render(entries, label):
        canvas = Image.new("RGBA", (width, height), "#33434d")
        ImageDraw.Draw(canvas).line((0, ground, width, ground), fill="#85949e")
        canvas.alpha_composite(composite(entries))
        ImageDraw.Draw(canvas).text((8, 8), label, fill="white")
        return canvas.convert("RGB")

    records, comparison = [], []
    for gender in manifest["genders"]:
        # Nonzero idle angles need hierarchical evaluation, not silent omission.
        idle = [p for p in manifest["rigPoseProfiles"] if p["gender"] == gender and p["pose"] == "idle"]
        if any(abs(p.get("rotation", 0)) > 0.00001 or abs(p.get("dx", 0)) > 0.00001
               or abs(p.get("dy", 0)) > 0.00001 for p in idle):
            raise ValueError("This diagnostic supports only the unrotated bind pose")
        parts = [p for p in manifest["parts"] if p["level"] == level and p["gender"] == gender]
        body = [p for p in manifest["rigParts"] if p["gender"] == gender]
        clothes = [p for p in manifest["equipmentComponents"]
                   if p["level"] == level and p["gender"] == gender]
        if set(p["slot"] for p in clothes) != set(slots):
            raise ValueError(f"Incomplete slot coverage: {gender}/{level}")
        panels = []
        for kind in ("full", "base"):
            reference = [p for p in parts if p["kind"] == kind]
            if len(reference) != 1:
                raise ValueError(f"Expected one {kind} reference for {gender}/{level}")
            panels.append(render(reference, f"{gender} Lv{level} {kind} reference"))
        panels += [render(body, "body rig / bind"), render(body + clothes, "10 slots / bind")]
        comparison += [panels[0], panels[2], panels[3]]
        fit = compare_bind_pose(composite([p for p in parts if p["kind"] == "full"]),
                                composite(body + clothes))
        for slot in slots:
            panels.append(render(body + [p for p in clothes if p["slot"] != slot], f"off: {slot}"))
        board = Image.new("RGB", (width * 7, height * math.ceil(len(panels) / 7)), "#182028")
        for i, panel in enumerate(panels):
            board.paste(panel, ((i % 7) * width, (i // 7) * height))
        board.save(output / f"{gender}-all-slots.jpg", quality=90)
        records.append({"gender": gender, "level": level, "slots": len(slots),
                        "bodyParts": len(body), "components": len(clothes), "referenceFit": fit})
    board = Image.new("RGB", (width * 3, height * len(records)), "#182028")
    for i, panel in enumerate(comparison):
        board.paste(panel, ((i % 3) * width, (i // 3) * height))
    board.save(output / "reference-body-equipped.jpg", quality=92)
    fit_passed = all(record["referenceFit"]["passed"] for record in records)
    registration = registration_errors(visible)
    report = {"status": "BIND_POSE_REVIEW_REQUIRED" if fit_passed and not registration else "BIND_POSE_FIT_REJECTED",
              "registrationPassed": not registration, "registrationErrors": registration,
              "referenceFitPassed": fit_passed, "pack": str(pack_dir.resolve()),
              "records": records, "pixelsPerWorldUnit": ppu,
              "nonClaims": ["No motion evaluation", "No Player evidence", "No visual approval"]}
    (output / "report.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n")
    return report


def _rasterize_registered_layer(atlas, layer):
    """Rasterize exported Unity geometry at canonical div4 pixel centers."""
    import numpy as np
    texture = np.asarray(atlas.convert('RGBA'))
    points = np.array([[p['x'], p['y']] for p in layer['points']], dtype=float) / 4
    uv = np.array([[p['x'], p['y']] for p in layer['uv']], dtype=float)
    indices = np.array(layer['triangles'], dtype=int).reshape(-1, 3)
    if not len(points) or uv.shape != points.shape or not len(indices) or not np.isfinite(points).all() or not np.isfinite(uv).all():
        raise ValueError('Incomplete or nonfinite Unity geometry')
    if (uv < 0).any() or (uv > 1).any():
        raise ValueError('Unity UV coordinates are outside the atlas')
    if indices.min() < 0 or indices.max() >= len(points):
        raise ValueError('Invalid Unity triangle indices')
    result = np.zeros((384, 256, 4), dtype=np.uint8)
    for ids in indices:
        tri = points[ids]
        lo = np.maximum(np.floor(tri.min(axis=0)).astype(int), [0, 0])
        hi = np.minimum(np.ceil(tri.max(axis=0)).astype(int), [256, 384])
        if np.any(hi <= lo):
            continue
        yy, xx = np.mgrid[lo[1]:hi[1], lo[0]:hi[0]]
        q = np.stack((xx + .5, yy + .5), axis=-1) - tri[0]
        a, b = tri[1] - tri[0], tri[2] - tri[0]
        det = a[0] * b[1] - a[1] * b[0]
        if abs(det) < 1e-10:
            continue
        w1 = (q[..., 0] * b[1] - q[..., 1] * b[0]) / det
        w2 = (a[0] * q[..., 1] - a[1] * q[..., 0]) / det
        covered = (w1 >= -1e-7) & (w2 >= -1e-7) & (w1 + w2 <= 1 + 1e-7)
        samples = uv[ids[0]] + w1[..., None] * (uv[ids[1]] - uv[ids[0]]) + w2[..., None] * (uv[ids[2]] - uv[ids[0]])
        tx = np.clip(np.floor(samples[..., 0] * texture.shape[1]).astype(int), 0, texture.shape[1] - 1)
        ty = np.clip(texture.shape[0] - 1 - np.floor(samples[..., 1] * texture.shape[0]).astype(int), 0, texture.shape[0] - 1)
        result[yy[covered], xx[covered]] = texture[ty[covered], tx[covered]]
    return Image.fromarray(result)


def registered_bind_input_paths(repo_root: Path):
    unity = repo_root / 'client/Unity'
    runtime = Path('Assets/Game/World/Runtime')
    paths = [p.relative_to(unity) for p in (unity / runtime).glob('TwoDRegistered*.cs')]
    paths += [Path('Assets/Game/Tests/EditMode/TwoDRegisteredEquipmentTests.cs'),
              runtime / 'Resources/LGOClasses/VoRegisteredLv1/manifest.json',
              runtime / 'Resources/LGOClasses/VoRegisteredLv1/anatomical-meshes.json', Path('Packages/packages-lock.json')]
    if runtime / 'TwoDRegisteredOutfit.cs' not in paths or not all((unity / p).is_file() for p in paths):
        raise ValueError('Incomplete registered runtime/exporter inputs')
    return sorted(str(p) for p in paths)


def render_registered_bind_dump(repo_root: Path, dump_path: Path, output: Path) -> dict:
    """Compare original canonical source assembly against exported bound Unity meshes.

    Layer selection/order comes from Unity. This checks bind projection/material
    fidelity; it does not independently approve sorting, anatomy, or art direction.
    """
    import hashlib
    def digest(path):
        return hashlib.sha256(path.read_bytes()).hexdigest()
    evidence = json.loads(dump_path.read_text())
    inputs = evidence.get('inputs', [])
    if sorted(p['path'] for p in inputs) != registered_bind_input_paths(repo_root):
        raise ValueError('Incomplete runtime/exporter input fingerprints')
    if any(digest(repo_root / 'client/Unity' / p['path']) != p['sha256'] for p in inputs):
        raise ValueError('Unity dump does not match current runtime/exporter inputs')
    layers = evidence['layers']
    if not layers or len({(l['atlas'], l['id']) for l in layers}) != len(layers):
        raise ValueError('Missing or repeated rendered layers')
    output.mkdir(parents=True, exist_ok=False)
    reference = Image.new('RGBA', (256, 384))
    candidate = Image.new('RGBA', reference.size)
    selected, sources, cache = [], [], {}
    for layer in sorted(layers, key=lambda l: l['order']):
        atlas_path = repo_root / 'client/Unity' / layer['atlas']
        manifest_path = atlas_path.parent / 'manifest.json'
        if digest(atlas_path) != layer['atlasSha256'] or digest(manifest_path) != layer['manifestSha256']:
            raise ValueError('Unity dump does not match current atlas/manifest')
        if atlas_path not in cache:
            cache[atlas_path] = Image.open(atlas_path).convert('RGBA'), json.loads(manifest_path.read_text())
        atlas, manifest = cache[atlas_path]
        matches = [p for p in manifest['parts'] if p['id'] == layer['id']]
        if len(matches) != 1:
            raise ValueError('Rendered layer is absent or ambiguous in manifest')
        part = matches[0]; selected.append(part)
        provenance = part.get('provenance', [])
        if not part.get('source') and not provenance:
            raise ValueError('Missing canonical source provenance')
        source_path = Path(part['source']) if part.get('source') else Path(provenance[0]['path'])
        expected_sha = part.get('sourceSha256') or provenance[0]['sha256']
        if digest(source_path) != expected_sha:
            raise ValueError('Canonical source fingerprint mismatch')
        source = Image.open(source_path).convert('RGBA')
        if source.size == (1024, 1536):
            source = source.resize((256, 384), Image.Resampling.LANCZOS)
        elif source.size != (256, 384):
            raise ValueError('Source must retain the full canonical canvas')
        # The authoring rect owns this part of the original canonical canvas.
        box = tuple(round(v / 4) for v in part['sourceCanvasRect'])
        owned = Image.new('RGBA', source.size)
        owned.alpha_composite(source.crop(box), box[:2])
        reference.alpha_composite(owned)
        candidate.alpha_composite(_rasterize_registered_layer(atlas, layer))
        sources.append({'id': part['id'], 'source': str(source_path), 'sha256': expected_sha})
    fit = compare_bind_pose(reference, candidate)
    errors = registration_errors(selected)
    reference.save(output / 'canonical-source.png'); candidate.save(output / 'unity-bind.png')
    board = Image.new('RGB', (512, 408), '#303b46')
    board.paste(reference, (0, 24), reference); board.paste(candidate, (256, 24), candidate)
    draw = ImageDraw.Draw(board); draw.text((6, 5), 'Canonical sources', fill='white'); draw.text((262, 5), 'Unity bound geometry + atlas', fill='white')
    board.save(output / 'bind-comparison.png')
    report = {'status': 'BIND_POSE_REVIEW_REQUIRED' if fit['passed'] and not errors else 'BIND_POSE_FIT_REJECTED',
              'referenceFitPassed': fit['passed'], 'registrationPassed': not errors, 'registrationErrors': errors,
              'referenceFit': fit, 'gender': evidence['gender'], 'renderedLayers': len(layers),
              'dumpSha256': digest(dump_path), 'sources': sources,
              'nonClaims': ['No independent sorting or art approval', 'No motion or physical-device approval',
                            'Canonical density software rasterization, not a new Player screenshot']}
    (output / 'report.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    return report


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument("--pack-dir", type=Path)
    mode.add_argument("--registered-bind-dump", type=Path)
    parser.add_argument("--repo-root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--out-dir", type=Path, required=True)
    parser.add_argument("--level", type=int, default=1)
    parser.add_argument("--require-fit", action="store_true")
    args = parser.parse_args()
    report = (render_registered_bind_dump(args.repo_root, args.registered_bind_dump, args.out_dir)
              if args.registered_bind_dump else render_pack(args.pack_dir, args.out_dir, args.level))
    print(json.dumps(report))
    if args.require_fit and (not report["referenceFitPassed"] or not report["registrationPassed"]):
        raise SystemExit(2)
