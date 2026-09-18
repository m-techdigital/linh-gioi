import json
import tempfile
import unittest
from pathlib import Path

import capture_lgo_character_hub as capture


def valid_ui_metrics(profile: str) -> dict:
    width, height = capture.PROFILES[profile]
    authority = capture.expected_evidence_authority(profile)
    panel_height = 941
    panel_width = round(width * panel_height / height)
    presentation_scale = 0.85 if profile == "mobile" else 1.0
    shell_width = 1098 * presentation_scale
    shell_height = 724 * presentation_scale
    return {
        "evidenceAuthority": authority,
        "screenWidth": width, "screenHeight": height,
        "panelWidth": panel_width, "panelHeight": panel_height,
        "safePanelX": 0, "safePanelY": 0,
        "safePanelWidth": panel_width, "safePanelHeight": panel_height,
        "layoutClass": "desktop" if profile == "pc" else profile,
        "inputClass": "pointer" if profile == "pc" else "touch",
        "panelSettings": "scaleMode=ScaleWithScreenSize referenceResolution=1672x941 screenMatchMode=MatchWidthOrHeight match=1",
        "characterHubShellX": (panel_width - shell_width) / 2,
        "characterHubShellY": (panel_height - shell_height) / 2 + 18,
        "characterHubShellWidth": shell_width, "characterHubShellHeight": shell_height,
        "characterHubShellScreenHeightRatio": shell_height / 941,
        "presentationScale": presentation_scale,
        "minimumTouchTargetPanelUnits": 44,
        "minimumTouchTargetScreenPixels": 44 * height / 941,
    }


class CaptureLgoCharacterHubTests(unittest.TestCase):
    def test_equipment_selection_detail_is_required_for_every_shared_slot(self) -> None:
        for slot in ('main_weapon','head_hair','inner_top','outer_tunic','lower_garment',
                     'waist','arm_guard','boots','light_armor','accessory'):
            self.assertIn(f'item-{slot}-selected.png', capture.REQUIRED_FRAMES)
        self.assertEqual(6, capture.REQUIRED_FRAMES.index("item-main_weapon-selected.png"), "Manifest order must match the existing Player capture sequence.")

    def test_requested_skill_node_is_explicit_and_bounded(self) -> None:
        for node in range(9):
            command = capture.build_player_command(Path('/tmp/Unity'), Path('/tmp/out'), 'pc', 1600, 900, skill_node=node)
            self.assertEqual(str(node), command[command.index('--lgo-character-hub-skill-node') + 1])
            errors = capture.validate_manifest({'skillSelectedNodeIndex': node}, Path('/does-not-exist'), 'pc', skill_node=node)
            self.assertNotIn('SKILL_SELECTED_NODE_MISMATCH', errors)
            self.assertIn('SKILL_SELECTED_NODE_MISMATCH', capture.validate_manifest({'skillSelectedNodeIndex': (node+1)%9}, Path('/does-not-exist'), 'pc', skill_node=node))
        for bad in (-1,9,True,'3'):
            with self.subTest(node=bad), self.assertRaises(ValueError):
                capture.build_player_command(Path('/tmp/Unity'),Path('/tmp/out'),'pc',1600,900,skill_node=bad)

    def test_profiles_use_three_real_target_viewports(self) -> None:
        self.assertEqual((1600, 900), capture.PROFILES["pc"])
        self.assertEqual((1024, 768), capture.PROFILES["tablet"])
        self.assertEqual((1600, 720), capture.PROFILES["mobile"])
        self.assertEqual(3, len(set(capture.PROFILES.values())))

    def test_mobile_metrics_require_compact_shell_and_reject_desktop_sized_occupancy(self) -> None:
        compact = {
            "evidenceAuthority": capture.expected_evidence_authority("mobile"),
            "uiMetrics": valid_ui_metrics("mobile"),
        }
        self.assertEqual([], capture.validate_ui_metrics(compact, "mobile"))

        oversized = json.loads(json.dumps(compact))
        oversized["uiMetrics"]["presentationScale"] = 1.0
        oversized["uiMetrics"]["characterHubShellWidth"] = 1098
        oversized["uiMetrics"]["characterHubShellHeight"] = 724
        oversized["uiMetrics"]["characterHubShellScreenHeightRatio"] = 724 / 941
        errors = capture.validate_ui_metrics(oversized, "mobile")
        self.assertIn("PRESENTATION_SCALE_INVALID", errors)
        self.assertIn("CHARACTER_HUB_OCCUPANCY_INVALID", errors)

        distorted = json.loads(json.dumps(compact))
        distorted["uiMetrics"]["characterHubShellWidth"] = 1098
        errors = capture.validate_ui_metrics(distorted, "mobile")
        self.assertIn("CHARACTER_HUB_SHELL_METRICS_MISMATCH", errors)
        self.assertIn("CHARACTER_HUB_SHELL_ASPECT_MISMATCH", errors)

    def test_capture_requires_data_only_potential_frames_for_all_five_classes(self) -> None:
        expected = tuple(
            f"potential-{class_id}-{state}.png"
            for class_id in ("vo", "kiem", "phap", "co", "linh")
            for state in ("default", "selected")
        )
        for frame in expected:
            self.assertIn(frame, capture.REQUIRED_FRAMES)

    def test_capture_requires_shared_spirit_pet_template_for_all_five_classes(self) -> None:
        expected = tuple(
            f"spirit-pet-{class_id}.png"
            for class_id in ("vo", "kiem", "phap", "co", "linh")
        )
        for frame in expected:
            self.assertIn(frame, capture.REQUIRED_FRAMES)

    def test_capture_requires_shared_skill_topology_for_all_five_classes(self) -> None:
        expected = tuple(
            f"skills-{class_id}.png"
            for class_id in ("vo", "kiem", "phap", "co", "linh")
        )
        for frame in expected:
            self.assertIn(frame, capture.REQUIRED_FRAMES)

    def test_selected_skill_detail_is_required_for_each_class(self) -> None:
        for class_id in capture.CHARACTER_HUB_CLASS_IDS:
            self.assertIn(f"skills-{class_id}-selected.png", capture.REQUIRED_FRAMES)

    def test_player_command_uses_target_resolution_and_internal_capture(self) -> None:
        command = capture.build_player_command(
            Path("/tmp/Unity"), Path("/tmp/evidence"), "mobile", 1600, 720
        )
        self.assertIn("-screen-width", command)
        self.assertEqual("1600", command[command.index("-screen-width") + 1])
        self.assertEqual("720", command[command.index("-screen-height") + 1])
        self.assertIn("--lgo-map01a-inventory-tabs-capture", command)
        self.assertNotIn("osascript", command)

    def test_manifest_must_match_profile_resolution_and_complete_frames(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            out = Path(temp)
            for frame in capture.REQUIRED_FRAMES:
                (out / frame).write_bytes(b"png")
            manifest = {
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "captureScope": "map01a-inventory-tabs",
                "usesOsMouseOrKeyboard": False,
                "width": 1600,
                "height": 900,
                "frames": list(capture.REQUIRED_FRAMES),
                "skillClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "skillSelectedNodeIndex": 5,
                "potentialClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "spiritPetClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "classSwitchScope": "character-hub-data-only-no-renderer-change",
                "evidenceAuthority": capture.expected_evidence_authority("pc"),
                "uiMetrics": valid_ui_metrics("pc"),
            }
            self.assertEqual([], capture.validate_manifest(manifest, out, "pc"))
            manifest["width"] = 1024
            self.assertIn("VIEWPORT_MISMATCH", capture.validate_manifest(manifest, out, "pc"))
            manifest["width"] = 1600
            manifest["potentialClassProfiles"] = ["vo"]
            self.assertIn("POTENTIAL_CLASS_PROFILE_MISMATCH", capture.validate_manifest(manifest, out, "pc"))
            manifest["potentialClassProfiles"] = list(capture.CHARACTER_HUB_CLASS_IDS)
            manifest["classSwitchScope"] = "renderer-class-switch"
            self.assertIn("CLASS_SWITCH_SCOPE_INVALID", capture.validate_manifest(manifest, out, "pc"))
            manifest["skillSelectedNodeIndex"] = 0
            self.assertIn("SKILL_SELECTED_NODE_MISMATCH", capture.validate_manifest(manifest, out, "pc"))

    def test_manifest_requires_measured_ui_metrics_and_truthful_simulation_authority(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            out = Path(temp)
            for frame in capture.REQUIRED_FRAMES:
                (out / frame).write_bytes(b"png")
            manifest = {
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "captureScope": "map01a-inventory-tabs",
                "usesOsMouseOrKeyboard": False,
                "width": 1600, "height": 720,
                "frames": list(capture.REQUIRED_FRAMES),
                "skillClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "skillSelectedNodeIndex": 5,
                "potentialClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "spiritPetClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "classSwitchScope": "character-hub-data-only-no-renderer-change",
            }
            errors = capture.validate_manifest(manifest, out, "mobile")
            self.assertIn("MISSING_UI_METRICS", errors)
            self.assertIn("EVIDENCE_AUTHORITY_MISMATCH", errors)

    def test_manifest_rejects_missing_frame_and_os_input(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            out = Path(temp)
            manifest = {
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "captureScope": "map01a-inventory-tabs",
                "usesOsMouseOrKeyboard": True,
                "width": 1024,
                "height": 768,
                "frames": list(capture.REQUIRED_FRAMES),
                "skillClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "skillSelectedNodeIndex": 5,
                "potentialClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "spiritPetClassProfiles": list(capture.CHARACTER_HUB_CLASS_IDS),
                "classSwitchScope": "character-hub-data-only-no-renderer-change",
            }
            errors = capture.validate_manifest(manifest, out, "tablet")
            self.assertIn("OS_INPUT_USED", errors)
            self.assertTrue(any(error.startswith("MISSING_FRAME:") for error in errors), errors)


if __name__ == "__main__":
    unittest.main()
