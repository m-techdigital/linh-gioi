# Linh Giới Online — 2D Production Workflow Research v0.1

Ngày cập nhật: 2026-09-10
Phạm vi: nhánh `feature/2d`, bỏ 3D/Meshy, ưu tiên map/NPC/trang bị 2D có thể lên runtime nhanh nhưng không phá North Star Social Action MMORPG.

## Kết luận vận hành

Không tiếp tục dựng art runtime bằng cách chỉnh từng khối thủ công. Từ batch tiếp theo, pipeline chuẩn là:

1. Chốt module trước khi polish: `tile`, `prop`, `npc`, `equipment`, `vfx`, `ui-chip`.
2. Mỗi module có source data riêng: id, slot/layer, anchor, palette, tags an toàn, level/route/context.
3. Runtime chỉ đọc data và render layer/slot; không hardcode từng mẫu mới trong controller.
4. Visual checkpoint phải capture Player thật bằng tool 2D chuẩn, rồi review ảnh trước khi claim pass.
5. Art blockout chỉ được dùng để kiểm layout/flow. Khi đã cần “giống ảnh”, chuyển sang spritesheet/atlas/PSD-layer workflow, không kéo dài việc ghép hình bằng rectangle primitive.

## Bài học từ nguồn ngoài

- Unity khuyến nghị Tilemap để giảm scene size, serialization overhead, renderer overhead và batching cost cho game 2D. Vì vậy Đông Môn/Linh Thành nên đi theo chunk tilemap + prop layer, không spawn quá nhiều GameObject riêng lẻ cho mỗi mảng nền.
- Sprite Atlas nên gom các sprite thường xuất hiện cùng scene vào cùng atlas, đồng thời tách atlas nhỏ theo usage chung. Với Linh Giới: `dong-mon-environment`, `linh-thanh-common`, `npc-gatekeeper`, `starter-equipment`, `ui-icons` là các cụm hợp lý.
- 2D paper-doll/avatar system là mô hình phù hợp cho MMO có đổi tóc/mắt/trang bị: mỗi bộ phận hoặc trang bị là layer/sprite riêng, runtime xếp theo slot thay vì vẽ sẵn mọi tổ hợp. Điều này giảm số asset cần vẽ và hỗ trợ mix-match quần/áo/phụ kiện.
- Spine skin/attachment workflow cho thấy cách production tốt: có template skin placeholders trước, sau đó gắn attachment cho từng skin. Nếu sau này dùng skeletal 2D, Linh Giới nên map slot hiện tại sang placeholder chuẩn như `hair_front`, `eyes`, `torso`, `pants`, `boots`, `weapon`, `back`, `pet`.
- Unity PSD Importer có character rig mode tạo Prefab từ layer của file nguồn. Khi bắt đầu làm art thật, nên yêu cầu output dạng layered PSD/PSB hoặc spritesheet tách layer để import có hệ thống, thay vì chỉ dùng ảnh concept phẳng.
- MapleStory là ví dụ đúng hướng sản phẩm: 2D side-scrolling MMORPG, world nhiều vùng, NPC/item/quest/social/trade/guild là vòng đời chính. Linh Giới nên ưu tiên map hub + NPC + item/equipment visible sớm trước khi mở hệ backend lớn.

## Quy tắc áp dụng cho Linh Giới từ batch sau

### Map

- Dùng 5 lớp cố định: sky/fog, far background, mid background, gameplay tilemap, foreground.
- Không thêm chi tiết random. Mỗi prop phải có id, anchor, layer, role và route context.
- Đông Môn cần ưu tiên: cổng thành, bia luyện khí, cầu gỗ, thác/nước linh, rừng ngoài thành, slime arena.
- Linh Thành cần ưu tiên: quảng trường, học viện, thương phố, đền linh, khu rèn, bang hội, cảng.

### NPC

- NPC phải là data-driven sprite source, không hardcode riêng từng NPC.
- NPC blockout hợp lệ chỉ để giữ flow; khi polish, mỗi NPC cần silhouette rõ: đầu/tóc/mũ, thân áo, tay/đạo cụ, bóng, phụ kiện nhận diện.
- Người Giữ Cổng batch hiện tại đã có `DongMonNpcSprites.json`; bước tiếp theo là thay màu/hình bằng spritesheet/atlas thật cho 13 part hiện có.

### Trang bị/nhân vật

- Giữ paper-doll slots: base, eyes, hair_back, hair_front, torso/top, pants, waist, gloves, boots, weapon, back, accessory, pet.
- Mỗi item có icon hành trang và layer mặc trên người. Không tạo item chỉ có icon mà không có runtime preview.
- Nam/nữ dùng chung quy chuẩn slot/anchor, nhưng asset riêng cho form và silhouette.

### Kiểm thử/evidence

- Player visual 2D dùng `tools/capture_lgo_2d_onboarding_visual.py`. Lý do: macOS Player cần launch từ `Contents/MacOS`; chạy trực tiếp từ repo root có thể init engine rồi thoát trước khi `GameBootstrap` ghi manifest.
- Bắt buộc qua: EditMode, 2D smoke, macOS Player build, 2D visual capture, `lgo_runtime_smoke_matrix --phase two-d`, `lgo_visual_evidence_matrix --verify-current`, no-3D/no-source-image guards.
- Không dùng full M4 visual runtime khi chỉ thay map/NPC 2D onboarding, trừ khi thay đổi chạm login/session/combat full route. Điều này tránh build cao không cần thiết.

## Mốc thời gian thực tế để đạt “giống ảnh”

- 0.5–1 ngày: placeholder đẹp hơn, silhouette NPC/map rõ hơn, vẫn là blockout.
- 3–5 ngày: một map Đồng Môn playable có tileset/prop/NPC basic đẹp tương đối, đủ duyệt hướng.
- 1–2 tuần: vertical slice gần board concept cho Đồng Môn: parallax, tileset coherent, NPC sprite tốt, UI map/minimap ổn, một vài animation/vfx.
- Nhiều tuần đến vài tháng: toàn bộ world map + Linh Thành + nhiều vùng có chất lượng đồng đều như concept.

## Nguồn đã tham khảo

- Unity — Optimize performance of 2D games with Unity Tilemap: https://unity.com/how-to/optimize-performance-2d-games-unity-tilemap
- Unity Manual — Sprite Atlas workflow: https://docs.unity3d.com/2022.2/Documentation/Manual/SpriteAtlasWorkflow.html
- Unity Blog — 2D Tilemap asset workflow from image to level: https://unity.com/blog/games/2d-tilemap-asset-workflow-from-image-to-level
- Unity Documentation — 2D PSD Importer summary: https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@3.0/manual/index.html
- Spine — Mix and Match / skins and attachments: https://en.esotericsoftware.com/spine-unity-mix-and-match
- RPG Maker forum — paper-doll explanation as separate equipment sprite sheets overlaid on character: https://forums.rpgmakerweb.com/index.php?threads/paperdolls-is-it-possible.46337/
- MapleStory overview as 2D side-scrolling MMORPG reference: https://en.wikipedia.org/wiki/MapleStory
