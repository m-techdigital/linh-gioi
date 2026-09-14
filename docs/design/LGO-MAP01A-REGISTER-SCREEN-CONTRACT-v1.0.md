# Map01A Đăng ký tài khoản — canonical screen contract v1.0

Ngày khóa design: 2026-09-14
Trạng thái: **LAYOUT_LOCKED / PLAYER_EVIDENCE_REVIEWED**

## Nguồn quyết định duy nhất

Canonical design duy nhất:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v8-register/01-register-account-CANONICAL.png`

- Canvas: `1672 × 941`.
- SHA-256: `a63c51012a184f3ad1752a351aeced21cbec639d0057272926c00952d26febfb`.
- Design kế thừa scene, brand, utility rail và notification của Entry canonical; chỉ thay vùng auth card.
- Output ImageGen trung gian nằm ngoài thư mục design. Canonical được pad thêm đúng một cột pixel biên phải để khớp canvas `1672 × 941`; không resize hoặc thay nội dung.
- Runtime screenshot là evidence đối chiếu, không phải design thứ hai.

## Scenario, dữ liệu và giới hạn

Screen mở từ action `Đăng ký` của Entry. Người chơi nhập tài khoản/email, mật khẩu, nhập lại mật khẩu và đồng ý điều khoản trước khi gửi. Action quay lại trả về Entry mà không đổi server, nhân vật, class hoặc Map01A state.

Auth backend chưa được mở trong runtime Map01A. Screen chỉ xác thực cục bộ các điều kiện hiển thị rồi báo trung thực rằng dịch vụ đăng ký chưa kết nối; không tạo tài khoản giả, không lưu mật khẩu và không chuyển sang Character Select.

## Hierarchy và layout canonical

1. Giữ nền Đông Lâm, logo/slogan, nhân vật trang trí, utility rail và notification card của Entry.
2. Một panel xanh đậm viền vàng nằm giữa, cao hơn login card nhưng không đè logo hoặc notification.
3. Title/subtitle riêng cho đăng ký.
4. Ba field cùng base: tài khoản/email, mật khẩu, nhập lại mật khẩu; hai field mật khẩu có reveal action.
5. Một hàng đồng ý điều khoản có trạng thái unchecked/checked rõ.
6. Một primary CTA toàn chiều rộng `Tạo tài khoản`.
7. Một navigation action yên tĩnh `Quay lại đăng nhập` ở đáy panel.

| Vùng | Rect mục tiêu trên 1672×941 | Quy tắc |
|---|---:|---|
| Panel | x `550..1122`, y `313..815` | dùng shared Entry shell; một panel duy nhất |
| Title/subtitle | y `337..410` | căn giữa; không wrap |
| Fields | x `589..1082`, y `423..613` | ba field cao khoảng 54 px, gap 14–16 px |
| Agreement | x `591..1020`, y `632..663` | target click tối thiểu 44 px; copy một dòng |
| Primary CTA | x `589..1082`, y `680..741` | CTA duy nhất, vàng, không có button cạnh bên |
| Back action | y `762..800` | secondary quiet; text rõ, không dùng glyph/icon tạm |

PC, mobile landscape và tablet giữ cùng composition bằng scale/safe margin. Không stack field/action sang layout khác, không cắt CTA và không thu font xuống dưới mức đọc được.

## State và interaction

| State | Hiển thị | Interaction |
|---|---|---|
| Default | ba field rỗng, agreement off, status yên tĩnh | field focus được; password bị mask |
| Reveal | icon/action đổi trạng thái | mỗi password field reveal độc lập |
| Missing input | status chỉ rõ cần nhập đủ | giữ screen, không gửi |
| Password mismatch | status báo hai mật khẩu chưa khớp | giữ giá trị để sửa |
| Agreement off | status yêu cầu đồng ý điều khoản | checkbox toggle được, không dead click |
| Backend unavailable | status báo dịch vụ chưa kết nối | không tạo account hoặc chuyển screen |
| Quay lại | secondary navigation | đóng Register, mở lại Entry, không đổi gameplay state |
| Escape | navigation | tương đương Quay lại |

## Base-first và asset/pixel budget

- Hierarchy/state thuộc một partial `CongDongLamArrivalHud.Register.cs`; Entry chỉ gọi route mở screen.
- Reuse Entry shell, text field, password reveal, action, status, ornament và icon helper trong `CongDongLamArrivalHud.Skin.cs`. Nếu cần role mới thì bổ sung shared helper trước, không style cục bộ trong partial.
- Reuse account/lock/eye icons từ HUD atlas hiện hành `512 × 512`, cell `96 px`, khoảng `85 KB`; hiển thị `20–24 px`. Back là text action, không dùng glyph/icon tạm. Không thêm runtime texture.
- Canonical khoảng `2.4 MB` là external review asset và không import vào Unity.
- Không emoji, glyph, placeholder, social-login icon, QR hoặc asset không có provenance.

## Gate hoàn thành

- TDD khóa một Register overlay, ba field, hai reveal action, agreement toggle, một primary CTA và back route.
- Test đủ missing input, mismatch, chưa đồng ý và backend unavailable; không claim account được tạo.
- Build Player sau C# change; capture và xem PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768`.
- Chỉ khóa layout khi không cắt/chồng/wrap và hierarchy bám canonical.
- Shared-skin, UI catalog, no-3D, no-source-image, change-budget và frozen diff audit đều sạch.

## Kết quả runtime khóa

- Full `TwoDCharacterRuntimeStateTests`: `27/27` pass.
- Player: `build/map01a-register-player-v1/LinhGioiOnline.app`; build `Succeeded`, `errors=0`, `warnings=44`, `totalSize=184132534`.
- Evidence: `build/map01a-register-runtime-v1/{pc,mobile,tablet}/register-account.png` tại `1600×900`, `1600×720`, `1024×768`; manifest đều ghi `captureScope=map01a-register`, `registerOverlayExpected=true`, `usesOsMouseOrKeyboard=false`.
- Visual audit đã xem trực tiếp cả ba frame: cùng composition landscape, không wrap/stack, không cắt/chồng title, field, agreement, CTA hoặc back action.
- Không thêm texture runtime; canonical external giữ nguyên hash đã ghi ở trên.
