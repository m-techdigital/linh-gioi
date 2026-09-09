# Linh Giới Online — 2D Đông Môn Landmark Pass v0.1

Ngày cập nhật: 2026-09-09
Branch: `feature/2d`

## Mục tiêu

Nâng blockout Đông Môn từ sân luyện phẳng thành map tutorial có mốc đọc được theo board/GDD 2D: Cổng Linh Thành, Bia Luyện Khí, Cầu Gỗ, Thác Nước, Sóng Linh và Rừng Ngoại Thành. Scope vẫn là procedural runtime để kiểm flow; chưa phải sprite atlas final.

## Landmark catalog

`TwoDMapDesignCatalog.DongMonLandmarks` khóa danh sách landmark Chapter 1 `Vết Nứt Đông Môn`:

- `gate-landmark`: Cổng Linh Thành, layer 2, node spawn/gatekeeper.
- `training-stone-landmark`: Bia Luyện Khí, layer 1, node training-stone.
- `wood-bridge`: Cầu Gỗ, layer 2, node jump.
- `spirit-waterfall`: Thác Nước, layer 3, node dash.
- `song-linh`: Sóng Linh, layer 0, node class-skill.
- `outer-forest`: Rừng Ngoại Thành, layer 3, node shadow-slime.

## Runtime rule

Landmark chỉ hỗ trợ đọc route/parallax/không khí, không che gameplay plane hoặc HUD. Snapshot `RuntimeMapSnapshot` phải chứa `Landmarks: Chapter 1` để future session không làm map mò hoặc bỏ mất mốc tutorial.
