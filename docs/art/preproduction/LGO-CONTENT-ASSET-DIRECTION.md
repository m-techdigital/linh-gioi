# Chuẩn bị asset theo thế giới và kịch bản gốc

Ngày 2026-09-08. Bản phân tích và thiết kế tác giả, chưa phải nội dung gameplay đã triển khai. Mục tiêu: cùng một bộ công cụ tạo được nhiều phong cách đúng vai trò, không biến mọi class/NPC thành người mặc trường bào.

## Nguồn và phạm vi

Đọc [Vision](../../00-VISION.md), [GDD](../../02-GDD.md), [scenario bible](../../story/LGO-EARLY-GAME-SCENARIO-BIBLE-v0.1.md), ba board gốc `linh-gioi-{concept-board,key-art,world-event-ui}.png`, và [concept đủ năm Lộ](../../reference-ui/lgo-five-paths-character-reference-v1.jpg). Đây là thế giới social action fantasy Á Đông hiện đại: thành phố sinh hoạt bình thường có thể chuyển thành chiến trường cộng đồng rồi được khôi phục.

Thiết kế chuẩn bị đủ **Võ/Kiếm/Pháp/Cơ/Linh**. GDD hiện dành Founder Alpha cho Võ/Kiếm; ba Lộ còn lại có lane thiết kế, không tự thêm class vào registry, balance hoặc server. Giới tính trên board là mẫu nhân vật, không khóa class theo giới tính. Sheet v0.20.0 chỉ là reference một số hướng; tên “Thiên Kiếm/Linh Vũ” trên đó không thay thế năm ID gốc.

Bản `linh-thanh-roster-draft.png` mới sinh bằng imagegen tích hợp là nghiên cứu trang phục/NPC, **không phải board định nghĩa đầy đủ class**. Hai bộ đầu của ảnh đó không thay bản năm Lộ; phần Linh ngà/jade trong nghiên cứu không phải màu đích của Linh gốc. Reuse concept năm Lộ đã có thay vì sinh thêm một thiết kế cạnh tranh. Prompt được lưu ở `roster-prompt.txt`.

## Nhân vật: chung công thức, riêng tạo hình

Catalog tác giả: [linh-thanh-roster.json](../../../client/art-source/roster/linh-thanh-roster.json), hiện đủ 5 hướng class và 5 vai NPC tương lai. Keeper và player nhập môn là preset thực tế riêng; player mới chưa đồng nghĩa đã dựng xong costume class Kiếm.

| Hướng | Nhận diện phải giữ | Phần cần dựng riêng / pose quyết định |
|---|---|---|
| Võ — `class.martial` | Vàng đất, quyền/bracer, tay rõ, quần linh hoạt | Áo ngắn, găng/bracer, quần; co tay, đá, né, không tà dài cản chân |
| Kiếm — `class.sword` | Xanh–trắng, kiếm, dáng linh hoạt | Áo/tà xẻ, vai gọn, kiếm và grip; vung kiếm, xoay, né |
| Pháp — `class.arcane` | Đen/jade, pháp trận hình học | Áo khoác có cấu trúc, focus; tay mở, niệm, ngắm; không đổi thành Linh |
| Cơ — `class.tech` | Kỹ nghệ, kính, da/đồng, thiết bị | Kính, áo làm việc, túi dụng cụ, pháo/thiết bị; grip hai tay và khoảng lùi; không dùng nguyên trường bào |
| Linh — `class.spirit` | Lavender/ngà, linh phù, dải vải mềm | Áo/váy có lớp trong, linh phù, ornament; tay mở, xoay, bước; phân biệt hiệu ứng bạn với Âm Giới bằng hình và tín hiệu trạng thái |

Chia món theo catalog slot hiện hữu: đầu/mặt, tóc, mũ, áo, quần, choàng, găng, giày, vai, bội/phụ kiện, đồ tay chính/phụ, mắt/râu khi cần. Dùng chung rig/animation khi tương thích; phải fit theo hình thể, không ép người bán hàng đậm người hoặc nhân vật nữ vào mesh nam gầy. Đầu phải đủ sọ/gáy dù thay tóc ngắn; áo phải kín cổ tay/eo dù đổi món. Không dùng tóc/cape để che giải phẫu bị thiếu. Cloth và phụ kiện ảnh hưởng silhouette được dựng, hoa văn nằm trên bề mặt liền.

NPC cần thể hiện công việc: Keeper chào/chỉ đường ở SCN-001; người trông điện điềm đạm; tiếp đón hội quán với sổ; người bán hàng với quầy/túi; người dẫn đường lớn tuổi với gậy/râu; người đưa tin với túi chéo/thư. Vai tương lai chỉ chuẩn bị asset và pose; không tự mở shop/guild/quest từ chữ trong sheet. Ngoài năm vai này, café/thợ thủ công/người câu cá/cư dân là nhóm mở rộng theo GDD; dùng trang phục đời thường, không mặc định giáp nghi lễ.

## Map: theo hoạt động và trạng thái

| Khu gốc | Trải nghiệm / bố cục cần chuẩn bị | Bộ asset, trạng thái và phụ thuộc |
|---|---|---|
| Linh Thành — `map.city.linh_thanh` | Cổng → Keeper → đá → sân nghỉ trong slice hiện tại; lâu dài có quảng trường, café, chợ, chế tác, hội quán, lối nhà ở, bờ câu cá | Bộ kiến trúc phố, shopfront/café, bàn ghế, mái/đèn, cây, bờ nước; chừa lối đông người và anchor sự kiện. Không coi cổng/sân luyện là cả thành phố |
| Rừng Sương — `map.field.mist_forest` | Đọc đường đi và cận chiến; điểm khám phá ngoài thành | Bộ địa hình/đá/rễ/cây/sương nhẹ riêng, landmark phân hướng; không reskin nhà phố thành rừng. Combat theo gate riêng |
| Linh Hà — `map.field.spirit_river` | Di chuyển, nguy hiểm tầm xa, móc thu thập theo GDD | Bờ nước/cầu/bến/đá/vegetation; tách mặt nước, lối đi và collision; không giả đã có gathering/fishing |
| Cổng Âm — `map.dungeon.shadow_gate` | Dungeon bốn người; boss dạy telegraph/break theo GDD | Bộ hành lang/cổng/phòng/đấu trường riêng; silhouette hiểm nguy, sàn đọc rõ; network/party/boss cần gate |

Ảnh bố cục 3D đã render: [cổng](linh-thanh-gate-draft.png), [quảng trường có café](linh-thanh-plaza-draft.png), [phố chợ](linh-thanh-market-draft.png). Đây là draft hình khối có thể tái dựng từ [catalog và script](../../../client/art-source/linh-thanh-kit/SOURCE.md), chưa phải art final hoặc map runtime.

Bộ kit Linh Thành hiện chuẩn bị cổng, nhà thấp, nhà phố cao, café, đèn, ghế, cây, sạp và hoa văn sân; JSON/Blender phục vụ duyệt hình khối, chưa có adapter runtime. Chỉ chung những họ vật liệu hợp lý: đá/gỗ/ngói/kim loại/vải; phần thiên nhiên và Âm Giới có bộ riêng. Theo owner 2026-09-08, mặt ngoài nhà cửa cần hơi cổ kính: gỗ sẫm, tường trát cũ nhẹ, ngói xám xanh, cửa song, mái hiên/biển treo; café và đời sống hiện đại nằm trong nền kiến trúc ấy, tránh mặt dựng kính lớn. Grid/pivot/socket dùng để ghép nhanh, không bắt mọi cảnh thành lưới vuông đều. Chi tiết mặt tiền, mái, cao độ, sinh hoạt tạo sự khác biệt giữa khu.

Âm Giới Xâm Lăng dùng cùng thành phố qua các lớp: bình thường → cảnh báo → cổng mở → phòng thủ → boss → kết quả → khôi phục (state đầy đủ tại GDD). Thiết kế asset nên chuẩn bị lớp portal/ánh sáng/đổ vỡ/phục hồi tách khỏi nền phố; không nhân bản cả map cho mỗi trạng thái. Các overlay này chưa được triển khai bởi bộ kit. Không chép VIP/currency/reward hay mật độ HUD trên board vào game như yêu cầu hệ thống.

## Item, skill và sinh vật

| Nhóm | Công thức tác giả | Kiểm tra liên quan khi triển khai |
|---|---|---|
| Trang phục/trang bị | Món + họ cơ thể + vật liệu + vùng da cần giữ + pose; recipe tham chiếu asset | Kín seam, không xuyên khi đổi/di chuyển, mesh/material dùng chung, chi phí phần mới |
| Vũ khí/đồ nghề | Mesh riêng + socket/grip + trạng thái cầm/cất; sword/bracer/focus/device/talisman riêng ngôn ngữ | Hướng/grip/two-hand, không xuyên người, collider gameplay tách đồ trang trí |
| Skill/VFX | Tách ý định/diễn hoạt/telegraph/impact/recovery; hình tín hiệu theo Võ/Kiếm/Pháp/Cơ/Linh | Timing, vùng ảnh hưởng đọc được, hủy/né, phân biệt bạn–địch; damage/cooldown thuộc GameData/server |
| Vật liệu/tiêu hao/đồ nhà | Icon cùng hệ, vật thể world chỉ khi cần quan sát/cầm/đặt | Độ rõ ở kích thước thật, tham chiếu item có thật; không sinh reward/giá/chỉ số từ concept |
| Linh thú/quái | Họ rig riêng theo giải phẫu; fox/turtle/bird trong GDD, slime khác humanoid | Idle/move/action theo loài, silhouette và telegraph; không ép tất cả vào rig65 người |

Chưa thiết kế tên/chỉ số hàng loạt skill/item vì kịch bản và vai trò chiến đấu phải quyết định trước. GDD loadout có basic/dodge/4 active/ultimate/spirit skill; catalog art không thay hợp đồng combat. Thứ tự phù hợp: hành vi mẫu và cửa sổ phản hồi → asset cần dùng → nối dữ liệu đã được duyệt → kiểm tra runtime đúng hành vi đó.

## Bài học áp dụng từ hệ thống đã có

- [Valve Dota 2 wearables](https://www.dota2.com/workshop/requirements/warlock): chia theo món có ý nghĩa và budget từng món. Áp dụng danh mục slot, không cắt hoa văn thành hàng chục mesh hoặc lấy nguyên budget Dota cho camera cận.
- [Epic modular characters](https://dev.epicgames.com/documentation/unreal-engine/working-with-modular-characters-in-unreal-engine): rig tương thích và cách ghép ảnh hưởng chi phí. Pipeline hiện dùng chung Avatar, giữ module ở Editor rồi bake preset; đổi đồ động/cache tổ hợp/socket còn là bước tiếp, chưa tuyên bố đã có.
- [Unity 6 model optimization](https://docs.unity3d.com/6000.0/Documentation/Manual/ModelingOptimizedCharacters.html): giảm renderer/material và kiểm soát skin influences; chất lượng phải cân bằng với chi phí. Giữ PC rõ nét trước, đo vertex Unity, texture/RAM và số actor thật; không lấy KB file Blender làm GPU budget.
- [Epic City Sample](https://dev.epicgames.com/documentation/unreal-engine/city-sample-project-unreal-engine-demonstration): họ module + luật bố trí + instance, nội dung phụ thuộc layout/đường. Áp dụng catalog và bố cục có chủ đích; không đưa Nanite/Lumen hay một generator thành phố khổng lồ vào Unity chỉ để giống công nghệ tham khảo.
- [Guild Wars 2 — Dynamic Events](https://www.guildwars2.com/en-gb/the-game/dynamic-events/): sinh hoạt địa phương, cùng tham gia sự kiện và kết quả làm đổi khu vực. Bài học thiết kế cho Linh Giới: viết trạng thái thường ngày/cảnh báo/phòng thủ/khôi phục của cùng địa điểm và người dân trước khi dựng asset; đây là đề xuất bám GDD, không sao chép loot/scaling hay tuyên bố đã có event MMO.

Ưu tiên tiếp: đóng lỗi player đầu/gáy và seam; hoàn thiện bộ phố có café và sinh hoạt; sau đó chọn một bộ Võ và một NPC đời thường để chứng minh công thức vượt khỏi trang phục Keeper. Chuẩn bị fit nữ trước Kiếm/Linh/Cơ theo mẫu tương ứng. Mỗi lượt phải có thay đổi hình hoặc tương tác rõ; chỉ build/capture phần vừa đổi sau macro pass.
