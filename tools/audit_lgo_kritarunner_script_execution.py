#!/usr/bin/env python3
"""Audit whether local Krita's kritarunner can execute a Python script.

This is intentionally an evidence tool, not a workaround. It records each
runner invocation and only reports pass when the script itself writes the
expected marker JSON.
"""

from __future__ import annotations

import argparse
import json
import os
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Iterable


DEFAULT_ATTEMPTS = [
    ("stem-cwd", "{stem}"),
    ("absolute-no-py", "{abs_no_py}"),
    ("absolute-with-py", "{abs_with_py}"),
    ("stem-pythonpath", "{stem}"),
]


def tail(text: str, limit: int = 4000) -> str:
    if len(text) <= limit:
        return text
    return text[-limit:]


def render_script_arg(template: str, script_path: Path) -> str:
    return template.format(
        stem=script_path.stem,
        abs_no_py=str(script_path.with_suffix("")),
        abs_with_py=str(script_path),
    )


def read_json_or_none(path: Path):
    if not path.exists():
        return None
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        return {"status": "INVALID_JSON_MARKER", "error": str(exc)}


def audit_kritarunner(
    runner_path: Path,
    script_path: Path,
    output_dir: Path,
    *,
    timeout_seconds: int = 25,
    attempts: Iterable[tuple[str, str]] = DEFAULT_ATTEMPTS,
):
    runner_path = Path(runner_path).expanduser().resolve()
    script_path = Path(script_path).expanduser().resolve()
    output_dir = Path(output_dir).expanduser().resolve()
    output_dir.mkdir(parents=True, exist_ok=True)

    report = {
        "status": "KRITARUNNER_SCRIPT_IMPORT_UNRESOLVED",
        "runtimePromotionAllowed": False,
        "scriptExecutionProven": False,
        "runner": str(runner_path),
        "script": str(script_path),
        "outputDir": str(output_dir),
        "timeoutSeconds": timeout_seconds,
        "attempts": [],
        "createdAt": datetime.now(timezone.utc).isoformat(),
    }

    if not runner_path.exists():
        report["status"] = "KRITARUNNER_MISSING"
        report["failures"] = [f"runner missing: {runner_path}"]
        return report
    if not script_path.exists():
        report["status"] = "KRITARUNNER_SCRIPT_MISSING"
        report["failures"] = [f"script missing: {script_path}"]
        return report

    for label, template in attempts:
        marker = output_dir / f"{label}-marker.json"
        script_arg = render_script_arg(template, script_path)
        cmd = [str(runner_path), "-s", script_arg, str(marker)]
        env = os.environ.copy()
        if label.endswith("pythonpath"):
            old = env.get("PYTHONPATH")
            env["PYTHONPATH"] = str(script_path.parent) if not old else f"{script_path.parent}{os.pathsep}{old}"
        try:
            completed = subprocess.run(
                cmd,
                cwd=script_path.parent if label.endswith("cwd") else None,
                env=env,
                text=True,
                capture_output=True,
                timeout=timeout_seconds,
            )
            exit_code = completed.returncode
            stdout = completed.stdout
            stderr = completed.stderr
            timed_out = False
        except subprocess.TimeoutExpired as exc:
            exit_code = None
            stdout = exc.stdout or ""
            stderr = exc.stderr or ""
            timed_out = True

        marker_json = read_json_or_none(marker)
        attempt_report = {
            "label": label,
            "command": cmd,
            "cwd": str(script_path.parent) if label.endswith("cwd") else None,
            "exitCode": exit_code,
            "timedOut": timed_out,
            "stdoutTail": tail(stdout),
            "stderrTail": tail(stderr),
            "marker": str(marker),
            "markerExists": marker.exists(),
            "markerJson": marker_json,
        }
        report["attempts"].append(attempt_report)

        if marker_json and marker_json.get("status") == "KRITARUNNER_SCRIPT_EXECUTED":
            report["status"] = "KRITARUNNER_SCRIPT_EXECUTION_PASS"
            report["scriptExecutionProven"] = True
            report["passingAttempt"] = label
            break

    if not report["scriptExecutionProven"]:
        stderr_joined = "\n".join(a["stderrTail"] for a in report["attempts"])
        if "ModuleNotFoundError" in stderr_joined:
            report["status"] = "KRITARUNNER_SCRIPT_IMPORT_UNRESOLVED"
        elif any(a["timedOut"] for a in report["attempts"]):
            report["status"] = "KRITARUNNER_SCRIPT_EXECUTION_TIMEOUT"
        else:
            report["status"] = "KRITARUNNER_SCRIPT_EXECUTION_UNPROVEN"

    return report


def parse_args(argv):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--runner", required=True, type=Path)
    parser.add_argument("--script", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--report", required=True, type=Path)
    parser.add_argument("--timeout-seconds", type=int, default=25)
    return parser.parse_args(argv)


def main(argv=None):
    args = parse_args(argv or sys.argv[1:])
    report = audit_kritarunner(
        args.runner,
        args.script,
        args.output_dir,
        timeout_seconds=args.timeout_seconds,
    )
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({"status": report["status"], "scriptExecutionProven": report["scriptExecutionProven"]}, ensure_ascii=False))
    return 0 if report["status"] in {"KRITARUNNER_SCRIPT_EXECUTION_PASS", "KRITARUNNER_SCRIPT_IMPORT_UNRESOLVED", "KRITARUNNER_SCRIPT_EXECUTION_UNPROVEN"} else 1


if __name__ == "__main__":
    raise SystemExit(main())
