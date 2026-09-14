# Map01A Entry/Login — implementation plan

Canonical duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v5-entry/01-entry-login-CANONICAL.png`. Baseline runtime: `build/map01a-entry-current-audit-v2/entry-login.png`. Entry/Login là screen active duy nhất; không sửa screen khác trong batch.

## Audit toàn màn trước code

| Vùng | Canonical | Baseline | Batch xử lý |
|---|---|---|---|
| Composition | city art + logo + form/card giữa + notification trái + utility rail phải | map art đúng sản phẩm nhưng logo/form nhỏ, thiếu hierarchy premium | giữ map/art runtime; sửa shell scale/position theo canonical, không chỉnh camera |
| Auth flow | 2 field, option row, login/register cùng hàng | đủ field thật nhưng label/title thừa, control rất nhỏ | bỏ subtitle kỹ thuật, gom form thành card 578/420 theo canvas |
| CTA | một primary `Đăng nhập`, secondary `Đăng ký` | `Bắt đầu` rời card và tạo luồng hai bước mơ hồ | bỏ giant start; login là CTA duy nhất, backend unavailable phản hồi inline |
| Server | row card 497/67, name/state/chevron | row có nút `Đổi máy chủ`, mật độ giống debug form | chuyển về server summary row; routing chưa mở giữ disabled/feedback thật |
| Notification/rail | card gọn trái dưới, bốn action dọc phải | notification quá nhỏ; rail đã có bốn action | scale theo canonical và dùng shared bases hiện hành |
| Viewport | một landscape composition | chưa có evidence ba profile từ cùng batch | capture PC/mobile/tablet một lượt sau build |

## Trình tự thực hiện

1. TDD RED khóa một CTA, compact two-action row, server summary và cùng composition ở ba viewport.
2. Sửa shared entry primitives/geometry trong `CongDongLamArrivalHud.Skin.cs` và layout constants/helper chung.
3. Sửa hierarchy/state trong `CongDongLamArrivalHud.Entry.cs`; giữ credential fields, password mask và remember-account logic hiện hành.
4. Chạy targeted EditMode; gom lỗi source rồi build Player một lần.
5. Capture default ở PC/mobile/tablet cùng Player; capture state thiếu credential/backend unavailable nếu capture API hỗ trợ mà không dùng OS input.
6. Review ảnh theo contract; gom mọi sai lệch lớn thành tối đa một correction batch rồi capture lại phần bị ảnh hưởng.
7. Chạy full test/validators/frozen audit, cập nhật PROJECT-STATE/NEXT-ACTION/catalog, commit và push checkpoint khi gate thật sự đạt.

Không tạo art/class/source mới trong batch. Không quay lại hai demo login cũ như nguồn quyết định.

## Gate result — 2026-09-14

- Canonical mới đã được audit và giữ làm một nguồn quyết định duy nhất.
- Runtime bỏ CTA `Bắt đầu`, dùng một primary `Đăng nhập`, field thật, password reveal, remember-account, server summary và status line trong cùng card.
- Full EditMode `24/24`; Player build `errors=0`; evidence PC/mobile landscape/tablet đã xem, không wrap/stack/clipping.
- UI catalog, shared skin, no-3D, no-source, change budget và frozen diff đều pass. Entry/Login được khóa; không vi chỉnh lại khi không có regression mới.
