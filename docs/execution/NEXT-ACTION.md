# NEXT ACTION — Linh Giới Online 2D

## Trạng thái

Branch hiện tại: `feature/2d`. Owner đã khóa hướng mới: **2D Side-Scrolling Social Action MMORPG**, HD anime/stylized, không pixel-art, không Meshy/3D, map parallax nhiều lớp, social hub + action combat. Cleanup 3D đã commit/push ở `d97a3c8`, cleanup trace dịch vụ 3D ở `6bf905e`, xoá ảnh source cũ ở `646492e`, Đông Môn procedural blockout ở `eeaf19d`. Batch hiện tại đang cập nhật direction lock + map A-Z spec + runtime map catalog theo kịch bản mới để tránh đi lệch trước khi design/triển khai tiếp.

## Gate hiện tại

- `python3.12 tools/validate_2d_branch_no_3d.py` phải pass trước khi tiếp tục gameplay 2D.
- `python3.12 tools/validate_2d_branch_no_source_images.py` phải pass để bảo đảm ảnh thiết kế/source cũ đã bị loại khỏi source tree.
- Unity batch compile/test phải chạy thật sau các thay đổi source.
- Player smoke phải chứng minh flow hoàn tất trong Player, không chỉ Editor.
- Runtime visual capture phải có manifest và ảnh đã xem bằng mắt nếu có thay đổi player-visible.
- Không sửa frozen surfaces nếu chưa có owner decision riêng.

## Việc tiếp theo

1. Commit/push batch direction lock + map catalog sau khi source validators, Unity EditMode, onboarding smoke và Player visual capture pass.
2. Bắt đầu 2D-01: Male/Female Base Character bằng source/runtime procedural hoặc asset 2D mới được duyệt, có layer body/underwear/hair rõ và không dùng ảnh cũ.
3. Sau 2D-01 mới sang 2D-02 Character Modular Runtime; map production lớn giữ cho 2D-09/2D-10, nhưng Đông Môn tutorial blockout vẫn được dùng làm runtime evidence.

## Blocker

Chưa có blocker. Visual capture hiện bắt được world stage, HUD world-space, minimap/route overlay và scene-beat metadata bằng camera render; manifest lưu `hudSnapshot`, `productionSceneBeatSnapshot`, `runtimeMapSnapshot` để kiểm copy/trạng thái. Ảnh source cũ đã bị loại khỏi source tree và có validator riêng để ngăn tái nhập nhầm.
