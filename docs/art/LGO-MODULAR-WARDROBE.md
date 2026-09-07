# Công thức trang phục dùng chung — NPC và nhân vật

Áp dụng cho pipeline ngoại hình; không mở hệ thống gameplay trang bị hoặc đổi protocol/schema. Mẫu hiện tại: Keeper SCN-001/002, theo `02_gate_keeper_character_sheet.png`. Ưu tiên silhouette, bề mặt liền và độ nét trên PC trước tối ưu mobile.

## Chia theo món đồ

Danh mục máy đọc: `client/art-source/appearance-slots.json`. Mỗi mặt mesh có thuộc tính FACE INT `appearance_slot`; tuyệt đối không suy đoán món đồ theo độ cao hoặc bone weight.

| Phần | Phạm vi |
|---|---|
| Headwear | Mũ, phụ kiện cố định trên mũ |
| Head / Hair | Đầu–mặt–cổ và tóc, tách để thay kiểu tóc |
| UpperBody / LowerBody | Áo gồm tay áo/tà thuộc áo; quần |
| Cape | Áo choàng liền, tách khỏi áo |
| Gloves / Boots | Cặp găng và cặp giày/ủng |
| Shoulders / Accessories | Giáp vai; đai, bội và phụ kiện |
| MainHand / OffHand | Vũ khí hoặc vật cầm, chỉ tạo khi có asset |
| Eyes / FacialHair | Mắt hình học hoặc râu khi thực sự cần; mắt vẽ trên mặt hiện thuộc Head |

Không chia riêng từng viền, nếp gấp, hoa văn hoặc mỗi tam giác. Chi tiết nhỏ nằm trong texture; hình học chỉ dành cho chi tiết ảnh hưởng đường bao và chuyển động. Hai bên của một cặp găng/ủng có thể cùng một slot, không cần thêm GameObject.

## Quy trình làm một bộ

1. Chốt design và các pose cần dùng: đứng, quay, giơ tay/chỉ đường. Dựng tổng thể các món lớn trong một lượt.
2. Dùng rig, tỷ lệ, bind pose và chuẩn UV của cùng họ cơ thể. Keeper dùng `SharedHumanoidAvatar.asset` đã kiểm chứng; không để Unity suy lại cột sống/cổ từ mesh của bộ đồ mới. Class dùng cùng hình thể có thể dùng chung; hình thể khác cần bản fit tương ứng, không hứa mọi mesh tự khớp mọi cơ thể.
3. Nối bề mặt trong từng món; đặt ranh giới dưới cổ áo, cổ tay, đai và cổ giày. Không giữ lớp cơ thể kín nằm xuyên qua trang phục. Phần da lộ phải được giữ, không xóa theo ngưỡng tọa độ tùy ý.
4. Gán slot rõ ràng và skin theo bộ phận giải phẫu. Mũ theo Head; áo theo thân/cánh tay; găng theo bàn tay; giày theo chân. Không ghim tay hoặc chân vào pelvis để chữa tà áo.
5. Export truyền slot bằng tên section `LGO_<Slot>_Body/Face`. Baker tách các mesh tác giả theo slot, bảo toàn vertex/UV/weight/bind pose, rồi ghép trước thành preset runtime dùng chung. Không ghép lại mỗi frame. Nhiều NPC cùng bộ dùng chung mesh/material/texture, nhưng có pose riêng.
6. Sau macro pass: kiểm tra đúng phần vừa đổi, chạy test bảo toàn module và chia sẻ preset; build/capture NPC thật trong Unity ở các pose trên. Capture thành công chưa phải đạt chất lượng hình ảnh.

Keeper hiện có 9 slot hình học: Headwear, Head, Hair, UpperBody, LowerBody, Cape, Gloves, Boots và Shoulders. Bội/đai hiện là chi tiết texture thuộc áo; chỉ tách Accessories khi có món cần thay độc lập.

Player nhập môn dùng cùng baker và Avatar, có 7 slot (không mũ/cape), 2 material và cùng hai atlas; không thêm texture. `tools/art/wardrobe_sections.py` truyền slot cho cả hai exporter. Source đầu phải đủ sọ/gáy dù thay kiểu tóc; không dựa vào cape/tóc dài che bề mặt bị thiếu. Tạo hình player vẫn candidate, chưa phải bộ class Kiếm final.

Cơ chế hiện tại ghép **bộ đã chọn sẵn trong Editor**. Chọn đồ động, cache theo tổ hợp, che cơ thể theo trang bị và vũ khí gắn socket là bước triển khai sau; danh mục slot không đồng nghĩa các gameplay đó đã tồn tại.

## Tham khảo và áp dụng

- [Epic — Working with Modular Characters](https://dev.epicgames.com/documentation/unreal-engine/working-with-modular-characters-in-unreal-engine?lang=en-US): cùng skeleton; nhiều component vẫn tăng chi phí render; mesh merge và atlas chung giảm chi phí lặp lại. Linh Giới áp dụng nguyên tắc này bằng baker Unity, không chuyển nguyên API Unreal.
- [Valve — Warlock wearable requirements](https://www.dota2.com/workshop/requirements/warlock): chia Head, Shoulders, Arms, Back/Robe, Weapon, Offhand và Belt; mỗi slot có budget riêng. Ví dụ áo choàng có texture256×512 và giới hạn LOD0 2.000 tam giác. Đây là minh họa cách quản lý món đồ, không áp nguyên budget của Dota vào camera cận cảnh Linh Giới.
- [Valve — Dota 2 Character Art Guide](https://help.steampowered.com/en/faqs/view/0688-7692-4D5A-1935): ưu tiên silhouette đọc được, cân bằng vùng đơn giản và vùng chi tiết, đánh giá trong game. Áp dụng cho độ rõ của trang phục, giữ phong cách Á Đông của Linh Giới.
- [Epic — City Sample Crowds](https://www.fab.com/listings/903037e9-e1ac-4f41-96e8-1683c6fa7ad4?lang=en): tổ hợp đầu, tóc, áo, quần, giày và phụ kiện theo các hình thể tương thích. Chỉ tham khảo cách tổ chức, không nhập asset UE vào Unity.

Chia món giúp tái sử dụng và sửa dễ hơn; tự nó không làm dung lượng giảm mạnh. Phải đo riêng mesh, texture dùng chung, dữ liệu bộ đồ mới và RAM/GPU. Không cam kết mỗi bộ dưới 100 KB khi chưa có asset hoàn chỉnh và phép đo.
