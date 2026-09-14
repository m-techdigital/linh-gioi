## Active — CONTINUE: Character Hub chỉ dùng một actor source-pose — 2026-09-15

`OPERATIONAL_GOAL_CURRENT`. Canonical vẫn là bộ năm tab tại
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

### Checkpoint hiện hành

- Đã loại thêm hai đường renderer sai còn sót: launcher source-pose không còn khởi tạo registered-outfit; khi source-pose hoạt động, atlas/rig `Map01A Võ avatar` bị khóa `forceRenderingOff` và không được chạy tiếp nhánh presentation cũ.
- Đã xóa `TwoDClassMixedLoadoutFitPreview`, class capture component/tool/test và bốn resource pack tĩnh Kiếm/Pháp/Cơ/Linh gây nhân vật rời thân. Validator hiện hành cấm đưa các đường này trở lại.
- Màn Nhân vật chỉ render chính actor source-pose đang hoạt động trên map. Portrait giữ stage `400×428`; 10 slot neo hai rail `76 px`; icon lấy atlas UI rõ, không lấy crop renderer cũ.
- Class selector chỉ bật khi có nhiều source-pose pack hợp lệ. Không có pack class thì giữ Võ; không tự dựng hoặc tiếp tục phát triển class.
- Player: `build/map01a-source-pose-exclusive-player-v1/LinhGioiOnline.app`.
- Evidence đã xem: `build/map01a-source-pose-exclusive-runtime-v1/pc/character-info.png`; manifest cùng thư mục đạt 9 frame, `usesOsMouseOrKeyboard=false`.

### Bước kế tiếp hợp lệ

1. Tiếp tục bind/audit Character Hub trên chính actor source-pose; không tạo renderer nhân vật thứ hai và không sinh/sửa source art ngẫu nhiên.
2. Mọi class được mở trong selector phải có pack source-pose hợp lệ và giữ state riêng; thiếu pack thì không hiện class, không fallback sang atlas/rig cũ.
3. Không khôi phục renderer/resource/capture đã xóa và không sửa frozen surfaces.

### Gate hiện hành

- Full Unity EditMode: `283 total / 282 passed / 0 failed / 1 ignored`.
- Pose pack `12/12`; registered capture `19/19`; shared-skin `21/21`.
- Shared-skin, no-3D, no-source-images và frozen diff pass.
- Launcher `10/10`; `TwoDCharacterRuntimeStateTests` `31/31`; full EditMode `283 total / 282 passed / 0 failed / 1 ignored`; pose pack `12/12`; registered capture `19/19`; shared-skin `21/21`; no-3D/no-source/frozen pass. Player build `errors=0`, `warnings=46`.
- Trạng thái: `CONTINUE`; chưa claim owner visual acceptance.
