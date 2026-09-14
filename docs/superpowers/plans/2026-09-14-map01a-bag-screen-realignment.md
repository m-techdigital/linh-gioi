# Map01A Rương đồ — implementation plan

Canonical screen duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/02-ruong-do-phan-loai-doc-tab-compact-APPROVED.png`.
Baseline Player: `build/map01a-bag-screen-baseline-v1/bag.png`. Screen active duy nhất là Rương đồ; Nhân vật đã khóa, Kỹ năng/Tiềm năng/Linh thú không sửa.

## Layout translation đã chốt trước code

- Giữ shared shell 1098×724, rail năm tab 992×50 và body hai cột 600/448, gap 12 trên cả PC/mobile/tablet.
- Main workspace chia rail phân loại dọc 108 và vùng grid còn lại. Rail có năm mục, icon + nhãn, cùng một base và selected state rõ.
- Toolbar grid chỉ giữ capacity, search và sort affordance. Badge `10/10 đang mặc` không chiếm toolbar vì thuộc màn Nhân vật.
- Grid là năm cột ô vuông khoảng 88–92 px; hình item là thông tin chính. Tên đầy đủ chuyển sang tooltip/detail-right, số lượng là badge góc. Không dùng card chữ cao 116 px.
- Grid dùng atlas UI equipment đã review và atlas item Map01A hiện hành; không crop sprite paper-doll tối làm icon Rương đồ.
- Footer chỉ giữ `Sắp xếp` và `Bán nhanh` theo canonical design. `Tách` không hiển thị khi chưa có interaction contract.
- Detail-right tái sử dụng component Nhân vật, luôn cập nhật khi chọn item; không tạo detail panel thứ hai.

## Gate

1. TDD khóa ô vuông năm cột, atlas icon riêng, rail dọc và action footer.
2. Triển khai qua shared Skin/base rồi bind state ở Inventory partial.
3. Build một Player, capture default/selected/search ở 1600×900, 1600×720 và 1024×768.
4. Visual audit: không card chữ dọc, không thumbnail tối, không clipping/overlap, rail/grid/detail đúng thứ bậc.
5. Validators, frozen diff, checkpoint commit/push; chỉ sau đó mới chuyển Kỹ năng.

## Kết quả gate

- Runtime dùng rail phân loại icon dọc, ô vuông 92 px theo năm cột, search căn giữa, footer hai action và detail-right dùng chung; không còn card item dạng hàng chữ cao.
- Atlas `map01a-bag-category-icons-v1` có năm icon, alpha/manifest/hash/provenance và trạng thái `DRAFT_RUNTIME_REVIEW`; equipment tile dùng atlas trang bị riêng đã có.
- Player `build/map01a-bag-screen-player-v3/LinhGioiOnline.app`; evidence hiện hành `build/map01a-bag-screen-runtime-v4/{pc,mobile,tablet}/` gồm default/search/selected, không dùng chuột hoặc bàn phím hệ điều hành.
- Đã đối chiếu trực tiếp ba `bag.png` với canonical design: giữ hai cột, không stack/reflow, không clipping/overlap; layout screen được khóa. Art pack vẫn chờ owner review mỹ thuật.
