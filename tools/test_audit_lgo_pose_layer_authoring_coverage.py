import hashlib
import json
import struct
import tempfile
import unittest
import zlib
from pathlib import Path

from audit_lgo_pose_layer_authoring_coverage import POSES, audit_source_root, write_audit


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def payload_sha(data):
    data = dict(data)
    data.pop("auditPayloadSha256", None)
    return hashlib.sha256(
        json.dumps(data, ensure_ascii=False, sort_keys=True, separators=(",", ":")).encode()
    ).hexdigest()


def write_png(path, size=(1024, 1536), rgba=(90, 110, 130, 255)):
    path.parent.mkdir(parents=True, exist_ok=True)
    width, height = size
    rows = []
    for _ in range(height):
        row = bytearray([0])
        for _ in range(width):
            row.extend(rgba)
        rows.append(bytes(row))

    def chunk(kind, data):
        payload = kind + data
        return (
            struct.pack(">I", len(data))
            + payload
            + struct.pack(">I", zlib.crc32(payload) & 0xFFFFFFFF)
        )

    path.write_bytes(
        b"".join(
            [
                b"\x89PNG\r\n\x1a\n",
                chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)),
                chunk(b"IDAT", zlib.compress(b"".join(rows), level=9)),
                chunk(b"IEND", b""),
            ]
        )
    )
    return path


class PoseLayerAuthoringCoverageTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name) / "authoring"
        self.root.mkdir()
        self.review = self.root / "source-review-v1"
        self.review.mkdir()
        self.body = Path(self.temp.name) / "body"
        self.body.mkdir()
        for pose in POSES:
            write_png(self.body / f"{pose}-base.png", rgba=(20, 30, 40, 255))

    def png(self, path):
        return write_png(path)

    def native_slot(self, folder):
        root = self.root / folder
        for variant in ("A", "B"):
            for pose in POSES:
                for component in ("front", "back"):
                    self.png(root / variant / pose / f"{component}.png")
        (root / "authoring-report.json").write_text(
            json.dumps(
                {
                    "status": "AUTHORING_DIAGNOSTIC_ONLY",
                    "runtimeEligible": False,
                    "sourceStatus": "SOURCE_REVIEW_REQUIRED",
                }
            )
        )

    def idle_candidate(self, folder, status="CURRENT_BEST_CANDIDATE_REVIEW_REQUIRED"):
        root = self.root / folder
        self.png(root / "material-export.png")
        (root / "material-provenance.json").write_text(
            json.dumps(
                {
                    "status": "CANDIDATE_SOURCE_REVIEW_REQUIRED",
                    "runtimeEligible": False,
                    "selectionStatus": status,
                    "reviewFinding": "VISUAL_REVIEW_REQUIRED",
                }
            )
        )

    def waist_run_candidate(self):
        root = self.root / "waist-belt-run-four-material-v1"
        for pose in ("run_contact_a", "run_a", "run_contact_b", "run_b"):
            self.png(root / f"{pose}-waist-belt-material-export.png")
        (root / "material-provenance.json").write_text(
            json.dumps(
                {
                    "status": "RUN_POSE_DRAFT_REVIEW_REQUIRED",
                    "runtimeEligible": False,
                    "poses": ["run_contact_a", "run_a", "run_contact_b", "run_b"],
                }
            )
        )

    def test_reports_six_pose_native_slots_and_idle_only_blockers(self):
        self.native_slot("inner-top-native-v2")
        self.native_slot("accessory-native-v1")
        self.idle_candidate("outer-top-material-idle-v2")
        self.idle_candidate("waist-belt-material-idle-v2")
        self.idle_candidate("shoulder-chest-guard-material-idle-v4")

        result = audit_source_root(self.root, self.body)

        self.assertEqual(result["status"], "SOURCE_COVERAGE_INCOMPLETE")
        self.assertEqual(result["slots"]["inner_top"]["present"], 24)
        self.assertEqual(result["slots"]["class_accessory"]["present"], 24)
        self.assertEqual(
            result["slots"]["waist_belt"]["missingPoses"],
            ["run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"],
        )
        self.assertTrue(
            any("outer_top: only idle candidate" in gate for gate in result["blockingGates"])
        )
        self.assertFalse(result["runtimeEligible"])

    def test_counts_waist_run_draft_without_treating_it_as_acceptance(self):
        self.native_slot("inner-top-native-v2")
        self.native_slot("accessory-native-v1")
        self.idle_candidate("outer-top-material-idle-v2")
        self.idle_candidate("waist-belt-material-idle-v2")
        self.waist_run_candidate()
        self.idle_candidate("shoulder-chest-guard-material-idle-v4")

        result = audit_source_root(self.root, self.body)

        self.assertEqual(
            result["slots"]["waist_belt"]["coveredCandidatePoses"],
            ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b"],
        )
        self.assertEqual(result["slots"]["waist_belt"]["missingPoses"], ["jump_tuck"])
        self.assertIn(
            "waist_belt: draft covers 5/6 poses but lacks A/B source acceptance",
            result["blockingGates"],
        )

    def test_write_audit_records_sha_and_never_promotes_runtime(self):
        self.native_slot("inner-top-native-v2")
        self.native_slot("accessory-native-v1")
        for folder in (
            "outer-top-material-idle-v2",
            "waist-belt-material-idle-v2",
            "shoulder-chest-guard-material-idle-v4",
        ):
            self.idle_candidate(folder)
        output = self.root / "source-review-v1" / "coverage.json"

        result = write_audit(self.root, self.body, output)

        written = json.loads(output.read_text())
        self.assertEqual(written["auditPayloadSha256"], payload_sha(written))
        self.assertFalse(result["runtimeEligible"])


if __name__ == "__main__":
    unittest.main()
