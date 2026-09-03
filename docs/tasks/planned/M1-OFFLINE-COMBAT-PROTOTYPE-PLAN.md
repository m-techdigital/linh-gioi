# M1 Offline Combat Prototype — Planned Task Pack

Status: `PLANNED_LOCKED_UNTIL_M0_RUNTIME_GATE`

This is a prepared task plan only. Do not execute it until `docs/execution/07-PHASE-GATES.md` allows M1 entry.

## Goal

Build the smallest playable offline combat loop that proves Unity runtime, GameData consumption, combat state transitions, and HUD feedback without online authority or persistence.

## In scope

- One prototype scene or generated bootstrap entry.
- Player fixture and enemy fixture from existing or M1-approved GameData.
- Local deterministic combat service.
- Attack/cooldown/HP/resource state.
- Minimal HUD using existing design tokens.
- EditMode tests for combat math and state transitions.

## Out of scope

- Online combat server authority.
- Protocol/schema changes without S0 contract task.
- Account persistence.
- Inventory/economy/marketplace/guild/social production systems.
- Production balancing/content expansion.

## Entry checklist

- [ ] M0 runtime closed or owner override recorded.
- [ ] Current source SHA selected.
- [ ] Unity compile/import evidence available.
- [ ] M0 source validation still PASS.
- [ ] No contract change required; otherwise create S0 contract request first.

## Acceptance criteria

- [ ] Unity import/compile PASS.
- [ ] Combat state tests PASS.
- [ ] Player can perform at least one attack.
- [ ] Enemy HP changes and death/end state is visible.
- [ ] HUD shows player HP and cooldown/resource feedback.
- [ ] No frozen contract drift.
- [ ] Handoff contains playable evidence and rollback notes.
