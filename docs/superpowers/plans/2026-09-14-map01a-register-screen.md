# Map01A Register Screen Implementation Plan

**Goal:** Tạo màn Đăng ký bám canonical, tách khỏi Entry nhưng dùng chung scene/base, xác thực local trung thực và quay lại đúng Entry.

**Architecture:** Một partial sở hữu hierarchy/state; Entry chỉ mở route. Shared Skin sở hữu metrics/style. Capture harness mở screen bằng flag riêng, không dùng input hệ điều hành.

**Spec:** `docs/design/LGO-MAP01A-REGISTER-SCREEN-CONTRACT-v1.0.md`

## Constraints

- Một canonical design duy nhất; không import mockup vào runtime.
- Không tạo account giả hoặc đổi server/character/class/gameplay state.
- PC/mobile landscape/tablet giữ cùng composition.
- Không sửa class/pose/wardrobe/source hoặc frozen surfaces.

## Task 1 — khóa behavior bằng test

- [x] Viết focused EditMode test cho route Entry → Register → Entry.
- [x] Khóa ba field, hai reveal action, agreement toggle và đúng một primary CTA.
- [x] Khóa missing/mismatch/agreement/backend-unavailable feedback và ghi nhận RED.

## Task 2 — shared-base runtime

- [x] Thêm `CongDongLamArrivalHud.Register.cs` và route từ Entry.
- [x] Bổ sung role helper vào shared Skin trước khi bind hierarchy/state.
- [x] Escape/back trở về Entry; không thay gameplay state.
- [x] Chạy focused và full `TwoDCharacterRuntimeStateTests` đến GREEN.

## Task 3 — Player evidence và checkpoint

- [x] Thêm capture flag/frame/manifest riêng cho Register.
- [x] Build một Player, capture PC/mobile landscape/tablet và xem trực tiếp cả ba ảnh.
- [x] Gom sai lệch layout lớn thành một lượt sửa; không lặp micro-pixel/build loop.
- [x] Cập nhật catalog/state/next action; chạy toàn gate, commit/push rồi mới chuyển Quên mật khẩu.
