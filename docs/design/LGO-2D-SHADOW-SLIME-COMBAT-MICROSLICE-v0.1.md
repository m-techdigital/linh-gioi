# Linh Giới Online — 2D Shadow Slime Combat Micro-slice v0.1

## Mục tiêu

Nối bài học `ClassSkill` trong Đông Môn tutorial vào một mục tiêu thật theo kịch bản mới: Shadow Slime xuất hiện sau bài Dash, người chơi dùng kỹ năng Võ Lv1 để đánh tan nó, sau đó tutorial hoàn tất và hướng người chơi quay lại Người Giữ Cổng. Đây là micro-slice player-visible để kiểm action flow, chưa phải hệ combat/HP/loot đầy đủ.

## Flow runtime

1. Người chơi nói chuyện với Người Giữ Cổng.
2. Người chơi kích hoạt Bia Luyện Khí.
3. Tutorial chạy `LearnJump` rồi `LearnDash`.
4. Sau Dash, Shadow Slime xuất hiện ở cuối sân và state chuyển sang `LearnClassSkill`.
5. Người chơi bấm `Q/L` dùng kỹ năng Võ Lv1.
6. Shadow Slime chuyển sang defeated, biến khỏi scene, HUD feedback nhắc rõ `Shadow Slime`.
7. Tutorial `Complete`, apply seed outfit Võ Lv1 và hướng quay lại Người Giữ Cổng/mở Linh Thành.

## Visual/readability rule

- Shadow Slime dùng silhouette nhỏ, tím/âm khí, tách khỏi Bia Luyện Khí và NPC để người chơi hiểu đây là target combat.
- Frame visual capture `07-skill-ready` phải thấy Shadow Slime/label; frame `08-complete` phải thể hiện trạng thái sau khi slime bị đánh tan.
- Manifest phải có `runtimeCombatSnapshot` với `ShadowSlimeVisible` và `ShadowSlimeDefeated` để kiểm trạng thái bằng dữ liệu, không chỉ cảm giác ảnh.

## Scope chưa mở

- Chưa mở HP bar, damage numbers, loot, reward item, server-authoritative combat hoặc economy mutation.
- Chưa thêm monster catalog/schema mới vì frozen surfaces không đổi trong batch này.
- Chưa dùng ảnh/source art mới; runtime vẫn là procedural sprite để kiểm flow.

## Acceptance criteria

- Unity EditMode tests có test state và controller snapshot cho Shadow Slime.
- `tools/run_lgo_2d_onboarding_smoke.sh` pass và kiểm Shadow Slime defeated.
- Player build/capture tạo đủ 8 frame, manifest PASS và có `runtimeCombatSnapshot`.
- Ít nhất frame `07-skill-ready` và `08-complete` được xem bằng mắt sau capture.
