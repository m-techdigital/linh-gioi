# NEXT ACTION — Character Hub / shared Skill artwork

## Active — hoàn thiện 5 icon còn thiếu và độ rõ của nguồn
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `origin/feature/2d`. Không tạo/đổi branch hoặc worktree.
- Phiên `S-LGO-SKILL-20260916-C9B4`: check pause/claims trước batch; không nhả quyền hoặc dừng job của phiên khác. V2 đã enroll, chưa được giao task V2; CLI v1 tương thích.
- Scope chỉ Character Hub 5 tab theo `redesign-v4-five-tabs`; không mở gameplay/class-pose/wardrobe/renderer/frozen surfaces hoặc các task M0 từ tài liệu lịch sử.

## v31 — đã nhập 11 inner và kiểm Player
- Tiếp nối 341c1153. Bổ sung Liên Kích, Hộ Thể, Kình Lực; Cơ Nỏ, Linh Cơ, Thiết Vệ, Cơ Trận; Thanh Tẩy, Linh Phù, Hộ Mệnh, Linh Giới.
- 40/45 skill UI có art: Võ 6/9, Kiếm 9/9, Pháp 9/9, Cơ 8/9, Linh 8/9. Có art không đồng nghĩa art final; dữ liệu tên/level/mô tả giữ nguyên.
- Một atlas 1024×1024 / 836114 byte, 44 module = 43 inner + một frame. Toàn bộ 33 module đã publish trước batch giữ nguyên từng pixel; UI/base, library, importer1024, actor renderer không đổi.
- Nguyên nhân sinh sai trước đây: prompt/style-reference không khóa hình học. Dùng composition underpaint 384px để hướng refinement, cùng strength0.60/IP0.25; guide không vào game. Chỉ 11 output đã xem được đăng ký, 5 mục chưa đạt hoặc bị lọc không nhập.
- Full graphics EditMode 305/305 không fail/skip; test11ID mới RED trước import. Python52/52 gồm 3 candidate checks. Replay PNG+manifest trùng byte; kiểm mọi33module cũ và library không đổi.
- Một build Player 0 lỗi/0 cảnh báo;87ảnh. Đã xem5class×3viewport + Tiềm năngPC: nội dung mới hiện ở cây/ô trang bị/chi tiết, không thấy cắt/chồng mới hoặc rò frame.
- Visual vẫn FIX_REQUIRED: vài hình mới mảnh/nhỏ trong vòng (Cơ Nỏ/Linh Cơ, phù/pendant trên tablet), và nguồn cũ còn khác nét/nền. Không dùng scale/offset từng class để che vấn đề source.

## Batch kế tiếp
1. Kiểm checkpoint và `build/character-hub-motif-v31/missing-artwork.json`: Phá Giáp, Phản Đòn, Chấn Kình, Hỏa Tuyến, Hồi Phục.
2. Xử lý đúng hình biểu đạt còn thiếu, không tái sinh mò cùng giả định đã thất bại. Ảnh bị loại giữ lại làm evidence, không gọi lại hoặc dùng ảnh ẩn trước lọc.
3. Củng cố silhouette/độ rõ ở source theo cả nhóm; giữ một profile canvas/aperture và không fit bbox riêng từng icon. Không thêm UI builder theo class hoặc mượn icon HUD.
4. Registry phải tích lũy đủ31 inner bổ sung hiện có. Revision nguồn ghi rõ ID cũ được đổi và pixel-diff tất cả phần còn lại.
5. Gom asset rồi full graphics + Player3viewport + eye review, cập nhật state và checkpoint/push qua supervisor; không gọi final chỉ vì đủ45 ảnh.

## Evidence / provenance
`build/character-hub-motif-v31/`: review.json, art-review.json, guide/source recipes, generation*.json, artwork-registry.json, full-editmode.xml, runtime/, owned-processes.json, player-processes.json, authoring ZIP+SHA.
`runtime/` là ảnh Player thật. `source-review-*.png` là bảng asset; `guides/` chỉ underpainting và không được nhập Resources. ZIP authoring portable không có folder cha, không giải nén source board vào Resources.
`CONTINUE / VISUAL_FIX_REQUIRED / ARTWORK_INCOMPLETE` — còn5 ảnh, chưa nghiệm thu production art hoặc thiết bị mobile/tablet thật.
