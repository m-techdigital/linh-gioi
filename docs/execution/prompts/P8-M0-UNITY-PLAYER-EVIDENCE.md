# P8 — M0 Unity Player Evidence Runner

Task ID: `LG-M0-UNITY-PLAYER-EVIDENCE`

Use this prompt when the Unity Editor itself is too large to mount in sandbox, but a local/CI machine can run Unity 6000.3.2f1 and produce a smaller Linux player smoke artifact.

## Input

Authoritative source must be either:

- `linh-gioi-m0-server-runtime-closed-unity-env-limited-v0.3.4-full-source.zip`, or
- an accepted byte-identical successor.

Do not start from older source/delta unless rebuilding the accepted successor by exact SHA.

## External runner objective

On a machine with Unity Editor 6000.3.2f1 and Linux Build Support installed:

1. Verify source artifact/SHA.
2. Run `tools/prepare_unity_protocol.py`.
3. Run Unity project import/generator.
4. Run Unity EditMode tests with result XML.
5. Build Linux player smoke artifact.
6. Package evidence ZIP and player tar.gz.
7. Compute SHA256 files.
8. Upload only small evidence/player artifacts to sandbox.

Command:

```bash
export UNITY_EDITOR=/absolute/path/to/Unity
./tools/unity_player_evidence/build_unity_player_evidence.sh --output-dir /tmp/lgo-unity-evidence
```

## Sandbox objective

After upload, sandbox must verify evidence and run the built player against the real Java realtime server:

```bash
python3 ./tools/unity_player_evidence/verify_unity_evidence_bundle.py \
  --player-archive /mnt/data/lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz \
  --player-sha256 /mnt/data/lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz.sha256 \
  --evidence-zip /mnt/data/lgo-unity-editor-evidence-6000.3.2f1-*.zip \
  --evidence-sha256 /mnt/data/lgo-unity-editor-evidence-6000.3.2f1-*.zip.sha256

./tools/unity_player_evidence/run_unity_player_smoke.sh \
  --player-archive /mnt/data/lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz
```

## Rules

- Do not upload Unity Editor.
- Do not include Unity `Library/`, `Temp/`, `Logs/`, Maven cache, JDK, or secrets in source packages.
- Do not mock Unity player runtime.
- Do not count source inspection as Unity runtime PASS.
- Do not claim in-sandbox Unity Editor gate unless Unity Editor actually ran in sandbox.
- Do not start M1 Combat in this task.

## Handoff

Return:

- `UNITY-PLAYER-EVIDENCE-REPORT.md`
- player/evidence artifact SHA256 values
- command logs
- pass/fail classification
- clear non-claims
- next allowed step
