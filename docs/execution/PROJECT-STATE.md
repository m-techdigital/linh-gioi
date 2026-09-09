# PROJECT STATE — Linh Giới Online 2D

## Current branch

`feature/2d`

## Current direction

Linh Giới Online tiếp tục theo North Star Social Action MMORPG, nhưng branch này dùng hướng runtime 2D-first. Linh Thành vẫn là hub xã hội; SCN-001/002 là lát cắt ưu tiên.

## Current source state

Đã dọn source/asset/tool/runtime hook của hướng dựng nhân vật/cảnh cũ. `PlayableWorldController` hiện còn là cầu nối tạm bằng sprite để giữ compile và flow trước khi thay bằng shell 2D sạch hơn.

## Validation spine

- `tools/validate_2d_branch_no_3d.py` bảo vệ branch khỏi việc kéo lại pipeline cũ.
- Unity batch compile/test là gate trước khi báo checkpoint.
- Runtime visual capture cần chạy khi có thay đổi player-visible.

## Next

Dựng lát cắt 2D SCN-001/002 có thể chơi, dễ capture, không mở rộng combat/reward/frozen contracts.
