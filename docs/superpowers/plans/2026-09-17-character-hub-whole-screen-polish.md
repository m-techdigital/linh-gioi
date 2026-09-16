# Character Hub Whole-Screen Polish Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hoàn thiện bốn screen ngoài Skill và audit lại toàn Character Hub bằng shared base, truthful state và responsive landscape.

**Architecture:** Sửa shared interaction/detail semantics trước, sau đó Character+Bag, Potential+SpiritPet, responsive polish và final 5-tab audit. Không thay actor/gameplay contract; mọi screen tái sử dụng shell/tab/inspector/action base.

**Tech Stack:** Unity 6000.3.2f1, UI Toolkit, C#, Python validators/capture.

**Spec:** `docs/superpowers/specs/2026-09-17-character-hub-whole-screen-polish-design.md`

## Global Constraints
- Không đổi branch/worktree, actor/renderer/frozen surfaces.
- Không bịa gameplay data; unavailable actions phải trung thực.
- Cùng landscape composition trên PC/mobile/tablet.
- TDD cho behavior/bug fix; Player evidence khi runtime đổi.

### Task 1: Shared unavailable/action semantics
- [x] RED test: unavailable actions visually distinct and non-interactive.
- [x] Implement one shared disabled/action treatment in `CongDongLamArrivalHud.Skin.cs`.
- [x] GREEN targeted EditMode + shared UI validator.

### Task 2: Character + Bag correctness
- [x] RED test: equipment rail must not label item level as enhancement `+N`.
- [x] RED test: zero-count supply/reward is not rendered as owned `x0` inventory content.
- [x] Implement truthful label/visibility and compact empty facts using shared inspector.
- [x] GREEN tests; inspect PC/tablet/mobile Character+Bag.
### Task 3: Potential read-only affordance
- [ ] RED test: add/reset remain disabled and expose unavailable semantics.
- [ ] Keep node selection interactive; make locked write actions visually unmistakable.
- [ ] GREEN tests + Player audit three viewports.

### Task 4: Spirit Pet density and locked roster
- [ ] RED test: locked roster cards cannot trigger detail/deploy behavior.
- [ ] Compact fact/skill rows for tablet without changing hierarchy.
- [ ] Make deployed/develop actions truthful and visually distinct.
- [ ] GREEN tests + Player audit three viewports.

### Task 5: Responsive/shared polish
- [ ] Audit font/density/touch targets across all five tabs.
- [ ] Adjust only shared scale/density rules; no per-class offsets.
- [ ] Validate no stack/crop/overflow at 1600×900, 1600×720, 1024×768.

### Task 6: Final Character Hub regression audit
- [ ] One final Player build after all runtime changes.
- [ ] Capture all five tabs and key selection/search/locked/empty states on three viewports.
- [ ] Compare visually against canonical screens; record remaining non-contract gaps.
- [ ] Full Unity/Python/validator/frozen-diff gates, then checkpoint/push.
