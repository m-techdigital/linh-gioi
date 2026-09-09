# Linh Giới Online — Võ Lv1 Starter Outfit v0.1

## Mục tiêu 2D-03

Tạo seed outfit Võ Lv1 rất cơ bản cho nam/nữ bằng module runtime, chưa làm art final. Đây là bước kiểm chứng hệ slot/fit profile trước khi sản xuất sprite/atlas thật.

## Định hướng hình ảnh

- Tinh thần: mạnh mẽ, kỷ luật, cơ động, võ đạo Á Đông.
- Level: Lv1 khởi đầu, không cầu kỳ, không giáp nặng.
- Nam/nữ có cùng ngôn ngữ màu vàng đen đỏ, nhưng item top tách riêng theo base filter để sau này có silhouette khác nhau.

## Module seed

- `top_vo_lv1_male`: OuterShirt, `male_base`, `fit_profile=base_male_torso;class=Vo;level=1`
- `top_vo_lv1_female`: OuterShirt, `female_base`, `fit_profile=base_female_torso;class=Vo;level=1`
- `pants_vo_lv1_unisex`: PantsOrSkirt, `unisex`, `fit_profile=base_hips_legs;class=Vo;level=1`
- `waist_vo_lv1_unisex`: Waist, `unisex`, `fit_profile=base_hips;class=Vo;level=1`
- `gloves_vo_lv1_unisex`: Gloves, `unisex`, `fit_profile=base_hands;class=Vo;level=1`
- `boots_vo_lv1_unisex`: Boots, `unisex`, `fit_profile=base_feet;class=Vo;level=1`

## Runtime checkpoint

Sau khi người chơi hoàn tất Bia Luyện Khí, runtime apply seed Võ Lv1 vào loadout để visual capture thấy đổi gear. Khi có asset thật, module ids này là chỗ thay sprite thay vì đổi logic flow.
