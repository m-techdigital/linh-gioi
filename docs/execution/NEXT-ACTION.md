## Active — NEED_HUMAN_VISUAL_REVIEW: Character Hub chỉ dùng một actor source-pose — 2026-09-14

`OPERATIONAL_GOAL_CURRENT`. Canonical vẫn là bộ năm tab tại
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.

### Checkpoint hiện hành

- Đã xóa `TwoDClassMixedLoadoutFitPreview`, class capture component/tool/test và bốn resource pack tĩnh Kiếm/Pháp/Cơ/Linh gây nhân vật rời thân. Validator hiện hành cấm đưa các đường này trở lại.
- Màn Nhân vật chỉ render chính actor source-pose đang hoạt động trên map. Portrait giữ stage `400×428`; 10 slot neo hai rail `76 px`; icon lấy atlas UI rõ, không lấy crop renderer cũ.
- Class selector chỉ bật khi có nhiều source-pose pack hợp lệ. Không có pack class thì giữ Võ; không tự dựng hoặc tiếp tục phát triển class.
- Player: `build/map01a-source-pose-character-hub-player-v2/LinhGioiOnline.app`.
- Evidence đã xem: `build/map01a-source-pose-character-hub-runtime-v2/pc/character-info.png`; manifest cùng thư mục đạt 9 frame, `usesOsMouseOrKeyboard=false`.

### Bước kế tiếp hợp lệ

1. Owner xem Player/evidence v2 cho màn Nhân vật. Chỉ sửa khi có visual regression cụ thể so với canonical.
2. Sau khi owner duyệt, chọn UI screen tiếp theo theo yêu cầu mới và design-first; không tự quay lại class, pose, wardrobe hoặc source art.
3. Không khôi phục renderer/resource/capture tĩnh đã xóa và không sửa frozen surfaces.

### Gate hiện hành

- Full Unity EditMode: `283 total / 282 passed / 0 failed / 1 ignored`.
- Pose pack `12/12`; registered capture `19/19`; shared-skin `21/21`.
- Shared-skin, no-3D, no-source-images và frozen diff pass.
- Trạng thái: `NEED_HUMAN_VISUAL_REVIEW`; chưa claim owner visual acceptance.
