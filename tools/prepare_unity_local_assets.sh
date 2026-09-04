#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_DIR="${OUT_DIR:-$ROOT/build/local-toolchain}"
NUPKG="$OUT_DIR/Google.Protobuf.3.13.0.nupkg"
PROJECT="$ROOT/client/Unity"
PLUGINS_DIR="$PROJECT/Assets/Plugins/Google.Protobuf"
DLL="$PLUGINS_DIR/Google.Protobuf.dll"
CLEAR_UNITY_CACHE="${LGO_UNITY_LOCAL_ASSETS_CLEAR_CACHE:-1}"

usage() {
  cat <<'USAGE'
Usage:
  ./tools/prepare_unity_local_assets.sh [--output-dir DIR] [--preserve-unity-cache]

Prepares disposable local Unity assets required before opening the project on a
fresh machine: Google.Protobuf.dll and generated C# protocol files. It does not
modify frozen protocol/schema contracts.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir) OUT_DIR="$2"; NUPKG="$OUT_DIR/Google.Protobuf.3.13.0.nupkg"; shift 2 ;;
    --preserve-unity-cache) CLEAR_UNITY_CACHE=0; shift ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

cd "$ROOT"
if [[ -f "$ROOT/.lgo-local-env" ]]; then
  source "$ROOT/.lgo-local-env"
fi

PROJECT_PROTOC="$ROOT/tools/protobuf/darwin-arm64/protoc"
PROJECT_PROTOC_SHA="$ROOT/tools/protobuf/darwin-arm64/SHA256"
if [[ -z "${PROTOC_BIN:-}" && -x "$PROJECT_PROTOC" && -f "$PROJECT_PROTOC_SHA" ]]; then
  PROTOC_BIN="$PROJECT_PROTOC"
  PROTOC_SHA256="$(awk '{print $1}' "$PROJECT_PROTOC_SHA")"
fi

echo "== Ensure Unity manifest dependencies =="
python3 - "$PROJECT/Packages/manifest.json" <<'PYMANIFEST'
from pathlib import Path
import json, sys
path = Path(sys.argv[1])
data = json.loads(path.read_text(encoding='utf-8'))
deps = data.setdefault('dependencies', {})
if deps.get('com.unity.modules.unitywebrequest') != '1.0.0':
    deps['com.unity.modules.unitywebrequest'] = '1.0.0'
    path.write_text(json.dumps(data, indent=2) + '\n', encoding='utf-8')
print('UNITY_WEBREQUEST_MODULE_READY', path)
PYMANIFEST

if [[ -z "${PROTOC_BIN:-}" || ! -x "${PROTOC_BIN:-}" ]]; then
  echo "ERROR: PROTOC_BIN is not configured. Run ./tools/local_macos_setup.sh first." >&2
  exit 10
fi
if [[ -z "${PROTOC_SHA256:-}" ]]; then
  echo "ERROR: PROTOC_SHA256 is not configured. Run ./tools/local_macos_setup.sh first." >&2
  exit 11
fi

mkdir -p "$PLUGINS_DIR" "$OUT_DIR"

if [[ ! -f "$DLL" ]]; then
  echo "== Download Google.Protobuf 3.13.0 nupkg =="
  curl -fL --retry 3 --connect-timeout 20 \
    -o "$NUPKG" \
    "https://www.nuget.org/api/v2/package/Google.Protobuf/3.13.0"

  python3 - "$NUPKG" "$DLL" <<'PY'
from pathlib import Path
import sys, zipfile
nupkg = Path(sys.argv[1])
out = Path(sys.argv[2])
with zipfile.ZipFile(nupkg) as zf:
    data = zf.read("lib/netstandard2.0/Google.Protobuf.dll")
out.parent.mkdir(parents=True, exist_ok=True)
out.write_bytes(data)
print("GOOGLE_PROTOBUF_DLL_READY", out)
PY
else
  echo "GOOGLE_PROTOBUF_DLL_ALREADY_PRESENT $DLL"
fi

ls -lh "$DLL"
shasum -a 256 "$DLL" || sha256sum "$DLL"

echo "== Generate Unity C# protocol =="
PROTOC_BIN="$PROTOC_BIN" PROTOC_SHA256="$PROTOC_SHA256" python3 tools/prepare_unity_protocol.py

echo "== Check generated protocol =="
find client/Unity/Assets/Game/Protocol -maxdepth 2 -type f -print | sort

if [[ "$CLEAR_UNITY_CACHE" == "1" ]]; then
  echo "== Clear partial Unity compile cache =="
  rm -rf client/Unity/Library/Bee
  rm -rf client/Unity/Library/ScriptAssemblies
  rm -rf client/Unity/Temp
else
  echo "== Preserve Unity compile cache for fast visual iteration =="
fi

echo "UNITY_LOCAL_ASSETS_READY"
