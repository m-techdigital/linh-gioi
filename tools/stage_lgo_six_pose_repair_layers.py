#!/usr/bin/env python3.12
"""Stage existing clean material exports into six-pose repair directories.

This does not invent missing poses and does not make runtime assets eligible.
It converts scattered clean source exports into a single auditable directory
shape so the repair gate reports the true remaining source gaps.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import struct
import zlib
from pathlib import Path

from inspect_lgo_source_png_inventory import inspect_png


DEFAULT_CANVAS_SIZE = [1024, 1536]
PNG_SIGNATURE = b"\x89PNG\r\n\x1a\n"


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


def read_rgba_png(path: Path) -> tuple[int, int, bytearray]:
    data = path.read_bytes()
    if not data.startswith(PNG_SIGNATURE):
        raise ValueError(f"not a PNG: {path}")
    pos = len(PNG_SIGNATURE)
    width = height = bit_depth = color_type = interlace = None
    idat = []
    while pos < len(data):
        size = struct.unpack(">I", data[pos : pos + 4])[0]
        kind = data[pos + 4 : pos + 8]
        chunk = data[pos + 8 : pos + 8 + size]
        pos += size + 12
        if kind == b"IHDR":
            width, height, bit_depth, color_type, _compression, _filter, interlace = struct.unpack(
                ">IIBBBBB", chunk
            )
        elif kind == b"IDAT":
            idat.append(chunk)
        elif kind == b"IEND":
            break
    if color_type != 6 or bit_depth != 8 or interlace != 0:
        raise ValueError(f"source must be non-interlaced RGBA/U8: {path}")
    stride = width * 4
    raw = zlib.decompress(b"".join(idat))
    offset = 0
    previous = bytearray(stride)
    pixels = bytearray()
    for _y in range(height):
        filter_type = raw[offset]
        offset += 1
        filtered = raw[offset : offset + stride]
        offset += stride
        row = bytearray(stride)
        for index, byte in enumerate(filtered):
            left = row[index - 4] if index >= 4 else 0
            up = previous[index]
            up_left = previous[index - 4] if index >= 4 else 0
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
                raise ValueError(f"unsupported PNG filter {filter_type}: {path}")
            row[index] = value & 0xFF
        pixels.extend(row)
        previous = row
    return width, height, pixels


def write_rgba_png(path: Path, width: int, height: int, pixels: bytes | bytearray) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    stride = width * 4
    rows = []
    for y in range(height):
        start = y * stride
        rows.append(bytes([0]) + bytes(pixels[start : start + stride]))

    def chunk(kind: bytes, data: bytes) -> bytes:
        payload = kind + data
        return struct.pack(">I", len(data)) + payload + struct.pack(">I", zlib.crc32(payload) & 0xFFFFFFFF)

    path.write_bytes(
        b"".join(
            [
                PNG_SIGNATURE,
                chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)),
                chunk(b"IDAT", zlib.compress(b"".join(rows), level=9)),
                chunk(b"IEND", b""),
            ]
        )
    )


def variant_b_pixels(source: bytearray) -> bytearray:
    pixels = bytearray(source)
    for index in range(0, len(pixels), 4):
        alpha = pixels[index + 3]
        if alpha == 0:
            pixels[index : index + 4] = b"\x00\x00\x00\x00"
            continue
        pixels[index] = min(255, int(pixels[index] * 1.08) + 12)
        pixels[index + 1] = max(0, int(pixels[index + 1] * 0.94) - 4)
        pixels[index + 2] = min(255, int(pixels[index + 2] * 1.04) + 8)
    return pixels


def transparent_pixels(width: int, height: int) -> bytearray:
    return bytearray(width * height * 4)


def _sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def _validate_clean_front(path: Path, source_root: Path, canvas_size: list[int]) -> dict:
    info = inspect_png(path, source_root, scan_alpha=True)
    if info["size"] != canvas_size:
        raise ValueError(f"wrong canvas {info['size']}: {path}")
    if not info["hasAlpha"]:
        raise ValueError(f"missing alpha: {path}")
    if not info.get("nonzeroAlpha"):
        raise ValueError(f"empty source: {path}")
    if info.get("nonzeroAlpha") == canvas_size[0] * canvas_size[1]:
        raise ValueError(f"full-canvas composite source: {path}")
    return info


def stage_repair_layers(
    source_root: Path | str,
    repair_plan: dict,
    source_mapping: dict,
    canvas_size: list[int] | None = None,
) -> dict:
    source_root = Path(source_root).resolve()
    canvas_size = canvas_size or DEFAULT_CANVAS_SIZE
    report = {
        "status": "PARTIAL_SOURCE_REPAIR_DRAFT_STAGED",
        "runtimePromotionAllowed": False,
        "sourceRoot": str(source_root),
        "canvasSize": canvas_size,
        "slots": {},
        "missingSources": [],
        "records": [],
        "usage": "Source staging only. Missing poses remain missing; no runtime pack or visual acceptance.",
    }
    for repair in repair_plan.get("slotRepairs") or []:
        slot = repair["slot"]
        candidate = source_root / repair["candidateDirectory"]
        staged = 0
        slot_mapping = source_mapping.get(slot, {})
        slot_records = []
        for pose in repair.get("requiredPoses") or []:
            relative = slot_mapping.get(pose)
            if not relative:
                report["missingSources"].append(f"{slot}/{pose} missing clean source")
                continue
            source_path = source_root / relative
            info = _validate_clean_front(source_path, source_root, canvas_size)
            width, height, pixels = read_rgba_png(source_path)
            if [width, height] != canvas_size:
                raise ValueError(f"wrong canvas {width}x{height}: {source_path}")
            transparent = transparent_pixels(width, height)
            for variant, variant_pixels in (("A", pixels), ("B", variant_b_pixels(pixels))):
                front = candidate / variant / pose / "front.png"
                back = candidate / variant / pose / "back.png"
                write_rgba_png(front, width, height, variant_pixels)
                write_rgba_png(back, width, height, transparent)
                slot_records.append(
                    {
                        "variant": variant,
                        "pose": pose,
                        "front": str(front),
                        "frontSha256": _sha256(front),
                        "back": str(back),
                        "backSha256": _sha256(back),
                    }
                )
            staged += 1
            report["records"].append(
                {
                    "slot": slot,
                    "pose": pose,
                    "source": str(source_path),
                    "sourceSha256": info["sha256"],
                    "sourceAlphaBBox": info["alphaBBox"],
                    "operation": "copy_A_tint_B_empty_back_no_pose_synthesis",
                }
            )
        report["slots"][slot] = {
            "candidateDirectory": repair["candidateDirectory"],
            "stagedPoseCount": staged,
            "requiredPoseCount": len(repair.get("requiredPoses") or []),
            "records": slot_records,
        }
    if not report["missingSources"]:
        report["status"] = "SOURCE_REPAIR_DRAFT_STAGED_FOR_LAYER_AUDIT"
    return report


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def write_report(report: dict, output: Path) -> None:
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("source_root", type=Path)
    parser.add_argument("--repair-plan", type=Path, required=True)
    parser.add_argument("--source-mapping", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--canvas-size", default="1024x1536")
    args = parser.parse_args()
    canvas_size = [int(part) for part in args.canvas_size.lower().split("x", 1)]
    report = stage_repair_layers(
        args.source_root,
        load_json(args.repair_plan),
        load_json(args.source_mapping),
        canvas_size,
    )
    write_report(report, args.output)
    print(
        json.dumps(
            {
                "status": report["status"],
                "runtimePromotionAllowed": report["runtimePromotionAllowed"],
                "missingSourceCount": len(report["missingSources"]),
                "stagedPoseCounts": {
                    slot: record["stagedPoseCount"] for slot, record in report["slots"].items()
                },
            },
            ensure_ascii=False,
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
