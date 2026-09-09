# Linh Giới Online — 2D Character Base Design v0.1

## Mục tiêu 2D-01

Tạo spine cho **Male Base** và **Female Base** theo hướng HD 2D anime/stylized. Đây là nền cho 2D-02 Character Modular Runtime; chưa sản xuất full art, chưa làm class outfit, chưa mở combat.

## Quy tắc

- Chỉ có hai base body chính: `male_base` và `female_base`.
- Khi tháo toàn bộ equipment, nhân vật vẫn hiển thị hợp lệ với tóc cơ bản, underwear/innerwear xám, shorts và socks.
- Class đến từ layer trang bị, weapon, accessory, VFX và animation, không đến từ body riêng.
- Tóc, mắt, trang bị, vũ khí, pet/spirit đều có id/slot riêng để đổi sau.
- Runtime ban đầu có thể procedural, nhưng phải giữ đúng layer order và anchor naming để asset 2D thật thay vào không phá flow.

## Layer order chuẩn

| Order | Layer | Ghi chú |
| ---: | --- | --- |
| 0 | Shadow | bóng chân |
| 10 | BackFX | hào quang sau |
| 20 | BackAccessory | áo choàng, kiếm sau lưng |
| 30 | HairBack | tóc sau |
| 40 | Body | body parts |
| 50 | Underwear | đồ mặc định khi tháo gear |
| 60 | PantsOrSkirt | quần/váy |
| 70 | InnerShirt | áo trong |
| 80 | OuterShirt | áo ngoài |
| 90 | Waist | thắt lưng |
| 100 | Shoulder | vai/giáp vai |
| 110 | Gloves | găng |
| 120 | Boots | giày |
| 130 | HairFront | tóc trước |
| 140 | HeadAccessory | phụ kiện đầu |
| 150 | Weapon | vũ khí |
| 160 | Offhand | offhand |
| 170 | PetSpirit | pet/drone/spirit |
| 180 | FrontFX | hiệu ứng trước |
| 190 | UIAnchor | nameplate/status |

## Body parts tối thiểu

Mỗi base phải có các anchor/body part sau:

- Root
- Hips
- Torso
- Chest
- Neck
- Head
- UpperArm_L/R
- Forearm_L/R
- Hand_L/R
- Thigh_L/R
- Calf_L/R
- Foot_L/R
- HairBackAnchor
- HairFrontAnchor
- WeaponAnchor
- OffhandAnchor
- PetAnchor
- UIAnchor

## Default outfit

Nam: body, basic hair, grey undershirt, grey shorts, grey socks.

Nữ: body, basic hair, grey sports-bra/inner top, grey shorts, grey socks.

Default outfit là safety layer, không phải class gear.

## Runtime checkpoint

Checkpoint 2D-01 cần chứng minh:

1. Catalog có đủ male/female base và layer/anchor tối thiểu.
2. Runtime controller expose snapshot để manifest kiểm được.
3. Player/NPC đang render bằng cùng logic layered base, không hardcode một khối robe duy nhất.
4. Unity EditMode + onboarding smoke + Player visual capture pass.
