# LGO Whole-flow P0 Controls — Design

Date: 2026-09-17
Status: OWNER_APPROVED

## Purpose

Establish a fresh Player-visible baseline for the current Map01A product flow and close control-integrity defects before opening auth, character-lifecycle, or combat expansion work.

This is the first subproject from the owner-approved whole-product backlog. It preserves the verified Character Hub V59 baseline and does not reopen its completed layout/artwork slice without a concrete regression.

## Scope

P0 contains exactly two deliverables:

1. Fresh whole-flow Player evidence for current screens/states at PC, tablet, and mobile landscape viewports.
2. Product control integrity: accurate control help, removal of review/dev hotkeys from normal product input, keyboard/touch parity checks, and foreground-overlay input blocking regression coverage.

P0 does not add product auth, create-character backend wiring, skill-hotbar equipment, five-class combat, social, economy, or new artwork.

## Baseline authority

- Worktree: `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`.
- Current verified source baseline: commit `5c446bc4fa0ba1d5d22575ed0c45f3ddf1e7d2a1` or its exact descendant containing only this approved P0 design/plan work.
- Character Hub V59 remains the reference for `Nhân vật`, `Rương đồ`, `Kỹ năng`, `Tiềm năng`, and `Linh thú`.
- Existing canonical contracts remain authoritative for Entry, Server Select, Register, Password Recovery Request, and Character Select.
- Existing Map01A world route and Q01–Q09 behavior remain unchanged.
## Whole-flow evidence matrix

The fresh baseline must cover the following product surfaces where they exist today:

- Entry/Login default plus credential-validation state.
- Server Select.
- Register default and local-validation state.
- Password Recovery Request default and local-validation state.
- Character Select.
- World HUD at normal play state.
- World HUD with touch movement/actions visible on touch profiles.
- NPC Dialogue.
- Gameplay Menu.
- Character Hub five tabs, reusing V59 capture semantics rather than inventing a second renderer.

Each supported surface is captured at:

- PC: `1600x900`, pointer input profile.
- Tablet: `1024x768`, touch profile.
- Mobile landscape: `1600x720`, touch profile.

The capture runner must use internal Player flags/state transitions. It must not automate the owner's OS mouse or keyboard and must record `usesOsMouseOrKeyboard=false` in manifests where that field is part of the current evidence contract.

Evidence is written to a new isolated directory under `build/whole-flow-p0-v1/`. Historical capture folders are never overwritten or silently reused.
## Product control contract

Normal product input while no foreground workspace blocks the world:

- Move: `A/D` or Left/Right Arrow.
- Run hold: Left/Right Shift on pointer profile; Run action on touch profile.
- Jump hold: `W`, `J`, or Up Arrow on pointer profile; Jump action on touch profile.
- Basic attack: `Z` on pointer profile; Basic Attack action on touch profile.
- Current class skill: `X` on pointer profile; Skill action on touch profile.
- Interact: `E` on pointer profile; contextual interaction action on touch profile.
- Inventory: `I` on pointer profile; Inventory shortcut on both profiles.

Review/source-pose/equipment-debug keys `C`, `L`, `G`, `V`, `M`, `B`, and `F` must not mutate normal product state. If a dedicated review/capture mode still requires them, that behavior must be gated by an explicit review-only condition rather than being active in ordinary play.

The Menu help panel must describe the actual product controls above. It must not describe Shift as movement and must include the current Skill binding.

Foreground workspaces must continue blocking world movement/action input: Entry, Server Select, Register, Password Recovery, Character Select, Menu, Inventory/Character Hub, and NPC Dialogue.

Touch controls must clear held movement/jump state on pointer release, capture loss, panel detach, geometry change, or when a foreground workspace blocks the world.
## Architecture and implementation boundaries

The existing `CongDongLamArrivalHud` remains the product UI/input owner for Map01A. P0 does not create a second input framework or a parallel UI document.

Control fixes should remain concentrated in:

- `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs` for world-input gating and product/review hotkey ownership.
- `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Menu.cs` for player-facing control help.
- `client/Unity/Assets/Game/UI/Runtime/RuntimeTouchMovementPad.cs` only if a concrete touch reset defect is demonstrated by a failing test.
- `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs` and `UIFoundationTests.cs` for regression coverage.

Fresh capture tooling should extend or wrap the existing deterministic Map01A capture path. It must not duplicate Character Hub renderer logic, and it must reuse one Player build for all three viewport captures whenever source does not change between profiles.

The capture layer is evidence-only. It may request deterministic UI states through command-line flags, but those flags must not change normal product behavior.

## Error and truthfulness rules

- A failed or missing screen capture fails the evidence batch; it is never silently replaced by historical evidence.
- A control unavailable in the current product must remain visibly unavailable rather than receiving a dead click.
- P0 must not claim login/register/recovery success because those backends are outside this subproject.
- Existing warning counts from Player build are reported exactly; warnings are not rewritten as zero.
- If the current Player exposes a new visual defect unrelated to control integrity, record it as backlog evidence unless it blocks usability or is directly caused by P0.
## Testing and evidence gates

Source/TDD gates:

- A focused RED/GREEN test must prove normal product mode no longer executes review-only hotkeys.
- A focused test must lock the Menu help copy to the actual product bindings.
- Existing foreground-workspace blocking tests must remain green and be extended if Register/Recovery/Menu are not already explicit in the matrix.
- Existing touch-pad release/capture-loss reset tests must remain green.
- Full `TwoDCharacterRuntimeStateTests` and relevant UI foundation tests must pass after the change.
- `git diff --check`, shared-skin, no-source-image, no-3D, and package/change-budget gates remain required where applicable.

Runtime gates:

- Build one fresh macOS Player after the final C# change.
- Capture the P0 whole-flow matrix at all three viewports from that Player.
- Verify every expected frame exists, has the requested dimensions, and belongs to the current run.
- Visually review Login, Register, Recovery, Character Select, Menu, touch HUD, normal HUD, Dialogue, and the five Character Hub tabs.
- Confirm no control/help regression, no overlay input leak, and no new stack/crop/overflow defect caused by P0.

## Completion criteria

P0 is ready for owner review only when all of the following are true:

1. Current source has a fresh three-viewport whole-flow evidence pack.
2. Menu help matches actual bindings.
3. Review/debug hotkeys cannot mutate normal product gameplay state.
4. Keyboard and touch actions preserve the existing world behavior without opening new gameplay semantics.
5. Foreground overlays block world input and clear held touch state safely.
6. Character Hub V59 remains visually and functionally intact.
7. Worktree scope is clean except for the intentional P0 commit(s), and all reported test/build/capture results are current evidence.
## Explicit non-goals

P0 does not:

- add product login/register/recovery APIs;
- implement session/token/logout;
- connect Character Select to persisted accounts;
- create/edit/delete characters;
- add a multi-slot skill hotbar or skill equip persistence;
- convert the current Võ-centric world combat path into five-class combat;
- add production enemy AI, loot, economy, shop, trade, party, guild, chat, or liveops;
- redesign Character Hub V59 or replace approved/canonical artwork.

## Follow-up sequence after P0

After owner accepts P0, later subprojects are opened independently in this order:

1. Product Auth Foundation: login/session/token/logout.
2. Register + three-stage password recovery.
3. Character persistence flow: list/create/load, then edit/delete as separately validated work.
4. Equipped-skill hotbar/input contract.
5. Five-class world skill integration.
6. Production combat vertical slice on the existing server-authoritative foundation.
7. Settings/Support/Cinematic/Party system screens.
8. Inventory/progression actions, then shop/trade/guild/chat/social expansion.

Each follow-up receives its own design/spec, implementation plan, TDD cycle, Player evidence, and review checkpoint. Completing P0 must not implicitly authorize hidden implementation inside those later scopes.