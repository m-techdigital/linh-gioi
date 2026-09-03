# M4 Visible UI Usability And Review Harness v0.14.0

Status: `M4_VISIBLE_UI_USABILITY_SOURCE_READY`

This task addresses a visible macOS player review finding: the M4 playable UI could run but was not reliably readable or navigable at 1280x720.

Changed:

- Reduced visible UI shell width and state complexity.
- Hid the full auth panel after login so Character Hall becomes the primary focus.
- Added clearer API status text.
- Added visible Quit actions and Escape key exit.
- Added `tools/run_m4_visible_ui_review.sh` for manual 1280x720 review.
- Added `tools/validate_m4_visible_ui.py`.
- Added `docs/execution/M4-VISIBLE-UI-REVIEW-COMMAND-v0.14.0.md`.
- Added the visible UI validator to `tools/lgo_m4_closure_check.sh`.

Did not change:

- no gameplay systems
- no combat
- no inventory
- no quests
- no guild, party, market, chat, economy, or live ops
- no production auth
- no DB persistence
- no protocol changes
- no GameData schema changes
- no ADR changes
- no design token changes
- no final production art claim

Validation:

```bash
git --no-pager diff --check
python3.12 tools/validate_m4_visible_ui.py
./tools/lgo_m4_closure_check.sh --source-only
./tools/lgo_m4_closure_check.sh --package-ready
```
