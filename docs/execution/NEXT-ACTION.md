# NEXT ACTION — Linh Giới Online 2D

## Trạng thái

Branch hiện tại: `feature/2d`. Owner đã yêu cầu chuyển sang game 2D, loại bỏ hướng 3D cũ, commit/push dần theo batch để đỡ rối. Cleanup 3D đã commit/push ở `d97a3c8`. Batch runtime 2D nhập môn đang là bước tiếp theo cần commit riêng sau verification.

## Gate hiện tại

- `python3.12 tools/validate_2d_branch_no_3d.py` phải pass trước khi tiếp tục gameplay 2D.
- Unity batch compile/test phải chạy thật sau các thay đổi source.
- Player smoke phải chứng minh flow hoàn tất trong Player, không chỉ Editor.
- Runtime visual capture phải có manifest và ảnh đã xem bằng mắt nếu có thay đổi player-visible.
- Không sửa frozen surfaces nếu chưa có owner decision riêng.

## Việc tiếp theo

1. Commit/push batch 2D onboarding slice nếu staged diff chỉ gồm runtime/test/tool/docs liên quan.
2. Sau đó nâng SCN-001/002 theo 2D production: sprite/art 2D đẹp hơn cho Cổng Linh Thành, Người Giữ Cổng, player, Bia Luyện Khí.
3. Chuyển HUD từ OnGUI tạm sang UI runtime capture được rõ, rồi thêm quest/tutorial/inventory shell theo lộ trình.

## Blocker

Chưa có blocker. Visual capture hiện bắt được world stage bằng camera render; HUD OnGUI đang được kiểm qua runtime flow/smoke JSON nhưng chưa xuất hiện trong ảnh camera-render.
