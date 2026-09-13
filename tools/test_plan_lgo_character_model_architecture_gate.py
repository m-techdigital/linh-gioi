import copy
import unittest

from plan_lgo_character_model_architecture_gate import plan_architecture_gate


REQUIRED_SLOTS = ["upper", "lower", "footwear", "waist", "rigid_hand_item"]
REQUIRED_ACTIONS = ["idle", "run", "jump", "attack", "return_to_idle"]


def candidate(*, unseen_minutes, operations, source_files, code_files, artifacts, frame_ms, memory_mb, draw_calls):
    return {
        "toolchain": {"status": "READY", "version": "fixture", "provenance": "fixture"},
        "probeStatus": "COMPLETE",
        "commonTask": {
            "neutralSwappableBody": True,
            "slotsCompleted": REQUIRED_SLOTS,
            "actionsCompleted": REQUIRED_ACTIONS,
            "unequipCompleted": True,
            "mixedLoadoutCompleted": True,
            "unseenItemAfterToolingLock": True,
            "oldCombinationsRegressionPassed": True,
        },
        "motionContinuity": {
            "transitionsCompleted": [
                "start_stop_direction_change",
                "run_jump_fall_land_run",
                "run_attack_run",
            ],
            "midKeyframeSamplesReviewed": True,
            "equipmentSwapDuringMotion": True,
            "equipmentSwapPreservedAnimationTime": True,
            "rootScaleStable": True,
            "singleTransformOwner": True,
            "actualVelocityDrivesLocomotion": True,
        },
        "deformationIntegrity": {
            "bodyBoneLengthDriftMaxRatio": 0.0,
            "rigidEdgeLengthDriftMaxRatio": 0.0,
            "rigidSocketDriftMaxSourcePx": 0.5,
            "softTriangleInversionCount": 0,
            "seamGapMaxSourcePx": 1.0,
            "authoredWeightsForDeformingParts": True,
            "sameTimestampBodyFullMixedReviewed": True,
        },
        "manualIntervention": {
            "sourceOperations": operations,
            "perPosePixelEdits": 0,
            "sourceCorrectionRounds": 1,
            "unseenItemMinutes": unseen_minutes,
            "specialPoseAttachments": 1,
        },
        "changeSurface": {
            "sourceFilesChanged": source_files,
            "codeFilesChanged": code_files,
            "generatedArtifacts": artifacts,
        },
        "automation": {
            "deterministicReplay": True,
            "provenanceRecorded": True,
        },
        "visualReview": {"status": "APPROVED", "artifacts": ["fixture.png"]},
        "performance": {
            "pc": {
                "status": "MEASURED",
                "frameTimeP95Ms": frame_ms,
                "memoryMb": memory_mb,
                "drawCalls": draw_calls,
            },
            "mobile": {"status": "DEFERRED_DEVICE_REQUIRED"},
        },
        "hardFailures": [],
    }


def complete_manifest():
    return {
        "gateId": "LGO_CHARACTER_MODEL_ARCHITECTURE_REVIEW_01",
        "baseline": {
            "status": "MEASURED",
            "evidence": ["baseline.json"],
            "metrics": {
                "manualSourceOperations": 30,
                "unseenItemMinutes": 240,
                "sourceCorrectionRounds": 4,
            },
        },
        "candidates": {
            "skeletal_2d": candidate(
                unseen_minutes=40,
                operations=8,
                source_files=3,
                code_files=1,
                artifacts=5,
                frame_ms=7.5,
                memory_mb=24,
                draw_calls=8,
            ),
            "modular_3d": candidate(
                unseen_minutes=70,
                operations=12,
                source_files=5,
                code_files=2,
                artifacts=7,
                frame_ms=9.0,
                memory_mb=32,
                draw_calls=12,
            ),
        },
    }


class CharacterModelArchitectureGateTests(unittest.TestCase):
    def test_missing_measured_baseline_blocks_candidate_work(self):
        manifest = complete_manifest()
        manifest["baseline"] = {"status": "UNMEASURED", "evidence": [], "metrics": {}}

        result = plan_architecture_gate(manifest)

        self.assertEqual(result["status"], "BASELINE_EVIDENCE_REQUIRED")
        self.assertEqual(result["nextAction"], "measure_pose_sprite_baseline")

    def test_ready_toolchains_do_not_count_as_completed_probes(self):
        manifest = complete_manifest()
        for record in manifest["candidates"].values():
            record["probeStatus"] = "NOT_RUN"
            record["commonTask"] = {}
            record["visualReview"] = {"status": "PENDING", "artifacts": []}

        result = plan_architecture_gate(manifest)

        self.assertEqual(result["status"], "RUN_SKELETAL_2D_PROBE")
        self.assertEqual(result["nextCandidate"], "skeletal_2d")
        self.assertFalse(result["candidates"]["skeletal_2d"]["eligible"])
        self.assertFalse(result["candidates"]["modular_3d"]["eligible"])

    def test_owner_rejected_skeletal_source_requires_new_blueprint_not_same_probe(self):
        manifest = complete_manifest()
        skeletal = manifest["candidates"]["skeletal_2d"]
        skeletal["probeStatus"] = "OWNER_REJECTED_SOURCE"
        skeletal["sourceGate"] = {
            "status": "OWNER_REJECTED_VISUAL",
            "requiresNewNeutralLayeredSource": True,
            "evidence": "bind-authority-candidate-v1/review-board.png",
        }
        skeletal["visualReview"] = {
            "status": "REJECTED",
            "artifacts": ["bind-authority-candidate-v1/review-board.png"],
        }
        skeletal["commonTask"] = {}

        result = plan_architecture_gate(manifest)

        self.assertEqual(result["status"], "AUTHOR_SKELETAL_2D_SOURCE_BLUEPRINT")
        self.assertEqual(result["nextAction"], "author_skeletal_2d_neutral_layered_source")
        self.assertEqual(result["nextCandidate"], "skeletal_2d")
        self.assertFalse(result["runtimePromotionAllowed"])

    def test_second_probe_runs_after_skeletal_evidence_is_eligible(self):
        manifest = complete_manifest()
        manifest["candidates"]["modular_3d"]["probeStatus"] = "NOT_RUN"
        manifest["candidates"]["modular_3d"]["commonTask"] = {}

        result = plan_architecture_gate(manifest)

        self.assertEqual(result["status"], "RUN_MODULAR_3D_PROBE")
        self.assertEqual(result["nextCandidate"], "modular_3d")

    def test_per_pose_pixel_edits_make_unseen_item_ineligible(self):
        manifest = complete_manifest()
        manifest["candidates"]["skeletal_2d"]["manualIntervention"]["perPosePixelEdits"] = 1

        result = plan_architecture_gate(manifest)

        skeletal = result["candidates"]["skeletal_2d"]
        self.assertFalse(skeletal["eligible"])
        self.assertIn("zero_per_pose_pixel_edits", skeletal["missingEvidence"])
        self.assertEqual(result["status"], "COMPLETE_COMMON_TASK_EVIDENCE")

    def test_third_source_correction_round_closes_candidate_loop(self):
        manifest = complete_manifest()
        manifest["candidates"]["skeletal_2d"]["manualIntervention"]["sourceCorrectionRounds"] = 3

        result = plan_architecture_gate(manifest)

        skeletal = result["candidates"]["skeletal_2d"]
        self.assertFalse(skeletal["eligible"])
        self.assertTrue(skeletal["closedByAntiLoop"])
        self.assertIn("source_correction_round_limit_exceeded", skeletal["hardFailures"])

    def test_pending_visual_review_prevents_architecture_decision(self):
        manifest = complete_manifest()
        manifest["candidates"]["skeletal_2d"]["visualReview"] = {
            "status": "PENDING",
            "artifacts": ["candidate-board.png"],
        }

        result = plan_architecture_gate(manifest)

        self.assertEqual(result["status"], "NEED_HUMAN_VISUAL_REVIEW")
        self.assertEqual(result["nextCandidate"], "skeletal_2d")
        self.assertFalse(result["candidates"]["skeletal_2d"]["eligible"])

    def test_clip_presence_without_runtime_continuity_evidence_is_not_eligible(self):
        manifest = complete_manifest()
        continuity = manifest["candidates"]["skeletal_2d"]["motionContinuity"]
        continuity["midKeyframeSamplesReviewed"] = False
        continuity["equipmentSwapPreservedAnimationTime"] = False

        result = plan_architecture_gate(manifest)

        skeletal = result["candidates"]["skeletal_2d"]
        self.assertFalse(skeletal["eligible"])
        self.assertIn("mid_keyframe_samples_reviewed", skeletal["missingEvidence"])
        self.assertIn("equipment_swap_preserves_animation_time", skeletal["missingEvidence"])
        self.assertEqual(result["status"], "COMPLETE_COMMON_TASK_EVIDENCE")

    def test_missing_real_velocity_and_transform_ownership_blocks_smoothness_claim(self):
        manifest = complete_manifest()
        continuity = manifest["candidates"]["skeletal_2d"]["motionContinuity"]
        continuity["actualVelocityDrivesLocomotion"] = False
        continuity["singleTransformOwner"] = False

        result = plan_architecture_gate(manifest)

        missing = result["candidates"]["skeletal_2d"]["missingEvidence"]
        self.assertIn("actual_velocity_drives_locomotion", missing)
        self.assertIn("single_transform_owner", missing)

    def test_smooth_motion_cannot_hide_body_stretch_or_rigid_item_warp(self):
        manifest = complete_manifest()
        integrity = manifest["candidates"]["skeletal_2d"]["deformationIntegrity"]
        integrity["bodyBoneLengthDriftMaxRatio"] = 0.02
        integrity["rigidEdgeLengthDriftMaxRatio"] = 0.03
        integrity["softTriangleInversionCount"] = 1
        integrity["authoredWeightsForDeformingParts"] = False

        result = plan_architecture_gate(manifest)

        missing = result["candidates"]["skeletal_2d"]["missingEvidence"]
        self.assertIn("body_bone_length_stability", missing)
        self.assertIn("rigid_shape_preservation", missing)
        self.assertIn("no_soft_mesh_triangle_inversion", missing)
        self.assertIn("authored_weights_for_deforming_parts", missing)

    def test_same_timestamp_body_full_mixed_review_is_required(self):
        manifest = complete_manifest()
        manifest["candidates"]["skeletal_2d"]["deformationIntegrity"][
            "sameTimestampBodyFullMixedReviewed"
        ] = False

        result = plan_architecture_gate(manifest)

        self.assertIn(
            "same_timestamp_body_full_mixed_review",
            result["candidates"]["skeletal_2d"]["missingEvidence"],
        )

    def test_lower_cost_candidate_dominates_without_weighted_score(self):
        result = plan_architecture_gate(complete_manifest())

        self.assertEqual(result["status"], "BENCHMARK_READY_FOR_DECISION")
        self.assertEqual(result["decision"]["status"], "DOMINANT_CANDIDATE")
        self.assertEqual(result["decision"]["dominantCandidate"], "skeletal_2d")
        self.assertNotIn("score", result["decision"])

    def test_crossed_cost_vectors_require_explicit_tradeoff(self):
        manifest = complete_manifest()
        candidate_3d = manifest["candidates"]["modular_3d"]
        candidate_3d["manualIntervention"]["unseenItemMinutes"] = 20
        candidate_3d["manualIntervention"]["sourceOperations"] = 4
        candidate_3d["performance"]["pc"]["frameTimeP95Ms"] = 11.0
        candidate_3d["performance"]["pc"]["memoryMb"] = 48

        result = plan_architecture_gate(manifest)

        self.assertEqual(result["status"], "BENCHMARK_READY_FOR_DECISION")
        self.assertEqual(result["decision"]["status"], "OWNER_TRADEOFF_REQUIRED")
        self.assertIn("unseenItemMinutes", result["decision"]["modular3dAdvantages"])
        self.assertIn("pcFrameTimeP95Ms", result["decision"]["skeletal2dAdvantages"])

    def test_rejected_candidate_can_be_compared_when_other_candidate_is_eligible(self):
        manifest = complete_manifest()
        rejected = manifest["candidates"]["modular_3d"]
        rejected["probeStatus"] = "CLOSED_HARD_FAILURE"
        rejected["hardFailures"] = [
            {"id": "clipping", "reproductionCount": 2, "evidence": "clipping.json"}
        ]
        rejected["visualReview"] = {"status": "REJECTED", "artifacts": ["clipping.png"]}

        result = plan_architecture_gate(manifest)

        self.assertEqual(result["status"], "BENCHMARK_READY_FOR_DECISION")
        self.assertEqual(result["decision"]["status"], "ONLY_ELIGIBLE_CANDIDATE")
        self.assertEqual(result["decision"]["dominantCandidate"], "skeletal_2d")


if __name__ == "__main__":
    unittest.main()
