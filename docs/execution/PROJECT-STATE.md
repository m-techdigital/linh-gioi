# PROJECT STATE — Linh Giới Online 2D

## Current branch

`feature/2d`

## Current direction

Linh Giới Online tiếp tục theo North Star Social Action MMORPG, nhưng branch này đi theo runtime 2D-first. Linh Thành vẫn là hub xã hội; SCN-001/002 là lát cắt ưu tiên: Cổng Linh Thành, Người Giữ Cổng, sân luyện và Bia Luyện Khí.

## Current source state

Đã dọn pipeline/source/asset/tool cũ liên quan hướng dựng nhân vật/cảnh 3D khỏi `feature/2d` và loại bỏ toàn bộ ảnh thiết kế/source cũ khỏi source tree để tránh kéo lại hướng art đã bỏ. Runtime 2D nhập môn hiện có một slice player-visible: di chuyển bằng WASD/phím mũi tên, focus NPC, mở thoại, nhận hướng dẫn tới Bia Luyện Khí, kích hoạt bia và hoàn tất nhập môn. Visual hiện là sprite/layer procedural gọn để kiểm flow, có HUD world-space được camera capture; chưa phải art final.

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
- Procedural Đông Môn blockout WIP: thêm scene-beat runtime cho Cổng Linh Thành, lối ngọc, lồng đèn, NPC, Bia Luyện Khí và metadata visual manifest.
- Runtime evidence latest: Player build `build/2d-onboarding-player/build-macos-player.log`; visual manifest có HUD + scene beats `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`; screenshot review PNG `build/2d-onboarding-visual/03-dialogue.png` và `build/2d-onboarding-visual/05-complete.png`.

## Next

Thiết kế và triển khai lộ trình map 2D từ các board mới: world map tổng thể, Linh Thành hub, Đông Môn tutorial side-scrolling, layer map/tileset/ký hiệu/flow. Bắt đầu bằng spec A-Z text + runtime blockout tương ứng, không dùng lại ảnh cũ, không mở combat/reward/frozen contracts khi chưa có gate riêng.
