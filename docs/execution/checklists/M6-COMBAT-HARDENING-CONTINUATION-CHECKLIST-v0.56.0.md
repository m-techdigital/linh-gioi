# M6 Combat Hardening Continuation Checklist v0.56.0

Marker: `M6_COMBAT_HARDENING_CONTINUATION_CHECKLIST_READY_v0.56.0`

## Scope

- Existing local combat accepted path keeps the same HP, amount, cooldown, range, and Vietnamese feedback.
- Existing server-authoritative pilot accepted and rejected paths keep the same protocol messages and GameData values.
- No new combat mechanics are introduced.

## Runtime Evidence

- Local smoke records accepted intent id, sequence, cooldown, outcome, snapshot validity, and target HP after hit.
- Local smoke records NO_TARGET, OUT_OF_RANGE, and COOLDOWN_ACTIVE rejection diagnostics.
- Unity-Java E2E smoke records accepted/result/snapshot details.
- Unity-Java E2E smoke records rejection codes, retryable flags, and cooldown remaining diagnostics.

## Frozen Surface Audit

- `protocol/**` unchanged.
- `gamedata/schemas/**` unchanged.
- `docs/adr/**` unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json` unchanged.

## Package Hygiene

- No `__pycache__`, `.pyc`, `.DS_Store`, `__MACOSX`, Unity cache, or generated package clutter is required for this task.
- Cache/package clutter is enforced by `tools/validate_package_hygiene.py`, so the v0.56 validator can run cleanly after `py_compile`.
