# Linh Giới Online — 2D Locomotion Animation v0.1

## Mục tiêu 2D-05

Thêm animation spine đầu tiên cho character 2D side-scrolling để slice Đông Môn không còn là tượng đứng yên. Batch này vẫn dùng procedural blockout, nhưng giữ đúng hướng 2D skeletal/sprite-swap để asset thật có thể thay vào sau.

## State animation chuẩn ban đầu

- `Idle`: thở nhẹ khi đứng yên.
- `Walk`: stride/bob khi player di chuyển trong tutorial.
- `Run`: state dành cho mở rộng tốc độ cao.
- `Jump`: state dành cho bước tutorial jump.
- `Dash`: state dành cho bước tutorial dash.
- `ClassSkill`: state dành cho skill class đầu tiên.
- `TrainingCompletePose`: pose cộng hưởng sau Bia Luyện Khí, đồng thời cho thấy Võ Lv1 seed outfit đã apply.

## Runtime evidence

`TwoDOnboardingController.RuntimeAnimationSnapshot` expose profile + state hiện tại vào visual manifest. Visual capture hiện kiểm được `locomotion=side_scroll`, danh sách state, và final pose `vo_lv1_training_complete`.

## Giới hạn hiện tại

Đây chưa phải animation art final. Mục tiêu là runtime spine và player-visible motion proof; bước tiếp theo có thể thay procedural part motion bằng sprite sheet/2D skeletal rig thật.
