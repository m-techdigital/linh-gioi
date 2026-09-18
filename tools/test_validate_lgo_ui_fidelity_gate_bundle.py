#!/usr/bin/env python3
import hashlib
import json
import tempfile
import unittest
from pathlib import Path

from validate_lgo_ui_fidelity_gate_bundle import validate_bundle


def sha(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


class FidelityGateBundleTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.root = Path(self.tmp.name)
        self.owner = self.root / "owner"
        self.owner.mkdir()
        self._write_fixture()

    def tearDown(self):
        self.tmp.cleanup()

    def write_json(self, rel, value):
        path = self.root / rel
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(json.dumps(value, indent=2), encoding="utf-8")
        return path
    def _ui_metrics(self, authority, mobile=False):
        scale = 0.85 if mobile else 1.0
        shell_w, shell_h = 1098 * scale, 724 * scale
        panel_w, panel_h = (2091, 941) if mobile else (1673, 941)
        return {
            "evidenceAuthority": authority,
            "screenWidth": 1600,
            "screenHeight": 720 if mobile else 900,
            "panelWidth": panel_w,
            "panelHeight": panel_h,
            "safePanelX": 0, "safePanelY": 0,
            "safePanelWidth": panel_w, "safePanelHeight": panel_h,
            "layoutClass": "mobile" if mobile else "desktop",
            "inputClass": "touch" if mobile else "pointer",
            "characterHubShellWidth": shell_w,
            "characterHubShellHeight": shell_h,
            "characterHubShellScreenHeightRatio": shell_h / panel_h,
            "presentationScale": scale,
            "minimumTouchTargetPanelUnits": 44,
            "minimumTouchTargetScreenPixels": 34 if mobile else 42,
            "gameplayPlayerStatus": {"x": 14, "y": 14, "width": 260, "height": 78},
            "gameplayRightInfo": {"x": panel_w - 242, "y": 14, "width": 228, "height": 310},
            "gameplayCombat": {"x": panel_w - 344, "y": 843, "width": 330, "height": 84},
            "gameplayContext": {"x": panel_w - 344, "y": 763, "width": 180, "height": 64},
            "gameplaySecondaryNav": {"x": panel_w - 98, "y": 763, "width": 84, "height": 68},
            "gameplayTouchPad": {"x": 14, "y": 803, "width": 124 if mobile else 0, "height": 124 if mobile else 0},
            "gameplayDialogue": {"x": 14, "y": 657, "width": 430, "height": 270},
        }
    def _write_profile(self, profile, authority, physical=False):
        mobile = profile == "mobile"
        base = self.root / "evidence" / profile
        hub = self._ui_metrics(authority, mobile)
        quest = dict(hub)
        world = {
            "profileName": "mobile" if mobile else "desktop",
            "cameraOrthographicSize": 4.12 if mobile else 3.8,
            "actorScreenHeightRatio": 0.198 if mobile else 0.215,
            "npcScreenHeightRatio": 0.231 if mobile else 0.25,
            "backgroundCoverageScale": 1.9,
            "interactionMarkerFontSize": 64,
            "interactionMarkerCharacterSize": 0.045,
        }
        self.write_json(f"evidence/{profile}/character-hub/manifest.json",
                        {"status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                         "uiMetrics": hub, "width": 1600, "height": 720 if mobile else 900})
        self.write_json(f"evidence/{profile}/quest/manifest.json",
                        {"status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                         "uiMetrics": quest, "worldMetrics": world,
                         "width": 1600, "height": 720 if mobile else 900})
        shot = base / "character-hub/character-info.png"
        shot.write_bytes((profile + "-frame").encode())
        return shot

    def _write_fixture(self):
        design = self.owner / "hub.png"
        design.write_bytes(b"canonical")
        code = self.root / "code.cs"
        code.write_text("// owner", encoding="utf-8")
        matrix = {
            "version": 1,
            "surfaces": [{
                "id": "hub-character",
                "designAuthority": {"kind": "owner", "relativePath": "hub.png", "sha256": sha(design)},
                "runtimeEvidence": ["evidence/pc/character-hub/character-info.png"],
                "codeOwners": ["code.cs"],
                "states": ["default"],
                "profiles": ["pc", "mobile-landscape-simulation"],
                "knownGaps": [],
            }],
        }
        matrix_path = self.write_json("matrix.json", matrix)
        budget = {
            "directStyleAssignments": 10,
            "numericStyleAssignments": 10,
            "semanticPaletteDeclarations": 0,
            "skinPartialMaxLoc": 100,
            "skinPartialMaxMethods": 20,
        }
        budget_path = self.write_json("budget.json", budget)
        config = {
            "version": 1,
            "requiredProfiles": ["pc", "mobile"],
            "surfaceEvidence": {"hub-character": "character-hub/character-info.png"},
            "geometry": {
                "hubAspect": 1098 / 724,
                "hubAspectTolerance": 0.01,
                "hubHeightRatio": {"pc": [0.75, 0.80], "mobile": [0.62, 0.70]},
                "presentationScale": {"pc": [0.995, 1.005], "mobile": [0.82, 0.90]},
                "actorHeightRatio": [0.18, 0.27],
                "npcHeightRatio": [0.16, 0.27],
            },
            "physicalMobileAuthorities": ["physical-android-device", "physical-ios-device"],
        }
        config_path = self.write_json("config.json", config)
        pc = self._write_profile("pc", "macos-player")
        mobile = self._write_profile("mobile", "macos-aspect-simulation")
        review = {
            "version": 1,
            "status": "PASS",
            "reviewedBy": "human-fixture",
            "artifacts": {
                "hub-character:pc": sha(pc),
                "hub-character:mobile": sha(mobile),
            },
        }
        review_path = self.write_json("review.json", review)
        artifacts = {}
        for path in [
            matrix_path, budget_path, config_path, review_path,
            self.root / "evidence/pc/character-hub/manifest.json",
            self.root / "evidence/pc/quest/manifest.json",
            self.root / "evidence/mobile/character-hub/manifest.json",
            self.root / "evidence/mobile/quest/manifest.json",
            pc, mobile,
        ]:
            artifacts[str(path.relative_to(self.root))] = sha(path)
        self.bundle = {
            "version": 1,
            "sourceHead": "abc123",
            "authorityMatrix": "matrix.json",
            "styleBudget": "budget.json",
            "gateConfig": "config.json",
            "review": "review.json",
            "evidenceRoot": "evidence",
            "styleMetrics": {
                "directStyleAssignments": 8,
                "numericStyleAssignments": 9,
                "semanticPaletteDeclarations": 0,
                "skinPartialMaxLoc": 90,
                "skinPartialMaxMethods": 18,
            },
            "artifacts": artifacts,
        }

    def validate(self, mode="foundation", bundle=None):
        return validate_bundle(bundle or self.bundle, self.root, self.owner, "abc123", mode=mode)
    def test_foundation_accepts_simulation_when_all_bindings_and_geometry_are_valid(self):
        self.assertEqual([], self.validate("foundation"))

    def test_rejects_source_head_mismatch(self):
        errors = validate_bundle(self.bundle, self.root, self.owner, "different", mode="foundation")
        self.assertTrue(any("source HEAD mismatch" in e for e in errors), errors)

    def test_rejects_screenshot_mutated_after_review(self):
        shot = self.root / "evidence/mobile/character-hub/character-info.png"
        shot.write_bytes(b"tampered")
        errors = self.validate("foundation")
        self.assertTrue(any("artifact sha256 mismatch" in e or "review hash mismatch" in e for e in errors), errors)

    def test_rejects_geometry_outside_safe_panel(self):
        p = self.root / "evidence/mobile/quest/manifest.json"
        doc = json.loads(p.read_text())
        doc["uiMetrics"]["gameplayCombat"]["x"] = 3000
        p.write_text(json.dumps(doc))
        self.bundle["artifacts"][str(p.relative_to(self.root))] = sha(p)
        errors = self.validate("foundation")
        self.assertTrue(any("outside safe panel" in e for e in errors), errors)

    def test_closure_rejects_mobile_aspect_simulation(self):
        errors = self.validate("closure")
        self.assertTrue(any("physical mobile device evidence required" in e for e in errors), errors)

    def test_closure_accepts_explicit_physical_mobile_evidence(self):
        for rel in ["evidence/mobile/character-hub/manifest.json", "evidence/mobile/quest/manifest.json"]:
            p = self.root / rel
            doc = json.loads(p.read_text())
            doc["uiMetrics"]["evidenceAuthority"] = "physical-android-device"
            p.write_text(json.dumps(doc))
            self.bundle["artifacts"][rel] = sha(p)
        self.assertEqual([], self.validate("closure"))

    def test_visual_evidence_closure_is_wired_to_authoritative_bundle_gate(self):
        repo = Path(__file__).resolve().parents[1]
        source = (repo / "tools/lgo_playable_closure_check.sh").read_text(encoding="utf-8")
        self.assertIn("LGO_UI_FIDELITY_GATE_BUNDLE", source)
        self.assertIn("validate_lgo_ui_fidelity_gate_bundle.py", source)
        self.assertIn("--mode closure", source)
        self.assertIn("LGO_UI_OWNER_ROOT", source)


if __name__ == "__main__":
    unittest.main()
