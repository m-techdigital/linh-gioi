import unittest

from lgo_next_task import active_next_action_task_from_text, active_state_execution_blocker_from_text


class LgoNextTaskTests(unittest.TestCase):
    def test_native_authoring_capability_blocker_stops_ready_advice(self):
        text = """## Active task state

```json
{"activeTask":"SIX_POSE_REGISTERED_OUTFIT_POSE_SET_AUTHORING","blockers":["KRITA_AUTOMATED_REOPEN_EXPORT_BLOCKED","ACCEPTED_SLEEVED_SOURCE_MISSING"]}
```
"""

        self.assertEqual(
            active_state_execution_blocker_from_text(text),
            "KRITA_AUTOMATED_REOPEN_EXPORT_BLOCKED",
        )

    def test_active_goal_lock_selects_six_pose_source_authoring(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Next valid work: execute Task 2 Step 2 from the active plan.",
                "",
                "## Next task",
                "",
                "`LGO-TASK-001`",
            ]
        )

        self.assertEqual(
            active_next_action_task_from_text(text),
            "SIX_POSE_REGISTERED_OUTFIT_SOURCE_AUTHORING",
        )

    def test_layer_coverage_complete_selects_source_visual_polish(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Layer coverage completion checkpoint: audit is SOURCE_REPAIR_LAYER_READY_FOR_VISUAL_BOARD with failureCount 0.",
                "Current visual status remains SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE.",
                "Do not claim final item or Player promotion from this checkpoint.",
            ]
        )

        self.assertEqual(
            active_next_action_task_from_text(text),
            "SIX_POSE_REGISTERED_OUTFIT_SOURCE_VISUAL_POLISH",
        )

    def test_surface_contract_decision_gate_overrides_visual_polish(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Current visual status remains SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE.",
                "Surface contract validation is NEED_OWNER_DECISION with ROUTE_SELECTION_REQUIRED.",
            ]
        )

        self.assertEqual(
            active_next_action_task_from_text(text),
            "SIX_POSE_REGISTERED_OUTFIT_SURFACE_CONTRACT_DECISION",
        )

    def test_owner_stopped_active_task_cannot_override_six_pose_lock(self):
        text = "\n".join(
            [
                "## ACTIVE GOAL LOCK — six-pose registered outfit path, 2026-09-13",
                "",
                "Historical note: Surface contract validation was NEED_OWNER_DECISION with ROUTE_SELECTION_REQUIRED.",
                "",
                "## Active task state",
                "",
                '```json',
                '{"activeTask":"OUTFIT_BODY_RIG_SOURCE_PROTOTYPE","blockers":[]}',
                '```',
            ]
        )

        self.assertEqual(
            active_next_action_task_from_text(text),
            "OWNER_STOPPED_PATH_REVIEW_REQUIRED",
        )


if __name__ == "__main__":
    unittest.main()
