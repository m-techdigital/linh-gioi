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


if __name__ == "__main__":
    unittest.main()
