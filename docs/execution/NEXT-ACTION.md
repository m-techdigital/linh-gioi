# NEXT ACTION — Character Hub / Skill artwork

## Active owner steer — một base cho cùng chức năng
- Giữ `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`; baseline v27 `affb970c`, upstream `origin/feature/2d`. Không đổi branch/worktree hoặc mở screen khác.
- Phiên chat này: `S-LGO-SKILL-20260916-C9B4`. Đọc `/Users/minhdc/Tools/mcp-session-manager/SESSION-CONTRACT.md`; check pause trước batch, claim worktree/runtime/branch đúng nhu cầu. Không dùng lại ID này cho chat khác.
- Canonical `redesign-v4-five-tabs`; không sửa class/pose/wardrobe/source renderer/camera/frozen.
- v27: một library 45 record, một loader/base cho 5 class; bỏ HUD fallback. 19 instance Skill dùng chung một vòng, nội dung rời; Tiềm năng giữ factory chung.
- Runtime hiện vẫn là atlas 512×512/252747 byte, 1 frame +12 nội dung (9 skill Kiếm/3 category). Còn thiếu 36 artwork thật; không coi test fixture là hình dùng trong game.

## v28 — đường nạp artwork bổ sung đã nối vào pipeline hiện hành
- `tools/pack_lgo_skill_icons.py --artwork-registry <registry.json> --output <candidate-dir>` nhận nguồn PNG/sha256/size và rect theo skillId; không thêm builder theo class.
- Registry version1: `review.status=SELF_REVIEWED`, `review.evidence`; `sources=[{id,path,sha256,size}]`; `parts=[{id,sourceId,rect:[x,y,w,h]}]`. Đường dẫn tương đối lấy theo thư mục registry.
- Chỉ nhận ID trong skill-library, không trùng/ghi đè module cũ/frame; rect vuông >=128px, nằm trong source; giữ canvas/aperture chung, không fit từng icon theo bounding box.
- Packer kiểm đầu vào và budget trước ghi output. Không registry: PNG và manifest giữ nguyên từng byte. Có 36 module: candidate 1024×1024/49 cell, vòng cũ vẫn chỉ có một sprite.
- Guard kích thước/import/budget của Skill chỉ mở theo metadata intake hợp lệ; baseline 512px/400000 byte và các pack khác không được nới.
- Evidence `build/character-hub-artwork-intake-v28/`: RED/GREEN, 48/48 Python, source/frozen unchanged proof; ví dụ registry được đánh dấu DRAFT, không nhập runtime.
- Lượt này không chạy Unity/build/capture: toàn bộ `client/Unity` không đổi; kết quả 304/304 và Player v27 là lịch sử, không phải test mới.

## Việc tiếp theo — artwork thật, không UI riêng
- [ ] Tạo/đăng ký 36 inner artwork theo `build/character-hub-skill-base-v27/missing-artwork.json`; kiểm đúng tên/ý nghĩa, không dùng HUD/Kiếm thay thế hoặc crop screenshot gameplay.
- [ ] Dùng registry chung và pipeline v28, review PNG rời ở kích thước hiển thị, rồi mới nhập atlas/import policy có provenance.
- [ ] Claim runtime thích hợp trước inference/Unity; kiểm registry để tránh phiên khác. Claim branch `feature/2d` trước push; không tự thu hồi claim của người khác.
- [ ] Sau thay asset thật: kiểm tree/equipped/detail/selection 5 class; Player PC/mobile/tablet và eye audit. Cập nhật evidence/PID/result/tests/next; kết thúc lượt ở WAITING_USER hoặc BLOCKED.

`CONTINUE / VISUAL_FIX_REQUIRED / ARTWORK_INCOMPLETE` — base và intake đã có; 36 artwork không được coi là hoàn thành chỉ vì test xanh.
