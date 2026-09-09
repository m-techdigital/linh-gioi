# LGO 2D — Đông Môn Tilemap Runtime Spine v0.1

Ngày: 2026-09-09
Branch: `feature/2d`
Scope: Chapter 1 `Vết Nứt Đông Môn`, runtime onboarding slice.

## Mục tiêu

Batch này chuyển phần collision/terrain Đông Môn từ cue rời sang một spine tilemap procedural có ID rõ ràng để các batch sau có thể thay bằng authored tileset/sprite atlas mà không đổi flow gameplay. Đây là lớp runtime kỹ thuật có thể nhìn thấy trong Player capture, không phải art final và không dùng lại/crop board thiết kế.

## Tile ID khóa cho Đông Môn

- `tile_ground_grass`: mép đất cỏ và fringe foreground cho vùng đi bộ chính.
- `tile_ground_stone`: mặt đường đá linh lực cho path chính.
- `tile_platform_wood`: platform/cầu gỗ quanh bài học jump.
- `tile_gap_marker`: khoảng trống reserved cho bài học jump, không phải tile đi bộ.
- `tile_dash_lane`: mặt lane đọc rõ cho bài học dash.
- `tile_slime_arena`: footing combat trước rìa rừng, dùng cho Shadow Slime micro-slice.

Mỗi tile definition bám một `CollisionBandId` hiện có: `ground-main`, `training-platform`, `jump-gap`, `dash-lane`, `slime-arena`. Quy tắc này giữ map work đi theo form chung thay vì chắp vá từng sprite.

## Runtime evidence contract

Runtime controller expose `RuntimeTilemapSnapshot`; visual manifest ghi `runtimeTilemapSnapshot` theo dạng:

`Chapter 1 Tilemap: Đông Môn | tile_ground_grass@ground-main/L1 | ...`

Player visual capture phải cho thấy ground strip/platform/gap/dash/slime arena mà không che HUD, player, NPC, Shadow Slime hoặc panel inventory input. Nếu chuyển sang Unity Tilemap/authoring asset thật ở batch sau, field snapshot vẫn giữ để validator và handoff không mất dấu tile identity.

## Giới hạn

- Chưa tạo Unity Tilemap asset production.
- Chưa ingest ảnh/source art; branch vẫn phải pass `tools/validate_2d_branch_no_source_images.py`.
- Chưa mở biome/map mới ngoài Đông Môn.
- Chưa sửa frozen surfaces.
## Chunk flow runtime

Batch tiếp theo gom tile lẻ thành các chunk theo tuyến tutorial:

- `chunk_gate_entry`: vùng spawn/cổng, dùng `tile_ground_grass`.
- `chunk_training_stone`: đường đá tới Bia Luyện Khí, dùng `tile_ground_stone`.
- `chunk_jump_bridge`: platform/cầu gỗ quanh bài học jump, dùng `tile_platform_wood`.
- `chunk_dash_lane`: lane dash đọc rõ trước combat, dùng `tile_dash_lane`.
- `chunk_slime_arena`: vùng chân combat cho Shadow Slime, dùng `tile_slime_arena`.

Snapshot `ChunkFlow` phải bám route node, ví dụ `chunk_gate_entry@spawnx4 -> ... -> chunk_slime_arena@shadow-slimex2`. Renderer procedural dùng cùng chunk ID trong tên sprite để khi chuyển sang authored tilemap có thể map lại từng chunk mà không đổi flow gameplay.
