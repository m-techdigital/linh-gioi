# LGO Map01A UI Review Catalog v0.1

Marker: `LGO_MAP01A_UI_REVIEW_CATALOG_READY`

This catalog points reviewers to the current Map01A 2D UI evidence. It is technical review evidence, not owner approval, production auth, final wardrobe approval, or class-art approval.

## Current evidence

| Screen / flow | Evidence | Status |
| --- | --- | --- |
| Entry/login modal | `build/map01a-entry-glass-card-runtime-v3/entry-login.png` and `build/map01a-entry-glass-card-runtime-v3/manifest.json` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, `usesOsMouseOrKeyboard=false`, 1600×900 |
| Character select modal | `build/map01a-character-select-runtime/character-select.png` and `build/map01a-character-select-runtime/manifest.json` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, `usesOsMouseOrKeyboard=false`, 1600×900 |
| Quest/HUD/inventory route | `build/map01a-detail-right-player/quest-capture/{pc,tablet,mobile}/` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, pc/tablet/mobile, 18 route frames and 38 dialogue frames each |
| Inventory tabs: Hành trang + Thông tin + Vật phẩm + Rương đồ | `build/map01a-inventory-context-runtime-v2/pc/character-info.png`, `build/map01a-inventory-context-runtime-v2/pc/supplies.png`, `build/map01a-inventory-context-runtime-v2/pc/storage.png`, and `build/map01a-inventory-context-runtime-v2/pc/manifest.json` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, `usesOsMouseOrKeyboard=false`; resolution recorded in manifest; internal Player flag, no OS mouse/keyboard |
| Inventory/Vật phẩm card rows | `build/map01a-supply-card-runtime-v1/{pc,tablet,mobile}/supplies.png` and manifests | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, item rows have name/count/state cards; no approved dedicated item icons yet |

## Required review frames

- Inventory/default bag and right detail: `build/map01a-detail-right-player/quest-capture/pc/07-q04-inventory-open.png` plus tablet/mobile equivalents.
- Full route end state: `build/map01a-detail-right-player/quest-capture/pc/18-q09-portal-open.png` plus tablet/mobile equivalents.
- Inventory tab review frames: `build/map01a-inventory-context-runtime-v2/pc/character-info.png`, `build/map01a-inventory-context-runtime-v2/pc/supplies.png`, and `build/map01a-inventory-context-runtime-v2/pc/storage.png`; these must show the Map01A inventory shell, not entry/login overlay.
- Entry, character select, and inventory-tab captures use internal Player capture flags, not OS mouse or keyboard automation.

## Non-claims

- This is not owner approval.
- This is not final login/auth/account implementation.
- This is not wardrobe/class-art production approval.
- This does not change Võ div4/base/camera/scale or any registered outfit source.

## Cập nhật 2026-09-13 — kiểm hành trang theo ngữ cảnh

Evidence mới cho mỗi profile `pc/tablet/mobile`: `build/map01a-inventory-context-runtime-v2/{profile}/bag.png`, `character-info.png`, `supplies.png`, `character-info-after-supplies.png`, `storage.png`. Kích thước trong manifest; có flag device tương ứng, là mô phỏng trên macOS. Đã xem PC Thông tin sau Vật phẩm, mobile Hành trang/Vật phẩm, tablet Rương đồ. Capture batchmode ảnh xám và graphics v1 panel sai chiều rộng đã bị loại. Không nghiệm thu icon/mỹ thuật hoặc giao dịch kho thật từ checkpoint này.

## Cập nhật 2026-09-13 — card vật phẩm

Evidence mới: `build/map01a-supply-card-runtime-v1/{pc,tablet,mobile}/supplies.png` cùng `bag.png`, `character-info.png`, `character-info-after-supplies.png`, `storage.png`. Đã xem PC/mobile `supplies.png`; dòng vật phẩm có tên, badge số lượng và trạng thái riêng thay vì chữ phẳng trên button. Đây không phải nghiệm thu icon item cuối: Unity/source chưa có bộ icon vật phẩm chuyên dụng được duyệt, nên checkpoint này cố ý không thêm icon giả hoặc generated/random art.
