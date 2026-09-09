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


## Đông Môn route label rail

Đông Môn route label rail giữ các mốc tutorial chính bằng chip `01 Cổng → 05 Slime`, giảm chữ trực tiếp trên collision lane để frame runtime đọc nhanh hơn khi chưa có production art. Rail này là UI/readability aid tạm thời cho blockout, không thay thế thiết kế map final.

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

### Runtime unlock presentation — Đông Môn → Quảng Trường

Sau khi người chơi hoàn tất flow Đông Môn và đánh tan Shadow Slime, runtime chỉ mở **presentation local** cho Linh Thành: HUD đổi mục tiêu sang “Mở Linh Thành: Quảng Trường”, scene hiện banner/path về Quảng Trường, manifest ghi `LinhThanhUnlock: unlocked=True | unlock=plaza | source=shadow-slime-complete | route=return-gate->plaza | safe-local-no-teleport`. Bước này giữ đúng thiết kế Zone Network và social hub nhưng chưa mở teleport thật, shop, giao dịch, bang hội hoặc backend xã hội.

### Runtime hub preview — Quảng Trường local-only

Sau unlock Đông Môn, Quảng Trường có lớp runtime preview tách khỏi shell nền: `PlazaHubRuntime: district=plaza | npc=gate-guide | npc=wandering-student | board=event-local-preview | guild-bulletin=locked | social-spawn=local-safe | safe-local-no-backend`. Scene bật cụm NPC/bảng sự kiện/khóa bang hội cùng banner “Mở Linh Thành → Quảng Trường” để người chơi thấy hướng social hub kế tiếp. Đây vẫn là preview local-only, chưa mở teleport thật, shop, giao dịch, bang hội hoặc backend xã hội.
### Runtime interaction checkpoint — Quảng Trường board local preview

Sau unlock Đông Môn, bảng sự kiện Quảng Trường có interaction local-only: `interaction=board-preview-open`. HUD phải chuyển `Khu vực: Quảng Trường`, dialogue mô tả nhiệm vụ cộng đồng ở local preview và inventory panel không được che frame board nếu người chơi chưa mở hành trang. Đây là bước chuẩn bị cho social event board sau này, chưa mở event backend, guild, shop, giao dịch hoặc teleport thật.

### Runtime interaction checkpoint — Quảng Trường NPC local preview

Sau khi unlock Đông Môn, Quảng Trường có Người Giữ Cổng và Thương Nhân preview theo hướng social hub: người chơi đọc chỉ dẫn hub, xem trước thử đồ an toàn và nhận rõ trạng thái chưa mở shop/economy/backend. Speaker của dialogue là dữ liệu riêng để tránh lẫn vai NPC khi mở rộng nhiều nhân vật trong hub.

## Runtime checkpoint — Quảng Trường target selector

Checkpoint 2026-09-10: Quảng Trường chưa phải production map hoàn chỉnh, nhưng đã có micro-flow player-visible an toàn sau Đông Môn. Ba target đầu tiên của hub là `event-board`, `gate-guide`, `merchant-preview`; input dùng `P` để đổi mục tiêu và `E/Enter` để tương tác local-only. Đây là nền để thiết kế layout social hub sau này: mỗi NPC/board/shop/guild node phải có target id, label, vị trí đọc rõ, trạng thái khóa/mở và guard backend rõ ràng trước khi bật chức năng thật.

Quy tắc map tiếp theo: nếu thêm Thương Phố/Bang Hội/Học Viện ở Quảng Trường, runtime trước tiên chỉ tạo shell/preview và evidence, chưa mở shop/economy/guild/backend. Nếu chuyển Đông Môn sang authored Tilemap, vẫn giữ route node đã có: gatekeeper → training-stone → jump-gap → dash-lane → slime-arena → return-gate.

### Plaza social triangle rule

Checkpoint runtime đầu tiên cho Quảng Trường dùng layout `spaced-social-triangle`: NPC giữ cổng đặt bên trái như điểm quay lại tutorial, Bảng Sự Kiện đặt giữa/phía sau như điểm đọc thông tin cộng đồng, Thương Nhân đặt bên phải như preview trang bị local-only. Mọi target hub về sau nên đi theo cùng nguyên tắc: vị trí đọc được trên frame 640x480, có label riêng, selector ring riêng, snapshot token riêng và guard backend rõ ràng.

### Hub transition shell rule

Transition đầu tiên giữa map tutorial và hub là `east-gate -> plaza`. Runtime chỉ được xem là preview khi snapshot có `mode=local-route-preview` và guard `safe-local-no-teleport-backend`; chưa được coi là teleport, streaming hoặc chuyển map server-authoritative. Mọi transition map sau này phải khai báo source, destination, trạng thái unlock, visual cue và guard backend tương tự trước khi mở chức năng thật.

## Runtime map checkpoints hiện tại

- World/Linh Thành/Đông Môn hiện là runtime checkpoint, chưa phải production-complete toàn bộ map.
- Đông Môn đã có route tutorial, collision band, tile chunk flow, tile palette contract, authored runtime source asset, authored chunk placement source, authored detail pass và parallax/landmark spine để kiểm movement/jump/dash/skill/Shadow Slime.
- Linh Thành đã có hub shell, Quảng Trường shell, Học Viện shell, Thương Phố shell, Đền Linh shell, Khu Dân Cư shell, Khu Rèn shell, Khu Bang Hội shell, Cảng Linh Thuyền shell, unlock presentation từ Đông Môn, board/NPC preview, target selector và label readability pass.
- Quảng Trường readability dùng quy tắc `world-label-density=reduced`: trong world chỉ để chip ngắn cho mục tiêu tương tác; text chi tiết đi vào HUD/snapshot/manifest để tránh che nhân vật và platform.
- Next production map nên chuyển tile palette contract thành authored Unity Tilemap asset/palette thật hoặc gate shell local-only, rồi mới nâng tileset/detail. Không crop/dán board, không trang trí ngẫu nhiên và không kéo Meshy/3D trở lại branch 2D.

Học Viện shell dùng `skill-hall=preview-only` và `class-trainer=locked` để giữ đúng lộ trình progression: bản đồ có điểm nhận diện học kỹ năng, nhưng chưa mở skill backend, class progression hoặc UI học kỹ năng khi chưa có task/gate riêng.

Thương Phố shell dùng `vendor-row=preview-only` và `auction-board=locked` để nhận diện khu thương mại trong map mà chưa mở shop, trade, economy hoặc backend khi chưa có gate riêng.

Đền Linh shell dùng `blessing-altar=preview-only` và `story-shrine=locked` để nhận diện trục tín ngưỡng/story trong Linh Thành mà chưa mở buff, story quest backend hoặc progression mới khi chưa có gate riêng.

Khu Dân Cư shell dùng `npc-home-row=preview-only` và `social-chat-node=locked` để mở trục social residential của Linh Thành ở mức map, nhưng chưa mở housing backend, social backend hoặc player-owned home khi chưa có gate riêng.

Khu Rèn shell dùng `anvil-row=preview-only` và `craft-board=locked` để nhận diện crafting district trong Linh Thành mà chưa mở crafting backend, upgrade economy hoặc item mutation khi chưa có gate riêng.

Khu Bang Hội shell dùng `guild-hall=preview-only`, `guild-banner=local-preview` và `notice-board=locked` để bám North Star social MMORPG ở mức map, nhưng chưa mở guild backend, membership, chat hoặc guild progression khi chưa có gate riêng.

Cảng Linh Thuyền shell dùng `spirit-boat=preview-only` và `travel-board=locked` để nhận diện trục travel/event của Linh Thành mà chưa mở travel backend, teleport hoặc world-route execution khi chưa có gate riêng.


Đông Môn authored detail pass dùng `DongMonAuthoredPass` để khóa hướng làm map theo route/collision bands: chi tiết nhỏ phải bám chunk, giữ `detail-density=readable`, `collision-boundaries=from-bands`, `no-random-decoration` và không che người chơi/HUD/combat.


Đông Môn tile palette contract dùng `DongMonTilePalette` để gắn mỗi tile với color role/pattern role/gameplay role: grass/stone/wood/gap/dash/slime có identity riêng, `safe-no-source-image`, và dải swatch runtime chỉ dùng để kiểm role trước khi thay bằng authored Tilemap asset thật.

Đông Môn authored source asset `Resources/LGOMaps/DongMonTilePalette.json` là nguồn runtime đầu tiên cho map 2D: nó giữ palette role dưới dạng data đóng gói trong Player, không dùng ảnh source/3D, và giúp bước sau parse placement/chunk hoặc thay bằng Unity Tilemap asset thật mà không đổi contract gameplay.

Đông Môn authored chunk placement source `Resources/LGOMaps/DongMonChunkPlacement.json` giữ vị trí/count/route-node của các chunk gameplay dưới dạng data đóng gói trong Player. Runtime hiện load placement này để dựng tile strip/paltform/dash/slime arena, giúp bước sau thay renderer procedural bằng Unity Tilemap hoặc sprite atlas mà không đổi route/collision contract.

Đông Môn Unity Tilemap runtime layer dùng `Grid` + `TilemapRenderer` để đặt 16 cell từ authored chunk placement resource. Trong giai đoạn blockout, Tilemap này là underlay mảnh dưới strip hiện có để kiểm pipeline Unity thật và tránh phá readability; bước sau mới thay dần strip/procedural shape bằng tile/atlas production sạch.
### Runtime checkpoint — Quảng Trường layout anchors

Quảng Trường runtime preview sau unlock Đông Môn có `PlazaHubLayout` với 5 anchor local-only: social-spawn trung tâm, event-board upper-mid, gate-guide bên trái, merchant-preview bên phải và guild-locked far-right. Layout này giúp Player capture đọc Quảng Trường như hub xã hội có cấu trúc, nhưng chưa mở event/shop/guild/backend.
### Runtime checkpoint — Quảng Trường anchor detail inspect

Quảng Trường runtime preview có `PlazaHubDetail` cho target đang chọn. Merchant preview ghi rõ `role=starter-gear-preview` và `detail=try-before-shop`, board giữ notice-only, gate-guide giữ route-guide; tất cả local-only để kiểm social hub flow mà chưa mở shop/event/guild/backend.
### Runtime checkpoint — Linh Thành district preview rail

Sau unlock Đông Môn, người chơi có thể xem lộ trình các khu Linh Thành bằng preview rail local-only: `DistrictPreviewRail: unlocked=True | selected=academy | route=plaza->academy | controls=M select-district`. Rail này nối Quảng Trường tới Học Viện, Thương Phố, Đền Linh, Khu Rèn, Khu Bang Hội và Cảng Linh Thuyền để giữ hướng social hub/map A-Z rõ hơn mà chưa mở teleport, shop, skill, crafting, guild hoặc travel backend.
### Runtime checkpoint — Linh Thành district detail/cycle

District preview rail giờ có detail snapshot riêng cho từng khu. Capture runtime chọn Học Viện rồi chuyển sang Thương Phố để chứng minh rail là flow cycle chứ không phải một marker tĩnh: `DistrictDetail: selected=market | role=starter-commerce-preview | detail=vendor-row-only | safe-no-trade-backend | safe-no-economy-backend`. Thương Phố vẫn là preview local-only, chưa mở shop, trade, economy hoặc backend.
### Runtime checkpoint — Linh Thành Đền Linh district preview

Checkpoint 2026-09-10 thêm frame runtime riêng cho Đền Linh trong district preview rail. Sau unlock Đông Môn, capture Player chuyển rail Học Viện → Thương Phố → Đền Linh và manifest giữ `DistrictPreviewRail: selected=spirit-temple | label=Đền Linh | route=plaza->spirit-temple` cùng `DistrictDetail: selected=spirit-temple | role=story-blessing-preview | detail=altar-local-only | next=quest-buff-gate`. Đây là preview local-only để chứng minh hướng map/story trong Linh Thành; chưa mở buff, story quest backend, teleport hoặc district backend.
### Runtime checkpoint — Linh Thành district rail coverage

Checkpoint 2026-09-10 mở rộng capture district preview rail tới các khu sau trong Linh Thành: Khu Rèn, Khu Bang Hội và Cảng Linh Thuyền. Player capture giờ chứng minh rail local-only đi qua Học Viện → Thương Phố → Đền Linh → Khu Rèn → Khu Bang Hội → Cảng Linh Thuyền; manifest cuối giữ `DistrictPreviewRail: selected=harbor | label=Cảng Linh Thuyền | route=plaza->harbor` và `DistrictDetail: role=travel-preview | detail=spirit-boat-locked | next=world-route-gate`. Đây là coverage tương tác bản đồ, chưa mở crafting, guild, travel, teleport hoặc backend district.
### Runtime checkpoint — Linh Thành district rail readability

Checkpoint 2026-09-10 cải thiện độ đọc của district preview rail: callout `Map: <khu> / local-only` và backplate giờ đi theo node đang chọn thay vì nằm cố định xa ở đáy màn. Manifest có `DistrictRailReadability: mode=selected-node-callout | label-follows-selected=True | backplate=follows-selected | callout-size=readable | avoids-hud-overlap`, giúp các frame Khu Rèn, Khu Bang Hội và Cảng Linh Thuyền đọc nhanh hơn trong Player capture. Đây chỉ là polish UX/readability cho preview local-only.
