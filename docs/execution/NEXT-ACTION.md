# NEXT ACTION — Character Hub / tiếp tục mỹ thuật Skill theo demo

## Active — Võ/Linh và hỏa lực Cơ còn cần hoàn thiện nét, không gamma
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`; SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`. Không restore/đổi branch/worktree hoặc sửa actor/frozen.
- Check pause/recovery/job/claims trước batch. `v3.finish` cuối lượt với CONTINUE khi còn việc hợp lệ; không NEED_USER hoặc tự nghiệm thu chỉ vì vừa checkpoint.
- Canonical UI `redesign-v4-five-tabs`; source class trong selected demos. Giữ base/frame/content tách riêng và exact ownership; không gán alias tên/cấp/gameplay từ ảnh.

## v42 đã chốt, v43 có output mới được kiểm Player
- Recovery v42 bảo toàn8WIP, chạy lại79Python, reuse315graphics/132PNG bằnghash; Thanh Tẩy giữ3linh thể aqua theo đúng ôdemo. Commit2bd8b7cc đã push/remoteverify. Buildv42 có0error/46warning, không gọi0warning.
- v43 thêm Hỏa Tuyến co_skill_8: ba luồng hỏa lực cam/trắng và điểm phát cơ giới; thay Liên Kích vo_skill_1 bằng ba ảnh quyền liên hoàn cam/vàng. Không có tên/cấp/mô tả mới. Catalog45/45cósprite, không phải45ảnh đạt mỹ thuật.
- Source native384+padding32→448, cùng packer128/frame. Bản nét mảnh giữ ởnative/; bản chọn native-reviewed/ vẽ lại mảng quyền và dải lửa, không gamma hoặc bboxfit. Model paint-pass bị chặn ởappend, không được ghi/chạy hoặc dùng đường khác; không claim model hoàn thiện ảnh.
- Hỏa Lực Liên Thanh/Liên Quyền trong demo chỉ là VISUAL_MOTIF_REFERENCE_ONLY_NOT_GAMEPLAY_ALIAS. Năm exact-label designBindings giữ nguyên; hai visualReferences được lưu trong registry/review, không nới gate exactlabel.
- Atlas1024×1024/893243byte,49module:45skill+3category+1frame.47module khác giữ RGBAexact, itematlas/actor/UIruntime/library không đổi.
- Test availability đọc45skill từcatalog chung thay danh sáchID viết tay. RED xác nhận co_skill_8 thiếu → GREEN.315/315graphics,79/79Python, một build/capture132ảnh. Eye8ảnh gồm HỏaTuyến+LiênKích ở3viewport, Linh và Tiềm năngPC.
- Không thấy clipping/frame/notice mới sai ở ảnh đã xem. Nét còn hình học; nòng nhỏ và luồng phụ HỏaTuyến, các mảng quyền LiênKích chưa có độ tinh tế như demo. Chưa owner/whole-art acceptance.

## Bước tiếp theo cụ thể
0. Recovery v43 đã xác minh năm WIP giữ đúng hash, chạy lại 79 test Python và kiểm 132 PNG/ZIP. Dùng Python hệ thống có Pillow cho test ảnh, Python 3.12 cho test UI; lần gọi gộp sai interpreter được giữ riêng. Unity 315/315 và Player được tái sử dụng vì input không đổi. Receipt sau checkpoint ghi commit/remote; không build hoặc tạo nguồn lại.
1. Rà nét và độ lấp đầy Hỏa Tuyến/Liên Kích ở64–128px với đúng vùngdemo đãpin; thêm chi tiết hình tác động hữu ích, không tănggamma, tint hoặc phóngUI để che nét. Không chạy refine_attacks.py viết dở hoặc gọi lại pass bị từ chối qua đường khác.
2. Tiếp nhóm Võ/Linh còn sai motif rõ trong Player: Chấn Kình đang mang hình đá/băng và Linh Thuẫn mang cánh/trái tim. Đọc lại đúng ôdemo, lập visual-only mapping khi nhãn khôngexact; không dùng vật phẩm/portrait thay hiệu ứng kỹ năng.
3. Giữ các nguồn đã cải thiện/5designBindings khi tích lũy registry. Review source→Player ởnode tương ứng qua --skill-node; không thêm wrapperclass hoặc đổi actor.
4. Chỉ chuyển READY_REVIEW khi thực sự đủ goal5tab. Số lượngsprite/testxanh/checkpoint không thay mỹ thuật hoặc device gate.

## Evidence
v43: `build/character-hub-attack-motifs-v43/`: runtime-review.json, runtime-final/, player-final/LinhGioiOnline.app, source-comparison-reviewed.png (nguồn, KHÔNG Player), import-proof.json, full-editmode.xml, package-proof.json, checkpoint-receipt.json saupush.
`lgo-attack-motifs-v43-authoring.zip` +`.zip.sha256`: source/recipe/cumulative registry/visual references; nativeRGBA và portablePNG replayexact. Khôngfont/model/binary, khôngunzipboard vàoResources.
`CONTINUE / PARTIAL_ACTION_MOTIF_ALIGNMENT / VISUAL_FIX_REQUIRED` — không nghiệm thu toànclass, CharacterHub hay thiết bịmobile/tablet thật.
