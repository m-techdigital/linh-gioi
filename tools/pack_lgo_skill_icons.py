"""Registered UI icon decomposition: one ring master, independent inner art."""
from __future__ import annotations
import argparse
import hashlib
import json
import math
import io
import os
import tempfile
from pathlib import Path
from PIL import Image, ImageChops

ROOT = Path(__file__).resolve().parents[1]
SOURCE = Path("/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/assets-skills/skill-icons-alpha-SELF-REVIEWED-v1.png")
SOURCE_SHA = "04398f792d6514a3d5dc87067bf1e16ad1a2dc53d9a78939b3f3fdba0d5bb749"
OUTPUT = ROOT / "client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ASkillIcons"
IDS = ("thien_kiem_quyet", "lang_khong_bo", "kiem_vu", "ho_the", "song_kiem", "phong_tram", "kiem_tran", "ngu_kiem", "van_kiem", "category_active", "category_passive", "category_method")

CENTERS = ((189,181),(545,181),(902,181),(1260,181),(189,532),(545,532),(902,532),(1260,532),(189,883),(545,883),(902,883),(1260,883))

def radial_mask(radius: float, outside: bool = False) -> Image.Image:
    mask = Image.new("L", (328, 328))
    values = [round(255 * max(0, min(1, (radius-math.hypot(x-163.5, y-163.5))/2)))
              for y in range(328) for x in range(328)]
    mask.putdata([255-v for v in values] if outside else values)
    return mask


BASE_BYTE_BUDGET = 400000
EXTRA_BYTE_BUDGET = 32768


def read_artwork_registry(registry: Path | None) -> tuple[list[dict], dict | None]:
    """Register data, not builders: any known skill can supply ring-free art."""
    if registry is None:
        return [], None
    registry = Path(registry).resolve()
    raw = registry.read_bytes()
    document = json.loads(raw)
    if not isinstance(document, dict) or document.get('version') != 1:
        raise ValueError('Unsupported artwork registry version')
    review = document.get('review', {})
    if not isinstance(review, dict) or review.get('status') != 'SELF_REVIEWED':
        raise ValueError('Artwork review required; DRAFT inputs cannot be packed')
    evidence = registry.parent / review.get('evidence', '')
    if not evidence.is_file():
        raise ValueError('Artwork review evidence missing')
    library = json.loads((OUTPUT / 'skill-library.json').read_text())['skills']
    allowed = {skill['iconId'] for skill in library}
    rows, definitions = document.get('parts'), document.get('sources')
    if not isinstance(rows, list) or not rows or not isinstance(definitions, list) or not definitions:
        raise ValueError('Artwork sources and parts must be nonempty lists')
    seen, sources, metadata = set(IDS) | {'frame'}, {}, []
    for entry in definitions:
        if not isinstance(entry, dict) or not isinstance(entry.get('id'), str) or not entry['id']:
            raise ValueError('Invalid artwork source ID')
        key = entry['id']
        if key in sources or not isinstance(entry.get('path'), str):
            raise ValueError('Duplicate source ID or invalid source path')
        path = (registry.parent / entry['path']).resolve()
        if not path.is_file():
            raise ValueError('Artwork source missing: ' + str(path))
        data = path.read_bytes()
        sha = hashlib.sha256(data).hexdigest()
        if sha != entry.get('sha256'):
            raise ValueError('Artwork source hash mismatch: ' + key)
        with Image.open(io.BytesIO(data)) as image:
            if image.format != 'PNG' or list(image.size) != entry.get('size'):
                raise ValueError('Artwork source PNG/size mismatch: ' + key)
            if max(image.size) > 4096:
                raise ValueError('Artwork source exceeds the 4096px input budget')
            sources[key] = image.convert('RGBA')
        metadata.append({'id': key, 'path': str(path), 'sha256': sha, 'size': entry['size']})
    items = []
    for entry in rows:
        if not isinstance(entry, dict) or not isinstance(entry.get('id'), str):
            raise ValueError('Invalid artwork part')
        key, source_id, rect = entry['id'], entry.get('sourceId'), entry.get('rect')
        if key in seen or key not in allowed:
            raise ValueError('Duplicate/existing or unknown skill artwork ID: ' + key)
        if not isinstance(source_id, str) or source_id not in sources:
            raise ValueError('Unknown artwork source: ' + str(source_id))
        if not isinstance(rect, list) or len(rect) != 4 or any(type(v) is not int for v in rect):
            raise ValueError('Artwork rect must contain four integer coordinates')
        x, y, width, height = rect
        board = sources[source_id]
        if width != height or width < 128:
            raise ValueError('Artwork must use a square source canvas of at least 128px; no upscale')
        if min(x, y) < 0 or x + width > board.width or y + height > board.height:
            raise ValueError('Artwork rect is outside its registered source bounds')
        tile = board.crop((x, y, x + width, y + height))
        if tile.getchannel('A').getbbox() is None:
            raise ValueError('Empty artwork: ' + key)
        seen.add(key)
        items.append({'id': key, 'sourceId': source_id, 'sourceRect': rect, 'image': tile})
    if len(items) + len(IDS) + 1 > 64:
        raise ValueError('Artwork atlas capacity exceeded')
    provenance = {'registrySha256': hashlib.sha256(raw).hexdigest(), 'registryPath': str(registry),
                  'contentCount': len(items), 'sources': sorted(metadata, key=lambda item: item['id']),
                  'review': {'status': 'SELF_REVIEWED', 'evidence': str(evidence.resolve()),
                             'sha256': hashlib.sha256(evidence.read_bytes()).hexdigest()},
                  'pixelPolicy': 'Same fixed 128px canvas and aperture as the base; never fit each symbol by its bounding box.'}
    return sorted(items, key=lambda item: item['id']), provenance


def pack(source: Path = SOURCE, output: Path = OUTPUT, registry: Path | None = None) -> dict:
    extras, intake = read_artwork_registry(registry)
    if hashlib.sha256(source.read_bytes()).hexdigest() != SOURCE_SHA:
        raise ValueError("Skill source hash mismatch: " + str(source))
    with Image.open(source) as image:
        if image.size != (1448, 1086):
            raise ValueError("Skill source must use its registered 4x3 grid of 362px cells")
        board = image.convert("RGBA")
    side = 512 if len(IDS) + 1 + len(extras) <= 16 else 1024
    columns = side // 128
    atlas = Image.new("RGBA", (side, side))
    inner = radial_mask(143)
    aperture = ImageChops.multiply(radial_mask(143, True), radial_mask(158))
    parts = []
    for index, icon_id in enumerate((*IDS, "frame")):
        # A single registered category medallion supplies the frame; no class
        # screenshot, symbol, caption or gameplay data is baked into the rim.
        source_index = 10 if icon_id == "frame" else index
        cx, cy = CENTERS[source_index]
        tile = board.crop((cx-164, cy-164, cx+164, cy+164))
        tile.putalpha(ImageChops.multiply(tile.getchannel("A"), aperture if icon_id == "frame" else inner))
        tile = tile.resize((120, 120), Image.Resampling.LANCZOS)
        tile.putalpha(tile.getchannel("A").point(lambda alpha: 0 if alpha <= 2 else alpha))
        x, top = index % columns * 128, index // columns * 128
        atlas.alpha_composite(tile, (x+4, top+4))
        parts.append(dict(id=icon_id, x=x, y=side-top-128, w=128, h=128,
                          role="shared-frame" if icon_id == "frame" else "inner-symbol"))
    inner120 = inner.resize((120, 120), Image.Resampling.LANCZOS)
    for index, item in enumerate(extras, len(IDS) + 1):
        tile = item['image'].resize((120, 120), Image.Resampling.LANCZOS)
        tile.putalpha(ImageChops.multiply(tile.getchannel('A'), inner120).point(lambda alpha: 0 if alpha <= 2 else alpha))
        x, top = index % columns * 128, index // columns * 128
        atlas.alpha_composite(tile, (x + 4, top + 4))
        parts.append(dict(id=item['id'], x=x, y=side-top-128, w=128, h=128, role='inner-symbol',
                          sharedFrameId='frame', sourceId=item['sourceId'], sourceRect=item['sourceRect']))
    encoded = io.BytesIO()
    atlas.save(encoded, format='PNG', optimize=True)
    png = encoded.getvalue()
    budget = BASE_BYTE_BUDGET + EXTRA_BYTE_BUDGET * len(extras)
    if len(png) > budget:
        raise ValueError('Skill icon atlas exceeds its byte budget: ' + str(budget))
    path = output / 'map01a-skill-icons.png'
    sha = hashlib.sha256(png).hexdigest()
    manifest = dict(id="map01a-skill-icons-v1", revision="shared-ring-inner-content-v2",
                    status="DRAFT_RUNTIME_REVIEW", runtimeApproved=False,
                    sourceBoard=str(source), sourceSha256=SOURCE_SHA,
                    sourceCenters=CENTERS, nativeCrop=328, alphaFloor=2, contentRadius=143, frameSourceIndex=10,
                    frameOuterRadius=158, textureSize=[side,side], cellSize=[128,128],
                    pngBytes=len(png), sha256=sha,
                    displayPolicy="One frame sprite; all consumers bind only inner art. Missing art is not replaced with HUD or another class.",
                    assets=[dict(path=str((OUTPUT/path.name).relative_to(ROOT)), role="ui-skill-icon-atlas",
                                 generator="pack_lgo_skill_icons", referenceOnly=False, sha256=sha)])
    if intake is not None:
        manifest["artworkIntake"] = intake
        manifest["byteBudget"] = budget
    text = json.dumps(manifest, ensure_ascii=False, indent=2)
    text = text[:-2] + ',\n  "parts": [\n    ' + ',\n    '.join(json.dumps(p) for p in parts) + '\n  ]\n}\n'
    # Reject invalid inputs/budget before touching the last published pair.
    output.mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory(prefix='.skill-pack-', dir=output) as temporary:
        staging = Path(temporary)
        (staging / path.name).write_bytes(png)
        (staging / 'manifest.json').write_text(text)
        os.replace(staging / path.name, path)
        os.replace(staging / 'manifest.json', output / 'manifest.json')
    return json.loads(text)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=SOURCE)
    parser.add_argument("--output", type=Path, default=OUTPUT)
    parser.add_argument("--artwork-registry", type=Path, help="Reviewed ring-free source registry keyed by existing skill icon IDs")
    args = parser.parse_args()
    result = pack(args.source, args.output, args.artwork_registry)
    print("SKILL_ICON_MODULES_PASS", result["pngBytes"], "bytes", len(result["parts"]), "modules")
