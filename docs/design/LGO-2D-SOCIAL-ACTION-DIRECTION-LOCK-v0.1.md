# Linh Giới Online — 2D Social Action Direction Lock v0.1

## Quyết định hướng mới

Linh Giới Online chuyển sang mô hình **2D Side-Scrolling Social Action MMORPG**. Game giữ thế giới, class, progression, câu chuyện, backend Java, account, character, GameData và networking; phần thay đổi lớn nằm ở Unity client, art pipeline, animation, map và combat presentation.

Hướng mỹ thuật khóa: **HD 2D anime / illustrated character**, skeletal animation, sprite swapping, môi trường nhiều lớp parallax, VFX điện ảnh. Không dùng pixel-art làm đích sản phẩm và không quay lại pipeline nhân vật 3D/Meshy.

## Ba trụ cột sản phẩm

| Trụ cột | Nội dung |
| --- | --- |
| Social MMORPG | thành phố, bạn bè, bang hội, giao dịch, nhà/thời trang, hoạt động cộng đồng |
| Action | combo, dash, jump, skill, boss, PvP |
| Progression | level, skill, trang bị, class, linh thú, ngoại hình |

North Star không đổi: Linh Thành là trung tâm xã hội; Âm Giới Xâm Lăng là hướng social action dài hạn.

## Chuyển đổi 3D sang 2D

| Hướng cũ 3D | Hướng mới 2D |
| --- | --- |
| Mesh body | base sprite / cutout body |
| 3D clothing mesh | layer trang phục 2D |
| Bone rig 3D | skeleton 2D |
| Socket 3D | anchor point |
| PBR material | sprite texture + shader 2D |
| Model weapon | sprite weapon |
| 3D VFX | particle/VFX 2D |
| LOD model | sprite atlas / resolution tier |
| Turnaround 3D | character sprite sheet |
| Cloth physics | bone/spring 2D |
| Hair physics | hair bones 2D |

Các concept class cũ còn giá trị như reference thiết kế, nhưng không còn là runtime asset 3D.

## Camera và map presentation

Game dùng side view với nhiều lớp:

1. Sky/Fog: trời, mây, ánh sáng, sương.
2. Far Background: núi xa, thành phố xa.
3. Mid Background: kiến trúc lớn, rừng, thác.
4. Near Background: nhà, cây, cầu, tháp.
5. Gameplay Plane: player, NPC, monster, boss, ground/platform/collision.
6. Foreground: cỏ, đèn, lá, particle, sương nhẹ.

Không làm màn hình phẳng kiểu web cũ. Runtime cần dần có foreground che nhẹ, lighting 2D, fog, particle, nước/reflection giả, ngày/đêm, NPC chuyển động và linh khí bay.

## Base character 2D

Chỉ có hai base body chính:

- Male Base
- Female Base

Class đến từ trang bị/layer, không tạo 10 body riêng cho 5 class.

Layer chuẩn:

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

Khi tháo toàn bộ equipment, nhân vật vẫn phải mặc được: tóc cơ bản, đồ lót/áo cơ bản xám, shorts, socks. Không món class nào được dùng để che lỗi body.

## Animation foundation

Dùng 2D skeletal animation + sprite swapping để một animation dùng được cho nhiều bộ đồ.

Locomotion dùng chung cho mọi class:

- Idle
- Walk
- Run
- Jump Start
- Jump Loop
- Fall
- Land
- Dash
- Turn
- Interact
- Sit
- Emote
- Hit
- Knockback
- Death
- Respawn

Combat animation tách theo class.

## 5 class trong 2D

| Class | Identity | VFX/đọc hình |
| --- | --- | --- |
| Võ | áp sát, combo, phá giáp, phản đòn | cam, orange, gold, dust, impact, shockwave |
| Kiếm | tốc độ, kiếm thuật, counter, mobility | afterimage, sword trail, sword aura, precise hit, air combo |
| Pháp | ranged, elemental, AoE, control | fire, ice, lightning, spirit, barrier, gravity |
| Cơ | ranged weapon, machinery, deployables | turret, mine, drone, cannon, rail shot |
| Linh | summon, support, control, purification | heal, buff, shield, summon, bind/debuff |

Progression hình ảnh từ Lv1 đến Lv100 là layer sprite, weapon sprite, accessory, secondary animation, shader và VFX; không phải thay model 3D.

## Zone Network

Không làm open world liên tục. Mỗi node là một hoặc vài map 2D:

```text
                    [THƯỢNG GIỚI]
                         │
              ┌──────────┴──────────┐
              │                     │
        [LINH SƠN]             [PHÁP VỰC]
              │                     │
              └──────┐       ┌──────┘
                     ▼       ▼
                    LINH THÀNH
                        │
       ┌────────────────┼────────────────┐
       ▼                ▼                ▼
  KHU DÂN CƯ       THƯƠNG PHỐ        HỌC VIỆN
       │                │                │
       └───────────┬────┴────┬───────────┘
                   ▼         ▼
               ĐÔNG MÔN    TÂY MÔN
                   │
              TRAINING FIELD
                   │
                LINH LÂM
                   │
               CỔ DI TÍCH
                   │
               ÂM GIỚI
```

## Opening và tutorial

Opening cinematic target:

1. Màn hình đen, tiếng chuông.
2. Khe nứt tím xuất hiện trên bầu trời Linh Thành.
3. Linh phù thành phố sáng lên.
4. Voice-over: “Hai thế giới từng tồn tại cạnh nhau, nhưng chưa bao giờ thực sự tách biệt.”
5. Camera lướt qua Linh Thành, NPC, Player, Linh thú, Cơ giới, Pháp trận.
6. Voice-over: “Cho đến ngày những cánh cửa bắt đầu mở.”
7. Portal Âm Giới xuất hiện, cut vào Đông Môn.

Tutorial chính thức:

1. Xuất hiện tại Linh Thành – Đông Môn.
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

Prototype hiện có mới chắc bước 1-4; các bước jump/dash/skill/slime/return là next slices.

## Chapter progression

### Chapter 1 — Vết Nứt Đông Môn

Người Giữ Cổng cảnh báo linh khí ngoài thành dao động. Player đi Linh Thành → Đông Môn → Linh Lâm, gặp Shadow Slime, Corrupted Spirit, Lost Merchant, cuối chapter đánh Mini Boss Linh Thú Biến Dị và nhặt Âm Giới Fragment.

### Chapter 2 — Những Cánh Cổng Không Thuộc Về Thế Giới Này

Portal nhỏ xuất hiện. Mỗi class nhìn vấn đề khác nhau: Võ bảo vệ dân cư, Kiếm truy tìm kẻ đứng sau, Pháp nghiên cứu portal, Cơ đo năng lượng, Linh nghe tiếng gọi từ bên kia.

### Chapter 3 — Âm Giới Xâm Lăng

World event theo channel/instance: portal Âm Giới xuất hiện ở nhiều zone, người chơi hợp lực, đánh World Boss và nhận server reward.

## Core loop và session 20 phút

Core loop: Login → Character → Linh Thành → Social/Adventure → Reward → Upgrade → New Content.

Một session 20 phút mẫu: vào Linh Thành, chat, nhận Daily, đi Linh Lâm, combat, mini boss, nhận item, về thành, đổi trang phục, gặp guild, logout. Game không được trở thành “chỉ đánh quái”.

## Trang bị và asset pipeline 2D

Mỗi item chuẩn hóa:

- Icon
- Preview
- Front Sprite
- Back Sprite
- Side/Secondary Sprite nếu cần
- Bone Mapping
- Anchor
- Sorting Layer
- Mask
- VFX

Asset pipeline mới: Concept → Character Sheet → Module Separation → Clean 2D Sprite → Rig → Animation → Sprite Atlas → Unity → Runtime Test.

Map pipeline mới: Concept Map → Background Far/Mid/Near/Foreground → Collision → Spawn NPC/Monster/Portal/Interactive → Lighting → VFX → Runtime.

## Roadmap chuyển hướng

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

Vertical Slice target: Login → Character Select → Enter World → Linh Thành 2D → Walk/Run/Jump/Dash → Gate Keeper → Dialogue → Training → Use Class Skill → Shadow Slime → Reward → Return to Town.

## Production interpretation

Tài liệu này là khóa hướng cho mọi design mới. Khi ảnh board/concept có chi tiết đẹp, ta dùng làm reference mỹ thuật; khi chữ trong scenario/GDD quy định flow, class, zone, pipeline hoặc guardrail thì chữ thắng ảnh. Các concept class/trang phục cũ chỉ được đưa vào branch 2D qua sprite layer/module/anchor/atlas, không crop dán board trực tiếp vào runtime và không phục hồi source image cũ đã bị xoá.

## Guardrails

- Không đầu tư tiếp vào Meshy, sculpt, retopo, 3D outfit, PBR character material, 3D armor sockets, 3D hair, 3D cloth, 3D character LOD cho player character chính.
- Không dùng lại ảnh thiết kế/source cũ đã bị loại khỏi branch; reference mới phải đi qua concept → clean sprite/module → rig/atlas → Unity runtime test.
- Design/runtime mới phải chứng minh được bằng Unity build/test/runtime evidence; screenshot phải được xem bằng mắt khi thay đổi player-visible.
- Mọi map/character/item mới cần id/layer/anchor/fit profile rõ để mix & match, animation và runtime test không bị làm mò.
- Trước khi làm map production, kiểm tra `docs/02-GDD.md`, tài liệu này và `docs/design/LGO-2D-MAP-AZ-DESIGN-v0.1.md`; nếu mâu thuẫn, ưu tiên yêu cầu owner mới nhất và cập nhật spine trước khi implement.
