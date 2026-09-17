# LGO UI Fidelity Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Complete `LGO-UIF-00..02`: bind every current product screen to owner design authority, establish one shared product UI token/USS/component foundation, and fix responsive/mobile authority before screen-level fidelity work.

**Architecture:** Keep `CongDongLamArrivalHud` behavior/state contracts intact while moving static visual ownership outward: `design-tokens.json → ThemeTokens runtime provider → shared USS/component classes → screen composition`. `RuntimeViewportMetrics → RuntimeUiLayoutProfile` owns safe-area/profile decisions, while one `PanelSettings` contract owns UI scale. World camera/actor presentation remains separate.

**Tech Stack:** Unity 6000.3.2f1, C# UI Toolkit, USS/UXML where static, NUnit EditMode, Python 3 validators, macOS Player evidence.

**Spec:** `docs/superpowers/specs/2026-09-17-lgo-ui-fidelity-foundation-design.md`

## Global Constraints

- Keep worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22` and branch `codex/character-hub-v22`.
- Owner canonical images outrank simplified runtime evidence for visual fidelity.
- Do not reopen closed account-binding/five-class migration semantics without regression evidence.
- Create Character remains DESIGN-GATED.
- No production code before a failing test/validator proves the intended change.
- Do not claim physical mobile PASS from macOS `1600x720` simulation.
- Do not change world camera/actor scale to compensate for UI scale in this phase.

---
### Task 1: UIF-00 authority matrix + validator

**Files:**
- Create: `docs/design/LGO-UI-AUTHORITY-MATRIX-v1.0.json`
- Create: `tools/validate_lgo_ui_authority_matrix.py`
- Create: `tools/test_validate_lgo_ui_authority_matrix.py`
- Evidence: `build/full-ui-ux-code-audit-v1/authority-matrix-validation.txt`

**Interfaces:**
- Consumes: owner design relative paths + SHA-256, runtime capture paths, source owner paths.
- Produces: validator exit 0 for a complete matrix; nonzero for missing owner/state/profile/code-owner or runtime-as-authority drift.

- [ ] **Step 1: Write validator tests first**

```python
def test_rejects_runtime_capture_as_authority_when_owner_exists(self):
    matrix = fixture(owner="redesign-v5-entry/01-entry-login-CANONICAL.png",
                     authority="build/whole-flow-p0-v1/pc/entry/entry-login.png")
    self.assertIn("runtime evidence cannot be design authority", validate(matrix))
```

- [ ] **Step 2: Run RED**

Run: `python3 tools/test_validate_lgo_ui_authority_matrix.py`
Expected: FAIL because validator/matrix contract does not exist.

- [ ] **Step 3: Implement minimal schema validator and populate every approved surface**

Validator requires `id`, `ownerAuthority`, `ownerStatus`, `runtimeEvidence`, `codeOwners`, `states`, `profiles`, `knownGaps`; when `--owner-root` is supplied it verifies file SHA-256.

- [ ] **Step 4: Run GREEN + owner-root hash validation**

Run: `python3 tools/test_validate_lgo_ui_authority_matrix.py && python3 tools/validate_lgo_ui_authority_matrix.py --owner-root /Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13`
Expected: PASS.

- [ ] **Step 5: Commit UIF-00**

```bash
git add docs/design/LGO-UI-AUTHORITY-MATRIX-v1.0.json tools/validate_lgo_ui_authority_matrix.py tools/test_validate_lgo_ui_authority_matrix.py
git commit -m "docs(ui): bind canonical screen authority matrix"
```
### Task 2: UIF-01 token authority at runtime

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Editor/ThemeTokenImporter.cs`
- Move/generated: `client/Unity/Assets/Resources/LGOThemeTokens.asset`
- Create: `client/Unity/Assets/Game/UI/Runtime/RuntimeUiTheme.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/UIFoundationTests.cs`

**Interfaces:**
- Produces: `RuntimeUiTheme.Current : ThemeTokens`, `RuntimeUiTheme.ResetForTests()`.
- Consumers: shared skin/factory and product HUD style bridge.

- [ ] **Step 1: Add failing EditMode tests**

```csharp
[Test] public void RuntimeThemeLoadsGeneratedDesignTokens()
{
    var theme = RuntimeUiTheme.Current;
    Assert.That(theme.gold, Is.EqualTo(ThemeTokens.FromJson(TokenFixture).gold));
    Assert.That(theme.minimumTouchTarget, Is.EqualTo(44));
}
```

Also add a source guard that fails while `CongDongLamArrivalHud.Skin.cs` defines semantic palette fields `UiGold`, `UiText`, `UiGlass`, `UiBlue`.

- [ ] **Step 2: Run RED focused UI foundation tests**

Run Unity EditMode `UIFoundationTests`; expected failure is missing `RuntimeUiTheme` / local palette guard.

- [ ] **Step 3: Implement runtime token provider and generated Resources asset**

`ThemeTokenImporter.GeneratedAssetPath` becomes `Assets/Resources/LGOThemeTokens.asset`. `RuntimeUiTheme.Current` loads `Resources.Load<ThemeTokens>("LGOThemeTokens")` and throws an explicit configuration error if absent; no duplicated color fallback.

- [ ] **Step 4: Replace product semantic palette declarations with token reads**

Static role-specific alpha/glass derivations may remain as helper calculations, but semantic gold/text/spirit/surface values come from `RuntimeUiTheme.Current`.

- [ ] **Step 5: Run GREEN and token hash/import validation**

Run focused EditMode tests plus existing token importer/project-generator tests. Commit only after generated asset hash matches `design-tokens.json`.

- [ ] **Step 6: Commit token authority**

```bash
git add client/Unity/Assets/Game/UI client/Unity/Assets/Resources/LGOThemeTokens.asset* client/Unity/Assets/Game/Tests/EditMode/UIFoundationTests.cs
git commit -m "refactor(ui): make design tokens runtime authority"
```
### Task 3: UIF-01 shared USS and component classes

**Files:**
- Create: `client/Unity/Assets/Resources/LGOUI/LgoRuntime.uss`
- Create: `client/Unity/Assets/Game/UI/Runtime/RuntimeUiStyleSheetProvider.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/UIFoundationTests.cs`

**Interfaces:**
- Produces: `RuntimeUiStyleSheetProvider.Attach(VisualElement root)` and shared USS classes `lgo-panel`, `lgo-modal`, `lgo-action`, `lgo-input`, `lgo-tab`, `lgo-icon-frame`, `lgo-badge`, `lgo-status`, `lgo-utility-action`, `lgo-ornament`.
- Consumers: existing product screen builders/skin helpers; screen controllers do not load their own stylesheet.

- [ ] **Step 1: Add failing tests for stylesheet attachment and shared classes**

```csharp
[Test] public void ProductRootAttachesSharedRuntimeStyleSheet()
{
    var root = new VisualElement();
    Assert.That(RuntimeUiStyleSheetProvider.Attach(root), Is.True);
    Assert.That(root.styleSheets.count, Is.GreaterThan(0));
}
```

Add tests asserting representative button/input/tab helpers add the shared role class instead of owning the entire static recipe inline.

- [ ] **Step 2: Run RED**, expecting missing provider/classes.

- [ ] **Step 3: Implement one stylesheet/provider and migrate repeated static primitive styling**

Keep dynamic selected/disabled/accent/texture state in C#. Move stable radius, border widths, base padding, typography defaults and repeated colors to USS.

- [ ] **Step 4: Run GREEN + current Character Hub/Auth focused tests**

- [ ] **Step 5: Commit**

```bash
git add client/Unity/Assets/Resources/LGOUI client/Unity/Assets/Game/UI/Runtime client/Unity/Assets/Game/Tests/EditMode/UIFoundationTests.cs
git commit -m "refactor(ui): add shared product USS primitives"
```
### Task 4: UIF-01 style-debt budget and skin responsibility split

**Files:**
- Create: `tools/report_lgo_product_ui_style_debt.py`
- Create: `tools/validate_lgo_product_ui_style_debt.py`
- Create: `tools/test_validate_lgo_product_ui_style_debt.py`
- Split: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs` into bounded partials by shared/core, auth/select, Character Hub/inventory, and world HUD roles.

**Interfaces:**
- Produces: deterministic JSON/text metrics for direct `.style` assignments, numeric inline styles, local semantic palette declarations, and per-file LOC/method count.
- Acceptance budget: first migration must reduce direct numeric product style debt from the audited baseline and validators prevent regression above the new measured ceiling.

- [ ] **Step 1: Write failing validator tests** for semantic palette duplication and an intentionally over-budget fixture.
- [ ] **Step 2: Run RED**; validator is absent.
- [ ] **Step 3: Implement report/validator using syntax-safe text scanning** over `Assets/Game/UI/Runtime`.
- [ ] **Step 4: Split the 2,076-line skin partial by responsibility without changing method signatures**; move methods, do not duplicate them.
- [ ] **Step 5: Run source validator + focused Unity tests** and record the new debt ceiling from exact source.
- [ ] **Step 6: Commit**

```bash
git add tools client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin*.cs
git commit -m "refactor(ui): bound product style ownership debt"
```
### Task 5: UIF-02 single PanelSettings scale authority

**Files:**
- Modify: `client/Unity/Assets/Resources/LGORuntimePanelSettings.asset`
- Modify: `client/Unity/Assets/Game/UI/Runtime/RuntimePanelSettingsProvider.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/UIFoundationTests.cs`

**Interfaces:**
- Produces one product policy: `ScaleWithScreenSize`, reference `1672x941`, `MatchWidthOrHeight`, landscape height authority (`match=1`).
- `CongDongLamArrivalHud.Attach` clones the provider result but does not override scale fields.

- [ ] **Step 1: Add failing policy tests**

```csharp
[Test] public void RuntimePanelPolicyUsesCanonicalLandscapeHeightAuthority()
{
    var settings = RuntimePanelSettingsProvider.LoadOrCreate();
    Assert.That(settings.referenceResolution, Is.EqualTo(new Vector2Int(1672, 941)));
    Assert.That(settings.match, Is.EqualTo(1f).Within(.001f));
}
```

Add an Attach test proving the live `UIDocument.panelSettings` has the same scale fields as the provider.

- [ ] **Step 2: Run RED**; current resource/provider is `1200x800/match=0`.
- [ ] **Step 3: Update resource + provider and remove HUD post-load scale overrides**.
- [ ] **Step 4: Run GREEN plus existing panel/UI tests**.
- [ ] **Step 5: Commit** with `fix(ui): unify landscape panel scale authority`.
### Task 6: UIF-02 viewport/safe-area/profile authority

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/RuntimeViewportMetrics.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/UIFoundationTests.cs`

**Interfaces:**
- Add pure `RuntimeViewportMetrics.FromMeasurements(...)` for deterministic safe-area/profile tests.
- `CongDongLamArrivalHud.Layout()` derives exactly one `RuntimeUiLayoutProfile` from `_metrics` and consumes semantic profile properties for right HUD width, combat-bar bottom, talk font and touch presentation metrics.

- [ ] **Step 1: Add RED matrix tests** for synthetic 16:9, 19.5:9, 20:9 and 4:3 safe-area inputs.

```csharp
var phone = RuntimeViewportMetrics.FromMeasurements(2532, 1170,
    new Rect(132, 63, 2268, 1070), 2038, 941, "mobile");
Assert.That(phone.SafePanelRect.x, Is.GreaterThan(0));
Assert.That(RuntimeUiLayoutProfile.FromViewport(phone).InputClass, Is.EqualTo("touch"));
```

- [ ] **Step 2: Run RED**; pure measurement constructor/profile consumption is absent.
- [ ] **Step 3: Implement pure metrics path and move duplicated HUD breakpoint values into semantic profile properties**.
- [ ] **Step 4: Run GREEN and ensure safe-area math never produces negative/overflow rectangles**.
- [ ] **Step 5: Commit** with `refactor(ui): centralize viewport and safe-area profile`.
### Task 7: UIF-02 measured touch-shell/evidence metrics

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Create/modify: `client/Unity/Assets/Game/UI/Runtime/RuntimeUiMetricsSnapshot.cs`
- Modify: `client/Unity/Assets/Game/World/Runtime/CongDongLamMap01AArtPreview.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`
- Test: `tools/test_capture_lgo_character_hub.py`

**Interfaces:**
- Produces a metrics snapshot containing screen pixels, panel units, safe-panel rect, profile/input class, PanelSettings description, Character Hub shell rect/occupancy, and token touch-target size in panel units + resulting screen pixels.
- Capture manifests embed this snapshot; `mobile` manifests explicitly carry `evidenceAuthority="macos-aspect-simulation"` until real device evidence exists.

- [ ] **Step 1: Add failing shell/manifest tests** proving canonical shell remains `1098x724` in panel units while height-match scaling reduces the mobile screen-height occupation relative to the old ~86.3% result.
- [ ] **Step 2: Run RED**; metrics/evidence fields are absent.
- [ ] **Step 3: Implement snapshot + manifest serialization without changing gameplay state**.
- [ ] **Step 4: Build one fresh Player and capture PC/tablet/mobile simulation**.
- [ ] **Step 5: Measure actual outer-shell bounds from screenshots and compare to manifest geometry; reject if inconsistent**.
- [ ] **Step 6: Commit** with `feat(ui): record responsive scale evidence metrics`.

### Task 8: Foundation closure and next-task handoff

**Files:**
- Update: `docs/execution/PROJECT-STATE.md`
- Update: `docs/execution/NEXT-ACTION.md`
- Evidence: `build/ui-fidelity-foundation-v1/`

- [ ] **Step 1: Run style/authority validators and focused EditMode tests**.
- [ ] **Step 2: Run full relevant UI/Character runtime EditMode suite in graphics mode**.
- [ ] **Step 3: Run shared-skin/no-3D/no-source-images/package/code-governance/git-diff gates**.
- [ ] **Step 4: Review fresh owner-vs-runtime evidence only for regressions introduced by foundation work; do not claim UIF-03 screen fidelity yet**.
- [ ] **Step 5: Record blockers truthfully**: Android/iOS final physical sizing remains unclosed if playback modules/devices are unavailable.
- [ ] **Step 6: Commit closure docs, push `feature/2d`, verify local HEAD equals `origin/feature/2d`, worktree clean**.
- [ ] **Step 7: Update MCP Manager checkpoint/evidence and set next action to `LGO-UIF-03 Entry/Auth family canonical fidelity` only after UIF-00..02 gates are green.