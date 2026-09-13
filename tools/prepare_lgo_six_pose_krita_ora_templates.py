#!/usr/bin/env python3.12
"""Create Krita/OpenRaster authoring templates from a six-pose workspace."""
from __future__ import annotations

import argparse
import json
import zipfile
from datetime import datetime, timezone
from pathlib import Path
from xml.sax.saxutils import escape


def _read_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def _layer_xml(name: str, src: str, opacity: float = 1.0, visible: bool = True) -> str:
    visibility = "visible" if visible else "hidden"
    return f'<layer name="{escape(name)}" src="{escape(src)}" opacity="{opacity:.3f}" visibility="{visibility}" />'


def _target_name(target: dict) -> str:
    return f"{target['slot']}__{target['pose']}"


def _template_layers(target: dict) -> list[tuple[str, Path, str, float, bool]]:
    layers: list[tuple[str, Path, str, float, bool]] = []
    for template in target.get("templates") or []:
        variant = template["variant"]
        component = template["component"]
        layers.append(
            (
                f"AUTHOR - {variant} {component}",
                Path(template["path"]),
                f"data/{variant}-{component}-author.png",
                1.0,
                True,
            )
        )
    source_ref = target.get("sourceReference") or {}
    if source_ref.get("copiedTo"):
        layers.append(("REFERENCE - current source", Path(source_ref["copiedTo"]), "data/source-reference.png", 0.65, True))
    layers.append(("GUIDE - overlay on body", Path(target["guideOverlay"]), "data/guide-overlay-on-body.png", 0.85, True))
    layers.append(("REFERENCE - body", Path(target["bodyReference"]), "data/body-reference.png", 1.0, True))
    return layers


def _write_ora(path: Path, target: dict, canvas_size: list[int]) -> dict:
    width, height = canvas_size
    path.parent.mkdir(parents=True, exist_ok=True)
    layers = _template_layers(target)
    stack_xml = "\n".join(
        [
            f'<image w="{width}" h="{height}" name="{escape(_target_name(target))}">',
            '  <stack name="root">',
            *[f"    {_layer_xml(name, src, opacity, visible)}" for name, _source, src, opacity, visible in layers],
            "  </stack>",
            "</image>",
        ]
    )
    with zipfile.ZipFile(path, "w", compression=zipfile.ZIP_DEFLATED) as archive:
        archive.writestr(zipfile.ZipInfo("mimetype"), b"image/openraster", compress_type=zipfile.ZIP_STORED)
        archive.writestr("stack.xml", stack_xml.encode("utf-8"))
        for name, source, src, _opacity, _visible in layers:
            if not source.exists():
                raise FileNotFoundError(f"missing ORA layer source for {name}: {source}")
            archive.write(source, src)
    return {
        "slot": target["slot"],
        "pose": target["pose"],
        "path": str(path),
        "layerCount": len(layers),
        "authorLayerCount": sum(1 for name, *_rest in layers if name.startswith("AUTHOR -")),
        "usage": "Open in Krita for authoring. AUTHOR layers are templates and are expected to be empty until an artist/source process edits them.",
    }


def prepare_ora_templates(manifest_path: Path | str, output_root: Path | str) -> dict:
    manifest_path = Path(manifest_path).resolve()
    output_root = Path(output_root).resolve()
    manifest = _read_json(manifest_path)
    canvas_size = manifest.get("canvasSize") or [1024, 1536]
    templates = []
    for target in manifest.get("targets") or []:
        ora_path = output_root / f"{_target_name(target)}.ora"
        templates.append(_write_ora(ora_path, target, canvas_size))
    report = {
        "status": "KRITA_ORA_AUTHORING_TEMPLATES_READY",
        "runtimePromotionAllowed": False,
        "visualAcceptance": False,
        "manifest": str(manifest_path),
        "outputRoot": str(output_root),
        "canvasSize": canvas_size,
        "templateCount": len(templates),
        "templates": templates,
        "createdAt": datetime.now(timezone.utc).isoformat(),
        "usage": "Authoring templates only. Do not pack ORA/template files and do not run non-empty AUTHOR-layer acceptance until edited source layers exist.",
    }
    output_root.mkdir(parents=True, exist_ok=True)
    (output_root / "krita-ora-template-manifest.json").write_text(
        json.dumps(report, ensure_ascii=False, indent=2) + "\n",
        encoding="utf-8",
    )
    (output_root / "README.md").write_text(render_readme(report), encoding="utf-8")
    return report


def render_readme(report: dict) -> str:
    lines = [
        "# Krita ORA authoring templates",
        "",
        "These ORA files are authoring templates only. Do not pack them as runtime source.",
        "",
        f"Status: `{report['status']}`",
        f"Template count: `{report['templateCount']}`",
        f"Runtime promotion allowed: `{str(report['runtimePromotionAllowed']).lower()}`",
        "",
        "Expected use:",
        "",
        "1. Open a target `.ora` in Krita.",
        "2. Draw or paste clean source art into the `AUTHOR - ...` layers.",
        "3. Export each edited AUTHOR layer as RGBA PNG to the matching `destinationExports` path in the workspace manifest.",
        "4. Run repair-layer audit and source-board review before any Player pack.",
        "",
        "Templates:",
        "",
    ]
    for template in report.get("templates") or []:
        lines.append(f"- `{template['slot']}` / `{template['pose']}` → `{Path(template['path']).name}`")
    lines.append("")
    return "\n".join(lines)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--manifest", type=Path, required=True)
    parser.add_argument("--output-root", type=Path, required=True)
    args = parser.parse_args()
    report = prepare_ora_templates(args.manifest, args.output_root)
    print(
        json.dumps(
            {
                "status": report["status"],
                "runtimePromotionAllowed": report["runtimePromotionAllowed"],
                "templateCount": report["templateCount"],
                "outputRoot": report["outputRoot"],
            },
            ensure_ascii=False,
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
