# Phân tích demo UI owner và hướng áp dụng game 2D

Ngày: 2026-09-13. Phân loại: **OWNER_DEMO_REFERENCE_ONLY**, chưa phải design chuẩn/đã duyệt; không tự thêm toàn bộ hệ thống trong ảnh vào gameplay. Owner giao code trước, tự kiểm Player, sau đó owner review chỉnh sửa. Đây là quyết định áp dụng có thể chỉnh, không phải chứng nhận chất lượng hiện tại.

Owner gửi thêm nhóm ưu tiên v2 tại `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/preferred-v2/`. Manifest đã ghi SHA-256/kích thước cho 4 PNG và status `OWNER_PRIORITY_UIUX_REFERENCE`. Chỉ đạo mới nhất của owner: các ảnh này hợp lý hơn nhưng vẫn chưa hoàn thiện, cần phân tích/redesign cho game 2D hiện tại, **không follow 100%**. Áp dụng vì vậy là lấy phân cấp, vị trí nhóm chức năng và chất liệu xanh đậm/vàng; không bê nguyên số liệu Lv80, currency, combat power, icon menu chết, slot không khớp chuẩn 10 hoặc nhân vật minh hoạ khác base.

## Nhóm ưu tiên v2

| Demo | Điểm lấy ngay | Điểm cần redesign |
|---|---|---|
| `01-hud-mobile-cong-dong-lam-joystick-cum-chien-dau-phai.png` | Avatar/HP/MP trái, nhiệm vụ + minimap phải, thao tác chiến đấu theo vùng ngón cái, chat nhỏ đáy | Runtime PC hiện dùng phím/chuột; không thêm menu/shop/event/chấm đỏ khi chưa có chức năng. HUD phải còn đủ khoảng nhìn map và NPC. |
| `02-nhan-vat-trang-bi-tui-do-ruong-do-chi-tiet.png` | Ba vùng nhân vật/trang bị, túi/rương grid, detail/action rõ | Chuẩn runtime là 10 slot hiện hành; rương đồ/kho chưa có model giao dịch nên chỉ dùng làm hướng màn sau. Không sinh LC/cường hoá/set bonus giả. |
| `03-tui-do-modal-nhan-vat-luoi-vat-pham-chi-tiet.png` | Overlay modal có tabs, grid item và detail phải; close lớn dễ thấy | Không copy số lượng slot/trang sức trong ảnh. Hành trang Map01A dùng state quest/item thật và không dựng actor thứ hai làm nhầm base. |
| `04-dang-nhap-may-chu-bat-dau-linh-gioi.png` | Logo/form trung tâm, máy chủ rõ trạng thái, CTA vàng lớn, thông báo phụ đáy | API hiện chỉ có dev auth; phải tách `Đăng nhập` và `Bắt đầu` để không thành hai CTA cùng nghĩa. Không tự mở password/production auth hay nút hỗ trợ/trailer giả. |

## Kho ảnh nguyên bản

Đã lưu 8 PNG trực tiếp từ dữ liệu đính kèm của tin nhắn owner, không resize/chỉnh sửa; 22.944.207 byte, 7 ảnh khác nhau. Ảnh 05 trùng byte ảnh 02 nhưng giữ nguyên thứ tự gửi. SHA-256, kích thước và thời điểm nhận trong [manifest nguồn](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/manifest.json). Nguồn ảnh nằm ngoài Unity/repo; không dùng board đã có chữ làm texture màn chơi.

- [01-hud-gameplay-dong-lam-touch-demo.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/01-hud-gameplay-dong-lam-touch-demo.png) — 1672×941.
- [02-hud-gameplay-dong-lam-desktop-demo.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/02-hud-gameplay-dong-lam-desktop-demo.png) — 1672×941.
- [03-dang-nhap-linh-gioi-demo.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/03-dang-nhap-linh-gioi-demo.png) — 1672×941.
- [04-chon-nhan-vat-nam-class-kiem-demo.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/04-chon-nhan-vat-nam-class-kiem-demo.png) — 1672×941.
- [05-hud-gameplay-dong-lam-desktop-demo-ban-lap.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/05-hud-gameplay-dong-lam-desktop-demo-ban-lap.png) — 1672×941.
- [06-nhan-vat-trang-bi-10-o-va-tui-do-demo.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/06-nhan-vat-trang-bi-10-o-va-tui-do-demo.png) — 1672×941.
- [07-tui-do-chi-tiet-so-sanh-trang-bi-demo.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/07-tui-do-chi-tiet-so-sanh-trang-bi-demo.png) — 1672×941.
- [08-thoi-trang-mac-thu-phoi-do-demo.png](/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/08-thoi-trang-mac-thu-phoi-do-demo.png) — 1672×941.

## Phân tích từng màn

| Demo | Điểm có ích | Vấn đề quan sát | Hướng áp dụng cho LGO 2D |
|---|---|---|---|
| 01 HUD touch | HP/MP gom trái, nhiệm vụ phải, thao tác dưới; icon kèm nhãn | Joystick nằm trên khung chat; nhiều nhóm nút và logo lớn chiếm màn; dấu đỏ dày | Vùng ngón cái có khoảng trống riêng; UI phụ thu gọn; chỉ hiện hệ thống đã dùng được. Tách chỉ dẫn bàn phím khỏi touch. |
| 02/05 HUD desktop | Hotbar gọn hơn touch; vị trí thông tin ổn định | Nhiều menu không thuộc Map01A, chữ nhỏ trên nền nhiều chi tiết; demo lặp | Giữ hotkey/tooltip; HUD mặc định chỉ HP/MP, nhiệm vụ, hành trang và thao tác có thật. Logo/slogan dành cho login, không chiếm góc gameplay. |
| 03 Login | Một CTA chính nổi bật, form và máy chủ phân cấp rõ | Hỗ trợ lặp hai nơi, nhóm trailer/thông báo cạnh form cạnh tranh chú ý; chưa có trạng thái lỗi | Form nhất quán nhãn/focus/lỗi/đang kết nối/thử lại; máy chủ rõ trạng thái. Không tạo mật khẩu/đăng ký giả khi backend hiện chỉ có dev auth. Nền mới cùng chất 2D, không nhập nguyên ảnh. |
| 04 Chọn nhân vật | Danh sách chọn, highlight và CTA vào game rõ | Trộn chọn class bên trái với chọn nhân vật bên phải; thanh chỉ số không đơn vị; xoá cạnh CTA chính | Tách chọn nhân vật có sẵn khỏi tạo nhân vật/chọn class. Xoá đặt trong menu phụ và xác nhận tên; preview phải dùng đúng dữ liệu nhân vật. Không dùng model/pose mới thay base đã chốt. |
| 06 Nhân vật + túi đồ | Slot bao quanh preview, lọc túi, nhận biết món đang mặc | Ba cột quá dày với mobile; có hai trang sức nhưng thiếu các slot áo trong/áo ngoài theo chuẩn 10 slot hiện hành; thông số/currency chưa tồn tại | Giữ 10 slot chuẩn runtime, không đổi schema theo ảnh. Bản chơi dùng panel gọn và actor thật; màn chi tiết sau dùng cùng state, không một hệ trang bị khác. Chỉ hiện số liệu có nguồn. |
| 07 Chi tiết/so sánh | Hai món có tiêu đề rõ, CTA trang bị riêng, lọc theo nhóm | Cột đang mặc chứa delta đỏ dễ hiểu nhầm chiều so sánh; quá nhiều nút phá huỷ/chuyển đổi ngang nhau | Delta luôn là món chọn trừ món đang mặc; ghi +/− và nhãn, không chỉ màu. Mobile xem một chi tiết, mở so sánh khi cần. Bán/phân giải ở menu phụ; không dựng số combat/set bonus giả. |
| 08 Thời trang | Phân biệt đang mặc, khoá, mặc thử, lưu; phối đồ dễ quan sát | Trộn trang bị/chỉ số với ngoại hình, nhiều nút áp dụng; preview khác base có thể lừa người chơi | Khi triển khai cosmetic, draft preview riêng với loadout đã áp dụng; huỷ trả trạng thái cũ, lưu áp dụng rõ. Một renderer/base/source đăng ký, không vẽ lại actor theo mỗi màn. Hệ dye/cánh/preset chưa triển khai. |

Ảnh cho định hướng navy–lam–vàng nhạt, typography rõ cấp và viền panel mảnh. Giảm glow/hoa văn quanh chữ nhỏ; không biến mọi nút thành CTA vàng. Nút chọn cần cả viền/nhãn, nút disabled có lý do. Bộ icon phải cùng nét và độ dày, nhưng chưa được sinh/cắt từ board ở batch chức năng này.

## Đối chiếu nguồn bên ngoài

Các điều áp dụng dưới đây là lựa chọn thiết kế của LGO, không phải các game tham khảo xác nhận giải pháp của dự án.

- [FFXIV — cấu trúc HUD, hotbar, menu và tuỳ chỉnh](https://na.finalfantasyxiv.com/game_manual/view/): HUD tách các nhóm chức năng; có tuỳ chỉnh vị trí/kích thước/hiển thị, log và hotbar. LGO lấy việc phân nhóm và giữ vị trí ổn định; không bê mật độ UI hoặc hệ thống của game 3D sang 2D.
- [FFXIV — đăng nhập và tạo nhân vật](https://na.finalfantasyxiv.com/game_manual/start/): đăng nhập tài khoản và bắt đầu nhân vật là các bước riêng. LGO phân biệt rõ auth, máy chủ, chọn/tạo nhân vật; chỉ nối API đã có, ghi phần backend còn thiếu.
- [MapleStory — Maple Guide](https://support-maplestory.nexon.com/hc/en-us/articles/204744405-How-does-Maple-Guide-work): gợi ý nội dung theo level và điều kiện nhiệm vụ. LGO dùng một việc tiếp theo ở Map01A; không tự thêm teleport hoặc mở toàn bộ hoạt động chỉ vì icon đẹp.
- [Guild Wars 2 — Fashion Templates](https://help.guildwars2.com/hc/en-us/articles/48794217062931-Feature-Explanation-Fashion-Templates): có inspect, lưu/đổi tên và xác nhận khi xoá template. LGO giữ phân biệt preview với áp dụng, bảo vệ phối đồ đã lưu; không triển khai tiền/phí transmutation hoặc thêm hệ cosmetic trong batch này.
- [Xbox Accessibility Guideline 101](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101): đánh giá chữ ở ảnh game thật, theo thiết bị/DPI và khả năng scale; không dùng fontSize trong code làm bằng chứng đủ đọc. LGO dùng font Việt đầy đủ, label thường thay đoạn ALL CAPS dài, cuộn một chiều khi thiếu chỗ. Chưa tuyên bố đạt toàn bộ XAG/thiết bị thật.
- [Unity 6 ScrollView](https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-uxml-element-ScrollView.html): nội dung cuộn có container riêng. Header/footer hành trang là sibling để hành động không trôi khi cuộn.

## Ma trận màn và trạng thái cần làm

| Màn/nhóm | Tương tác và trạng thái phải kiểm | Trạng thái thực tế/giới hạn |
|---|---|---|
| Map01A/HUD | Đi/chạy/nhảy/đánh, HP/MP đổi, nhiệm vụ tiến độ, tương tác gần NPC, input desktop/touch | Đã có local Q01–Q09; bảo toàn actor/camera. UI cần polish theo bộ chung. |
| Đăng nhập | Nhập thiếu/sai, gửi một lần, đang kết nối, timeout/mất mạng/thử lại, máy chủ không sẵn sàng | Chưa có entry UI 2D; hiện API dev, không password/production auth. |
| Chọn/tạo nhân vật | Chưa có nhân vật, chọn/thông tin/giới/class, tên lỗi, vào game, quay lại, thao tác xoá an toàn | Audit account API trước nối; nút F hiện phục vụ catalog review, không giả thành đổi nghề gameplay. |
| Hành trang | Chọn từng 10 slot, mặc/tháo, đổi cấp có thật, chuyển class/giới, đóng/mở giữ state, dùng vật phẩm | Batch hiện tại: panel phải để tránh che actor ở spawn; header/footer cố định, tabs; chưa nghiệm thu visual. |
| Rương đồ/kho | Gửi/rút, chọn số lượng, hết chỗ, không đủ món, stack, món đang mặc, đóng/mở, không nhân đôi/mất đồ | Yêu cầu mới owner; chưa triển khai kho. Khác rương nhiệm vụ Map01A đang phát thưởng. Cần model giao dịch chung với inventory, API có sẵn thì reuse; contract thiếu phải ghi gate, không sửa frozen. |
| Chi tiết/so sánh | Tên/slot/cấp/trạng thái, tương thích class/giới, delta đúng chiều, mục không có dữ liệu | Chỉ hiện thuộc tính thật; không bịa combat power/rarity/set effect theo demo. |
| Hội thoại NPC | Trước/đang/sau nhiệm vụ, nhiều trang, hỏi thêm, đóng sớm, nhận/trả, quay lại | Đã có sáu NPC và session chung; cần skin cùng UI và kiểm lại cả input/che nền. |
| Kỹ năng | Skill có thật, khoá/chưa có, cooldown/thiếu điều kiện, hotkey/touch | Giữ phạm vi combat hiện hành, không sinh skill từ icon demo. |
| Thời trang | Mặc thử/huỷ/lưu, phối chéo, item khoá, preview đúng source/base | Chưa mở lại redraw/level expansion; giữ WIP và mục tiêu wardrobe còn tồn đọng. |
| Menu/cài đặt/hỗ trợ | Mở/đóng/quay lại, focus, âm lượng/hiển thị/input khi có chức năng thật | Thiết kế chung; không đặt nút chết, số ping/currency giả. |
| Chat/bang/bạn bè/shop/sự kiện | Chỉ đưa vào flow khi roadmap và backend đủ | Reference dài hạn; chưa là tính năng đã mở bởi bộ demo. |

## Tiêu chí kiểm và thứ tự

Map01A ổn định trước; tiếp theo login/chọn nhân vật, hành trang/rương đồ, các màn/nút và thoại liên quan. Batch hành trang chức năng đang có được hoàn tất an toàn, không rollback chỉ vì nhận thêm demo. Kiểm các màn ở PC/tablet/mobile; chủ động xem ảnh và thao tác chuột/phím/touch, không dùng test xanh thay visual. Không tắt đổi class, không biến preview thành actor thứ hai, không đổi camera/tỷ lệ để nhường UI.

Giữ riêng: chưa có dữ liệu/feature, kỹ thuật đã chạy, ảnh đã tự xem, và owner đã review. Không ghi “design chuẩn” hoặc “hoàn thành goal” khi một màn hoặc wardrobe vẫn chưa đạt. Sau code owner sẽ review, không yêu cầu duyệt lại từng chi tiết nhỏ.
