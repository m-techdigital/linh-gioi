# Linh Giới Online — Runtime Kits

Runtime kits are toolchains/cache material, not game source. Never package them into source deltas or full source successors.

## Server runtime kit

Canonical server kit used for M0 server runtime closure:

- `lgo-runtime-kit-server-macos-20260902-233921.tar.gz`
- SHA256: `2b57e1b9afef05798e263a95b753965c8b7ac158511888121b81722114862edb`

Contents:
- Oracle JDK 25 Linux x64 archive.
- Apache Maven 3.9.16 archive.
- Maven offline repository cache.
- install script.
- server runtime helper script.

Expected install path in sandbox:

```bash
mkdir -p /mnt/data/lgo-toolchains
# extract kit and run its install-toolchain.sh
source /mnt/data/lgo-toolchains/toolchain.env
java --version
javac --version
mvn --version
```

Required versions:
- Java/Javac: 25.x
- Maven: 3.9.16

## Unity runtime kit status

Unity Linux Editor addon was intentionally removed from Project Source by the project owner during the no-Unity rerun. Runtime work involving `lgo-unity-linux-editor-addon-*` must be skipped unless a future task explicitly re-uploads/restores Unity runtime bytes.

Current Unity classification:
- `UNVERIFIED_ENVIRONMENT / OWNER_DEFERRED`

Future Unity closure, when reopened, requires:
- Unity Editor 6000.3.2f1 Linux.
- successful `UNITY_EDITOR -version`.
- `./tools/unity_batch_test.sh` evidence.
- full `./tools/validate_m0_runtime.sh` classification.

## Packaging exclusions

Do not include these in project source ZIPs/deltas:
- JDK archives or extracted JDK.
- Maven binary or Maven local repository.
- Unity Editor archive or extracted Editor.
- Unity `Library/`, `Temp/`, `Logs/`, `obj/`, `bin/`.
- Maven `target/`.
- local `.env` or secrets.
- runtime smoke logs except inside evidence archives.

## Lightweight Unity player evidence kit

Added in v0.3.5 to avoid moving a multi-GB Unity Editor through constrained sandboxes.

Source-managed scripts:

```text
tools/unity_player_evidence/build_unity_player_evidence.sh
tools/unity_player_evidence/run_unity_player_smoke.sh
tools/unity_player_evidence/verify_unity_evidence_bundle.py
```

External output artifacts:

```text
lgo-unity-player-smoke-linux-6000.3.2f1-<timestamp>.tar.gz
lgo-unity-player-smoke-linux-6000.3.2f1-<timestamp>.tar.gz.sha256
lgo-unity-editor-evidence-6000.3.2f1-<timestamp>.zip
lgo-unity-editor-evidence-6000.3.2f1-<timestamp>.zip.sha256
```

This route supports sandboxes that cannot mount Unity Editor. It keeps upload size low by sending only a built Linux player smoke artifact plus Editor evidence logs from an external Unity machine/CI.

Classification rule:

- Player smoke PASS in sandbox proves Unity-built executable can handshake with Java realtime.
- External evidence proves Editor import/compile/EditMode/build occurred outside sandbox.
- Reports must not claim Unity Editor ran inside sandbox unless it actually did.
