#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
STYLE_KEYS = (
    "directStyleAssignments",
    "numericStyleAssignments",
    "semanticPaletteDeclarations",
    "skinPartialMaxLoc",
    "skinPartialMaxMethods",
)
HUD_RECTS = (
    "gameplayPlayerStatus",
    "gameplayRightInfo",
    "gameplayCombat",
    "gameplayContext",
    "gameplaySecondaryNav",
    "gameplayTouchPad",
    "gameplayDialogue",
)


def file_sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()
def read_json(path: Path, errors: list[str], label: str):
    if not path.is_file():
        errors.append(f"{label} missing: {path}")
        return {}
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception as exc:
        errors.append(f"{label} invalid JSON: {path}: {exc}")
        return {}


def resolve_artifact(root: Path, rel: str, errors: list[str], label: str) -> Path:
    if not isinstance(rel, str) or not rel:
        errors.append(label + " path is required")
        return root / "__missing__"
    candidate = (root / rel).resolve()
    try:
        candidate.relative_to(root.resolve())
    except ValueError:
        errors.append(label + " escapes bundle root: " + rel)
    return candidate


def current_git_head(root: Path) -> str:
    result = subprocess.run(
        ["git", "rev-parse", "HEAD"], cwd=root, text=True,
        stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        return ""
    return result.stdout.strip()


def in_range(value, bounds) -> bool:
    return (isinstance(bounds, list) and len(bounds) == 2
            and float(bounds[0]) <= float(value) <= float(bounds[1]))


def rect_inside(rect, width: float, height: float) -> bool:
    if not isinstance(rect, dict):
        return False
    try:
        x, y = float(rect["x"]), float(rect["y"])
        w, h = float(rect["width"]), float(rect["height"])
    except (KeyError, TypeError, ValueError):
        return False
    return x >= -0.01 and y >= -0.01 and w >= 0 and h >= 0 and x + w <= width + 0.01 and y + h <= height + 0.01
def rects_intersect(a, b) -> bool:
    if not isinstance(a, dict) or not isinstance(b, dict):
        return False
    if min(float(a.get("width", 0)), float(a.get("height", 0)),
           float(b.get("width", 0)), float(b.get("height", 0))) <= 0:
        return False
    return not (
        float(a["x"]) + float(a["width"]) <= float(b["x"])
        or float(b["x"]) + float(b["width"]) <= float(a["x"])
        or float(a["y"]) + float(a["height"]) <= float(b["y"])
        or float(b["y"]) + float(b["height"]) <= float(a["y"])
    )


def validate_matrix_files(matrix, root: Path, owner_root: Path | None, errors: list[str]) -> None:
    surfaces = matrix.get("surfaces")
    if not isinstance(surfaces, list) or not surfaces:
        errors.append("authority matrix surfaces missing")
        return
    for surface in surfaces:
        sid = surface.get("id", "<unknown>")
        authority = surface.get("designAuthority") or {}
        kind = authority.get("kind")
        rel = authority.get("relativePath")
        expected = authority.get("sha256")
        if kind == "owner":
            if owner_root is None:
                errors.append(f"{sid}: owner root is required")
                continue
            path = owner_root / str(rel)
        elif kind == "approved-proposal":
            path = root / str(rel)
        elif kind == "runtime-only":
            path = root / str(rel)
        else:
            errors.append(f"{sid}: invalid authority kind {kind}")
            continue
        if not path.is_file():
            errors.append(f"{sid}: authority file missing: {path}")
        elif not expected or file_sha256(path) != expected:
            errors.append(f"{sid}: authority sha256 mismatch")
        for rel_owner in surface.get("codeOwners") or []:
            if not (root / rel_owner).is_file():
                errors.append(f"{sid}: missing code owner {rel_owner}")
        for rel_evidence in surface.get("runtimeEvidence") or []:
            if not (root / rel_evidence).is_file():
                errors.append(f"{sid}: missing runtime evidence {rel_evidence}")
def validate_artifact_hashes(bundle, root: Path, errors: list[str]) -> None:
    artifacts = bundle.get("artifacts")
    if not isinstance(artifacts, dict) or not artifacts:
        errors.append("bundle.artifacts must be a non-empty object")
        return
    for rel, expected in artifacts.items():
        path = resolve_artifact(root, rel, errors, "artifact")
        if not path.is_file():
            errors.append("artifact missing: " + rel)
        elif file_sha256(path) != expected:
            errors.append("artifact sha256 mismatch: " + rel)


def validate_style(bundle, budget, errors: list[str], root: Path) -> None:
    metrics = bundle.get("styleMetrics")
    if not isinstance(metrics, dict):
        errors.append("bundle.styleMetrics missing")
        return
    source = root / "client/Unity/Assets/Game/UI/Runtime"
    if source.is_dir():
        from report_lgo_product_ui_style_debt import scan
        actual = scan(source)["metrics"]
        for key in STYLE_KEYS:
            if int(metrics.get(key, -1)) != int(actual.get(key, -2)):
                errors.append(
                    f"style metric snapshot mismatch {key}: bundle={metrics.get(key)} current={actual.get(key)}")
    for key in STYLE_KEYS:
        if key not in budget:
            errors.append("style budget missing " + key)
            continue
        if key not in metrics:
            errors.append("style metrics missing " + key)
            continue
        if int(metrics[key]) > int(budget[key]):
            errors.append(f"style budget exceeded {key}: actual={metrics[key]} budget={budget[key]}")


def validate_surface_review(bundle, config, review, root: Path, errors: list[str]) -> None:
    if review.get("status") != "PASS":
        errors.append("visual review status must be PASS")
    if not str(review.get("reviewedBy", "")).strip():
        errors.append("visual review reviewer is required")
    reviewed = review.get("artifacts")
    if not isinstance(reviewed, dict):
        errors.append("visual review artifacts missing")
        return
    evidence_root = str(bundle.get("evidenceRoot", "")).rstrip("/")
    required_profiles = config.get("requiredProfiles") or []
    for sid, spec in (config.get("surfaceEvidence") or {}).items():
        if isinstance(spec, str):
            path_rel = spec
            profiles = required_profiles
        else:
            path_rel = spec.get("path")
            profiles = spec.get("profiles") or required_profiles
        for profile in profiles:
            rel = f"{evidence_root}/{profile}/{path_rel}"
            path = resolve_artifact(root, rel, errors, f"{sid}:{profile}")
            if not path.is_file():
                errors.append(f"{sid}:{profile}: screenshot missing")
                continue
            actual = file_sha256(path)
            if reviewed.get(f"{sid}:{profile}") != actual:
                errors.append(f"{sid}:{profile}: review hash mismatch")
def validate_profile_geometry(profile: str, bundle, config, root: Path, errors: list[str]) -> str:
    evidence_root = str(bundle.get("evidenceRoot", "")).rstrip("/")
    hub_rel = f"{evidence_root}/{profile}/character-hub/manifest.json"
    quest_rel = f"{evidence_root}/{profile}/quest/manifest.json"
    hub = read_json(resolve_artifact(root, hub_rel, errors, profile + " hub manifest"), errors, profile + " hub manifest")
    quest = read_json(resolve_artifact(root, quest_rel, errors, profile + " quest manifest"), errors, profile + " quest manifest")
    if hub.get("status") != "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED":
        errors.append(profile + ": character-hub capture status invalid")
    if quest.get("status") != "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED":
        errors.append(profile + ": quest capture status invalid")
    expected_viewport = (config.get("viewports") or {}).get(profile)
    if isinstance(expected_viewport, list) and len(expected_viewport) == 2:
        for label, document in (("character-hub", hub), ("quest", quest)):
            if [document.get("width"), document.get("height")] != expected_viewport:
                errors.append(
                    f"{profile}: {label} viewport mismatch "
                    f"actual={[document.get('width'), document.get('height')]} expected={expected_viewport}")
    ui = hub.get("uiMetrics") or {}
    qui = quest.get("uiMetrics") or {}
    authority = str(qui.get("evidenceAuthority") or ui.get("evidenceAuthority") or "")
    if ui.get("evidenceAuthority") != qui.get("evidenceAuthority"):
        errors.append(profile + ": evidence authority mismatch between hub and quest")

    geometry = config.get("geometry") or {}
    aspect = float(geometry.get("hubAspect", 0))
    tolerance = float(geometry.get("hubAspectTolerance", 0))
    shell_w = float(ui.get("characterHubShellWidth", 0))
    shell_h = float(ui.get("characterHubShellHeight", 0))
    if shell_h <= 0 or abs(shell_w / shell_h - aspect) > tolerance:
        errors.append(profile + ": Character Hub canonical aspect drift")
    height_bounds = (geometry.get("hubHeightRatio") or {}).get(profile)
    if not in_range(ui.get("characterHubShellScreenHeightRatio", -1), height_bounds):
        errors.append(profile + ": Character Hub height occupancy outside gate")
    scale_bounds = (geometry.get("presentationScale") or {}).get(profile)
    if not in_range(ui.get("presentationScale", -1), scale_bounds):
        errors.append(profile + ": presentation scale outside gate")
    safe_width = float(qui.get("safePanelWidth", 0))
    safe_height = float(qui.get("safePanelHeight", 0))
    panel_width = float(qui.get("panelWidth", 0))
    panel_height = float(qui.get("panelHeight", 0))
    safe_x, safe_y = float(qui.get("safePanelX", 0)), float(qui.get("safePanelY", 0))
    if safe_width <= 0 or safe_height <= 0 or safe_x < 0 or safe_y < 0 or safe_x + safe_width > panel_width + .01 or safe_y + safe_height > panel_height + .01:
        errors.append(profile + ": safe panel outside panel bounds")
    for key in HUD_RECTS:
        rect = qui.get(key)
        if not rect_inside(rect, safe_width, safe_height):
            errors.append(profile + ": " + key + " outside safe panel")
    if rects_intersect(qui.get("gameplayTouchPad"), qui.get("gameplayCombat")):
        errors.append(profile + ": touch pad intersects combat dock")
    if rects_intersect(qui.get("gameplayTouchPad"), qui.get("gameplayContext")):
        errors.append(profile + ": touch pad intersects context action")

    minimum_touch = float(qui.get("minimumTouchTargetPanelUnits", 0))
    if minimum_touch < float(config.get("minimumTouchTargetPanelUnits", 44)):
        errors.append(profile + ": minimum touch target token regressed")

    world = quest.get("worldMetrics") or {}
    if not in_range(world.get("actorScreenHeightRatio", -1), geometry.get("actorHeightRatio")):
        errors.append(profile + ": actor screen-height ratio outside gate")
    if not in_range(world.get("npcScreenHeightRatio", -1), geometry.get("npcHeightRatio")):
        errors.append(profile + ": NPC screen-height ratio outside gate")
    if float(world.get("backgroundCoverageScale", 0)) <= 0:
        errors.append(profile + ": background coverage is not positive")
    return authority


def validate_bundle(bundle, root=ROOT, owner_root=None, current_head=None, mode="foundation"):
    root = Path(root)
    owner_root = Path(owner_root) if owner_root is not None else None
    errors: list[str] = []
    if bundle.get("version") != 1:
        errors.append("bundle.version must be 1")
    if mode not in {"foundation", "closure"}:
        errors.append("mode must be foundation or closure")
    expected_head = str(bundle.get("sourceHead", ""))
    actual_head = current_head if current_head is not None else current_git_head(root)
    if not expected_head or expected_head != actual_head:
        errors.append(f"source HEAD mismatch: bundle={expected_head} current={actual_head}")
    validate_artifact_hashes(bundle, root, errors)
    matrix = read_json(resolve_artifact(root, bundle.get("authorityMatrix"), errors, "authority matrix"), errors, "authority matrix")
    budget = read_json(resolve_artifact(root, bundle.get("styleBudget"), errors, "style budget"), errors, "style budget")
    config = read_json(resolve_artifact(root, bundle.get("gateConfig"), errors, "gate config"), errors, "gate config")
    review = read_json(resolve_artifact(root, bundle.get("review"), errors, "visual review"), errors, "visual review")
    if config.get("authorityMatrix") and config.get("authorityMatrix") != bundle.get("authorityMatrix"):
        errors.append("gate config authorityMatrix does not match bundle")
    if config.get("styleBudget") and config.get("styleBudget") != bundle.get("styleBudget"):
        errors.append("gate config styleBudget does not match bundle")
    validate_matrix_files(matrix, root, owner_root, errors)
    validate_style(bundle, budget, errors, root)
    validate_surface_review(bundle, config, review, root, errors)

    authorities = {}
    for profile in config.get("requiredProfiles") or []:
        authorities[profile] = validate_profile_geometry(profile, bundle, config, root, errors)
    if mode == "closure" and "mobile" in authorities:
        accepted = set(config.get("physicalMobileAuthorities") or [])
        if authorities["mobile"] not in accepted:
            errors.append(
                "physical mobile device evidence required for closure: "
                + (authorities["mobile"] or "missing authority"))
    return errors


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle", required=True)
    parser.add_argument("--root", default=str(ROOT))
    parser.add_argument("--owner-root", required=True)
    parser.add_argument("--mode", choices=("foundation", "closure"), default="closure")
    args = parser.parse_args()
    root = Path(args.root).resolve()
    bundle = read_json(Path(args.bundle), [], "bundle")
    errors = validate_bundle(bundle, root, Path(args.owner_root), mode=args.mode)
    if errors:
        print("LGO_UI_FIDELITY_DEVICE_GATE_FAIL")
        for error in errors:
            print("- " + error)
        return 1
    physical = "required-and-present" if args.mode == "closure" else "not-required"
    print(f"LGO_UI_FIDELITY_DEVICE_GATE_PASS mode={args.mode} physicalMobile={physical}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
