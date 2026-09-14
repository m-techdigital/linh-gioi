# Map01A Character Screen Realignment Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Do not dispatch subagents for this batch.

**Goal:** Bring the runtime `Nhân vật` screen into the approved canonical composition on PC, mobile landscape, and tablet, with readable dedicated equipment icons and no changes to class art or gameplay state.

**Architecture:** Use one 1672×941 reference canvas and one bounded 1098×724 shell that scales uniformly across viewport profiles. Keep the existing shared five-tab shell and detail-right component; update shared geometry once, then bind a dedicated ten-cell UI icon atlas only in the active `Nhân vật` screen. Other tabs remain behaviorally unchanged and may inherit shared shell geometry only.

**Tech Stack:** Unity 6 UI Toolkit, C#, PNG atlas + JSON manifest, NUnit EditMode tests, Python validators.

**Spec:** `docs/design/LGO-MAP01A-CHARACTER-HUB-SCREEN-CONTRACT-v1.0.md`

## Global Constraints

- Active screen is `Nhân vật`; do not implement or polish Rương đồ, Kỹ năng, Tiềm năng, or Linh thú in this batch.
- Do not modify class/pose/wardrobe/source/camera/scale code or assets.
- Do not modify `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, or `client/Unity/Assets/Game/UI/design-tokens.json`.
- Keep one two-column composition on PC, mobile landscape, and tablet; no stacked/reflow layout.
- Build and capture once after the coherent screen batch, then group any visual corrections into one follow-up pass.

---

### Task 1: Lock the screen geometry in tests

**Files:**
- Modify: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Consumes: `CongDongLamArrivalHud.CalculateInventoryModalRect(Rect, bool)` and `CalculateInventoryDesktopColumnWidths()`.
- Produces: regression assertions for canonical shell, 600/448 columns, one row on all three viewport profiles, and a dedicated icon for each canonical equipment slot.

- [ ] Add assertions that the 1672×941 design canvas resolves to shell `(286,127,1098,724)` within one pixel and columns `(600,448)`.
- [ ] Add assertions that mobile/tablet use the same bounded shell policy and that `Map01A Inventory Body` keeps `FlexDirection.Row`.
- [ ] Add assertions that all ten `Map01A Character Hero Quick Icon N` elements receive non-null sprites from the dedicated UI atlas.
- [ ] Run the focused tests and retain the expected RED result before implementation.

### Task 2: Add the reviewed ten-item UI atlas

**Files:**
- Create: `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ACharacterEquipmentIcons/map01a-character-equipment-icons.png`
- Create: `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ACharacterEquipmentIcons/manifest.json`
- Create: Unity `.meta` files for the resource folder, PNG, and manifest.
- Modify: `client/Unity/Assets/Game/World/Runtime/CongDongLamMap01AArtPreview.cs`

**Interfaces:**
- Produces: `public Sprite GetMap01ACharacterEquipmentIconSprite(string slot)`.
- Manifest slots: `main_weapon`, `head_hair`, `inner_top`, `outer_top`, `lower_body`, `waist_belt`, `arm_guard`, `footwear`, `shoulder_chest_guard`, `class_accessory`.

- [ ] Copy the self-reviewed 640×256 alpha atlas from `build/map01a-character-icon-design-v1/map01a-character-equipment-icons-grabcut-v2.png` and record its source/imagegen/mask provenance in `manifest.json`.
- [ ] Configure the PNG as an un-mipped transparent UI texture, max size 1024, bilinear filtering, no NPOT scaling.
- [ ] Load and cache the ten 128×128 cells with bounds and duplicate-id guards; unknown ids return null.
- [ ] Run the focused atlas/character test and confirm it passes.

### Task 3: Apply the canonical shell and two-column scale policy

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Inventory.cs`

**Interfaces:**
- `UIDocument.panelSettings`: reference 1672×941, `ScaleWithScreenSize`, `MatchWidthOrHeight`, match 0.5.
- `CalculateInventoryModalRect`: centered canonical shell with optical vertical offset and 24-unit minimum safe margin.
- Body remains a row; column widths are 600 and 448 with 12 gap at canonical size.

- [ ] Replace the touch full-width modal branch with the single canonical bounded-shell calculation.
- [ ] Remove the column-stacking branch; preserve one row for every profile.
- [ ] Set title/tab/shell metrics from the canonical design through shared Skin helpers, including a 52-unit tab rail and no runtime subtitle.
- [ ] Keep Rương đồ/Kỹ năng/Tiềm năng/Linh thú content unchanged; only shared shell geometry may move with the active screen.
- [ ] Run the focused layout tests and confirm they pass.

### Task 4: Finish the `Nhân vật` hierarchy and dedicated thumbnails

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Inventory.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs`

**Interfaces:**
- Character hero rails and detail-right call `GetMap01ACharacterEquipmentIconSprite(slot)`.
- Rương đồ tiles continue using `GetVoEquipmentThumbnailSprite(slot)` until their own screen batch.

- [ ] Remove the inner boxed-preview treatment so the actor and ten slots read as one 600×599 workspace.
- [ ] Size the actor stage, five-slot rails, identity, Lv/LC, and HP/MP bands as one vertical composition matching the canonical screen.
- [ ] Bind the ten dedicated icons to the two rails and selected item detail only while `Nhân vật` is active.
- [ ] Preserve current slot selection, equip/unequip behavior, actor state, and all item ids.
- [ ] Run all `TwoDCharacterRuntimeStateTests`.

### Task 5: One integrated Player review and checkpoint

**Files:**
- Modify: `docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md`
- Modify: `docs/execution/PROJECT-STATE.md`
- Modify: `docs/execution/NEXT-ACTION.md`
- Modify: `build/codex-autopilot/status.json`

**Interfaces:**
- Produces one Player and three evidence directories for the same `Nhân vật` state.

- [ ] Run Python UI tests/validators, no-3D, no-source-image, `git diff --check`, change budget, and frozen diff audit.
- [ ] Build the macOS Player once from the completed batch.
- [ ] Capture `Nhân vật` at PC 1600×900, mobile landscape 1600×720, and tablet 1024×768 without OS mouse/keyboard automation.
- [ ] Review the three images against `01-nhan-vat-nam-tab-compact-APPROVED.png` in shell → columns → hierarchy → icon clarity order.
- [ ] If a grouped correction is required, apply it once and repeat only the affected tests/build/captures.
- [ ] Update evidence/state, commit the coherent screen checkpoint, and push to `origin/feature/2d` only when the frozen audit is clean.
