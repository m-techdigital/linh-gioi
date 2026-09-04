# 06 — Project Governance Index

Purpose: make Linh Giới Online executable by multiple sandboxes without losing baseline, runtime truth, ownership, or delivery order.

This file is the navigation hub for all planning, progress, gate, and handoff documents. It does not open feature work by itself.

## Current truth

- Current milestone: `M5 Guided Training Loop`.
- Current safe implementation state: `M5_GUIDED_TRAINING_LOOP_SOURCE_READY`.
- Current accepted foundation: `M0_RUNTIME_CLOSED` from `linh-gioi-m0-runtime-closed-v0.4.1-full-source.zip`.
- Current accepted gameplay baseline: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` from `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip`.
- Current active runtime follow-up: M4 playable vertical slice verification using Java Netty + Unity `6000.3.2f1` player smoke.
- M3 server/API persistence is closed as `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED`; M3-B remains covered by its dedicated runtime smoke evidence.

## Read order for every new sandbox

1. `START-HERE.md`
2. `docs/execution/PROJECT-STATE.md`
3. `docs/execution/MILESTONE-ROADMAP.md`
4. `docs/execution/07-PHASE-GATES.md`
5. `docs/execution/08-DELIVERY-CADENCE.md`
6. `docs/execution/09-EVIDENCE-AND-QUALITY-STANDARD.md`
7. `docs/execution/03-HANDOFF-CONTRACT.md`
8. Relevant task/prompt file under `docs/tasks/` or `docs/execution/prompts/`

## Control documents

| Document | Use |
|---|---|
| `docs/execution/PROJECT-STATE.md` | Current milestone, accepted baseline, closed/deferred gates. |
| `docs/execution/TASK-LEDGER.md` | Historical task status, accepted overlays, next allowed step. |
| `docs/execution/MILESTONE-ROADMAP.md` | M0-M7 milestone sequence and high-level gates. |
| `docs/execution/07-PHASE-GATES.md` | Detailed entry/exit gates, owner approvals, runtime rows. |
| `docs/execution/08-DELIVERY-CADENCE.md` | How work is split into batches/sandboxes without drift. |
| `docs/execution/09-EVIDENCE-AND-QUALITY-STANDARD.md` | Evidence rules, anti-overclaim, test validity. |
| `docs/execution/10-OWNER-REVIEW-PLAYBOOK.md` | How the owner reviews a handoff and accepts/rejects it. |
| `docs/execution/11-RISK-REGISTER.md` | Known risks, mitigations, trigger conditions. |
| `docs/execution/12-DESIGN-PREPRODUCTION-PLAN.md` | Visual/gameplay preparation before feature implementation. |

## Runtime evidence docs

- `docs/execution/M1-RUNTIME-EVIDENCE.md`
- `docs/execution/M2-RUNTIME-EVIDENCE.md`

## Design boards

- `docs/reference-art/design-boards/roadmap-flow.svg`
- `docs/reference-art/design-boards/m0-to-m1-gate.svg`
- `docs/reference-art/design-boards/core-gameplay-loop.svg`
- `docs/reference-art/design-boards/hud-wireframe.svg`
- `docs/reference-art/design-boards/world-hub-wireframe.svg`
- `docs/reference-art/design-boards/production-board.svg`

These are planning/reference assets. They are not production art, UI prefabs, scenes, or gameplay implementation.


## Active M4 documents

- `docs/tasks/M4-PLAYABLE-VERTICAL-SLICE.md`
- `docs/tasks/M4-PLAYABLE-UI-AND-ART-QUALITY-PASS-v0.12.0.md`
- `docs/tasks/M4-PLAYABLE-SLICE-STABILIZATION-v0.13.0.md`
- `docs/execution/M4-CLOSURE-COMMAND-v0.13.0.md`
- `docs/tasks/M4-VISIBLE-UI-USABILITY-AND-REVIEW-HARNESS-v0.14.0.md`
- `docs/execution/M4-VISIBLE-UI-REVIEW-COMMAND-v0.14.0.md`
- `docs/tasks/M5-FIRST-PLAYABLE-LOOP-FOUNDATION-v0.15.0.md`
- `docs/execution/LGO-PLAYABLE-CLOSURE-COMMAND-v0.15.0.md`
- `docs/tasks/M5-VISUAL-EVIDENCE-AND-UX-ACCEPTANCE-v0.16.0.md`
- `docs/execution/LGO-VISUAL-EVIDENCE-REVIEW-COMMAND-v0.16.0.md`
- `docs/art/LGO-VISUAL-REFERENCE-PACK-v0.16.5.md`
- `docs/tasks/M5-GUIDED-TRAINING-LOOP-v0.17.0.md`
- `docs/execution/prompts/M4-2-PLAYABLE-UI-REDESIGN-FROM-DESIGN-LOCK.md`
- `docs/tasks/M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION.md`
- `docs/execution/prompts/P11-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION.md`
- `docs/execution/M3B-UNITY-ACCOUNT-CHARACTER-EVIDENCE.md`
- `docs/execution/checklists/M3B-UNITY-ACCOUNT-CHARACTER-CLOSURE-CHECKLIST.md`
