#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MODE="${1:---dry-run}"

usage() {
  cat <<'USAGE'
Usage:
  ./tools/lgo_local_artifact_cleanup.sh --dry-run
  ./tools/lgo_local_artifact_cleanup.sh --apply
  ./tools/lgo_local_artifact_cleanup.sh --apply-player-build
  ./tools/lgo_local_artifact_cleanup.sh --apply-all

Reports or removes local generated handoff archives under build/ only.
It never touches source assets, reference art, protocol, gamedata, ADR, or design tokens.
It keeps visual evidence and dev-loop logs because task ledgers may point at them.
Use --apply-player-build only when you want to delete the local Unity Player build; it can be rebuilt by the visual runtime harness.
USAGE
}

case "$MODE" in
  --dry-run|--apply|--apply-player-build|--apply-all) ;;
  --help|-h) usage; exit 0 ;;
  *) usage >&2; exit 2 ;;
esac

cd "$ROOT"
test "$(basename "$PWD")" = "LinhGioiOnline"
TARGET_LIST="$(mktemp "${TMPDIR:-/tmp}/lgo-artifact-cleanup.XXXXXX")"
trap 'rm -f "$TARGET_LIST"' EXIT

echo "LGO_LOCAL_ARTIFACT_CLEANUP_MODE ${MODE#--}"

if [[ ! -d build ]]; then
  echo "LGO_LOCAL_ARTIFACT_CLEANUP_NOTHING build directory missing"
  exit 0
fi

echo "LGO_LOCAL_ARTIFACT_CLEANUP_SIZE_BEFORE"
for dir in build build/unity-player-macos build/chatgpt-handoff build/visual-evidence build/dev-loop; do
  if [[ -d "$dir" ]]; then
    du -sh "$dir"
  fi
done

{
  if [[ "$MODE" == "--dry-run" || "$MODE" == "--apply" || "$MODE" == "--apply-all" ]]; then
    find build/chatgpt-handoff -type f \( -name '*.zip' -o -name '*.tar.gz' -o -name '*.sha256' \) 2>/dev/null
  fi
  if [[ "$MODE" == "--dry-run" || "$MODE" == "--apply-player-build" || "$MODE" == "--apply-all" ]]; then
    if [[ -d build/unity-player-macos ]]; then
      printf '%s\n' "build/unity-player-macos"
    fi
  fi
} > "$TARGET_LIST"

if [[ ! -s "$TARGET_LIST" ]]; then
  echo "LGO_LOCAL_ARTIFACT_CLEANUP_NOTHING no generated handoff/log artifacts matched"
  exit 0
fi

echo "LGO_LOCAL_ARTIFACT_CLEANUP_TARGETS"
cat "$TARGET_LIST"

if [[ "$MODE" == "--dry-run" ]]; then
  echo "LGO_LOCAL_ARTIFACT_CLEANUP_RESULT DRY_RUN"
  exit 0
fi

while IFS= read -r target; do
  case "$target" in
    build/chatgpt-handoff/*)
      rm -f "$target"
      ;;
    build/unity-player-macos)
      rm -rf "$target"
      ;;
    *)
      echo "FIX_REQUIRED refusing to remove unexpected path: $target" >&2
      exit 3
      ;;
  esac
done < "$TARGET_LIST"

echo "LGO_LOCAL_ARTIFACT_CLEANUP_SIZE_AFTER"
for dir in build build/unity-player-macos build/chatgpt-handoff build/visual-evidence build/dev-loop; do
  if [[ -d "$dir" ]]; then
    du -sh "$dir"
  fi
done
echo "LGO_LOCAL_ARTIFACT_CLEANUP_RESULT APPLIED"
