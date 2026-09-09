# Linh Giới Online — Scenario Mapping cho nhánh 2D

Tài liệu này giữ vai trò mapping kịch bản sang triển khai 2D sau khi branch `feature/2d` ngừng hướng dựng nhân vật/cảnh 3 chiều.

## SCN-001 — Cổng Linh Thành

- Người chơi bắt đầu tại Cổng Linh Thành.
- Trọng tâm runtime: nền 2D, nhân vật dạng sprite, NPC gác cổng, thoại ngắn, lựa chọn vào thành.
- Tương tác tối thiểu: di chuyển, focus NPC, mở thoại, chọn tiếp tục, chuyển sang vùng luyện.
- Visual reference: gate keeper, spirit gate, HUD mockup và palette trong pack reference.

## SCN-002 — Bia Luyện Khí

- Người chơi tới sân luyện gần bia đá.
- Trọng tâm runtime: sprite nhân vật, bia luyện, feedback chạm/khởi động, hướng dẫn thao tác đầu tiên.
- Tương tác tối thiểu: di chuyển tới bia, focus, kích hoạt, hiện phản hồi rõ ràng trên HUD.

## Quy tắc dùng reference

- Reference chỉ định mood, màu, UI, bố cục và nhịp tương tác.
- Không kéo lại pipeline tạo nhân vật/cảnh 3 chiều.
- Nếu thiếu asset, dùng sprite 2D placeholder có nhãn rõ rồi thay bằng asset duyệt sau.
