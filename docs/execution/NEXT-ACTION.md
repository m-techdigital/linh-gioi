## Active — CONTINUE: Character Hub 5 tab × 5 class — 2026-09-14

`OPERATIONAL_GOAL_CURRENT`. Goal hiện hành là hoàn thiện Character Hub theo canonical duy nhất:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

Năm tab dùng chung một shell/base: `Nhân vật`, `Rương đồ`, `Kỹ năng`, `Tiềm năng`, `Linh thú`. Năm class dùng chung component tree và thứ tự `Võ → Kiếm → Pháp → Cơ → Linh`; dữ liệu/trạng thái phải tách riêng theo class.

### Checkpoint đã khóa

- Full filigree chỉ dùng ở shell ngoài; workspace/detail-right dùng section border 1 px; icon/card dùng inset border một lớp.
- Button có chrome texture giữ border trang trí bên trong texture và outer/CSS border bằng `0`.
- Tab `48 px / 17 px`, close `52 px / 27 px`; cùng shared helper trên cả năm tab.
- Selector class nằm ở title row, không tạo tab thứ sáu và không phụ thuộc source-pose launcher.
- Snapshot slot đang chọn, tháo/mặc và cấp đồ đã tách theo từng class; baseline Võ giữ icon UI rõ hiện hành.
- Player: `build/map01a-five-tab-depth-player-v12/LinhGioiOnline.app`. Evidence PC cuối: `build/map01a-five-tab-depth-runtime-v12/pc/`; evidence ba viewport của frame/button: `build/map01a-five-tab-depth-runtime-v11/{pc,mobile,tablet}/`.

### Batch kế tiếp

1. Tạo một catalog/profile dữ liệu chung cho đúng năm class, bind Kỹ năng/Tiềm năng/Linh thú trong cùng layout; không hardcode Kiếm cho mọi class.
2. Giữ selected/equipped/level/skill/potential state riêng khi chuyển class và giữ nguyên tab đang mở.
3. Full-body class preview chỉ lấy từ `LGOClasses/*MixedLoadoutFitPreview` hiện có qua renderer/capture rõ ràng. Không generate/redraw, không sửa pose/source/wardrobe và không đưa crop tối vào UI như icon final.
4. Thêm capture tự động đủ `5 class × 5 tab` ở PC, mobile landscape và tablet; xem trực tiếp toàn bộ trước khi claim pass.
5. Chạy full EditMode, shared-skin, no-3D, no-source-images, frozen diff; cập nhật state rồi commit/push một batch coherent.

### Gate hiện hành

- Python shared-skin `20/20`.
- Unity EditMode `286 total / 285 passed / 0 failed / 1 ignored`.
- `validate_lgo_ui_shared_skin.py`, no-3D, no-source-images và frozen diff pass.
- Trạng thái: `CONTINUE`; chưa hoàn tất profile/preview/evidence 5 class.

Không quay lại pose/source/wardrobe hoặc tạo art class bằng phương pháp ngẫu nhiên. Không sửa frozen surfaces.
