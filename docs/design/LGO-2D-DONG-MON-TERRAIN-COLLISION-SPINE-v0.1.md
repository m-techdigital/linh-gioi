# Linh Giới Online — Đông Môn Terrain Collision Spine v0.1

Ngày cập nhật: 2026-09-09. Batch này khóa spine địa hình/collision cho map tutorial Đông Môn theo `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md`, trước khi thay bằng tileset/art final.

## Mục tiêu

Đông Môn phải đọc được như side-scrolling map thật: có nền đi bộ, gap để học jump, lane để học dash, platform quanh Bia Luyện Khí và vùng đọc combat cho Shadow Slime. Đây chưa phải hệ vật lý production hay Tilemap Collider hoàn chỉnh; đây là contract runtime/evidence để các bước art, tilemap và collision sau không làm mò.

## Collision bands hiện tại

| Id | Vai trò | Dùng cho |
| --- | --- | --- |
| `ground-main` | Main ground | walk/run safe lane trên tuyến Đông Môn |
| `training-platform` | Bia platform | vùng tương tác Bia/Đá Luyện Khí |
| `jump-gap` | Jump gap | cue học jump và test khoảng trống |
| `dash-lane` | Dash lane | cue học dash ngang, tránh đặt prop che đường đọc hình |
| `slime-arena` | Shadow Slime arena | vùng combat dừng trước rừng ngoài thành |

## Runtime/evidence rules

- Controller phải expose `RuntimeTerrainCollisionSnapshot` và visual manifest phải chứa `runtimeTerrainCollisionSnapshot`.
- Runtime cue `JUMP GAP` và `DASH LANE` chỉ là guide blockout; không được che HUD, Người Giữ Cổng, player, Bia Luyện Khí hoặc Shadow Slime.
- Khi chuyển sang tileset thật, giữ id/band trước rồi thay sprite/collider sau để smoke và visual capture vẫn so được hành vi.
- Không mở rộng HP/loot/economy/server-authoritative combat trong batch terrain này.
