# Đông Môn illustrated — góc cổng thành, draft 2026-09-10

**Chưa duyệt mỹ thuật.** Lát cắt này hiện thực yêu cầu owner tiếp tục từ checkpoint `bc09282`, theo ảnh gameplay HD illustrated đã gửi. Chỉ mở art preview cho Đông Môn; không mở hệ thống trong HUD/kiếm khí của ảnh tham chiếu. Nhân vật/trang bị 5 class thuộc tab riêng.

## Bố cục và kịch bản

Viewport 1280×720, camera orthographic hiện có. Thành phố và núi xa phủ viewport, xanh sáng/đá ngà/vàng cũ, phía phải thoáng. Cổng đá mái xanh ở trái/giữa, chân cổng và Người Giữ Cổng trên bậc đá cao. Đường đi bộ trước cảnh tiếp nối spawn. NPC trẻ đội nón, áo vải xanh/ngà, chào hướng phải; asset độc lập, không lấy crop board. Player hiện có là placeholder có nhãn, không thay art 5 class.

Trạng thái phải thấy được: đến cổng → đi gần NPC (trong FocusRange, không đứng chồng) → bấm E mở thoại → tiếp tục nhận chỉ dẫn. Không đổi tọa độ tương tác hoặc state machine. Overlay draft chỉ hiện địa danh, một mục tiêu, phím điều khiển và thoại khi mở. Không vẽ HP, shop/chat/quest hệ thống mới từ ảnh tham chiếu.

## Nguồn và demo

Bốn ảnh mới tạo qua built-in imagegen: skyline, cổng alpha, terrain alpha, NPC alpha. Raw PNG và prompt/provenance giữ ở `build/dongmon-art/source/`. Bản NPC sửa nền bị bake checkerboard bị loại; không đưa vào atlas. Demo bố cục trong `build/dongmon-art/demo/index.html` ghép trực tiếp layer riêng, không dùng screenshot/concept nguyên tấm như runtime art.

Pack runtime có hai texture: skyline 1536×864 và atlas RGBA 2048×2048 chứa cổng, NPC và dải terrain, gutter tối thiểu 8px. JSON lưu rect pixel, chiều world, thứ tự vẽ và hệ số parallax. Atlas packing chỉ resize/đặt các asset mới, không crop ảnh reference. Mỗi texture có SHA-256, nguồn imagegen và role runtime; guard vẫn từ chối mọi image ngoài danh sách hoặc sai hash.

## Cách hiện thực có phạm vi riêng

Một component `DongMonIllustratedPreview` và nhánh bootstrap chỉ hoạt động với `--lgo-dongmon-art-preview` hoặc `--lgo-dongmon-art-capture`. Reuse controller/state hiện có; ẩn renderer blockout trong preview và khôi phục khi component bị hủy. Không sửa controller, state, equipment hoặc module/class art. Preview vẽ sprite từ atlas, lặp terrain và thay NPC riêng; skyline dịch nhẹ theo vị trí player để kiểm parallax. Texture tối đa 2048, không mipmap, bilinear/clamp, atlas dùng full-rect UV và alpha gốc, không trộn/khóa source 5 class.

## Kế hoạch thực thi và gate

- [x] Tạo/demo và kiểm alpha/provenance; pack assets mới.
- [x] Test guard với ảnh lạ, thiếu/hash sai, đường dẫn ngoài pack; test Unity resource/atlas rect, parallax và phục hồi renderer/state.
- [x] Implement component preview + bootstrap entry và capture 5 frame PNG kèm manifest PASS chỉ khi thật sự đạt thoại/chỉ dẫn/parallax.
- [x] Chạy EditMode, onboarding smoke, build macOS, capture thường 20 frame và capture art riêng. Review Player mắt thật; không dùng screenshot demo làm runtime evidence.
- [x] Guard no-3D/no-old-source, audit frozen/diff, review code; chuẩn bị evidence/status và checkpoint fast-forward trong worktree riêng.

Gate mỹ thuật owner: bố cục, chiều sâu, palette/material, tỷ lệ, silhouette và độ đọc ở Player 1280×720. Chưa coi nhân vật placeholder hay test xanh là production-art pass. Không nhân rộng map trước khi owner duyệt capture cụ thể này.

## Evidence thực tế và giới hạn

EditMode 139 pass / 0 fail / 1 skipped; 5 guard test pass; onboarding smoke Complete; macOS build Succeeded; baseline capture 20 BMP và art capture 5 frame đã chạy. Review mắt các frame art 01–05: cổng/NPC đứng trên terrace, không còn khe hở chân cổng, mép dưới kín, thoại gọn không che NPC; skyline thay đổi vị trí với parallaxDelta=-0.126. Baseline 01/03/20 vẫn là renderer cũ, preview chỉ được mở bằng flag.

Evidence cuối: `build/dongmon-art/player-final/`; capture lỗi đầu ở `player/` được giữ (LaunchServices -600 khi kích hoạt sớm), bản trước chỉnh placement ở `player-retry/`. Tool hiện chờ `UnloadTime:` rồi kích hoạt đúng app. Browser không khả dụng; HTML demo chưa được browser-QA, kết luận mỹ thuật dựa trên Player thực. Source prompt briefs và source hashes nằm trong evidence, không phải asset reference cũ.

Đánh giá: **DRAFT sẵn sàng owner review**, không production-art PASS. Player placeholder chưa hợp phong cách; không tự sửa art 5 class của tab kia. Pack PNG ~5.46 MB; texture RGBA chưa nén tương đương ~21 MB, budget nén mobile là gate sau khi hướng art được duyệt. Hai texture mới được allowlist chính xác bằng đường dẫn/hash; guard vẫn chặn mọi ảnh reference hoặc ảnh ngoài pack.

Batch có 2 texture + metadata Unity, component/test/capture/packer/guard và handoff. Nếu file budget mặc định 18 bị vượt do metadata, dùng gate checkpoint có ngưỡng 22 files / 1000 changed lines cho đúng một feature này; không mở thêm scope.
