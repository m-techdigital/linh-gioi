# LGO Map01A UI Review Catalog v0.1

Marker: `LGO_MAP01A_UI_REVIEW_CATALOG_READY`

This catalog points reviewers to the current Map01A 2D UI evidence. It is technical review evidence, not owner approval, production auth, final wardrobe approval, or class-art approval.

## Current evidence

| Screen / flow | Evidence | Status |
| --- | --- | --- |
| Entry/login modal | `build/map01a-entry-form-runtime/entry-login.png` and `build/map01a-entry-form-runtime/manifest.json` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, `usesOsMouseOrKeyboard=false`, 1600×900 |
| Character select modal | `build/map01a-character-select-runtime/character-select.png` and `build/map01a-character-select-runtime/manifest.json` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, `usesOsMouseOrKeyboard=false`, 1600×900 |
| Quest/HUD/inventory route | `build/map01a-detail-right-player/quest-capture/{pc,tablet,mobile}/` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, pc/tablet/mobile, 18 route frames and 38 dialogue frames each |

## Required review frames

- Inventory/default bag and right detail: `build/map01a-detail-right-player/quest-capture/pc/07-q04-inventory-open.png` plus tablet/mobile equivalents.
- Full route end state: `build/map01a-detail-right-player/quest-capture/pc/18-q09-portal-open.png` plus tablet/mobile equivalents.
- Entry and character select use internal Player capture flags, not OS mouse or keyboard automation.

## Non-claims

- This is not owner approval.
- This is not final login/auth/account implementation.
- This is not wardrobe/class-art production approval.
- This does not change Võ div4/base/camera/scale or any registered outfit source.
