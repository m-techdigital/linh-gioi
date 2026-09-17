# LGO UI Fidelity Foundation — Design

Date: 2026-09-17
Status: OWNER_APPROVED / IMPLEMENTATION_AUTHORIZED

## Purpose

Establish one maintainable UI authority before further screen-by-screen polish. This phase implements `LGO-UIF-00`, `LGO-UIF-01`, and `LGO-UIF-02` from the approved whole-game audit/backlog.

The phase does not redesign gameplay or auth semantics. It makes owner design authority explicit, collapses duplicated UI token/style ownership, introduces reusable USS-backed primitives, and makes viewport/mobile scaling measurable rather than treating a `1600x720` macOS capture as physical-device proof.

## Baseline and authority

- Worktree: `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`.
- Source baseline at approval: `9f4591ad79f8daa2f7878c3e1ce79b42a4b5bffb`.
- Audit evidence: `build/full-ui-ux-code-audit-v1/`.
- Machine-readable backlog: `build/full-ui-ux-code-audit-v1/backlog.json`.
- Owner design root: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/`.
- Map composition root: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/`.

Owner canonical/approved images outrank simplified runtime captures for visual fidelity. Existing functional closures remain valid unless a regression is proven.
## UIF-00 — Design authority baseline

Create a committed authority matrix covering Entry/Login, Server Select, Register, Recovery Request/Verify/New Password, Character Select, Character Hub five tabs, Gameplay HUD/Menu/Dialogue, and Map01A world composition.

Each surface entry records:
- owner authority family/path and SHA-256 when an owner source exists;
- current runtime evidence path;
- current C# owner(s);
- supported states and profile classes;
- known visual/architecture gaps;
- whether the source is canonical, approved candidate, generated proposal, or runtime-only fallback.

A validator must fail if a surface with owner canonical art points at runtime evidence as its design authority, lacks a code owner, or omits profile/state metadata. Validation accepts an explicit owner-root argument so the same matrix can be statically checked when the external design pack is unavailable and hash-verified on the owner's machine.

No production UI behavior changes in UIF-00.
## UIF-01 — Shared UI foundation

`Assets/Game/UI/design-tokens.json` becomes the semantic product-UI token authority. The generated `ThemeTokens` runtime asset is loadable through `Resources`; product UI code reads colors, spacing and touch-target tokens through one runtime provider.

`RuntimeArtCatalog` may continue owning world/art colors, but product UI must not keep a second local palette. `CongDongLamArrivalHud.Skin.cs` local `UiGlass/UiGold/UiText/...` constants are retired or derived through the product token provider instead of redefining semantic colors.

Add one product USS stylesheet and one root-level stylesheet attachment. Reusable component classes own the static appearance for panel/shell, action button, input, tab, icon frame, badge/status, modal, utility action and ornament roles. C# remains responsible for dynamic state, data binding, texture assignment, visibility and geometry that genuinely depends on measured viewport/state.

Migration is incremental: UIF-01 moves repeated primitives first, not every screen-specific pixel value. A static validator records direct numeric `.style` assignments and local palette declarations, and prevents the count from increasing after the first reduction. Screen-specific visual fidelity work continues in UIF-03+.
## UIF-02 — Responsive/mobile authority

There must be one product `PanelSettings` scale contract. The current `1200x800/match=0` resource/provider versus `1672x941/match=.5` HUD override is removed. The canonical owner canvas `1672x941` is the reference for the product panel contract; the provider and resource asset must agree, and the HUD must not override them after loading.

`RuntimeViewportMetrics` becomes the measured viewport/safe-area authority and `RuntimeUiLayoutProfile` becomes the single product UI profile derived from it. Screen code may use semantic profile properties but must not invent duplicate mobile/tablet breakpoint tables.

Character Hub mobile sizing is not solved by globally shrinking all UI. The shell keeps the approved two-column composition, but its maximum vertical occupancy is explicitly bounded for compact/touch landscapes and measured against the safe panel. Touch target size and shell occupancy are reported separately.

Synthetic regression cases cover 16:9, 19.5:9, 20:9 and 4:3 tablet safe-area geometry. These tests prove layout math only. Final Android/iOS mobile PASS remains blocked until Android/iOS Unity support and authoritative simulator/device evidence are available.
## World-scale boundary

UIF-02 may measure camera/actor/UI relationships but does not redesign Map01A camera or actor art. World camera/actor/NPC normalization remains `LGO-UIF-07`.

The current fixed Map01A orthographic size `3.8`, stable actor world height around `1.68–1.70`, and existing `RegisteredActorScreenHeightRatio` are recorded as baseline facts. Any later camera change must use measured actor-screen ratios and world-composition evidence rather than compensating for UI scale by changing character size.

## Testing and evidence

Every source-changing step uses RED → GREEN. Required focused coverage includes authority-matrix validation, runtime token loading, stylesheet attachment/class reuse, PanelSettings policy equality, safe-area/profile classification, and Character Hub shell occupancy.

After source stabilizes, run relevant UI/EditMode suites and fresh Player captures. macOS `1600x720` remains labeled simulation evidence, not physical mobile proof. Capture manifests must record screen dimensions, panel dimensions, safe area, layout/input profile, and the PanelSettings description.

## Non-goals

- No Create Character implementation; it remains DESIGN-GATED.
- No auth/API/security redesign.
- No fake Kiếm/Pháp/Cơ/Linh renderer art.
- No world camera/rig redesign in UIF-00..02.
- No mass texture compression or Addressables migration.
- No arbitrary one-off pixel patch that bypasses the shared foundation.

## Completion gate

UIF-00..02 are ready for review when design authority is machine-checkable, product UI has one token/style foundation with a decreasing inline-style budget, PanelSettings/profile ownership is singular, compact/touch shell occupancy is intentional and regression-tested, macOS simulations are truthfully labeled, and remaining physical-device limitations are explicit rather than converted into PASS.