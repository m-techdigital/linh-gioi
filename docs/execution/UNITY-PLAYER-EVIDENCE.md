# Linh Giới Online — Unity Player Evidence Strategy

Status: `SUPPORTED_FROM_v0.3.5`
Scope: M0 runtime-closure support only; no gameplay feature work.

## Why this exists

The Unity Editor 6000.3.2f1 Linux toolchain is several GB and is not reliable to upload or mount in constrained sandboxes. The M0 server runtime is already closed, so the remaining risk is Unity runtime evidence. This strategy avoids uploading the full Editor by splitting responsibility:

- External Unity machine/CI: runs the Editor, imports the project, compiles C#, runs EditMode tests, and builds a small Linux player smoke artifact.
- Sandbox: verifies the evidence bundle, runs the small Linux player against the real Java realtime server, and records player-to-server handshake evidence.

This does not package Unity Editor, Unity `Library/`, Unity `Temp/`, Maven cache, JDK, or local secrets into source artifacts.

## What this can prove inside sandbox

When provided a Linux player smoke archive built from this source, sandbox can verify:

- player archive SHA256;
- editor evidence bundle SHA256;
- Unity version evidence includes `6000.3.2f1`;
- EditMode test results contain real test cases;
- Linux player build evidence exists;
- real Java realtime server starts in sandbox;
- Unity-built Linux player starts in sandbox;
- Unity player sends `ClientHello` through production `TcpRealtimeClient`;
- Java realtime server returns `ServerHello`;
- player result JSON reports accepted handshake;
- server shutdown is graceful.

## What this does not prove by itself

Sandbox still does not run Unity Editor. Therefore, unless the external evidence is accepted by project owner policy, this remains weaker than a full in-sandbox Editor gate. It does not make a false claim that Unity Editor was installed in sandbox.

## Artifacts produced by external runner

The runner creates four uploadable files:

```text
lgo-unity-player-smoke-linux-6000.3.2f1-<timestamp>.tar.gz
lgo-unity-player-smoke-linux-6000.3.2f1-<timestamp>.tar.gz.sha256
lgo-unity-editor-evidence-6000.3.2f1-<timestamp>.zip
lgo-unity-editor-evidence-6000.3.2f1-<timestamp>.zip.sha256
```

Typical size is expected to be far below the full Unity Editor archive. Exact size depends on Unity build output and platform support.

## Commands

External machine with Unity 6000.3.2f1 and Linux Build Support:

```bash
export UNITY_EDITOR=/absolute/path/to/Unity
./tools/unity_player_evidence/build_unity_player_evidence.sh --output-dir /tmp/lgo-unity-evidence
```

Sandbox after upload:

```bash
python3 ./tools/unity_player_evidence/verify_unity_evidence_bundle.py \
  --player-archive /mnt/data/lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz \
  --player-sha256 /mnt/data/lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz.sha256 \
  --evidence-zip /mnt/data/lgo-unity-editor-evidence-6000.3.2f1-*.zip \
  --evidence-sha256 /mnt/data/lgo-unity-editor-evidence-6000.3.2f1-*.zip.sha256

./tools/unity_player_evidence/run_unity_player_smoke.sh \
  --player-archive /mnt/data/lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz
```

## Allowed final decisions

- `M0_RUNTIME_CLOSED`: only if project policy accepts external Unity Editor evidence plus sandbox player smoke as sufficient, or if Unity Editor also runs in sandbox.
- `M0_RUNTIME_CLOSED_WITH_EXTERNAL_UNITY_EDITOR_EVIDENCE`: allowed only with explicit owner acceptance and clear report wording.
- `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED`: default if external evidence is unavailable or not accepted as full Editor-gate substitute.
- `FIX_REQUIRED`: if player smoke, Editor evidence, source validation, server runtime, or checksum verification fails due project/source/tooling defects.

## Non-claims

Do not claim these unless actually executed and evidenced:

- Unity Editor installed in sandbox;
- Unity project import in sandbox;
- Unity package restore in sandbox;
- Unity EditMode tests in sandbox;
- Unity full Editor gate in sandbox.

## v0.3.6 local-run diagnostic behavior

The Unity player evidence builder now records each Unity batchmode command through
`commands.log`. If Unity import, EditMode tests, or Linux player build fails, the
script still emits a failure evidence ZIP with the available logs and exits
non-zero. This prevents shell pipelines such as `... | tee log` from hiding the
failing Unity step.

## v0.3.7 macOS protoc bootstrap note

`tools/unity_player_evidence/build_unity_player_evidence.sh` now bootstraps a temporary macOS `protoc 3.13.0` when `PROTOC_BIN` is not provided. This is necessary because the repository bundles a Linux x86_64 protoc for sandbox validation, while local Unity evidence may be built on macOS. The helper records `PROTOC_BIN`, exact `PROTOC_SHA256`, and `protoc --version` in `protoc-macos-bootstrap.log`; `protocol_codegen.py` still verifies the compiler version is exactly `libprotoc 3.13.0`.

On Apple Silicon, the downloaded x86_64 protoc may require Rosetta. If bootstrap fails with CPU architecture errors, run:

```bash
softwareupdate --install-rosetta --agree-to-license
```

## v0.3.9 local Unity retry notes

When running Unity evidence locally, the source must enable `com.unity.modules.uielements` in `client/Unity/Packages/manifest.json`. The generated protocol assembly should not override references for `Google.Protobuf.dll`; the pinned DLL is prepared under `Assets/Plugins/Google.Protobuf` and left for Unity to auto-reference during import.
