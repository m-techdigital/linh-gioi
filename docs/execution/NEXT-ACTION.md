# NEXT ACTION — Character Hub UI

## Authoritative resume
- Worktree: `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`; branch `codex/character-hub-v22`; upstream `origin/feature/2d`.
- v24 đã push `f32e74e4`. v25 là commit chứa bàn giao này; fetch và xác nhận HEAD/upstream trước sửa tiếp.
- Canonical duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.
- Không sửa class/pose/wardrobe/source renderer/camera, frozen surfaces hoặc main checkout dirty.

## v25 — shared ornament / selection: batch đã kiểm
- Hai inspector, icon detail và selection dùng lại corner/edge vector của RuntimeUiSkin; không thêm PNG, không đổi atlas hay tọa độ/canvas.
- Rail trang bị nay highlight đúng slot đang chọn trong detail; Skill selected dùng khung vuông vàng độc lập, unselected không giữ vòng CSS xanh cũ.
- Tiềm năng giữ frame/content tách riêng; square hero frame tắt khi vào Tiềm năng và dùng lại đúng instance khi quay về tab khác.
- Player lượt đầu phát hiện viền đè nhãn level: test RED tái hiện index frame sau label; sửa base `Insert(0)` thay vì dịch từng nhãn. Lượt đầu giữ làm evidence lỗi, không coi visual PASS.
- Test: RED ban đầu 2/2 fail đúng hành vi thiếu; paint-order RED 1/1. Final full graphics EditMode 298/298, 0 fail/skip; Python 7 asset +25 governance +7 capture PASS.
- Player final build errors=0, warnings=46; 87 ảnh ở `build/character-hub-frame-v25/runtime-final/{pc,mobile,tablet}`. Đã xem đủ 5 tab trên cả 3 viewport, không cắt/chồng do batch; nhãn level không bị viền xuyên qua.
- Tổng 2 build/2 capture matrix: lượt hai chỉ chạy vì lỗi paint-order có evidence. Không đổi Resources/ProjectSettings/class/frozen; Unity-generated drift đã lưu rồi loại.
- Review độc lập Codex bị chặn quota trước khi review, không claim gate đó PASS; đã kiểm trực tiếp diff, test và ảnh Player.

## Active next — inspector content / state fidelity
- Gom batch theo hierarchy: fact rows/icon/text/value, trạng thái nút và connector Skill bám canonical; dùng component chung, không tạo số liệu gameplay giả để giống ảnh.
- Giữ mọi frame/content độc lập, frame được tạo một lần và phải được vẽ trước nội dung để nằm bên dưới nhãn/icon; không dựng lại theo class/tab.
- Sau batch: full tests liên quan, Player 3 viewport, eye audit, frozen/budget rồi commit/push qua supervisor; không force-push.

`CONTINUE / VISUAL_FIX_REQUIRED`: chưa nghiệm thu toàn bộ 5 tab. Một số icon Skill ngoài Kiếm và typography/art chưa sát canonical; mobile/tablet mới là viewport macOS, không phải thiết bị thật.
