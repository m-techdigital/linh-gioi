# NEXT ACTION — Character Hub / nội dung icon đúng nguồn

## Active — đăng ký artwork đúng vật phẩm và tiếp tục đối chiếu class demo
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`. Không đổi/reset/restore source.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`. Check pause/recovery, job và claims trước batch. Không release/kill job phiên khác. Kết thúc turn phải WAITING_USER/BLOCKED.
- Canonical vẫn `redesign-v4-five-tabs`; class demos là nguồn hình ảnh, không tự thay tên/cấp/chức năng gameplay. Không sửa actor/pose/wardrobe/renderer hoặc frozen surfaces.

## v37 — sửa sở hữu icon, không coi thiếu ảnh là visual hoàn tất
- Đã tiếp tục WIP thật còn lại từ lượt suy nghĩ thất bại: baseline120b420f, batchB-32cf114ae874. Có code/test/build cũ; đã xác minh job kết thúc và lưu incoming snapshot, không restore hoặc replay lệnh cũ.
- `EquipmentItemIconCatalog` tra exact classId +itemId +slot +gender +level; thiếu registration trả null. Không mượn hình theo slot hoặc crop renderer cũ. Duplicate/wildcard/thiếu field/sprite/variant bị từ chối.
- `BindLgoItemIconContent` là base chung cho rail nhân vật, row/grid và inspector. Thiếu ảnh vẫn giữ ô/click/selection; notice tái sử dụng, không giữ nhầm ảnh khi đổi class hoặc chuyển sang bình máu. Có positive integration fixture kiểm đủ đường hiển thị, không nhập sprite fixture vào Player.
- Equipment atlas PNG nguyên byte; `itemBindings: []` vì10 hình slot cũ chưa được đăng ký cho item/biến thể nào. Player hiện thấy tên ô +“Chưa có ảnh”. KHÔNG có10art mới, KHÔNG gọi đạt design.
- Review Player còn phát hiện badge tìm kiếm lặp sức chứa làm tràn sang inspector; đã rút còn số kết quả, giữ sức chứa trong tooltip và khi bỏ tìm kiếm, không đổi geometry/canvas.
- Full graphics EditMode312/312 no fail/skip; Python54/54. Final Player102ảnh; đã xem10ảnh cuối: Nhân vật/tìm-bình-máu3viewport, Rương đồ/Skill/Tiềm năng/Linh thúPC. Không thấy tràn header sau sửa hoặc leak notice sang ba tab khác.
- Trong lượt recovery có2build/capture: chốt input WIP, rồi sửa overflow thấy bằng mắt. Build/capture cũ trước gián đoạn và mọi log RED giữ riêng, không đổi tên thành final.

## Bước tiếp theo
1. Đối chiếu itemId/class/slot/gender/level thật với nguồn demo tương ứng trước khi thêm `itemBindings`; không gán nguyên bộ Kiếm cho mọi class để làm đầy ô. Giữ selector/base chung, không đổi renderer để phù hợp icon.
2. Nội dung Skill vẫn44/45 cósprite nhưng chưa khớp mỹ thuật; Hỏa Tuyến còn thiếu, Thanh Tẩy chưa được thay. Tiếp tục nhóm có mapping demo rõ, không chỉ tint hoặc sinh theo tên suy diễn.
3. Vật phẩm/phân loại/pet/skill có nguồn và vai trò khác nhau. Không dùng slot/category/pet portrait làm ảnh nội dung chỉ vì đã có sprite.
4. Khi có registration thật: test pixel/provenance/identity, kiểm cả cùngitem khácgender/tier và đổiitem trongcùngslot; build/capture Player đúng3viewport, không nhận fixture thay artwork.

## Evidence
`build/character-hub-item-ownership-v37/runtime-review.json`, `resume-review/` (snapshot), `final-editmode-green.xml`, `runtime-final/`, `player-final/LinhGioiOnline.app`, `owned-processes.json`, `player-processes-final.json`, `checkpoint-receipt.json` sau push.
`runtime/` là trước gián đoạn; `runtime-resume/` là trước sửa search overflow. Chỉ `runtime-final/` là tập cuối. Không có source-art ZIP mới vì không tạo/thay PNG.
`CONTINUE / ITEM_OWNERSHIP_CORRECTED / VISUAL_ARTWORK_INCOMPLETE` — không nghiệm thu toàn Character Hub hoặc mobile/tablet thật.
