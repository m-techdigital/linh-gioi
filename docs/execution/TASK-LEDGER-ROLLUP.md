# TASK LEDGER ROLLUP — 2D Pivot

## 2026-09-09 — Branch 2D cleanup

Owner yêu cầu dừng hướng dựng nhân vật/cảnh cũ và chuyển sang game 2D trên `feature/2d`. Batch cleanup đã dọn source, asset source, tooling thử nghiệm, generated staging/evidence và cache nặng liên quan hướng cũ; thêm validator `tools/validate_2d_branch_no_3d.py` để ngăn kéo lại pipeline đó. Commit/push: `d97a3c8 Remove 3D asset pipeline from 2D branch`.

## 2026-09-09 — Runtime 2D onboarding slice

Đang đóng gói slice SCN-001/002 đầu tiên: Cổng Linh Thành, Người Giữ Cổng, thoại nhập môn, đường dẫn tới Bia Luyện Khí, kích hoạt bia và trạng thái hoàn tất. Evidence đã chạy qua Editor smoke, Player smoke và visual capture 5 frame trong `build/2d-onboarding-visual/`.

## Next

Commit/push runtime 2D slice, sau đó nâng art/UI 2D cho cùng flow thay vì mở rộng hệ thống mới.
