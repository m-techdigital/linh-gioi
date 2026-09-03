#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PROJECT="$ROOT/client/Unity"
UNITY_EDITOR="${UNITY_EDITOR:-}"
OUT_DIR="$ROOT/build/unity-player-evidence"
TIMESTAMP="$(date -u +%Y%m%dT%H%M%SZ)"
PLAYER_NAME="LinhGioiM0PlayerSmoke.x86_64"
STATUS="PASS"
FAILED_STEP=""

usage() {
  cat <<'USAGE'
Usage:
  UNITY_EDITOR=/path/to/Unity ./tools/unity_player_evidence/build_unity_player_evidence.sh [--output-dir DIR]

Creates a small Unity evidence bundle and a Linux player smoke archive. This script
runs on a machine that has Unity Editor 6000.3.2f1 and Linux Build Support installed.
It does not package Unity Editor or Library/Temp caches.

If a Unity step fails, the script still writes a failure evidence ZIP and exits non-zero.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir)
      OUT_DIR="$2"; shift 2 ;;
    --help|-h)
      usage; exit 0 ;;
    *)
      echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

if [[ -z "$UNITY_EDITOR" ]]; then
  for candidate in "/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" \
                   "/Applications/Unity/Hub/Editor/6000.3.2f1/Editor/Unity" \
                   "$(command -v Unity 2>/dev/null || printf '')" \
                   "$(command -v unity-editor 2>/dev/null || printf '')"; do
    if [[ -n "$candidate" && -x "$candidate" ]]; then UNITY_EDITOR="$candidate"; break; fi
  done
fi
if [[ -z "$UNITY_EDITOR" || ! -x "$UNITY_EDITOR" ]]; then
  echo "ERROR: Unity Editor 6000.3.2f1 is required. Set UNITY_EDITOR=/absolute/path/to/Unity." >&2
  exit 20
fi

mkdir -p "$OUT_DIR"
EVIDENCE_DIR="$OUT_DIR/evidence-$TIMESTAMP"
PLAYER_DIR="$OUT_DIR/player-$TIMESTAMP"
mkdir -p "$EVIDENCE_DIR" "$PLAYER_DIR"
COMMANDS_LOG="$EVIDENCE_DIR/commands.log"
: > "$COMMANDS_LOG"

sha256_file() {
  if command -v sha256sum >/dev/null 2>&1; then
    sha256sum "$@"
  else
    shasum -a 256 "$@"
  fi
}

sha256_value() {
  sha256_file "$1" | awk '{print $1}'
}

ensure_macos_protoc() {
  # protocol_codegen.py only bundles Linux x86_64 protoc. For local macOS Unity
  # evidence generation, provide a temporary protoc 3.13.0 and pass its exact
  # SHA256 through PROTOC_SHA256. The compiler version is still verified by
  # protocol_codegen.py as "libprotoc 3.13.0".
  if [[ -n "${PROTOC_BIN:-}" ]]; then
    if [[ ! -x "$PROTOC_BIN" ]]; then
      echo "ERROR: PROTOC_BIN is set but not executable: $PROTOC_BIN" >&2
      return 31
    fi
    if [[ -z "${PROTOC_SHA256:-}" ]]; then
      export PROTOC_SHA256="$(sha256_value "$PROTOC_BIN")"
    fi
    echo "PROTOC_BIN=$PROTOC_BIN"
    echo "PROTOC_SHA256=$PROTOC_SHA256"
    "$PROTOC_BIN" --version
    return 0
  fi

  local tool_dir="$OUT_DIR/.toolchain/protoc-3.13.0-osx-x86_64"
  local archive="$OUT_DIR/.toolchain/protoc-3.13.0-osx-x86_64.zip"
  local url="${PROTOC_MACOS_URL:-https://github.com/protocolbuffers/protobuf/releases/download/v3.13.0/protoc-3.13.0-osx-x86_64.zip}"
  mkdir -p "$OUT_DIR/.toolchain"

  if [[ ! -x "$tool_dir/bin/protoc" ]]; then
    echo "Downloading temporary macOS protoc 3.13.0 for Unity evidence generation: $url" >&2
    rm -rf "$tool_dir"
    curl -fL --retry 3 --connect-timeout 20 -o "$archive" "$url"
    unzip -q "$archive" -d "$tool_dir"
    chmod +x "$tool_dir/bin/protoc"
  fi

  export PROTOC_BIN="$tool_dir/bin/protoc"
  export PROTOC_SHA256="$(sha256_value "$PROTOC_BIN")"
  echo "PROTOC_BIN=$PROTOC_BIN"
  echo "PROTOC_SHA256=$PROTOC_SHA256"
  "$PROTOC_BIN" --version
}


ensure_unity_managed_dependencies() {
  local plugins_dir="$PROJECT/Assets/Plugins/Google.Protobuf"
  local dll="$plugins_dir/Google.Protobuf.dll"
  local nupkg="$OUT_DIR/.toolchain/Google.Protobuf.3.13.0.nupkg"
  local url="${GOOGLE_PROTOBUF_NUPKG_URL:-https://www.nuget.org/api/v2/package/Google.Protobuf/3.13.0}"

  mkdir -p "$plugins_dir" "$OUT_DIR/.toolchain"

  if [[ ! -f "$dll" ]]; then
    echo "Google.Protobuf.dll not present; downloading pinned Google.Protobuf 3.13.0 nupkg for local Unity evidence build." >&2
    curl -fL --retry 3 --connect-timeout 20 -o "$nupkg" "$url"
    python3 - "$nupkg" "$dll" <<'PYDLL'
from pathlib import Path
import sys, zipfile
nupkg = Path(sys.argv[1])
out = Path(sys.argv[2])
candidates = [
    'lib/netstandard2.0/Google.Protobuf.dll',
    'lib/netstandard1.1/Google.Protobuf.dll',
    'lib/net45/Google.Protobuf.dll',
]
with zipfile.ZipFile(nupkg) as zf:
    names = set(zf.namelist())
    selected = next((c for c in candidates if c in names), None)
    if selected is None:
        raise SystemExit('ERROR: Google.Protobuf.dll not found in pinned nupkg')
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_bytes(zf.read(selected))
    print(f'GOOGLE_PROTOBUF_DLL_PREPARED source={selected} output={out}')
PYDLL
  fi

  if [[ ! -f "$dll" ]]; then
    echo "ERROR: Google.Protobuf.dll is still missing after dependency preparation: $dll" >&2
    return 32
  fi

  {
    echo "GOOGLE_PROTOBUF_DLL=$dll"
    echo "GOOGLE_PROTOBUF_DLL_SHA256=$(sha256_value "$dll")"
    if [[ -f "$nupkg" ]]; then
      echo "GOOGLE_PROTOBUF_NUPKG=$nupkg"
      echo "GOOGLE_PROTOBUF_NUPKG_SHA256=$(sha256_value "$nupkg")"
    fi
  }
}

zip_evidence() {
  local evidence_zip="$OUT_DIR/lgo-unity-editor-evidence-6000.3.2f1-$TIMESTAMP.zip"
  python3 - "$EVIDENCE_DIR" "$evidence_zip" <<'PY'
from pathlib import Path
import sys, zipfile
source = Path(sys.argv[1])
out = Path(sys.argv[2])
with zipfile.ZipFile(out, 'w', compression=zipfile.ZIP_DEFLATED) as zf:
    for path in sorted(source.rglob('*')):
        if path.is_file():
            zf.write(path, path.relative_to(source))
PY
  sha256_file "$evidence_zip" > "$evidence_zip.sha256"
  echo "$evidence_zip"
}

write_summary() {
  local summary="$EVIDENCE_DIR/unity-evidence-summary.md"
  cat > "$summary" <<EOF
# Linh Gioi M0 Unity Player Evidence

TASK_ID: LG-M0-UNITY-PLAYER-EVIDENCE
TIMESTAMP_UTC: $TIMESTAMP
STATUS: $STATUS
FAILED_STEP: ${FAILED_STEP:-none}
UNITY_EDITOR: $UNITY_EDITOR
PLAYER_EXECUTABLE: $PLAYER_NAME

Evidence collected:
- Unity version probe
- temporary checksum-recorded macOS protoc 3.13.0 bootstrap if needed
- pinned Google.Protobuf 3.13.0 DLL preparation for Unity asmdef compile
- canonical C# protobuf generation
- Unity project import/generated foundation
- Unity EditMode test results
- Linux player build log
- generated Unity file list
- player file list

This bundle does not include Unity Editor, Library, Temp, or Logs caches.
EOF
}

fail_with_evidence() {
  local rc="$1"
  STATUS="FAIL"
  write_summary
  zip_evidence >/dev/null || true
  cat > "$OUT_DIR/UPLOAD-THESE-FILES.txt" <<EOF
Unity evidence run failed at: $FAILED_STEP
Upload this evidence ZIP and SHA for debugging:
$(ls -1 "$OUT_DIR"/lgo-unity-editor-evidence-6000.3.2f1-$TIMESTAMP.zip 2>/dev/null | xargs -n1 basename 2>/dev/null || true)
$(ls -1 "$OUT_DIR"/lgo-unity-editor-evidence-6000.3.2f1-$TIMESTAMP.zip.sha256 2>/dev/null | xargs -n1 basename 2>/dev/null || true)
EOF
  echo "UNITY_PLAYER_EVIDENCE_FAILED step=$FAILED_STEP rc=$rc evidence_dir=$EVIDENCE_DIR" >&2
  echo "If present, upload evidence ZIP + SHA from: $OUT_DIR" >&2
  exit "$rc"
}

run_capture() {
  local label="$1"; shift
  local log="$EVIDENCE_DIR/$label.log"
  echo "== $label ==" | tee -a "$COMMANDS_LOG"
  printf 'cwd=%s\ncommand=' "$ROOT" >> "$COMMANDS_LOG"
  printf '%q ' "$@" >> "$COMMANDS_LOG"
  printf '\nstarted_at_utc=%s\n' "$(date -u +%FT%TZ)" >> "$COMMANDS_LOG"
  set +e
  (cd "$ROOT" && "$@") >"$log" 2>&1
  local rc=$?
  set -e
  printf 'exit_code=%s\nfinished_at_utc=%s\nlog=%s\n\n' "$rc" "$(date -u +%FT%TZ)" "$log" >> "$COMMANDS_LOG"
  if [[ "$rc" -ne 0 ]]; then
    FAILED_STEP="$label"
    return "$rc"
  fi
  return 0
}

{
  echo "TASK_ID=LG-M0-UNITY-PLAYER-EVIDENCE"
  echo "timestamp_utc=$TIMESTAMP"
  echo "host_os=$(uname -a)"
  echo "unity_editor=$UNITY_EDITOR"
  "$UNITY_EDITOR" -version
} > "$EVIDENCE_DIR/unity-version.txt" 2>&1 || {
  FAILED_STEP="unity-version"
  fail_with_evidence 21
}

if ! grep -q '6000\.3\.2f1' "$EVIDENCE_DIR/unity-version.txt"; then
  FAILED_STEP="unity-version-mismatch"
  echo "ERROR: Unity version is not 6000.3.2f1" >> "$EVIDENCE_DIR/unity-version.txt"
  fail_with_evidence 22
fi

set +e
ensure_macos_protoc >"$EVIDENCE_DIR/protoc-macos-bootstrap.log" 2>&1
protoc_rc=$?
set -e
if [[ "$protoc_rc" -ne 0 ]]; then
  FAILED_STEP="protoc-macos-bootstrap"
  fail_with_evidence "$protoc_rc"
fi

set +e
ensure_unity_managed_dependencies >"$EVIDENCE_DIR/unity-managed-dependencies.log" 2>&1
deps_rc=$?
set -e
if [[ "$deps_rc" -ne 0 ]]; then
  FAILED_STEP="unity-managed-dependencies"
  fail_with_evidence "$deps_rc"
fi

run_capture prepare-unity-protocol env PROTOC_BIN="$PROTOC_BIN" PROTOC_SHA256="$PROTOC_SHA256" python3 "$ROOT/tools/prepare_unity_protocol.py" || fail_with_evidence $?

run_capture unity-import-generate "$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$PROJECT" \
  -executeMethod LinhGioi.Foundation.Editor.M0ProjectGenerator.RebuildGeneratedFoundation \
  -logFile "$EVIDENCE_DIR/unity-import-generate.log" || fail_with_evidence $?

RESULTS_XML="$EVIDENCE_DIR/unity-editmode-results.xml"
run_capture unity-editmode "$UNITY_EDITOR" -batchmode -nographics \
  -projectPath "$PROJECT" \
  -runTests -testPlatform EditMode \
  -testResults "$RESULTS_XML" \
  -logFile "$EVIDENCE_DIR/unity-editmode.log" || fail_with_evidence $?

if [[ ! -s "$RESULTS_XML" ]] || ! grep -q '<test-case' "$RESULTS_XML"; then
  FAILED_STEP="unity-editmode-results"
  {
    echo "ERROR: Unity EditMode command exited 0 but did not write a non-empty test result XML with test-case entries."
    echo "expected=$RESULTS_XML"
    ls -lh "$EVIDENCE_DIR" || true
  } > "$EVIDENCE_DIR/unity-editmode-results-check.log"
  fail_with_evidence 24
fi

PLAYER_EXE="$PLAYER_DIR/$PLAYER_NAME"
run_capture unity-linux-player-build env LGO_PLAYER_OUTPUT="$PLAYER_EXE" "$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$PROJECT" \
  -executeMethod LinhGioi.Foundation.Editor.M0LinuxPlayerEvidenceBuilder.BuildLinuxPlayerSmoke \
  -logFile "$EVIDENCE_DIR/unity-linux-player-build.log" || fail_with_evidence $?

test -x "$PLAYER_EXE" || {
  FAILED_STEP="player-executable-missing"
  echo "ERROR: expected player executable missing: $PLAYER_EXE" > "$EVIDENCE_DIR/player-executable-check.log"
  fail_with_evidence 23
}

find "$PLAYER_DIR" -type f ! -name '._*' | sort > "$EVIDENCE_DIR/player-file-list.txt"
if [[ -d "$PROJECT/Assets/Game/Generated" ]]; then
  find "$PROJECT/Assets/Game/Generated" -type f | sort > "$EVIDENCE_DIR/generated-unity-file-list.txt"
else
  echo "WARN: generated Unity directory missing" > "$EVIDENCE_DIR/generated-unity-file-list.txt"
fi

write_summary

PLAYER_ARCHIVE="$OUT_DIR/lgo-unity-player-smoke-linux-6000.3.2f1-$TIMESTAMP.tar.gz"
COPYFILE_DISABLE=1 python3 - "$PLAYER_DIR" "$PLAYER_ARCHIVE" <<'PYTAR'
from pathlib import Path
import sys, tarfile
source = Path(sys.argv[1])
out = Path(sys.argv[2])
with tarfile.open(out, 'w:gz') as tf:
    for path in sorted(source.rglob('*')):
        rel = path.relative_to(source)
        if any(part.startswith('._') for part in rel.parts):
            continue
        tf.add(path, arcname=str(rel), recursive=False)
PYTAR
sha256_file "$PLAYER_ARCHIVE" > "$PLAYER_ARCHIVE.sha256"

EVIDENCE_ZIP="$(zip_evidence)"

cat > "$OUT_DIR/UPLOAD-THESE-FILES.txt" <<EOF
Upload these files to the sandbox:
$(basename "$PLAYER_ARCHIVE")
$(basename "$PLAYER_ARCHIVE.sha256")
$(basename "$EVIDENCE_ZIP")
$(basename "$EVIDENCE_ZIP.sha256")
EOF

printf 'UNITY_PLAYER_EVIDENCE_READY\nplayer=%s\nevidence=%s\n' "$PLAYER_ARCHIVE" "$EVIDENCE_ZIP"
