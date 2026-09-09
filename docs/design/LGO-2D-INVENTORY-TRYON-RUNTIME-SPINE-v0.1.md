# Linh Giới Online — 2D Inventory Try-On Runtime Spine v0.1

Ngày cập nhật: 2026-09-09
Branch: `feature/2d`

## Mục tiêu

Đưa flow hành trang đã duyệt vào runtime 2D ở mức nhẹ: `icon → xem món → thử trên người → áp dụng/hủy`. Batch này không mở hệ persistence, loot, economy hoặc database item; chỉ chứng minh module Võ/Kiếm có thể đi qua UI spine và snapshot runtime.

## Runtime behavior

- Scene Đông Môn hiển thị strip `HÀNH TRANG` với icon Võ, Kiếm và kiếm starter.
- Item được chọn mẫu: `top_kiem_lv1_male`.
- Snapshot `RuntimeInventoryTryOnSnapshot` ghi flow, selected item, preview item, apply target và cancel target.
- Visual manifest thêm `runtimeInventoryTryOnSnapshot` để evidence không chỉ dựa vào hình.

## Quy tắc triển khai tiếp

Khi mở inventory thật, giữ nguyên các nguyên tắc đã khóa:

- Tóc/mắt/trang bị/vũ khí/pet có id riêng.
- Áo nam/nữ tách theo base torso; module không phụ thuộc giới tính dùng fit profile chung.
- Flow try-on phải hủy được mà không đổi loadout thật, apply mới thay đổi slot.
- Item icon có thể là sprite atlas sau này; runtime hiện tại chỉ dùng procedural marker để giữ branch nhẹ.
