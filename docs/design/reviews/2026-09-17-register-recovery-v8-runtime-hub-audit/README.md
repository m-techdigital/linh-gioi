# Register/Recovery runtime Hub audit v8

Review-only. No auth runtime implementation is changed by this package.

## Authority checked
- Entry family: owner v5→v9 designs.
- Character Hub design: owner-approved v4 five-tab set.
- Character Hub runtime: V59 Player evidence under `build/character-hub-whole-polish-v59/runtime-final/`.
- Current auth runtime: P0 whole-flow evidence under `build/whole-flow-p0-v1/`.

## Required runtime reuse after visual approval
- Auth modal outer chrome must reuse the existing Character Hub surface/frame language; do not create a parallel auth skin.
- Primary gold CTA must reuse `ApplyLgoCharacterHubGoldAction` / `character-hub-action-gold` semantics rather than restyling a generic button to look similar.
- Shared inputs continue through `ApplyLgoInputField`/Entry field icon + reveal helpers.
- Inner content should use the same section/panel depth as Character Hub, without adding five tabs, detail-right, or a Hub close button to auth flow.
- Entry v5→v9 scene, auth titles, one-column hierarchy and utility rail remain authoritative.
- Modal opening motion should reuse the proven Hub fade timings where the same overlay lifecycle applies.

## Review conclusion
V7 remains the visual candidate. This audit changes implementation guidance, not the approved Entry composition: implement by reusing the proven Hub chrome helpers/assets instead of introducing new look-alike auth CSS/UI Toolkit styling.
