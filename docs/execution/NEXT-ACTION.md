# NEXT ACTION — Linh Giới Online 2D

## Trạng thái

Branch hiện tại: `feature/2d`. Owner đã yêu cầu chuyển sang game 2D, loại bỏ hướng 3D cũ, bỏ Meshy và bỏ toàn bộ ảnh thiết kế/source cũ vì không dùng được nữa. Cleanup 3D đã commit/push ở `d97a3c8`, cleanup trace dịch vụ 3D đã commit/push ở `6bf905e`. Batch hiện tại đang đóng gói xoá ảnh source cũ, thêm validator no-source-images và giữ runtime 2D nhập môn/HUD world-space chạy được bằng procedural sprites.

## Gate hiện tại

- `python3.12 tools/validate_2d_branch_no_3d.py` phải pass trước khi tiếp tục gameplay 2D.
- `python3.12 tools/validate_2d_branch_no_source_images.py` phải pass để bảo đảm ảnh thiết kế/source cũ đã bị loại khỏi source tree.
- Unity batch compile/test phải chạy thật sau các thay đổi source.
- Player smoke phải chứng minh flow hoàn tất trong Player, không chỉ Editor.
- Runtime visual capture phải có manifest và ảnh đã xem bằng mắt nếu có thay đổi player-visible.
- Không sửa frozen surfaces nếu chưa có owner decision riêng.

## Việc tiếp theo

1. Commit/push batch xoá ảnh source cũ + HUD world-space sau khi staged diff chỉ gồm runtime/test/docs/validator/deletions liên quan.
2. Sau đó nâng SCN-001/002 theo 2D production bằng asset 2D mới có kiểm duyệt/design mới, không reuse ảnh thiết kế cũ.
3. Mở quest/tutorial/inventory shell theo lộ trình sau khi visual cơ bản đủ rõ.

## Blocker

Chưa có blocker. Visual capture hiện bắt được world stage và HUD world-space bằng camera render; manifest cũng lưu `hudSnapshot` để kiểm copy/trạng thái. Ảnh source cũ đã bị loại khỏi source tree và có validator riêng để ngăn tái nhập nhầm.
