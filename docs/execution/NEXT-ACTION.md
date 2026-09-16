# NEXT ACTION — Character Hub / icon theo demo gốc

## Active — tiếp tục đúng design, không chạy theo số lượng ảnh
- Giữ `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`; v36 kế thừa9fa22c3a. Không đổi/reset/restore source.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`; check pause/recovery/job/claims trước batch. Registry V3 không thay quyền owner nghiệm thu.
- Canonical UI: `redesign-v4-five-tabs`. Nội dung class phải đối chiếu class demo đã pin, không dùng Kiếm làm màu/motif cho mọi class. Không đổi actor/pose/wardrobe/renderer/frozen/gameplay.

## v36 — Hộ Thể đã vào Player; chưa nghiệm thu cả class
- Hộ Thể (`vo_skill_6`) thay giáp xanh bằng khiên vàng 2D theo đúng ô Hộ Thể trong demo Võ. Nguồn native384, antialias2x, padding32 cố định vào448; không copy/phóng pixel demo làm runtime, không bbox-fit hoặc offset riêng.
- Chỉ1 nội dung đổi,47module khác và vòng ngoài giữ nguyên pixel; atlas1024²/887653byte,48module. Bốn designBindings gồm ba liên kết v35 được bảo toàn.
- Packer chặn registry tích lũy âm thầm bỏ toàn bộ/một phần designBindings: RED hai case → wholepacker13/13GREEN. Đây là kiểm metadata, không duyệt mỹ thuật/gameplay.
- Capture loop chung thêm trạng thái chọn node5 cho cả5class, không đổi renderer. Mỗi viewport34ảnh, tổng102; đã xem15ảnh selected +Tiềm năngPC. Hộ Thể xuất hiện đúng cây và inspector trên3viewport; không thấy cắt/chồng mới trong ảnh đã xem.
- Full graphics EditMode305/305 không skip; Python53/53; một Player build0error/0warning. Pair PNG/manifest và portablePNG replay đúng byte. Mobile/tablet là viewport macOS, chưa thiết bị thật.
- Thanh Tẩy: append code nguồn bị tool chặn và xác minh chưa ghi; không chạy lại qua đường khác, không có ảnh mới. Nguồn v35 sai tượng/bệ vẫn bị loại.

## Batch kế tiếp
1. Kiểm checkpoint/source/job và claim đúng tài nguyên. Đọc `runtime-review.json` v36, không dùng `review.json` source-stage cũ để kết luận Player chưa chạy.
2. Khi thao tác được cho phép, tiếp tục Thanh Tẩy theo demo và nội dung linh lực, không tượng/bệ. Không tự đổi ID/tên/cấp để khớp ảnh. Gom các nội dung có mapping chức năng rõ thành batch class nhất quán.
3. Nhiều ảnh Võ/Linh/Pháp/Cơ cũ vẫn chưa sát demo; Hỏa Tuyến còn thiếu. Số44/45 chỉ là số vị trí có sprite, không tỷ lệ art đã đạt. Các nhãn/cấp mâu thuẫn giữ unresolved, không ép11mốc thành9skill.
4. Batch icon vật phẩm riêng: resolver đang trả UI icon theo slot trước class/item. Cần ownership itemId/class/set qua dữ liệu chung; không copy builder theo class hoặc sửa actor/wardrobe.
5. Giữ vòng/component/canvas chung. Review demo → nội dung rời → Player; trạng thái selected của capture hiện là node5 theo cùng loop, không đại diện đã eye-review mọi lựa chọn.

## Evidence / source
`build/character-hub-symbols-v36/`: runtime-review.json (final batch), import-proof.json, runtime/ (102PlayerPNG), player/LinhGioiOnline.app, native-artwork.json, source-comparison.png (KHÔNG phải Player), owned-processes.json.
`lgo-skill-symbols-v36-authoring.zip` +`.zip.sha256`: nguồn/registry portable sau runtime review; không unzip vào Resources. Bản `*-candidate.zip` và review.json là lịch sử trước nhập, không nghiệm thu cuối.
V34 audit còn ở `build/character-hub-design-reconciliation-v34/`; v33 matte test hoãn được giữ nguyên byte trong `build/character-hub-demo-alignment-v35/deferred-v33/`.
`CONTINUE / PARTIAL_DEMO_ALIGNMENT / VISUAL_FIX_REQUIRED` — chưa owner acceptance hoặc toàn goal5tab. Chỉ supervised checkpoint/push sau kiểm scope/evidence, rồi kết thúc batch/turn và WAITING_USER.
