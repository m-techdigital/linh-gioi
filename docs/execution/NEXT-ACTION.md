# NEXT ACTION — Character Hub / icon theo demo gốc

## Active — v35: sửa ba nội dung có đối chiếu demo trực tiếp
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`; baseline81d2d2a2. Không đổi/reset/restore branch/worktree.
- Phiên `S-LGO-SKILL-20260916-C9B4`; task operator mới `T-69b4b27c4b39` đã nhận, batch `B-0876b01f8bd4` đang giữ worktree+Unity. Không tự hoàn thành task, bật auto hoặc duyệt recipe.
- Owner v34 yêu cầu bám demo mỗi class và các nhóm icon; không quay lại sinh đủ45ảnh theo tên suy diễn. Layout/frame/component chung, nội dung đúng class/chức năng. Không đổi actor/pose/wardrobe/renderer/frozen/gameplay.

## Batch đã nhập và kiểm Player, chưa nghiệm thu toàn goal
- Đối chiếu5nhãn trùng trực tiếp trên2board đã pinSHA: Võ Phá Giáp/Phản Đòn/Hộ Thể; Linh Triệu Linh/Thanh Tẩy. Không thay ID/tên/cấp hoặc coi nhãn trùng là phê duyệt cơ chế gameplay.
- Đã redraw5source từ vùng demo của đúng class, không dùng embedding/palette Kiếm. Crop board chỉ là guide; không crop/phóng ảnh đó vào Resources.
- Eye-review chọn3: Phá Giáp/Phản Đòn là bóng võ giả và quyền khí cam-vàng; Triệu Linh là biểu tượng triệu hồi linh lực lam-tím thay chân dung cáo. Loại Hộ Thể vì thành kim loại/floor, Thanh Tẩy vì thành tượng/bệ. Giữ toàn bộ bản bị loại.
- Candidate48module/1024px/888779byte: chỉ3nội dung đổi,45module khác và frame giữ nguyên pixel. Vẫn44/45skill có ảnh; không dùng số lượng làm visual acceptance.
- Packer hiện hành thêm optional designBindings: classId/runtimeName/demoLabel/hash/vùng reference phải hợp lệ, quan hệ EXACT_LABEL_VISUAL_ONLY; không tạo builder theo class. Test2RED→wholepacker12GREEN. Liên kết này không tự kiểm chất lượng hình ảnh.
- [x] Ba nội dung đã nhập; graphics EditMode305/305 không skip, Python51/51; build0error/0warning;87frame, eye5class×3viewport +Tiềm năngPC. Đúng3module đổi,45module giữ pixel; replay exact và portablePNG exact.
- [ ] Chỉ commit/push khi batch kiểm đủ; đóng batch, tạo checkpoint/turnV3 và WAITING_USER. Không tự task.complete.

## Việc tiếp theo sau batch
- Tiếp tục nhóm Võ/Linh có đối chiếu chức năng rõ; xử lý các tên/cấp mâu thuẫn bằng crosswalk, không tự ép11mốc demo thành9skill.
- Item thumbnail resolver hiện trả bộ iconKiếm theo slot trước class: sửa bằng ownership itemId/class/set ở batch riêng, không thay actor/wardrobe.
- Category/HUD/status/attribute là icon chức năng dùng chung; pet portrait theo petId, pet skill theo petSkillId. Không lấy portrait làm spell chỉ vì cùng chủ đề.
- Hỏa Tuyến vẫn thiếu; nhiều artwork class cũ vẫn bị owner bác. Đây chưa phải nghiệm thu toàn CharacterHub.

## Evidence / WIP
`build/character-hub-demo-alignment-v35/`: mapping.json, source-registration.json, reference-review.png, source-comparison.png, art-review.json, candidate-atlas/, published/, prior-wip/, generation.json và logs. Source comparison KHÔNG phải Player.
Audit gốc: `build/character-hub-design-reconciliation-v34/DESIGN-AUDIT.md` và review-notes.json. build_audit.py v34 viết dở, không dùng như evidence đã chạy.
Test matte v33 chưa có implementation đã chuyển nguyên byte vào build/character-hub-demo-alignment-v35/deferred-v33, không đưa vào checkpoint asset này. Không xóa công sức cũ hoặc bỏ test thuộc implementation đã chấp nhận.
`CONTINUE / VISUAL_FIX_REQUIRED / PARTIAL_DEMO_ALIGNMENT` — bộ icon toàn5class chưa đạt design.
