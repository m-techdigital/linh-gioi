# Nhập môn Linh Thành: kịch bản và design nháp

Ngày: 2026-09-07. Trạng thái: DRAFT, chưa owner duyệt; không phải evidence runtime.

## Căn cứ

- Thiết kế liên kết NPC, Lộ, skill, trang bị và trang phục: `docs/design/LGO-GAME-SYSTEMS-NARRATIVE-DESIGN.md`; đây là đề xuất phát triển theo kịch bản gốc, không mở hệ thống production.
- Định hướng gốc owner cung cấp: nhân vật Người Thức Tỉnh, Linh Thành là trung tâm đời sống và chiến đấu; social city chuyển thành chiến trường trong Âm Giới Xâm Lăng. Không chọn class ngay đầu hành trình theo định hướng gốc; prototype hiện vẫn dùng class.sword và chưa triển khai progression mở Lộ.
- `docs/02-GDD.md`: city -> field/combat -> city/social/upgrade, `map.city.linh_thanh`, năm hướng phát triển với hai class trong Founder Alpha. Không mở social/economy chỉ vì xuất hiện trong concept.
- `docs/reference-art/linh-gioi-world-event-ui.png`: không gian Neo-Asian, grouping HUD và trục social city/world event; không sao chép VIP, tiền, cấp hoặc tính năng chưa có vào prototype.
- `docs/reference-ui/lgo-character-three-slots-draft-v1.jpg`: hướng skin đã duyệt, kính tối trung tính, vàng/ngà, base shell/button chung.
- Storyboard mới: `docs/reference-ui/lgo-linh-thanh-onboarding-storyboard-draft-v1.jpg`.

## Phân biệt hiện tại và đề xuất

M5 hiện là bài thử kỹ thuật: gặp Người Giữ Cổng -> tương tác Đá Luyện -> pulse/completed; lưu vị trí và về sảnh. Bia đánh/skill preview là bài thử riêng, không phải nhiệm vụ canon nối tiếp. Không có đủ địa hình, camera, thành phố, quest progression hay phần thưởng để gọi đây là onboarding hoàn chỉnh.

Đề xuất đặt vòng nhập môn ngắn tại khoảng sân sát Linh Môn, thuộc hướng vào Linh Thành, không tạo một map sân luyện tách khỏi sản phẩm. Tên khu vực, thoại và tuyến nhiệm vụ dưới đây là đề xuất, chưa tự thay GDD hoặc dữ liệu frozen.

| Cảnh | Ý định người chơi | Hành vi/feedback | Chuyển trạng thái | Khoảng cách với source |
|---|---|---|---|---|
| 1. Qua Linh Môn | Biết mình vừa đến đâu, ai đón mình | Đường đá dẫn tới NPC; một mục tiêu ngắn; movement trái, context action phải | Đến phạm vi NPC | Có input/proximity; chưa có sân nối city và camera như demo |
| 2. Người Giữ Cổng | Hiểu thao tác tương tác và hướng đi | Một header NPC, body thoại cuộn, footer Tiếp tục/Đóng; chặn input world khi modal mở | Kết thúc thoại -> mục tiêu Đá Luyện | Prototype đã bỏ header lặp, đóng giữa chừng không hoàn tất; thoại và scene còn cần đối chiếu design truyện |
| 3. Chạm Đá Luyện | Thử tương tác, nhận phản hồi rõ | Vòng focus tại vật thể, pulse ngắn, mục tiêu đổi trạng thái; không loot/EXP giả | Tương tác hợp lệ một lần | Có local pulse/completed; chưa có quest persistence |
| 4. Ra quảng trường | Biết bước tiếp theo và thấy thế giới mở ra | Hướng đi rõ về quảng trường, hết hướng dẫn nhập môn | Tới hub; nội dung tiếp theo theo roadmap | Chưa có tuyến đường/đích đến; không thay bằng popup thắng hoặc vòng luyện lặp |

## Trạng thái cần thiết kế trước khi code tiếp

- Chưa vào phạm vi / trong phạm vi / rời phạm vi: không để action nhắm nhầm NPC hoặc vật thể.
- Dialogue mở / dài / cuộn đến cuối / đóng giữa chừng / mở lại: chung base modal, body-only scroll, footer không tràn; world input bị chặn khi mở.
- Tương tác thành công / không hợp lệ / nhấn nhiều lần: không phát nhiều reward hoặc tự tiến quest. Hiện chưa có reward.
- Hoàn tất / quay lại hub / reconnect: chỉ hiển thị tiến trình đã lưu thật; local completed không được quảng cáo là quest persistence.
- PC rộng/hẹp, tablet 4:3, mobile landscape, resize trong cùng phiên: vị trí nhóm chức năng thống nhất; text/button không đè scene trọng tâm. Storyboard không phải chứng minh responsive hoặc kích thước pixel.
- Combat HUD, bốn active skill/ultimate/spirit, né và world-event chuyển trạng thái cần design riêng trước triển khai; không suy ra đã có từ các icon trong reference.

## Quy tắc asset và nghiệm thu

- Demo hội thoại riêng: `docs/reference-ui/lgo-gatekeeper-dialogue-draft-v1.jpg`, DRAFT chưa duyệt. Hai trạng thái ngắn/dài minh họa một speaker header, body cuộn, footer hai nút cùng tier không bọc thêm card. Cảnh nền và portrait mới là concept, không tự thay NPC runtime. Ảnh sinh chưa đúng tỷ lệ 16:9 và body còn thụt theo cột portrait; không dùng tọa độ ảnh làm kích thước code hoặc coi đó là viewport evidence. Khi triển khai, body dùng vùng còn lại của base modal, header/footer không co và không lọt khỏi shell.
- Nhãn `Đã hiểu` trong demo chỉ phù hợp nếu action thật xác nhận hướng dẫn; không đổi nhãn của action hoàn tất nhiệm vụ sang xác nhận rồi giữ hành vi cũ. Thoại dài trong ảnh là đề xuất lời văn, chưa là dữ liệu canon.
- Storyboard chỉ review kịch bản/composition, không crop/slice/import composite. Bốn khung chưa phải viewport matrix chuẩn; chi tiết chữ trang trí trong ảnh không phải nội dung canon.
- Tách brief cho ground, kiến trúc, NPC, nhân vật, icon và VFX; mỗi asset ghi kích thước hiển thị thực, texture/import profile, source bytes và memory/build cần đo. Không phát sinh ảnh lớn chỉ để dùng như icon.
- Chỉ bắt đầu vertical slice world khi có layout/demo cho cảnh được chọn, acceptance trạng thái và mapping tới hành vi được roadmap cho phép. So ảnh runtime với đúng cảnh/viewport/state, không coi capture hoặc concept đẹp là PASS.
- Không mở production auth/DB, economy, social hay event thật trong batch thiết kế này.

## Provenance

Imagegen trong chat, tham chiếu world-event và Character Hall đã duyệt; nguồn ngoài repo `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-69276ad5-037e-449c-8a6f-e671b1473e0d.png`. JPEG quality78 bằng sips, không crop. Demo mới chưa owner duyệt.

Demo dialogue: cùng thư mục generated_images, `exec-0d2fe0d9-4b9e-4b83-b783-603fe51850ee.png`, tham chiếu Character Hall đã duyệt và NPC direction v0.20.0; JPEG quality78, không crop/slice/import runtime.
