# Map01A Entry/Login — canonical screen contract v1.0

Ngày khóa design: 2026-09-14  
Trạng thái: **CANONICAL_DESIGN_LOCKED / LAYOUT_LOCKED**

## Nguồn quyết định duy nhất

Canonical design duy nhất:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v5-entry/01-entry-login-CANONICAL.png`

- Canvas: `1672 × 941`.
- SHA-256: `204178a0a0236873e59af781479d4f7440b5f702f6f8a84130b121cfeca1b52d`.
- Hai ảnh `03-dang-nhap-linh-gioi-demo.png` và `preferred-v2/04-dang-nhap-may-chu-bat-dau-linh-gioi.png` là lịch sử tham khảo đã được hợp nhất. Chúng không còn quyền quyết định layout/runtime.
- Runtime screenshot/evidence chỉ là kết quả đối chiếu, không được dùng làm design thứ hai.

## Scenario và hierarchy

Screen xuất hiện trước character select và Map01A playable HUD. Người chơi nhập tài khoản, mật khẩu, có thể nhớ tên tài khoản, chọn/đọc trạng thái máy chủ và thực hiện một CTA đăng nhập. Auth backend chưa được mở nên runtime phải phản hồi trung thực, không giả đăng nhập thành công.

Thứ tự thị giác bắt buộc:

1. Art nền Đông Lâm phủ canvas và logo Linh Giới ở trục giữa trên.
2. Một form card xanh đen viền vàng ở giữa, nằm dưới logo; không có card/form thứ hai.
3. Hai field thật, option row, hai action cùng hàng, server row và một status line yên tĩnh.
4. Notification card gọn ở góc trái dưới.
5. Utility rail dọc ở phải gồm `Thông Báo`, `Hỗ Trợ`, `Cinematic`, `Cài Đặt`.

Không có nút `Bắt đầu` khổng lồ sau nút `Đăng nhập`; `Đăng nhập` là primary CTA duy nhất. Không có guest login, tab phụ hoặc support action lặp trong form.

## Layout canonical

| Vùng | Rect/tỷ lệ mục tiêu | Quy tắc |
|---|---:|---|
| Logo block | tâm x `835`, y `92..307` | logo lớn nhưng không đụng form |
| Form card | `548, 319, 578, 420` | căn giữa; padding ngang khoảng 40 px |
| Credential fields | `589, 351, 497, 124` | hai field cao 54 px, gap 16 px |
| Option row | `589, 487, 497, 31` | checkbox trái, forgot link phải |
| Auth actions | `589, 534, 497, 61` | hai nút cùng base, primary/secondary rõ |
| Server row | `589, 613, 497, 67` | status không co chữ hoặc wrap |
| Status line | `629, 695, 447, 28` | reserved; default info, lỗi inline |
| Notification | `23, 784, 545, 126` | không che form/character |
| Utility rail | x `1560..1644` | bốn action dọc, cùng icon/button base |

Geometry được phép scale đồng nhất; không đổi hierarchy hoặc wrap/stack để vừa viewport.

## State và interaction

| State | Hiển thị | Interaction |
|---|---|---|
| Default | hai field rỗng hoặc tên đã nhớ; status server `Mượt` | account/password focus được; password mask |
| Remember off/on | box và mark đổi trạng thái | chỉ lưu username local; tuyệt đối không lưu password |
| Missing credentials | status line báo nhập đủ thông tin | focus field thiếu; không chuyển screen |
| Backend unavailable | status line báo dịch vụ đăng nhập chưa kết nối | không giả auth; giữ dữ liệu field |
| Register | secondary action | phản hồi read-only/locked trung thực khi chưa có backend |
| Server switch | row/chevron rõ | disabled/read-only nếu routing chưa có contract |
| Utility rail | bốn action cùng base | action chưa có chức năng phải có disabled/feedback rõ; không dead click |

## Viewport gate

| Profile | Evidence size | Bắt buộc |
|---|---:|---|
| PC | `1600 × 900` | đủ logo, form, notification và rail |
| Mobile landscape | `1600 × 720` | giữ cùng composition; scale đồng nhất, không xếp dọc |
| Tablet | `1024 × 768` | giữ cùng composition; safe margin, không cắt CTA/rail |

## Base-first và asset gate

- Reuse `CongDongLamArrivalHud.Skin.cs` cho entry shell, field, button, detail/server card, status card và side action. Không hard-code một skin mới trong `Entry.cs`.
- `Entry.cs` chỉ được dựng hierarchy, tên element, state và callback.
- Icon phải dùng atlas/provenance hiện hành; không emoji hoặc glyph tạm.
- Không sửa background/camera/class/pose/wardrobe/source để làm form trông đúng.

## Gate result và chuyển screen

- Player `build/map01a-entry-canonical-player-v1/LinhGioiOnline.app` build thành công, `errors=0`.
- Evidence `build/map01a-entry-canonical-runtime-v1/{pc,mobile,tablet}/entry-login.png` đã xem trực tiếp. Cả ba giữ cùng hierarchy, không stack, cắt hoặc chồng form/notice/rail.
- Full `TwoDCharacterRuntimeStateTests` đạt `24/24`; UI catalog/shared-skin/no-3D/no-source/frozen gates sạch.
- Credential, password mask/reveal, remember-account và hai trạng thái thiếu input/backend unavailable được khóa bằng test; runtime không giả auth thành công.
- Kết luận: Entry/Login đạt layout gate. Screen kế tiếp phải quay về design gate và có canonical/contract riêng trước code.
