"""Registered UI icon decomposition: one ring master, independent inner art."""
from __future__ import annotations
import argparse
import hashlib
import json
import math
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


def pack(source: Path = SOURCE, output: Path = OUTPUT) -> dict:
    if hashlib.sha256(source.read_bytes()).hexdigest() != SOURCE_SHA:
        raise ValueError("Skill source hash mismatch: " + str(source))
    with Image.open(source) as image:
        if image.size != (1448, 1086):
            raise ValueError("Skill source must use its registered 4x3 grid of 362px cells")
        board = image.convert("RGBA")
    atlas = Image.new("RGBA", (512, 512))
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
        x, top = index % 4 * 128, index // 4 * 128
        atlas.alpha_composite(tile, (x+4, top+4))
        parts.append(dict(id=icon_id, x=x, y=512-top-128, w=128, h=128,
                          role="shared-frame" if icon_id == "frame" else "inner-symbol"))
    output.mkdir(parents=True, exist_ok=True)
    path = output / "map01a-skill-icons.png"
    atlas.save(path, optimize=True)
    if path.stat().st_size > 400000:
        raise ValueError("Skill icon atlas exceeds 400000 bytes")
    sha = hashlib.sha256(path.read_bytes()).hexdigest()
    manifest = dict(id="map01a-skill-icons-v1", revision="shared-ring-inner-content-v2",
                    status="DRAFT_RUNTIME_REVIEW", runtimeApproved=False,
                    sourceBoard=str(source), sourceSha256=SOURCE_SHA,
                    sourceCenters=CENTERS, nativeCrop=328, alphaFloor=2, contentRadius=143, frameSourceIndex=10,
                    frameOuterRadius=158, textureSize=[512,512], cellSize=[128,128],
                    pngBytes=path.stat().st_size, sha256=sha,
                    displayPolicy="One frame sprite; all consumers bind only inner art. Missing art is not replaced with HUD or another class.",
                    assets=[dict(path=str((OUTPUT/path.name).relative_to(ROOT)), role="ui-skill-icon-atlas",
                                 generator="pack_lgo_skill_icons", referenceOnly=False, sha256=sha)])
    text = json.dumps(manifest, ensure_ascii=False, indent=2)
    text = text[:-2] + ',\n  "parts": [\n    ' + ',\n    '.join(json.dumps(p) for p in parts) + '\n  ]\n}\n'
    (output / "manifest.json").write_text(text)
    return json.loads(text)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=SOURCE)
    parser.add_argument("--output", type=Path, default=OUTPUT)
    args = parser.parse_args()
    result = pack(args.source, args.output)
    print("SKILL_ICON_MODULES_PASS", result["pngBytes"], "bytes", len(result["parts"]), "modules")
