# NEXT ACTION — Linh Giới Online 2D

## Trạng thái

Branch hiện tại: `feature/2d`. Owner đã yêu cầu chuyển sang game 2D, loại bỏ hướng 3D cũ, bỏ Meshy và bỏ toàn bộ ảnh thiết kế/source cũ vì không dùng được nữa. Cleanup 3D đã commit/push ở `d97a3c8`, cleanup trace dịch vụ 3D ở `6bf905e`, xoá ảnh source cũ ở `646492e`. Batch hiện tại đang nâng runtime Đông Môn tutorial bằng procedural 2D scene beats để có blockout player-visible trước khi triển khai map A-Z theo 3 board map mới owner gửi.

## Gate hiện tại

- `python3.12 tools/validate_2d_branch_no_3d.py` phải pass trước khi tiếp tục gameplay 2D.
- `python3.12 tools/validate_2d_branch_no_source_images.py` phải pass để bảo đảm ảnh thiết kế/source cũ đã bị loại khỏi source tree.
- Unity batch compile/test phải chạy thật sau các thay đổi source.
- Player smoke phải chứng minh flow hoàn tất trong Player, không chỉ Editor.
- Runtime visual capture phải có manifest và ảnh đã xem bằng mắt nếu có thay đổi player-visible.
- Không sửa frozen surfaces nếu chưa có owner decision riêng.

## Việc tiếp theo

1. Commit/push batch nâng blockout Đông Môn procedural nếu staged diff chỉ gồm runtime/test/docs liên quan.
2. Lập spec triển khai map A-Z từ board mới: world map tổng thể, sơ đồ kết nối khu vực, Linh Thành hub, Đông Môn tutorial, layer parallax, tileset/thành phần map, NPC/monster mẫu, ký hiệu map và flow trải nghiệm.
3. Triển khai runtime từng lát: trước tiên Đông Môn tutorial side-scrolling, sau đó hub Linh Thành overview/world-map shell; không reuse ảnh thiết kế cũ và không mở frozen contracts.

## Blocker

Chưa có blocker. Visual capture hiện bắt được world stage, HUD world-space và scene-beat metadata bằng camera render; manifest lưu `hudSnapshot` + `productionSceneBeatSnapshot` để kiểm copy/trạng thái. Ảnh source cũ đã bị loại khỏi source tree và có validator riêng để ngăn tái nhập nhầm.
