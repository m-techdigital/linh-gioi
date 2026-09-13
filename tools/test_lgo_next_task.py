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


if __name__ == "__main__":
    unittest.main()
