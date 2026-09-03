#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import os
import platform
import re
import subprocess
import sys
import tempfile
from pathlib import Path
from typing import Iterable

ROOT = Path(__file__).resolve().parents[1]
PROTOCOL_DIR = ROOT / "protocol"
PINNED_VERSION = (ROOT / "tools" / "protobuf" / "VERSION").read_text(encoding="utf-8").strip()
EXPECTED_VERSION_TEXT = f"libprotoc {PINNED_VERSION}"
BUNDLED_LINUX_X86_64 = ROOT / "tools" / "protobuf" / "linux-x86_64" / "protoc"
BUNDLED_SHA_FILE = ROOT / "tools" / "protobuf" / "linux-x86_64" / "SHA256"
BUNDLED_DARWIN_ARM64 = ROOT / "tools" / "protobuf" / "darwin-arm64" / "protoc"
BUNDLED_DARWIN_ARM64_SHA_FILE = ROOT / "tools" / "protobuf" / "darwin-arm64" / "SHA256"
DEFAULT_OUTPUT_ROOT = ROOT / "build" / "generated" / "protocol"
NEGATIVE_FIXTURE = ROOT / "tests" / "protocol" / "invalid" / "invalid_syntax.proto"
SHA256_RE = re.compile(r"^[0-9a-fA-F]{64}$")


class ProtocolToolError(RuntimeError):
    pass


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def validate_sha256(value: str, *, source: str) -> str:
    normalized = value.strip().lower()
    if not SHA256_RE.fullmatch(normalized):
        raise ProtocolToolError(f"invalid SHA256 declaration from {source}: expected exactly 64 hex characters")
    return normalized


def expected_bundled_sha256(sha_file: Path) -> str:
    try:
        first = sha_file.read_text(encoding="utf-8").strip().split()[0]
    except (OSError, IndexError) as exc:
        raise ProtocolToolError(f"cannot read bundled SHA256 declaration: {sha_file}: {exc}") from exc
    return validate_sha256(first, source=str(sha_file))


def verify_binary_checksum(candidate: Path, expected: str, *, source: str) -> str:
    try:
        actual = sha256_file(candidate)
    except OSError as exc:
        raise ProtocolToolError(f"cannot hash protoc binary: {candidate}: {exc}") from exc
    if actual != expected:
        raise ProtocolToolError(
            "protoc SHA256 mismatch: "
            f"expected={expected} actual={actual} path={candidate} checksum_source={source}"
        )
    return actual


def resolve_protoc() -> tuple[Path, str]:
    override = os.environ.get("PROTOC_BIN")
    if override:
        candidate = Path(override).expanduser().resolve()
        if not candidate.is_file():
            raise ProtocolToolError(f"PROTOC_BIN does not point to a file: {candidate}")
        if not os.access(candidate, os.X_OK):
            raise ProtocolToolError(f"PROTOC_BIN is not executable: {candidate}")

        try:
            is_bundled = BUNDLED_LINUX_X86_64.is_file() and candidate.samefile(BUNDLED_LINUX_X86_64)
        except OSError:
            is_bundled = False

        if is_bundled:
            expected = expected_bundled_sha256(BUNDLED_SHA_FILE)
            checksum_source = str(BUNDLED_SHA_FILE)
        elif BUNDLED_DARWIN_ARM64.is_file() and candidate.samefile(BUNDLED_DARWIN_ARM64):
            expected = expected_bundled_sha256(BUNDLED_DARWIN_ARM64_SHA_FILE)
            checksum_source = str(BUNDLED_DARWIN_ARM64_SHA_FILE)
        else:
            declared = os.environ.get("PROTOC_SHA256")
            if not declared:
                raise ProtocolToolError(
                    "PROTOC_SHA256 is required whenever PROTOC_BIN overrides the bundled compiler; "
                    "pin the exact compiler binary, not only its version string"
                )
            expected = validate_sha256(declared, source="PROTOC_SHA256")
            checksum_source = "PROTOC_SHA256"
        actual = verify_binary_checksum(candidate, expected, source=checksum_source)
        return candidate, actual

    machine = platform.machine().lower()
    system = platform.system().lower()
    if system == "linux" and machine in {"x86_64", "amd64"} and BUNDLED_LINUX_X86_64.is_file():
        if not os.access(BUNDLED_LINUX_X86_64, os.X_OK):
            raise ProtocolToolError(f"bundled protoc is not executable: {BUNDLED_LINUX_X86_64}")
        expected = expected_bundled_sha256(BUNDLED_SHA_FILE)
        actual = verify_binary_checksum(BUNDLED_LINUX_X86_64, expected, source=str(BUNDLED_SHA_FILE))
        return BUNDLED_LINUX_X86_64, actual
    if system == "darwin" and BUNDLED_DARWIN_ARM64.is_file():
        if not os.access(BUNDLED_DARWIN_ARM64, os.X_OK):
            raise ProtocolToolError(f"bundled protoc is not executable: {BUNDLED_DARWIN_ARM64}")
        expected = expected_bundled_sha256(BUNDLED_DARWIN_ARM64_SHA_FILE)
        actual = verify_binary_checksum(BUNDLED_DARWIN_ARM64, expected, source=str(BUNDLED_DARWIN_ARM64_SHA_FILE))
        return BUNDLED_DARWIN_ARM64, actual

    raise ProtocolToolError(
        "no checksum-pinned bundled protoc is available for this host. "
        "Set PROTOC_BIN to a local protoc binary and PROTOC_SHA256 to its exact SHA256; "
        f"the compiler version must also be exactly {EXPECTED_VERSION_TEXT!r}."
    )


def run_process(command: list[str], *, cwd: Path | None = None) -> subprocess.CompletedProcess[str]:
    try:
        return subprocess.run(
            command,
            cwd=cwd,
            text=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            check=False,
        )
    except OSError as exc:
        raise ProtocolToolError(f"failed to execute {' '.join(command)}: {exc}") from exc


def verify_protoc_version(protoc: Path) -> None:
    result = run_process([str(protoc), "--version"])
    actual = result.stdout.strip()
    if result.returncode != 0:
        detail = result.stderr.strip() or result.stdout.strip() or "<no compiler output>"
        raise ProtocolToolError(f"protoc --version failed rc={result.returncode}: {detail}")
    if actual != EXPECTED_VERSION_TEXT:
        raise ProtocolToolError(
            f"protoc version mismatch: expected={EXPECTED_VERSION_TEXT!r} actual={actual!r} path={protoc}"
        )


def proto_files() -> list[Path]:
    files = sorted(PROTOCOL_DIR.glob("*.proto"), key=lambda p: p.name)
    if not files:
        raise ProtocolToolError(f"no .proto files found under {PROTOCOL_DIR}")
    return files


def relative_proto_names(files: Iterable[Path]) -> list[str]:
    return [path.name for path in files]


def run_checked(command: list[str], *, cwd: Path | None = None) -> subprocess.CompletedProcess[str]:
    result = run_process(command, cwd=cwd)
    if result.returncode != 0:
        detail = result.stderr.strip() or result.stdout.strip() or "<no compiler output>"
        raise ProtocolToolError(f"command failed rc={result.returncode}: {' '.join(command)}\n{detail}")
    return result


def compile_descriptor(protoc: Path, descriptor_out: Path) -> None:
    files = proto_files()
    descriptor_out.parent.mkdir(parents=True, exist_ok=True)
    command = [
        str(protoc),
        f"--proto_path={PROTOCOL_DIR}",
        f"--descriptor_set_out={descriptor_out}",
        "--include_imports",
        *relative_proto_names(files),
    ]
    run_checked(command, cwd=PROTOCOL_DIR)
    if not descriptor_out.is_file() or descriptor_out.stat().st_size == 0:
        raise ProtocolToolError(f"descriptor compilation produced no output: {descriptor_out}")


def resolve_output_root(requested: Path) -> Path:
    output = requested.expanduser().resolve()
    root = ROOT.resolve()
    build_lexical = (ROOT / "build").absolute()
    build_resolved = (ROOT / "build").resolve()

    if build_resolved != build_lexical:
        raise ProtocolToolError(
            f"repository build path resolves outside its canonical location: lexical={build_lexical} resolved={build_resolved}"
        )

    if output.is_relative_to(root) and not output.is_relative_to(build_resolved):
        raise ProtocolToolError(
            "generated output inside the repository is restricted to build/**; "
            f"refusing source/frozen path: {output}"
        )
    return output


def clean_directory(path: Path) -> None:
    if path.exists():
        shutil_error: OSError | None = None
        try:
            import shutil
            shutil.rmtree(path)
        except OSError as exc:
            shutil_error = exc
        if shutil_error is not None:
            raise ProtocolToolError(f"cannot clean generated output directory {path}: {shutil_error}")
    try:
        path.mkdir(parents=True, exist_ok=True)
    except OSError as exc:
        raise ProtocolToolError(f"cannot create generated output directory {path}: {exc}") from exc


def generate(protoc: Path, output_root: Path, language: str) -> dict[str, Path]:
    files = proto_files()
    outputs: dict[str, Path] = {}

    if language in {"csharp", "all"}:
        csharp = output_root / "csharp"
        clean_directory(csharp)
        outputs["csharp"] = csharp
    if language in {"java", "all"}:
        java = output_root / "java"
        clean_directory(java)
        outputs["java"] = java

    command = [str(protoc), f"--proto_path={PROTOCOL_DIR}"]
    if "csharp" in outputs:
        command.append(f"--csharp_out={outputs['csharp']}")
    if "java" in outputs:
        command.append(f"--java_out={outputs['java']}")
    command.extend(relative_proto_names(files))
    run_checked(command, cwd=PROTOCOL_DIR)

    for lang, directory in outputs.items():
        generated = sorted(p for p in directory.rglob("*") if p.is_file())
        if not generated:
            raise ProtocolToolError(f"{lang} generation produced zero files under {directory}")
    return outputs


def tree_manifest(root: Path) -> list[tuple[str, str]]:
    manifest: list[tuple[str, str]] = []
    for path in sorted((p for p in root.rglob("*") if p.is_file()), key=lambda p: p.relative_to(root).as_posix()):
        manifest.append((path.relative_to(root).as_posix(), sha256_file(path)))
    return manifest


def manifest_digest(manifest: list[tuple[str, str]]) -> str:
    digest = hashlib.sha256()
    for rel, sha in manifest:
        digest.update(rel.encode("utf-8"))
        digest.update(b"\0")
        digest.update(sha.encode("ascii"))
        digest.update(b"\n")
    return digest.hexdigest()


def verify_determinism(protoc: Path) -> tuple[int, int, str]:
    with tempfile.TemporaryDirectory(prefix="lg-proto-a-") as first_tmp, tempfile.TemporaryDirectory(prefix="lg-proto-b-") as second_tmp:
        first = Path(first_tmp)
        second = Path(second_tmp)
        generate(protoc, first, "all")
        generate(protoc, second, "all")
        first_manifest = tree_manifest(first)
        second_manifest = tree_manifest(second)
        if first_manifest != second_manifest:
            raise ProtocolToolError("determinism check failed: repeated generation produced different files/content")
        csharp_count = sum(1 for rel, _ in first_manifest if rel.startswith("csharp/"))
        java_count = sum(1 for rel, _ in first_manifest if rel.startswith("java/"))
        if csharp_count == 0 or java_count == 0:
            raise ProtocolToolError(
                f"determinism check missing generated language output: csharp={csharp_count} java={java_count}"
            )
        return csharp_count, java_count, manifest_digest(first_manifest)


def verify_negative_fixture(protoc: Path) -> str:
    if not NEGATIVE_FIXTURE.is_file():
        raise ProtocolToolError(f"negative fixture missing: {NEGATIVE_FIXTURE}")
    with tempfile.TemporaryDirectory(prefix="lg-proto-negative-") as tmp:
        descriptor = Path(tmp) / "invalid.pb"
        command = [
            str(protoc),
            f"--proto_path={NEGATIVE_FIXTURE.parent}",
            f"--descriptor_set_out={descriptor}",
            NEGATIVE_FIXTURE.name,
        ]
        result = run_process(command, cwd=NEGATIVE_FIXTURE.parent)
        if result.returncode == 0:
            raise ProtocolToolError("negative fixture unexpectedly compiled successfully")
        diagnostic = (result.stderr.strip() or result.stdout.strip()).splitlines()
        if not diagnostic:
            raise ProtocolToolError("negative fixture failed without compiler diagnostics")
        return diagnostic[0]


def verify_all(protoc: Path, compiler_sha256: str) -> None:
    static = run_checked([sys.executable, str(ROOT / "tools" / "validate_proto_contract.py")], cwd=ROOT)
    if static.stdout.strip():
        print(static.stdout.strip())

    with tempfile.TemporaryDirectory(prefix="lg-proto-descriptor-") as tmp:
        descriptor = Path(tmp) / "linhgioi-v1.pb"
        compile_descriptor(protoc, descriptor)
        print(f"PROTOC DESCRIPTOR COMPILE PASS: bytes={descriptor.stat().st_size}")

    csharp_count, java_count, digest = verify_determinism(protoc)
    print(f"PROTOBUF C# GENERATION PASS: files={csharp_count}")
    print(f"PROTOBUF JAVA GENERATION PASS: files={java_count}")
    print(f"PROTOBUF DETERMINISM PASS: manifest_sha256={digest}")

    diagnostic = verify_negative_fixture(protoc)
    print(f"PROTOBUF NEGATIVE COMPILE PASS: compiler_rejected_fixture={NEGATIVE_FIXTURE.relative_to(ROOT)}")
    print(f"PROTOBUF NEGATIVE DIAGNOSTIC: {diagnostic}")
    print(
        "PROTOBUF TOOLING VERIFY PASS: "
        f"protoc={EXPECTED_VERSION_TEXT} path={protoc} sha256={compiler_sha256}"
    )


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Canonical Linh Gioi M0 protobuf codegen/verification tool")
    subparsers = parser.add_subparsers(dest="command", required=True)

    gen = subparsers.add_parser("generate", help="generate disposable C#/Java sources")
    gen.add_argument("--language", choices=["csharp", "java", "all"], default="all")
    gen.add_argument("--output-root", type=Path, default=DEFAULT_OUTPUT_ROOT)

    subparsers.add_parser("verify", help="run compiler, codegen, determinism and negative checks")
    subparsers.add_parser("version", help="print and verify the pinned compiler")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    try:
        protoc, compiler_sha256 = resolve_protoc()
        verify_protoc_version(protoc)
        if args.command == "version":
            print(
                "PROTOBUF TOOLCHAIN PASS: "
                f"{EXPECTED_VERSION_TEXT} path={protoc} sha256={compiler_sha256}"
            )
            return 0
        if args.command == "generate":
            output_root = resolve_output_root(args.output_root)
            outputs = generate(protoc, output_root, args.language)
            for lang, directory in outputs.items():
                files = tree_manifest(directory)
                print(
                    f"PROTOBUF {lang.upper()} GENERATION PASS: "
                    f"files={len(files)} output={directory} manifest_sha256={manifest_digest(files)} "
                    f"protoc_sha256={compiler_sha256}"
                )
            return 0
        if args.command == "verify":
            verify_all(protoc, compiler_sha256)
            return 0
        raise ProtocolToolError(f"unsupported command: {args.command}")
    except ProtocolToolError as exc:
        print(f"PROTOBUF TOOLING FAILED: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
