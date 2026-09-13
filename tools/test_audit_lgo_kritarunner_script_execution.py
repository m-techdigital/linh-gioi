import json
import os
import tempfile
import unittest
from pathlib import Path

from audit_lgo_kritarunner_script_execution import audit_kritarunner


class KritaRunnerScriptExecutionAuditTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.runner = self.root / "fake_kritarunner.py"
        self.script = self.root / "probe_script.py"
        self.script.write_text("probe", encoding="utf-8")

    def write_runner(self, body):
        self.runner.write_text(
            "#!/usr/bin/env python3\n"
            "import json, pathlib, sys\n"
            + body,
            encoding="utf-8",
        )
        self.runner.chmod(0o755)

    def test_reports_pass_when_runner_writes_attempt_marker(self):
        self.write_runner(
            "\nmarker = pathlib.Path(sys.argv[-1])\n"
            "marker.write_text(json.dumps({'status':'KRITARUNNER_SCRIPT_EXECUTED'}), encoding='utf-8')\n"
        )

        report = audit_kritarunner(
            self.runner,
            self.script,
            self.root / "out",
            timeout_seconds=5,
            attempts=[("stem-cwd", "{stem}")],
        )

        self.assertEqual(report["status"], "KRITARUNNER_SCRIPT_EXECUTION_PASS")
        self.assertTrue(report["scriptExecutionProven"])
        self.assertEqual(report["attempts"][0]["exitCode"], 0)

    def test_reports_import_unresolved_when_runner_cannot_import_script(self):
        self.write_runner(
            "\nsys.stderr.write(\"ModuleNotFoundError: No module named 'probe_script'\\n\")\n"
            "raise SystemExit(1)\n"
        )

        report = audit_kritarunner(
            self.runner,
            self.script,
            self.root / "out",
            timeout_seconds=5,
            attempts=[("stem-cwd", "{stem}")],
        )

        self.assertEqual(report["status"], "KRITARUNNER_SCRIPT_IMPORT_UNRESOLVED")
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertFalse(report["scriptExecutionProven"])
        self.assertIn("ModuleNotFoundError", report["attempts"][0]["stderrTail"])

    def test_relative_runner_path_still_works_when_attempt_changes_cwd(self):
        self.write_runner(
            "\nmarker = pathlib.Path(sys.argv[-1])\n"
            "marker.write_text(json.dumps({'status':'KRITARUNNER_SCRIPT_EXECUTED'}), encoding='utf-8')\n"
        )
        old_cwd = Path.cwd()
        try:
            os.chdir(self.root)
            report = audit_kritarunner(
                Path("fake_kritarunner.py"),
                Path("probe_script.py"),
                Path("out-relative"),
                timeout_seconds=5,
                attempts=[("stem-cwd", "{stem}")],
            )
        finally:
            os.chdir(old_cwd)

        self.assertEqual(report["status"], "KRITARUNNER_SCRIPT_EXECUTION_PASS")
        self.assertTrue(report["scriptExecutionProven"])


if __name__ == "__main__":
    unittest.main()
