#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
import struct
import sys
import zlib
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any


EXPECTED_SCREENSHOTS = (
    "login.png",
    "character-lobby.png",
    "character-select.png",
    "enter-world.png",
    "world-hub.png",
    "near-gatekeeper-prompt.png",
    "near-training-stone-prompt.png",
    "target-dummy-state.png",
    "npc-dialogue.png",
    "session-menu.png",
)


@dataclass(frozen=True)
class PngInfo:
    width: int
    height: int
    bit_depth: int
    color_type: int
    interlace: int
    bytes_size: int
    sha256: str
    sample_unique: int | None = None
    luminance_range: int | None = None
    alpha_coverage: float | None = None
    pixel_review: str | None = None


def read_png(path: Path, *, sample_budget: int = 120_000) -> PngInfo:
    data = path.read_bytes()
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError("not a PNG")

    offset = 8
    width = height = bit_depth = color_type = interlace = None
    idat = bytearray()
    while offset < len(data):
        if offset + 8 > len(data):
            raise ValueError("truncated PNG chunk")
        length = struct.unpack(">I", data[offset : offset + 4])[0]
        chunk_type = data[offset + 4 : offset + 8]
        chunk_start = offset + 8
        chunk_end = chunk_start + length
        if chunk_end + 4 > len(data):
            raise ValueError("truncated PNG chunk data")
        chunk_data = data[chunk_start:chunk_end]
        if chunk_type == b"IHDR":
            width, height, bit_depth, color_type, _, _, interlace = struct.unpack(">IIBBBBB", chunk_data)
        elif chunk_type == b"IDAT":
            idat.extend(chunk_data)
        elif chunk_type == b"IEND":
            break
        offset = chunk_end + 4

    if width is None or height is None or bit_depth is None or color_type is None or interlace is None:
        raise ValueError("missing IHDR")

    info = PngInfo(
        width=width,
        height=height,
        bit_depth=bit_depth,
        color_type=color_type,
        interlace=interlace,
        bytes_size=len(data),
        sha256=hashlib.sha256(data).hexdigest(),
    )
    sampled = sample_png_pixels(idat, width, height, bit_depth, color_type, interlace, sample_budget=sample_budget)
    if sampled is None:
        return info
    unique, lum_range, alpha_coverage = sampled
    if unique <= 1 or lum_range <= 2:
        pixel_review = "LIKELY_BLANK_OR_FLAT"
    elif alpha_coverage is not None and alpha_coverage < 0.02:
        pixel_review = "LIKELY_TRANSPARENT_OR_EMPTY"
    else:
        pixel_review = "PIXEL_VARIATION_PRESENT"
    return PngInfo(
        width=info.width,
        height=info.height,
        bit_depth=info.bit_depth,
        color_type=info.color_type,
        interlace=info.interlace,
        bytes_size=info.bytes_size,
        sha256=info.sha256,
        sample_unique=unique,
        luminance_range=lum_range,
        alpha_coverage=alpha_coverage,
        pixel_review=pixel_review,
    )


def sample_png_pixels(
    idat: bytearray,
    width: int,
    height: int,
    bit_depth: int,
    color_type: int,
    interlace: int,
    *,
    sample_budget: int,
) -> tuple[int, int, float | None] | None:
    if bit_depth != 8 or interlace != 0:
        return None
    channels_by_color_type = {0: 1, 2: 3, 4: 2, 6: 4}
    channels = channels_by_color_type.get(color_type)
    if channels is None:
        return None
    row_bytes = width * channels
    raw = zlib.decompress(bytes(idat))
    expected = (row_bytes + 1) * height
    if len(raw) < expected:
        raise ValueError("truncated PNG image data")

    prev = bytearray(row_bytes)
    samples: set[tuple[int, ...]] = set()
    min_lum = 255
    max_lum = 0
    alpha_seen = alpha_nonzero = 0
    stride_x = max(1, width // max(1, int((sample_budget / max(1, height)) ** 0.5)))
    stride_y = max(1, height // max(1, sample_budget // max(1, width // stride_x)))

    offset = 0
    for y in range(height):
        filter_type = raw[offset]
        offset += 1
        row = bytearray(raw[offset : offset + row_bytes])
        offset += row_bytes
        unfilter(row, prev, channels, filter_type)
        if y % stride_y == 0:
            for x in range(0, width, stride_x):
                px = tuple(row[x * channels : (x + 1) * channels])
                samples.add(px)
                if channels == 1:
                    lum = px[0]
                elif channels == 2:
                    lum = px[0]
                    alpha_seen += 1
                    if px[1] > 5:
                        alpha_nonzero += 1
                else:
                    lum = (px[0] * 299 + px[1] * 587 + px[2] * 114) // 1000
                    if channels == 4:
                        alpha_seen += 1
                        if px[3] > 5:
                            alpha_nonzero += 1
                min_lum = min(min_lum, lum)
                max_lum = max(max_lum, lum)
        prev = row

    alpha_coverage = None if alpha_seen == 0 else alpha_nonzero / alpha_seen
    return len(samples), max_lum - min_lum, alpha_coverage


def unfilter(row: bytearray, prev: bytearray, bpp: int, filter_type: int) -> None:
    if filter_type == 0:
        return
    for i in range(len(row)):
        left = row[i - bpp] if i >= bpp else 0
        up = prev[i]
        up_left = prev[i - bpp] if i >= bpp else 0
        if filter_type == 1:
            row[i] = (row[i] + left) & 0xFF
        elif filter_type == 2:
            row[i] = (row[i] + up) & 0xFF
        elif filter_type == 3:
            row[i] = (row[i] + ((left + up) // 2)) & 0xFF
        elif filter_type == 4:
            row[i] = (row[i] + paeth(left, up, up_left)) & 0xFF
        else:
            raise ValueError(f"unsupported PNG filter {filter_type}")


def paeth(a: int, b: int, c: int) -> int:
    p = a + b - c
    pa = abs(p - a)
    pb = abs(p - b)
    pc = abs(p - c)
    if pa <= pb and pa <= pc:
        return a
    if pb <= pc:
        return b
    return c


def load_manifest(out_dir: Path) -> dict[str, Any]:
    manifest_path = out_dir / "visual-runtime-evidence-manifest.json"
    if not manifest_path.is_file():
        return {}
    return json.loads(manifest_path.read_text(encoding="utf-8"))


def analyze(out_dir: Path) -> tuple[dict[str, Any], int]:
    manifest = load_manifest(out_dir)
    expected_width = int(manifest.get("width", 1920))
    expected_height = int(manifest.get("height", 1080))
    manifest_checkpoints = {
        checkpoint.get("file"): checkpoint
        for checkpoint in manifest.get("checkpoints", [])
        if isinstance(checkpoint, dict) and checkpoint.get("file")
    }
    findings: list[dict[str, Any]] = []
    sha_to_files: dict[str, list[str]] = {}
    status = "EVIDENCE_CAPTURED_FOR_REVIEW"

    for file_name in EXPECTED_SCREENSHOTS:
        path = out_dir / file_name
        checkpoint = manifest_checkpoints.get(file_name, {})
        if not path.is_file():
            status = "FIX_REQUIRED"
            findings.append(
                {
                    "file": file_name,
                    "status": "FIX_REQUIRED",
                    "reason": "missing expected screenshot",
                    "reference": checkpoint.get("reference", ""),
                }
            )
            continue
        try:
            info = read_png(path)
        except Exception as exc:  # noqa: BLE001 - user-facing evidence classification
            status = "FIX_REQUIRED"
            findings.append(
                {
                    "file": file_name,
                    "status": "FIX_REQUIRED",
                    "reason": f"{type(exc).__name__}: {exc}",
                    "reference": checkpoint.get("reference", ""),
                }
            )
            continue

        sha_to_files.setdefault(info.sha256, []).append(file_name)
        item_status = "REVIEW_READY"
        reasons: list[str] = []
        if (info.width, info.height) != (expected_width, expected_height):
            item_status = "FIX_REQUIRED"
            reasons.append(f"unexpected resolution {info.width}x{info.height}; expected {expected_width}x{expected_height}")
        if info.bytes_size < 64 * 1024:
            item_status = "FIX_REQUIRED"
            reasons.append(f"suspiciously small file size {info.bytes_size} bytes")
        if info.pixel_review in {"LIKELY_BLANK_OR_FLAT", "LIKELY_TRANSPARENT_OR_EMPTY"}:
            item_status = "FIX_REQUIRED"
            reasons.append(info.pixel_review)
        if item_status == "FIX_REQUIRED":
            status = "FIX_REQUIRED"
        findings.append(
            {
                "file": file_name,
                "status": item_status,
                "reason": "; ".join(reasons) if reasons else "dimension, byte-size, and pixel-variation heuristics are ready for human review",
                "width": info.width,
                "height": info.height,
                "bytes": info.bytes_size,
                "sha256": info.sha256,
                "sample_unique": info.sample_unique,
                "luminance_range": info.luminance_range,
                "alpha_coverage": info.alpha_coverage,
                "pixel_review": info.pixel_review or "DIMENSION_ONLY",
                "reference": checkpoint.get("reference", ""),
                "expectation": checkpoint.get("expectation", ""),
            }
        )

    duplicates = {sha: files for sha, files in sha_to_files.items() if len(files) > 1}
    if duplicates:
        for files in duplicates.values():
            findings.append(
                {
                    "file": ", ".join(files),
                    "status": "REVIEW_REQUIRED_DUPLICATE_FRAME",
                    "reason": "duplicate screenshot bytes across different checkpoints; acceptable for adjacent steady-state checkpoints only after visual review",
                }
            )

    result = {
        "marker": "LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY",
        "status": status,
        "pass_claim": False,
        "non_claim": "Heuristics only detect likely evidence problems; they do not claim VISUAL_RUNTIME_PASS.",
        "output_dir": str(out_dir),
        "expected_width": expected_width,
        "expected_height": expected_height,
        "checked_at_utc": datetime.now(timezone.utc).isoformat(),
        "findings": findings,
    }
    return result, 0 if status != "FIX_REQUIRED" else 1


def write_markdown(result: dict[str, Any], path: Path) -> None:
    lines = [
        "# Visual Runtime Evidence Heuristics",
        "",
        "Marker: `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`",
        "",
        f"Status: `{result['status']}`",
        "",
        "This report reviews evidence files only. It does not claim `VISUAL_RUNTIME_PASS`.",
        "",
        "| Checkpoint | Status | Size | Resolution | Pixel heuristic | Notes |",
        "|---|---|---:|---|---|---|",
    ]
    for finding in result["findings"]:
        size = str(finding.get("bytes", ""))
        resolution = ""
        if finding.get("width") and finding.get("height"):
            resolution = f"{finding['width']}x{finding['height']}"
        pixel_review = str(finding.get("pixel_review", ""))
        notes = str(finding.get("reason", "")).replace("|", "\\|")
        lines.append(f"| `{finding['file']}` | `{finding['status']}` | {size} | {resolution} | `{pixel_review}` | {notes} |")
    lines.extend(
        [
            "",
            "Review categories still requiring human/Codex visual inspection: layout, scale, spacing, sharpness, asset quality, hierarchy, readability, and reference similarity.",
            "",
        ]
    )
    path.write_text("\n".join(lines), encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser(description="Analyze LGO visual runtime screenshot evidence with lightweight PNG heuristics.")
    parser.add_argument("output_dir", type=Path, help="Visual evidence output directory.")
    parser.add_argument("--allow-review-required", action="store_true", help="Write reports and exit 0 even when heuristics classify FIX_REQUIRED.")
    args = parser.parse_args()
    out_dir = args.output_dir
    if not out_dir.is_dir():
        print(f"ERROR: evidence output directory does not exist: {out_dir}", file=sys.stderr)
        return 2

    result, rc = analyze(out_dir)
    (out_dir / "visual-runtime-evidence-heuristics.json").write_text(
        json.dumps(result, ensure_ascii=False, indent=2) + "\n",
        encoding="utf-8",
    )
    write_markdown(result, out_dir / "visual-runtime-evidence-heuristics.md")
    print(f"LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY status={result['status']} output={out_dir}")
    print("LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED")
    if rc and args.allow_review_required:
        return 0
    return rc


if __name__ == "__main__":
    raise SystemExit(main())
