# TASK LEDGER — 2D Branch

## 2026-09-09 — 2D pivot cleanup

- Branch: `feature/2d`.
- Owner yêu cầu loại bỏ hướng dựng nhân vật/cảnh cũ và chuyển sang game 2D.
- Đã dọn source Unity, asset source, tooling thử nghiệm, generated staging/evidence và cache nặng liên quan hướng cũ.
- Đã thêm validator `tools/validate_2d_branch_no_3d.py`.
- Frozen surfaces không đổi.

## Next

Dựng runtime 2D SCN-001/002 có thể chơi và capture thật.

## 2026-09-09 — 2D onboarding runtime slice

- Branch: `feature/2d`.
- Dựng slice nhập môn 2D bám SCN-001/002: player bắt đầu ở Cổng Linh Thành, tới Người Giữ Cổng, mở thoại, nhận hướng dẫn tới Bia Luyện Khí, kích hoạt bia và hoàn tất nhập môn.
- Thêm state machine, controller sprite/layer placeholder, smoke runner Editor/Player, visual capture runner 5 trạng thái và edit-mode coverage cho flow.
- Evidence runtime: `build/2d-onboarding-player/player-smoke.json`, `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`, ảnh review `build/2d-onboarding-visual/05-complete.png`.
- Giới hạn đã biết: visual capture camera-render chưa bắt HUD OnGUI; HUD cần chuyển sang UI runtime ở batch tiếp theo.

## Next after runtime slice

Nâng art/UI 2D cho SCN-001/002, ưu tiên sprite 2D đẹp hơn và HUD capture được, không mở rộng combat/reward/frozen contracts.


## 2026-09-09 — 2D onboarding HUD capture

- Thay HUD OnGUI tạm bằng HUD world-space để visual capture camera-render bắt được nội dung player-facing.
- Thêm test `RuntimeControllerMaintainsCameraCapturedHudText`; RED đã fail vì thiếu `WorldHudSnapshot`/`WorldHudLineCount`, GREEN qua Unity compile/test sau implementation.
- Visual manifest bổ sung `hudLineCount` và `hudSnapshot`.
- Evidence: `build/2d-onboarding-visual/03-dialogue.png`, `build/2d-onboarding-visual/05-complete.png`, `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`.

## Next after HUD capture

Thay dần sprite placeholder bằng art 2D đẹp hơn cho Cổng Linh Thành, NPC, player và Bia Luyện Khí; không quay lại pipeline 3D cũ.

## 2026-09-09 — Remove obsolete source images for 2D branch

- Owner yêu cầu bỏ hết ảnh thiết kế/source cũ vì không còn dùng được cho hướng 2D mới.
- Đã xoá ảnh source cũ khỏi Unity Art/UI Resources và docs/reference/design cũ; evidence runtime trong `build/` không thuộc source tree.
- Thêm validator `tools/validate_2d_branch_no_source_images.py` để phát hiện ảnh source lẻn lại.
- Runtime 2D onboarding vẫn dùng procedural sprites/HUD world-space nên không phụ thuộc các ảnh đã xoá.

## Next after image cleanup

Thiết kế lại art 2D cho SCN-001/002 theo mẫu mới được duyệt, bắt đầu từ Cổng Linh Thành, Người Giữ Cổng, player và Bia Luyện Khí.

## 2026-09-09 — Đông Môn procedural blockout detail pass

Nâng runtime 2D onboarding theo hướng map Đông Môn tutorial: thêm biển Cổng Linh Thành, viền/ấn ngọc, lồng đèn, lối ngọc, đường kẻ sân, label Bia Luyện Khí, đốm linh khí và silhouette nhân vật nhiều lớp hơn. Thêm scene-beat snapshot/count vào controller và visual manifest để gate không chỉ dựa vào cảm giác. Evidence mới: Unity EditMode, Editor smoke, macOS Player build và Player visual capture 5 frame.

## Next after Đông Môn blockout

Triển khai design map A-Z từ board mới owner gửi: world map tổng thể → Linh Thành hub → Đông Môn tutorial → layer/tileset/ký hiệu/flow, bắt đầu bằng spec text và runtime blockout không dùng ảnh cũ.

## 2026-09-09 — 2D direction lock + map catalog realignment

Owner cung cấp kịch bản game mới trước khi design. Đã khóa lại hướng: 2D Side-Scrolling Social Action MMORPG, HD anime/stylized, không pixel-art, không quay lại Meshy/3D, giữ backend/world/class/progression/story. Thứ tự ưu tiên mới: 2D-00 Direction Lock → 2D-01 Male/Female Base Character → 2D-02 Character Modular Runtime → class Lv1 → animation → Linh Thành/Đông Môn map → Shadow Slime combat → vertical slice. Runtime map catalog bắt đầu phản ánh World/Linh Thành/Đông Môn tutorial mới và visual manifest có map snapshot để tránh làm mò.

## Next after direction lock

Sau khi checkpoint này pass/push, bắt đầu 2D-01 Male/Female Base Character; chỉ triển khai map production lớn sau khi character base/modular runtime đủ spine.

## 2026-09-09 — 2D-01 Male/Female Base Character

- Bám kịch bản game mới: 2D Side-Scrolling Social Action MMORPG, HD anime/stylized, không Meshy/3D, class đến từ equipment/layer chứ không tách body riêng.
- Thêm design doc `docs/design/LGO-2D-CHARACTER-BASE-DESIGN-v0.1.md` để khóa layer order, anchors và default outfit trước khi sản xuất modular gear.
- Thêm runtime catalog `TwoDCharacterBaseCatalog` với `male_base`/`female_base`, layer order chuẩn, anchor tối thiểu và snapshot dùng trong visual manifest.
- Runtime onboarding expose `runtimeCharacterBaseSnapshot` và minimap label Base để capture có bằng chứng base nam/nữ đang nằm trong spine runtime.
- Evidence: Unity EditMode, onboarding smoke, Player visual capture manifest `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`, ảnh review `03-dialogue.png` và `05-complete.png`.

## Next after 2D-01

2D-02 Character Modular Runtime: item/layer registry, equip/unequip, preview hành trang đã duyệt, giữ tóc/mắt/trang bị/vũ khí/pet có id riêng và dùng form chung để tránh fit/chắp vá từng món.

## 2026-09-09 — 2D-02 Character Modular Runtime

Đã thêm spine module/equipment cho hướng 2D: catalog module starter, slot riêng cho tóc trước/sau, mắt, áo, quần, giày, vũ khí và pet/spirit; loadout hỗ trợ flow hành trang đã duyệt `select_icon -> inspect_item -> try_on -> cancel_or_apply`. Runtime manifest có `runtimeEquipmentSnapshot` để kiểm slot/id/fit profile trước khi tạo đồ Võ Lv1 thật.

## Next after 2D-02

2D-03 Võ Lv1 outfit/module cơ bản cho nam/nữ, giữ form đơn giản theo level đầu và khai báo qua slot/fit profile chung thay vì hardcode từng món.

## 2026-09-09 — 2D-03 Võ Lv1 starter outfit seed

Đã thêm seed outfit Võ Lv1 qua module runtime: top riêng nam/nữ, quần, đai, găng và boots dùng fit profile chung. Khi hoàn tất Bia Luyện Khí, runtime apply loadout Võ Lv1 để capture thấy đổi trang bị trên player; manifest xác nhận `top=top_vo_lv1_male`, `waist=waist_vo_lv1_unisex`, `gloves=gloves_vo_lv1_unisex`, `boots=boots_vo_lv1_unisex`, `class=Vo`, `level=1`.

## Next after 2D-03

Tiếp theo nên làm 2D-05 Animation Locomotion để slice nhập môn bớt tĩnh, hoặc 2D-04 Kiếm Lv1 nếu cần mở thêm class module trước.
