# Local macOS Setup — Linh Giới Online

Use this file when running the project directly from a local macOS checkout.

## Required tools

- Homebrew
- `python@3.12`
- `coreutils` for GNU `timeout`
- Unity Editor `6000.3.2f1` installed through Unity Hub
- Java 25 and Maven 3.9.16 for server runtime checks

Install the first two local validation dependencies:

```bash
brew install python@3.12 coreutils
```

## One-command local setup

From the project root:

```bash
bash tools/local_macos_setup.sh
```

The script will:

- configure Homebrew Python 3.12 in `PATH`;
- expose GNU `timeout` as `timeout`;
- try to create `.venv`;
- if `.venv` creation is blocked, install Python packages into a project-local user base;
- locate or download pinned macOS `protoc 3.13.0`;
- verify its exact SHA256;
- write `.lgo-local-env`;
- run `./tools/validate_m1_source.sh`.

Expected final line:

```text
M1 SOURCE VALIDATION PASS
```

## Reusing the setup later

```bash
cd "$HOME/Projects/LinhGioiOnline"
source .lgo-local-env
./tools/validate_m1_source.sh
```

## Unity project

```bash
UNITY_EDITOR="$(find /Applications/Unity/Hub/Editor -type f -path '*/6000.3.2f1/Unity.app/Contents/MacOS/Unity' | head -n 1)"
"$UNITY_EDITOR" -projectPath "$HOME/Projects/LinhGioiOnline/client/Unity"
```

Open this scene in Unity:

```text
Assets/Game/Generated/Scenes/M1OfflineCombatPrototype.unity
```

## M1 runtime evidence

Use:

```bash
cat M1-RUNTIME-EVIDENCE-LOCAL-COMMANDS-v0.5.1.md
```

or:

```bash
cat docs/execution/M1-RUNTIME-EVIDENCE-LOCAL-COMMANDS-v0.5.1.md
```

M1 is not runtime-closed until Unity runtime evidence passes.
