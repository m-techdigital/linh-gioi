# Map01A Password Recovery Request Screen Plan

**Goal:** Triển khai screen đầu tiên của flow khôi phục mật khẩu theo một canonical candidate, dùng chung Entry auth base và phản hồi trung thực khi backend chưa có.

**Architecture:** Một partial sở hữu hierarchy/state. Entry chỉ mở route. Register và Recovery cùng dùng shared auth-flow helpers. Capture harness mở screen bằng flag riêng, không dùng input hệ điều hành.

**Spec:** `docs/design/LGO-MAP01A-PASSWORD-RECOVERY-REQUEST-SCREEN-CONTRACT-v1.0.md`

## Task 1 — behavior contract bằng test

- [x] Thêm focused EditMode test cho Entry → Recovery → Entry.
- [x] Khóa một account field, đúng một primary CTA và hai feedback state.
- [x] Khóa việc không mở OTP/reset screen hoặc thay gameplay/class state.

## Task 2 — shared-base runtime

- [x] Tách auth-flow panel/primary/back base trong Skin và cho Register reuse.
- [x] Thêm recovery partial, route từ Entry và Escape/back behavior.
- [x] Chạy focused rồi full `TwoDCharacterRuntimeStateTests` đến GREEN.

## Task 3 — evidence và checkpoint

- [x] Thêm capture flag/frame/manifest và catalog coverage.
- [x] Build một Player; capture/view PC, mobile landscape, tablet.
- [x] Chạy source/frozen/change-budget gates, cập nhật state/goal rồi commit/push.
