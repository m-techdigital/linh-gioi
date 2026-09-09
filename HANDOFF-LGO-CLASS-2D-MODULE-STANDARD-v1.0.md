# Handoff chuẩn module class 2D v1.0 — Võ

Ngày 2026-09-09. Owner thu hẹp phạm vi hiện tại về class Võ; bốn spec class còn lại trong prompt ban đầu được hoãn, không tạo file giả đủ danh sách. Quy ước chung giữ 5 class ID, nhưng checker chỉ yêu cầu spec/pack Võ.

## Quyết định và hiện trạng

Đã review trực tiếp 23 PNG trong `/Users/minhdc/Projects/2D/Vo`; chọn outfit nam/nữ `22_11_44 … (2)/(3)`, grid nam/nữ `22_13_40 … (3)/(2)`, module map `22_13_40 … (1)`, mood VFX `lgo-vo-ky-nang-chien-dau-vfx.png`. Tên đầy đủ, quyết định từng file và SHA-256 ở `docs/art/classes/vo/LGO-VO-2D-MODULE-SPEC-v1.0.md`.

Không có board đạt chuẩn item rời: còn ngón tay, áo/giáp và quần/đai gộp; bản mới nhất còn lệch xanh Lv100. Chọn reference không đồng nghĩa duyệt sạch mọi ô. Thiết kế sửa: quyền khí ở bàn tay, bảo hộ ở cẳng tay; tóc không đầu; áo/giáp/đai độc lập; Võ đen–đỏ–vàng–ngà, aura cam/vàng tách khỏi item. Chưa sinh ảnh chỉnh mới.

Bộ bàn giao: 5 tài liệu chuẩn chung tại `docs/art/LGO-CLASS-*` và checklist, 1 spec Võ, 1 checker, handoff này và cập nhật NEXT-ACTION. Ảnh gốc giữ nguyên ngoài repo, không đưa lại source images vào branch. Các thay đổi Unity/AGENTS/reference đang có trước batch không thuộc checkpoint này.

## Validation và giới hạn

Chạy từ repo root:

```sh
pwd
git --no-pager status --short --untracked-files=all
git --no-pager diff --check
PYTHONPYCACHEPREFIX=build/pycache python3.12 -m py_compile tools/validate_class_2d_module_spec.py
python3.12 tools/validate_class_2d_module_spec.py
python3.12 build/class-2d-review/check_validator.py
python3.12 tools/validate_class_2d_module_spec.py --require-assets
python3.12 tools/validate_2d_branch_no_source_images.py
python3.12 tools/report_lgo_change_budget.py
```

`--require-assets` phải trả khác 0 khi chưa có ảnh; đây là evidence thiếu pack, không lỗi cần che. Regression harness local kiểm tài liệu thiếu, phrase thiếu, tên/folder sai, thiếu slot/board và header PNG giả trên fixture tạm; không chứng nhận art. Không chạy Unity cho batch docs/checker. Kết quả thực lưu `build/class-2d-review/validation.log`, không commit build artifacts.

Kết quả đã chạy: diff check và py_compile đạt; checker đọc 7/7 tài liệu đạt; 15 regression cases đạt; gate no-source-images đạt. Gate --require-assets trả 1 vì thiếu 226/226 ảnh như dự kiến. Change budget báo WARN do tổng workspace 34 file có cả thay đổi có sẵn; checkpoint này giới hạn 8 file art/checker/handoff, không gom Unity. Owner xác nhận tab khác đang xử lý world map/hệ thống; ghi chú NEXT-ACTION dùng chung để ngoài commit, không reset/stash thay đổi tab khác.

## Gate kế tiếp

Thiếu demo clean side-view lv001 nam/nữ để kiểm tách 10 slot trên hai base; kế tiếp làm demo và review trước lv050/220 item. Gate ingest riêng: `tools/validate_2d_branch_no_source_images.py` hiện cấm ảnh trong source, không tắt hoặc nới gate tự động. Chưa có runtime/production asset để import. Không tiếp gameplay/task class khác trong scope owner vừa thu hẹp.

## Non-claims

- no gameplay implementation
- no production art claim
- no runtime asset claim
- no protocol changes
- no GameData schema changes

Không sửa server runtime, `protocol/**`, `gamedata/schemas/**`, `docs/adr/**` hoặc `client/Unity/Assets/Game/UI/design-tokens.json` trong batch. Commit chỉ tài liệu/checker thuộc batch, không push ngoài supervisor.
