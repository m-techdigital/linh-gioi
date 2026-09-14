# Map01A character hub — canonical screen contract v1.0

Ngày khóa: 2026-09-14  
Trạng thái: **OWNER_APPROVED_DESIGN_SET / CHROME_V2_RUNTIME_REVIEW / OWNER_VISUAL_REVIEW_REQUIRED**

## Mục đích

Đây là nguồn quyết định duy nhất cho năm màn trong character hub. Mỗi màn có đúng một ảnh canonical; ảnh runtime/evidence chỉ dùng để đối chiếu, không trở thành design thứ hai. Không sửa code của màn kế tiếp khi màn hiện hành chưa qua gate layout và visual ở cả ba viewport.

## Một design canonical cho mỗi screen

Thư mục nguồn đã duyệt:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`

| Thứ tự | Screen | Canonical design duy nhất | Trạng thái triển khai |
|---:|---|---|---|
| 1 | Nhân vật | `01-nhan-vat-nam-tab-compact-APPROVED.png` | **CHROME_V2_REVIEW** |
| 2 | Rương đồ | `02-ruong-do-phan-loai-doc-tab-compact-APPROVED.png` | **CHROME_V2_REVIEW** |
| 3 | Kỹ năng | `03-ky-nang-five-tab-APPROVED.png` | **CHROME_V2_REVIEW** |
| 4 | Tiềm năng | `04-tiem-nang-five-tab-APPROVED.png` | **CHROME_V2_REVIEW** |
| 5 | Linh thú | `05-linh-thu-five-tab-APPROVED.png` | **CHROME_V2_REVIEW** |

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

## Shared chrome v2

- Pack `map01a-character-hub-chrome-v2` sở hữu bảy texture runtime: shell, panel, tab idle/selected, action xanh/vàng và close bát giác. Manifest ghi hash của cả năm canonical và hash từng asset; generator không crop board tham chiếu.
- Shell dùng nền navy, top rail/diamond, viền old-gold nhiều lớp và corner filigree; các panel nội dung dùng một surface họa tiết chung. Mọi tab và action cùng loại gọi helper trong `CongDongLamArrivalHud.Skin.cs`, không tự dựng skin tại partial.
- Phân cấp frame bắt buộc: chỉ shell dùng full filigree; workspace và detail-right dùng viền section mờ; icon/card nằm trong panel dùng inset frame một lớp. Không lặp full filigree ở phần tử con vì sẽ tạo cảm giác box chồng box. Cả ba cấp đều dùng shared helper, không tự khai báo viền trong từng screen.
- Button có chrome texture giữ border trang trí được vẽ bên trong texture và đặt outer/CSS border về `0`; không chồng hai hệ border. Tab chính cao 48 px, chữ 17 px; close 52 px, chữ 27 px tại canonical scale để không lấn hierarchy của title/content.
- Mở modal fade trong `190 ms`, backdrop `160 ms`; đổi tab/content fade trong `130 ms`, tab selected pulse `150 ms`. Capture evidence phải đợi ít nhất `240 ms` để đánh giá trạng thái ổn định thay vì frame giữa animation.
- Evidence review hiện hành: `build/map01a-five-tab-chrome-runtime-v3/{pc,mobile,tablet}/`. Player tương ứng: `build/map01a-five-tab-chrome-player-v3/LinhGioiOnline.app`.

## Contract từng screen

### Binding 5 class trên cùng layout

- Character Hub dùng đúng một selector gọn trong title row, thứ tự `Võ → Kiếm → Pháp → Cơ → Linh`; selector không tạo tab thứ sáu, không đổi cấu trúc hai cột và luôn dùng được ở cả năm tab.
- Đổi class chỉ thay data/preview trong cùng component tree. Tab đang mở được giữ nguyên và main/detail cập nhật trong cùng một lượt refresh.
- Mỗi class giữ snapshot riêng cho slot đang chọn, trạng thái tháo/mặc và cấp của mười món. Chuyển class rồi quay lại phải khôi phục đúng snapshot; không dùng chung state vô tình giữa hai class.
- Selector Character Hub chỉ dùng catalog source-pose đã được launcher nạp và cùng actor với map. Class thiếu pack hợp lệ không được hiện; tuyệt đối không fallback sang `*MixedLoadoutFitPreview`, registered-outfit hoặc atlas/rig avatar cũ, và không tạo source art mới.
- Kỹ năng/tiềm năng/linh thú là profile hiển thị riêng theo class trong cùng base; asset chưa có nguồn class-specific phải dùng icon provenance-backed dùng chung và copy trung thực, không giả thành asset final riêng của class.
- Topology ba màn được khởi tạo đúng một lần: Skill giữ graph `3×3` và bốn ô trang bị; Tiềm năng giữ một vector base gồm vòng ngoài, đường nối, core và năm node; Linh thú giữ preview/roster/detail. Đổi class chỉ bind icon/text/value/state vào control có sẵn, không `RemoveFromHierarchy`, không dựng lại panel và không tạo topology riêng theo class.

### 1. Nhân vật — layout locked

- Main workspace: full-body actor ở giữa, năm slot mỗi bên, identity + Lv/LC + HP/MP ở đáy.
- Detail-right: icon món đang chọn, tên/level/trạng thái, thuộc tính có dữ liệu thật, set/fit có dữ liệu thật, action `Tháo` và `Khóa` theo state hiện hành.
- Chọn bất kỳ slot nào phải đổi cùng một detail-right; không mở panel chi tiết thứ hai.
- Asset gate: actor dùng runtime source hiện hành và không sửa class/pose/wardrobe/camera/scale. Atlas UI riêng `map01a-character-equipment-icons-v1` chứa đủ mười thumbnail, có alpha và manifest/hash/provenance; vẫn giữ `DRAFT_RUNTIME_REVIEW` chờ owner duyệt mỹ thuật.

### 2. Rương đồ

- Main workspace: category rail dọc, capacity/search/sort row, grid năm cột và footer action.
- Detail-right giữ cùng component/hình học với màn Nhân vật.
- Chọn item cập nhật detail-right. Filter/search chỉ thao tác trên dữ liệu hiện có; không tạo item/currency/rarity giả.

### 3. Kỹ năng

- Main workspace: category rail dọc, skill graph ba hàng, equipped-skill strip ở đáy.
- Detail-right: icon, cấp, mô tả, thông số và action; progression chưa có contract phải giữ read-only rõ ràng.

### 4. Tiềm năng

- Main workspace: một diagram kinh mạch trung tâm và điểm còn lại ở đáy; không thay bằng card grid.
- Một vector topology base dùng chung vẽ sẵn vòng ngoài, các vòng đồng tâm, năm đường nối, core, ba lớp khung tròn, ô giá trị, ô cộng điểm và dấu cộng của đủ năm node theo canonical. Button/node phía trên là interaction overlay trong suốt; profile class chỉ cấp icon, tên, giá trị, mô tả và trạng thái chọn. Năm định nghĩa thuộc tính dùng một catalog bất biến chung; chỉ recommendation/selection/state khác nhau mới nằm theo class. Không để từng class hoặc từng button tự dựng lại khung, ô hay dấu cộng của node.
- Detail-right: node đang chọn, hiệu quả hiện tại/kế tiếp, chi phí và action bị khóa khi chưa có contract ghi state.

### 5. Linh thú

- Preview, thanh thân mật/tăng trưởng, roster bốn ô, inspector và đúng hai hàng kỹ năng được dựng một lần trên shared component tree. `CharacterHubSpiritPetPreview` chỉ bind art có provenance, tên/cấp, badge, chỉ số, hai skill icon/name/level/description và trạng thái vào các control có sẵn; không ghép toàn bộ skill thành một chuỗi text hoặc dựng lại row theo class.

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

Shared geometry của năm tab giữ đúng contract; feedback owner đã mở lại visual chrome và evidence cũ không còn dùng để claim hoàn thành. Chrome v2 đã được review nội bộ trên đủ ba viewport tại `build/map01a-five-tab-chrome-runtime-v3/{pc,mobile,tablet}/`, không wrap/stack/cắt/chồng và đang chờ owner visual review. Không mở screen ngoài hub trước gate này. Không resume class/pose/wardrobe/source và không rollback code class.
