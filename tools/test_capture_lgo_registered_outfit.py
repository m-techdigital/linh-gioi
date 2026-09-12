import sys
import hashlib
import json
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from capture_lgo_registered_outfit import validate_registered_capture_result, validate_pose_review_pack, validate_pose_review_log, pose_review_fingerprint, validate_pose_review_unchanged, player_code_fingerprint


class RegisteredOutfitCaptureValidationTests(unittest.TestCase):
    def test_capture_requires_owner_selected_source(self):
        import capture_lgo_registered_outfit as capture
        from unittest.mock import patch
        approved = {
            'atlas-review.json': '6f78205aa3d1b2f2f4ea7fc43d7abe39a6fb2a7dec93ec5ee28ab997b4bfd98e',
            'atlas-review.png': '27630a5ceece2500e412620b70cf43e61a80bbef5d4391ae450d3d19d6829010',
        }
        with patch.object(capture, 'pose_review_fingerprint', return_value=approved):
            capture.validate_owner_pose_source(Path('copy-of-approved-pack'))
        with patch.object(capture, 'pose_review_fingerprint', return_value={
                **approved,
                'outer-top-review/atlas-review.json': 'overlay-manifest',
                'outer-top-review/atlas-review.png': 'overlay-atlas',
        }):
            capture.validate_owner_pose_source(Path('approved-pack-with-review-overlay'))
        for changed in approved:
            fingerprint = dict(approved, **{changed: 'different'})
            with self.subTest(changed=changed), patch.object(capture, 'pose_review_fingerprint', return_value=fingerprint):
                with self.assertRaisesRegex(ValueError, 'owner-selected'):
                    capture.validate_owner_pose_source(Path('unapproved-pack'))

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

    def test_source_pose_capture_requires_metrics_from_visible_source_actor_and_stable_scale(self):
        result = self.valid_result()
        result['actorFrameMetrics'] = [dict(actor='source_pose', rootScaleX=1.0, rootScaleY=1.0,
                                            file=f'{index:02d}-other.png', poseFrame='idle')
                                       for index in range(result['frames'])]
        expected = ('run_contact_a', 'run_a', 'run_contact_b', 'run_b')
        for offset, (gender, action) in enumerate((('male', 'walk'), ('male', 'run'),
                                                   ('female', 'walk'), ('female', 'run'))):
            for phase, pose in enumerate(expected):
                result['actorFrameMetrics'][offset * 4 + phase].update(
                    file=f'{offset * 4 + phase + 1:02d}-{gender}-{action}-phase-{phase}.png', poseFrame=pose)
        self.assertEqual(validate_registered_capture_result(code=0, result=result, width=1280,
            height=720, png_count=154, source_pose_review=True), [])
        result['actorFrameMetrics'][8]['actor'] = 'registered_outfit'
        self.assertIn('SOURCE_POSE_METRICS_MEASURED_WRONG_ACTOR', validate_registered_capture_result(
            code=0, result=result, width=1280, height=720, png_count=154, source_pose_review=True))
        result['actorFrameMetrics'][8]['actor'] = 'source_pose'
        result['actorFrameMetrics'][19]['rootScaleY'] = 1.2
        self.assertIn('SOURCE_POSE_ROOT_SCALE_CHANGED', validate_registered_capture_result(
            code=0, result=result, width=1280, height=720, png_count=154, source_pose_review=True))

    def test_source_pose_capture_rejects_duplicate_or_missing_four_beat_frame(self):
        result = self.valid_result()
        expected = ('run_contact_a', 'run_a', 'run_contact_b', 'run_b')
        result['actorFrameMetrics'] = [dict(actor='source_pose', rootScaleX=1.0, rootScaleY=1.0,
                                            file=f'{index:02d}-other.png', poseFrame='idle')
                                       for index in range(result['frames'])]
        for offset, (gender, action) in enumerate((('male', 'walk'), ('male', 'run'),
                                                   ('female', 'walk'), ('female', 'run'))):
            for phase, pose in enumerate(expected):
                result['actorFrameMetrics'][offset * 4 + phase].update(
                    file=f'{offset * 4 + phase + 1:02d}-{gender}-{action}-phase-{phase}.png', poseFrame=pose)
        result['actorFrameMetrics'][6]['poseFrame'] = 'run_a'
        self.assertIn('SOURCE_POSE_FOUR_BEAT_SEQUENCE_INVALID:male:run',
                      validate_registered_capture_result(code=0, result=result, width=1280,
                          height=720, png_count=154, source_pose_review=True))

    def test_variant_capture_requires_full_level_and_mixed_switches(self):
        result = self.valid_result()
        result.update(poseReviewLv10Verified=True, poseReviewFullLevelsVerified=[1, 10],
                      poseReviewFullLevelFrames=[2, 3], poseReviewMixedVerified=True,
                      frames=156, actorScreenMetricFrames=156,
                      poseReviewVariantSwitches=20)
        self.assertEqual(validate_registered_capture_result(code=0, result=result, width=1280,
            height=720, png_count=156, pose_review_variant_levels={10}), [])
        result['poseReviewMixedVerified'] = False
        self.assertIn('POSE_REVIEW_MIXED_NOT_VERIFIED', validate_registered_capture_result(
            code=0, result=result, width=1280, height=720, png_count=156,
            pose_review_variant_levels={10}))

    def test_variant_capture_accepts_full_level_frames_for_both_genders(self):
        result = self.valid_result()
        result.update(poseReviewLv10Verified=True, poseReviewFullLevelsVerified=[1, 10],
                      poseReviewFullLevelFrames=[2, 3, 75, 76], poseReviewMixedVerified=True,
                      frames=158, actorScreenMetricFrames=158,
                      poseReviewVariantSwitches=40)
        self.assertEqual(validate_registered_capture_result(code=0, result=result,
            width=1280, height=720, png_count=158, pose_review_variant_levels={10},
            pose_review_variant_gender_count=2), [])

    def test_four_tier_capture_requires_lv20_lv30_and_full_switch_matrix(self):
        result = self.valid_result()
        result.update(poseReviewLv10Verified=True, poseReviewLv20Verified=True,
                      poseReviewLv30Verified=True, poseReviewMixedVerified=True,
                      poseReviewFullLevelsVerified=[1, 10, 20, 30],
                      poseReviewFullLevelFrames=[2, 3, 4, 5], frames=158,
                      actorScreenMetricFrames=158, poseReviewVariantSwitches=40)
        self.assertEqual(validate_registered_capture_result(code=0, result=result, width=1280,
            height=720, png_count=158, pose_review_variant_levels={10, 20, 30}), [])
        result['poseReviewFullLevelsVerified'] = [1, 10, 20]
        result['poseReviewVariantSwitches'] = 29
        errors = validate_registered_capture_result(code=0, result=result, width=1280,
            height=720, png_count=158, pose_review_variant_levels={10, 20, 30})
        self.assertIn('POSE_REVIEW_FULL_LEVEL_MATRIX_NOT_VERIFIED', errors)
        self.assertIn('POSE_REVIEW_VARIANT_SWITCH_COUNT_MISMATCH', errors)

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

    def test_capture_fingerprints_optional_outer_top_overlay(self):
        self.write_pack()
        overlay = self.root / 'outer-top-review'
        overlay.mkdir()
        (overlay / 'atlas-review.png').write_bytes(b'outer-top-atlas')
        (overlay / 'atlas-review.json').write_text(json.dumps({
            'status': 'REVIEW_ONLY', 'runtimeEligible': False, 'samplingDivisor': 4,
            'reviewSlot': 'outer_top',
        }))
        expected = pose_review_fingerprint(self.root)
        self.assertEqual(set(expected), {
            'atlas-review.json', 'atlas-review.png',
            'outer-top-review/atlas-review.json', 'outer-top-review/atlas-review.png',
        })
        (overlay / 'atlas-review.png').write_bytes(b'changed-outer-top-atlas')
        with self.assertRaisesRegex(ValueError, 'changed during capture'):
            validate_pose_review_unchanged(self.root, expected)

    def test_capture_fingerprints_every_present_ten_slot_directory(self):
        self.write_pack()
        for subdir in ('outer-top-review', 'waist-belt-review'):
            overlay = self.root / subdir
            overlay.mkdir()
            (overlay / 'atlas-review.png').write_bytes(subdir.encode())
            (overlay / 'atlas-review.json').write_text('{}')
        names = set(pose_review_fingerprint(self.root))
        self.assertIn('outer-top-review/atlas-review.png', names)
        self.assertIn('waist-belt-review/atlas-review.png', names)
        self.assertEqual(len(names), 6)

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

    def test_log_requires_optional_outer_top_overlay_to_load(self):
        self.write_pack()
        overlay = self.root / 'outer-top-review'
        overlay.mkdir()
        (overlay / 'atlas-review.json').write_text('{}')
        (overlay / 'atlas-review.png').write_bytes(b'overlay')
        lines = ['LGO_POSE_REVIEW_LOADED ' + str(self.root.resolve())]
        lines += ['LGO_POSE_REVIEW_FRAME ' + p['id'] for p in self.pack['sprites']]
        with self.assertRaisesRegex(ValueError, 'outer_top'):
            validate_pose_review_log('\n'.join(lines), self.root, self.pack)
        lines.append('LGO_POSE_REVIEW_OVERLAY_LOADED outer_top ' + str(overlay.resolve()))
        self.assertEqual(validate_pose_review_log('\n'.join(lines), self.root, self.pack),
                         ['idle', 'jump_tuck', 'run_a', 'run_b'])


if __name__ == "__main__":
    unittest.main()
