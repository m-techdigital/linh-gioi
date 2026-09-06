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
  ./tools/lgo_local_artifact_cleanup.sh --apply-unity-cache
  ./tools/lgo_local_artifact_cleanup.sh --apply-all

Reports or removes local generated handoff archives under build/ and optional Unity local caches.
It never touches source assets, reference art, protocol, gamedata, ADR, or design tokens.
It keeps visual evidence and dev-loop logs because task ledgers may point at them.
Use --apply-player-build only when you want to delete the local Unity Player build; it can be rebuilt by the visual runtime harness.
Use --apply-unity-cache only when you want to delete client/Unity/Library; Unity can rebuild it, but the next Editor/build run may be slow.
--apply-all intentionally excludes client/Unity/Library so broad cleanup does not trigger an expensive Unity reimport by accident.
USAGE
}

case "$MODE" in
  --dry-run|--apply|--apply-player-build|--apply-unity-cache|--apply-all) ;;
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
for dir in build build/unity-player-macos build/chatgpt-handoff build/visual-evidence build/dev-loop client/Unity/Library; do
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
  if [[ "$MODE" == "--dry-run" || "$MODE" == "--apply-unity-cache" ]]; then
    if [[ -d client/Unity/Library ]]; then
      printf '%s\n' "client/Unity/Library"
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
    client/Unity/Library)
      if [[ "$MODE" != "--apply-unity-cache" ]]; then
        echo "FIX_REQUIRED refusing Unity cache removal outside --apply-unity-cache: $target" >&2
        exit 3
      fi
      rm -rf "$target"
      ;;
    *)
      echo "FIX_REQUIRED refusing to remove unexpected path: $target" >&2
      exit 3
      ;;
  esac
done < "$TARGET_LIST"

echo "LGO_LOCAL_ARTIFACT_CLEANUP_SIZE_AFTER"
for dir in build build/unity-player-macos build/chatgpt-handoff build/visual-evidence build/dev-loop client/Unity/Library; do
  if [[ -d "$dir" ]]; then
    du -sh "$dir"
  fi
done
echo "LGO_LOCAL_ARTIFACT_CLEANUP_RESULT APPLIED"
