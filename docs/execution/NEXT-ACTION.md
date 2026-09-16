# NEXT ACTION — Character Hub / Skill theo demo, không thay bằng gamma

- Recovery closure: WIP8file được giữ nguyên, 79Python chạy lại; reuse315graphics/132PNG vì inputruntime khớpSHA. Đã xem lại Thanh Tẩy3viewport và nguồn so demo; build v42 có0error/46warning, không phải0warning. Không sinh/build lại chỉ để khôi phục lượt.
## Active — tiếp tục nội dung Võ/Linh và Hỏa Tuyến, giữ pipeline chung
- Worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`. Không đổi/reset/restore source.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`; check pause/recovery/job/claims trước batch, không lấy quyền/process phiên khác. Cuối lượt `v3.finish` với CONTINUE khi còn việc hợp lệ; không tự nghiệm thu.
- UI authority vẫn `redesign-v4-five-tabs`; demo class là tham chiếu hình ảnh, không tự đổi runtimeID/tên/cấp/chức năng. Không sửa actor/pose/wardrobe/renderer hoặc frozen surfaces.

## v42 — Thanh Tẩy có nội dung mới trên Player, không phải cả bộ đã đạt
- Baseline35639c7e. Thanh Tẩy `linh_skill_4` được thay ngọn lửa xanh bằng ba linh thể +hạt sáng xanh ngọc/trắng theo đúng ô Thanh Tẩy trong demo Linh. Native384, raster antialias768 nội bộ, padding32→448; không crop board làm asset, không thêm vòng ngoài hoặc gamma.
- Đã xem demo→nguồn→Player. Nguồn còn đơn giản/góc cạnh hơn demo; chưa owner/whole-art acceptance. Linh/Vo còn nhiều nguồn sai hình tác dụng hoặc palette.
-47module khác giữ RGBA exact, frame/library/item atlas/UI base không đổi. Atlas1024×1024/886859byte;5designBindings exact. Catalog vẫn44/45cóhình, Hỏa Tuyến chưa có mới.
- Hai write bị công cụ từ chối: phần vẽ Hỏa Tuyến/Liên Kích và bản tinh chỉnh Thanh Tẩy tiếp. Readback xác nhận phần đầu vắng; bản sau không dùng. Không đổi đường ghi để vượt chặn hoặc lấy draft sai để lấp.
- Capture chung nhận `--skill-node 0..8`, default5 giữ nguyên; chuyển thành `--lgo-character-hub-skill-node` cho Player và kiểm lại manifest theo node đã yêu cầu. Không rẽ nhánh UI theo class. Lượt v42 chọn node3 cho cả5class để nhìn đúng Thanh Tẩy trong inspector.
-315/315graphics không fail/skip;79/79Python; một build/capture132ảnh. Đã xem8ảnh: Thanh Tẩy3viewport, Skill Võ/CơPC, Kiếmtablet, Phápmobile và Tiềm năngPC. Không thấy clipping/notice/frame mới sai. Node parser và Python command có RED→GREEN.

## Bước tiếp theo — không đóng goal chỉ vì batch đã chốt
1. Hỏa Tuyến: demo Cơ có nhãn “Hỏa Lực Liên Thanh” và luồng hỏa lực cam/trắng. Chỉ dùng làm tham chiếu hình tác động, không ghi thành EXACT_LABEL hoặc tự alias ID/cấp; packer exact-label hiện vẫn giữ nguyên. Chưa có artwork mới từ lượt v42.
2. Liên Kích: đối chiếu chuyển động quyền liên hoàn cam/vàng ở demo Võ; không dùng hình đá/huy hiệu xanh hiện tại làm chuẩn. Không giả Liên Quyền và Liên Kích đã cùng gameplay contract.
3. Các kỹ năng có nhãn/nguồn khớp rõ tiếp tục qua registry tích lũy, giữ5designBindings. Với nhãn khác, cần ghi rõ phạm vi tham chiếu hình ảnh, không nới exact-label gate hoặc đổi tên để làm đủ.
4. Dùng capture `--skill-node` chọn đúng vị trí cần nhìn; một base/frame dùng chung. Không tạo thêm wrapper chọn class hoặc tăng gamma để che source chưa đúng. Giữ draft/lỗi làm evidence; chỉ push batch đã test/Player audit.

## Evidence
`build/character-hub-skill-motifs-v42/`: native/, paint_actions.py, generation.json, art-review.json, demo-before-after.png (nguồn, KHÔNG Player), artwork-registry.json, import-proof.json, full-editmode.xml, runtime-review.json, runtime-final/, player-final/LinhGioiOnline.app.
`lgo-thanh-tay-v42-authoring.zip` +`.zip.sha256`: native, recipe, cumulative source/registry và demo reference-only; portablePNG/nativeRGBA replay đúng byte. Không có font/model/binary; không unzip vào Resources.
`checkpoint-receipt.json` xác minh sau push. `CONTINUE / PARTIAL_THANH_TAY_DEMO_ALIGNMENT / VISUAL_FIX_REQUIRED` — không nghiệm thu toàn Character Hub hoặc mobile/tablet thật.
