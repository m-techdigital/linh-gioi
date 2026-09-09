# NEXT ACTION — Linh Giới Online 2D

## Trạng thái

Branch hiện tại: `feature/2d`. Owner đã yêu cầu chuyển sang game 2D và loại bỏ hướng dựng nhân vật/cảnh cũ khỏi branch này. Source Unity, asset source, tooling thử nghiệm, generated staging/evidence và cache nặng của hướng cũ đã được dọn.

## Gate hiện tại

- `python3.12 tools/validate_2d_branch_no_3d.py` phải pass trước khi tiếp tục gameplay 2D.
- Unity batch compile/test phải chạy thật sau các thay đổi source.
- Không sửa frozen surfaces nếu chưa có owner decision riêng.

## Việc tiếp theo

Dựng lát cắt 2D SCN-001/002:

1. Tạo runtime 2D shell cho Cổng Linh Thành và Bia Luyện Khí bằng sprite/layer placeholder gọn.
2. Giữ flow người chơi: di chuyển, focus NPC, thoại, chuyển sân luyện, kích hoạt bia.
3. Capture runtime thật và xem ảnh trước khi claim visual pass.

## Blocker

Chưa có blocker sau cleanup.
