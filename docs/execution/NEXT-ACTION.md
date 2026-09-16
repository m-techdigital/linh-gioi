# NEXT ACTION — Character Hub / shared Skill artwork

## Active — hoàn thiện16 icon còn thiếu trên một base
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `origin/feature/2d`; không tạo/đổi branch/worktree.
- Phiên `S-LGO-SKILL-20260916-C9B4`: đọc SESSION-CONTRACT, check pause, claim đúng tài nguyên; giữ v1 tương thích, client V2 đã enroll/inbox resolved. Không tự lấy claim của phiên khác.
- Goal vẫn Character Hub5tab theo `redesign-v4-five-tabs`; không mở M0/auth/combat/class-pose/wardrobe/renderer/frozen surfaces từ tài liệu lịch sử.

## v30 + v30b — đã qua runtime gate trong worktree
- Đã được cấp claim Unity sau khi phiên chibi nhả; RED thiếu phap_skill_1 xác minh đúng trước nhập. Full graphics EditMode305/305 không fail/skip; một build0error/0warning;87frame qua command/manifest validator hiện hành.
- 10 nội dung mới: Pháp1/2/5/9; Võ5/9; Cơ2/3/7; Linh6. Sửa nguồn Trọng Lực/Nguyên Tố (Pháp6/7) từv29; thử nghiệm mới3/4/8 bị loại do sai nghĩa/brief, không nhập.
- Hiện29/45 skill UI có ảnh: Võ3/9, Kiếm9/9, Pháp9/9, Cơ4/9, Linh4/9; còn16 thiếu. Không thay tên/level/mô tả hoặc giả dữ liệu gameplay để giống design.
- Một atlas1024×1024/674383byte,33module=32inner+1frame. So publishedv29: thêm10, sửa2, giữ21module cũ pixel-identical; riêng13module Kiếm/category/frame ban đầu đều nguyên vẹn. So candidatev30: chỉ2ID thay pixel,31module giữ nguyên.
- Full test kiểm45selection qua tree/detail/tooltip/missing-state và19frame instance. Đã eye-review15ảnh5class×3viewport +Tiềm năngPC; không thấy cắt/chồng mới hoặc rò frame Skill sang Tiềm năng.
- Kết quả kỹ thuật không đóng visual goal. Nguồn Pháp3/4/8, Võ7/Cơ4/Linh hiện hành còn lệch style/nền/độ lấp đầy; không đổi scale/offset theo class để che source.

## Batch tiếp theo
1. Kiểm checkpoint/pause/claims và `build/character-hub-source-coherence-v30b/missing-artwork.json`; ưu tiên các ô mặc định còn trống: Liên Kích, Cơ Nỏ, Hồi Phục.
2. Giữ một registry tích lũy20inner bổ sung; revision nguồn phải ghi đúngID/hashes/review. Không tái sinh frame/category/Kiếm, không lấy icon HUD/ảnh bị loại lấp chỗ thiếu.
3. Chốt motif và style source cả nhóm; kiểm common canvas384+padding32→448, xuất128 cùng aperture. Không brute-force lại giả định đã thất bại; nếu motif không đúng thì giữ missing rõ ràng.
4. RED availability choID mới, candidate pixel-diff/metadata/replay, nhập một lượt, full graphics + Player3viewport + eye audit trước checkpoint tiếp.

## Evidence / provenance
- `build/character-hub-source-coherence-v30b/`: review.json, import-proof.json, artwork-registry.json, generation.json, full-editmode.xml, runtime/, owned-processes.json, player-processes.json, authoring ZIP+SHA.
- `runtime/` là ảnh thật từ Player; `phap-source-review.png` chỉ asset review. Nguồn/candidate v30 vàv29 cùng các thử nghiệm bị loại giữ nguyên để đối chiếu.
- Capture dùng builder/validator cũ; wrapper phiên ghi PID từng Player, không terminate khi pause/timeout. Các claim tạm chỉ được trả khi job đã kết thúc.
`CONTINUE / VISUAL_FIX_REQUIRED / ARTWORK_INCOMPLETE` — chưa nghiệm thu art toàn5tab hoặc thiết bị mobile/tablet thật.
