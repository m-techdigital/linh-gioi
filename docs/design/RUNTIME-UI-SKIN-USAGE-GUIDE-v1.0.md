# Runtime UI Skin Usage Guide v1.0

Status: `LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY`

## Purpose

`RuntimeUiSkin` is the shared runtime styling surface for playable UI that is built in C# UI Toolkit code. New UI should reuse these helpers before adding new one-off style blocks in `M4PlayableClientController`.

The goal is consistency and maintainability: one role should have one visual owner, while layout and responsive placement can remain close to the screen logic that owns the flow.

## Current Helper Ownership

Primitive helpers:

- `ApplyRadius`: corner radius only.
- `ApplyPadding`: spacing only.
- `ApplyEdgeFrame`: explicit border color/width only.

Generic runtime roles:

- `ApplyPanelFrame`: default compact glass panel.
- `ApplyInsetRowFrame`: framed row or short inline status block.
- `ApplyBaseButtonFrame`: normal text button shell.
- `ApplyCompactActionFrame`: dense action button shell with explicit state colors.
- `ApplyRuntimeIconFrame`: square icon frame.
- `ApplySettingToggleFrame`: local settings toggle row.
- `ApplySettingToggleState`: local settings toggle state accent.
- `ApplyBadgeFrame`: small non-interactive badge.
- `ApplyToastFrame`: transient message shell.
- `ApplyStatusChipFrame`: status chip shell.

Login roles:

- `ApplyLoginCtaBacking`: quiet login CTA readability backing.
- `ApplyServerSelectorFrame`: server selector row.

Character Hall roles:

- `ApplyCharacterHallPanelFrame`: main lobby/character hall panel.
- `ApplyCharacterListFrame`: character list surface.
- `ApplyCharacterPreviewFrame`: selected character preview.
- `ApplyCharacterCreateFrame`: create-character form surface.
- `ApplyCharacterPortraitFrame`: portrait stage frame.
- `ApplyLobbyInputFrame`: lobby text input shell.
- `ApplyEmptyCharacterCardFrame`: empty-character guidance card.

World/HUD roles:

- `ApplyPreviewPanelFrame`: world preview or embedded preview panel.
- `ApplyWorldHudGroupFrame`: grouped HUD/action/status block.
- `ApplyHudStatusCompactFrame`: compact HUD status label.
- `ApplySessionMenuFrame`: session menu surface.
- `ApplyLocalSettingsPanelFrame`: local display settings panel.
- `ApplyCombatCooldownIconFrame`: combat cooldown icon shell.
- `ApplyCombatCooldownIconState`: combat cooldown state accent.
- `SessionMenuBackground`: responsive session menu background color.
- `WorldHudBackground`: responsive world HUD background color.

## Rules For New UI

- Prefer a role helper when the element has a stable product meaning: login CTA, panel, list, preview, status chip, session menu, world HUD group, or character form.
- Use primitive helpers only for local spacing, layout glue, or genuinely new composition work before a role helper is justified.
- If a new screen repeats the same color/border/radius block twice, add or extend a role helper instead of duplicating it.
- Keep responsive placement, width/height clamps, and device profile branching in the owning controller or view builder unless it becomes reused across screens.
- Do not put final art-quality claims in helper names or docs. These helpers describe runtime structure and presentation, not production art approval.
- Do not touch frozen surfaces to add UI style helpers.

## When To Add A New Helper

Add a new helper when at least one of these is true:

- The same frame/panel/button treatment appears in more than one screen.
- The role is a repeated UI concept in the roadmap.
- A validator needs one stable hook to prevent style drift.
- A screenshot review finds inconsistent borders, density, or contrast across related screens.

Do not add a new helper for a single one-line margin, a one-off width clamp, or screen-specific text/layout behavior.

## Validation Expectations

Runtime UI skin adoption is source-validated by:

- `tools/validate_lgo_runtime_ui_skin_foundation.py`
- `tools/validate_lgo_runtime_ui_skin_adoption_audit.py`
- `tools/validate_lgo_character_hall_style_adoption.py`
- `tools/validate_lgo_world_hud_style_adoption.py`
- `tools/validate_lgo_runtime_ui_skin_usage_guide.py`

Visual confidence still requires screenshot review through the visual evidence harness. Source validators cannot claim `VISUAL_RUNTIME_PASS`.

## Next Safe Adoption Targets

- Review remaining direct `RuntimeArtCatalog` style use in `M4PlayableClientController` and classify each call as layout glue, state-specific color, or candidate role helper.
- Split highly repeated UI construction into reusable factories only when it reduces real duplication without hiding screen flow.
- Keep login, Character Hall, World Hub, NPC Dialogue, and Session Menu aligned to this guide before opening new UI-heavy gameplay screens.
