# Runtime UI Responsive Layout Helper Review v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY`

## Scope

This pass reviews and extracts pure responsive layout calculations from `M4PlayableClientController` without moving screen state.

## Decision

`RuntimeUiLayoutProfile` now owns the reusable, side-effect-free profile calculation for:

- viewport fallback width and height;
- profile name selection: `desktop`, `tablet`, `mobile`;
- mobile short-side scale;
- login logo width/height;
- login CTA card width/padding;
- login primary button height/font size.

The controller still owns applying those values to live UI elements because that code depends on visible screen state, dialogue/session state, and existing runtime references.

## Runtime Viewport Standard

Use this standard for all future player-visible UI fixes before changing size constants:

- Source of truth: use resolved UI Toolkit panel/root viewport metrics, not screenshot pixels or raw `Screen.width/height`, for UI element sizing.
- Scale policy: runtime `PanelSettings` must stay on `ScaleWithScreenSize` with the shared runtime reference policy; profile helpers decide `desktop`, `tablet`, or `mobile` from short/long side bands.
- Safe area policy: convert safe-area/player-window values once into panel-space before applying margins or clamps.
- Container policy: large panels must have bounded max sizes; optional or long content goes into an owned content container or `ScrollView`, while primary actions remain visible without scrolling.
- Priority policy: each screen must declare what is critical per state. Login keeps logo/server/CTA; Character Hall keeps selected/create CTA; World HUD keeps current objective/action; Session Menu keeps resume/save/back/quit.
- Evidence policy: visible fixes require profile screenshots for desktop/tablet/mobile unless the change is intentionally profile-specific. Do not claim `VISUAL_RUNTIME_PASS` from capture alone.

## Responsive Formula

Do not tune a visible UI element from raw screenshot size alone. Compute layout from the resolved panel-space safe viewport:

- `safeWidth = uiViewportWidth - safePaddingLeft - safePaddingRight`
- `safeHeight = uiViewportHeight - safePaddingTop - safePaddingBottom`
- `panelWidth = clamp(safeWidth * profileRatio, minReadableWidth, maxDesignedWidth)`
- `panelMaxHeight = safeHeight - requiredOuterMargins`
- `contentMaxHeight = panelMaxHeight - fixedHeaderHeight - fixedActionHeight - fixedPaddingAndGaps`
- `columnWidth = (100% - gap * (columns - 1)) / columns`

If `contentMaxHeight` is smaller than the content's natural height, the content area must become a vertical `ScrollView`. It must not increase panel height beyond `panelMaxHeight`. If `columnWidth` cannot preserve readable tap targets, the row must reduce columns or move secondary actions below the primary action.

Dialogue current ratios:

- Total dialogue panel max height: mobile `50vh`, tablet `48vh`, desktop `52vh` in UI Toolkit panel-space units.
- Dialogue is an overlay sibling of the HUD, not a child of the HUD column.
- Dialogue overlay uses equal top/bottom insets: `(safeHeight - dialogueMaxHeight) / 2`, clamped above the profile margin floor.
- Dialogue width is clamped from safe viewport width, then left/right insets are computed symmetrically.
- Dialogue line scroll max height: mobile `12vh`, tablet/desktop `16vh`.
- Dialogue line scroll min height: mobile `8vh`, tablet/desktop `10vh`.
- Dialogue panel vertical margin uses one shared top/bottom value per profile, so bottom margin may not be smaller than top margin.

## Anti-Overflow Contract

Every runtime panel must follow these rules:

- Root/panel width is clamped by safe viewport, then children use parent-relative `width/maxWidth: 100%`.
- Long narrative/status content is bounded in a content viewport or `ScrollView`; it is never allowed to push actions out of the screen.
- Primary actions stay visible without scrolling. Secondary content can collapse, hide, or scroll by profile priority.
- Button metrics come from the profile/system constants for the screen state; button text does not decide container width.
- Button sizing uses shared semantic tiers from `RuntimeUiButtonTier`: `Small`, `Compact`, `Standard`, `Primary`, and `Hero`. Screen-level code should select a tier by action priority instead of passing one-off height/font values.
- Horizontal action rows use a column formula. Dialogue actions use two equal columns when both actions must remain visible in a compact HUD.
- Margin top/bottom is profile-ratio based or shared spacing, not a one-off fixed correction from a single screenshot. Dialogue panels must use symmetrical vertical margins so text length cannot visually pin the panel to the bottom edge.
- Dialog/modal structure must be explicit: shell/header stays fixed, body owns scrollable content, footer owns progress/actions and does not scroll. Dialog overlays must be siblings of HUD panels, not nested inside HUD flow.

Runtime owner: `RuntimeUiOverflowGuard` centralizes bounded action rows, bounded scroll regions, and responsive action columns.

Overlay owner: `RuntimeUiOverflowGuard.ApplyViewportOverlaySurface` is the shared base for center/left/right overlay placement. New modal/dialog surfaces should select `RuntimeUiOverlayPlacement.Center`, `Left`, or `Right` and pass viewport-derived width, max-height, and insets instead of setting absolute coordinates by hand.

## Target Case Matrix

This matrix is the current visual target contract. Demo coverage is being expanded from the north-star sheet into profile-specific target sheets; runtime work must still follow this matrix immediately.

| Screen/state | Critical visible content | Overflow rule | Current target/evidence |
|---|---|---|---|
| Login | logo, server/account state, `Vào Thế Giới` CTA | hide decorative stage on mobile before shrinking CTA | latest profile evidence, target sheet pending |
| Character Hall empty | character list/create route, readable empty state | list/form owns bounded columns; long hints wrap inside card | target sheet pending |
| Character Hall selected | selected character and enter-world CTA | mobile uses bottom action dock; secondary create remains smaller | `docs/reference-ui/lgo-runtime-ui-north-star-v1.png` and latest mobile evidence |
| Character Hall create | name input and create/cancel actions | form stays bounded; input uses max width; actions never exceed parent | target sheet pending |
| World Hub steady | current objective, nearest interaction, top quit/status | auxiliary debug/meta hides on compact profiles | `docs/reference-ui/lgo-runtime-ui-north-star-v1.png` |
| World dialogue | speaker, dialogue line, progress, continue/close | line scrolls vertically; actions remain visible; mobile stacks actions | latest `npc-dialogue.png` evidence |
| World long dialogue | speaker, long dialogue body, progress, continue/close | body scrolls within max-height; footer stays visible and inside panel | latest `npc-dialogue-long.png` evidence |
| World target dummy/combat | combat focus and actionable feedback | objective signal has priority over unrelated target label | latest target-dummy evidence |
| Skill preview | selected skill, cooldown/telegraph action | compact profiles hide footer/guidance clutter | latest skill-preview evidence |
| Session menu | resume/save/back/quit | details/settings collapse or scroll; action grid remains visible | `docs/reference-ui/lgo-runtime-ui-north-star-v1.png` and latest session-menu evidence |

## North-Star Reference

Reference images:

- `docs/reference-ui/lgo-runtime-ui-north-star-v1.png`
- `docs/reference-ui/lgo-runtime-ui-mobile-tablet-targets-v1.png`

Purpose:

- Treat it as the target composition direction for Character Hall, World Hub HUD, and Session Menu.
- Use it to judge hierarchy, spacing, bounded panels, and content priority before tuning runtime constants.
- Do not import it as runtime art and do not slice/crop it into UI assets.
- Treat generated labels/layout as composition reference, not pixel-perfect copy. Runtime implementation must still follow current game flow, current allowed systems, and profile evidence.

Immediate runtime deltas to close against the reference:

- Character Hall selected state should move toward a stable action bar with less floating-card clutter.
- World Hub should keep one dominant objective signal and avoid secondary labels stealing focus.
- Session Menu should remain a compact centered pause modal with primary actions visible first.
- Dialogue should keep narrative text inside a bounded content viewport, never letting text length change modal height or action visibility.

## Why This Boundary

The extracted values are pure calculations and can be reused by future screen-specific polish without duplicating profile thresholds. The remaining layout application code is intentionally left in `M4PlayableClientController` because it still coordinates auth, lobby, world HUD, session menu, dialogue, and evidence states.

## Follow-Up

Continue with `LGO-RUNTIME-UI-RESPONSIVE-CONSTANTS-AUDIT-v1.0` to identify whether world HUD/session/Character Hall viewport clamps should become named profile constants. Do not extract them until there is a clear repeated-use benefit.

## Non-Claims

- No gameplay change.
- No account/character flow semantics change.
- No combat mechanic change.
- No runtime image payload change.
- No visual runtime PASS claim.
