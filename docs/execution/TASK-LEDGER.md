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
