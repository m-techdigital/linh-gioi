# NEXT ACTION — Character Hub / Skill artwork

## Active — hoàn thiện artwork trên base chung, không mở UI theo class
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `origin/feature/2d`.
- Phiên MCP hiện hành: `S-LGO-SKILL-20260916-C9B4`; check pause/claims trước batch. SESSION-CONTRACT V2 đã đọc; CLI v1 vẫn tương thích. Không đổi task/worktree hoặc chạy migration/restart theo thông báo.
- Canonical duy nhất: `redesign-v4-five-tabs`; không mở class/pose/wardrobe/renderer/camera/frozen surfaces.
- v27 có một library45skill +một base frame/content cho5class. v28 mở intake qua `--artwork-registry`; không viết packer/UI builder riêng.

## v29 — 10 artwork thật đã nối vào Player, còn26 thiếu
- Tạo nguồn local với DreamShaper8/Diffusers đã có, không tải model; giữ safety checker. Pilot4 ảnh bị loại. Batch36ID có2 ảnh bị lọc; sau review chỉ10 nội dung được chọn,26 không promotion.
- IDs mới: vo_skill_7; phap_skill_3/4/6/7/8; co_skill_4; linh_skill_1/3/8. Nguồn384px +padding32 cố định → canvas448px; không fit từng hình theo bbox.
- Atlas1024×1024/474211byte gồm23module:12inner cũ +10inner mới +một frame. Toàn bộ13module cũ giữ nguyên từng pixel. 19 frame instance vẫn dùng chung; runtime UI/base/library không đổi.
- Full graphics EditMode305/305, no fail/skip; kiểm toàn bộ45 lựa chọn qua tree/detail/tooltip/missing state và frame identity. Python49/49. Một Player build/capture mới:0error/0warning trong build này;87ảnh.
- Đã xem5class×3viewport +Tiềm năng PC. Không thấy cắt/chồng do batch; ảnh mới lên tree/equipped/detail. Đây là PARTIAL_DRAFT_RUNTIME_VALIDATED, KHÔNG phải visual final. Nền/palette/độ lấp đầy của một số motif còn cần polish ở source.
- Sửa lỗi thật của packer: không-registry hoặc registry thiếu ID không được âm thầm xóa artwork đã đăng ký. Có RED→GREEN; giữ nguyên output khi bị từ chối.

## Bước tiếp theo
- [ ] Hoàn thiện26 artwork còn thiếu theo `build/character-hub-artwork-v29/missing-artwork.json`, không dùng lại ảnh bị loại hoặc icon HUD/Kiếm để lấp.
- [ ] Củng cố nền/palette/độ lấp đầy của source mới khi đối chiếu canonical; không thêm scale/offset/nhánh UI theo class để che vấn đề source.
- [ ] Registry tiếp phải tích lũy cả10ID đã đăng ký; `tools/pack_lgo_skill_icons.py --artwork-registry ... --output build/...` trước khi thay output runtime.
- [ ] Giữ một frame master; kiểm một lượt5class/3viewport sau batch nguồn mới. Không coi test xanh là duyệt visual hoặc production gameplay.

## Evidence / provenance
- `build/character-hub-artwork-v29/`: review.json, art-review.json, artwork-registry.json, generation*.json, raw PNG, source board, final-editmode.xml, runtime/, owned-processes.json.
- Nguồn+registry/recipe được giữ trong `lgo-skill-artwork-v29-authoring.zip` cùng SHA256; đây là authoring/provenance, không unzip vào Resources hoặc chạy lại script generation mù quáng.
- Image tool từng tạo nhầm dashboard giả: đã loại, không phải evidence MCP/Player. Evidence Player duy nhất trong runtime/ do Unity capture.
- Process registry từng đầy: PID thật có thêm trong result/evidence; không sửa manager hoặc xóa record của phiên khác. Trước kết thúc chuyển WAITING_USER/BLOCKED; chỉ release claim của chính phiên khi đã xác minh job kết thúc.

`CONTINUE / VISUAL_FIX_REQUIRED / ARTWORK_INCOMPLETE` —19/45 skill có art;26 còn thiếu. Mobile/tablet là viewport macOS, không phải thiết bị thật.
