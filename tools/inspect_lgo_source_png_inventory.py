#!/usr/bin/env python3.12
"""Inspect source PNG directories for repeatable repair-batch evidence."""
from __future__ import annotations

import argparse
import hashlib
import json
import struct
import zlib
from pathlib import Path


PNG_SIGNATURE = b"\x89PNG\r\n\x1a\n"


def _read_png_chunks(path: Path) -> tuple[dict, bytes]:
    data = path.read_bytes()
    if not data.startswith(PNG_SIGNATURE):
        raise ValueError("not a PNG")
    pos = len(PNG_SIGNATURE)
    info: dict = {"idat": []}
    while pos < len(data):
        size = struct.unpack(">I", data[pos : pos + 4])[0]
        kind = data[pos + 4 : pos + 8]
        chunk = data[pos + 8 : pos + 8 + size]
        pos += size + 12
        if kind == b"IHDR":
            width, height, bit_depth, color_type, compression, filter_method, interlace = struct.unpack(
                ">IIBBBBB", chunk
            )
            info.update(
                {
                    "width": width,
                    "height": height,
                    "bitDepth": bit_depth,
                    "colorType": color_type,
                    "compression": compression,
                    "filterMethod": filter_method,
                    "interlace": interlace,
                }
            )
        elif kind == b"IDAT":
            info["idat"].append(chunk)
        elif kind == b"IEND":
            break
    return info, data


def _paeth(left: int, up: int, up_left: int) -> int:
    p = left + up - up_left
    pa = abs(p - left)
    pb = abs(p - up)
    pc = abs(p - up_left)
    if pa <= pb and pa <= pc:
        return left
    if pb <= pc:
        return up
    return up_left


def _scan_rgba_alpha(info: dict) -> tuple[int | None, list[int] | None]:
    if info.get("colorType") != 6 or info.get("bitDepth") != 8 or info.get("interlace") != 0:
        return None, None
    width = info["width"]
    height = info["height"]
    bpp = 4
    stride = width * bpp
    raw = zlib.decompress(b"".join(info["idat"]))
    previous = bytearray(stride)
    offset = 0
    nonzero = 0
    min_x, min_y = width, height
    max_x, max_y = -1, -1
    for y in range(height):
        filter_type = raw[offset]
        offset += 1
        filtered = raw[offset : offset + stride]
        offset += stride
        row = bytearray(stride)
        for index, byte in enumerate(filtered):
            left = row[index - bpp] if index >= bpp else 0
            up = previous[index]
            up_left = previous[index - bpp] if index >= bpp else 0
            if filter_type == 0:
                value = byte
            elif filter_type == 1:
                value = byte + left
            elif filter_type == 2:
                value = byte + up
            elif filter_type == 3:
                value = byte + ((left + up) // 2)
            elif filter_type == 4:
                value = byte + _paeth(left, up, up_left)
            else:
                raise ValueError(f"unsupported PNG filter {filter_type}")
            row[index] = value & 0xFF
        for x in range(width):
            if row[x * bpp + 3] != 0:
                nonzero += 1
                min_x = min(min_x, x)
                max_x = max(max_x, x)
                min_y = min(min_y, y)
                max_y = max(max_y, y)
        previous = row
    bbox = None if nonzero == 0 else [min_x, min_y, max_x, max_y]
    return nonzero, bbox


def inspect_png(path: Path, source_root: Path, scan_alpha: bool) -> dict:
    info, data = _read_png_chunks(path)
    record = {
        "path": str(path.relative_to(source_root)),
        "size": [info.get("width"), info.get("height")],
        "bitDepth": info.get("bitDepth"),
        "colorType": info.get("colorType"),
        "hasAlpha": info.get("colorType") in (4, 6),
        "sha256": hashlib.sha256(data).hexdigest(),
    }
    if scan_alpha:
        nonzero, bbox = _scan_rgba_alpha(info)
        record["nonzeroAlpha"] = nonzero
        record["alphaBBox"] = bbox
    return record


def inspect_inventory(source_root: Path | str, directories: list[str], scan_alpha: bool = False) -> dict:
    source_root = Path(source_root).resolve()
    report = {
        "status": "SOURCE_PNG_INVENTORY_READY",
        "sourceRoot": str(source_root),
        "scanAlpha": scan_alpha,
        "directories": {},
    }
    for directory in directories:
        root = source_root / directory
        files = []
        if root.exists():
            for path in sorted(root.rglob("*.png")):
                files.append(inspect_png(path, source_root, scan_alpha))
        report["directories"][directory] = {
            "exists": root.exists(),
            "pngCount": len(files),
            "files": files,
        }
    return report


def write_inventory(
    source_root: Path | str,
    directories: list[str],
    output: Path | str,
    scan_alpha: bool = False,
) -> dict:
    report = inspect_inventory(source_root, directories, scan_alpha)
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return report


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("source_root", type=Path)
    parser.add_argument("--dir", action="append", dest="directories", required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--scan-alpha", action="store_true")
    args = parser.parse_args()
    report = write_inventory(args.source_root, args.directories, args.output, args.scan_alpha)
    print(
        json.dumps(
            {
                "status": report["status"],
                "directories": {
                    key: {"exists": value["exists"], "pngCount": value["pngCount"]}
                    for key, value in report["directories"].items()
                },
            },
            ensure_ascii=False,
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
