# PROJECT STATE — Linh Giới Online 2D

## Current branch

`feature/2d`

## Current direction

Linh Giới Online tiếp tục theo North Star Social Action MMORPG, nhưng branch này khóa hướng **2D Side-Scrolling Social Action MMORPG**: HD anime/stylized, không pixel-art, map parallax nhiều lớp, combat nhanh có walk/run/jump/dash/skill, hub xã hội đông người. Linh Thành vẫn là hub xã hội; Đông Môn tutorial là lát cắt ưu tiên đầu: Người Giữ Cổng, Bia/Đá Luyện, movement/jump/dash, skill class, Shadow Slime, quay NPC và mở Linh Thành.

## Current source state

Đã dọn pipeline/source/asset/tool cũ liên quan hướng dựng nhân vật/cảnh 3D khỏi `feature/2d` và loại bỏ toàn bộ ảnh thiết kế/source cũ khỏi source tree để tránh kéo lại hướng art đã bỏ. Runtime 2D nhập môn hiện có một slice player-visible: di chuyển bằng WASD/phím mũi tên, focus NPC, mở thoại, nhận hướng dẫn tới Bia Luyện Khí, kích hoạt bia và hoàn tất nhập môn. Visual hiện là sprite/layer procedural gọn để kiểm flow, có HUD world-space, minimap/route overlay, snapshot base character nam/nữ và snapshot modular equipment được camera capture; chưa phải art final.

## Validation spine

- `python3.12 tools/validate_2d_branch_no_3d.py` bảo vệ branch khỏi việc kéo lại pipeline cũ.
- `python3.12 tools/validate_2d_branch_no_source_images.py` bảo vệ branch khỏi ảnh thiết kế/source cũ; evidence trong `build/` được bỏ qua.
- Unity batch compile/test là gate trước khi báo checkpoint.
- `tools/run_lgo_2d_onboarding_smoke.sh` kiểm flow nhập môn bằng Unity Editor command-line.
- Player smoke và visual capture trong `build/2d-onboarding-player/` + `build/2d-onboarding-visual/` là evidence runtime thật cho slice 2D, gồm HUD snapshot trong manifest và ảnh PNG review.

## Latest checkpoint evidence

- Cleanup commit: `d97a3c8 Remove 3D asset pipeline from 2D branch`.
- Meshy/service trace cleanup commit: `6bf905e Remove obsolete 3D service traces from 2D branch`.
- Source image cleanup commit: `646492e Remove legacy images and keep 2D HUD procedural`.
- Procedural Đông Môn blockout commit: `eeaf19d Improve 2D onboarding map blockout details`.
- Direction/map catalog checkpoint: `1fa3231 Lock 2D social action direction and map catalog`.
- Character base checkpoint: `0d31cc8 Add 2D character base catalog`.
- Modular equipment WIP/checkpoint: 2D-02 đã có module catalog starter, slot riêng cho hair/eyes/top/pants/boots/weapon/pet, loadout try-on/cancel/apply và `runtimeEquipmentSnapshot` trong visual manifest. Evidence runtime latest: visual manifest `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`; screenshot review PNG `build/2d-onboarding-visual/03-dialogue.png` và `build/2d-onboarding-visual/05-complete.png`.

## Next

Đóng gói checkpoint 2D-02 Character Modular Runtime, sau đó sang 2D-03 Võ Lv1 outfit/module cơ bản cho nam/nữ: chỉ làm seed module đơn giản bám level đầu, chưa mở hệ thống art production lớn. Map production lớn quay lại ở 2D-09/2D-10 sau khi character modular runtime đủ spine; hiện giữ Đông Môn blockout/minimap để runtime không bị trống.
