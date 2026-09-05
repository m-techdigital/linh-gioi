# LGO Character Hall Mobile Selected CTA Evidence Refresh v1.0

Status: `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_EVIDENCE_REFRESH_READY`

## Scope

This pass refreshes runtime profile screenshots after the selected-character CTA hierarchy polish.

## Evidence

- `build/visual-evidence/profiles/mobile/character-lobby.png`
- `build/visual-evidence/profiles/mobile/character-select.png`
- `build/visual-evidence/profiles/index.md`

## Review Notes

- No-character mobile state now shows `Vào sân luyện` with a visibly disabled treatment.
- Selected-character mobile state prioritizes `Vào sân luyện` before the secondary `Tạo thêm` action.
- The player can still create another character; the change is presentation hierarchy only.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay change.
- No account or character-flow semantic change.
- No new runtime image import.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Continue with the next runtime visual debt visible in screenshots: login panel/CTA polish, world hub spacing, or evidence tooling hardening, depending on `NEXT-ACTION.md`.
