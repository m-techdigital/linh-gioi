# Linh Giới Online — Audit tái sử dụng sandbox chuẩn hóa module class 2D v1

Ngày audit: 2026-09-10  
Sandbox được audit: local `feature/2d` tại `efa46a8`  
Nhánh tích hợp hiện hành: `origin/feature/2d` tại `bb20119`

## Kết luận tích hợp

Không cherry-pick nguyên sandbox. Hai nhánh tách nhau từ `21b4502`; nhánh hiện hành có 54 commit riêng, sandbox class có 30 commit riêng. Diff toàn nhánh gồm 122 file, trong đó có 48 file bị xóa. Lấy nguyên nhánh có nguy cơ xóa runtime Map 01A và code mới.

Tái sử dụng theo file/contract sau khi Map 01A đạt gate. Ảnh owner trong source pack là visual authority; output procedural và primitive chỉ được dùng làm fixture hoặc contract QA.

## Phần giữ lại

| Nguồn | Quyết định | Giá trị được giữ | Gate còn thiếu |
|---|---|---|---|
| `d3ac3be` | Port chọn lọc sau map | Spec 5 class, 10 slot cố định, validator theo class, test validator | Thu scope runtime về Lv1–30; validator chỉ chứng minh tài liệu/tên file |
| `0b85c4f` | Port chọn lọc sau map | Prompt bundle hiểu ownership/palette/weapon riêng của Võ, Kiếm, Pháp, Cơ, Linh | Không sinh Lv31–100 trong phase hiện tại; output phải qua review ảnh |
| `4648ccf` | Giữ | Review starter art và anchor rõ; utility normalize canvas sửa alpha bbox | Demo mặc sẵn vẫn là draft, không phải 10 module rời |
| `f50d516` | Giữ và tiếp tục | Mask polygon, trim/restore, atlas lossless, ba slot Võ `inner_top`, `arm_guard`, `main_weapon` | Cổ tay/skin exclusion, back pieces, 7 slot còn lại, nữ, motion |
| `efa46a8` | Giữ | Despill cạnh alpha, không đổi alpha/canvas | Cần visual QA trên nền sáng/tối; không cứu được contour sai |
| Uncommitted ground alignment | Giữ | Căn nam/nữ cùng ground bằng translation `+29 px`, scale 1, giữ pixel | Chưa chốt canonical base/rig; equipment nữ phải nhận cùng offset |
| Uncommitted chroma key | Chỉ giữ như pre-process | Tạo alpha sơ bộ từ nền xanh | Không phải segmentation production; cần contour, despill và review |
| 25 commit preview/generator đầu | Giữ công cụ QA cần thiết | Naming, prompt/export manifest, alpha/anchor/bbox checks, remove-slot/mix/motion fixture | Hình procedural không được dùng làm visual authority hoặc final art |

## Phần đã bảo toàn

- Bundle lịch sử đầy đủ: `/tmp/lgo-class-module-standardization-stopped-efa46a8.bundle`, SHA-256 `9576608245eb879a23a403187d5c3cad0c1b7daae8368bbe1281fb5fb201ec30`.
- Patch uncommitted class: `/tmp/lgo-main-stopped-class-task-backup/tracked.patch`.
- Mask pilot untracked: `/tmp/lgo-main-stopped-class-task-backup/vo/vo-male-lv001-mask-pilot-v1.json`.
- WIP hình Võ đã copy theo SHA vào `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/class-work-in-progress/vo-lv001`.

Không sửa, reset hoặc stash main checkout trong quá trình audit.

## Nguồn hình được kế thừa

Source pack hiện có 87 entry: 10 map canonical, 2 map context, 37 overview/base/class, 26 detailed redraw source và 12 Võ WIP. Hai mươi sáu ảnh chi tiết được chọn trực tiếp theo spec của sandbox:

- Võ: outfit nam/nữ, module map, grid nam/nữ, VFX mood;
- Kiếm: module map, grid nữ/nam, silhouette + VFX mood;
- Pháp: module map, grid nam/nữ, outfit nam/nữ, VFX mood;
- Cơ: module map, grid nam/nữ, silhouette + weapon/VFX;
- Linh: module map, grid nam/nữ, outfit nam/nữ, VFX mood.

Các ảnh này mang trạng thái `REDRAW_SOURCE_ONLY`. Quan sát trực tiếp cho thấy nhiều ô còn dính đầu/mặt/da/ngón/torso, gộp áo–giáp–đai hoặc bake glow. Vì vậy không crop thẳng thành runtime item. Board Cơ bị đặt nhầm trong thư mục Linh có SHA trùng byte đã bị loại khỏi tập Linh và ghi trong manifest.

## Bằng chứng kỹ thuật

- Python compile: 5 tool class PASS.
- Unit test: 28/28 PASS cho spec validator, prompt ownership, normalize canvas, masked pilot và despill.
- `validate_class_2d_module_spec.py`: PASS 13/13 tài liệu, scope đủ 5 class; output tự xác nhận asset completeness/alpha/rig/runtime chưa được gate.
- Test uncommitted: chroma key self-test PASS; normalize/ground alignment 7/7 PASS.

## Thứ tự sử dụng

1. Hoàn thiện Map 01A từ 10 nguồn map canonical và kiểm trong Player.
2. Port tối thiểu các spec/tool cần cho Võ Lv1–30 lên đúng HEAD mới; không port xóa file hoặc state cũ.
3. Dùng base đã căn và ba slot masked pilot làm điểm bắt đầu; sửa lỗi hiện có trước khi tạo bảy slot còn lại.
4. Chứng minh Võ nam/nữ: 10 slot, equip/unequip, anchor/pivot/sort, idle/walk/run/jump/basic attack và `Liên Quyền` trong cùng Player scene.
5. Khi Võ đạt gate mới nhân contract sang Kiếm/Pháp/Cơ/Linh Lv1–30.

