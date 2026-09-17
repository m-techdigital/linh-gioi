# LGO Whole-flow P0 Controls Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Produce fresh three-viewport Player evidence for the current Map01A product flow and close control-integrity defects without reopening Character Hub V59 or expanding into auth/combat features.

**Architecture:** Keep `CongDongLamArrivalHud` as the only Map01A UI/input owner. Gate review-only keyboard mutations behind one explicit opt-in policy, fix player-facing help on the existing Menu surface, and add a deterministic evidence orchestrator that reuses the existing Player capture flags and Character Hub renderer rather than duplicating screen logic.

**Tech Stack:** Unity 6000.3.2f1, C# UI Toolkit, NUnit EditMode tests, Python 3 capture tooling, macOS Player evidence.

**Spec:** `docs/superpowers/specs/2026-09-17-lgo-whole-flow-p0-controls-design.md`

## Global Constraints

- Work only in `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22` on `codex/character-hub-v22`; do not change branch/worktree.
- Character Hub V59 remains authoritative and must not be redesigned or regenerated with a second renderer.
- Normal product controls are A/D or arrows, Shift, W/J/Up, Z, X, E, and I as specified by the approved spec.
- Review/debug keys `C/L/G/V/M/B/F` must be inert in normal product mode; any retained behavior requires explicit `--lgo-map01a-review-hotkeys` opt-in.
- No product auth, character lifecycle backend, skill-hotbar, five-class combat, economy, social, or artwork scope enters P0.
- Every behavior change follows RED → verify RED → minimal GREEN → verify GREEN before refactor.
- Final Player evidence must be fresh, isolated under `build/whole-flow-p0-v1/`, and report actual warnings/errors without rewriting history.

---### Task 1: Product/review hotkey ownership

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Produces: `CongDongLamArrivalHud.ShouldEnableReviewHotkeysForArgs(string[] args) -> bool`.
- Runtime opt-in flag: `--lgo-map01a-review-hotkeys`.

- [ ] **Step 1: Write the failing policy test**

Add `ProductModeDisablesReviewHotkeysUnlessExplicitlyOptedIn` asserting ordinary/capture args return false and the explicit review flag returns true.

- [ ] **Step 2: Run focused EditMode test and verify RED**

Run the Unity EditMode filter for `ProductModeDisablesReviewHotkeysUnlessExplicitlyOptedIn` and require failure because the policy method does not exist.

- [ ] **Step 3: Implement the minimal policy and gate only review keys**

Add the static policy method and wrap `C/L/G/V/M/B/F` polling in `Update()` with that policy. Do not alter A/D, Shift, W/J/Up, Z, X, E, I, H, K, or R semantics.

- [ ] **Step 4: Run focused test and existing world-input tests GREEN**

Run the new test plus `GameplayWorldInputIsBlockedByEveryForegroundWorkspace` and `SourceGameplayHudKeepsVitalsAndHidesLegacyModeButtonsWhenInventoryCloses`.

- [ ] **Step 5: Keep diff local; do not commit until Task 3 source/tool tests are green**### Task 2: Accurate Menu help and blocking matrix

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Menu.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Produces named label `Map01A Menu Controls Help` with the approved pointer-control copy.
- Reuses `CongDongLamArrivalHud.ShouldBlockWorldInput(...)` unchanged unless the failing test proves a missing foreground state.

- [ ] **Step 1: Write failing Menu copy assertions**

Extend `GameplayHudShowsProductShortcutGateWithoutDeadClicks` to require the named help label and text containing `A/D`, `Shift`, `W/J`, `Z`, `X`, `E`, and `I`, while explicitly rejecting `Di chuyển: Shift`.

- [ ] **Step 2: Extend the foreground blocking test**

Add an explicit assertion that `serverSelectOpen:true` blocks world input. Verify Register/Recovery continue hiding `_safe` through the Entry shell in their existing route tests rather than adding parallel blocker state.

- [ ] **Step 3: Run focused tests and verify RED**

Require the Menu copy test to fail because the current label is unnamed/inaccurate. If the server-select blocker assertion already passes, retain it as regression coverage rather than manufacturing a failure.

- [ ] **Step 4: Implement minimal Menu help change**

Name the existing label and replace its content with two readable lines: movement/run/jump on line one; attack/skill/interact/inventory on line two. Do not add a second help component.

- [ ] **Step 5: Run focused tests GREEN**### Task 3: Deterministic whole-flow evidence runner

**Files:**
- Create: `tools/capture_lgo_whole_flow_p0.py`
- Create: `tools/test_capture_lgo_whole_flow_p0.py`
- Modify only if required for validation-state evidence: `client/Unity/Assets/Game/World/Runtime/CongDongLamMap01AArtPreview.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- CLI: `python3 tools/capture_lgo_whole_flow_p0.py --player <.../Contents/MacOS/Unity> --out-dir build/whole-flow-p0-v1 --profile all`.
- Profiles: `pc=1600x900`, `tablet=1024x768`, `mobile=1600x720`.
- Output per profile: `entry/`, `server-select/`, `register/`, `password-recovery/`, `character-select/`, `menu/`, `quest/`, `character-hub/`, plus profile `manifest.json`.

- [ ] **Step 1: Write failing Python runner tests**

Test exact viewport matrix, command construction, expected flags/frame names/scopes, refusal to reuse an existing profile directory without explicit replacement, and combined-manifest validation rejecting missing/stale frames.

- [ ] **Step 2: Run Python test and verify RED**

Run `python3 -m unittest tools.test_capture_lgo_whole_flow_p0 -v`; require import/module failure because the runner is not implemented yet.

- [ ] **Step 3: Implement runner around existing capture contracts**

Launch the same built Player binary for each deterministic screen flag. Reuse `capture_lgo_character_hub.py` semantics for the five-tab workspace and the Map01A quest capture for world HUD/dialogue. Never use OS mouse/keyboard automation.

- [ ] **Step 4: Add validation-state capture only where evidence is missing**

If Entry/Register/Recovery default capture cannot produce the required local-validation frame, add one opt-in evidence flag/state transition inside the existing capture coroutines. Preserve historical default frame names/fields so old canonical tooling remains compatible.

- [ ] **Step 5: Run Python tests and focused Unity capture-contract tests GREEN**### Task 4: Source regression gates and P0 commit

**Files:**
- Modify: only files already listed by Tasks 1–3.

**Interfaces:**
- No new runtime API beyond the explicit review-hotkey policy and evidence-only capture CLI.

- [ ] **Step 1: Run targeted Python and Unity tests**

Require all P0-focused tests GREEN, including `UIFoundationTests` touch-pad release/capture-loss cases.

- [ ] **Step 2: Run full Unity EditMode suite**

Run the same graphics-capable `TwoDCharacterRuntimeStateTests`/full EditMode path used by V59 and record exact passed/failed/skipped counts.

- [ ] **Step 3: Run source validators**

Run `git diff --check`, shared-skin validator, no-source-image guard, no-3D guard, package hygiene where applicable, and current change-budget validator. Frozen protocol/GameData/ADR/design-token surfaces must remain unchanged.

- [ ] **Step 4: Review diff for scope**

Confirm there is no auth, character persistence, combat expansion, artwork replacement, or Character Hub renderer change.

- [ ] **Step 5: Commit the P0 source/tool change**

Commit with a focused message such as `fix(ui): close p0 product control integrity`. Do not push until runtime evidence also passes unless a later evidence-only commit is required.

### Task 5: Fresh Player build, three-viewport evidence, visual audit, closure

**Files/Evidence:**
- Generate: `build/whole-flow-p0-v1/player/LinhGioiOnline.app`
- Generate: `build/whole-flow-p0-v1/{pc,tablet,mobile}/...`
- Update only after evidence passes: `docs/execution/PROJECT-STATE.md`, `docs/execution/NEXT-ACTION.md`

- [ ] **Step 1: Build one fresh macOS Player from the committed P0 source**

Use Unity 6000.3.2f1 with `LinhGioi.Foundation.Editor.M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke`; log actual errors/warnings.

- [ ] **Step 2: Run the whole-flow capture runner for all three profiles**

Require isolated current-run manifests and every expected frame at exact viewport dimensions.

- [ ] **Step 3: Directly inspect representative frames**

Review Login default/validation, Register default/validation, Recovery default/validation, Character Select, Menu, normal/touch HUD, Dialogue, and all five Character Hub tabs. Record any concrete defect; fix only P0-caused/control-blocking issues, otherwise preserve as later backlog.

- [ ] **Step 4: Re-run final validators after any evidence-driven fix**

If source changed after visual review, repeat RED/GREEN, full EditMode, Player build, and the entire affected capture matrix; never mix stale and fresh evidence.

- [ ] **Step 5: Update project state/next action, commit, push, and verify remote**

Record P0 evidence and the next independently scoped subproject (Product Auth Foundation). Push only after confirming remote is still a fast-forward descendant of the expected baseline.

- [ ] **Step 6: Register immutable evidence and finish the batch**

Link the final test/build/capture manifests with MCP Session Manager, create a verified checkpoint, release new claims, and continue directly into the next approved subproject without asking for confirmation unless a genuine blocker/decision is encountered.