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
- [x] A đã commit `6165e23f`; 9 file/529 dòng, Python 32/32, frozen/no-3D/no-source PASS; chưa push riêng.
- [x] B đã commit `8ca19ff5`: Kỹ năng 3×3, Linh thú/typography/roster đã tái dựng; EditMode 294/294 không skip; B-runtime có 87 ảnh, đã xem Kỹ năng và Linh thú trên ba viewport, không cắt/chồng, hiện trọn hai skill Linh thú.
- [x] C1: chuẩn bị module UI core 224×224 có source/hash, generator và importer; Python 5/5, no-source/no-3D PASS. Chưa thay topology runtime; code UI vẫn giống B.
- [ ] C2: tích hợp icon Tiềm năng frame/content; candidate cuối đã kiểm 296/296 và 87 ảnh, chỉ reuse evidence khi toàn bộ hash input trùng C-verified-inputs.json.
- [ ] So byte-for-byte toàn bộ source cuối với WIP đã bảo toàn (ngoại trừ docs và drift ProjectSettings được loại có kiểm chứng); kiểm final Player rồi push fast-forward qua supervisor.
- [ ] Tiếp tục fidelity shared ornament/frame, không mở screen ngoài năm tab.

- Audit bổ sung: report_lgo_change_budget.py bỏ sót dòng file mới; dùng thêm effective_budget.py trong evidence để đếm đủ, không stage/nới ngưỡng né gate.

## Gate
`CONTINUE / VISUAL_FIX_REQUIRED`: checkpoint kỹ thuật không đồng nghĩa toàn bộ năm tab đã được owner nghiệm thu; mobile/tablet là viewport macOS, không phải thiết bị thật.
