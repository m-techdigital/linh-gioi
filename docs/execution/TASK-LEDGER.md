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
