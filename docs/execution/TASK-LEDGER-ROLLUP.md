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
