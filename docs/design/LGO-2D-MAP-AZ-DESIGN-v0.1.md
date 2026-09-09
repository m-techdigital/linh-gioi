# Linh Giới Online — 2D Map A-Z Design v0.1

## Mục tiêu

Branch `feature/2d` dùng lại North Star Social Action MMORPG nhưng chuyển sang **2D Side-Scrolling Social Action MMORPG** theo direction lock `docs/design/LGO-2D-SOCIAL-ACTION-DIRECTION-LOCK-v0.1.md`. Bộ ảnh thiết kế cũ đã bị loại khỏi source; tài liệu này chuyển ba board map mới của owner thành cấu trúc triển khai text để runtime, tooling và art mới có cùng một spine.

## Cấu trúc tổng thể

### World map

World map là màn tổng quan hành trình, nhưng runtime không làm open-world liên tục. Mỗi vùng là node trong Zone Network. Linh Thành nằm ở trung tâm và kết nối các vùng theo progression:

| Vùng | Level | Vai trò gameplay | Ghi chú triển khai 2D |
| --- | ---: | --- | --- |
| Đô Thị | 1-40 | xã hội, công nghệ, tutorial hiện đại | mở sau onboarding để nhấn “con người + công nghệ” |
| Đông Vực | 1-30 | khu tân thủ, quái cơ bản, tutorial | lát cắt đầu tiên: Đông Môn → Training Field → Linh Lâm |
| Linh Thành | 1-100 | hub trung tâm: giao dịch, học viện, bang hội, nhiệm vụ | hub chính, không biến sân luyện thành đích sản phẩm |
| Tây Vực | 30-60 | phụ bản trung cấp, tài nguyên | mở sau khi có quest loop |
| Hải Vực | 40-70 | khám phá, tàu thuyền, tài nguyên biển | dùng layer nước/parallax riêng |
| Cổ Di Tích | 30-60 | dungeon khảo cổ, bí mật thế giới | cần gate nhiệm vụ |
| Linh Sơn | 20-50 | tu luyện, môn phái, thiên nhiên | hướng social + tu tiên |
| Pháp Vực | 50-80 | nghiên cứu, pháp thuật, phụ bản | cần icon học thuật |
| Thiên Vực | 70-100 | endgame thần thoại | chưa mở production sớm |
| Thượng Giới | 80-100 | raid/event cao cấp | chỉ concept dài hạn |
| Âm Giới | 60-100 | world boss, xâm lăng, social action | North Star dài hạn |

### Linh Thành hub

Linh Thành là hub xã hội. Runtime 2D cần map overview trước, sau đó mới chi tiết từng khu:

| Khu | Vai trò | Runtime đầu tiên |
| --- | --- | --- |
| Đông Môn | cửa thành/tutorial | đã có onboarding blockout |
| Quảng Trường | social spawn/sự kiện | hub shell sau Đông Môn |
| Học Viện | kỹ năng/lớp học | mở khi có progression |
| Đền Linh | tín ngưỡng/buff/story | mở khi có quest |
| Khu Dân Cư | nhà ở/hội thoại NPC | social loop |
| Khu Rèn | chế tạo | sau inventory shell |
| Thương Phố | giao dịch | sau shop shell |
| Khu Bang Hội | cộng đồng | sau account/social shell |
| Tây Môn/Bắc Môn/Nam Môn | cổng khu vực | map travel shell |
| Cảng Linh Thuyền | travel/event | sau world-map shell |

### Đông Môn tutorial

Đông Môn là side-scrolling tutorial map đầu tiên. Route target theo kịch bản mới:

1. Người chơi xuất hiện tại Linh Thành – Đông Môn.
2. Đi tới Người Giữ Cổng.
3. Nói chuyện.
4. Đi tới Đá/Bia Luyện.
5. Học di chuyển.
6. Học jump.
7. Học dash.
8. Dùng skill class.
9. Đánh Shadow Slime.
10. Quay lại NPC.
11. Mở Linh Thành.

Prototype hiện có mới giữ chắc bước 1-4. Các bước 5-11 là next runtime slices; riêng skill/combat/reward cần gate riêng trước khi mở rộng sâu.

## Layer map 2D

Mỗi map runtime dùng cùng cấu trúc layer:

| Layer | Nội dung | Quy tắc |
| --- | --- | --- |
| 5 Sky/Fog | mây, trời, ánh sáng | nhẹ, ít chi tiết động |
| 4 Far Background | núi xa, thành phố xa | parallax chậm |
| 3 Mid Background | kiến trúc lớn, rừng | nhận diện vùng |
| 2 Near Background | cây, nhà, cổng | tạo chiều sâu |
| 1 Gameplay | địa hình, platform, nhân vật | collision rõ, đọc được tương tác |
| 0 Foreground | cỏ, hàng rào, vật cản trước | không che HUD/action |


## Parallax/collision pipeline

Map mới đi theo pipeline đã khóa trong scenario spine: Concept Map → Background Far/Mid/Near/Foreground → Collision → Spawn → Lighting → VFX → Runtime. Parallax không chỉ là trang trí; nó quyết định lớp đọc hình, còn collision band quyết định walk/run/jump/dash/combat có nền rõ. Mỗi landmark hoặc route node ở Đông Môn phải biết mình thuộc layer nào, có collision/gap/platform nào liên quan và có được đưa vào visual manifest hay chưa.

## Tileset và thành phần

Các thành phần phải có id riêng để sau này thay art mà không đổi flow:

- Terrain: grass, dirt, stone, cliff, platform.
- Architecture: gate, pagoda, bridge, wall, pillar, signboard.
- Nature: tree, blossom tree, rock, bush, water, waterfall.
- Interaction: ladder, barrel, crate, sign, banner, resource node.
- Effects: spirit portal, fire, water splash, wind grass, aura.
- Markers: NPC, main quest, side quest, teleport, safe zone, treasure, elite, boss.

## Quy tắc triển khai

- Không dùng Meshy và không đưa ảnh cũ vào source.
- Design board mới là reference; runtime ban đầu dùng procedural/blockout có id và metric để kiểm.
- Mỗi map mới phải có: zone id, display name, level band, route nodes, markers, layer budget, smoke target và visual evidence.
- Đông Môn là slice đầu tiên; Linh Thành overview/world map shell đến sau khi route catalog đã có test.
- Không sửa `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json` trong batch map nếu chưa có owner decision.

## Checkpoint mong muốn tiếp theo

1. Runtime catalog có world zones, Linh Thành districts và Đông Môn route nodes.
2. Controller hiển thị minimap/route summary trong Player capture.
3. Visual manifest có map snapshot để future session không làm mò.
4. Player visual capture cho thấy: Cổng Linh Thành, Bia Luyện Khí, lối đi Đông Môn và một map/minimap route rõ ràng.

## Runtime checkpoint — World/Linh Thành Zone Network

Checkpoint 2026-09-09 đưa phần World Map/Linh Thành từ text spec vào runtime spine:

- `WorldMapNetwork: hub=linh-thanh` là snapshot kết nối từ Linh Thành tới Đông Vực, Đô Thị, Linh Sơn, Tây Vực và Âm Giới.
- `LinhThanhHubRuntime` giữ các district đầu cần mở dần: Đông Môn, Quảng Trường, Học Viện, Đền Linh, Khu Dân Cư.
- Minimap overlay trong onboarding hiển thị node World nhỏ `LT ↔ Đông Vực / Âm Giới` để người chơi thấy Đông Môn thuộc mạng vùng lớn hơn, không phải một sân luyện cô lập.

Đây vẫn là runtime spine/prototype; chưa claim World Map production, chưa mở teleport/loading, chưa mở social/economy/guild backend và chưa dùng ảnh source.

## Runtime checkpoint — Linh Thành Hub Shell

Checkpoint 2026-09-09 mở rộng runtime spine từ Đông Môn sang hub shell đầu tiên của Linh Thành:

- `HubShell: linh-thanh` giữ các district có thể mở theo lộ trình: Đông Môn, Quảng Trường, Học Viện và Thương Phố.
- Runtime scene thêm silhouette/marker nhỏ cho Đông Môn → Quảng Trường/Học Viện/Thương Phố để người chơi thấy cổng đang nối vào thành phố xã hội, không phải một màn luyện cô lập.
- Smoke matrix 2D yêu cầu visual manifest giữ `HubShell: linh-thanh`, `district=plaza` và `district=market`.

Đây chưa phải Linh Thành production map, chưa mở shop/giao dịch/bang hội/backend; chỉ là shell player-visible để các batch hub sau có spine chung.

## Runtime checkpoint — Quảng Trường Plaza Shell

Checkpoint 2026-09-09 thêm lát cắt Quảng Trường đầu tiên vào runtime spine:

- `PlazaShell: district=plaza` mô tả social spawn local-safe, event board preview và guild bulletin preview.
- Runtime scene thêm marker nhỏ `LGO 2D Plaza Social Spawn Preview`, `LGO 2D Plaza Event Board Preview`, `LGO 2D Plaza Guild Bulletin Preview`.
- Contract `safe-no-trade-backend` khóa rõ rằng đây chưa mở giao dịch, bang hội, economy hoặc backend mutation.

Mục tiêu của checkpoint là mở đường hub xã hội theo North Star trong Player capture, nhưng vẫn giữ branch ở mức pre-alpha runtime prototype.
