# Reference bổ sung: nhân vật và điều khiển

## Nguồn và phạm vi

- `lgo-character-three-slots-draft-v1.jpg`: owner duyệt làm hướng visual mới ngày 2026-09-07, gồm nền tối trung tính, viền vàng mảnh, chữ ngà/vàng, tiêu đề serif; chuyển skin cũ qua base dùng chung. Luôn 3 slot, silhouette giữa, form/hai nút cột phải. Ảnh sinh ngày 2026-09-06 còn lệch đáy hai cột: runtime phải căn đáy bằng layout constraints, không sao chép tọa độ lỗi. Đây là design reference, không phải evidence hay asset runtime. JPEG quality 82, không crop.

- Reference HUD chính theo owner: `../reference-art/linh-gioi-world-event-ui.png`. Phân nhóm movement trái, combat phải, điều hướng hệ thống đáy, nhiệm vụ trái và trạng thái nhân vật riêng. Prototype chỉ triển khai hành vi thật đang được phép; không lấy sân luyện làm đích thay thế vòng chơi GDD. Bản HUD sinh bổ sung không thay nguồn này.

- `../reference-art/linh-gioi-concept-board.png`: năm hướng Võ, Kiếm, Pháp, Cơ, Linh. Không thay thiết kế tổng thể bằng class đang có trong prototype.
- `../02-GDD.md`, mục 3: `class.martial` và `class.sword` là mục tiêu Founder Alpha; `class.arcane`, `class.tech`, `class.spirit` dành cho tương lai. Giới tính trên board là mẫu tạo hình, không phải quy tắc khóa giới tính class.
- `lgo-five-paths-character-reference-v1.jpg`: concept đủ năm hướng, bám board gốc; chưa phải nội dung playable, sprite hay animation. Không cắt nhân vật từ composite để import.
- `lgo-sword-costume-skill-concepts-v1.jpg`: ba trang phục của một nhân vật Kiếm, không phải ba class hoặc ba cấp trang bị. Ba biểu tượng bám tên preview hiện có; không chứng minh cả ba đã có combat server-authoritative.
- `lgo-cross-platform-controls-draft-v1.jpg`: bản nháp HUD, chưa duyệt layout. Ô tablet chưa đúng 4:3, thứ tự icon mobile chưa đồng nhất; key badges chỉ là đề xuất, chưa phải mapping runtime. Không dùng ảnh này làm bằng chứng layout đạt.

## Kích thước và dung lượng

Các JPG là reference ngoài Unity Assets/Resources, không tăng payload runtime. Bản sinh gốc giữ ngoài repo; không import composite. Khi tạo asset riêng, đo kích thước hiển thị lớn nhất bằng pixel thực sau panel scaling, rồi chọn texture gần nhất đủ nét. Điểm bắt đầu để thử: icon 128x128 cho vùng hiển thị tối đa 128 pixel; chỉ nâng 256 khi số đo hoặc kiểm tra nét yêu cầu. Không đồng nhất kích thước vùng chạm với kích thước hình icon.

Dung lượng JPG/PNG trên đĩa không thay cho số đo texture memory hoặc build size. Chọn compression theo nền tảng và kiểm tra alpha/viền icon ở kích thước thật. Không mặc định mọi icon cần 512 hoặc 1024.

### Font runtime theo demo mới

- Base typography dùng Noto Serif Bold cho heading, Noto Sans Regular cho body; `unityFontDefinition` qua `FontDefinition.FromFont`, không dựa vào font OS. Nguồn [Noto fonts](https://github.com/notofonts/noto-fonts), thư mục `hinted/ttf/NotoSerif/NotoSerif-Bold.ttf` và `hinted/ttf/NotoSans/NotoSans-Regular.ttf`; license OFL giữ cạnh font trong `UI/Resources/LGOUI`.
- SHA256 nguồn lần lượt `3b2086a869bcded2aeb4416fc281ceec9d6ce3c06756cda19f8f763636204e7d` và `b85c38ecea8a7cfb39c24e395a4007474fa5a4fc864f6ee33309eb4948d232d5`. FontTools 4.64.0, subset `U+0020-024F,U+0300-036F,U+1E00-1EFF,U+2000-206F,U+20AB`, body thêm `U+2190-21FF`, `--no-hinting --name-IDs='*' --name-languages='*'`.
- `HeadingSerif.ttf` 104556 byte, `BodySans.ttf` 92908 byte: tổng source 197464 byte, không phải tổng memory atlas hoặc build delta. Đã kiểm tra glyph heading tiếng Việt cả NFC/NFD. Không coi tên font trong style là bằng chứng đã render: kiểm tra `unityFontDefinition` và ảnh Player.

### Avatar ô trống

- `UI/Resources/LGOUI/EmptyCharacterAvatar.png`: sinh riêng trong chat 2026-09-07, silhouette đầu/vai trung tính nền alpha; không crop/slice composite. Nguồn ngoài repo: `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-a8cbda0e-52fe-4f7b-ac94-28a9398cff7c.png`.
- Resize bằng `sips -z 128 128`; PNG 14254 byte, vùng UI 48 logical unit; import max128, không mipmap/readable. Dung lượng source không phải GPU memory/build delta. Avatar hồ sơ hiện vẫn dùng full-body V3B nhỏ, chưa portrait riêng theo demo.

## Nguồn nghiên cứu điều khiển

### Nền Linh Thành mới

- `Art/Runtime/V3B/Resources/LGOArtV3B/Login/linh_thanh_night_v1.jpg`: sinh riêng bằng imagegen 2026-09-07, theo hướng cảnh đêm của `lgo-character-three-slots-draft-v1.jpg`, không crop/slice hoặc xóa UI từ composite. Nguồn ngoài repo: `~/.codex/generated_images/01a0748f-76a8-7be2-bd55-33fe5e41c403/exec-2c5015b0-417f-4d01-91d3-b1942f3901e4.png`.
- 1672x941 nguyên bản, JPEG quality60 bằng sips, 431721 byte (budget login background 512 KiB). Không upscale. Import texture không alpha/mipmap/readable; desktop/iOS max2048, Android max1024. Chưa đo memory/build trên thiết bị Android/iOS thật.
- Registry dùng chung Login/Character Hall, nền cũ chỉ fallback và vẫn nằm trong Resources; không claim đã giảm tổng build. Asset mới ngoài manifest lịch sử, report inventory hiện liệt kê riêng và tính trong tổng V3B thật. Chưa production-final art.

- [Blizzard: Making Diablo Immortal for PC](https://news.blizzard.com/en-us/article/23797159/making-diablo-immortal-for-pc): PC chuyển tương tác sang chuột/bàn phím, bổ sung WASD; không phải sao chép nguyên input mobile.
- [Blizzard: Controller support](https://news.blizzard.com/en-us/article/23814052/diablo-immortal-cut-down-demons-with-a-controller): nhận diện phương thức nhập và hỗ trợ controller trên mobile/PC.
- Hướng LGO theo yêu cầu owner: cùng base movement/skill controls có mặt trên PC/tablet/mobile; tách input modality khỏi viewport. Giữ keyboard, pointer và touch đi qua cùng hành vi, không tạo ba implementation riêng.

## Prompt và provenance

Tạo bằng built-in image generation trong chat ngày 2026-09-06; chuyển JPG quality 82 bằng sips, không crop/slice. Prompt cốt lõi:

- HUD: ba viewport PC/tablet/mobile, cùng movement pad và ba skill cyan/violet/green, vùng giữa thoáng, inset an toàn, không shop/chat/level giả; tham chiếu `lgo-runtime-ui-north-star-v1.png`. Kết quả còn sai bố cục như ghi trên.
- Costume: cùng nam Kiếm tu, ba biến thể Training/Ceremonial/Travel, ba concept Wind Slash/Shadow Bind/Spirit Guard, không chỉ số hoặc progression; tham chiếu north-star.
- Five paths: đúng năm nhân vật Võ vàng với quyền/bracer, Kiếm xanh-trắng với kiếm, Pháp jade với pháp trận, Cơ với cơ khí/kính, Linh tím với linh phù; toàn thân, cùng scale, không chỉ số/level/skill tự đặt; tham chiếu concept board gốc. Hai hướng đầu ghi Founder Alpha target, ba hướng sau ghi Future paths.
