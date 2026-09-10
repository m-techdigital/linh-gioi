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


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--pack-dir", type=Path, required=True)
    parser.add_argument("--out-dir", type=Path, required=True)
    parser.add_argument("--level", type=int, default=1)
    parser.add_argument("--require-fit", action="store_true")
    args = parser.parse_args()
    report = render_pack(args.pack_dir, args.out_dir, args.level)
    print(json.dumps(report))
    if args.require_fit and (not report["referenceFitPassed"] or not report["registrationPassed"]):
        raise SystemExit(2)
