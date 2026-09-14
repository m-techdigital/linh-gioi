import json
import tempfile
import unittest
from pathlib import Path

from audit_lgo_source_staging_selection import audit_selection


class SourceStagingSelectionAuditTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def write_provenance(self, directory, **data):
        path = self.root / directory / "material-provenance.json"
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(json.dumps(data), encoding="utf-8")

    def write_authoring_selection(self, directory, **data):
        path = self.root / directory / "authoring-selection.json"
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(json.dumps(data), encoding="utf-8")

    def test_rejects_selected_source_inside_visual_rejected_directory(self):
        self.write_provenance(
            "outer-top-material-idle-v7",
            visualReviewStatus="VISUAL_REJECTED_PRESERVED_FOR_LESSONS",
            runtimeEligible=False,
        )
        selection = {
            "slots": {
                "outer_top": {
                    "idle": "outer-top-material-idle-v7/outer-top-idle-material.png",
                }
            }
        }

        report = audit_selection(self.root, selection)

        self.assertEqual(report["status"], "SOURCE_STAGING_SELECTION_REJECTED")
        self.assertIn(
            "outer_top/idle selects visually rejected directory outer-top-material-idle-v7",
            report["failures"],
        )

    def test_rejects_source_when_nearest_authoring_selection_rejects_visual(self):
        self.write_authoring_selection(
            "design-locked-authoring-v1",
            status="REJECTED_SOURCE_VISUAL",
            runtimeEligible=False,
        )
        selection = {
            "slots": {
                "outer_top": {
                    "idle": "design-locked-authoring-v1/outer_top/idle/front.png",
                }
            }
        }

        report = audit_selection(self.root, selection)

        self.assertEqual(report["status"], "SOURCE_STAGING_SELECTION_REJECTED")
        self.assertIn(
            "outer_top/idle selects authoring-rejected directory design-locked-authoring-v1",
            report["failures"],
        )

    def test_rejects_any_source_inside_rejected_evidence_tree(self):
        rejected_root = self.root.parent / "rejected-evidence"
        rejected_asset = rejected_root / "phap-lv001" / "bad-v1" / "outer-top.png"
        rejected_asset.parent.mkdir(parents=True, exist_ok=True)
        rejected_asset.write_bytes(b"evidence")
        selection = {
            "slots": {
                "outer_top": {
                    "idle": str(rejected_asset),
                }
            }
        }

        report = audit_selection(self.root, selection)

        self.assertEqual(report["status"], "SOURCE_STAGING_SELECTION_REJECTED")
        self.assertIn(
            "outer_top/idle selects quarantined rejected evidence",
            report["failures"],
        )

    def test_rejects_selected_source_mentioned_by_later_prior_rejected_entry(self):
        self.write_provenance(
            "outer-top-run-four-material-v3-bodymask",
            priorRejected=["outer-top-run-four-material-v2"],
            runtimeEligible=False,
        )
        selection = {
            "slots": {
                "outer_top": {
                    "run_a": "outer-top-run-four-material-v2/run_a-outer-top-material-export.png",
                }
            }
        }

        report = audit_selection(self.root, selection)

        self.assertEqual(report["status"], "SOURCE_STAGING_SELECTION_REJECTED")
        self.assertIn(
            "outer_top/run_a selects prior-rejected directory outer-top-run-four-material-v2",
            report["failures"],
        )

    def test_allows_current_review_required_source_when_not_rejected(self):
        self.write_provenance(
            "waist-belt-material-idle-v2",
            status="CANDIDATE_SOURCE_REVIEW_REQUIRED",
            selectionStatus="CURRENT_BEST_CANDIDATE_REVIEW_REQUIRED",
            runtimeEligible=False,
        )
        selection = {
            "slots": {
                "waist_belt": {
                    "idle": "waist-belt-material-idle-v2/waist-belt-idle-material-export.png",
                }
            }
        }

        report = audit_selection(self.root, selection)

        self.assertEqual(report["status"], "SOURCE_STAGING_SELECTION_REVIEW_ONLY")
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertEqual(report["failures"], [])

    def test_prior_rejected_absolute_provenance_path_resolves_to_source_directory(self):
        rejected = self.root / "waist-belt-material-idle-v1" / "material-provenance.json"
        rejected.parent.mkdir(parents=True, exist_ok=True)
        rejected.write_text("{}", encoding="utf-8")
        self.write_provenance(
            "waist-belt-material-idle-v2",
            priorRejected=str(rejected),
            runtimeEligible=False,
        )
        selection = {
            "slots": {
                "waist_belt": {
                    "idle": "waist-belt-material-idle-v1/waist-belt-idle-material.png",
                }
            }
        }

        report = audit_selection(self.root, selection)

        self.assertEqual(report["status"], "SOURCE_STAGING_SELECTION_REJECTED")
        self.assertIn("waist-belt-material-idle-v1", report["rejectionIndex"]["priorRejected"])
        self.assertIn(
            "waist_belt/idle selects prior-rejected directory waist-belt-material-idle-v1",
            report["failures"],
        )

    def test_rejected_tombstone_metadata_cannot_crash_or_pass_as_selection(self):
        selection = {
            "status": "REJECTED_SOURCE_MOVED",
            "runtimeEligible": False,
            "evidencePath": str(self.root.parent / "rejected-evidence" / "bad-source"),
        }

        report = audit_selection(self.root, selection)

        self.assertEqual(report["status"], "SOURCE_STAGING_SELECTION_REJECTED")
        self.assertIn(
            "selection must contain an object-valued slots field",
            report["failures"],
        )


if __name__ == "__main__":
    unittest.main()
