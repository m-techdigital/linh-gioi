# M1 Offline Combat Prototype Checklist

## Entry

- [x] M0 final decision is `M0_RUNTIME_CLOSED`.
- [x] Baseline is v0.4.1 runtime-closed source or verified successor.
- [x] No open M0 `FIX_REQUIRED` item.

## Source implementation

- [x] Offline simulator exists.
- [x] Combatants validate state bounds.
- [x] Basic attack path exists.
- [x] Skill path exists.
- [x] Cooldown rejection exists.
- [x] Range rejection exists.
- [x] Defeat/victory result exists.
- [x] Starter skill is read from compiled GameData.
- [x] Starter monster is read from compiled GameData.
- [x] Prototype HUD is isolated under combat UI assembly.
- [x] Base UI does not depend on combat.
- [x] Offline smoke command exists.
- [x] Compiled GameData version is checked.
- [x] Duplicate canonical combat IDs are rejected.
- [x] Missing starter skill/monster IDs are rejected.
- [x] Invalid combat requests reject without HP mutation.

## Tests

- [x] Static M1 validator PASS.
- [ ] Unity EditMode tests PASS on Unity `6000.3.2f1`.
- [ ] Offline player/editor smoke JSON PASS.
- [ ] M1 evidence bundle verifier PASS.
- [ ] Generated prototype scene opens.

## Regression

- [x] M0 source validation PASS in sandbox.
- [x] Protocol source unchanged.
- [x] Server production source unchanged.
- [x] GameData schemas unchanged.
- [x] Design tokens unchanged.
- [x] No generated/cache/build artifacts in source delta.

## Exit decision

Current sandbox exit: `M1_SOURCE_HARDENED_READY_FOR_RUNTIME_VERIFY`.

Runtime closure requires Unity local/CI evidence in a runtime-capable environment.
