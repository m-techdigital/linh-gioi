# M1 Runtime Evidence — Offline Combat Prototype

Status: `RUNTIME_CLOSED`

This document defines the runtime evidence required before `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` can be claimed.

## Scope

M1 runtime evidence proves only the offline deterministic combat prototype. It does not prove online session authority, account persistence, economy, guild, marketplace, PvP ranking, or live content operations.

## Required environment

- Unity Editor `6000.3.2f1` with Linux Build Support.
- Project source `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip` or a verified successor.
- Python 3 for source/evidence validators.
- A sandbox or Linux host capable of running the generated Linux player, preferably with `xvfb-run`.

## Local Unity evidence command

From repo root on a Unity-capable workstation:

```bash
python3 -m pip install -r tools/requirements.txt
./tools/validate_m1_source.sh
UNITY_EDITOR=/path/to/Unity ./tools/m1_offline_combat_evidence/build_m1_offline_combat_evidence.sh --output-dir "$PWD/build/m1-offline-combat-evidence"
```

The command must create four files:

```text
lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz
lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz.sha256
lgo-unity-editor-evidence-6000.3.2f1-*.zip
lgo-unity-editor-evidence-6000.3.2f1-*.zip.sha256
```

## Evidence ZIP must contain

- `unity-version.txt`
- `commands.log`
- `prepare-unity-protocol.log`
- `unity-import-generate.log`
- `unity-editmode.log`
- `unity-editmode-results.xml`
- `unity-linux-player-build.log`
- `unity-evidence-summary.md`
- `generated-unity-file-list.txt`
- `player-file-list.txt`

`unity-editmode-results.xml` must contain nonzero tests, zero failed tests, and zero skipped tests. The expected M1 tests include catalog parsing, duplicate/missing catalog rejection, invalid request rejection, deterministic damage, cooldown rejection, out-of-range rejection, deterministic victory, and HUD binding.

## Sandbox replay command

After uploading the four generated files to a Linux-capable sandbox, verify the evidence bundle and run the offline combat smoke. The player must be launched with `--lgo-m1-offline-combat-smoke`:

```bash
python3 tools/m1_offline_combat_evidence/verify_m1_evidence_bundle.py \
  --player-archive <player.tar.gz> \
  --player-sha256 <player.tar.gz.sha256> \
  --evidence-zip <evidence.zip> \
  --evidence-sha256 <evidence.zip.sha256>

./tools/m1_offline_combat_evidence/run_m1_offline_combat_smoke.sh \
  --player-archive <player.tar.gz> \
  --gamedata-manifest gamedata/compiled/gamedata-manifest.json \
  --output-dir build/m1-offline-combat-smoke-sandbox
```

Expected JSON facts:

```json
{
  "status": "PASS",
  "result": {
    "targetDefeated": true,
    "skillId": "skill.sword.wind_slash",
    "enemyContentId": "monster.shadow.slime"
  }
}
```

## Runtime closure decision

`M1_OFFLINE_COMBAT_RUNTIME_CLOSED` requires all rows below to pass:

| Gate | Required result |
|---|---|
| Source validation | `./tools/validate_m1_source.sh` PASS |
| Unity version | exactly `6000.3.2f1` |
| Unity import/generate | exit code 0 |
| Unity EditMode | nonzero tests, 0 failed, 0 skipped |
| Linux player build | build log proves success and executable exists |
| Evidence verifier | `M1_EVIDENCE_BUNDLE_VERIFY_PASS` |
| Offline smoke replay | `M1_OFFLINE_COMBAT_PLAYER_SMOKE_PASS` |
| Frozen contract audit | protocol/schema/ADR/design tokens unchanged |

If any runtime row is missing, classify as `M1_RUNTIME_ENVIRONMENT_LIMITED` or `FIX_REQUIRED`, not PASS.
