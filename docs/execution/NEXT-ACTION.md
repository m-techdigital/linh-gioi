# NEXT ACTION — Character Hub / artwork Skill theo demo

## Active — tiếp tục mỹ thuật Võ/Linh/Cơ, không đóng goal theo số sprite
- Worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`. Check pause/recovery/job/claims trước batch; không lấy quyền hoặc dừng process phiên khác. Cuối lượt dùng `v3.finish` CONTINUE khi còn việc hợp lệ.
- Canonical UI vẫn `redesign-v4-five-tabs`. Giữ base dùng chung, frame/content tách rời, exact ownership, actor và frozen surfaces. Không đổi tên/cấp/gameplay từ nhãn demo.

## Checkpoint đã kiểm
- V43 đã commit/push `ac6fe903`: Hỏa Tuyến có nội dung và Liên Kích cam/vàng; 5 file/157 dòng. Closure giữ input runtime, chạy lại79Python, kiểm lại315graphics/132PNG, không build lại.
- V44 thay riêng `linh_skill_3` bằng lá chắn linh lực xanh và lõi phù văn, bỏ hình cánh/trái tim. Nhãn Hộ Linh trong demo là VISUAL_MOTIF_REFERENCE_ONLY_NOT_GAMEPLAY_ALIAS; 5exact designBindings trước giữ nguyên.
-48module RGBA khác giữ nguyên, gồm frame và các nội dung Hỏa Tuyến/Liên Kích/Thanh Tẩy đã xử lý. Atlas1024×1024/883203byte;49module và45skill có hình, không phải45art đã đạt.
- Nguồn chọn384px, alpha chung r160..170, padding32→448; không gamma, bbox-fit hoặc scale/offset riêng trong UI. Không sửa code runtime, actor, library hoặc atlas trang bị.
-315/315graphics không fail/skip;79/79Python. Một build0lỗi/0cảnh báo và132PNG. Đã xem6ảnh: Linh Thuẫn được chọn ở3viewport; Võ/Cơ/Tiềm năngPC. Không thấy clipping/notice/frame mới sai; rune vẫn dày ở ôtablet, chưa nghiệm thu toàn mỹ thuật.
- Bốn candidate đầu được giữ; hai Chấn Kình sai giáp/huy hiệu và một Linh dạng la bàn bị loại. Lượt sửa subject Chấn Kình bị model checker chặn trước khi lưu, dừng ngay; không có candidate sửa được nhập hoặc bỏ checker.
- Ghi script package_sources.py bị tool từ chối, readbackENOENT. Nguồn/registry/replay và provenance có sẵn; CHƯA có ZIP portable v44. Không claim portable replay hoặc tái chạy inference.

## Bước tiếp theo cụ thể
1. Tiếp tục nét và hình tác động Hỏa Tuyến/co_skill_8, Liên Kích/vo_skill_1 và Thanh Tẩy/linh_skill_4 từ vùng demo đã pin. Giữ source384 và bố cục đăng ký; không tăng gamma hoặc phóng riêng UI. Không chạy script viết dở hoặc vòng lặp model đã bị chặn.
2. Chấn Kình/vo_skill_4 chưa được thay: cần artwork quyền/xung kích thật, không lấy hai candidate giáp/huy hiệu để lấp. Xử lý nguồn theo review mới, giữ checker và không tự gán alias Xung Kích.
3. Hoàn thiện gói authoring v44 khi thao tác được phép; hiện dùng selected/, artwork-registry.json và các nguồn hash-pin để đối chiếu. Không tạo lại inference/build chỉ vì đóng lượt hoặc cập nhật tài liệu.
4. Chỉ chuyển READY_REVIEW khi toàn bộ goal5tab đã sát design và audit Player; phiên kết thúc lượt không phải nghiệm thu goal.

## Evidence
`build/character-hub-shockward-v44/`: runtime-review.json, runtime-final/, player-final/LinhGioiOnline.app, source-comparison.png (nguồn, KHÔNG Player), selected/, generation.json, initial-review.json, art-review.json, artwork-registry.json, candidate-proof.json, import-proof.json, full-editmode.xml.
`checkpoint-receipt.json` ghi commit/remote sau push. V43 receipt nằm trong `build/character-hub-attack-motifs-v43/`.
`CONTINUE / PARTIAL_LINH_WARD_MOTIF_CORRECTION / VISUAL_FIX_REQUIRED` — không nghiệm thu toàn class, Character Hub hoặc thiết bị mobile/tablet thật.
