import sys
import hashlib
import json
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from capture_lgo_registered_outfit import validate_registered_capture_result, validate_pose_review_pack, validate_pose_review_log, pose_review_fingerprint, validate_pose_review_unchanged, player_code_fingerprint


class RegisteredOutfitCaptureValidationTests(unittest.TestCase):
    def test_managed_code_change_is_detected_with_identical_engine(self):
        with tempfile.TemporaryDirectory() as directory:
            contents = Path(directory) / 'Game.app/Contents'
            player = contents / 'MacOS/Unity'
            assembly = contents / 'Resources/Data/Managed/LinhGioi.World.dll'
            player.parent.mkdir(parents=True)
            assembly.parent.mkdir(parents=True)
            player.write_bytes(b'same Unity engine')
            assembly.write_bytes(b'old capture code')
            before = player_code_fingerprint(player)
            assembly.write_bytes(b'fixed HUD and animation clock')
            after = player_code_fingerprint(player)
            self.assertEqual(before['MacOS/Unity'], after['MacOS/Unity'])
            self.assertNotEqual(before, after)

    def valid_result(self):
        return {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "frames": 154,
            "basePoseFrames": 30,
            "actionTransitions": 20,
            "heldJumpRestarts": 4,
            "toggles": 40,
            "width": 1280,
            "height": 720,
            "errors": [],
            "actorScreenMetricFrames": 154,
            "minActorScreenHeightRatio": 0.18,
            "maxActorScreenHeightRatio": 0.24,
        }

    def matrix_result(self):
        result = self.valid_result()
        core = ('inner_top', 'outer_tunic', 'lower_garment', 'waist')
        result.update(frames=186, actorScreenMetricFrames=186, registeredEquipment=True,
                      closedBody=True, maxEquipmentAttachments=17, maxBodyVariants=1)
        result['wardrobeCombinations'] = [
            dict(gender=gender, bits=bits, frame=1 + index * 16 + bits,
                 file=f'{1 + index * 16 + bits:02d}-{gender}-wardrobe-{bits:02d}.png',
                 enabledCore=[slot for bit, slot in enumerate(core) if bits & (1 << bit)])
            for index, gender in enumerate(('male', 'female')) for bits in range(16)]
        return result

    def test_matrix_requires_unique_images_and_actual_equipment_states(self):
        def check(result):
            return validate_registered_capture_result(code=0, result=result, width=1280,
                height=720, png_count=186, registered_equipment=True, wardrobe_matrix=True)
        self.assertEqual(check(self.matrix_result()), [])
        for defect in ('missing', 'duplicate', 'frame_reuse', 'wrong_equipment', 'image_reuse', 'non_png'):
            result = self.matrix_result()
            rows = result['wardrobeCombinations']
            if defect == 'missing': rows.pop()
            elif defect == 'duplicate': rows[-1] = dict(rows[0])
            elif defect == 'frame_reuse': rows[-1]['frame'] = rows[0]['frame']
            elif defect == 'image_reuse': rows[-1]['file'] = rows[0]['file']
            elif defect == 'non_png': rows[0]['file'] = 'player.log'
            else: rows[0]['enabledCore'] = ['inner_top']
            with self.subTest(defect=defect): self.assertTrue(check(result))

    def test_accepts_capture_when_actor_screen_metrics_cover_every_frame(self):
        self.assertEqual(
            validate_registered_capture_result(
                code=0,
                result=self.valid_result(),
                width=1280,
                height=720,
                png_count=154,
                closed_far_arms=False,
                closed_body=False,
                registered_equipment=False,
            ),
            [],
        )

    def test_rejects_capture_without_actor_screen_metrics(self):
        result = self.valid_result()
        result.pop("actorScreenMetricFrames")
        result.pop("minActorScreenHeightRatio")
        result.pop("maxActorScreenHeightRatio")

        errors = validate_registered_capture_result(
            code=0,
            result=result,
            width=1280,
            height=720,
            png_count=154,
            closed_far_arms=False,
            closed_body=False,
            registered_equipment=False,
        )

        self.assertIn("ACTOR_SCREEN_METRIC_FRAMES_MISSING", errors)
        self.assertIn("ACTOR_SCREEN_HEIGHT_RATIO_MISSING", errors)

    def test_rejects_capture_when_actor_screen_metrics_are_not_for_all_frames(self):
        result = self.valid_result()
        result["actorScreenMetricFrames"] = 153
        result["minActorScreenHeightRatio"] = 0.0
        result["maxActorScreenHeightRatio"] = 0.17

        errors = validate_registered_capture_result(
            code=0,
            result=result,
            width=1280,
            height=720,
            png_count=154,
            closed_far_arms=False,
            closed_body=False,
            registered_equipment=False,
        )

        self.assertIn("ACTOR_SCREEN_METRIC_FRAMES_INCOMPLETE", errors)
        self.assertIn("ACTOR_SCREEN_HEIGHT_RATIO_INVALID", errors)


class PoseReviewCaptureValidationTests(unittest.TestCase):
    def setUp(self):
        temp = tempfile.TemporaryDirectory()
        self.addCleanup(temp.cleanup)
        self.root = Path(temp.name)
        atlas = b'fingerprinted-atlas-fixture'
        (self.root / 'atlas-review.png').write_bytes(atlas)
        self.pack = {'status': 'REVIEW_ONLY', 'runtimeEligible': False, 'samplingDivisor': 4,
                     'pngBytes': len(atlas), 'atlasSha256': hashlib.sha256(atlas).hexdigest(),
                     'sprites': [{'id': name} for name in ('idle', 'run_a', 'run_b', 'jump_tuck')]}

    def write_pack(self):
        (self.root / 'atlas-review.json').write_text(json.dumps(self.pack))

    def test_review_pack_requires_div4_and_matching_atlas(self):
        self.write_pack()
        self.assertEqual(validate_pose_review_pack(self.root)['samplingDivisor'], 4)
        self.pack['samplingDivisor'] = 8
        self.write_pack()
        with self.assertRaisesRegex(ValueError, 'div4'):
            validate_pose_review_pack(self.root)
        self.pack['samplingDivisor'] = 4
        self.write_pack()
        (self.root / 'atlas-review.png').write_bytes(b'changed')
        with self.assertRaisesRegex(ValueError, 'fingerprint'):
            validate_pose_review_pack(self.root)

    def test_capture_rejects_manifest_or_atlas_changes(self):
        self.write_pack()
        expected = pose_review_fingerprint(self.root)
        validate_pose_review_unchanged(self.root, expected)
        for name in ('atlas-review.json', 'atlas-review.png'):
            with self.subTest(file=name):
                path = self.root / name
                original = path.read_bytes()
                path.write_bytes(original + b' ')
                with self.assertRaisesRegex(ValueError, 'changed during capture'):
                    validate_pose_review_unchanged(self.root, expected)
                path.write_bytes(original)

    def test_review_pack_cannot_be_promoted(self):
        self.pack['runtimeEligible'] = True
        self.write_pack()
        with self.assertRaisesRegex(ValueError, 'REVIEW_ONLY'):
            validate_pose_review_pack(self.root)

    def test_log_requires_exact_loaded_path_and_every_exact_pose(self):
        lines = ['LGO_POSE_REVIEW_LOADED ' + str(self.root.resolve())]
        lines += ['LGO_POSE_REVIEW_FRAME ' + p['id'] for p in self.pack['sprites']]
        self.assertEqual(validate_pose_review_log('\n'.join(lines), self.root, self.pack),
                         ['idle', 'jump_tuck', 'run_a', 'run_b'])
        with self.assertRaisesRegex(ValueError, 'loaded path'):
            validate_pose_review_log('\n'.join(lines[1:]), self.root, self.pack)
        lines[-1] += '_wrong'
        with self.assertRaisesRegex(ValueError, 'jump_tuck'):
            validate_pose_review_log('\n'.join(lines), self.root, self.pack)


if __name__ == "__main__":
    unittest.main()
