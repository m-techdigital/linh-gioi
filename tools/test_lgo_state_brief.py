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

    def test_active_task_state_overrides_historical_next_task(self):
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

        self.assertIn("OUTFIT_BODY_RIG_SOURCE_PROTOTYPE", next_task_section(text))
        self.assertNotIn("Stale inventory", next_task_section(text))
        self.assertIn("PLAYER_BUILD_BLOCKED_BY_URP_LIT_SHADER_COMPILE", current_blocker_section(text))


if __name__ == "__main__":
    unittest.main()
