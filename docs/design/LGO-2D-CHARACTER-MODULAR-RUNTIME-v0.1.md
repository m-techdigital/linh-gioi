# Linh Giới Online — 2D Character Modular Runtime v0.1

## Mục tiêu 2D-02

Tạo spine runtime để mọi trang phục/phụ kiện 2D sau này đi qua cùng một form: slot, layer, base filter, fit profile, preview, cancel/apply. Batch này chưa sản xuất art final; mục tiêu là ngăn quay lại kiểu xử lý từng món rời rạc.

## Flow hành trang đã duyệt

`chọn icon → xem riêng món 3D/2D item view → thử trên người → Hủy thử/Áp dụng`

Trong branch 2D, item view ban đầu là preview module 2D; sau này UI có thể show sprite atlas/animation preview thật.

## Slot tối thiểu

- `HairFront`, `HairBack`: tóc có id riêng.
- `Eyes`: mắt có id riêng, không dính vào body.
- `InnerShirt`, `OuterShirt`, `PantsOrSkirt`, `Boots`: trang phục cơ bản và class gear.
- `Weapon`, `Offhand`: vũ khí/cầm phụ.
- `PetSpirit`: pet/drone/spirit đi kèm.

## Fit profile ban đầu

- `base_male_torso`
- `base_female_torso`
- `base_hips_legs`
- `base_feet`
- anchor-based slots: `HairFrontAnchor`, `HairBackAnchor`, `WeaponAnchor`, `PetAnchor`

Fit profile là hợp đồng runtime/source cho asset 2D sau này. Khi thêm đồ mới, item phải khai báo slot + base filter + fit profile/anchor; không được hardcode fit theo từng bộ.

## Runtime checkpoint

Checkpoint 2D-02 cần chứng minh:

1. Catalog có đủ module starter nam/nữ + Võ Lv1 seed item.
2. Loadout hỗ trợ try-on, cancel và apply.
3. Runtime manifest có `runtimeEquipmentSnapshot` chứa preview flow và slot/id quan trọng.
4. Unity EditMode + onboarding smoke + Player visual capture pass.
