#!/usr/bin/env python3
import argparse
import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_MATRIX = ROOT / "docs/design/LGO-UI-AUTHORITY-MATRIX-v1.0.json"
REQUIRED_NONEMPTY = ("runtimeEvidence", "codeOwners", "states", "profiles")
VALID_KINDS = {"owner", "approved-proposal", "runtime-only"}


def file_sha256(path):
    digest = hashlib.sha256()
    with Path(path).open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def validate_document(document, owner_root=None):
    errors = []
    if document.get("version") != 1:
        errors.append("matrix.version must be 1")
    surfaces = document.get("surfaces")
    if not isinstance(surfaces, list) or not surfaces:
        return errors + ["surfaces must be a non-empty list"]
    seen = set()
    for index, surface in enumerate(surfaces):
        prefix = f"surface[{index}]"
        if not isinstance(surface, dict):
            errors.append(prefix + " must be an object")
            continue
        sid = surface.get("id")
        if not isinstance(sid, str) or not sid.strip():
            errors.append(prefix + ".id is required")
        elif sid in seen:
            errors.append("duplicate surface id: " + sid)
        else:
            seen.add(sid)
        authority = surface.get("designAuthority")
        if not isinstance(authority, dict):
            errors.append(prefix + ".designAuthority is required")
            authority = {}
        kind = authority.get("kind")
        if kind == "runtime":
            errors.append(prefix + ": runtime evidence cannot be design authority")
        elif kind not in VALID_KINDS:
            errors.append(prefix + ".designAuthority.kind is invalid")
        path = authority.get("relativePath")
        if not isinstance(path, str) or not path.strip():
            errors.append(prefix + ".designAuthority.relativePath is required")
        for field in REQUIRED_NONEMPTY:
            value = surface.get(field)
            if not isinstance(value, list) or not value:
                errors.append(prefix + "." + field + " must be a non-empty list")
        if not isinstance(surface.get("knownGaps"), list):
            errors.append(prefix + ".knownGaps must be a list")
        if kind == "owner" and owner_root and isinstance(path, str) and path:
            candidate = Path(owner_root) / path
            if not candidate.is_file():
                errors.append(prefix + ".designAuthority missing owner file: " + str(candidate))
            else:
                expected = authority.get("sha256")
                if expected and file_sha256(candidate) != expected:
                    errors.append(prefix + ".designAuthority sha256 mismatch: " + path)
        if kind == "approved-proposal" and isinstance(path, str) and path:
            candidate = ROOT / path
            if not candidate.is_file():
                errors.append(prefix + ".designAuthority missing proposal file: " + path)
            else:
                expected = authority.get("sha256")
                if expected and file_sha256(candidate) != expected:
                    errors.append(prefix + ".designAuthority sha256 mismatch: " + path)
    return errors


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--matrix", default=str(DEFAULT_MATRIX))
    parser.add_argument("--owner-root")
    args = parser.parse_args()
    document = json.loads(Path(args.matrix).read_text(encoding="utf-8"))
    errors = validate_document(document, args.owner_root)
    if errors:
        print("LGO_UI_AUTHORITY_MATRIX_FAIL")
        for error in errors:
            print("- " + error)
        raise SystemExit(1)
    print(f"LGO_UI_AUTHORITY_MATRIX_PASS surfaces={len(document['surfaces'])}")


if __name__ == "__main__":
    main()
