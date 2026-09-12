# Map01A — UI chơi và các màn tiếp nối

Trạng thái: thiết kế triển khai theo phạm vi owner, chưa nghiệm thu toàn bộ visual. Mẫu cụ thể: external selected source `map-01a-cong-dong-lam/01-gameplay-screen-16x9.png` (nhóm trạng thái trái, nhiệm vụ phải, điều khiển đáy), `05-dialogue-and-quest-flow.png` và `10-collision-trigger-ui-safe-area.png`. Không nhập board vào runtime hoặc mở shop/chat/reward chỉ vì có trong ảnh.

## Batch HUD hiện tại

- Chơi: tên class/giới và thanh HP/MP ở trái, cạnh thông tin khu vực; nhiệm vụ/tương tác giữ chung base hiện hành, trung tâm dành cho map và nhân vật.
- Hành trang: dùng panel 10 món hiện hành, đổi class và giới ngay trong panel. Giới thiếu pack bị vô hiệu hóa, không bật renderer cũ. Đóng panel vẫn giữ trang bị và class.
- Source-pose đã chọn: không hiện dãy nút chuyển mode/full/base cũ và không nhận phím C chuyển renderer; các món thao tác trong hành trang. Giữ F/G và đường state chung đã có.
- Hội thoại: ẩn vitals và thao tác nền; session nhiều trang/đóng/hỏi thêm/nhận việc đã triển khai. Không chồng nhãn POSE THỬ bằng OnGUI lên HUD; log/manifest vẫn ghi REVIEW_ONLY.
- Nhãn NPC nằm trên đầu, ẩn khi thoại mở; hành trang/tương tác chung một hàng flex, không neo hai nút bằng khoảng cách giả định theo độ dài chữ.
- Chung UI Toolkit, font Noto và PanelSettings đã có. Không tạo hệ UI/input khác. Kiểm PC/tablet/mobile bằng Player, cả trạng thái panel đóng/mở, thoại và HP thay đổi.

## Sau map

Owner chỉnh hướng ngày 2026-09-12: **thiết kế mới cho game 2D hiện tại**, không dùng login/sảnh V3B, nền Linh Thành đêm cũ hoặc bố cục 3D làm mẫu. Góc ngang, nét vẽ và màu sắc theo Cổng Đông Lâm đang chạy. Owner có thể cung cấp design khi cần; chưa có bộ UI mới được duyệt và chưa nhập ảnh login mới vào runtime.

Bộ màn cần cùng một ngôn ngữ UI: đăng nhập (trống/nhập/đang kết nối/lỗi/vào game), HUD chơi (HP/MP, nhiệm vụ, di chuyển, kỹ năng đang có, tương tác), hành trang (10 slot, xem món, tháo/mặc, cấp có thật, class/giới có pack), thoại NPC (nhiều trang, hỏi thêm, nhận việc, đóng, quay lại). Hành trang giữ header/footer, chỉ phần danh sách cuộn; trung tâm màn chơi dành cho actor/map. Chỉ tái sử dụng phần kỹ thuật UI Toolkit, font, input và state hiện hành, không ép layout cũ vào màn mới.

`AccountApiClient.LoginDevAsync` là API dev có thể dùng khi nối đăng nhập; chưa phải entry hoạt động. Không tự mở production auth/đăng ký hoặc sửa contract. Tạo demo 2D cụ thể trước khi nối; các trạng thái phải kiểm trên Player.

Hành trang tiếp theo cần card/inspect món, trạng thái đang mặc/tháo, cấp có thật và nút đóng rõ; giữ catalog/equip đã audit. HUD tiếp theo chỉ dùng hành vi hiện có; chưa thêm chat/guild/shop/minimap tương tác giả. Lỗi atlas kiến trúc đã sửa sau khi phục hồi raw source ngày 2026-09-12; gate sửa cắt mảnh có Player evidence. Đây chưa phải nghiệm thu toàn bộ sản phẩm.

Tham khảo: [Unity 6 UI Document](https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-create-ui-document-component.html) mô tả nhiều document dùng chung panel/focus. Áp dụng ở đây là tái sử dụng panel hiện hành và điều khiển theo trạng thái màn, không nhân UI riêng theo class hoặc tỷ lệ màn hình.
