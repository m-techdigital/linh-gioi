# NEXT ACTION — Character Hub UI

## Authoritative resume
- Worktree: `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`; branch `codex/character-hub-v22`; upstream `origin/feature/2d`.
- Design duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.
- Toàn bộ WIP v23 có bản bảo toàn kiểm hash: `/Users/minhdc/Projects/LGO-UI-Checkpoint-Split-20260916-014527`.
- Không sửa class/pose/wardrobe/source renderer/camera hoặc frozen surfaces; main checkout dirty giữ nguyên.

## Active — hoàn thiện shared frame / hierarchy theo design
- WIP đã chia theo dependency, không stage né budget: A `6165e23f` (9 file/529 dòng), B `8ca19ff5` (6/277), C1 `3710578e` (8/191 tính cả file mới).
- C2 nối bộ icon Tiềm năng: một frame master và sáu nội dung độc lập, atlas 512×256/109488 byte; topology, inspector và cost dùng chung sprite. Không thay class data hoặc renderer.
- Candidate C2 khớp từng byte với toàn bộ input đã chạy C-editmode/C-player; proof: `build/character-hub-checkpoints-v24/C2-input-replay-proof.json`.
- EditMode có graphics: A 293/293, B 294/294, C 296/296; không fail/skip. Lượt A -nographics từng Ignore pointer test đã được thay bằng graphics, không tính skipped là PASS.
- Mỗi candidate A/B/C có 87 frame tại `build/character-hub-checkpoints-v24/{A,B,C}-runtime/`; đã xem Nhân vật/Rương đồ và Skill/Pet trên ba viewport, Tiềm năng PC/default + mobile/tablet selected, và bốn tab PC để kiểm frame rò.
- Python cuối 7 asset +25 governance +7 capture; replay atlas/chín PNG byte-for-byte; no-source/no-3D/frozen PASS.
- [ ] Commit/push C2 qua supervisor sau budget và xác nhận remote không diverge.
- [ ] Batch tiếp: inspector dùng hoa văn cùng base với shell, hierarchy khung rõ hơn, Skill selected bám design; không thêm khung trùng lên icon Tiềm năng.
- [ ] Test trạng thái, build/capture một lượt cả năm tab trên PC/mobile landscape/tablet, review bằng mắt trước checkpoint tiếp.

## Gate
`CONTINUE / VISUAL_FIX_REQUIRED`: checkpoint kỹ thuật không đồng nghĩa owner nghiệm thu toàn bộ năm tab. Một số icon Skill ngoài Kiếm còn generic, visual hero/chrome chưa hoàn toàn sát canonical; không sửa class source để che gap.
Mobile/tablet là viewport trên macOS, không phải thiết bị thật. Budget kiểm thêm dòng file mới bằng `build/character-hub-checkpoints-v24/effective_budget.py`; không nâng ngưỡng.
