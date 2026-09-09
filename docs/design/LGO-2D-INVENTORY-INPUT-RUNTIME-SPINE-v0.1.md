# Linh Giới Online — 2D Inventory Input Runtime Spine v0.1

Ngày cập nhật: 2026-09-09. Batch này nâng inventory try-on từ strip preview tĩnh thành runtime input spine có thể mở, chọn, thử, áp dụng và hủy ở trong Player capture.

## Scope

Flow bám quyết định owner đã duyệt: chọn icon → xem riêng món/inspect → thử trên người → hủy thử hoặc áp dụng. Đây vẫn là local runtime spine, chưa phải inventory/economy/server persistence thật.

## Input hiện tại

| Phím | Hành vi |
| --- | --- |
| `I` | Mở/đóng panel hành trang |
| `Tab` | Chọn item kế tiếp trong danh sách thử nghiệm |
| `T` | Thử item đang chọn trên người |
| `Y` | Áp dụng preview đang thử |
| `Esc` | Hủy preview và đóng panel |

Item seed hiện dùng các module đã có: Võ Lv1 starter, `top_kiem_lv1_male` và `weapon_kiem_lv1_starter`. Mọi thao tác đi qua `TwoDCharacterLoadout` để giữ cùng cơ chế preview/apply/cancel và không tạo luật fit riêng cho từng món.

## Evidence rules

- `RuntimeInventoryInputSnapshot` phải ghi `InventoryInputState`, item selected, controls và loadout snapshot.
- Visual manifest phải chứa `runtimeInventoryInputSnapshot`.
- Player capture phải có frame `09-inventory-try` và `10-inventory-applied` để chứng minh runtime flow, không chỉ source inspection.
- Panel không được che HUD, slime/combat state hoặc tuyến đọc movement chính.
