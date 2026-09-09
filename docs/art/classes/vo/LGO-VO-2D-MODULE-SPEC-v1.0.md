# Võ — 2D Module Spec v1.0

Ngày 2026-09-09. Quyết định chọn reference theo quyền owner giao; phần thiết kế chỉnh sửa dưới đây là **DRAFT**, chưa phải owner visual approval hoặc production art. Chỉ áp dụng Võ, nam/nữ; không mở gameplay hoặc class khác.

## Quyết định thiết kế

Chọn cặp outfit #04/#05 làm nguồn ngoại hình; #19/#18 làm nguồn grid nam/nữ; #17 làm nguồn module map; #21 làm nguồn mood VFX. Không chọn nguyên bảng nào làm asset pack sạch. Đã mở và review trực tiếp cả 23 PNG, không suy luận chất lượng từ tên “mới nhất” hay dòng “không dính da thịt” trong ảnh.

Giữ tóc nam ngắn đen, nữ buộc cao; nam quần võ rộng thu ống, nữ shorts/hạ y linh hoạt. Đen làm nền, đỏ làm dấu nhấn Võ, vàng đồng biểu đạt kim loại, ngà cân sáng. Không lấy xanh băng Lv100 từ #18/#19; VFX chỉ cam/vàng ấm tách riêng. Không thay tỷ lệ hai base hiện hành bằng số đầu/thước đo trong board.

Không dùng ba hướng song song: bộ overview hệ slot cũ chỉ giữ lịch sử; grid mới dùng để truy vết item và lỗi; outfit chọn dùng để giữ silhouette. Khi các ảnh lệch nhau, quy tắc sở hữu slot thắng hình vẽ; vẽ lại item riêng, không cắt full outfit.

## Thiết kế lại phần tách lớp

- main_weapon: găng quyền ở khớp đấm/bàn tay, chấm dứt tại cổ tay; bên trong rỗng, không ngón người. Có preview đôi trái/phải.
- arm_guard: ống cẳng tay thuôn, chấm dứt trước cổ tay, không phần găng; khác hẳn hình khối weapon kể cả ở Lv100.
- head_hair: chỉ tóc/băng/trâm, không đầu/mặt/cổ. Tóc mới thay lớp tóc cơ bản cùng vùng che, không dính vào áo.
- inner_top: áo vải sát người; outer_top: jacket/vạt ngắn riêng, không áo trong hoặc dây đai. Nữ starter phải đọc ra áo ngoài riêng, không sao chép áo trong lần hai.
- lower_body: quần nam/shorts nữ có cạp may đơn giản; waist_belt sở hữu toàn bộ khóa, dây đỏ, tua và ngọc cố định vào đai.
- shoulder_chest_guard: mảnh giáp vai/ngực cùng dây cài; bỏ thân áo/ống tay áo. Không chép lại giáp đang có trên outer_top.
- footwear: boot mũi kín đế thấp, không chân/da; bỏ biến thể cao gót hở ngón.
- class_accessory: charm nắm đấm rời nhỏ; không gộp thành nhiều slot hoặc lặp ngọc đai.

## Kịch bản, trạng thái, tương tác cần chứng minh sau này

2D-03 Võ Lv1, cùng hai base tại Linh Thành/Đông Môn; SCN-001 chào/nói chuyện và SCN-002 tiếp cận/interact là ngữ cảnh đọc nhân vật. Batch chỉ chuẩn hóa tài liệu. Các prototype gameplay hiện hữu không bị phủ nhận hoặc thay đổi.

Thiết kế cần hỗ trợ: base khi tháo hết đồ vẫn kín đáo; equip/unequip từng slot; mặc đầy đủ; phối khác level; hair front/back; idle/đi/quay người/interact/ngồi với lớp trước/sau hợp lý. Pose vận động nằm trong task animation riêng; không lấy pose combat board làm quyền mở combat mới. Chưa có demo clean side-view để xác nhận khả năng tháo/lắp: đây là gate còn thiếu, front-view board không thay được.

## Kế hoạch asset có thể giao

1. Vẽ clean demo lv001 nam/nữ: 10 item rời mỗi giới, một montage lắp đủ/tháo từng slot và góc side-view dùng chung base. Không copy board vào Unity.
2. Review rõ bàn tay/quyền khí/cẳng tay, áo trong/ngoài/giáp và đai/quần; nếu sai sửa item, không nới checklist.
3. Làm lv050 nam/nữ để thử độ phức tạp; chỉ khi hai mốc đạt mới mở rộng 11 mốc (220 preview item).
4. Dàn 6 board đúng tên chuẩn từ thiết kế đã kiểm. Gate no-source-images hiện vẫn chặn ingest vào branch; giữ reference ngoài repo cho tới task asset riêng.

Chưa sinh ảnh mới trong batch này. Prompt vẽ lại đã có ở `docs/art/LGO-CLASS-IMAGE-PROMPT-TEMPLATES-v1.0.md`. Không công bố bản chỉnh spec như một ảnh đã vẽ xong.

## Kiểm kê nguồn đã review

Nguồn gốc: `/Users/minhdc/Projects/2D/Vo`. ID chỉ là số thứ tự kiểm kê theo tên file, không level. Giữ ảnh ngoài source; SHA-256 giúp kiểm lại đúng bản, không chứng minh giấy phép production.

| ID | File gốc | Kết luận |
| --- | --- | --- |
| 01 | `01b0e2c0-559b-4175-9ae9-cfb358d971e6.png` | Tham khảo đỏ/đen; sơ đồ slot cũ thiếu lớp áo và lặp 3 phụ kiện. |
| 02 | `ChatGPT Image 22_11_37 9 thg 9, 2026.png` | Dự phòng overview; không lấy bảng 5 món làm chuẩn 10 slot. |
| 03 | `ChatGPT Image 22_11_42 9 thg 9, 2026 (1).png` | Dự phòng silhouette; không lấy aura tràn làm trang phục. |
| 04 | `ChatGPT Image 22_11_44 9 thg 9, 2026 (2).png` | CHỌN nguồn outfit nam; giữ quần võ và progression, giảm hào quang Lv100. |
| 05 | `ChatGPT Image 22_11_44 9 thg 9, 2026 (3).png` | CHỌN nguồn outfit nữ; giữ tóc buộc cao và silhouette; boot đế thấp. |
| 06 | `ChatGPT Image 22_11_45 9 thg 9, 2026 (4).png` | Loại làm module chuẩn: thiếu head_hair/weapon riêng, áo/giáp/đai gộp, găng có tay. |
| 07 | `ChatGPT Image 22_11_46 9 thg 9, 2026 (5).png` | Loại làm module chuẩn: áo có torso, quần có đùi, găng có ngón, giày có chân. |
| 08 | `ChatGPT Image 22_11_46 9 thg 9, 2026 (6).png` | Loại hệ slot: bao tay/găng chiến trùng, thiếu quần/áo/tóc, quá nhiều phụ kiện. |
| 09 | `ChatGPT Image 22_11_47 9 thg 9, 2026 (7).png` | Dự phòng mood VFX; mô tả combat/unlock không authoritative. |
| 10 | `ChatGPT Image 22_11_48 9 thg 9, 2026 (8).png` | Tham khảo silhouette/chất liệu; số đo 7.5 đầu không tự thay base runtime. |
| 11 | `ChatGPT Image 22_12_49 9 thg 9, 2026 (1).png` | Dự phòng overview; tóc nữ có mặt, hệ slot cũ thiếu lớp áo/giáp. |
| 12 | `ChatGPT Image 22_12_49 9 thg 9, 2026 (2).png` | Dự phòng overview; trang bị cũ 3 phụ kiện, không dùng làm chuẩn module. |
| 13 | `ChatGPT Image 22_12_59 9 thg 9, 2026 (1).png` | Dự phòng grid nam: đủ tên 10 slot nhưng tóc có đầu, găng có ngón, áo/quần có đai. |
| 14 | `ChatGPT Image 22_12_59 9 thg 9, 2026 (2).png` | Loại grid nữ khỏi bản chọn: còn body nhiều hàng, giày cao gót/hở ngón. |
| 15 | `ChatGPT Image 22_13_16 9 thg 9, 2026 (1).png` | Dự phòng grid nam: bố cục rõ nhưng mặt/tay/torso còn dính; hạ y đổi form. |
| 16 | `ChatGPT Image 22_13_17 9 thg 9, 2026 (2).png` | Dự phòng grid nữ: cùng lỗi body, đai, găng; không dùng để extraction. |
| 17 | `ChatGPT Image 22_13_40 9 thg 9, 2026 (1).png` | CHỌN nguồn sơ đồ ý tưởng; sửa callout sai vị trí, găng có ngón, áo gộp vai/đai và thứ tự layer thiếu tóc. |
| 18 | `ChatGPT Image 22_13_40 9 thg 9, 2026 (2).png` | CHỌN grid nữ để thiết kế lại: tóc/áo trong sạch hơn bản cũ; weapon/guard còn ngón, áo ngoài có vai da/giáp; Lv100 lệch xanh. |
| 19 | `ChatGPT Image 22_13_40 9 thg 9, 2026 (3).png` | CHỌN grid nam để thiết kế lại: tóc rời, hàng/cột rõ; weapon/guard trùng và có ngón; áo ngoài/giáp, quần/đai còn gộp; Lv100 xanh. |
| 20 | `lgo-vo-ho-so-nam-nu-lv001-100.png` | Tham khảo đời sống/chất liệu; ghi chú production cũ không dùng cho pipeline 2D. |
| 21 | `lgo-vo-ky-nang-chien-dau-vfx.png` | CHỌN mood VFX cam/vàng và pose; thiếu đủ 11 mốc, không chốt skill/timing hoặc gameplay. |
| 22 | `lgo-vo-nu-turnaround-khoi-hanh-va-tien-trinh.png` | Tham khảo tóc nữ/góc mặt; không chọn pipeline hoặc starter cầu kỳ ghi trong ảnh. |
| 23 | `lgo-vo-tong-quan-nam-nu-lv001-100.png` | Tham khảo mood xã hội; loại chỉ dẫn pipeline cũ và danh sách skill khỏi chuẩn mới. |

## Fingerprint ảnh nguồn

| ID | Kích thước | SHA-256 |
| --- | --- | --- |
| 01 | 1536×1024 | `9c99020d9de994c4e4aa64018291c8d72156044bebf7bf14ce90a76e11d1a9c2` |
| 02 | 1448×1086 | `fb42654835527788683a9a105518261f058d6d90a4892a0d659c653354015b6b` |
| 03 | 1448×1086 | `f6ac61d354cc2e4121c333f183a71d8f32b9edfad20cbda5f7559da5005d9e9d` |
| 04 | 1448×1086 | `75f4d7adbf415fabeab433e865fe25cd792ab24fc85ee9e7cda5104fb24c3091` |
| 05 | 1448×1086 | `4a121ce3e84070f02b6976ff576ba758c8eb46858e14058eaa3766b4bd7bd2ad` |
| 06 | 1448×1086 | `99bf9860b785a1af8ede9bc6b4ea3a033b0437c97a73239c08d5b15b213d575d` |
| 07 | 1448×1086 | `12bbacbd5ee720593075ce63fa37b66a964530e0bb65d0faba489d45c42f8595` |
| 08 | 1448×1086 | `50f2050bac49a562176ba5e3b0b9e957232e5c367dfabc42af845ad9269df629` |
| 09 | 1448×1086 | `ae87a82e5a110ac09fbd050e2c03874398501bf8348295f886394ddf2e012d05` |
| 10 | 1448×1086 | `20f223a4a2fd0f65c80633d8f90a172d708923a1457bbd8040dbac0156163f9c` |
| 11 | 1536×1024 | `115c1bcd4a64e15baed91f7a7d26695795fa0ac6097648335bcfc2ada91b6d5f` |
| 12 | 1536×1024 | `de99563db90f582b13c8d51146e1e508b7c75ab1cb3c937b65332d20bc373730` |
| 13 | 1491×1055 | `bc9567a7e3a83a9593a6931309f47c0e61ffedaa303eed32cc7d89ea9af1f22b` |
| 14 | 1491×1055 | `dc278d618caaded8cf2ac6a9d3ffb3c61cf28b5ad8b7152e48df8a7462a268f5` |
| 15 | 1672×941 | `a70c9c297c704fd326ae5b7a2263fca67fcaf07344492a92b8da5280b9103ab6` |
| 16 | 1672×941 | `ea2269e75a2d7dd210e113b46feea298724d72d374e41d20b9fe7d974b898508` |
| 17 | 1672×941 | `5705f259ebf6f4b50162843880e84972aa039c1186ca5913a0e689b84b45fcf6` |
| 18 | 1672×941 | `08f0f878c309b50c34bf985b0f0bb8974ee0f4fe8a52e9c9387a8dfc2702bf94` |
| 19 | 1672×941 | `b99fe6ae31c732bdf0bcbc99aebdd4ce45ea14fe5beaa5ea3ca0eb2dae1586de` |
| 20 | 1491×1055 | `8b0b65089323f29fe46a470c08a1e737149fdd9ee9f8d839f4e7b26abb1f86d4` |
| 21 | 1491×1055 | `584cfc08836db6d8ac9daa08b3e104130e009b9714ad9c51893ab368df4d7d0a` |
| 22 | 1491×1055 | `470a79ecdc4718341ec172ee02266a63aca003774124303c8afc2999dc553182` |
| 23 | 1448×1086 | `fcd21193923c32808c42a04234c0a5575f6aadbc51cff50379663be5ed1b8e43` |
