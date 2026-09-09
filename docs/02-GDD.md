# 02 — Game Design Document v0.1

## 0. 2D scenario lock — 2026-09-09

Linh Giới Online hiện khóa hướng **2D Side-Scrolling Social Action MMORPG** cho branch `feature/2d`: HD anime/stylized, không pixel-art, camera side view, map parallax nhiều lớp, combat nhanh có walk/run/jump/dash/skill, hub xã hội đông người. Đây là cách hiện thực mới của cùng thế giới Linh Giới; không làm lại backend/account/character/GameData/networking chỉ vì bỏ 3D.

Ba trụ cột sản phẩm:

| Trụ cột | Nội dung phải giữ |
| --- | --- |
| Social MMORPG | Linh Thành, bạn bè, bang hội, giao dịch, nhà/thời trang, hoạt động cộng đồng |
| Action | combo, dash, jump, skill, boss, PvP, world event |
| Progression | level, skill, trang bị, class, linh thú, ngoại hình |

Các concept class/trang phục đã có chuyển sang ngôn ngữ 2D: base sprite/cutout body, layer trang phục 2D, skeleton 2D, anchor point, sprite texture + shader 2D, sprite weapon, particle/VFX 2D, sprite atlas/resolution tier và character sprite sheet. Không tiếp tục đầu tư vào Meshy, sculpt, retopo body, 3D outfit mesh, PBR character material, 3D armor sockets, 3D hair/cloth hoặc character LOD cho player character chính.

Base character chỉ gồm **Male Base** và **Female Base**. Class đến từ trang bị/layer, không từ body riêng. Khi tháo toàn bộ equipment, nhân vật vẫn phải có basic hair, đồ mặc định xám, shorts và socks; không dùng một món class để che lỗi body. Item 2D phải chuẩn hóa icon, preview, front/back sprite, optional side/secondary sprite, bone mapping, anchor, sorting layer, mask và VFX để mix & match được.

Map dùng side view với layer Sky/Fog, Far Background, Mid Background, Near Background, Gameplay Plane và Foreground. Runtime không được là màn phẳng; production map cần parallax, foreground che nhẹ, lighting/fog/particle, nước/reflection giả, ngày/đêm, NPC chuyển động và linh khí môi trường theo từng bước triển khai.

World structure dùng **Zone Network**, không open-world liên tục: Thượng Giới, Linh Sơn, Pháp Vực, Linh Thành, Đông/Tây/Nam/Bắc Môn, Khu Dân Cư, Thương Phố, Học Viện, Training Field, Linh Lâm, Cổ Di Tích và Âm Giới. Mỗi node là một hoặc vài map 2D với route/spawn/marker rõ ràng.

Opening target: màn đen, tiếng chuông, khe nứt tím trên bầu trời Linh Thành, linh phù sáng lên, camera lướt qua Linh Thành/NPC/Player/Linh thú/Cơ giới/Pháp trận, rồi portal Âm Giới mở và cut vào Đông Môn.

Tutorial chính thức bắt đầu ở **Linh Thành – Đông Môn**:

1. Đi tới Người Giữ Cổng.
2. Nói chuyện.
3. Đi tới Đá/Bia Luyện.
4. Học di chuyển.
5. Học jump.
6. Học dash.
7. Dùng skill class.
8. Đánh Shadow Slime.
9. Quay lại NPC.
10. Mở Linh Thành.

Chapter 1 — **Vết Nứt Đông Môn**: Người Giữ Cổng báo linh khí ngoài thành dao động; player đi Linh Thành → Đông Môn → Linh Lâm, gặp Shadow Slime, Corrupted Spirit, Lost Merchant, cuối chapter đánh Mini Boss Linh Thú Biến Dị và nhặt Âm Giới Fragment.

Chapter 2 — **Những Cánh Cổng Không Thuộc Về Thế Giới Này**: portal nhỏ xuất hiện; Võ bảo vệ dân cư, Kiếm truy tìm kẻ đứng sau, Pháp nghiên cứu portal, Cơ đo năng lượng, Linh nghe tiếng gọi từ phía bên kia. Đây là cách giữ cùng cốt truyện nhưng tạo khác biệt gameplay class.

Chapter 3 — **Âm Giới Xâm Lăng**: world event theo channel/instance, portal xuất hiện ở nhiều zone, người chơi hợp lực qua các zone, đánh World Boss và nhận server reward. Contribution lâu dài phải hỗ trợ nhiều vai trò, không chỉ raw DPS.

Một session 20 phút mẫu: Login → Linh Thành → chat/nhận Daily → đi Linh Lâm → combat/mini boss → nhận item → về thành → đổi trang phục → gặp guild → logout. Game không được trượt thành “chỉ đánh quái”.

Roadmap 2D hiện hành: 2D-00 Direction Lock → 2D-01 Male/Female Base Character → 2D-02 Character Modular Runtime → 2D-03 Võ Lv1 → 2D-04 Kiếm Lv1 → 2D-05 Pháp Lv1 → 2D-06 Cơ Lv1 → 2D-07 Linh Lv1 → 2D-08 Animation Foundation → 2D-09 Linh Thành Map → 2D-10 Gate Keeper Tutorial → 2D-11 Shadow Slime Combat → 2D-12 Vertical Slice. Vertical Slice phải đạt: Login → Character Select → Enter World → Linh Thành 2D → Walk/Run/Jump/Dash → Gate Keeper → Dialogue → Training → Use Class Skill → Shadow Slime → Reward → Return to Town.

## 1. Core loop

### Moment-to-moment
Move -> Attack -> Dodge -> Skill -> Reaction -> Loot.

### Session loop
Quest / field event -> combat -> loot -> city -> upgrade / social / trade -> next activity.

### Long loop
Build -> relationships -> guild -> collection -> housing identity -> reputation -> new content.

## 2. Founder Alpha world

### `map.city.linh_thanh`
Primary social hub and signature world-event battleground.

Must eventually contain:
- central plaza;
- market district;
- café/social landmark;
- crafting district;
- guild access;
- housing access;
- fishing/social edge;
- world-event portal anchors.

### `map.field.mist_forest`
Intro field emphasizing readable melee combat.

### `map.field.spirit_river`
Second field emphasizing movement, ranged hazards and gathering hooks.

### `map.dungeon.shadow_gate`
First 4-player dungeon, final boss teaching break/telegraph mechanics.

## 3. Paths/classes

Founder Alpha supports:

### `class.sword`
- fantasy: agile sword fighter;
- strengths: mobility, crit, combo flow;
- skill ceiling: animation cancel / perfect dodge follow-up.

### `class.martial`
- fantasy: close-range martial fighter;
- strengths: counter, stagger, pressure;
- skill ceiling: timing and resource rhythm.

Future paths such as `class.arcane`, `class.tech`, `class.spirit` are reserved but out of current scope.

## 4. Combat loadout

Per character:
- basic attack chain;
- dodge;
- four active skills;
- one ultimate;
- one spirit skill.

Boss content must not be reducible to standing still and damage-spamming.

## 5. Spirit companions

Founder Alpha target IDs:
- `spirit.fox.ember` — mobility/crit leaning;
- `spirit.turtle.jade` — shield/counter leaning;
- `spirit.bird.storm` — attack-speed/chain effect leaning.

Spirits change build behavior. Cosmetic variants may monetize appearance; canonical combat availability cannot depend on paid-only acquisition.

## 6. Progression

Founder Alpha vertical progression:
- character level 1–20;
- equipment tier;
- skill ranks with bounded modifiers.

Horizontal progression:
- spirit collection;
- achievement;
- profile titles;
- cosmetic collection;
- housing trophy display.

## 7. Social

Minimum Founder Alpha social graph:
- nearby presence;
- chat;
- friend;
- party;
- guild-lite;
- profile;
- home visit.

## 8. Economy

Initial flow:
Monster/gathering -> material -> craft -> equipment/consumable -> marketplace -> combat/social use.

Server records currency mutations in an auditable ledger. Marketplace trades use escrow semantics.

## 9. Signature event

### `event.world.shadow_invasion`
Vietnamese display name: **Âm Giới Xâm Lăng**.

State machine:
1. scheduled;
2. warning;
3. invasion_open;
4. wave_defense;
5. boss;
6. success / failure;
7. rewards;
8. restoration.

Contribution must support more than raw DPS in later iterations. Founder Alpha may start with combat + objective contribution.

## 10. Monetization constitution

Allowed direction:
- fashion;
- weapon appearance;
- mount/spirit appearance;
- housing decoration;
- emotes;
- profile cosmetics;
- cosmetic-oriented season pass;
- bounded QoL.

Forbidden direction:
- paid-only best-in-slot power;
- paid-only PvP stat advantage;
- paid currency directly becoming unlimited tradable gold.
