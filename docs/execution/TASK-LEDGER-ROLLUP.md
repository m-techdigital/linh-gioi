# TASK LEDGER ROLLUP — 2D Pivot

## 2026-09-09 — Branch 2D cleanup

Owner yêu cầu dừng hướng dựng nhân vật/cảnh cũ và chuyển sang game 2D trên `feature/2d`. Batch cleanup đã dọn source, asset source, tooling thử nghiệm, generated staging/evidence và cache nặng liên quan hướng cũ; thêm validator `tools/validate_2d_branch_no_3d.py` để ngăn kéo lại pipeline đó. Commit/push: `d97a3c8 Remove 3D asset pipeline from 2D branch`.

## 2026-09-09 — Runtime 2D onboarding slice

Đang đóng gói slice SCN-001/002 đầu tiên: Cổng Linh Thành, Người Giữ Cổng, thoại nhập môn, đường dẫn tới Bia Luyện Khí, kích hoạt bia và trạng thái hoàn tất. Evidence đã chạy qua Editor smoke, Player smoke và visual capture 5 frame trong `build/2d-onboarding-visual/`.

## Next

Commit/push runtime 2D slice, sau đó nâng art/UI 2D cho cùng flow thay vì mở rộng hệ thống mới.


## 2026-09-09 — HUD capture cho 2D onboarding

Chuyển HUD nhập môn từ IMGUI tạm sang text/sprite world-space để Player visual capture thấy được title, khu vực, mục tiêu, gợi ý, action, feedback và thoại NPC. Visual manifest bổ sung `hudLineCount`/`hudSnapshot`; ảnh review `03-dialogue.png` và `05-complete.png` đã được xem bằng mắt.

## Next after HUD

Nâng art 2D cho cùng flow SCN-001/002 và giữ capture runtime thật trước checkpoint.

## 2026-09-09 — Xoá ảnh source cũ cho branch 2D

Owner yêu cầu loại bỏ toàn bộ ảnh thiết kế cũ vì không dùng được nữa. Batch hiện tại xoá ảnh source/reference/runtime placeholder cũ khỏi source tree, giữ evidence trong `build/` ngoài kiểm soát source, thêm `tools/validate_2d_branch_no_source_images.py`, và xác minh runtime 2D onboarding vẫn chạy bằng procedural sprites + HUD world-space.

## Next after source image cleanup

Commit/push checkpoint, sau đó tạo lại hướng art 2D mới cho SCN-001/002 bằng design mới đã duyệt thay vì tái dùng ảnh cũ.
