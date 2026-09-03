#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
import re
import shutil
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Iterable

try:  # Preferred validator path when local Python dependencies are available.
    import yaml  # type: ignore
except ModuleNotFoundError:  # Local macOS fallback: keep validators runnable without pip.
    yaml = None  # type: ignore

try:
    from jsonschema import Draft202012Validator  # type: ignore
except ModuleNotFoundError:
    Draft202012Validator = None  # type: ignore

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_GAMEDATA_ROOT = REPO_ROOT / "gamedata"
KIND_TO_SCHEMA = {
    "skills": "skill.schema.json",
    "items": "item.schema.json",
    "monsters": "monster.schema.json",
    "events": "world-event.schema.json",
}


@dataclass(frozen=True)
class ValidationResult:
    manifest: dict[str, Any]
    errors: tuple[str, ...]

    @property
    def valid(self) -> bool:
        return not self.errors


@dataclass(frozen=True)
class _ValidationError:
    path: tuple[str | int, ...]
    message: str


class _YamlParseError(ValueError):
    pass


def _parse_scalar(raw: str) -> Any:
    raw = raw.strip()
    if raw == "":
        return ""
    if raw in {"true", "True"}:
        return True
    if raw in {"false", "False"}:
        return False
    if raw in {"null", "Null", "~"}:
        return None
    if (raw.startswith('"') and raw.endswith('"')) or (raw.startswith("'") and raw.endswith("'")):
        return raw[1:-1]
    try:
        if re.fullmatch(r"[-+]?\d+", raw):
            return int(raw)
        if re.fullmatch(r"[-+]?(?:\d+\.\d*|\d*\.\d+)(?:[eE][-+]?\d+)?", raw) or re.fullmatch(r"[-+]?\d+[eE][-+]?\d+", raw):
            return float(raw)
    except ValueError:
        pass
    return raw


def _strip_comment(line: str) -> str:
    in_single = False
    in_double = False
    for i, ch in enumerate(line):
        if ch == "'" and not in_double:
            in_single = not in_single
        elif ch == '"' and not in_single:
            in_double = not in_double
        elif ch == "#" and not in_single and not in_double:
            return line[:i]
    return line


def _prepare_yaml_lines(text: str) -> list[tuple[int, str]]:
    lines: list[tuple[int, str]] = []
    for lineno, original in enumerate(text.splitlines(), start=1):
        if "\t" in original:
            raise _YamlParseError(f"line {lineno}: tabs are not supported by the local fallback YAML parser")
        raw = _strip_comment(original).rstrip()
        if not raw.strip():
            continue
        indent = len(raw) - len(raw.lstrip(" "))
        if indent % 2:
            raise _YamlParseError(f"line {lineno}: indentation must use two-space steps")
        lines.append((indent, raw.lstrip(" ")))
    return lines


def _parse_yaml_block(lines: list[tuple[int, str]], index: int, indent: int) -> tuple[Any, int]:
    if index >= len(lines):
        return {}, index
    current_indent, content = lines[index]
    if current_indent < indent:
        return {}, index
    if current_indent != indent:
        raise _YamlParseError(f"unexpected indentation near: {content}")
    if content.startswith("- "):
        return _parse_yaml_list(lines, index, indent)
    return _parse_yaml_mapping(lines, index, indent)


def _parse_yaml_mapping(lines: list[tuple[int, str]], index: int, indent: int) -> tuple[dict[str, Any], int]:
    mapping: dict[str, Any] = {}
    while index < len(lines):
        line_indent, content = lines[index]
        if line_indent < indent:
            break
        if line_indent != indent:
            raise _YamlParseError(f"unexpected nested mapping indentation near: {content}")
        if content.startswith("- "):
            break
        if ":" not in content:
            raise _YamlParseError(f"expected key: value mapping near: {content}")
        key, raw_value = content.split(":", 1)
        key = key.strip()
        if not key:
            raise _YamlParseError(f"empty mapping key near: {content}")
        raw_value = raw_value.strip()
        index += 1
        if raw_value:
            mapping[key] = _parse_scalar(raw_value)
        else:
            if index < len(lines) and lines[index][0] > line_indent:
                mapping[key], index = _parse_yaml_block(lines, index, lines[index][0])
            else:
                mapping[key] = None
    return mapping, index


def _parse_yaml_list(lines: list[tuple[int, str]], index: int, indent: int) -> tuple[list[Any], int]:
    items: list[Any] = []
    while index < len(lines):
        line_indent, content = lines[index]
        if line_indent < indent:
            break
        if line_indent != indent or not content.startswith("- "):
            break
        item_text = content[2:].strip()
        index += 1
        if not item_text:
            if index < len(lines) and lines[index][0] > line_indent:
                value, index = _parse_yaml_block(lines, index, lines[index][0])
            else:
                value = None
        elif ":" in item_text and not item_text.startswith(('"', "'")):
            key, raw_value = item_text.split(":", 1)
            item: dict[str, Any] = {}
            item[key.strip()] = _parse_scalar(raw_value.strip()) if raw_value.strip() else None
            if index < len(lines) and lines[index][0] > line_indent:
                nested, index = _parse_yaml_mapping(lines, index, lines[index][0])
                item.update(nested)
            value = item
        else:
            value = _parse_scalar(item_text)
        items.append(value)
    return items, index


def _safe_yaml_load(text: str) -> Any:
    lines = _prepare_yaml_lines(text)
    if not lines:
        return None
    value, index = _parse_yaml_block(lines, 0, lines[0][0])
    if index != len(lines):
        raise _YamlParseError(f"unparsed YAML content near: {lines[index][1]}")
    return value


class _MiniDraft202012Validator:
    def __init__(self, schema: dict[str, Any]) -> None:
        self.schema = schema

    def iter_errors(self, document: Any) -> Iterable[_ValidationError]:
        yield from self._validate(document, self.schema, ())

    def _validate(self, value: Any, schema: dict[str, Any], path: tuple[str | int, ...]) -> Iterable[_ValidationError]:
        expected_type = schema.get("type")
        if expected_type and not self._matches_type(value, expected_type):
            yield _ValidationError(path, f"{value!r} is not of type {expected_type!r}")
            return

        if "const" in schema and value != schema["const"]:
            yield _ValidationError(path, f"{value!r} was expected to be constant {schema['const']!r}")

        if "enum" in schema and value not in schema["enum"]:
            yield _ValidationError(path, f"{value!r} is not one of {schema['enum']!r}")

        if isinstance(value, (int, float)) and not isinstance(value, bool):
            if "minimum" in schema and value < schema["minimum"]:
                yield _ValidationError(path, f"{value!r} is less than the minimum of {schema['minimum']!r}")
            if "maximum" in schema and value > schema["maximum"]:
                yield _ValidationError(path, f"{value!r} is greater than the maximum of {schema['maximum']!r}")

        if isinstance(value, str) and "pattern" in schema:
            if not re.search(schema["pattern"], value):
                yield _ValidationError(path, f"{value!r} does not match {schema['pattern']!r}")

        if isinstance(value, dict):
            required = schema.get("required", [])
            for key in required:
                if key not in value:
                    yield _ValidationError(path + (key,), f"{key!r} is a required property")
            properties = schema.get("properties", {})
            if schema.get("additionalProperties") is False:
                for key in sorted(value):
                    if key not in properties:
                        yield _ValidationError(path + (key,), f"additional property {key!r} is not allowed")
            for key, subschema in properties.items():
                if key in value:
                    yield from self._validate(value[key], subschema, path + (key,))

        if isinstance(value, list):
            if "minItems" in schema and len(value) < schema["minItems"]:
                yield _ValidationError(path, f"{value!r} is too short")
            if schema.get("uniqueItems") is True:
                rendered = [json.dumps(item, ensure_ascii=False, sort_keys=True) for item in value]
                if len(rendered) != len(set(rendered)):
                    yield _ValidationError(path, f"{value!r} has non-unique elements")
            if "items" in schema:
                for index, item in enumerate(value):
                    yield from self._validate(item, schema["items"], path + (index,))

    @staticmethod
    def _matches_type(value: Any, expected: str) -> bool:
        if expected == "object":
            return isinstance(value, dict)
        if expected == "array":
            return isinstance(value, list)
        if expected == "string":
            return isinstance(value, str)
        if expected == "integer":
            return isinstance(value, int) and not isinstance(value, bool)
        if expected == "number":
            return isinstance(value, (int, float)) and not isinstance(value, bool)
        if expected == "boolean":
            return isinstance(value, bool)
        return True


def _validator(schema: dict[str, Any]) -> Any:
    if Draft202012Validator is not None:
        return Draft202012Validator(schema)
    return _MiniDraft202012Validator(schema)


def load_yaml(path: Path) -> Any:
    try:
        with path.open("r", encoding="utf-8") as handle:
            if yaml is not None:
                return yaml.safe_load(handle)
            return _safe_yaml_load(handle.read())
    except getattr(yaml, "YAMLError", _YamlParseError) as exc:  # type: ignore[arg-type]
        raise ValueError(f"{path}: invalid YAML: {exc}") from exc
    except _YamlParseError as exc:
        raise ValueError(f"{path}: invalid YAML: {exc}") from exc


def relative(path: Path, root: Path) -> str:
    try:
        return path.relative_to(root).as_posix()
    except ValueError:
        return path.as_posix()


def canonical_json(value: Any) -> bytes:
    return json.dumps(value, ensure_ascii=False, sort_keys=True, separators=(",", ":")).encode("utf-8")


def _error_sort_key(item: Any) -> tuple[list[str], str]:
    return ([str(part) for part in getattr(item, "path", [])], getattr(item, "message", ""))


def validate_gamedata(gamedata_root: Path) -> ValidationResult:
    gamedata_root = gamedata_root.resolve()
    schema_root = gamedata_root / "schemas"
    registry_path = gamedata_root / "registry.yaml"
    errors: list[str] = []

    try:
        registry = load_yaml(registry_path) or {}
    except (OSError, ValueError) as exc:
        return ValidationResult({}, (str(exc),))

    if not isinstance(registry, dict):
        return ValidationResult({}, (f"{registry_path}: registry must be a mapping",))

    gamedata_version = registry.get("gamedata_version", 1)
    if not isinstance(gamedata_version, int) or gamedata_version < 1:
        errors.append(f"{relative(registry_path, gamedata_root.parent)}:gamedata_version: must be a positive integer")
        gamedata_version = 1

    classes = set(registry.get("classes", []) or [])
    maps = set(registry.get("maps", []) or [])
    ids: dict[str, Path] = {}
    compiled_documents: list[dict[str, Any]] = []

    for kind, schema_name in KIND_TO_SCHEMA.items():
        schema_path = schema_root / schema_name
        try:
            schema = json.loads(schema_path.read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError) as exc:
            errors.append(f"{relative(schema_path, gamedata_root.parent)}: cannot load schema: {exc}")
            continue
        validator = _validator(schema)

        source_dir = gamedata_root / kind
        for path in sorted(source_dir.glob("*.yaml"), key=lambda p: p.name):
            try:
                document = load_yaml(path)
            except (OSError, ValueError) as exc:
                errors.append(str(exc))
                continue
            display = relative(path, gamedata_root.parent)
            if not isinstance(document, dict):
                errors.append(f"{display}:<root>: document must be a mapping")
                continue

            for error in sorted(validator.iter_errors(document), key=_error_sort_key):
                location = ".".join(map(str, getattr(error, "path", []))) or "<root>"
                errors.append(f"{display}:{location}: {error.message}")

            content_id = document.get("id")
            if isinstance(content_id, str) and content_id:
                previous = ids.get(content_id)
                if previous is not None:
                    errors.append(
                        f"{display}:id: duplicate id {content_id!r}; first defined in "
                        f"{relative(previous, gamedata_root.parent)}"
                    )
                else:
                    ids[content_id] = path

            if kind == "skills":
                class_id = document.get("class_id")
                if class_id not in classes:
                    errors.append(f"{display}:class_id: unknown class {class_id!r}")
            elif kind == "events":
                map_id = document.get("map_id")
                if map_id not in maps:
                    errors.append(f"{display}:map_id: unknown map {map_id!r}")

            compiled_documents.append({"kind": kind, "source": display, "data": document})

    compiled_documents.sort(key=lambda item: (item["kind"], str(item["data"].get("id", "")), item["source"]))
    content_hash = hashlib.sha256(canonical_json(compiled_documents)).hexdigest()
    manifest = {
        "gamedata_version": gamedata_version,
        "content_sha256": content_hash,
        "content_count": len(compiled_documents),
        "ids": sorted(ids),
        "documents": compiled_documents,
    }
    return ValidationResult(manifest, tuple(errors))


def write_manifest(result: ValidationResult, output_path: Path) -> None:
    if not result.valid:
        raise ValueError("cannot write compiled GameData manifest while validation errors exist")
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(
        json.dumps(result.manifest, ensure_ascii=False, sort_keys=True, indent=2) + "\n",
        encoding="utf-8",
    )


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate and compile Linh Gioi GameData")
    parser.add_argument("--gamedata-root", type=Path, default=DEFAULT_GAMEDATA_ROOT)
    parser.add_argument("--output", type=Path, default=None)
    parser.add_argument("--check", action="store_true", help="validate existing compiled output instead of writing it")
    parser.add_argument("--include-invalid-fixtures", action="store_true", help=argparse.SUPPRESS)
    args = parser.parse_args()

    gamedata_root = args.gamedata_root.resolve()
    output = args.output or gamedata_root / "compiled" / "gamedata-manifest.json"
    result = validate_gamedata(gamedata_root)

    if not result.valid:
        print("GAMEDATA VALIDATION FAILED", file=sys.stderr)
        for error in result.errors:
            print(f" - {error}", file=sys.stderr)
        return 1

    rendered = json.dumps(result.manifest, ensure_ascii=False, sort_keys=True, indent=2) + "\n"
    if args.check:
        if not output.exists():
            print(f"GAMEDATA CHECK FAILED: compiled output missing: {output}", file=sys.stderr)
            return 1
        existing = output.read_text(encoding="utf-8")
        if existing != rendered:
            print("GAMEDATA CHECK FAILED: compiled output is stale or non-deterministic", file=sys.stderr)
            return 1
    else:
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(rendered, encoding="utf-8")

    dependency_mode = "external" if yaml is not None and Draft202012Validator is not None else "stdlib-fallback"
    print(
        f"GAMEDATA VALID: {result.manifest['content_count']} docs, "
        f"version={result.manifest['gamedata_version']}, sha256={result.manifest['content_sha256']} "
        f"validator={dependency_mode}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
