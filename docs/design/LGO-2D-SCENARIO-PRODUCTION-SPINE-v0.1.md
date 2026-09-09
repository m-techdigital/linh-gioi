# Linh Giới Online — 2D Scenario Production Spine v0.1

Ngày cập nhật: 2026-09-09. Tài liệu này chuyển kịch bản game mới của owner thành nguồn định hướng chính trước mọi batch design/runtime trên branch `feature/2d`.

## 1. Định hướng khóa

Linh Giới Online đi theo mô hình **2D Side-Scrolling Social Action MMORPG**: nhân vật anime-stylized độ phân giải cao, **HD 2D anime / illustrated character**, không pixel-art, camera side view, bản đồ nhiều lớp parallax, combat nhanh có jump/dash/skill, và hub xã hội đông người.

Điểm quan trọng nhất: **không làm lại Linh Giới Online từ đầu**. Ta giữ thế giới, class, nhân vật, progression, câu chuyện, backend Java, account, character, GameData và networking; chỉ đổi cách hiện thực hóa client/gameplay presentation từ pipeline 3D sang pipeline 2D phù hợp hơn.

Ba trụ cột sản phẩm:

| Trụ cột | Nội dung production |
| --- | --- |
| Social MMORPG | Thành phố, bạn bè, bang hội, giao dịch, nhà/thời trang, hoạt động cộng đồng |
| Action | Combo, dash, jump, skill, boss, PvP, world event |
| Progression | Level, skill, trang bị, class, linh thú, ngoại hình |

## 2. Chuyển đổi 3D sang 2D

Không bỏ concept class/trang phục đã làm; chỉ đổi ngôn ngữ triển khai:

| 3D cũ | 2D mới |
| --- | --- |
| Mesh body | Base sprite / cutout body |
| 3D clothing mesh | Layer trang phục 2D |
| Bone rig 3D | Skeleton 2D |
| Socket 3D | Anchor point |
| PBR material | Sprite texture + shader 2D |
| Model weapon | Sprite weapon |
| 3D VFX | Particle/VFX 2D |
| LOD model | Sprite atlas / resolution tier |
| Turnaround 3D | Character sprite sheet |
| Cloth/hair physics | Bone / spring 2D |

Dừng ngay việc đầu tư vào character sculpt, retopology body, 3D outfit model, PBR character material, 3D armor sockets, 3D hair, 3D cloth và 3D character LOD cho player character chính. Các bảng ảnh class/trang phục cũ chỉ còn là concept/reference để vẽ lại thành layer sprite sạch, không crop/dán board vào runtime.

## 3. Camera, map và production feel

Runtime dùng side view có chiều sâu:

```text
Layer 5 — Sky/Fog: trời, mây, ánh sáng, sương
Layer 4 — Far Background: núi/thành phố rất xa
Layer 3 — Mid Background: kiến trúc lớn, rừng, thác
Layer 2 — Near Background: nhà, cây, cầu, tháp
Layer 1 — Gameplay Plane: player, NPC, monster, boss, ground/platform/collision
Layer 0 — Foreground: cỏ, đèn, lá, particle, sương nhẹ
```

Không làm một màn phẳng kiểu web cũ. Mỗi map production phải đi dần tới foreground che nhẹ nhân vật, background nhiều lớp, ánh sáng 2D, fog, particle, nước/reflection giả lập, dynamic weather, ngày/đêm, NPC chuyển động và linh khí bay trong môi trường.

Map pipeline chuẩn: **Concept Map → Background Far/Mid/Near/Foreground → Collision → Spawn → Lighting → VFX → Runtime**. Mỗi map cần zone id, display name, level band, route node, collision band, spawn NPC/Monster/Portal/Interactive, visual marker, smoke target và evidence runtime.

## 4. Base character và trang bị 2D

Chỉ có hai base body chính:

- Male Base
- Female Base

Class đến từ trang bị/layer, không tạo 10 body riêng cho 5 class. Khi tháo toàn bộ equipment, nhân vật vẫn phải có basic hair, đồ mặc định xám, shorts và socks; không một trang bị class nào được trở thành bắt buộc để che lỗi body.

Layer chuẩn của nhân vật:

```text
CHARACTER_ROOT
├── Shadow
├── Back FX
├── Back Accessory
├── Hair Back
├── Body
│   ├── Head
│   ├── Torso
│   ├── UpperArm L/R
│   ├── Forearm L/R
│   ├── Hand L/R
│   ├── Thigh L/R
│   ├── Calf L/R
│   └── Foot L/R
├── Underwear
├── Pants / Skirt
├── Inner Shirt
├── Outer Shirt
├── Waist
├── Shoulder
├── Gloves
├── Boots
├── Hair Front
├── Head Accessory
├── Weapon
├── Offhand
├── Pet / Drone / Spirit
├── Front FX
└── UI Anchor
```

Mỗi item chuẩn hóa: Icon, Preview, Front Sprite, Back Sprite, Side/Secondary Sprite nếu cần, Bone Mapping, Anchor, Sorting Layer, Mask và VFX. Trang bị phải mix được, ví dụ quần Kiếm + áo Pháp + găng Võ + giày Cơ + tóc Linh vẫn render đúng trên cùng skeleton/base.

Asset pipeline chuẩn: **Concept → Character Sheet → Module Separation → Clean 2D Sprite → Rig → Animation → Sprite Atlas → Unity → Runtime Test**.

## 5. Animation foundation

Dùng 2D skeletal animation + sprite swapping để một animation dùng được cho nhiều bộ đồ.

Locomotion chung cho mọi class: Idle, Walk, Run, Jump Start, Jump Loop, Fall, Land, Dash, Turn, Interact, Sit, Emote, Hit, Knockback, Death, Respawn. Combat animation tách theo class.

## 6. 5 class trong gameplay 2D

| Class | Identity | Đọc hình/VFX chính |
| --- | --- | --- |
| Võ | Áp sát, combo, phá giáp, phản đòn | Cam/orange/gold, dust, impact, shockwave |
| Kiếm | Tốc độ, kiếm thuật, counter, mobility | Afterimage, sword trail, sword aura, precise hit, air combo |
| Pháp | Ranged, elemental, AoE, control | Fire, ice, lightning, spirit, barrier, gravity |
| Cơ | Ranged weapon, machinery, deployables | Turret, mine, drone, cannon, rail shot |
| Linh | Summon, support, control, purification | Heal, buff, shield, summon, bind/debuff |

Progression hình ảnh Lv1 → Lv100 tăng qua sprite layer, weapon sprite, accessory, secondary animation, shader và VFX. Lv1 phải clean, ít layer, ít VFX; Lv20 rõ class hơn; Lv50 silhouette mạnh; Lv80 có aura/accessory lớn; Lv100 identity tối đa.

## 7. Zone Network và chapter

Không làm open-world liên tục. Dùng **Zone Network**, mỗi node là một hoặc vài map 2D:

```text
[THƯỢNG GIỚI]
      │
 ┌────┴────┐
[LINH SƠN] [PHÁP VỰC]
      \     /
     LINH THÀNH
 ┌────┼────┐
KHU DÂN CƯ  THƯƠNG PHỐ  HỌC VIỆN
      │
   ĐÔNG MÔN → TRAINING FIELD → LINH LÂM → CỔ DI TÍCH → ÂM GIỚI
```

**Opening cinematic** mục tiêu: màn hình đen, tiếng chuông, khe nứt tím trên bầu trời Linh Thành, linh phù thành phố sáng lên, voice-over “Hai thế giới từng tồn tại cạnh nhau, nhưng chưa bao giờ thực sự tách biệt.” Camera lướt qua Linh Thành, NPC, Player, Linh thú, Cơ giới, Pháp trận; sau câu “Cho đến ngày những cánh cửa bắt đầu mở.” portal Âm Giới xuất hiện và cut vào Đông Môn.

**Tutorial chính thức** bắt đầu tại Linh Thành – Đông Môn: đi tới Người Giữ Cổng → nói chuyện → đi tới Đá/Bia Luyện → học di chuyển → học jump → học dash → dùng skill class → đánh Shadow Slime → quay lại NPC → mở Linh Thành.

### Chapter 1 — Vết Nứt Đông Môn

Người Giữ Cổng cảnh báo linh khí ngoài thành dao động. Player đi Linh Thành → Đông Môn → Linh Lâm, gặp Shadow Slime, Corrupted Spirit và Lost Merchant. Cuối chapter là Mini Boss Linh Thú Biến Dị và mảnh Âm Giới Fragment.

### Chapter 2 — Những Cánh Cổng Không Thuộc Về Thế Giới Này

Portal nhỏ bắt đầu xuất hiện. Các class nhìn cùng vấn đề theo cách khác nhau: Võ bảo vệ dân cư, Kiếm truy tìm kẻ đứng sau, Pháp nghiên cứu cấu trúc portal, Cơ đo/phân tích năng lượng, Linh nghe tiếng gọi từ phía bên kia.

### Chapter 3 — Âm Giới Xâm Lăng

World event lớn: portal Âm Giới xuất hiện ở nhiều zone, người chơi hợp lực trong channel/instance, tiến tới World Boss và server reward. Contribution dài hạn phải hỗ trợ nhiều vai trò, không chỉ raw DPS.

## 8. Core loop, session 20 phút và UI

Core loop: Login → Character → Linh Thành → Social/Adventure → Reward → Upgrade → New Content. Một session 20 phút mẫu: vào Linh Thành, chat với bạn, nhận Daily, đi Linh Lâm, combat, mini boss, nhận item, về thành, đổi trang phục, gặp guild, logout. Game không được trượt thành “chỉ đánh quái”.

HUD gameplay 2D MMO cần avatar/HP/MP, minimap, quest tracker, joystick/skill buttons trên mobile, và phím PC như WASD, Space, Shift, 1/2/3/4, Q/E/R. UI phải phục vụ action nhanh và social hub, không bê bảng concept dày chữ vào HUD runtime.

## 9. Roadmap production

| Mã | Nội dung |
| --- | --- |
| 2D-00 | Direction Lock |
| 2D-01 | Male/Female Base Character |
| 2D-02 | 2D Character Modular Runtime |
| 2D-03 | Võ Lv1 Full Character |
| 2D-04 | Kiếm Lv1 |
| 2D-05 | Pháp Lv1 |
| 2D-06 | Cơ Lv1 |
| 2D-07 | Linh Lv1 |
| 2D-08 | Animation Foundation |
| 2D-09 | Linh Thành Map |
| 2D-10 | Gate Keeper Tutorial |
| 2D-11 | Shadow Slime Combat |
| 2D-12 | Vertical Slice |

Vertical Slice cuối chuỗi phải đạt: Login → Character Select → Enter World → Linh Thành 2D → Walk/Run/Jump/Dash → Gate Keeper → Dialogue → Training → Use Class Skill → Shadow Slime → Reward → Return to Town. Khi đoạn này đẹp và chơi được, mới nhân rộng nội dung.

## 10. Guardrails vận hành

- Design mới đọc tài liệu này, `docs/02-GDD.md`, direction lock, scenario bible và map A-Z trước khi triển khai.
- Chữ trong kịch bản/GDD thắng ảnh board nếu có xung đột scope. Ảnh board/concept chỉ là reference mỹ thuật cho sprite/module mới.
- Không quay lại Meshy/3D cho thiết kế 2D.
- Không thêm source image cũ vào branch; asset mới phải qua clean sprite/module/atlas/runtime test.
- Không mở rộng guild/chat/market/server-authoritative combat/economy nếu chưa có gate riêng.
- Mọi batch player-visible phải có build/test/runtime evidence thật; screenshot runtime phải được xem bằng mắt trước khi claim visual.
