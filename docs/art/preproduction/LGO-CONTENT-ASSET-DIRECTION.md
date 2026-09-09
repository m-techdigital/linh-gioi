# Linh Giới Online — Content Asset Direction cho nhánh 2D

Branch `feature/2d` dùng hướng asset 2D-first. Tài liệu này chỉ giữ định hướng thị giác để dựng runtime có thể chơi nhanh, tránh quay lại pipeline tạo mô hình nhân vật/cảnh 3 chiều.

## Linh Thành

Linh Thành là trung tâm xã hội: cổng thành, quảng trường, phố thấp, nhà phố cao, quán nhỏ, đèn, ghế, cây, sạp và hoa văn sân. Mặt ngoài hơi cổ kính: gỗ sẫm, tường trát cũ nhẹ, ngói xám xanh, cửa song, mái hiên và biển treo. Cafe/đời sống hiện đại nằm trong nền kiến trúc đó, không biến thành mặt dựng kính lớn.

## Runtime 2D ưu tiên

- Sprite nền theo lớp: xa, giữa, sàn tương tác, props phía trước.
- Nhân vật/NPC dạng sprite hoặc sprite-sheet, có pivot chân thống nhất.
- Props tương tác có silhouette rõ, hit area dễ đọc, icon/hud đồng bộ.
- Asset thiếu thì dùng placeholder phẳng có nhãn, màu và kích thước đúng gameplay trước khi polish.

## Budget

- Ưu tiên texture/sprite atlas gọn, dễ thay thế.
- Không thêm công cụ sinh mô hình hoặc importer 3 chiều trong branch này.
- Mỗi batch cần cải thiện player-visible hoặc giảm rủi ro runtime 2D.
