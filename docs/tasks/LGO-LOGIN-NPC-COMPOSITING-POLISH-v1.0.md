# LGO Login NPC Compositing Polish v1.0

Status: `LGO_LOGIN_NPC_COMPOSITING_POLISH_READY`

## Scope

Improve the login first-screen composition using existing V3B runtime candidate assets only.

## Changes

- Rebalanced the login layout so the V3B text logo owns the main brand moment without extra overlaid title copy.
- Moved the Gate Keeper NPC into a right-side composition stage on desktop/tablet and hid it on mobile to preserve clarity and performance.
- Tightened the server selector and Vao The Gioi CTA panel so the screen reads as a game login instead of loose UI blocks.
- Kept dimensions responsive by viewport profile instead of relying on one fixed pixel layout.

## Boundaries

- V3B runtime candidate assets only.
- no V3BA.
- no reference poster import.
- no composite sheet slicing.
- no new gameplay, auth, DB, economy, social, or liveops.
- no production art claim.
- no final visual pass claim.

## Evidence

- Source validation: `python3.12 tools/validate_lgo_login_npc_compositing_polish.py`
- Runtime evidence target: `build/visual-evidence/latest/login.png`
- Profile evidence target: `build/visual-evidence/profiles/{desktop,tablet,mobile}/login.png`
