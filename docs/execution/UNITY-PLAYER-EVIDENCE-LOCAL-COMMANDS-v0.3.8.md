# Unity Player Evidence Local Commands v0.3.8

Run on macOS with Unity `6000.3.2f1` installed.

```bash
cd "$HOME"
mkdir -p "$HOME/lgo-unity-player-evidence-v038"
cd "$HOME/lgo-unity-player-evidence-v038"

LOG="$PWD/unity-player-evidence-run-v0.3.8.log"
: > "$LOG"

SOURCE_ZIP="$(find "$HOME/Downloads" -maxdepth 1 -type f -name 'linh-gioi-m0-hybrid-runtime-support-v0.3.8-full-source*.zip' | head -n 1)"
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
  --output-dir "$HOME/lgo-unity-player-evidence-v038/dist" 2>&1 | tee -a "$LOG"

echo "== OUTPUT ==" | tee -a "$LOG"
ls -lh "$HOME/lgo-unity-player-evidence-v038/dist" 2>&1 | tee -a "$LOG"
```

On success, upload the four generated files from `~/lgo-unity-player-evidence-v038/dist/`:

```text
lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz
lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz.sha256
lgo-unity-editor-evidence-6000.3.2f1-*.zip
lgo-unity-editor-evidence-6000.3.2f1-*.zip.sha256
```

On failure, upload the evidence ZIP + SHA generated in the same `dist/` directory.
