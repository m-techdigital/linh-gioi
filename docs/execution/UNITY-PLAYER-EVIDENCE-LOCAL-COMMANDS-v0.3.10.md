# Linh Giới Online — Unity Player Evidence Local Commands v0.3.10

Use this with:

- `linh-gioi-m0-hybrid-runtime-support-v0.3.10-full-source.zip`
- Unity Editor `6000.3.2f1`
- Linux Build Support installed in Unity Hub.

This version fixes the v0.3.9 local Unity import failures by:

- adding an explicit `LinhGioi.UI.Editor` reference to `LinhGioi.Foundation.Editor.asmdef`;
- adding deterministic `UnityEngine.UIElementsModule` references to UI, UI editor, foundation editor, and EditMode test assemblies;
- allowing the EditMode test assembly to reference the prepared pinned `Google.Protobuf.dll`;
- changing the protocol EditMode test so it parses a canonical wire payload without directly importing `Google.Protobuf`.

Run on macOS:

```bash
cd "$HOME"
mkdir -p "$HOME/lgo-unity-player-evidence-v0310"
cd "$HOME/lgo-unity-player-evidence-v0310"

LOG="$PWD/unity-player-evidence-run-v0.3.10.log"
: > "$LOG"

SOURCE_ZIP="$(find "$HOME/Downloads" -maxdepth 1 -type f -name 'linh-gioi-m0-hybrid-runtime-support-v0.3.10-full-source*.zip' | head -n 1)"
UNITY_EDITOR="$(find /Applications/Unity/Hub/Editor -type f -path '*/6000.3.2f1/Unity.app/Contents/MacOS/Unity' | head -n 1)"

echo "SOURCE_ZIP=$SOURCE_ZIP" | tee -a "$LOG"
echo "UNITY_EDITOR=$UNITY_EDITOR" | tee -a "$LOG"

rm -rf source dist
mkdir -p source dist
unzip -q "$SOURCE_ZIP" -d source

BUILD_SCRIPT="$(find "$PWD/source" -type f -path '*/tools/unity_player_evidence/build_unity_player_evidence.sh' | head -n 1)"
SOURCE_ROOT="$(cd "$(dirname "$BUILD_SCRIPT")/../.." && pwd)"

echo "BUILD_SCRIPT=$BUILD_SCRIPT" | tee -a "$LOG"
echo "SOURCE_ROOT=$SOURCE_ROOT" | tee -a "$LOG"

chmod +x "$BUILD_SCRIPT"
"$UNITY_EDITOR" -version 2>&1 | tee -a "$LOG"

cd "$SOURCE_ROOT"
set -o pipefail
UNITY_EDITOR="$UNITY_EDITOR" \
"$BUILD_SCRIPT" \
  --output-dir "$HOME/lgo-unity-player-evidence-v0310/dist" 2>&1 | tee -a "$LOG"

echo "== OUTPUT ==" | tee -a "$LOG"
ls -lh "$HOME/lgo-unity-player-evidence-v0310/dist" 2>&1 | tee -a "$LOG"
```

If success, upload these four files from `dist/`:

```text
lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz
lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz.sha256
lgo-unity-editor-evidence-6000.3.2f1-*.zip
lgo-unity-editor-evidence-6000.3.2f1-*.zip.sha256
```

If it fails, upload the evidence ZIP and `.sha256` generated in `dist/`.

To extract key errors from a failed run:

```bash
cd "$HOME/lgo-unity-player-evidence-v0310/dist"
EVIDENCE_DIR="$(find "$PWD" -maxdepth 1 -type d -name 'evidence-*' | sort | tail -n 1)"
cd "$EVIDENCE_DIR"

echo "== KEY ERRORS =="
grep -nE "error CS|Exception|FAILED|Scripts have compiler errors|Enable the built in package|does not contain a definition" unity-import-generate.log unity-editmode.log unity-linux-player-build.log 2>/dev/null | head -n 160

echo
cat commands.log
```
