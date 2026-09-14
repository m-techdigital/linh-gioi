## Active — CONTINUE: Character Hub dùng contract chung, class chỉ truyền data — 2026-09-15

`OPERATIONAL_GOAL_CURRENT`. Canonical vẫn là bộ năm tab tại
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

### Checkpoint hiện hành

- Character Hub, hành trang, HUD và Character Select gọi một contract chung cho actor/motion/trang bị. UI không còn gọi API `Vo*` hoặc rẽ nhánh theo tên class.
- Class khác nhau bằng profile/data (`classId`, item ID, skill, tiềm năng, linh thú). Fallback item ID cũng lấy `ActiveEquipmentClassId`, không cố định `vo_`.
- Source-pose capture chỉ khởi tạo source-pose. Runtime từ chối cấu hình có hai renderer authority; registered-outfit chỉ còn ở capture WIP explicit.
- Player mới: `build/map01a-shared-character-contract-player-v1/LinhGioiOnline.app`.
- Evidence đã xem: `build/map01a-shared-character-contract-runtime-v1/pc/`, đủ 9 frame năm tab, một actor, không wrap/cắt/chồng. Pack capture chỉ có body div4 nên chưa phải evidence nghiệm thu wardrobe/class art.

### Bước kế tiếp hợp lệ

1. Tiếp tục UI theo canonical năm tab qua shared base/contract; nội dung khác nhau chỉ bind từ profile/data.
2. Không tiếp tục phát triển class/pose/wardrobe trong task này. Không sinh/sửa source art ngẫu nhiên hoặc dựng renderer riêng cho từng class.
3. Không khôi phục renderer/resource/capture đã xóa, không chạy Player lỗi cũ và không sửa frozen surfaces.

### Gate hiện hành

- Python route/shared contract `33/33`; pose pack `12/12`; registered capture `19/19`.
- `TwoDCharacterRuntimeStateTests` `34/34`; full Unity EditMode `283 total / 282 passed / 0 failed / 1 ignored`.
- Shared-skin, no-3D, no-source-images và frozen diff pass.
- Player build `errors=0`, `warnings=46`; capture 9 frame đã xem.
- Trạng thái: `CONTINUE`; chưa claim owner visual acceptance.
