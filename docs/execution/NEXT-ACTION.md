# NEXT ACTION — Character Hub / icon đúng demo và ownership

## Active — tiếp tục nguồn còn thiếu, giữ base hiện hành
- Worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`; branch `codex/character-hub-v22`; upstream `feature/2d`. Không đổi/reset/restore.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`; check pause/job/claims trước batch. Không tác động phiên khác; cuối lượt WAITING_USER/BLOCKED.
- Canonical layout `redesign-v4-five-tabs`; class demo chỉ là nguồn hình, không tự đổi ID/cấp/chức năng. Không sửa actor/pose/wardrobe/renderer/frozen surfaces.

## v38 — ba icon Võ nam Lv1 đã kiểm Player
- Tiếp tục batchB-abb93ee2bbbf sau gián đoạn; baselineb0114b28. Đã xác minh nguồn10candidate/PID kết thúc/testWIP, không restore hoặc chạy lại generation.
- Nguồn đối chiếu: `vo/detail/15-male-equipment-grid-redraw-source.png`, cộtLv1. Chỉ chọn lower_garment/waist/boots; bảy bản còn lại có da/mặt hoặc sai loại, giữ evidence nhưng không nhập.
- `pack_lgo_equipment_item_icons.py` dùng chung: exact class/item/slot/gender/level, hash nguồn/demo/review, alpha/canvas/budget/duplicate checks. Một profile384→viewport256→120trongcell128; không bbox-fit từng item hoặc scale riêng trongUI.
- Atlas640×384/238156byte, giữ ngân sách250000byte;13module=10cũ pixel-identical+3content mới. Ba binding chỉ Võ–male–Lv1; lớp/giới/tier khác vẫn thiếu khi chưa đăng ký. UI/base/actor không đổi.
- Full graphics EditMode313/313, Python66/66; một Playerbuild0error0warning. Đủ132ảnh/3viewport, đã xem3item selected+Nhân vật ở mỗi viewport (12ảnh). Thêm một vòngcapture10slot dùng chung, không class-specific builder.
- Lượt PC đầu bị FRAME_LIST_MISMATCH do thứ tự expected trongPython khácPlayer. Có testRED→GREEN, sửa expected đúng thứ tự; không sửa hoặc chụp lại evidencePC. Cùngbinary chạy tiếp tablet/mobile; mọi log lỗi giữ nguyên.
- Đây là PARTIAL_VO_ITEM_ART_RUNTIME_VALIDATED, không owner/full-art acceptance. Vải/giày còn tối ở ôtablet nhỏ; actor vẫn là source-pose cũ, chưa tuyên bố outfit đã khớp toàn bộ demo.

## Bước tiếp theo
1. Sửa bảy nguồn Võ còn lỗi theo nhóm, ưu tiên găng/tóc/áo phải hoàn toàn không dính cơ thể; loại mẫu giáp/phụ kiện sai loại. Không lấy các bản bị loại để lấp chỗ trống.
2. Giữ cùng profile/canvas/atlas lifecycle; kiểm pixel budget trước thêm nguồn vì atlas hiện238156/250000byte. Không tự nới ngưỡng, không đẻ builder theo class.
3. Bổ sung đúng itemId/class/slot/gender/level với provenance; kiểm biến thể khác không mượn ảnh. Chỉ dùng `--output` thư mục candidate mới, không ghi đè bộ đang hợp lệ.
4. Skill vẫn44/45 cósprite và nhiều ô chưa khớp demo; Hỏa Tuyến/Thanh Tẩy và classpalette còn việc. Không gọi số lượng ảnh là nghiệm thu mỹ thuật.

## Evidence
`build/character-hub-vo-item-art-v38/`: runtime-review.json, art-review.json, final-editmode.xml, runtime-final/, player-final/LinhGioiOnline.app, owned-processes.json, player-processes-final.json.
`lgo-vo-item-icons-v38-authoring.zip` +`.zip.sha256`:3nguồn alpha, baseatlas và registry portable, demo reference/recipe metadata. Không giải nén vào Resources; README ghi lệnh tái đóng gói. PNG portable trùngbyte, manifest đường dẫn có thể khác.
`CONTINUE / PARTIAL_ITEM_ART / VISUAL_FIX_REQUIRED` — chưa nghiệm thu toàn CharacterHub hoặc thiết bị mobile/tablet thật.

## Bàn giao checkpoint v38
- Lượt trước đã bị chặn tại supervisedcheckpoint; không có commit hoặc checkpoint.log ở lần đó. Đây là lịch sử, không phải lý do tạo lại bộ ảnh.
- Lượt nối tiếp đã kiểm: đúng11file, runtime/source/tool trùng source-hashes cũ, chỉ dònghandoff NEXT-ACTION khác; PID v38 không còn chạy, index rỗng. Chạy lại66Python và source/no3D/shared-skin PASS.
- Reuse XML313/313 và132ảnhPlayer vì input runtime giữ nguyên. Không có generation/build/capture mới trong lượt checkpoint. Kết quả commit/remote cuối xem `build/character-hub-vo-item-art-v38/checkpoint-receipt.json`; không suy ra đãpush từ trạng tháiCONTINUE.
