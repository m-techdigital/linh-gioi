import json
import tempfile
import unittest
from pathlib import Path

from audit_lgo_slot_envelope_fit import (
    audit_candidate_against_brief,
    build_slot_envelope,
    build_slot_envelope_index,
)
from stage_lgo_six_pose_repair_layers import write_rgba_png


def write_alpha_rect(path: Path, bbox, size=(1024, 1536), rgba=(30, 40, 60, 255)):
    width, height = size
    pixels = bytearray(width * height * 4)
    min_x, min_y, max_x, max_y = bbox
    for y in range(min_y, max_y + 1):
        for x in range(min_x, max_x + 1):
            index = (y * width + x) * 4
            pixels[index : index + 4] = bytes(rgba)
    write_rgba_png(path, width, height, pixels)


class SlotEnvelopeFitTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.brief = {
            "status": "MISSING_SOURCE_AUTHORING_BRIEF_READY",
            "targets": [
                {
                    "slot": "waist_belt",
                    "pose": "jump_tuck",
                    "slotGuide": {
                        "center": [439.71, 803.1],
                        "line": [[382.17, 783.78], [497.25, 822.42]],
                        "angleDegrees": 18.56,
                        "halfWidthPx": 60.7,
                    },
                    "measurements": {
                        "torsoLengthPx": 379.63,
                        "hipWidthPx": 59.84,
                    },
                }
            ],
        }
        self.outer_target = {
            "slot": "outer_top",
            "pose": "run_contact_a",
            "slotGuide": {
                "center": [608.92, 608.08],
                "shoulderLine": [[537.92, 500.04], [735.08, 571.96]],
                "waistLine": [[462.59, 727.06], [637.49, 790.86]],
                "torsoAxis": [[655.0, 490.0], [527.0, 818.0]],
            },
            "measurements": {
                "torsoLengthPx": 352.09,
                "shoulderWidthPx": 169.25,
                "hipWidthPx": 82.02,
            },
        }
        self.guard_target = {
            "slot": "shoulder_chest_guard",
            "pose": "run_contact_a",
            "slotGuide": {
                "center": [623.0, 572.0],
                "shoulderLine": [[552.71, 505.43], [720.29, 566.57]],
                "chestLine": [[554.7, 588.3], [663.14, 627.86]],
                "angleDegrees": 20.04,
            },
            "measurements": {
                "torsoLengthPx": 352.09,
                "shoulderWidthPx": 169.25,
                "hipWidthPx": 82.02,
            },
        }

    def test_rejects_oversized_imagegen_belt_before_source_staging(self):
        candidate = self.root / "oversized.png"
        write_alpha_rect(candidate, [158, 554, 866, 1094])

        result = audit_candidate_against_brief(self.brief, candidate, "waist_belt", "jump_tuck")

        self.assertEqual(result["status"], "SLOT_ENVELOPE_FIT_FAIL")
        self.assertFalse(result["sourceCandidateAllowed"])
        self.assertEqual(result["candidate"]["alphaBBox"], [158, 554, 866, 1094])
        self.assertIn("bboxWidth", result["failures"])
        self.assertIn("bboxHeight", result["failures"])
        self.assertIn("centerDistance", result["failures"])

    def test_small_candidate_in_slot_passes_fit_gate_but_not_visual_acceptance(self):
        candidate = self.root / "in-slot.png"
        write_alpha_rect(candidate, [379, 773, 500, 833])

        result = audit_candidate_against_brief(self.brief, candidate, "waist_belt", "jump_tuck")

        self.assertEqual(result["status"], "SLOT_ENVELOPE_FIT_PASS_REVIEW_REQUIRED")
        self.assertTrue(result["sourceCandidateAllowed"])
        self.assertFalse(result["runtimePromotionAllowed"])
        self.assertFalse(result["visualAccepted"])
        self.assertEqual(result["candidate"]["alphaBBox"], [379, 773, 500, 833])
        self.assertEqual(result["failures"], {})

    def test_builds_reusable_numeric_envelope_from_slot_guide(self):
        target = self.brief["targets"][0]

        envelope = build_slot_envelope(target)

        self.assertEqual(envelope["slot"], "waist_belt")
        self.assertEqual(envelope["pose"], "jump_tuck")
        self.assertEqual(envelope["profile"], {"canvasSize": [1024, 1536], "originX": 512, "groundY": 1484})
        self.assertAlmostEqual(envelope["guide"]["lineLengthPx"], 121.39, places=2)
        self.assertEqual(envelope["limits"]["maxBBoxWidthPx"], 267)
        self.assertEqual(envelope["limits"]["maxBBoxHeightPx"], 146)
        self.assertEqual(envelope["limits"]["maxCenterDistancePx"], 55)
        self.assertFalse(envelope["runtimePromotionAllowed"])

    def test_writes_repeatable_report_file(self):
        candidate = self.root / "oversized.png"
        output = self.root / "report.json"
        write_alpha_rect(candidate, [158, 554, 866, 1094])

        result = audit_candidate_against_brief(
            self.brief,
            candidate,
            "waist_belt",
            "jump_tuck",
            output=output,
        )

        written = json.loads(output.read_text())
        self.assertEqual(written["status"], result["status"])
        self.assertEqual(written["auditPayloadSha256"], result["auditPayloadSha256"])

    def test_builds_outer_top_envelope_from_shoulder_waist_torso_guide(self):
        envelope = build_slot_envelope(self.outer_target)

        self.assertEqual(envelope["slot"], "outer_top")
        self.assertEqual(envelope["pose"], "run_contact_a")
        self.assertEqual(envelope["guide"]["kind"], "torso")
        self.assertAlmostEqual(envelope["guide"]["lineLengthPx"], 352.09, places=2)
        self.assertEqual(envelope["limits"]["maxBBoxWidthPx"], 279)
        self.assertEqual(envelope["limits"]["maxBBoxHeightPx"], 405)
        self.assertEqual(envelope["limits"]["maxCenterDistancePx"], 77)

    def test_builds_guard_envelope_from_shoulder_chest_guide(self):
        envelope = build_slot_envelope(self.guard_target)

        self.assertEqual(envelope["slot"], "shoulder_chest_guard")
        self.assertEqual(envelope["guide"]["kind"], "guard")
        self.assertAlmostEqual(envelope["guide"]["lineLengthPx"], 178.38, places=2)
        self.assertEqual(envelope["limits"]["maxBBoxWidthPx"], 245)
        self.assertEqual(envelope["limits"]["maxBBoxHeightPx"], 158)
        self.assertEqual(envelope["limits"]["maxCenterDistancePx"], 54)

    def test_builds_envelope_index_for_all_missing_targets(self):
        brief = {"targets": [self.brief["targets"][0], self.outer_target, self.guard_target]}

        index = build_slot_envelope_index(brief)

        self.assertEqual(index["status"], "SLOT_ENVELOPE_INDEX_READY")
        self.assertEqual(index["targetCount"], 3)
        self.assertEqual(
            [item["id"] for item in index["envelopes"]],
            ["waist_belt/jump_tuck", "outer_top/run_contact_a", "shoulder_chest_guard/run_contact_a"],
        )
        self.assertFalse(index["runtimePromotionAllowed"])


if __name__ == "__main__":
    unittest.main()
