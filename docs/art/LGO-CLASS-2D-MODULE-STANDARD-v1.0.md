# Chuẩn module class 2D v1.0

Ngày 2026-09-09. Scope đang áp dụng: **Võ (`vo`)**, male và female. Owner thu hẹp batch hiện tại về Võ; chưa tạo spec triển khai hoặc art cho Kiếm/Pháp/Cơ/Linh. Chuẩn tài liệu này không đổi equipment contract runtime.

Nguồn ưu tiên: yêu cầu owner → `docs/02-GDD.md` và `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md` → north-star lock → visual reference usage guide → ảnh. Hai base nam/nữ dùng chung giữa các class; tháo đồ vẫn có tóc cơ bản, đồ xám, shorts và socks. Không đổi tỷ lệ body theo level hoặc dùng board để thay skeleton.

## Định danh ổn định

Class registry dùng cho tên reference: `vo`, `kiem`, `phap`, `co`, `linh`. Đây không phải ID protocol/GameData. Chỉ `vo` được kiểm độ đầy đủ trong batch này; không yêu cầu asset của bốn class còn lại.

Gender: `male`, `female`.

Level: `lv001`, `lv010`, `lv020`, `lv030`, `lv040`, `lv050`, `lv060`, `lv070`, `lv080`, `lv090`, `lv100`.

Thứ tự slot cố định, không đồng nghĩa sorting layer:

1. `main_weapon` — Vũ khí chính / Class weapon
2. `head_hair` — Đầu / Tóc / Mũ / Băng trán
3. `inner_top` — Áo trong
4. `outer_top` — Áo ngoài / Chiến y / Robe / Jacket
5. `lower_body` — Quần / Váy / Hạ y
6. `waist_belt` — Đai lưng
7. `arm_guard` — Bảo hộ tay / Cẳng tay / Găng phụ trợ
8. `footwear` — Giày / Ủng
9. `shoulder_chest_guard` — Giáp vai / Ngực nhẹ
10. `class_accessory` — Phụ kiện / Linh ấn / Trang sức / Class emblem

## Quy tắc bắt buộc

Equipment phải **không dính da thịt**: không body, tay, chân, mặt, torso hoặc base character. Ngoại lệ về tóc duy nhất: `head_hair` được chứa tóc rời, không đầu người; các slot khác không chứa tóc. Đồ hở ngón có lỗ trong suốt, không vẽ da để lấp lỗ.

- Không gộp `main_weapon` với `arm_guard`.
- Không gộp `inner_top` với `outer_top`.
- Không gộp `lower_body` với `waist_belt`.
- Không gộp `outer_top` với `shoulder_chest_guard`.

Mỗi pixel chi tiết thuộc đúng một slot: vạt may vào áo thuộc áo, tua treo từ đai thuộc đai, charm tháo riêng thuộc accessory. Tóc front/back, đôi giày, trái/phải của quyền khí là các phần của một slot, không tạo slot thứ 11. Base và VFX không phải equipment slot.

## Folder và naming

`assets/reference/classes/{class_id}/{gender}/equipment/{slot_id}/{class_id}_{gender}_{slot_id}_{level}.png`

Ví dụ: `assets/reference/classes/vo/male/equipment/main_weapon/vo_male_main_weapon_lv050.png`.

Folder vật lý đi theo class/gender/equipment/slot; level nằm trong filename, **không thêm thư mục level** làm sai convention. Một PNG canonical là preview item rời ở một góc chuẩn, không phải gói đầy đủ cho rig/animation. Side/back, các mảnh cutout, anchors, masks và VFX cần task/pipeline riêng trước runtime; không tự thêm suffix vào convention này.

**Đường dẫn là quy ước đích, chưa tạo ảnh tại đó.** Gate hiện hành `tools/validate_2d_branch_no_source_images.py` cấm source image trong branch. Giữ ảnh upload ngoài repo; không copy board vào các folder trên để giả đủ bộ. Khi có task ingest riêng phải giải quyết gate này minh bạch, không tắt validator.

## Base, anchor và motion contract

Base nhân vật không đổi theo level. Mọi item Võ phải khớp cùng một skeleton nam và một skeleton nữ qua `lv001` đến `lv100`; level chỉ thay trang bị, không thay chiều cao, tỉ lệ, khớp, thế đứng hoặc camera side-view. Cross-level mixing là gate bắt buộc: item cấp thấp/cao có thể dùng chéo trên cùng base mà không scale body hoặc dịch anchor.

Anchor tối thiểu phải ổn định cho tóc, áo trong, áo ngoài, hạ y, đai, giáp vai/ngực, cẳng tay, quyền khí, giày và phụ kiện. Motion test tối thiểu gồm `idle`, `walk`, `run`, `jump_start`, `jump_air`, `fall`, `land`, `basic_attack`, `skill_windup`, `skill_cast`, `skill_recover`. VFX skill là layer riêng, không bake vào equipment.

Chi tiết đang áp dụng cho Võ ở `docs/art/classes/vo/LGO-VO-2D-BASE-RIG-ANIMATION-SPEC-v1.0.md`. Màn rương/paper doll để thử đồ nằm ở `docs/design/LGO-2D-VO-CHEST-PAPERDOLL-DESIGN-v0.1.md`.

## Board phải có cho Võ

Prefix: `assets/reference/classes/vo/boards/`.

- `vo_module_map_v1.png`: mannequin reference tách vùng với bảng item; đủ 10 nhãn, chỉ vị trí/quan hệ, không xem thứ tự slot là z-order.
- `vo_male_equipment_grid_v1.png`: 10 hàng slot × 11 cột level, item rời.
- `vo_female_equipment_grid_v1.png`: cùng cấu trúc nam, item rời.
- `vo_male_outfit_progression_v1.png`: 11 nhân vật cùng base, pose, camera, tỷ lệ.
- `vo_female_outfit_progression_v1.png`: cùng tiêu chí.
- `vo_skill_vfx_progression_v1.png`: nghiên cứu visual, không chốt skill/unlock/damage.

Đối với class khác thay prefix `vo` bằng class_id khi được giao. Image boards là reference/design source, **không tự động là runtime assets**. Equipment grid chỉ có detached item assets; mỗi hàng đúng một slot, mỗi cột đúng một level; không mannequin, chân dung hoặc body trong ô item. Full outfit progression chỉ để visual review, **không dùng để trích xuất item**, không crop/slice board vào Unity.

## Cổng nghiệm thu

1. Spec: chạy `python3.12 tools/validate_class_2d_module_spec.py`; kiểm tài liệu/ID/quy tắc, không đánh giá ảnh.
2. Pack reference Võ đầy đủ: chạy cùng lệnh với `--require-assets --asset-root /path/to/assets/reference/classes`. Phải đủ 220 item PNG và 6 board; thiếu ảnh trả mã khác 0, không tạo placeholder.
3. Visual separation: người review mở từng ảnh theo checklist, ghi nguồn và lỗi. Checker tên file không phát hiện da, slot gộp hoặc alpha giả.
4. Runtime: chỉ sau clean sprite, rig, import và screenshot review; chưa chạy trong batch spec.

Tài liệu chi tiết: equipment slots, progression rules, image prompt templates, asset separation checklist và `classes/vo/LGO-VO-2D-MODULE-SPEC-v1.0.md` cùng thư mục này.

Hợp đồng sâu cho fit state, phối chéo level, attachment bone, coverage/occlusion và atlas residency nằm ở `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md`. Mọi class sau phải dùng chung contract này; không tạo loadout theo nguyên bộ level hoặc tự cấp runtime status từ crop.
