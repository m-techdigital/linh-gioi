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

## 2026-09-13 — Inventory grid tile v4 evidence

- Evidence: `build/map01a-inventory-grid-tile-runtime-v4/inventory/bag.png`, `character-info.png`, `supplies.png`, `storage.png`.
- Result: Hành trang now uses dense 6-column equipment cells with reserved empty inventory slots and keeps item detail on the right.
- Review status: technical runtime checkpoint only. It improves the previous sparse/row-card presentation but remains below final owner-reference quality because approved dedicated item/outfit icon art is still missing. Do not replace this with fake emoji/random/3D-derived icons.

## 2026-09-13 — Entry login depth/CTA evidence

- Evidence: `build/map01a-entry-depth-polish-runtime-v2/entry/entry-login.png`.
- Result: entry/login backdrop is less blacked out, the login panel has shared soft-depth styling, and the primary CTA has ornament rails.
- Review status: technical runtime checkpoint only; still needs richer final login visual treatment before owner acceptance.


## 2026-09-13 — Shared density/skin guard evidence

- Evidence: `build/map01a-shared-skin-density-runtime-v1/entry/entry-login.png` and `build/map01a-shared-skin-density-runtime-v1/inventory/{bag,character-info,character-info-after-supplies,supplies,storage}.png`.
- Result: shared button/tab/detail/card density is less oversized; entry primary CTA and inventory tabs now have regression coverage so they do not drift back to prototype/web-control scale.
- Review status: technical visual checkpoint only. Manual review still finds the login too form-like and inventory too table-like versus owner references. Continue with deeper shell redesign and approved art/asset treatment before owner acceptance.


## 2026-09-13 — Inventory shell polish/runtime audit evidence

- Evidence: `build/map01a-inventory-shell-polish-runtime-v1/inventory/bag.png`, `character-info.png`, `character-info-after-supplies.png`, `supplies.png`, `storage.png`.
- Result: Hành trang now has capacity/equipped badges and bottom actions; Thông tin now has a named stat strip and loadout matrix.
- Review status: `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, not owner visual acceptance. Manual audit still finds the inventory shell table-like and below the uploaded design references. Dedicated portrait/full-body/item icon art remains pending provenance-backed assets; do not fill the gap with fake/generated/random icons.

## 2026-09-13 — Inventory card/detail polish evidence

- Evidence: `build/map01a-inventory-card-polish-runtime-v2/inventory/bag.png`, `character-info.png`, `character-info-after-supplies.png`, `supplies.png`, `storage.png`.
- Rejected evidence: `build/map01a-inventory-card-polish-runtime-v1/inventory/bag.png` used crop-heavy item scaling and was visually worse for several outfit thumbnails.
- Result: Hành trang item cards and right detail card are clearer; right detail groups level/equip/fit facts in a card with chips.
- Review status: `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, not owner visual acceptance. Manual audit still finds the screen below the uploaded references because dedicated item/trang phục icons and character portrait/full-body presentation are not yet provenance-backed assets.

## 2026-09-13 — Inventory category chips evidence

- Evidence: `build/map01a-inventory-category-chips-runtime-v1/inventory/bag.png`, `supplies.png`, `character-info.png`, `storage.png`.
- Result: Hành trang category controls now render as compact chips instead of two full-width tab bars. Passive chips reserve future categories without opening fake item data.
- Review status: `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED`, not owner visual acceptance. Manual audit still finds the inventory shell below the uploaded references; continue shell/ornament/icon/portrait work using shared skin and provenance-backed assets only.
