# NEXT ACTION — Character Hub UI

## Authoritative resume
- Worktree: `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`; branch `codex/character-hub-v22`; upstream `origin/feature/2d`.
- Design duy nhất: `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs/`.
- Bản WIP đầy đủ v23 được bảo toàn tại `/Users/minhdc/Projects/LGO-UI-Checkpoint-Split-20260916-014527`; manifest và binary patch đã kiểm hash.
- Không sửa class/pose/wardrobe/source renderer/camera hoặc frozen surfaces; main checkout dirty giữ nguyên.

## Active — tích hợp WIP thành checkpoint có thể kiểm chứng v24
- A: Nhân vật/Rương đồ và inspector chung đã tái dựng độc lập; full EditMode có graphics 293/293, 0 fail/skip.
- A-runtime: `build/character-hub-checkpoints-v24/A-runtime/{pc,mobile,tablet}/`, 87 ảnh; đã xem Nhân vật và Rương đồ trên ba viewport: đủ 25 ô vuông, không cắt hàng cuối, tên/vitals/action rõ và không chồng.
- Lượt test đầu dùng -nographics có 1 test pointer bị Ignore; giữ log, không tính là PASS. Lượt có graphics chạy đủ và thay thế kết luận đó.
- [ ] Commit A sau Python/shared/frozen/budget; chưa push riêng khi các nhóm kế thừa vẫn chờ tích hợp.
- [ ] B: tích hợp Kỹ năng 3×3 và Linh thú/typography/roster vào base chung; test và Player lại đúng candidate.
- [ ] C: tích hợp icon Tiềm năng frame/content tách riêng, generator/atlas/provenance; kiểm không dựng lại frame khi đổi class.
- [ ] So byte-for-byte toàn bộ source cuối với WIP đã bảo toàn (ngoại trừ docs và drift ProjectSettings được loại có kiểm chứng); kiểm final Player rồi push fast-forward qua supervisor.
- [ ] Tiếp tục fidelity shared ornament/frame, không mở screen ngoài năm tab.

## Gate
`CONTINUE / VISUAL_FIX_REQUIRED`: checkpoint kỹ thuật không đồng nghĩa toàn bộ năm tab đã được owner nghiệm thu; mobile/tablet là viewport macOS, không phải thiết bị thật.
