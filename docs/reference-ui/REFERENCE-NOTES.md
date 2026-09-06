# Reference bổ sung: nhân vật và điều khiển

## Nguồn và phạm vi

- Reference HUD chính theo owner: `../reference-art/linh-gioi-world-event-ui.png`. Phân nhóm movement trái, combat phải, điều hướng hệ thống đáy, nhiệm vụ trái và trạng thái nhân vật riêng. Prototype chỉ triển khai hành vi thật đang được phép; không lấy sân luyện làm đích thay thế vòng chơi GDD. Bản HUD sinh bổ sung không thay nguồn này.

- `../reference-art/linh-gioi-concept-board.png`: năm hướng Võ, Kiếm, Pháp, Cơ, Linh. Không thay thiết kế tổng thể bằng class đang có trong prototype.
- `../02-GDD.md`, mục 3: `class.martial` và `class.sword` là mục tiêu Founder Alpha; `class.arcane`, `class.tech`, `class.spirit` dành cho tương lai. Giới tính trên board là mẫu tạo hình, không phải quy tắc khóa giới tính class.
- `lgo-five-paths-character-reference-v1.jpg`: concept đủ năm hướng, bám board gốc; chưa phải nội dung playable, sprite hay animation. Không cắt nhân vật từ composite để import.
- `lgo-sword-costume-skill-concepts-v1.jpg`: ba trang phục của một nhân vật Kiếm, không phải ba class hoặc ba cấp trang bị. Ba biểu tượng bám tên preview hiện có; không chứng minh cả ba đã có combat server-authoritative.
- `lgo-cross-platform-controls-draft-v1.jpg`: bản nháp HUD, chưa duyệt layout. Ô tablet chưa đúng 4:3, thứ tự icon mobile chưa đồng nhất; key badges chỉ là đề xuất, chưa phải mapping runtime. Không dùng ảnh này làm bằng chứng layout đạt.

## Kích thước và dung lượng

Các JPG là reference ngoài Unity Assets/Resources, không tăng payload runtime. Bản sinh gốc giữ ngoài repo; không import composite. Khi tạo asset riêng, đo kích thước hiển thị lớn nhất bằng pixel thực sau panel scaling, rồi chọn texture gần nhất đủ nét. Điểm bắt đầu để thử: icon 128x128 cho vùng hiển thị tối đa 128 pixel; chỉ nâng 256 khi số đo hoặc kiểm tra nét yêu cầu. Không đồng nhất kích thước vùng chạm với kích thước hình icon.

Dung lượng JPG/PNG trên đĩa không thay cho số đo texture memory hoặc build size. Chọn compression theo nền tảng và kiểm tra alpha/viền icon ở kích thước thật. Không mặc định mọi icon cần 512 hoặc 1024.

## Nguồn nghiên cứu điều khiển

- [Blizzard: Making Diablo Immortal for PC](https://news.blizzard.com/en-us/article/23797159/making-diablo-immortal-for-pc): PC chuyển tương tác sang chuột/bàn phím, bổ sung WASD; không phải sao chép nguyên input mobile.
- [Blizzard: Controller support](https://news.blizzard.com/en-us/article/23814052/diablo-immortal-cut-down-demons-with-a-controller): nhận diện phương thức nhập và hỗ trợ controller trên mobile/PC.
- Hướng LGO theo yêu cầu owner: cùng base movement/skill controls có mặt trên PC/tablet/mobile; tách input modality khỏi viewport. Giữ keyboard, pointer và touch đi qua cùng hành vi, không tạo ba implementation riêng.

## Prompt và provenance

Tạo bằng built-in image generation trong chat ngày 2026-09-06; chuyển JPG quality 82 bằng sips, không crop/slice. Prompt cốt lõi:

- HUD: ba viewport PC/tablet/mobile, cùng movement pad và ba skill cyan/violet/green, vùng giữa thoáng, inset an toàn, không shop/chat/level giả; tham chiếu `lgo-runtime-ui-north-star-v1.png`. Kết quả còn sai bố cục như ghi trên.
- Costume: cùng nam Kiếm tu, ba biến thể Training/Ceremonial/Travel, ba concept Wind Slash/Shadow Bind/Spirit Guard, không chỉ số hoặc progression; tham chiếu north-star.
- Five paths: đúng năm nhân vật Võ vàng với quyền/bracer, Kiếm xanh-trắng với kiếm, Pháp jade với pháp trận, Cơ với cơ khí/kính, Linh tím với linh phù; toàn thân, cùng scale, không chỉ số/level/skill tự đặt; tham chiếu concept board gốc. Hai hướng đầu ghi Founder Alpha target, ba hướng sau ghi Future paths.
