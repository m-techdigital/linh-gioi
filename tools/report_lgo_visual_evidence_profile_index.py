#!/usr/bin/env python3
from __future__ import annotations

import hashlib
import json
import struct
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Any


ROOT = Path(__file__).resolve().parents[1]
PROFILES_DIR = ROOT / "build/visual-evidence/profiles"
OUT_JSON = PROFILES_DIR / "index.json"
OUT_MD = PROFILES_DIR / "index.md"
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
PROFILE_ORDER = ("desktop", "tablet", "mobile")


def read_json(path: Path) -> dict[str, Any]:
    if not path.is_file():
        return {}
    try:
        value = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError:
        return {"_error": "invalid json"}
    return value if isinstance(value, dict) else {"_error": "json root is not an object"}


def png_summary(path: Path) -> dict[str, Any]:
    if not path.is_file():
        return {"status": "MISSING"}
    data = path.read_bytes()
    if len(data) < 24 or data[:8] != b"\x89PNG\r\n\x1a\n":
        return {"status": "INVALID_PNG", "bytes": len(data)}
    width, height = struct.unpack(">II", data[16:24])
    return {
        "status": "PRESENT",
        "width": width,
        "height": height,
        "bytes": len(data),
        "sha256": hashlib.sha256(data).hexdigest(),
        "path": str(path.relative_to(ROOT)),
    }


def profile_index(profile: str) -> dict[str, Any]:
    profile_dir = PROFILES_DIR / profile
    manifest = read_json(profile_dir / "visual-runtime-evidence-manifest.json")
    heuristics = read_json(profile_dir / "visual-runtime-evidence-heuristics.json")
    checkpoints_by_file = {
        checkpoint.get("file"): checkpoint
        for checkpoint in manifest.get("checkpoints", [])
        if isinstance(checkpoint, dict) and checkpoint.get("file")
    }
    screenshots = []
    missing = []
    for file_name in EXPECTED_SCREENSHOTS:
        item = png_summary(profile_dir / file_name)
        item["file"] = file_name
        checkpoint = checkpoints_by_file.get(file_name, {})
        item["reference"] = checkpoint.get("reference", "")
        item["expectation"] = checkpoint.get("expectation", "")
        if item["status"] != "PRESENT":
            missing.append(file_name)
        screenshots.append(item)
    return {
        "profile": profile,
        "dir": str(profile_dir.relative_to(ROOT)),
        "manifest": str((profile_dir / "visual-runtime-evidence-manifest.json").relative_to(ROOT))
        if (profile_dir / "visual-runtime-evidence-manifest.json").is_file()
        else None,
        "heuristics": str((profile_dir / "visual-runtime-evidence-heuristics.json").relative_to(ROOT))
        if (profile_dir / "visual-runtime-evidence-heuristics.json").is_file()
        else None,
        "heuristics_status": heuristics.get("status"),
        "resolution": {
            "width": manifest.get("width"),
            "height": manifest.get("height"),
        },
        "missing": missing,
        "screenshots": screenshots,
    }


def build_index() -> dict[str, Any]:
    profiles = [profile_index(profile) for profile in PROFILE_ORDER]
    any_missing = any(profile["missing"] for profile in profiles)
    return {
        "marker": "LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY",
        "status": "FIX_REQUIRED" if any_missing else "INDEX_READY",
        "pass_claim": False,
        "non_claim": "This index only maps captured evidence; it does not claim VISUAL_RUNTIME_PASS.",
        "created_at_utc": datetime.now(timezone.utc).isoformat(),
        "profiles": profiles,
    }


def write_markdown(index: dict[str, Any]) -> None:
    lines = [
        "# LGO Visual Evidence Profile Index",
        "",
        "Marker: `LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY`",
        "",
        f"Status: `{index['status']}`",
        "",
        "This index maps evidence files only. It does not claim `VISUAL_RUNTIME_PASS`.",
        "",
    ]
    for profile in index["profiles"]:
        resolution = profile["resolution"]
        width = resolution.get("width") or "?"
        height = resolution.get("height") or "?"
        lines.extend(
            [
                f"## {profile['profile']}",
                "",
                f"- Directory: `{profile['dir']}`",
                f"- Manifest: `{profile['manifest'] or 'MISSING'}`",
                f"- Heuristics: `{profile['heuristics'] or 'MISSING'}`",
                f"- Heuristics status: `{profile['heuristics_status'] or 'MISSING'}`",
                f"- Resolution: `{width}x{height}`",
                "",
                "| Checkpoint | Status | File | Size | Resolution | Reference |",
                "|---|---|---|---:|---|---|",
            ]
        )
        for screenshot in profile["screenshots"]:
            rel_path = screenshot.get("path", "")
            size = screenshot.get("bytes", "")
            png_width = screenshot.get("width", "")
            png_height = screenshot.get("height", "")
            png_resolution = f"{png_width}x{png_height}" if png_width and png_height else ""
            reference = str(screenshot.get("reference", "")).replace("|", "\\|")
            lines.append(
                f"| `{screenshot['file']}` | `{screenshot['status']}` | `{rel_path}` | {size} | {png_resolution} | {reference} |"
            )
        lines.append("")
    OUT_MD.write_text("\n".join(lines), encoding="utf-8")


def main() -> int:
    PROFILES_DIR.mkdir(parents=True, exist_ok=True)
    index = build_index()
    OUT_JSON.write_text(json.dumps(index, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    write_markdown(index)
    print(f"LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY status={index['status']} output={OUT_JSON}")
    print("LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED")
    return 0 if index["status"] == "INDEX_READY" else 1


if __name__ == "__main__":
    raise SystemExit(main())
