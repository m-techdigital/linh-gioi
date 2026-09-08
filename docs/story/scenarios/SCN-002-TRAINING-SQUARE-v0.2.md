# SCN-002 v0.2 — Đến quảng trường Đá Luyện

**Draft thiết kế lại theo yêu cầu owner 2026-09-08. Chưa triển khai runtime.**

Hướng mới: Đá Luyện là landmark và nơi sinh hoạt trong Linh Thành, theo sheet03/storyboard05. Bố cục này thay mục tiêu “đá thấp bên phố” ở v0.1 cho công việc tiếp theo. Runtime hiện tại vẫn là prototype cũ; không coi thiết kế này là đã có multiplayer, tiến trình hay phần thưởng thật.

Map/camera/kích thước và demo: [Quảng trường Đá Luyện](../../design/linh-thanh-training-square/DESIGN.md).

[Storyboard 4 nhịp](../../design/linh-thanh-training-square/arrival-storyboard-draft-v2.png) — ảnh thiết kế, chưa phải gameplay capture.

## Ý nghĩa trong hành trình

Người Giữ Cổng đón người mới ở tuyến nhập thành. Người chơi đi vào một khoảng sân rộng, nhận ra Đá Luyện trước khi cần đọc hướng dẫn. Đây là lần đầu thấy nhịp sống thành phố: người đi qua, trò chuyện ở trà đình, làm nghề và luyện tập. Đá Luyện kết nối năm Lộ, không quyết định hoặc thay class của người chơi.

Thanh Huyền là người hướng dẫn Sân Luyện, kế thừa nhân vật trong storyboard05. Có thể giữ cách xưng “Sư phụ” trong lời thoại phù hợp, nhưng thiết kế nghề nghiệp/trang phục không biến toàn bộ cư dân thành tu sĩ. Người Giữ Cổng không phải người đứng sát đá và cũng không yêu cầu người chơi quay lại cổng mới được đi tiếp.

## Lần đầu đến — một đoạn chơi khoảng 2–3 phút, cần đo bằng playtest

| Nhịp | Người chơi thấy/làm | Phản hồi và điều kiện chuyển |
|---|---|---|
| 1. Được chỉ đường | Keeper: “Qua con phố này là quảng trường Đá Luyện. Thanh Huyền ở đó sẽ giúp bạn làm quen.” | Mục tiêu “Đến quảng trường Đá Luyện”; hướng đi theo cửa sân, không dẫn xuyên nhà tới tọa độ lõi |
| 2. Phố mở ra sân | Từ lối nam nhìn thấy đỉnh tinh thể, người quanh bệ, trà đình và đường vòng | Hiện tên khu một lần; camera vẫn do người chơi điều khiển, không tự kéo vào cinematic |
| 3. Gặp Thanh Huyền | NPC đứng lệch trục; “Đá Luyện ở đây dành cho mọi người. Bạn cứ thử chạm vào dấu trên bệ.” | Hai lựa chọn: “Thử cộng hưởng” và “Hỏi về nơi này”; đóng thoại vẫn đi được |
| 4. Tới điểm chạm | Dấu nhỏ trên mép bệ sáng dịu; “Chạm Đá Luyện” khi đủ gần | Vùng chạm ở tầm tay, tách khỏi collider của tinh thể lớn. Có thể tiếp cận từ nhiều phía |
| 5. Cộng hưởng đầu tiên | Player đặt tay; vân sáng chạy từ điểm chạm qua giá đỡ lên lõi; mảnh phụ chuyển động nhẹ; vòng sáng lan trong vùng bệ | Hiệu ứng khoảng 2–3 giây đề xuất, nhìn rõ nhưng không quét kín màn. Input di chuyển có ưu tiên, không khóa bằng animation |
| 6. Kết quả | “Đá Luyện cộng hưởng với linh lực của bạn.” → “Một luồng sáng dịu lan ra. Bạn cảm thấy bình tĩnh hơn.” | Thông báo ngắn trong HUD chung; không popup thưởng giả hay bảng thống kê lúc giới thiệu |
| 7. Tự do đi tiếp | Thanh Huyền: “Khi muốn luyện tập, cứ quay lại. Qua bên kia là quán trà và khu chợ.” | Hoàn tất đoạn giới thiệu, trả mục tiêu khám phá thành. Có thể ngồi nghỉ, hỏi thêm hoặc đi tiếp; không buộc chạy về Keeper |

Lời hỏi thêm: “Người đi đường, thợ máy hay người dùng thuật đều có cách cộng hưởng riêng. Đá không chọn con đường thay bạn.” Giữ rõ Võ/Kiếm/Pháp/Cơ/Linh; không đổi thành ba class Công/Thủ/Hỗ trợ từ một panel của storyboard.

## Khi quay lại — thiết kế hệ thống theo sheet, không cắt mất tầm nhìn dài hạn

| Nhóm hoạt động | Hướng thiết kế | Giai đoạn và điều kiện triển khai |
|---|---|---|
| Cộng hưởng tự do | Chạm/ngắm hiệu ứng, trò chuyện hoặc quan sát người luyện; giữ khu vực có đời sống khi chưa nhận nhiệm vụ | Ưu tiên sau đoạn giới thiệu; không thưởng mỗi lần bấm |
| Luyện nhịp | Tương tác nhịp ngắn có phản hồi rõ, độ khó dễ tiếp cận; không ép người mới chơi ngay | Thiết kế chi tiết input/khả năng tiếp cận và gate minigame trước khi code |
| Hướng luyện | Công/Thủ/Hỗ trợ là trọng tâm một buổi luyện trong class hiện có, không thay class và không sinh class mới | Cần thống nhất với build/skill/combat roadmap; chưa đặt chỉ số |
| Hoạt động hằng ngày | Mục tiêu có giới hạn, khuyến khích trở lại và cùng luyện | Reward/cooldown/xác nhận server phải đi cùng gate economy; chưa chốt tiền, EXP hoặc buff |
| Luyện cùng nhóm/bang | Nhiều người có chỗ đứng quanh đài, hiệu ứng cá nhân không che nhau; hoạt động cộng tác có điểm bắt đầu/kết thúc rõ | Cần multiplayer synchronization, giới hạn VFX và kiểm tra tải |
| Âm Khí xâm nhiễu | Sự kiện biến đổi Đá Luyện và quảng trường, liên hệ Âm Giới Xâm Lăng; cảnh báo và đường rút rõ | Sau gate world-event/combat, không chen trận đánh vào lần chạm đầu |
| Mở kỹ năng/nâng cấp | Đá có thể là nơi hướng dẫn hoặc điểm vào hệ thống đã được roadmap cho phép | Không tự mở bảng nâng cấp độc lập, không cộng chỉ số local giả |

“Quá tải” và “nhiễm Âm Khí” trong sheet là trạng thái của hoạt động/sự kiện tương lai, không phải ngẫu nhiên phạt người đang làm tutorial. Giá trị buff, phần thưởng và điều kiện cấp độ trong ảnh là concept cần cân bằng; chưa chuyển thành dữ liệu game.

## Trạng thái và ranh giới kỹ thuật cần giữ khi hiện thực hóa

- Luồng đầu: `Arrive → MeetGuide → ApproachTouchPoint → Resonating → Introduced → ExploreCity`. Đây là ký hiệu thiết kế, chưa thêm enum/schema/protocol.
- Hủy trước khi xác nhận: về trạng thái tới gần, chưa hoàn thành. Di chuyển sau xác nhận: dừng cử chỉ, kết quả chỉ ghi một lần; không được nhân đôi phần thưởng nếu hệ thống thưởng được bổ sung sau này.
- Nhiều người tương tác dùng các vị trí chạm khác nhau của cùng một landmark; không khóa cả đài cho một người, không đổi trạng thái tutorial của người khác.
- VFX chung idle dịu; phản hồi cá nhân có mức ưu tiên và phạm vi. Không giả lập đồng bộ bằng nhân bản các player đứng trang trí rồi claim multiplayer.
- Đóng/mở lại hội thoại và rời/quay lại khu không reset tiến trình ngoài ý muốn. Persistence thật cần gate hiện có; candidate local phải ghi rõ local.

## Đánh giá demo và runtime tiếp theo

Thiết kế đạt hướng khi đá là tâm không gian, có tỷ lệ người rõ, đường tới và đường đi vòng dễ đọc, sinh hoạt không cản điểm chạm, kiến trúc cổ kính và đa dạng năm Lộ vẫn hiện diện. Runtime phải chứng minh các điều này trong camera chơi thật cùng thao tác đi/chạm/thoát; hình demo hoặc build thành công không thay cho kiểm chứng đó.
