# P8 Prompt — M1 Runtime Closure

You are closing runtime evidence for Linh Giới Online M1 Offline Combat Prototype.

## Preconditions

- M0 final decision is `M0_RUNTIME_CLOSED`.
- Source baseline is `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip` or a verified successor.
- Do not use v0.3.x/v0.4.0 artifacts except for forensic comparison.

## Required reading

- `README.md`
- `START-HERE.md`
- `docs/execution/PROJECT-STATE.md`
- `docs/tasks/M1-OFFLINE-COMBAT-PROTOTYPE.md`
- `docs/execution/M1-RUNTIME-EVIDENCE.md`
- `docs/execution/checklists/M1-RUNTIME-CLOSURE-CHECKLIST.md`
- `docs/execution/09-EVIDENCE-AND-QUALITY-STANDARD.md`
- `docs/execution/03-HANDOFF-CONTRACT.md`

## Scope

Close M1 runtime evidence only. The required offline player command is `--lgo-m1-offline-combat-smoke`. Do not implement M2, online authority, persistence, database, account, economy, guild, marketplace, PvP ranking, or large content expansion.

## Required commands

Source validation:

```bash
./tools/validate_m1_source.sh
```

Local Unity evidence on Unity machine:

```bash
UNITY_EDITOR=/path/to/Unity ./tools/m1_offline_combat_evidence/build_m1_offline_combat_evidence.sh --output-dir "$PWD/build/m1-offline-combat-evidence"
```

Sandbox evidence verification:

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

## Evidence rules

- No `skip-as-PASS`.
- No `executed=0` counted as PASS.
- No masked failures.
- Unity version must be exactly `6000.3.2f1`.
- Evidence ZIP must include `unity-editmode-results.xml`.
- M1 offline smoke must produce JSON `status=PASS` and deterministic result fields.
- Runtime claim must distinguish editor evidence from player replay evidence.

## Final decision values

Use exactly one:

- `M1_OFFLINE_COMBAT_RUNTIME_CLOSED`
- `M1_RUNTIME_ENVIRONMENT_LIMITED`
- `FIX_REQUIRED`
- `BLOCKED_CONTRACT`

Stop after handoff. Do not start M2 in this task.
