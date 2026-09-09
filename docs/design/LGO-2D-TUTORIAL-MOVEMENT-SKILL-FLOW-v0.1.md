# Linh Giới Online — 2D Tutorial Movement/Skill Flow v0.1

## Mục tiêu

Mở rộng tutorial Đông Môn sau Bia Luyện Khí thành chuỗi có hành động thật: Jump → Dash → ClassSkill → Complete. Đây là bước nối gameplay nhập môn với animation spine 2D, để các state không chỉ tồn tại trong catalog.

## Flow runtime

1. Người chơi nói chuyện với Người Giữ Cổng.
2. Người chơi tới Bia Luyện Khí.
3. Bia mở bài tập `LearnJump`; action `Jump`, phím `Space/J`.
4. Jump mở `LearnDash`; action `Dash`, phím `Shift/K`.
5. Dash mở `LearnClassSkill`; action `Skill`, phím `Q/L`.
6. Skill hoàn tất nhập môn, apply Võ Lv1 seed outfit và mở hướng vào Linh Thành.

## Animation intent

- Movement thường đặt `LastAnimationIntent=Walk`.
- Jump đặt `LastAnimationIntent=Jump`.
- Dash đặt `LastAnimationIntent=Dash`.
- Skill đặt `LastAnimationIntent=ClassSkill`.
- Completion render `TrainingCompletePose`.

## Runtime evidence

Visual capture tăng từ 5 lên 8 frame:

- `05-jump-ready`
- `06-dash-ready`
- `07-skill-ready`
- `08-complete`

Manifest phải có `screenshotCount=8`, `finalStep=Complete`, HUD completion và `runtimeAnimationSnapshot` chứa Jump/Dash/ClassSkill/TrainingCompletePose.
