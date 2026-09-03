#!/usr/bin/env python3
from __future__ import annotations

import hashlib
import importlib.util
import os
import shutil
import stat
import subprocess
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
WRAPPER = ROOT / "tools" / "protocol_codegen.sh"
EXPECTED_VERSION = "libprotoc 3.13.0"


def run_wrapper(*args: str, env: dict[str, str] | None = None) -> subprocess.CompletedProcess[str]:
    merged = os.environ.copy()
    if env:
        merged.update(env)
    return subprocess.run(
        [str(WRAPPER), *args],
        cwd=ROOT,
        env=merged,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def pinned_protoc_env_or_empty() -> dict[str, str]:
    """Return the environment needed to reach a checksum-pinned protoc on this host.

    Linux x86_64 sandboxes use the bundled compiler with no override. Local macOS
    machines are intentionally not bundled; they must provide PROTOC_BIN and
    PROTOC_SHA256. This keeps the test strict without pretending unsupported hosts
    have a bundled compiler.
    """
    proto_bin = os.environ.get("PROTOC_BIN", "")
    proto_sha = os.environ.get("PROTOC_SHA256", "")
    if proto_bin or proto_sha:
        return {"PROTOC_BIN": proto_bin, "PROTOC_SHA256": proto_sha}
    return {"PROTOC_BIN": "", "PROTOC_SHA256": ""}


class ProtocolCodegenToolingTests(unittest.TestCase):
    def make_fake(self, directory: Path, *, executable: bool = True) -> Path:
        fake = directory / "fake-protoc"
        fake.write_text(
            "#!/usr/bin/env bash\n"
            "if [[ \"${1:-}\" == \"--version\" ]]; then echo 'libprotoc 3.13.0'; exit 0; fi\n"
            "exit 17\n",
            encoding="utf-8",
        )
        mode = fake.stat().st_mode
        fake.chmod(mode | stat.S_IXUSR | stat.S_IXGRP | stat.S_IXOTH if executable else mode & ~0o111)
        return fake

    def test_pinned_tool_reports_version_path_and_sha(self) -> None:
        result = run_wrapper("version", env=pinned_protoc_env_or_empty())
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertIn(EXPECTED_VERSION, result.stdout)
        self.assertIn("path=", result.stdout)
        self.assertIn("sha256=", result.stdout)

    def test_override_requires_explicit_checksum(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            fake = self.make_fake(Path(tmp))
            result = run_wrapper("version", env={"PROTOC_BIN": str(fake), "PROTOC_SHA256": ""})
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("PROTOC_SHA256 is required", result.stderr)
        self.assertNotIn("Traceback", result.stderr)

    def test_override_rejects_wrong_checksum(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            fake = self.make_fake(Path(tmp))
            result = run_wrapper(
                "version",
                env={"PROTOC_BIN": str(fake), "PROTOC_SHA256": "0" * 64},
            )
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("protoc SHA256 mismatch", result.stderr)
        self.assertNotIn("Traceback", result.stderr)

    def test_pinned_fake_override_version_can_pass_but_verify_propagates_compile_failure(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            fake = self.make_fake(Path(tmp))
            env = {"PROTOC_BIN": str(fake), "PROTOC_SHA256": sha256(fake)}
            version = run_wrapper("version", env=env)
            verify = run_wrapper("verify", env=env)
        self.assertEqual(version.returncode, 0, version.stderr)
        self.assertNotEqual(verify.returncode, 0)
        self.assertIn("command failed rc=17", verify.stderr)
        self.assertNotIn("Traceback", verify.stderr)

    def test_non_executable_override_fails_without_traceback(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            fake = self.make_fake(Path(tmp), executable=False)
            result = run_wrapper(
                "version",
                env={"PROTOC_BIN": str(fake), "PROTOC_SHA256": sha256(fake)},
            )
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("PROTOC_BIN is not executable", result.stderr)
        self.assertNotIn("Traceback", result.stderr)



    def test_failed_generation_removes_stale_target_before_compiler_failure(self) -> None:
        target_root = ROOT / "build" / "generated" / "protocol"
        csharp = target_root / "csharp"
        shutil.rmtree(target_root, ignore_errors=True)
        csharp.mkdir(parents=True, exist_ok=True)
        stale = csharp / "STALE.cs"
        stale.write_text("stale", encoding="utf-8")
        try:
            with tempfile.TemporaryDirectory() as tmp:
                fake = self.make_fake(Path(tmp))
                result = run_wrapper(
                    "generate",
                    "--language",
                    "csharp",
                    env={"PROTOC_BIN": str(fake), "PROTOC_SHA256": sha256(fake)},
                )
            self.assertNotEqual(result.returncode, 0)
            self.assertIn("command failed rc=17", result.stderr)
            self.assertFalse(stale.exists())
            self.assertEqual(list(csharp.glob("*")), [])
            self.assertNotIn("Traceback", result.stderr)
        finally:
            shutil.rmtree(target_root, ignore_errors=True)

    def test_in_repo_output_override_cannot_write_source_or_frozen_paths(self) -> None:
        target = ROOT / "protocol" / "generated-attack-regression"
        self.assertFalse(target.exists())
        result = run_wrapper(
            "generate",
            "--language",
            "csharp",
            "--output-root",
            str(target),
            env=pinned_protoc_env_or_empty(),
        )
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("restricted to build/**", result.stderr)
        self.assertFalse(target.exists())
        self.assertNotIn("Traceback", result.stderr)


    def test_unsupported_host_does_not_fall_back_to_ambient_path_protoc(self) -> None:
        module_path = ROOT / "tools" / "protocol_codegen.py"
        spec = importlib.util.spec_from_file_location("lg_protocol_codegen_test_module", module_path)
        self.assertIsNotNone(spec)
        self.assertIsNotNone(spec.loader)
        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)
        old_bin = os.environ.pop("PROTOC_BIN", None)
        old_sha = os.environ.pop("PROTOC_SHA256", None)
        old_system = module.platform.system
        old_machine = module.platform.machine
        try:
            module.platform.system = lambda: "FreeBSD"
            module.platform.machine = lambda: "riscv64"
            with self.assertRaises(module.ProtocolToolError) as ctx:
                module.resolve_protoc()
            self.assertIn("no checksum-pinned bundled protoc", str(ctx.exception))
        finally:
            module.platform.system = old_system
            module.platform.machine = old_machine
            if old_bin is not None:
                os.environ["PROTOC_BIN"] = old_bin
            if old_sha is not None:
                os.environ["PROTOC_SHA256"] = old_sha

    def test_missing_override_fails_without_traceback(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            missing = Path(tmp) / "does-not-exist"
            result = run_wrapper(
                "version",
                env={"PROTOC_BIN": str(missing), "PROTOC_SHA256": "0" * 64},
            )
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("does not point to a file", result.stderr)
        self.assertNotIn("Traceback", result.stderr)


if __name__ == "__main__":
    unittest.main()
