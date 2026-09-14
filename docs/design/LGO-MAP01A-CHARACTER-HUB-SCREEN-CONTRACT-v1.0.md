# Map01A character hub — canonical screen contract v1.0

Ngày khóa: 2026-09-14  
Trạng thái: **OWNER_APPROVED_DESIGN_SET / RUNTIME_REALIGN_REQUIRED**

## Mục đích

Đây là nguồn quyết định duy nhất cho năm màn trong character hub. Mỗi màn có đúng một ảnh canonical; ảnh runtime/evidence chỉ dùng để đối chiếu, không trở thành design thứ hai. Không sửa code của màn kế tiếp khi màn hiện hành chưa qua gate layout và visual ở cả ba viewport.

## Một design canonical cho mỗi screen

Thư mục nguồn đã duyệt:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`

| Thứ tự | Screen | Canonical design duy nhất | Trạng thái triển khai |
|---:|---|---|---|
| 1 | Nhân vật | `01-nhan-vat-nam-tab-compact-APPROVED.png` | **ACTIVE** — phải hoàn thiện trước |
| 2 | Rương đồ | `02-ruong-do-phan-loai-doc-tab-compact-APPROVED.png` | Chờ Nhân vật đạt gate |
| 3 | Kỹ năng | `03-ky-nang-five-tab-APPROVED.png` | Chờ Rương đồ đạt gate |
| 4 | Tiềm năng | `04-tiem-nang-five-tab-APPROVED.png` | Chờ Kỹ năng đạt gate |
| 5 | Linh thú | `05-linh-thu-five-tab-APPROVED.png` | Chờ Tiềm năng đạt gate |

Các demo cũ trong thư mục cha là tài liệu lịch sử. Chúng không được dùng để quyết định layout hoặc mở lại hệ tab/cột cũ.

## Canvas và layout chung đã khóa

Canonical canvas của cả năm ảnh là `1672 × 941`. Runtime dùng cùng một composition trên PC, tablet và mobile landscape; thay đổi kích thước bằng scale của toàn UI theo viewport, không đổi thành bố cục xếp dọc.

| Vùng | Rect canonical (x, y, w, h) | Quy tắc |
|---|---:|---|
| Modal shell | `286, 127, 1098, 724` | Căn giữa ngang, lệch xuống 18 px so với tâm canvas để nhường HUD trên |
| Title row | `318, 139, 1040, 42` | Tiêu đề trái; close phải; không subtitle kỹ thuật |
| Five-tab rail | `318, 190, 992, 52` | Năm tab ngắn, cùng base; close không nằm trong rail |
| Content body | `307, 251, 1060, 599` | Luôn hai cột; không wrap/stack |
| Main workspace | `307, 251, 600, 599` | Preview/grid/diagram theo từng screen |
| Column gap | `907, 251, 12, 599` | Một gutter chung |
| Detail-right | `919, 251, 448, 599` | Chi tiết/chọn/action luôn ở bên phải |

Tỷ lệ chính: shell `65.7% × 76.9%` canvas; body main/detail là `600/448`, gap `12`. Padding shell xấp xỉ 20 px. Các số này là layout geometry; font, icon và copy chỉ được hiệu chỉnh sau khi rect tổng và hierarchy đã đạt.

## Chính sách theo viewport

| Profile evidence | Kích thước | Hành vi bắt buộc |
|---|---:|---|
| PC | `1600 × 900` | Scale từ canvas canonical, giữ hai cột và đủ chiều cao body |
| Mobile landscape | `1600 × 720` | Giữ nguyên composition hai cột; thu phóng đồng nhất, không xuống dòng hoặc đẩy detail xuống dưới |
| Tablet | `1024 × 768` | Giữ nguyên composition hai cột; căn giữa, phần nền dư thuộc map backdrop |

Panel UI dùng reference `1672 × 941`, `ScaleWithScreenSize`, `MatchWidthOrHeight`, match `0.5`. Mọi thay đổi dành riêng cho touch chỉ liên quan input/hit state; không được thay hierarchy, thứ tự vùng hoặc tỷ lệ cột của modal.

## Contract từng screen

### 1. Nhân vật — active

- Main workspace: full-body actor ở giữa, năm slot mỗi bên, identity + Lv/LC + HP/MP ở đáy.
- Detail-right: icon món đang chọn, tên/level/trạng thái, thuộc tính có dữ liệu thật, set/fit có dữ liệu thật, action `Tháo` và `Khóa` theo state hiện hành.
- Chọn bất kỳ slot nào phải đổi cùng một detail-right; không mở panel chi tiết thứ hai.
- Asset gate: actor dùng runtime source hiện hành và không sửa class/pose/wardrobe/camera/scale. Mười equipment thumbnail cần một atlas UI 2D riêng đã review; không crop tối trực tiếp từ full-body atlas làm final.

### 2. Rương đồ

- Main workspace: category rail dọc, capacity/search/sort row, grid năm cột và footer action.
- Detail-right giữ cùng component/hình học với màn Nhân vật.
- Chọn item cập nhật detail-right. Filter/search chỉ thao tác trên dữ liệu hiện có; không tạo item/currency/rarity giả.

### 3. Kỹ năng

- Main workspace: category rail dọc, skill graph ba hàng, equipped-skill strip ở đáy.
- Detail-right: icon, cấp, mô tả, thông số và action; progression chưa có contract phải giữ read-only rõ ràng.

### 4. Tiềm năng

- Main workspace: một diagram kinh mạch trung tâm và điểm còn lại ở đáy; không thay bằng card grid.
- Detail-right: node đang chọn, hiệu quả hiện tại/kế tiếp, chi phí và action bị khóa khi chưa có contract ghi state.

### 5. Linh thú

- Main workspace: hero art lớn, identity/progress và roster ngang ở đáy.
- Detail-right: portrait, thuộc tính, kỹ năng và action; dùng asset provenance-backed hiện hành, không letterbox canvas vuông.

## Quy trình bắt buộc cho từng screen

1. **Chọn đúng một screen active.** Ghi canonical file, scenario, state và interaction vào `NEXT-ACTION.md`.
2. **Audit toàn ảnh trước khi sửa.** So theo thứ tự shell → title/tab → body/cột → hierarchy nội vùng → asset → typography/copy. Gom sai lệch thành một batch; không chỉnh từng pixel rời.
3. **Khóa asset trước runtime wiring.** Liệt kê asset đã có, thiếu và provenance. Asset thiếu phải có design board/brief rồi được review; không dùng emoji, hình học tạm, crop tối hoặc ảnh ngẫu nhiên làm final.
4. **Sửa base trước.** Geometry/skin/tab/detail/card lặp lại phải nằm trong shared base/`CongDongLamArrivalHud.Skin.cs`; partial chỉ tạo hierarchy và bind data/state/action.
5. **TDD cho contract nhìn thấy.** Test khóa một design source, canvas policy, hai cột và component chung. Test không được thay thế visual review.
6. **Triển khai trọn screen trong một batch.** Hoàn tất toàn bộ vùng lớn và asset của screen rồi mới build; không build sau mỗi chỉnh nhỏ.
7. **Capture một lượt ba profile.** Chụp PC, mobile landscape và tablet bằng Player, đủ default/selected/action state của chính screen active.
8. **Đối chiếu theo checklist cố định.** Layout tổng, tỷ lệ vùng, clipping/overlap, độ rõ asset, trạng thái chọn, detail-right và khả năng thao tác. Mọi lỗi được gom thành một lượt sửa kế tiếp.
9. **Gate chuyển màn.** Chỉ chuyển screen khi test pass, validators pass, frozen diff sạch và ảnh ba profile đã được xem. Nếu còn sai layout/art rõ, trạng thái là `FIX_REQUIRED` hoặc `CONTINUE`, không ghi hoàn thành.

## Gate hiện hành

Chỉ screen **Nhân vật** được phép sửa. Rương đồ/Kỹ năng/Tiềm năng/Linh thú giữ nguyên cho đến khi Nhân vật có evidence mới đạt contract này. Không resume class/pose/wardrobe/source và không rollback code class.
