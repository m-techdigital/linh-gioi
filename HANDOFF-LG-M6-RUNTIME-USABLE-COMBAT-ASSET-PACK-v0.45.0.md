# Handoff: LG M6 Runtime-Usable Combat Asset Pack v0.45.0

Decision: `M6_RUNTIME_USABLE_COMBAT_ASSET_PACK_INGEST_SOURCE_CLOSED_v0.45.0`

Ingested assets:
- `docs/reference-art/v0.45.0/runtime-assets/target-dummy-idle-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/target-dummy-selected-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/target-dummy-hit-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/target-dummy-recover-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/skill-wind-slash-frame-01-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/skill-wind-slash-frame-02-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/skill-wind-slash-frame-03-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/skill-wind-slash-frame-04-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/skill-impact-spark-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/cooldown-ring-ready-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/cooldown-ring-cooldown-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/target-marker-selected-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/warning-telegraph-circle-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/combat-button-normal-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/combat-button-pressed-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/combat-button-cooldown-v0450.png`
- `docs/reference-art/v0.45.0/runtime-assets/combat-panel-9slice-v0450.png`

Validation gates:
- `python3.12 -m py_compile tools/validate_m6_runtime_usable_combat_asset_pack.py`
- `git --no-pager diff --check`
- `python3.12 tools/validate_m6_runtime_usable_combat_asset_pack.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

Notes:
- The preview board is reference evidence only.
- Unity import/wiring belongs to v0.46.
