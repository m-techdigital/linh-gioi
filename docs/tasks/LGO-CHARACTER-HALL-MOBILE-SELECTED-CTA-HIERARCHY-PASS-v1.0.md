# LGO Character Hall Mobile Selected CTA Hierarchy Pass v1.0

Status: `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_HIERARCHY_READY`

## Scope

This pass improves the selected-character mobile Character Hall hierarchy without changing account, character creation, selection, or enter-world semantics.

## Changes

- Mobile selected state moves `Vào sân luyện` before the secondary create action.
- Mobile selected state relabels the secondary create action as `Tạo thêm`.
- Enter-world CTA receives stronger size and priority only when a character is selected on mobile.
- Desktop/tablet default create/select layout remains unchanged.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay change.
- No account or character-flow semantic change.
- No new runtime image import.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Refresh profile screenshots and verify mobile `character-select.png` shows the primary enter-world action clearly.
