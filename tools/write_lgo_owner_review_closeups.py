#!/usr/bin/env python3.12
"""Write per-class owner-review close-up contact sheets from Player capture frames.

The sheets are evidence for human visual review only. They do not promote any
class to owner/production approval.
"""
from __future__ import annotations

import argparse
import json
import subprocess
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

CLASS_RUNTIME_DIRS = {
    "kiem": "build/kiem-semantic-v3-runtime-v1/pc",
    "phap": "build/phap-canonical-v2-runtime-v1/pc",
    "co": "build/co-semantic-v3-runtime-v3/pc",
    "linh": "build/linh-semantic-v3-runtime-v2/pc",
}

FRAME_MATRIX = [
    ("male idle", "01-male-idle.png"),
    ("male Lv10", "03-male-level-10.png"),
    ("male run0", "10-male-run-phase-0.png"),
    ("male jump", "19-male-jump.png"),
    ("male off weapon", "49-male-off-main_weapon.png"),
    ("male off outer", "52-male-off-outer_tunic.png"),
    ("female idle", "90-female-idle.png"),
    ("female Lv10", "92-female-level-10.png"),
    ("female run0", "99-female-run-phase-0.png"),
    ("female jump", "108-female-jump.png"),
    ("female off weapon", "138-female-off-main_weapon.png"),
    ("female off outer", "141-female-off-outer_tunic.png"),
]

SWIFT_SOURCE = r'''
import AppKit
import Foundation

struct Frame: Decodable { let label: String; let path: String }
struct Input: Decodable { let title: String; let output: String; let frames: [Frame] }

let inputURL = URL(fileURLWithPath: CommandLine.arguments[1])
let input = try JSONDecoder().decode(Input.self, from: Data(contentsOf: inputURL))
let columns = 6
let cellW = 340
let cellH = 320
let labelH = 26
let titleH = 44
let width = columns * cellW
let rows = Int(ceil(Double(input.frames.count) / Double(columns)))
let height = titleH + rows * cellH
let canvas = NSImage(size: NSSize(width: width, height: height))
canvas.lockFocus()
NSColor(calibratedRed: 0.07, green: 0.08, blue: 0.10, alpha: 1).setFill()
NSRect(x: 0, y: 0, width: width, height: height).fill()
let titleAttrs: [NSAttributedString.Key: Any] = [
    .font: NSFont.boldSystemFont(ofSize: 22),
    .foregroundColor: NSColor(calibratedRed: 0.95, green: 0.82, blue: 0.44, alpha: 1)
]
(input.title as NSString).draw(in: NSRect(x: 12, y: height - titleH + 10, width: width - 24, height: 28), withAttributes: titleAttrs)
let labelAttrs: [NSAttributedString.Key: Any] = [
    .font: NSFont.systemFont(ofSize: 13),
    .foregroundColor: NSColor.white
]
for (index, frame) in input.frames.enumerated() {
    let col = index % columns
    let row = index / columns
    let x = col * cellW
    let yTop = titleH + row * cellH
    let drawY = height - yTop - cellH
    (frame.label as NSString).draw(in: NSRect(x: x + 8, y: drawY + cellH - labelH + 5, width: cellW - 16, height: labelH), withAttributes: labelAttrs)
    guard let image = NSImage(contentsOfFile: frame.path),
          let cg = image.cgImage(forProposedRect: nil, context: nil, hints: nil) else {
        NSColor.red.setFill(); NSRect(x: x + 8, y: drawY + 8, width: cellW - 16, height: cellH - labelH - 16).fill(); continue
    }
    let cropW = min(280, cg.width)
    let cropH = min(460, cg.height)
    let cropX = max(0, (cg.width - cropW) / 2)
    let cropY = max(0, (cg.height - cropH) / 2)
    let cropRect = CGRect(x: cropX, y: cropY, width: cropW, height: cropH)
    let cropped = cg.cropping(to: cropRect) ?? cg
    let croppedImage = NSImage(cgImage: cropped, size: NSSize(width: cropped.width, height: cropped.height))
    let target = NSRect(x: x + 8, y: drawY + 8, width: cellW - 16, height: cellH - labelH - 20)
    croppedImage.draw(in: target, from: NSRect(x: 0, y: 0, width: cropped.width, height: cropped.height), operation: .sourceOver, fraction: 1)
    NSColor(calibratedRed: 0.52, green: 0.43, blue: 0.22, alpha: 1).setStroke()
    NSBezierPath(rect: target).stroke()
}
canvas.unlockFocus()
let rep = NSBitmapImageRep(data: canvas.tiffRepresentation!)!
let data = rep.representation(using: .jpeg, properties: [.compressionFactor: 0.92])!
try FileManager.default.createDirectory(at: URL(fileURLWithPath: input.output).deletingLastPathComponent(), withIntermediateDirectories: true)
try data.write(to: URL(fileURLWithPath: input.output))
'''


def write_sheet(class_id: str, runtime_dir: Path, output: Path) -> None:
    frames = []
    missing = []
    for label, filename in FRAME_MATRIX:
        path = runtime_dir / filename
        if not path.is_file():
            missing.append(str(path))
        frames.append({"label": label, "path": str(path.resolve())})
    if missing:
        raise FileNotFoundError("Missing close-up source frames: " + ", ".join(missing))
    payload = {"title": f"{class_id} owner-review close-up · technical visual evidence", "output": str(output.resolve()), "frames": frames}
    with tempfile.TemporaryDirectory(prefix="lgo-closeup-") as td:
        td_path = Path(td)
        swift = td_path / "make_sheet.swift"
        spec = td_path / "input.json"
        swift.write_text(SWIFT_SOURCE, encoding="utf-8")
        spec.write_text(json.dumps(payload, ensure_ascii=False), encoding="utf-8")
        subprocess.run(["/usr/bin/swift", str(swift), str(spec)], check=True)


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output-dir", type=Path, default=ROOT / "build/source-pose-catalog-audit-v2")
    parser.add_argument("--class-id", choices=sorted(CLASS_RUNTIME_DIRS), action="append")
    args = parser.parse_args(argv)
    class_ids = args.class_id or sorted(CLASS_RUNTIME_DIRS)
    args.output_dir.mkdir(parents=True, exist_ok=True)
    for class_id in class_ids:
        runtime_dir = ROOT / CLASS_RUNTIME_DIRS[class_id]
        output = args.output_dir / f"{class_id}-owner-review-closeup.jpg"
        write_sheet(class_id, runtime_dir, output)
        print(f"LGO_OWNER_REVIEW_CLOSEUP_WRITTEN class={class_id} output={output} bytes={output.stat().st_size}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
