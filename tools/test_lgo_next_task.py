import unittest

from lgo_next_task import active_next_action_task_from_text


class LgoNextTaskTests(unittest.TestCase):
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


if __name__ == "__main__":
    unittest.main()
