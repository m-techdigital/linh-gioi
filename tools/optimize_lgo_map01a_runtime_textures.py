#!/usr/bin/env python3
"""Create deterministic, size-bounded Map 01A runtime PNGs in one batch."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image


TEXTURES = {
    "far-background.png": 256,
    "props-atlas.png": 256,
    "sheet-props.png": 192,
    "modules-atlas.png": 256,
    "npcs-atlas.png": 256,
    "landmarks-atlas.png": 256,
    "combat-atlas.png": 256,
}


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def optimize(source: Path, destination: Path, colors: int) -> dict[str, object]:
    with Image.open(source) as image:
        original_mode = image.mode
        rgba = image.convert("RGBA")
        indexed = rgba.quantize(
            colors=colors,
            method=Image.Quantize.FASTOCTREE,
            dither=Image.Dither.NONE,
        )
        destination.parent.mkdir(parents=True, exist_ok=True)
        indexed.save(destination, format="PNG", optimize=True, compress_level=9)
        with Image.open(destination) as check:
            if check.size != image.size:
                raise RuntimeError(f"Dimension changed for {source.name}: {image.size} -> {check.size}")
            source_alpha = rgba.getchannel("A").getextrema()
            output_alpha = check.convert("RGBA").getchannel("A").getextrema()
            if source_alpha[0] == 0 and output_alpha[0] != 0:
                raise RuntimeError(f"Transparency was lost for {source.name}")
            if source_alpha[1] == 255 and output_alpha[1] < 250:
                raise RuntimeError(f"Opaque pixels became visibly translucent for {source.name}")
    return {
        "name": source.name,
        "dimensions": list(rgba.size),
        "sourceMode": original_mode,
        "runtimeMode": "P",
        "paletteColors": colors,
        "beforeBytes": source.stat().st_size,
        "afterBytes": destination.stat().st_size,
        "sha256": sha256(destination),
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--input-dir", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    args = parser.parse_args()
    records = []
    for filename, colors in TEXTURES.items():
        source = args.input_dir / filename
        if not source.is_file():
            raise FileNotFoundError(source)
        records.append(optimize(source, args.output_dir / filename, colors))
    report = {
        "id": "map01a-runtime-texture-optimization-v1",
        "textures": records,
        "totalBeforeBytes": sum(int(record["beforeBytes"]) for record in records),
        "totalAfterBytes": sum(int(record["afterBytes"]) for record in records),
    }
    report["savedBytes"] = report["totalBeforeBytes"] - report["totalAfterBytes"]
    (args.output_dir / "optimization-report.json").write_text(
        json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    print(json.dumps(report, ensure_ascii=False, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
