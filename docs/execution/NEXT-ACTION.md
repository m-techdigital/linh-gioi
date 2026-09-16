# NEXT ACTION — Character Hub / shared Skill artwork

## Active — Hỏa Tuyến còn thiếu; tiếp tục fidelity của nguồn chung
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `origin/feature/2d`. Không đổi branch/worktree hoặc restore baseline cũ.
- Phiên `S-LGO-SKILL-20260916-C9B4`: check pause/recovery và claim trước batch; V3 đã đọc, chưa có task opt-in được giao; CLI v1 tương thích. Không nhả claim hoặc dừng job của phiên khác.
- Scope vẫn Character Hub5tab theo `redesign-v4-five-tabs`, không mở M0/auth/gameplay/class-pose/wardrobe/renderer/frozen surfaces từ lịch sử.

## v32 — bốn nội dung mới đã qua Player, KHÔNG visual final
- Kế thừa6036d501. Thêm Phá Giáp, Phản Đòn, Chấn Kình và Hồi Phục. Hiện44/45skill có art: Võ9, Kiếm9, Pháp9, Cơ8, Linh9. `co_skill_8` Hỏa Tuyến vẫn thiếu.
- Tạo guide bố cục mới; refine4source. Phản Đòn lần đầu thành nhân vật, Hỏa Tuyến giống lưỡi dao: loại, sửa hình học cả hai và kiểm lại2source. Phản Đòn sau sửa đạt motif; Hỏa Tuyến bị lọc, không nhập hoặc lấy ảnh ẩn trước lọc. Không lặp thêm cùng cách trong batch.
- Hồi Phục là illustration nguyên bản vẽ trực tiếp bằng recipe, không phải guide tái sử dụng hoặc output đã bị lọc. Nguồn còn phẳng hơn phong cách Kiếm: giữ visual debt này, không gọi production art final.
- Cùng canvas384+padding32→448 và aperture hiện hành; atlas1024×1024/885216byte,48module=47inner+1frame. Mọi44module cũ giữ nguyên từng pixel. UI/base/library45skill/importer/renderer không đổi.
- Test candidate RED thiếu4→GREEN3/3; Unity REDvo_skill_2→full graphics305/305 không fail/skip. Python52/52; replay PNG+manifest và portablePNG exact; original healing recipe pixel replay exact.
- Một Player build0error/0warning,87frame. Đã xem5class×3viewport +Tiềm năngPC; không thấy cắt/chồng mới hoặc rò frame. Số lượng icon không phải mức nghiệm thu visual.

## Batch kế tiếp
1. Check checkpoint/pause/registry; xử lý đúng `co_skill_8` Hỏa Tuyến. Không dùng hai bản thử đã loại, không thay bằng skill khác hoặc HUD. Cần hình hỏa lực/tracer rõ ở nguồn và128px, không đơn thuần hai lưỡi sáng.
2. Gom phần cần polish ở source: độ lấp đầy của Phản Đòn/Chấn Kình và một số Cơ/Linh; nét/nền/phong cách của source cũ còn không đồng nhất. Không thêm offset/scale hoặc builder theo class để che vấn đề nguồn.
3. Registry tích lũy35inner bổ sung hiện hành; nguồn revision phải có ID/hash/review và pixel-diff các module không đổi. Giữ frame/category/Kiếm gốc.
4. Gom source một batch, RED khi thêmID, rồi full graphics+Player3viewport/eye audit trước checkpoint/push qua supervisor. Không mở screen khác hoặc gọi final vì đủ45ảnh.

## Evidence / provenance
`build/character-hub-last-five-v32/`: art-review.json, review.json, source-painting-manifest.json, guide/refine recipes, registry, raw/corrections/authored, modules/, full-editmode.xml, runtime/, owned-processes.json, player-processes.json, ZIPauthoring+SHA.
`runtime/` là ảnh Player thật; `source-composite-review.png` chỉ ảnh nguồn/lắp lớp. Guides không vào Resources. Gói authoring là nguồn/recipe/provenance, không giải nén source board vào Unity.
`CONTINUE / VISUAL_FIX_REQUIRED / ARTWORK_INCOMPLETE` — còn1icon thiếu, chưa nghiệm thu art toàn5tab hoặc mobile/tablet thiết bị thật.
