# LGO Session Menu Setting Row Visual Polish v1.0

Status: `LGO_SESSION_MENU_SETTING_ROW_VISUAL_POLISH_READY`

## Scope

This pass improves the Session Menu local setting rows through shared reusable runtime UI helpers. The goal is to make settings read as part of the V3B pause shell instead of plain tool rows, while keeping the existing local-only setting behavior unchanged.

## Implementation Notes

- `RuntimeUiSkin.ApplySettingToggleFrame` now owns the V3B row frame, touch-safe height, glass background, border rhythm, and type weight for local setting toggles.
- `RuntimeUiSkin.ApplySettingToggleStatePill` owns the compact Vietnamese `Bật` / `Tắt` badge used by setting rows.
- `RuntimeUiFactory.NewLocalSettingToggle` adds the shared state pill and routes state changes back through `RuntimeUiSkin.ApplySettingToggleState`.
- Existing setting semantics are preserved: `Tọa độ`, `Chỉ dẫn`, and `HUD gọn` still drive the same local display flags.

## Evidence

- Runtime screenshot refresh pending under `build/visual-evidence/latest/session-menu.png`.
- Visual pass is not claimed until screenshots are captured and reviewed after this source change.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new runtime image payload.
- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-SESSION-MENU-SETTING-ROW-EVIDENCE-REFRESH-v1.0`: build, capture, and review the Session Menu screenshot after the shared setting-row polish.
