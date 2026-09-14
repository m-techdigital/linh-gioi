import unittest

from lgo_state_brief import current_blocker_section, next_task_section, resume_section


class LgoStateBriefTests(unittest.TestCase):
    def test_active_goal_lock_overrides_stale_quick_resume_and_next_task(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Active six-pose task.",
                "",
                "Next valid work: author missing source targets.",
                "",
                "## Quick Resume",
                "",
                "Stale Map01A task.",
                "",
                "## Next task",
                "",
                "Stale inventory task.",
            ]
        )

        self.assertIn("Active six-pose task", resume_section(text))
        self.assertNotIn("Stale Map01A", resume_section(text))
        self.assertNotIn("Next valid work:", resume_section(text))
        self.assertIn("author missing source targets", next_task_section(text))
        self.assertNotIn("Stale inventory", next_task_section(text))

    def test_active_goal_lock_suppresses_stale_current_blocker(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Next valid work: author missing source targets.",
                "",
                "## Current blocker",
                "",
                "Stale Map01A blocker.",
            ]
        )

        blocker = current_blocker_section(text)

        self.assertIn("No current blocker from active lock", blocker)
        self.assertNotIn("Stale Map01A blocker", blocker)

    def test_active_goal_lock_reports_visual_polish_after_layer_coverage_complete(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Current visual status remains SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE.",
                "",
                "Next valid work: execute source visual polish, not another missing-target authoring loop.",
            ]
        )

        blocker = current_blocker_section(text)

        self.assertIn("source visual polish", blocker)
        self.assertIn("SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE", blocker)
        self.assertNotIn("source-authoring task", blocker)

    def test_active_goal_lock_reports_surface_contract_route_decision_first(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Current visual status remains SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE.",
                "Surface contract validation is NEED_OWNER_DECISION with ROUTE_SELECTION_REQUIRED.",
            ]
        )

        blocker = current_blocker_section(text)

        self.assertIn("surface contract route decision", blocker)
        self.assertIn("ROUTE_SELECTION_REQUIRED", blocker)
        self.assertNotIn("source visual polish", blocker)

    def test_owner_stopped_active_task_is_reported_as_blocked(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Owner steering update.",
                "",
                "## Active task state",
                "",
                "```json",
                '{"activeTask":"OUTFIT_BODY_RIG_SOURCE_PROTOTYPE","status":"SOURCE_CREATED_PLAYER_BUILD_BLOCKED","blockers":["PLAYER_BUILD_BLOCKED_BY_URP_LIT_SHADER_COMPILE"]}',
                "```",
                "",
                "## Historical context kept for provenance",
                "",
                "## Next task",
                "",
                "Stale inventory task.",
            ]
        )

        self.assertIn("OWNER_STOPPED_PATH_REVIEW_REQUIRED", next_task_section(text))
        self.assertNotIn("Stale inventory", next_task_section(text))
        self.assertIn("OWNER_STOPPED_PATH", current_blocker_section(text))

    def test_human_visual_review_status_is_reported_as_current_gate(self):
        text = """## ACTIVE GOAL LOCK — two whole-pose six-frame character bases

Current review candidates are ready.

## Active task state

```json
{"activeTask":"LGO_CHARACTER_BASE_SIX_POSE_REBUILD_01","status":"NEED_HUMAN_VISUAL_REVIEW"}
```
"""

        blocker = current_blocker_section(text)

        self.assertIn("NEED_HUMAN_VISUAL_REVIEW", blocker)
        self.assertNotIn("No current blocker", blocker)

    def test_chibi_concept_visual_review_does_not_describe_stale_six_pose_boards(self):
        text = """## ACTIVE GOAL LOCK — purpose-designed mini chibi modular concept

Concept candidate is ready.

## Active task state

```json
{"activeTask":"LGO_CHARACTER_CHIBI_MODULAR_CONCEPT_01","phase":"OWNER_CONCEPT_REVIEW","status":"NEED_HUMAN_VISUAL_REVIEW"}
```
"""

        blocker = current_blocker_section(text)

        self.assertIn("mini chibi modular concept", blocker)
        self.assertNotIn("paired six-pose", blocker)

    def test_chibi_motion_review_names_current_artifacts_and_runtime_lock(self):
        text = """## ACTIVE GOAL LOCK — chibi true-side modular motion proof

## Active task state

```json
{"activeTask":"LGO_CHIBI_SIDE_MODULAR_MOTION_PROOF_01","phase":"ARTICULATED_MOTION_CANDIDATE_OWNER_REVIEW","status":"NEED_HUMAN_VISUAL_REVIEW"}
```
"""

        blocker = current_blocker_section(text)

        self.assertIn("Blender MP4", blocker)
        self.assertIn("front-tuck", blocker)
        self.assertIn("runtime promotion remains locked", blocker)
        self.assertNotIn("paired six-pose", blocker)

    def test_owner_decision_status_is_reported_as_current_gate(self):
        text = """## ACTIVE GOAL LOCK — two whole-pose six-frame character bases

Pose control source is ready but art transfer is blocked.

## Active task state

```json
{"activeTask":"LGO_CHARACTER_BASE_SIX_POSE_REBUILD_01","phase":"POSE_CONTROL_READY_ART_TRANSFER_BLOCKED","status":"NEED_OWNER_DECISION"}
```
"""

        blocker = current_blocker_section(text)

        self.assertIn("NEED_OWNER_DECISION", blocker)
        self.assertIn("art transfer", blocker)
        self.assertNotIn("No current blocker", blocker)

    def test_rigid_outfit_visual_failure_routes_to_source_reauthor(self):
        text = """## ACTIVE GOAL LOCK — Unity rigid outfit pilot visual-first

## Active task state

```json
{"activeTask":"LGO_RIGID_OUTFIT_PILOT_01","phase":"RIGID_NATIVE_SOURCE_REAUTHOR_REQUIRED","status":"FIX_REQUIRED_VISUAL_OUTFIT1"}
```
"""

        blocker = current_blocker_section(text)

        self.assertIn("FIX_REQUIRED_VISUAL_OUTFIT1", blocker)
        self.assertIn("rigid-native bind source", blocker)
        self.assertIn("outfit #2", blocker)

    def test_rigid_outfit_final_one_stops_for_owner_review(self):
        text = """## ACTIVE GOAL LOCK — Unity rigid outfit pilot visual-first

## Active task state

```json
{"activeTask":"LGO_RIGID_OUTFIT_PILOT_01","phase":"FINAL_OUTFIT1_V3_READY_FOR_OWNER_REVIEW","status":"NEED_HUMAN_VISUAL_REVIEW"}
```
"""

        blocker = current_blocker_section(text)

        self.assertIn("NEED_HUMAN_VISUAL_REVIEW", blocker)
        self.assertIn("final-1-combo-review-v3", blocker)
        self.assertIn("Stop before outfit #2", blocker)


if __name__ == "__main__":
    unittest.main()
