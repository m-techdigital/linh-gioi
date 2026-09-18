# LGO UIF-06 Gameplay HUD Fidelity Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rebuild Map01A gameplay HUD information architecture around owner desktop/touch references while keeping the center playfield readable and preserving all existing gameplay/input semantics.

**Architecture:** Keep `CongDongLamArrivalHud` as state/event binder, but move device-specific world-HUD geometry into one pure `RuntimeGameplayHudLayout` derived from `RuntimeUiLayoutProfile` + safe-panel rect. Reuse existing world skin primitives, add only semantic HUD-zone roles, consolidate permanent secondary actions behind the existing menu, and preserve dialogue/input blocking as a foreground workspace.

**Tech Stack:** Unity 6000.3.2f1, C#, UI Toolkit, NUnit EditMode tests, macOS Player capture harness, Python static/governance validators.

**Spec:** `build/full-ui-ux-code-audit-v1/backlog.json` task `LGO-UIF-06`, `build/full-ui-ux-code-audit-v1/audit.md`, and `docs/design/LGO-UI-AUTHORITY-MATRIX-v1.0.json`.

## Global Constraints

- Owner references remain `02-hud-gameplay-dong-lam-desktop-demo.png` and `01-hud-gameplay-dong-lam-touch-demo.png`.
- Preserve gameplay/input semantics; no new combat mechanics, social systems, auth, persistence, character schema, camera, actor, NPC, or world-scale changes.
- UIF-05 is closed at runtime `3fd986ef` / closure `e84fcfdd`; do not replay or redesign Character Hub.
- PC and touch share information semantics, but geometry/physical sizing may differ by profile.
- Core movement/combat/context controls stay at edges/thumb zones; the center combat playfield stays unobstructed.
- Secondary navigation must not permanently occupy the combat playfield.
- Foreground overlays continue blocking world movement/action input.
- Mobile 1600x720 remains macOS touch/aspect simulation, not physical-device proof.
- Incidental Unity `.meta`, `GraphicsSettings.asset`, and `QualitySettings.asset` rewrites are excluded unless exact-runtime evidence proves necessity.
- Every source-changing task is RED -> verified RED -> minimal GREEN -> focused regression before broader gates.

---

## File Structure

- Create: `client/Unity/Assets/Game/UI/Runtime/RuntimeGameplayHudLayout.cs` — pure semantic rectangles/insets for world HUD zones.
- Create: matching `.meta` generated once and kept stable.
- Modify: `client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs` — expose profile-owned gameplay HUD dimensions only.
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs` — bind existing controls to semantic zones; no gameplay-rule changes.
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.World.cs` — shared world-HUD zone/action roles only.
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Menu.cs` — make menu the canonical expansion point for secondary navigation.
- Modify: `client/Unity/Assets/Game/Tests/EditMode/UIFoundationTests.cs` — pure profile/safe-area geometry contracts.
- Modify: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs` — hierarchy, visibility, input blocking, and no-dead-click regressions.
- Use: `tools/capture_lgo_whole_flow_p0.py` and existing quest captures; change capture tooling only if a proven evidence gap requires it.

### Task 1: Semantic gameplay-HUD layout contract

**Interfaces:**
- Consumes: `RuntimeUiLayoutProfile`, safe-panel `Rect`, and touch/pointer input class.
- Produces: `RuntimeGameplayHudLayout.Calculate(Rect safeRect, RuntimeUiLayoutProfile profile)`.
- Produces rectangles: `PlayerStatus`, `RightInfo`, `Combat`, `Context`, `SecondaryNav`, `TouchPad`, and `Dialogue`.

- [ ] **Step 1: Add RED safe-area/non-overlap tests**

Add focused cases to `UIFoundationTests.cs` for desktop 1673x941, tablet 1255x941, mobile 2091x941, plus a wide-phone cutout safe rect. Assert every zone stays inside the safe rect, combat/context do not intersect right-info, touch pad does not intersect combat/context, and center reserve remains open.

- [ ] **Step 2: Run RED**

Run Unity EditMode filter:
`LinhGioi.Tests.EditMode.UIFoundationTests.GameplayHudLayoutKeepsEdgeZonesInsideSafeArea,LinhGioi.Tests.EditMode.UIFoundationTests.GameplayHudLayoutKeepsTouchThumbZonesSeparated`

Expected: FAIL because `RuntimeGameplayHudLayout` does not exist.- [ ] **Step 3: Implement minimal pure layout type**

Use an immutable value type with explicit rectangles and one `Calculate` method. Geometry must derive from safe-panel size and profile-owned widths/insets; do not read `Screen`, `Camera`, scene state, or input directly.

```csharp
internal readonly struct RuntimeGameplayHudLayout
{
    internal readonly Rect PlayerStatus, RightInfo, Combat, Context, SecondaryNav, TouchPad, Dialogue;

    internal static RuntimeGameplayHudLayout Calculate(Rect safeRect, RuntimeUiLayoutProfile profile)
    {
        // Edge-docked geometry only; center reserve is implicit negative space.
    }
}
```

Move only stable gameplay-HUD dimensions into `RuntimeUiLayoutProfile` properties such as edge inset, status width, right-info width, control dock sizes, and touch-pad size.

- [ ] **Step 4: Run GREEN**

Run the two new UIFoundation tests plus existing `LayoutProfileOwnsHudDensityByDeviceProfile` and `ViewportMetricsConvertWidePhoneSafeAreaWithoutOverflow`.

Expected: all PASS.

- [ ] **Step 5: Commit only if this slice is independently green**

Commit message: `refactor(ui): centralize gameplay hud layout authority`.

### Task 2: Bind status + quest/minimap to semantic edge zones

**Files:**
- Modify: `CongDongLamArrivalHud.cs`
- Modify: `CongDongLamArrivalHud.Skin.World.cs`
- Test: `TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Consumes: `RuntimeGameplayHudLayout.PlayerStatus` and `.RightInfo`.
- Preserves existing element names/data: Vitals, Location Title, Minimap, Quest Tabs, Quest Body.

- [ ] **Step 1: Add RED hierarchy/zone-role test**

Add `GameplayHudDocksStatusAndQuestMapToSharedEdgeZones`. Assert player cluster owns a player-status zone class, right cluster owns a right-info zone class, all current data children stay under their existing cluster, and no new duplicate Vitals/Quest/Minimap trees exist.- [ ] **Step 2: Verify RED**

Run the single new test. Expected: FAIL on missing semantic zone classes/layout binding.

- [ ] **Step 3: Bind existing clusters to layout rectangles**

Apply the calculated rectangles in `Layout()`; remove duplicated hard-coded cluster width/placement where the layout contract now owns it. Add shared class helpers such as `lgo-gameplay-player-zone` and `lgo-gameplay-right-zone`; do not create another palette or panel system.

- [ ] **Step 4: GREEN + regressions**

Run:
- new status/quest-map zone test
- `SourceGameplayHudKeepsVitalsAndHidesLegacyModeButtonsWhenInventoryCloses`
- `GameplayHudUsesCompactReferenceHierarchyAndBoundedDialogue`

Expected: PASS with identical gameplay data and hidden review controls.

### Task 3: Combat, context action, and touch thumb-zone fidelity

**Files:**
- Modify: `RuntimeGameplayHudLayout.cs`
- Modify: `CongDongLamArrivalHud.cs`
- Modify: `CongDongLamArrivalHud.Skin.World.cs`
- Test: `UIFoundationTests.cs`, `TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Consumes: `Combat`, `Context`, `TouchPad` rectangles.
- Preserves callbacks for Run, Jump, Basic Attack, Skill, route action, NPC talk, and `RuntimeTouchMovementPad`.

- [ ] **Step 1: RED tests for thumb-zone reach and no overlap**

Assert on touch profiles that joystick is bottom-left, combat is bottom-right, context action is adjacent to/above the combat dock without intersection, all targets remain inside safe area, and pointer profile hides the joystick while retaining the same combat semantics.

- [ ] **Step 2: Verify RED**

Run the new focused tests. Expected: FAIL against current fixed `Place(... bottom 318/394 ...)` and mixed placement rules.

- [ ] **Step 3: Implement geometry-only relocation**

Use the semantic rectangles in `Layout()`. Keep button callbacks/tooltips and touch hold/capture behavior unchanged. Reuse `ApplyLgoHudCombatAction`, `ApplyLgoHudContextAction`, and `RuntimeTouchMovementPad.ApplyCircularPresentation`; change styling only when needed to satisfy the owner/touch hierarchy and target size.- [ ] **Step 4: GREEN + input regressions**

Run new thumb-zone tests plus:
- `UIFoundationTests.TouchPadPointerReleaseAndCaptureLossClearMovement`
- `UIFoundationTests.TouchPadDisplacementIsScaleIndependentAndBounded`
- `GameplayWorldInputIsBlockedByEveryForegroundWorkspace`

Expected: PASS.

### Task 4: Consolidate secondary navigation behind the existing menu

**Files:**
- Modify: `CongDongLamArrivalHud.cs`
- Modify: `CongDongLamArrivalHud.Menu.cs`
- Modify: `CongDongLamArrivalHud.Skin.World.cs`
- Test: `TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Menu remains canonical access to Character, Rương đồ, Kỹ năng, Tiềm năng, Linh thú.
- Always-visible HUD keeps only the smallest justified navigation affordance(s); removed permanent shortcuts remain reachable through Menu with no dead clicks.

- [ ] **Step 1: RED product-navigation test**

Update/add a focused test that requires the permanent secondary dock to stay compact and bounded, requires Menu to expose all five existing destinations, and verifies Character/Bag/Skills are still reachable after consolidation.

- [ ] **Step 2: Verify RED**

Expected: current four-button `Map01A Product Shortcut Actions` violates the compact-secondary-dock contract.

- [ ] **Step 3: Minimal consolidation**

Do not invent destinations. Prefer one compact Menu affordance plus only a directly justified quick action if owner/runtime evidence requires it. Route all other secondary navigation through the existing menu panel; keep existing destination methods.

- [ ] **Step 4: GREEN**

Run:
- navigation consolidation test
- `GameplayHudShowsProductShortcutGateWithoutDeadClicks`
- `MenuControlsHelpMatchesPointerAndTouchProfiles`

If the legacy test encodes the superseded four-button permanent dock, replace only that geometry expectation while preserving its no-dead-click assertions.### Task 5: Dialogue foreground workspace and HUD occlusion

**Files:**
- Modify: `RuntimeGameplayHudLayout.cs`
- Modify: `CongDongLamArrivalHud.cs`
- Test: `TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Dialogue uses `RuntimeGameplayHudLayout.Dialogue`.
- `ShouldBlockWorldInput` remains the behavioral authority.
- While dialogue is open, combat/context/touch/secondary controls stay hidden; quest context remains readable where the approved layout allows.

- [ ] **Step 1: RED dialogue-zone test**

Require dialogue bounds to be safe-area-contained on PC/tablet/mobile, require it not to cover the right-info zone on desktop/tablet, and require underlying action docks hidden while dialogue is active.

- [ ] **Step 2: Verify RED**

Run the new test plus existing bounded-dialogue test; record exact failing geometry/visibility assertion.

- [ ] **Step 3: Bind dialogue to shared layout authority**

Replace duplicated hard-coded dialogue placement with the calculated dialogue rectangle. Do not alter dialogue text, quest progression, choice semantics, or route actions.

- [ ] **Step 4: GREEN + behavior regressions**

Run:
- new dialogue-zone test
- `DialoguePanelShowsQuestContextAndProgressInsideConversation`
- `DialogueChoiceButtonsCancelOrReadInformationWithoutAcceptingQuest`
- `DialogueHidesUnderlyingActionsAndRestoresThemAfterContinue`
- `GameplayWorldInputIsBlockedByEveryForegroundWorkspace`

Expected: PASS.

### Task 6: Exact-source runtime evidence and closure

**Evidence:** PC 1600x900, tablet-simulation 1024x768, mobile-landscape-simulation 1600x720.- [ ] **Step 1: Recheck machine pressure and foreign heavy jobs**

Do not overlap full Unity/build/capture with active heavy LGO-WEB/AXIRO jobs. If heavy work is active, checkpoint and wait; do not start a duplicate or competing runtime.

- [ ] **Step 2: Run focused source/static gates**

Run current Character Hub/whole-flow capture tests, shared-skin tests/validator, style-debt tests/validator, no-3D, no-source-images, package hygiene, code governance, and `git diff --check`.

- [ ] **Step 3: Run full Unity EditMode on exact source**

Expected baseline count is at least the current 372 tests plus new UIF-06 tests; record the exact count rather than hard-coding success.

- [ ] **Step 4: Build one fresh macOS Player from exact source**

Use the existing `M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke` path. Record build result/errors/warnings. Do not reuse a pre-change binary.

- [ ] **Step 5: Capture gameplay AFTER evidence**

Capture the existing quest/world flow on PC/tablet/mobile-landscape simulation from the same binary. Preserve `usesOsMouseOrKeyboard=false` claims where applicable and label touch/mobile evidence as macOS simulation.

- [ ] **Step 6: Produce measurable audit**

Create owner/reference vs BEFORE vs AFTER boards and a machine-readable occlusion report containing safe rect, player/right/combat/context/secondary/touch/dialogue rectangles, intersection flags, and center-playfield reserve ratio per profile.

- [ ] **Step 7: Visual review**

Reject any AFTER frame with center-playfield obstruction, thumb-zone overlap, clipped safe-area controls, dead navigation, or owner-hierarchy regression. Fix only proven defects with a new RED test before rebuilding.

- [ ] **Step 8: Restore incidental Unity rewrites**

Compare post-Unity status against pre-run source. Restore only review/build-generated `.meta`, Graphics, or Quality drift proven unrelated to UIF-06.

- [ ] **Step 9: Fresh exact-source final gates**

Re-run all source/static gates after restoring incidental drift. Verify `git diff --check`, exact changed-file list, and that auth/persistence/world-scale files did not drift.

- [ ] **Step 10: Commit/push runtime source**

Commit only UIF-06 runtime/test changes after exact-source GREEN. Push normally to `origin/feature/2d`, then verify local HEAD == remote and ahead/behind 0/0.

- [ ] **Step 11: Publish closure authority**

Update project-state/next-action docs with exact commit, test count, Player path, evidence paths, and simulation-vs-device limitation. Commit/push docs separately if repo convention requires exact runtime-source binding.

- [ ] **Step 12: Submit review, not self-complete**

Attach evidence to MCP task `T-bced33fb2edb`, call the session's review/finish flow with `READY_REVIEW`, and leave `task.complete` to operator/reviewer authority.

## Self-review

- Spec coverage: all eight UIF-06 scope items map to Tasks 2–5; safe-area/occlusion/input evidence maps to Tasks 1, 3, 5, 6.
- Scope isolation: no camera/actor/world-scale work (UIF-07), texture optimization (UIF-08), or mega-class decomposition beyond directly touched HUD layout ownership (UIF-09).
- Behavior preservation: callbacks, quest progression, auth, persistence, account binding, and combat rules stay unchanged.
- Evidence truth: PC is macOS Player; tablet/mobile are macOS profile/aspect simulation until later authoritative device gates.
- No placeholders remain; exact new interfaces and focused regression tests are named above.
