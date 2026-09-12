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

Đăng nhập theo `LOGIN-GATE-ENTRY-VISUAL-SPEC-v1.md`: cổng/nền, title, vùng chọn máy chủ/tài khoản thử và CTA vào thế giới. Audit thấy `RuntimeLoginResponsiveLayout` còn nhưng entry 2D không gọi nó, asset Login V3B cũng không có trên branch; không coi helper tồn tại là màn đã hoạt động. Reuse `AccountApiClient.LoginDevAsync` khi nối luồng, không tự mở production auth/đăng ký hoặc sửa contract. Phải tạo demo cụ thể trước khi nối.

Hành trang tiếp theo cần card/inspect món, trạng thái đang mặc/tháo, cấp có thật và nút đóng rõ; giữ catalog/equip đã audit. HUD tiếp theo chỉ dùng hành vi hiện có; chưa thêm chat/guild/shop/minimap tương tác giả. Lỗi atlas kiến trúc thiếu source vẫn là gate art chưa đạt, độc lập với công việc UI.

Tham khảo: [Unity 6 UI Document](https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-create-ui-document-component.html) mô tả nhiều document dùng chung panel/focus. Áp dụng ở đây là tái sử dụng panel hiện hành và điều khiển theo trạng thái màn, không nhân UI riêng theo class hoặc tỷ lệ màn hình.
