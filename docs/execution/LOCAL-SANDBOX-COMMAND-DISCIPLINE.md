# Linh Giới Online — Local Sandbox Command Discipline

Purpose: keep every future sandbox aligned on the same local command assumptions and avoid repeating the failures encountered during M3-B Unity account/character runtime closure.

## Required defaults for assistant-provided commands

- Assume the operator is standing at the repository root: `LinhGioiOnline`.
- Prefer commands that can be pasted directly from repo root.
- Do not use placeholder values in executable verification commands.
- Do not use `...` in commands.
- Do not use `|| true` in validation, build, test, smoke, or runtime gates.
- Avoid heredoc blocks unless strictly necessary; they can leave the shell at a continuation prompt (`>`).
- Stop before runtime smoke if the target player executable does not exist.
- Treat missing tools, incompatible binaries, unavailable Unity modules, or host/player architecture mismatch as `UNVERIFIED_ENVIRONMENT`, not PASS.
- Do not claim a milestone or runtime slice closed until the required runtime marker is observed on the current source/toolchain/provenance.
- Evidence that is still valid on the same source/toolchain/provenance may be reused; do not rerun solely to make a newer log.

## ZIP handoff defaults

- Full source ZIP and delta ZIP must be directly unzip-able from repo root.
- Delta ZIP must not contain an extra parent directory.
- Deletions must be recorded explicitly when present.
- Final handoff/report/evidence/SHA-summary files are required for final closure handoff; for routine source updates, provide full source ZIP, delta ZIP, and SHA256 first.

## Unity generated protocol rule

For Unity runtime/build gates, generated C# protocol files must exist under the stable protocol assembly path before opening/building Unity:

```bash
python3.12 tools/prepare_unity_protocol.py --output "$PWD/client/Unity/Assets/Game/Protocol/Generated"
```

Expected files:

```text
client/Unity/Assets/Game/Protocol/Generated/Combat.cs
client/Unity/Assets/Game/Protocol/Generated/Common.cs
client/Unity/Assets/Game/Protocol/Generated/Handshake.cs
client/Unity/Assets/Game/Protocol/Generated/Movement.cs
client/Unity/Assets/Game/Protocol/Generated/Social.cs
client/Unity/Assets/Game/Protocol/Generated/WorldEvent.cs
```

Do not run source packaging hygiene validation after generating disposable Unity protocol files unless the command is explicitly intended to check generated local state.

## Current macOS player path

Unity `StandaloneOSX` output creates the executable at:

```text
build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity
```

Do not use this incorrect path:

```text
build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/LinhGioiOnline
```

## M3-B local verification command sequence from repo root

```bash
cd "$HOME/Projects/LinhGioiOnline"

export PATH="$(brew --prefix python@3.12)/bin:$PATH"
source "$PWD/.lgo-local-env"

rm -rf "$PWD/client/Unity/Assets/Game/Generated" "$PWD/client/Unity/Assets/Game/Generated.meta" "$PWD/client/Unity/Assets/Game/Protocol/Generated" "$PWD/client/Unity/Assets/Game/Protocol/Generated.meta"

./tools/validate_m3b_source.sh
./tools/prepare_unity_local_assets.sh
python3.12 tools/prepare_unity_protocol.py --output "$PWD/client/Unity/Assets/Game/Protocol/Generated"

rm -rf "$PWD/client/Unity/Library" "$PWD/client/Unity/Temp" "$PWD/client/Unity/Logs"
mkdir -p "$PWD/build/unity-player-macos"

"$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$PWD/client/Unity" -executeMethod LinhGioi.Foundation.Editor.M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke --lgo-player-output "$PWD/build/unity-player-macos/LinhGioiOnline.app" -logFile "$PWD/build/unity-player-macos/build.log"

MACOS_PLAYER="$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
chmod +x "$MACOS_PLAYER"

./server/build.sh
./tools/run_m3b_unity_account_character_once.sh --unity-player "$MACOS_PLAYER"
```

Expected M3-B runtime closure marker:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS
```

## M3-B final closure scope

Accepted local closure status for this slice:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_CLOSED_LOCAL
```

This does not claim all of M0 runtime closed and does not open M4.
