import json
import subprocess
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

import capture_lgo_class_equipment as capture


class ClassEquipmentCaptureCommandTests(unittest.TestCase):
    def test_command_uses_map01a_preview_and_absolute_output(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            out = Path(tmp) / "relative-out"
            command = capture.build_command(player, "phap", out)
            self.assertIn("--lgo-map01a-art-preview", command)
            self.assertIn("--lgo-phap-capture", command)
            art_dir = command[command.index("--lgo-map01a-art-dir") + 1]
            self.assertTrue(Path(art_dir).is_absolute())
            self.assertIn(str(out.resolve()), command)

    def test_rejects_unknown_class(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            with self.assertRaisesRegex(ValueError, "unsupported class"):
                capture.build_command(player, "vo", Path(tmp) / "out")

    def test_capture_accepts_only_audit_not_promotion_ready_manifest(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            out = Path(tmp) / "out"

            def write_manifest(command, cwd=None):
                art_dir = Path(command[command.index("--lgo-map01a-art-dir") + 1])
                art_dir.mkdir(parents=True, exist_ok=True)
                (art_dir / "manifest.json").write_text(json.dumps({
                    "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                    "fitStatus": "DRAFT_RUNTIME_FIT",
                    "promotionStatus": "AUDIT_ONLY_NOT_PROMOTION_READY",
                    "runtimeEligibleCount": 0,
                    "frames": 27,
                    "actorFrameMetrics": [
                        {"file": "male-lv10-full.bmp", "actorHeightRatio": 0.28},
                        {"file": "male-lv10-jump-apex.bmp", "actorHeightRatio": 0.31}
                    ],
                    "idleActorHeightRatio": 0.28,
                    "maxRunToIdleHeightRatio": 1.04,
                    "maxJumpToIdleHeightRatio": 1.11,
                    "maxMotionToIdleHeightRatio": 1.11,
                    "errors": [],
                }))
                return subprocess.CompletedProcess(command, 0)

            with patch.object(capture.subprocess, "run", side_effect=write_manifest):
                data = capture.capture(player, "kiem", out)

            self.assertEqual("AUDIT_ONLY_NOT_PROMOTION_READY", data["promotionStatus"])
            self.assertEqual(0, data["runtimeEligibleCount"])


    def test_capture_rejects_manifest_without_actor_scale_metrics(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            out = Path(tmp) / "out"

            def write_manifest(command, cwd=None):
                art_dir = Path(command[command.index("--lgo-map01a-art-dir") + 1])
                art_dir.mkdir(parents=True, exist_ok=True)
                (art_dir / "manifest.json").write_text(json.dumps({
                    "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                    "fitStatus": "DRAFT_RUNTIME_FIT",
                    "promotionStatus": "AUDIT_ONLY_NOT_PROMOTION_READY",
                    "runtimeEligibleCount": 0,
                    "frames": 27,
                    "errors": [],
                }))
                return subprocess.CompletedProcess(command, 0)

            with patch.object(capture.subprocess, "run", side_effect=write_manifest):
                with self.assertRaisesRegex(RuntimeError, "actor scale metrics"):
                    capture.capture(player, "kiem", out)

    def test_capture_accepts_manifest_with_actor_scale_metrics(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            out = Path(tmp) / "out"

            def write_manifest(command, cwd=None):
                art_dir = Path(command[command.index("--lgo-map01a-art-dir") + 1])
                art_dir.mkdir(parents=True, exist_ok=True)
                (art_dir / "manifest.json").write_text(json.dumps({
                    "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                    "fitStatus": "DRAFT_RUNTIME_FIT",
                    "promotionStatus": "AUDIT_ONLY_NOT_PROMOTION_READY",
                    "runtimeEligibleCount": 0,
                    "frames": 27,
                    "actorFrameMetrics": [
                        {"file": "male-lv10-full.bmp", "actorHeightRatio": 0.28},
                        {"file": "male-lv10-jump-apex.bmp", "actorHeightRatio": 0.31}
                    ],
                    "idleActorHeightRatio": 0.28,
                    "maxRunToIdleHeightRatio": 1.04,
                    "maxJumpToIdleHeightRatio": 1.11,
                    "maxMotionToIdleHeightRatio": 1.11,
                    "errors": [],
                }))
                return subprocess.CompletedProcess(command, 0)

            with patch.object(capture.subprocess, "run", side_effect=write_manifest):
                data = capture.capture(player, "kiem", out)

            self.assertLessEqual(data["maxJumpToIdleHeightRatio"], 1.18)

    def test_capture_rejects_manifest_that_claims_promotion_ready(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            out = Path(tmp) / "out"

            def write_manifest(command, cwd=None):
                art_dir = Path(command[command.index("--lgo-map01a-art-dir") + 1])
                art_dir.mkdir(parents=True, exist_ok=True)
                (art_dir / "manifest.json").write_text(json.dumps({
                    "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                    "fitStatus": "DRAFT_RUNTIME_FIT",
                    "promotionStatus": "VISUAL_ACCEPTED_FOR_PACKING",
                    "runtimeEligibleCount": 1,
                    "frames": 27,
                    "errors": [],
                }))
                return subprocess.CompletedProcess(command, 0)

            with patch.object(capture.subprocess, "run", side_effect=write_manifest):
                with self.assertRaisesRegex(RuntimeError, "audit-only"):
                    capture.capture(player, "kiem", out)


if __name__ == "__main__":
    unittest.main()
