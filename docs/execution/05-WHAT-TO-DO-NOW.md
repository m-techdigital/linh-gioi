# 05 — What To Do Now

Current decision: `M6_COMBAT_HARDENING_CONTINUATION_CLOSED_LOCAL_v0.56.0`.

Use the current repository source as the authoritative working baseline. Do not go back to old milestone ZIPs unless a review task explicitly asks for SHA/provenance comparison.

## Immediate Next Step

Run the continuous cycle before opening new work:

```bash
./tools/lgo_continuous_cycle.py --phase source
```

If Unity runtime evidence is required and the local Unity/player environment is available:

```bash
./tools/lgo_continuous_cycle.py --phase runtime
```

## Current Development Direction

Continue with roadmap-valid work that improves:

- playable runtime stability;
- UI/UX clarity;
- combat evidence and rejection-path diagnostics;
- source/package automation;
- asset pipeline size budgeting and import hygiene.

Do not generate or import new image assets during code-focused tasks. Future runtime art must follow `docs/art/RUNTIME-ASSET-SIZE-BUDGET.md`.

## Do Not Open Without Explicit Contract Review

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`
- production auth;
- DB implementation;
- economy, inventory rewards, social, guild, party, market, live ops;
- production-final art claims.

## Required Reading Before New Work

- `docs/execution/CONTINUOUS-DEVELOPMENT-OPERATING-MODE.md`
- `docs/execution/CODE-GOVERNANCE-CONTRACT.md`
- `docs/execution/LGO-MASTER-ROADMAP-v1.0.md`
- `docs/execution/LGO-NEXT-50-TASKS-BACKLOG-v1.0.md`
- `docs/execution/TASK-LEDGER.md`
- `docs/art/RUNTIME-ASSET-SIZE-BUDGET.md`
