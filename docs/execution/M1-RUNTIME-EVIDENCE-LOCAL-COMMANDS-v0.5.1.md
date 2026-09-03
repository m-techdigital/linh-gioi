# Linh Giới Online — M1 Runtime Evidence Local Commands v0.5.1

Use this on macOS after downloading `linh-gioi-m1-independent-audit-v0.5.1-full-source.zip` into `~/Downloads`.

```bash
cd "$HOME"
mkdir -p "$HOME/lgo-m1-runtime-evidence-v051"
cd "$HOME/lgo-m1-runtime-evidence-v051"

LOG="$PWD/m1-runtime-evidence-v0.5.1.log"
: > "$LOG"

SOURCE_ZIP="$(find "$HOME/Downloads" -maxdepth 1 -type f -name 'linh-gioi-m1-independent-audit-v0.5.1-full-source*.zip' | head -n 1)"
UNITY_EDITOR="$(find /Applications/Unity/Hub/Editor -type f -path '*/6000.3.2f1/Unity.app/Contents/MacOS/Unity' | head -n 1)"

echo "SOURCE_ZIP=$SOURCE_ZIP" | tee -a "$LOG"
echo "UNITY_EDITOR=$UNITY_EDITOR" | tee -a "$LOG"

rm -rf source dist
mkdir -p source dist
unzip -q "$SOURCE_ZIP" -d source

BUILD_SCRIPT="$(find "$PWD/source" -type f -path '*/tools/m1_offline_combat_evidence/build_m1_offline_combat_evidence.sh' | head -n 1)"
SOURCE_ROOT="$(cd "$(dirname "$BUILD_SCRIPT")/../.." && pwd)"

echo "BUILD_SCRIPT=$BUILD_SCRIPT" | tee -a "$LOG"
echo "SOURCE_ROOT=$SOURCE_ROOT" | tee -a "$LOG"

chmod +x "$BUILD_SCRIPT"
"$UNITY_EDITOR" -version 2>&1 | tee -a "$LOG"

cd "$SOURCE_ROOT"
set -o pipefail
./tools/validate_m1_source.sh 2>&1 | tee -a "$LOG"
UNITY_EDITOR="$UNITY_EDITOR" "$BUILD_SCRIPT" --output-dir "$HOME/lgo-m1-runtime-evidence-v051/dist" 2>&1 | tee -a "$LOG"

echo "== OUTPUT ==" | tee -a "$LOG"
ls -lh "$HOME/lgo-m1-runtime-evidence-v051/dist" 2>&1 | tee -a "$LOG"
```

Upload the four generated files listed in `dist/UPLOAD-THESE-FILES-M1.txt`.
