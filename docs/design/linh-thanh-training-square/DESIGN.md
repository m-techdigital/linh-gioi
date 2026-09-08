# Quảng trường Đá Luyện — thiết kế lại v0.2

Trạng thái: **draft thiết kế theo yêu cầu owner 2026-09-08; đã có candidate macro runtime, chưa art final**.

Owner xác định phố hiện tại không phù hợp để đặt Đá Luyện và yêu cầu thiết kế lại map/kịch bản theo sheet03. Hướng mới thay giả định bố cục “đá nhỏ sát nhà bên đường” trong SCN-002 v0.1. Giữ phố cũ làm tuyến nhập thành hoặc kit cho phố khác; không xóa hay phóng to đồng loạt cảnh đó. Không thay map ID, protocol, schema hoặc design tokens.

## Mẫu gốc và vai trò trong thế giới

- [Sheet03 — Đá Luyện](../../reference-art/linh-gioi-story-design-reference-pack-v0.1/images/reference_only/03_training_stone_prop_sheet.png): tinh thể xanh, mảnh lơ lửng, giá đồng chạm, bệ cổ và vòng cộng hưởng; một landmark cộng đồng có tỷ lệ lớn hơn người.
- [Storyboard05 — lần đầu đến Sân Luyện](../../reference-art/linh-gioi-story-design-reference-pack-v0.1/images/reference_only/05_scenario_002_training_stone_storyboard.png): tiếp cận/người hướng dẫn → cộng hưởng → rèn luyện → hoạt động trở lại và sự kiện.
- [Concept board](../../reference-art/linh-gioi-concept-board.png): social city, café, nghề nghiệp, năm Lộ Võ/Kiếm/Pháp/Cơ/Linh, fantasy Á Đông hiện đại.
- [Demo bố cục](training-square-draft-v2.png), ảnh minh họa thiết kế, không làm background/billboard runtime. Mặt bằng bằng mét trong `layout-v2.json` là dữ liệu bố trí đề xuất để dựng 3D; nếu hình sinh lệch kích thước, số liệu mặt bằng được ưu tiên.

Đá Luyện thuộc **một khu quảng trường trong Linh Thành**, nối với phố nhập thành, chợ, café và khu nghề. Đây chưa phải yêu cầu tạo một map mạng riêng hoặc thêm màn loading. Nơi này phục vụ cả gặp gỡ, quan sát, luyện tập và đi qua; không buộc mọi người ngồi thiền hay biến toàn bộ game thành tu tiên.

## Bố cục lớn cần dựng trước

| Cụm | Kích thước/vị trí đề xuất | Người chơi nhìn và làm gì |
|---|---|---|
| Khoảng sân | 40 × 48 m; đá đặt tại tâm, nền ngang | Từ lối vào nhận ra landmark và đường tới ngay; thấy người khác sinh hoạt |
| Đá và giá đỡ | Tổng cao khoảng 6,5 m, theo tỷ lệ sheet03; chân đài đường kính khoảng 5 m | Tinh thể và giá đồng có silhouette riêng, mảnh phụ có khoảng trống rõ; không kéo scale model nhỏ để thay model landmark |
| Bệ tiếp cận | Cao 0,8 m, bậc rộng thấp phía nam; đường dốc ngắn bên hông | Tiếp cận điểm chạm trên mép bệ ở tầm tay; không phải chạm vào đỉnh tinh thể |
| Vùng tương tác | Từ mép bệ đến bán kính khoảng 7 m; 4 vị trí tiếp cận | Đứng cạnh bạn bè, góc camera không bị chân đài che; điểm chạm chung cùng một Đá Luyện |
| Vòng đi qua | Bán kính 7–11 m, thông suốt quanh đài | Người đi chợ/café không phải cắt qua người đang tương tác |
| Lối nam | Rộng 12 m, nối phố nhập thành; vào sân tại z=-24 m | Một đoạn mở rộng chuyển từ phố sang sân, thấy toàn bộ đá trước khi tới điểm chạm |
| Mép đông | Trà đình/ghế ngồi/cây, lối café | Hoạt động cộng đồng ở rìa, không chiếm vòng tương tác |
| Mép đông bắc | Gian nghề nhỏ và chỗ đứng của NPC thợ | Thể hiện đời sống và Lộ Cơ trong cùng thế giới |
| Mép tây/bắc | Lối chợ và đường sâu vào thành | Có đích đi tiếp sau cộng hưởng; tránh quảng trường cụt chỉ để làm tutorial |
| Người hướng dẫn | Thanh Huyền, vị trí (-5, -12 m), lệch trái lối vào và ngoài vòng đi | Chào/hướng dẫn chung cho năm Lộ; không đứng che trục nhìn tinh thể |

Nhà cổ đặt ngoài vùng đi vòng, khoảng lùi từ tâm tối thiểu 14 m. Ghế, đèn và cây ở rìa sân; không tạo vòng cột bao kín đá. Kiến trúc gồm tường đá cũ, chân cột chắc, gỗ sẫm, mái ngói xanh xám cong vừa phải, đồng xỉn và đèn ấm. Không dùng cổng vàng khổng lồ cạnh tranh với Đá Luyện.

## Ba camera dùng để chốt tổng thể

1. **Lối nam:** camera chơi thật sau lưng người, nhìn thấy đủ đỉnh đá, chân đài và hai đường vòng; có người cao khoảng 1,8 m làm thước đo.
2. **Điểm chạm:** người và bề mặt chạm cùng khung; camera được lùi/xoay nhẹ, luôn còn đường thoát; không cần cinematic khóa input.
3. **Từ trà đình nhìn về sân:** thấy nhóm người quanh đá và người đi qua, phân biệt chỗ nghỉ với vùng tương tác.

Giữ PC sắc nét làm mốc art. Không đặt số lượng NPC hoặc kích thước texture tối ưu mobile vào tiêu chí duyệt phom ở bước này. Vẫn dùng bộ nhà/mái/đèn và vật liệu chung; số người trong demo chỉ minh họa đời sống, không chứng minh multiplayer hoặc FPS.

Ảnh demo thể hiện tốt silhouette và không khí; tỷ lệ bậc/bệ trong ảnh là minh họa. Khi dựng master, chốt bệ tiếp cận 0,8 m và chiều cao điểm chạm theo mặt bằng, không suy collider/kích thước bằng cách đo pixel ảnh.

## Cấu trúc asset để dựng tiếp

- `StoneLandmark`: lõi tinh thể, cụm mảnh phụ, khung đồng, bệ đá, dấu cộng hưởng là các phần có ý nghĩa chỉnh riêng trong master; xuất mesh/material phù hợp runtime.
- `TrainingSquare`: mặt sân, bệ tiếp cận, vòng đi, đường nhánh, rìa sinh hoạt. Dùng cùng hệ tọa độ mét trong layout; va chạm và điểm chạm thuộc scene, không lấy bounds toàn tinh thể làm vùng tương tác.
- `CityPerimeter`: kit nhà/hiên/trà đình/gian nghề/cây/đèn dùng lại; thay biến thể và vị trí để tránh dãy nhà giống hệt.
- `Population`: dùng công thức rig/slot trang phục chung, giữ khác biệt silhouette các Lộ; không nhân bản Keeper thành toàn bộ dân thành.

## Trình tự triển khai và kiểm chứng

1. Dựng **cả sân + landmark đúng tỷ lệ + tuyến đi + camera + silhouette rìa sân** trong một master; so ba camera với demo trước khi chi tiết hóa.
2. Kết nối flow giới thiệu → điểm chạm → kết quả → đi tiếp trong candidate mới; phố cũ vẫn có thể dùng lại nhưng không còn là đích đặt Đá Luyện.
3. Hoàn thiện cả chất liệu tinh thể/đồng/đá và ánh sáng theo sheet; không tiếp vá màu hoặc đổi hệ số model nhỏ đang đặt ở phố.
4. Chỉ build/capture sau lượt tổng thể. Kiểm tra riêng tuyến nam–đá–café/chợ, điểm chạm và camera; không chạy lại toàn bộ QA các màn không đổi.

Nghiệm thu bố cục cần ảnh camera chơi thật, đi vòng không kẹt, điểm chạm trong tầm tay, người chơi không bị che khi cộng hưởng, cảnh vẫn đọc được khi có nhóm người. Đây là tiêu chí cần chạy, **chưa phải PASS**.

Kịch bản và các giai đoạn cơ chế: [SCN-002 v0.2](../../story/scenarios/SCN-002-TRAINING-SQUARE-v0.2.md).

## Candidate runtime ngày 2026-09-08

Mở bằng `python3.12 tools/preview_training_square.py` sau build development. Sân/landmark/camera/tuyến chạm–trà đình đã chạy; candidate dùng Keeper hiện hữu hướng dẫn tạm, chưa Thanh Huyền, dân cư, đường dốc hay bốn điểm chạm. Phố cũ vẫn ở nhánh riêng. Chín ảnh thực1920×1080 được review tại `build/visual-evidence/training-square-framing-desktop/review.json`; đây là checkpoint bố cục và flow, không nghiệm thu art final hoặc toàn bộ SCN002v0.2.
