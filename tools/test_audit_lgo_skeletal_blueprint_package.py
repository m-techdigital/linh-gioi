import json
import struct
import tempfile
import unittest
import zlib
from pathlib import Path

from audit_lgo_skeletal_blueprint_package import audit_blueprint_package


REQUIRED_POSES = [
    "idle",
    "run_contact_a",
    "run_a",
    "run_contact_b",
    "run_b",
    "jump_tuck",
]

REQUIRED_COMPONENTS = [
    "head_neck",
    "torso_hips",
    "near_upper_arm",
    "near_forearm_hand",
    "far_upper_arm",
    "far_forearm_hand",
    "near_thigh",
    "near_shin_foot",
    "far_thigh",
    "far_shin_foot",
]

REQUIRED_JOINTS = [
    "crown",
    "chin",
    "neck",
    "near_shoulder",
    "near_elbow",
    "near_wrist",
    "far_shoulder",
    "far_elbow",
    "far_wrist",
    "near_hip",
    "near_knee",
    "near_ankle",
    "far_hip",
    "far_knee",
    "far_ankle",
]


def write_cutout(path, mode="RGBA", size=(1024, 1536), full_opaque=False):
    width, height = size
    color_type = 6 if mode == "RGBA" else 2
    channels = 4 if mode == "RGBA" else 3
    rows = []
    for y in range(height):
        row = bytearray([0])
        for x in range(width):
            if mode == "RGBA":
                alpha = 255 if full_opaque or (x == width // 2 and y == height // 2) else 0
                row.extend([10, 20, 30, alpha])
            else:
                row.extend([10, 20, 30])
        rows.append(bytes(row))

    def chunk(kind, data):
        payload = kind + data
        return struct.pack(">I", len(data)) + payload + struct.pack(">I", zlib.crc32(payload) & 0xFFFFFFFF)

    png = b"".join(
        [
            b"\x89PNG\r\n\x1a\n",
            chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, color_type, 0, 0, 0)),
            chunk(b"IDAT", zlib.compress(b"".join(rows), level=9)),
            chunk(b"IEND", b""),
        ]
    )
    path.write_bytes(png)


class SkeletalBlueprintPackageAuditTests(unittest.TestCase):
    def _package(self):
        temporary = tempfile.TemporaryDirectory()
        root = Path(temporary.name)
        (root / "body.kra").write_bytes(b"native")
        write_cutout(root / "composite.png")
        for pose in REQUIRED_POSES:
            write_cutout(root / f"{pose}.png")
        components = []
        for component_id in REQUIRED_COMPONENTS:
            path = root / f"{component_id}.png"
            write_cutout(path)
            components.append(
                {
                    "id": component_id,
                    "path": path.name,
                    "slot": "body",
                    "ownership": ["torso"] if component_id == "torso_hips" else ["hand"],
                }
            )
        manifest = {
            "canvas": {"width": 1024, "height": 1536, "originX": 512, "groundY": 1484},
            "nativeSource": "body.kra",
            "composite": "composite.png",
            "neutralBody": {"bakedOptionalOutfit": False},
            "motionTargets": [{"pose": pose, "path": f"{pose}.png"} for pose in REQUIRED_POSES],
            "jointCenters": {
                joint: {"xy": [512, 800], "status": "VISIBLE_REVIEWED"}
                for joint in REQUIRED_JOINTS
            },
            "bodyComponents": components,
            "hiddenSurfaces": [
                "under_arm_torso_edge",
                "sleeve_underlap_area",
                "inside_leg_overlap",
                "lower_garment_contact_zone",
                "foot_sole_alternative",
            ],
            "lowerLegFootPolicy": "rigid_shin_with_separate_foot_endpoint",
            "drawOrder": [component["id"] for component in components],
            "ownershipTable": {component["id"]: component["ownership"] for component in components},
            "visualReview": {"status": "APPROVED", "artifacts": ["review-board.png"]},
        }
        manifest_path = root / "blueprint-manifest.json"
        manifest_path.write_text(json.dumps(manifest), encoding="utf-8")
        return temporary, manifest_path

    def test_accepts_complete_blueprint_package(self):
        temporary, manifest = self._package()
        self.addCleanup(temporary.cleanup)

        report = audit_blueprint_package(manifest)

        self.assertEqual("PASS", report["status"])
        self.assertEqual([], report["failures"])
        self.assertFalse(report["runtimePromotionAllowed"])

    def test_rejects_missing_jump_and_missing_joint_before_runtime_probe(self):
        temporary, manifest = self._package()
        self.addCleanup(temporary.cleanup)
        data = json.loads(manifest.read_text(encoding="utf-8"))
        data["motionTargets"] = [
            target for target in data["motionTargets"] if target["pose"] != "jump_tuck"
        ]
        del data["jointCenters"]["far_ankle"]
        manifest.write_text(json.dumps(data), encoding="utf-8")

        report = audit_blueprint_package(manifest)

        self.assertEqual("REJECT_BLUEPRINT_PACKAGE", report["status"])
        self.assertIn("MISSING_MOTION_TARGET_JUMP_TUCK", report["failures"])
        self.assertIn("MISSING_JOINT_FAR_ANKLE", report["failures"])

    def test_rejects_source_without_real_alpha_or_with_optional_outfit(self):
        temporary, manifest = self._package()
        self.addCleanup(temporary.cleanup)
        data = json.loads(manifest.read_text(encoding="utf-8"))
        data["neutralBody"]["bakedOptionalOutfit"] = True
        write_cutout(manifest.parent / "composite.png", mode="RGB")
        manifest.write_text(json.dumps(data), encoding="utf-8")

        report = audit_blueprint_package(manifest)

        self.assertIn("NEUTRAL_BODY_HAS_BAKED_OPTIONAL_OUTFIT", report["failures"])
        self.assertIn("COMPOSITE_MISSING_ALPHA_CHANNEL", report["failures"])

    def test_rejects_incomplete_hidden_surfaces_and_body_parts(self):
        temporary, manifest = self._package()
        self.addCleanup(temporary.cleanup)
        data = json.loads(manifest.read_text(encoding="utf-8"))
        data["hiddenSurfaces"] = ["under_arm_torso_edge"]
        data["bodyComponents"] = [
            component for component in data["bodyComponents"] if component["id"] != "near_shin_foot"
        ]
        manifest.write_text(json.dumps(data), encoding="utf-8")

        report = audit_blueprint_package(manifest)

        self.assertIn("MISSING_BODY_COMPONENT_NEAR_SHIN_FOOT", report["failures"])
        self.assertIn("MISSING_HIDDEN_SURFACE_FOOT_SOLE_ALTERNATIVE", report["failures"])


if __name__ == "__main__":
    unittest.main()
