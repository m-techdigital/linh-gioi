#!/usr/bin/env python3
"""Install and verify one pinned Blender build below the repository build tree."""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import plistlib
import shutil
import subprocess
import tempfile
import urllib.request
from dataclasses import asdict, dataclass
from pathlib import Path


RELEASE_ROOT = "https://download.blender.org/release/Blender4.5"


@dataclass(frozen=True)
class BlenderRelease:
    version: str = "4.5.13"
    platform: str = "macos-arm64"

    @property
    def archive_name(self) -> str:
        return f"blender-{self.version}-{self.platform}.dmg"

    @property
    def archive_url(self) -> str:
        return f"{RELEASE_ROOT}/{self.archive_name}"

    @property
    def checksum_url(self) -> str:
        return f"{RELEASE_ROOT}/blender-{self.version}.sha256"


def parse_checksum_manifest(manifest: str, archive_name: str) -> str:
    for raw_line in manifest.splitlines():
        fields = raw_line.strip().split()
        if len(fields) >= 2 and fields[-1].lstrip("*") == archive_name:
            digest = fields[0].lower()
            if len(digest) != 64 or any(character not in "0123456789abcdef" for character in digest):
                raise ValueError(f"Invalid SHA-256 for {archive_name}")
            return digest
    raise ValueError(f"Checksum entry not found for {archive_name}")


def verify_sha256(path: Path, expected: str) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    actual = digest.hexdigest()
    if actual.lower() != expected.lower():
        raise ValueError(f"SHA-256 mismatch for {path}: expected {expected}, got {actual}")
    return actual


def blender_python_command(executable: Path, script: Path, *script_arguments: str) -> list[str]:
    command = [
        str(executable),
        "--background",
        "--factory-startup",
        "--python-exit-code",
        "2",
        "--python",
        str(script),
    ]
    if script_arguments:
        command.extend(["--", *script_arguments])
    return command


def _download(url: str, destination: Path) -> None:
    destination.parent.mkdir(parents=True, exist_ok=True)
    temporary = destination.with_suffix(destination.suffix + ".part")
    request = urllib.request.Request(url, headers={"User-Agent": "LGO-Blender-Bootstrap/1.0"})
    with urllib.request.urlopen(request, timeout=60) as response, temporary.open("wb") as output:
        shutil.copyfileobj(response, output, length=1024 * 1024)
    os.replace(temporary, destination)


def _run(command: list[str], *, timeout: int = 120) -> subprocess.CompletedProcess[str]:
    return subprocess.run(command, check=True, text=True, capture_output=True, timeout=timeout)


def _mount_dmg(archive: Path) -> Path:
    result = subprocess.run(
        ["hdiutil", "attach", "-nobrowse", "-readonly", "-plist", str(archive)],
        check=True,
        capture_output=True,
        timeout=120,
    )
    payload = plistlib.loads(result.stdout)
    mount_points = [
        entity.get("mount-point")
        for entity in payload.get("system-entities", [])
        if entity.get("mount-point")
    ]
    if not mount_points:
        raise RuntimeError(f"No mount point returned for {archive}")
    return Path(mount_points[-1])


def bootstrap(output_root: Path, release: BlenderRelease = BlenderRelease()) -> dict[str, object]:
    output_root = output_root.resolve()
    install_root = output_root / f"blender-{release.version}"
    application = install_root / "Blender.app"
    executable = application / "Contents" / "MacOS" / "Blender"
    downloads = output_root / "downloads"
    archive = downloads / release.archive_name
    checksum_manifest = downloads / f"blender-{release.version}.sha256"

    if not checksum_manifest.exists():
        _download(release.checksum_url, checksum_manifest)
    expected_sha256 = parse_checksum_manifest(checksum_manifest.read_text(), release.archive_name)

    if not archive.exists():
        _download(release.archive_url, archive)
    actual_sha256 = verify_sha256(archive, expected_sha256)

    if not executable.exists():
        install_root.mkdir(parents=True, exist_ok=True)
        mount_point = _mount_dmg(archive)
        try:
            source_application = mount_point / "Blender.app"
            if not source_application.exists():
                raise RuntimeError(f"Blender.app missing from {mount_point}")
            with tempfile.TemporaryDirectory(dir=install_root, prefix="install-") as temporary_directory:
                staged_application = Path(temporary_directory) / "Blender.app"
                _run(["ditto", str(source_application), str(staged_application)], timeout=600)
                os.replace(staged_application, application)
        finally:
            _run(["hdiutil", "detach", str(mount_point)], timeout=120)

    _run(["codesign", "--verify", "--deep", "--strict", str(application)], timeout=120)
    version_result = _run([str(executable), "--version"], timeout=120)
    first_line = version_result.stdout.splitlines()[0] if version_result.stdout else ""
    expected_prefix = f"Blender {release.version}"
    if not first_line.startswith(expected_prefix):
        raise RuntimeError(f"Expected {expected_prefix}, got {first_line!r}")

    report = {
        "status": "BLENDER_TOOLCHAIN_PASS",
        "release": asdict(release),
        "archiveUrl": release.archive_url,
        "archive": str(archive),
        "archiveSha256": actual_sha256,
        "officialSha256": expected_sha256,
        "application": str(application),
        "executable": str(executable),
        "versionLine": first_line,
        "codesignVerified": True,
    }
    install_root.mkdir(parents=True, exist_ok=True)
    report_path = install_root / "bootstrap-report.json"
    report_path.write_text(json.dumps(report, indent=2) + "\n")
    return report


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--output-root", type=Path, default=Path("build/toolchains"))
    parser.add_argument("--version", default="4.5.13")
    arguments = parser.parse_args()
    report = bootstrap(arguments.output_root, BlenderRelease(version=arguments.version))
    print(json.dumps(report, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
