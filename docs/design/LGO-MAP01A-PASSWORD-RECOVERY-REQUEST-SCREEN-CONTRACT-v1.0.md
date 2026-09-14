# Map01A Yêu cầu khôi phục mật khẩu — screen contract v1.0

Ngày khóa design: 2026-09-14
Trạng thái: **TECHNICAL_LAYOUT_LOCKED / OWNER_REVIEW_PENDING**

## Nguồn quyết định duy nhất

Canonical candidate duy nhất:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v9-password-recovery-request/01-password-recovery-request-CANONICAL-CANDIDATE.png`

- Canvas: `1672 × 941`.
- SHA-256: `c0576e5e078941e856e50a7d3885175480e9de30d3808c61d8c2d094ac7b3a48`.
- Kế thừa scene, logo, notification và utility rail từ Entry canonical; chỉ thay control card trung tâm.
- Đây là candidate đang dùng để triển khai technical checkpoint, chưa phải owner approval. Runtime screenshot là evidence đối chiếu, không phải design thứ hai.

## Flow và giới hạn

Khôi phục mật khẩu được tách thành ba screen tuần tự: `Yêu cầu khôi phục → Xác minh mã → Đặt mật khẩu mới`. Contract này chỉ sở hữu screen đầu tiên. Không đặt OTP hoặc mật khẩu mới trong cùng panel.

Người chơi mở screen từ `Quên mật khẩu` trên Entry, nhập tài khoản/email và bấm `Gửi hướng dẫn`. Recovery backend chưa có nên runtime chỉ kiểm tra input cục bộ và báo trung thực dịch vụ chưa kết nối; không giả gửi mã, không chuyển sang bước xác minh và không lưu dữ liệu nhạy cảm.

## Hierarchy và layout

1. Giữ toàn bộ Entry scene layers và brand block.
2. Một panel xanh đậm viền vàng ở giữa, thấp hơn Register vì chỉ có một field.
3. Title `KHÔI PHỤC MẬT KHẨU`, subtitle và một dòng hướng dẫn ngắn.
4. Một field thật `Tài khoản / Email` dùng icon account hiện hành.
5. Một primary CTA toàn chiều rộng `Gửi hướng dẫn`.
6. Một quiet action `Quay lại đăng nhập` ở đáy.

| Vùng | Rect mục tiêu trên 1672×941 | Quy tắc |
|---|---:|---|
| Panel | x `550..1122`, y `320..741` | một panel; dùng shared auth-flow base |
| Title/subtitle | y `352..464` | căn giữa; không wrap |
| Hướng dẫn | x `590..1082`, y `477..510` | tối đa một dòng ở canonical |
| Account field | x `590..1082`, y `516..571` | field thật; không dùng text trang trí |
| Primary CTA | x `590..1082`, y `592..657` | CTA duy nhất |
| Back action | y `671..716` | quiet action; trở về Entry |

PC `1600×900`, mobile landscape `1600×720` và tablet `1024×768` giữ cùng composition bằng scale/safe margin; không wrap/stack thành layout khác.

## State và interaction

| State | Kết quả |
|---|---|
| Default | field lấy account đang nhập ở Entry nếu có; status rỗng |
| Missing input | `Nhập tài khoản hoặc email để nhận hướng dẫn.` |
| Backend unavailable | `Dịch vụ khôi phục mật khẩu chưa kết nối. Vui lòng thử lại sau.` |
| Back/Escape | đóng recovery, phục hồi Entry; không đổi gameplay/class/pose/wardrobe |

Không có success giả, countdown, resend, OTP hoặc password field trong screen này.

## Base-first và asset budget

- `CongDongLamArrivalHud.PasswordRecovery.cs` chỉ sở hữu hierarchy/state/action.
- Panel, primary/back action và Entry text field phải đi qua shared auth-flow helpers trong `CongDongLamArrivalHud.Skin.cs`; Register cũng dùng cùng base để tránh hai hệ.
- Reuse icon account từ HUD atlas hiện hành `512×512`, khoảng `85 KB`, hiển thị `24 px`; không thêm runtime texture.
- Canonical external khoảng `2.6 MB`, không import vào Unity.

## Gate hoàn thành

- TDD khóa route Entry → Recovery → Entry, một field, một primary CTA, missing/backend feedback và không đổi gameplay state.
- Capture flag riêng, Player build sau C# change và evidence PC/mobile landscape/tablet được xem trực tiếp.
- Shared-skin/catalog/no-3D/no-source/change-budget/frozen diff đều sạch.
- Chỉ sau checkpoint này mới mở design gate `Xác minh mã`; không gộp screen.

## Kết quả technical checkpoint

- Focused recovery test `1/1`; full `TwoDCharacterRuntimeStateTests` `28/28` pass.
- Player `build/map01a-password-recovery-request-player-v1/LinhGioiOnline.app`: build `Succeeded`, `errors=0`, `warnings=44`, `totalSize=184137654`.
- Evidence `build/map01a-password-recovery-request-runtime-v1/{pc,mobile,tablet}/password-recovery-request.png` tại `1600×900`, `1600×720`, `1024×768`; mọi manifest ghi đúng scope, overlay và `usesOsMouseOrKeyboard=false`.
- Đã xem trực tiếp cả ba frame: cùng composition landscape, không wrap/stack/cắt/chồng; một field và một CTA duy nhất.
- Shared auth-flow panel/primary/back được Register và Recovery dùng chung; không thêm texture runtime hoặc hệ style thứ hai.
- Owner visual review vẫn là gate trước khi đổi candidate thành approved canonical và mở screen `Xác minh mã`.
