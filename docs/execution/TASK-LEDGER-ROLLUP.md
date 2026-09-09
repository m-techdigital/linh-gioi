# TASK LEDGER ROLLUP — 2D Pivot

## 2026-09-09 — Branch 2D cleanup

Owner yêu cầu dừng hướng dựng nhân vật/cảnh cũ và chuyển sang game 2D trên `feature/2d`. Batch cleanup đã dọn source, asset source, tooling thử nghiệm, generated staging/evidence và cache nặng liên quan hướng cũ; thêm validator `tools/validate_2d_branch_no_3d.py` để ngăn kéo lại pipeline đó. Commit/push: `d97a3c8 Remove 3D asset pipeline from 2D branch`.

## 2026-09-09 — Runtime 2D onboarding slice

Đang đóng gói slice SCN-001/002 đầu tiên: Cổng Linh Thành, Người Giữ Cổng, thoại nhập môn, đường dẫn tới Bia Luyện Khí, kích hoạt bia và trạng thái hoàn tất. Evidence đã chạy qua Editor smoke, Player smoke và visual capture 5 frame trong `build/2d-onboarding-visual/`.

## Next

Commit/push runtime 2D slice, sau đó nâng art/UI 2D cho cùng flow thay vì mở rộng hệ thống mới.


## 2026-09-09 — HUD capture cho 2D onboarding

Chuyển HUD nhập môn từ IMGUI tạm sang text/sprite world-space để Player visual capture thấy được title, khu vực, mục tiêu, gợi ý, action, feedback và thoại NPC. Visual manifest bổ sung `hudLineCount`/`hudSnapshot`; ảnh review `03-dialogue.png` và `05-complete.png` đã được xem bằng mắt.

## Next after HUD

Nâng art 2D cho cùng flow SCN-001/002 và giữ capture runtime thật trước checkpoint.

## 2026-09-09 — Xoá ảnh source cũ cho branch 2D

Owner yêu cầu loại bỏ toàn bộ ảnh thiết kế cũ vì không dùng được nữa. Batch hiện tại xoá ảnh source/reference/runtime placeholder cũ khỏi source tree, giữ evidence trong `build/` ngoài kiểm soát source, thêm `tools/validate_2d_branch_no_source_images.py`, và xác minh runtime 2D onboarding vẫn chạy bằng procedural sprites + HUD world-space.

## Next after source image cleanup

Commit/push checkpoint, sau đó tạo lại hướng art 2D mới cho SCN-001/002 bằng design mới đã duyệt thay vì tái dùng ảnh cũ.

## 2026-09-09 — Đông Môn procedural blockout detail pass

Nâng runtime 2D onboarding theo hướng map Đông Môn tutorial: thêm biển Cổng Linh Thành, viền/ấn ngọc, lồng đèn, lối ngọc, đường kẻ sân, label Bia Luyện Khí, đốm linh khí và silhouette nhân vật nhiều lớp hơn. Thêm scene-beat snapshot/count vào controller và visual manifest để gate không chỉ dựa vào cảm giác. Evidence mới: Unity EditMode, Editor smoke, macOS Player build và Player visual capture 5 frame.

## Next after Đông Môn blockout

Triển khai design map A-Z từ board mới owner gửi: world map tổng thể → Linh Thành hub → Đông Môn tutorial → layer/tileset/ký hiệu/flow, bắt đầu bằng spec text và runtime blockout không dùng ảnh cũ.

## 2026-09-09 — 2D direction lock + map catalog realignment

Owner cung cấp kịch bản game mới trước khi design. Đã khóa lại hướng: 2D Side-Scrolling Social Action MMORPG, HD anime/stylized, không pixel-art, không quay lại Meshy/3D, giữ backend/world/class/progression/story. Thứ tự ưu tiên mới: 2D-00 Direction Lock → 2D-01 Male/Female Base Character → 2D-02 Character Modular Runtime → class Lv1 → animation → Linh Thành/Đông Môn map → Shadow Slime combat → vertical slice. Runtime map catalog bắt đầu phản ánh World/Linh Thành/Đông Môn tutorial mới và visual manifest có map snapshot để tránh làm mò.

## Next after direction lock

Sau khi checkpoint này pass/push, bắt đầu 2D-01 Male/Female Base Character; chỉ triển khai map production lớn sau khi character base/modular runtime đủ spine.

## 2026-09-09 — 2D-01 Male/Female Base Character

Đã bắt đầu spine base character theo kịch bản mới: `male_base` và `female_base`, default hair/innerwear xám, layer order từ Shadow đến UIAnchor, anchor tối thiểu cho body/equipment/hair/weapon/pet. Runtime controller expose `runtimeCharacterBaseSnapshot` vào visual manifest để các batch sau không tự chế slot/layer ngoài form chung. Đây vẫn là procedural/blockout base, chưa phải art final.

## Next after 2D-01

Sau khi commit/push checkpoint này, chuyển sang 2D-02 Character Modular Runtime: registry item/layer, equip/unequip, inventory preview flow đã được duyệt, và slot/id riêng cho tóc, mắt, trang bị, vũ khí, pet/spirit.

## 2026-09-09 — 2D-02 Character Modular Runtime

Đã thêm spine module/equipment cho hướng 2D: catalog module starter, slot riêng cho tóc trước/sau, mắt, áo, quần, giày, vũ khí và pet/spirit; loadout hỗ trợ flow hành trang đã duyệt `select_icon -> inspect_item -> try_on -> cancel_or_apply`. Runtime manifest có `runtimeEquipmentSnapshot` để kiểm slot/id/fit profile trước khi tạo đồ Võ Lv1 thật.

## Next after 2D-02

2D-03 Võ Lv1 outfit/module cơ bản cho nam/nữ, giữ form đơn giản theo level đầu và khai báo qua slot/fit profile chung thay vì hardcode từng món.

## 2026-09-09 — 2D-03 Võ Lv1 starter outfit seed

Đã thêm seed outfit Võ Lv1 qua module runtime: top riêng nam/nữ, quần, đai, găng và boots dùng fit profile chung. Khi hoàn tất Bia Luyện Khí, runtime apply loadout Võ Lv1 để capture thấy đổi trang bị trên player; manifest xác nhận `top=top_vo_lv1_male`, `waist=waist_vo_lv1_unisex`, `gloves=gloves_vo_lv1_unisex`, `boots=boots_vo_lv1_unisex`, `class=Vo`, `level=1`.

## Next after 2D-03

Tiếp theo nên làm 2D-05 Animation Locomotion để slice nhập môn bớt tĩnh, hoặc 2D-04 Kiếm Lv1 nếu cần mở thêm class module trước.

## 2026-09-09 — 2D-05 Animation Locomotion spine

Đã thêm animation profile side-scroll cho character 2D: Idle, Walk, Run, Jump, Dash, ClassSkill và TrainingCompletePose. Runtime controller expose `runtimeAnimationSnapshot`; procedural pose hiện đổi nhẹ tay/chân/đầu khi di chuyển và phóng nhẹ ở completion pose sau Bia Luyện Khí. Player visual capture kiểm được frame `02-gate-focus.png` và `05-complete.png`.

## Next after 2D-05

Ưu tiên mở tutorial Jump/Dash/Skill để các animation state có trigger gameplay thật, hoặc 2D-04 Kiếm Lv1 nếu muốn mở thêm class catalog song song.

## 2026-09-09 — Tutorial Jump/Dash/ClassSkill flow

Đã mở rộng Đông Môn tutorial: sau Bia Luyện Khí, người chơi phải qua LearnJump, LearnDash và LearnClassSkill trước khi Complete. Runtime input hỗ trợ Space/J cho Jump, Shift/K cho Dash, Q/L cho skill; HUD đổi mục tiêu/hint/action theo từng bước. Visual capture tăng lên 8 frame và manifest xác nhận `screenshotCount=8`, `finalStep=Complete`, animation tokens Jump/Dash/ClassSkill/TrainingCompletePose.

## Next after movement/skill tutorial

Nối `ClassSkill` vào Shadow Slime combat micro-slice để có mục tiêu/quái thật, hoặc mở 2D-04 Kiếm Lv1 nếu cần thêm class module.

## 2026-09-09 — Khóa kịch bản 2D mới vào GDD

Owner cung cấp kịch bản game mới trước khi design map. Đã cập nhật `docs/02-GDD.md` với scenario lock 2D: Social Action MMORPG side-scrolling, HD anime/stylized, Zone Network, opening cinematic, tutorial Đông Môn 10 bước, Chapter 1-3, item/character/map pipeline 2D và roadmap 2D-00 → 2D-12. Cập nhật direction lock/execution docs để các batch sau không quay lại Meshy/3D, không crop/dán board, và không làm map production lệch khỏi GDD.

## Next after scenario lock

Tiếp tục Shadow Slime combat micro-slice đang có test đỏ, sau đó chọn 2D-04 Kiếm Lv1 hoặc map production theo roadmap; mọi design/map mới phải kiểm `docs/02-GDD.md` trước khi implement.
## 2026-09-09 — Shadow Slime combat runtime checkpoint

Nối `ClassSkill` trong tutorial Đông Môn vào mục tiêu thật: Shadow Slime xuất hiện sau Dash, HUD yêu cầu dùng kỹ năng Võ Lv1, skill đánh tan slime và hoàn tất nhập môn. Thêm `ShadowSlimeVisible`/`ShadowSlimeDefeated`, `runtimeCombatSnapshot`, smoke assertion và visual manifest field để kiểm trạng thái bằng test + runtime evidence. Scope vẫn nhỏ: chưa mở HP, loot, reward/economy hoặc combat server-authoritative.

## Next after Shadow Slime runtime checkpoint

Đóng checkpoint sau validation; tiếp theo chọn 2D-04 Kiếm Lv1 hoặc map production Linh Thành/Đông Môn theo GDD mới.
