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
| Login + inventory hierarchy polish | `build/map01a-ui-hierarchy-polish-runtime-v2/entry/entry-login.png`, `build/map01a-ui-hierarchy-polish-runtime-v2/inventory/supplies.png`, `character-info.png`, `storage.png` | `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, graphics Player capture, no OS mouse/keyboard; still not final design acceptance |

## Required review frames

- Inventory/default bag and right detail: `build/map01a-detail-right-player/quest-capture/pc/07-q04-inventory-open.png` plus tablet/mobile equivalents.
- Full route end state: `build/map01a-detail-right-player/quest-capture/pc/18-q09-portal-open.png` plus tablet/mobile equivalents.
- Inventory tab review frames: `build/map01a-inventory-context-runtime-v2/pc/character-info.png`, `build/map01a-inventory-context-runtime-v2/pc/supplies.png`, and `build/map01a-inventory-context-runtime-v2/pc/storage.png`; these must show the Map01A inventory shell, not entry/login overlay.
- Entry, character select, and inventory-tab captures use internal Player capture flags, not OS mouse or keyboard automation.

## Item icon source audit

- Current audit: `docs/design/LGO-MAP01A-ITEM-ICON-SOURCE-AUDIT-v0.1.md`.
- Result: No approved dedicated UI icon set currently exists for Map01A HP/MP consumables or starter reward, so runtime UI must keep text/count/state cards until provenance-backed item art is available.
- Quality guard: do not replace missing item art with fake/generic/generated icons or class wardrobe crops.

## Non-claims

- This is not owner approval.
- This is not final login/auth/account implementation.
- This is not wardrobe/class-art production approval.
- This does not change Võ div4/base/camera/scale or any registered outfit source.

## Cập nhật 2026-09-13 — kiểm hành trang theo ngữ cảnh

Evidence mới cho mỗi profile `pc/tablet/mobile`: `build/map01a-inventory-context-runtime-v2/{profile}/bag.png`, `character-info.png`, `supplies.png`, `character-info-after-supplies.png`, `storage.png`. Kích thước trong manifest; có flag device tương ứng, là mô phỏng trên macOS. Đã xem PC Thông tin sau Vật phẩm, mobile Hành trang/Vật phẩm, tablet Rương đồ. Capture batchmode ảnh xám và graphics v1 panel sai chiều rộng đã bị loại. Không nghiệm thu icon/mỹ thuật hoặc giao dịch kho thật từ checkpoint này.

## Cập nhật 2026-09-13 — card vật phẩm

Evidence mới: `build/map01a-supply-card-runtime-v1/{pc,tablet,mobile}/supplies.png` cùng `bag.png`, `character-info.png`, `character-info-after-supplies.png`, `storage.png`. Đã xem PC/mobile `supplies.png`; dòng vật phẩm có tên, badge số lượng và trạng thái riêng thay vì chữ phẳng trên button. Đây không phải nghiệm thu icon item cuối: Unity/source chưa có bộ icon vật phẩm chuyên dụng được duyệt, nên checkpoint này cố ý không thêm icon giả hoặc generated/random art.

## Cập nhật 2026-09-13 — hierarchy polish login/hành trang

Evidence mới: `build/map01a-ui-hierarchy-polish-runtime-v2/entry/entry-login.png` và `build/map01a-ui-hierarchy-polish-runtime-v2/inventory/{bag,character-info,character-info-after-supplies,supplies,storage}.png`. Capture v1 chạy `-batchmode` cho ảnh xám đã bị loại; v2 chạy graphics Player, `usesOsMouseOrKeyboard=false`. Login đã bỏ copy debug `chưa mở` khỏi các affordance chính; inventory đổi copy trạng thái vật phẩm sang player-facing hơn và tăng nhẹ hierarchy/detail. Visual vẫn chỉ là technical checkpoint, chưa đạt sát design owner vì còn thiếu logo/ornament/icon/item art thật và card vẫn còn tính kỹ thuật.
