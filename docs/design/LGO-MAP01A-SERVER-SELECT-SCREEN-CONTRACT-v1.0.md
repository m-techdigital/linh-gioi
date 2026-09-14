# Map01A Chọn máy chủ — canonical screen contract v1.0

Ngày khóa design: 2026-09-14
Trạng thái: **LAYOUT_LOCKED / PLAYER_EVIDENCE_REVIEWED**

## Nguồn quyết định duy nhất

Canonical design duy nhất:
`/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v7-server-select/01-server-select-CANONICAL.png`

- Canvas: `1672 × 941`.
- SHA-256: `653e6850feb17425e72a0900b2e8d7d2e5bac9ba6d1ec9b310f0c46187665007`.
- Ảnh giữ visual language của Entry canonical; form đăng nhập được thay bằng modal chọn server riêng.
- Hai output ImageGen trung gian nằm ngoài thư mục design và không phải nguồn quyết định. Title canonical được sửa bằng mask đúng vùng chữ để giữ nguyên phần còn lại và bảo đảm dấu tiếng Việt chính xác.
- Runtime screenshot là evidence đối chiếu, không phải design thứ hai.

## Scenario và phạm vi dữ liệu

Screen được mở từ server row của Entry/Login hoặc Character Select. Người chơi xem server hiện hành, chọn server có thật và xác nhận quay về screen đã mở nó.

Runtime hiện chỉ có một server đã biết: `S1 · Đông Lâm`, trạng thái `Mượt`. Không bịa region, ping, shard, server phụ, hàng đợi hoặc trạng thái backend. Việc xác nhận chỉ cập nhật lựa chọn local và quay về nguồn; không claim routing hay kết nối server thật.

## Hierarchy và layout canonical

1. Giữ nguyên nền Đông Lâm, logo, nhân vật trang trí, utility rail và notification card của Entry.
2. Một modal chính giữa, kích thước gần với auth card hiện hành để chuyển screen không giật bố cục.
3. Header và subtitle ngắn ở đầu modal.
4. Một server card selected chiếm toàn chiều rộng nội dung; icon thật, tên, trạng thái và nhãn máy chủ hiện tại.
5. Status line yên tĩnh nằm dưới card.
6. Action row gồm `Quay lại` secondary và `Xác nhận` primary.

| Vùng | Rect/tỷ lệ mục tiêu trên 1672×941 | Quy tắc |
|---|---:|---|
| Modal | x `548..1124`, y `320..742` | panel navy/gold dùng shared Entry shell |
| Title block | y `350..420` | không wrap; title/subtitle cùng trục giữa |
| Server card | x `589..1084`, y `444..560` | một card duy nhất; selected blue glow |
| Status | y `574..608` | không chiếm thêm row khi copy đổi |
| Actions | x `589..1084`, y `633..694` | hai button cân nhau, gap `24..28` |

PC, mobile landscape và tablet giữ cùng composition bằng scale/safe margin; không stack action, không đổi modal thành full-screen list và không wrap tên server.

## State và interaction

| State | Hiển thị | Interaction |
|---|---|---|
| Default/selected | `S1 · Đông Lâm`, `Mượt`, `Máy chủ hiện tại` | chọn lại card giữ selected và phản hồi local |
| Xác nhận | primary gold | lưu lựa chọn local, đóng screen và quay về Entry hoặc Character Select đã mở nó |
| Quay lại | secondary dark | đóng screen, không đổi lựa chọn và quay về nguồn |
| Escape | navigation | tương đương Quay lại |

Mọi control nhận click phải đổi state hoặc screen. Không có dead control. Server row ở Entry và Character Select phải cùng gọi một route shared; không copy hai modal hoặc hai cơ chế chọn server.

## Base-first và asset budget

- Hierarchy/state nằm trong một partial `CongDongLamArrivalHud.ServerSelect.cs`.
- Modal, server card, action và status dùng helper/class shared trong `CongDongLamArrivalHud.Skin.cs`; không tạo skin riêng trong partial.
- Reuse icon `server` từ HUD atlas hiện hành `512 × 512`, cell `96 px`, khoảng `85 KB`; icon hiển thị tối đa `48 px`, đủ density cao. Không thêm texture runtime cho screen này.
- Nền và logo dùng layer Entry hiện hành. Canonical 2.5 MB chỉ là external review asset, không import vào Unity build.
- Không dùng emoji, glyph icon, placeholder hoặc ảnh sinh ngẫu nhiên trong runtime.

## Gate hoàn thành

- Test RED/GREEN khóa một overlay, một server card, hai actions và route từ cả Entry/Character Select.
- Test xác nhận/quay lại phục hồi đúng source screen và không đổi class/source/gameplay state.
- Build Player sau C# change.
- Capture và xem trực tiếp PC `1600 × 900`, mobile landscape `1600 × 720`, tablet `1024 × 768`.
- Chỉ khóa layout khi không cắt/chồng/wrap, server card đọc rõ và hierarchy bám canonical.
- Shared-skin, UI catalog, no-3D, no-source-image, change-budget và frozen diff audit đều sạch.

## Kết quả runtime đã khóa

- Player: `build/map01a-server-select-player-v1/LinhGioiOnline.app`; build `Succeeded`, `errors=0`.
- Evidence: `build/map01a-server-select-runtime-v1/{pc,mobile,tablet}/server-select.png` và manifest tương ứng.
- Profile đã xem trực tiếp: PC `1600×900`, mobile landscape `1600×720`, tablet `1024×768`. Cả ba giữ một composition, không cắt/chồng/wrap và không sinh server giả.
- EditMode: focused route test `1/1`, full `TwoDCharacterRuntimeStateTests` `26/26`.
- Runtime dùng lại scene/logo/icon atlas hiện hành; không import canonical 2.5 MB và không thêm texture cho screen.
