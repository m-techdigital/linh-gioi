# Linh Giới Online — 2D Kiếm Lv1 Starter Outfit v0.1

Ngày cập nhật: 2026-09-09
Branch: `feature/2d`

## Mục tiêu

Bổ sung lớp trang bị Kiếm Lv1 để kiểm chứng hệ modular 2D sau khi đã có Võ Lv1. Kiếm Lv1 là class tốc độ/kỹ thuật, nên trang phục khởi đầu giữ form gọn, màu xanh đen, ít chi tiết, có `sword_trail_seed` cho VFX chém nhẹ ở giai đoạn tutorial.

## Module runtime

- `top_kiem_lv1_male`: áo Kiếm nam, fit `base_male_torso`.
- `top_kiem_lv1_female`: áo Kiếm nữ, fit `base_female_torso`.
- `pants_kiem_lv1_unisex`: quần gọn, fit `base_hips_legs`.
- `waist_kiem_lv1_unisex`: đai xanh đen, fit `base_hips`.
- `gloves_kiem_lv1_unisex`: bọc tay nhẹ, fit `base_hands`.
- `boots_kiem_lv1_unisex`: giày linh hoạt, fit `base_feet`.
- `weapon_kiem_lv1_starter`: kiếm gỗ luyện tập, anchor `WeaponAnchor`, VFX seed `sword_trail_seed`.

## Quy tắc fit/mix

Tất cả module đi qua cùng slot/fitting spine đã khóa cho 2D: tóc/mắt có id riêng, áo nam/nữ tách riêng theo base torso, phần còn lại dùng fit profile chung nếu không phụ thuộc giới tính. Loadout phải mix được, ví dụ áo Kiếm + kiếm Kiếm + quần Võ, để tránh vòng lặp mỗi món đồ phải fit thủ công với toàn bộ món khác.

## Runtime preview

Trong scene Đông Môn có rack nhỏ `KIẾM LV1` để chứng minh module class thứ hai đã đi vào flow runtime. Đây là preview procedural để giữ branch nhẹ; chưa phải final sprite atlas.
