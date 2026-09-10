# Linh Giới Online — Selected 2D Source Catalog v1

Ngày khóa: 2026-09-10

Source pack local: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1`

Manifest SHA-256: `0145e96148a73c423be823ad3c1b447c4fda3470338c5e4836010abab22bca53`

## Quyền ưu tiên

1. Scope chữ owner ngày 2026-09-10 là nguồn sản phẩm cao nhất.
2. Mười ảnh trong `map-01a-cong-dong-lam/01..10` là nguồn hình canonical cho Map 01A.
3. Hai ảnh trong `context/` chỉ dùng kiểm độ đồng nhất và coverage.
4. Runtime blockout/primitive hiện có chỉ giữ contract kỹ thuật, không còn là nguồn mỹ thuật.

Khi chữ trong ảnh mâu thuẫn với scope owner, scope owner thắng. Vì vậy Map 01A là Cổng Đông Lâm Lv1–3, mở Suối Thanh Minh; không boss, không mở Linh Thành, không skill thứ hai và không monetization.

## Mười nguồn Map 01A canonical

| File | Dùng để khóa |
|---|---|
| `01-gameplay-screen-16x9.png` | Camera side-view, tỷ lệ nhân vật/cảnh, vùng HUD |
| `02-full-horizontal-level-strip.png` | Thứ tự 10 khu từ spawn đến portal Suối Thanh Minh |
| `03-zone-npc-quest-placement.png` | Tuyến NPC, quest chính/phụ và khu chức năng |
| `04-npc-character-and-placement.png` | Ngoại hình và vai trò 6 NPC chính |
| `05-dialogue-and-quest-flow.png` | Nhịp thoại, quest state và reward |
| `06-enemy-spawn-loot-resource.png` | 4 quái Lv1–3, spawn edge, loot/resource |
| `07-environment-vegetation-ambient.png` | Cây cỏ, fauna và nhịp cảnh |
| `08-tileset-props-architecture.png` | Terrain, cổng, nhà, cầu, props, chest |
| `09-parallax-lighting-vfx.png` | Palette, ánh sáng và lớp chiều sâu |
| `10-collision-trigger-ui-safe-area.png` | Collision, trigger, portal và UI safe area |

Runtime không import nguyên board. Mỗi asset phải được crop/redraw/extract thành layer riêng, lưu nguồn và SHA trong provenance. Layer map khóa theo owner: L0 Sky, L1 Clouds, L2 Far mountains, L3 Linh Thành silhouette, L4 Distant trees, L5 Waterfalls, L6 Mid architecture, L7 Gameplay terrain, L8 Interactive props, L9 Characters/NPC/enemies, L10 Foreground vegetation, L11 VFX/particles.

## Nguồn class canonical

`classes-lv001-030/common` giữ 7 sheet cho tỷ lệ nam/nữ, chi tiết đầu–tóc–tay–chân, module/anchor, rig/silhouette và palette. Mỗi thư mục `vo`, `kiem`, `phap`, `co`, `linh` giữ 6 sheet: identity/progression, profile nam/nữ, turnaround nam, turnaround nữ, equipment/weapon và skill/motion/VFX.

Các sheet có chữ 3D hoặc hiển thị tới Lv100 chỉ cung cấp tỷ lệ, silhouette, màu, ownership trang bị và motion intent. Pipeline runtime vẫn là cutout/sprite 2D. Phase hiện tại chỉ dùng phần Lv1–30.

## Công sức class đang giữ lại

`class-work-in-progress/vo-lv001` chứa 12 file đã được lấy nguyên từ batch trước:

- base nam/nữ đã căn cùng ground line; nữ dịch `+29 px`, không resize;
- mask plan có polygon và anchor;
- ba mảnh alpha `inner_top`, `arm_guard`, `main_weapon`;
- atlas thử nghiệm, composite full-equipped và ảnh bật/tắt từng slot;
- report xác nhận trim/atlas round-trip lossless và các validator đã chạy.

Phần này không làm lại. Nó là đầu vào của vertical slice Võ sau khi Map 01A đạt gate. Giới hạn còn thật: ba mảnh chưa được duyệt runtime, tiếp xúc cổ tay còn lỗi, base nam còn alpha edge, thiếu bảy slot, thiếu nữ và chưa kiểm motion/deformation.

## Trình tự thực thi

1. Khóa source/provenance và giữ lại WIP class — hoàn tất qua catalog này.
2. Dựng Cổng Đông Lâm Map 01A theo 10 nguồn canonical: đầu tiên khóa world strip, 12 layer, zone/collision/trigger và một màn gameplay 16:9.
3. Tách/chuẩn hóa asset cảnh theo atlas, ghép Player, chạy quest Lv1–3 và capture Player thật. Không mở Map 01B trước portal gate.
4. Sau khi ảnh Player Map 01A đạt review, dùng WIP Võ để hoàn thiện một class mẫu Lv1–30: đủ base nam/nữ, 10 slot, thay đồ, anchor/pivot/sort, idle/walk/run/jump/basic attack và `Liên Quyền`.
5. Chỉ nhân pipeline sang Kiếm/Pháp/Cơ/Linh Lv1–30 sau khi Võ đạt runtime gate.

## Non-claims

- Source pack là reference và WIP, không tự động là production art.
- Ba mảnh Võ đã tách chưa được phép import runtime.
- Catalog không mở gameplay, economy, backend, Linh Thành hoặc map thứ hai.
- Không dùng Meshy/3D và không thay đổi frozen surfaces.
