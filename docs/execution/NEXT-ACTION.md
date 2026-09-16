# NEXT ACTION — Character Hub / nội dung icon đúng demo

## Active — đồng nhất chất lượng nguồn trên base hiện hành
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`. Không reset, restore hoặc đổi branch/worktree.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`. Trước mỗi batch kiểm pause/recovery/job/claims; không tác động process hoặc claim phiên khác. Cuối lượt dùng `v3.finish` và WAITING_USER/BLOCKED đúng kết quả.
- Canonical UI vẫn `redesign-v4-five-tabs`; demo class là căn cứ hình ảnh, không tự đổi tên/cấp/chức năng gameplay. Actor/pose/wardrobe/renderer và frozen surfaces không thuộc batch.

## v40 — đã nhập đủ 10 icon Võ nam Lv1; chưa nghiệm thu toàn mỹ thuật
- Bổ sung găng, áo ngoài, giáp vai/ngực bất đối xứng và vòng đồng tua đen theo cột Lv1 demo Võ nam. Không nhập lại các draft dính cơ thể, họa tiết mặt trời hoặc phụ kiện sai loại.
- Bốn hình dựng rỗng native384, một lượt hoàn thiện chất liệu local, khóa alpha và bảo vệ viền ngoài từ hình dựng. Canvas384/viewport256→120 trong cell128 dùng chung; không scale/offset riêng trong UI.
- Cộng sáu registration v39: đủ10/10 nội dung cho exact Võ–male–Lv1. Các class/giới/cấp khác không mượn; renderer trên người giữ nguyên, không claim toàn bộ đồ đang mặc khớp thumbnail.
- Atlas640×640/248683byte, ngân sách250000 không tăng;20module. Toàn bộ16module và6binding cũ giữ nguyên. RGBround4 là encoding đã có, không phải lossless cho nguồn mới; alpha chính xác. Texture cao hơn không có nghĩa giảm bộ nhớ GPU.
- Full graphics EditMode313/313 không fail/skip; test thiếu găng RED rồi GREEN sau nhập. Một Player build v40 có0error/0warning và132ảnh,44ảnh mỗi viewport.
- Lượt trước xem10ảnh PC/mobile; closure đã xem thêm5ảnh tablet. Không thấy cắt/chồng mới hoặc notice thiếu ảnh ở bốn món; áo và tua đen vẫn nhỏ/tối trong ô tablet, hình mới còn góc cạnh và chưa đồng nhất toàn bộ nét/nền/độ sáng.
- Closure chạy lại69/69 Python và3guard, kiểm132PNG/manifest/kích thước, replay/hash/source/ZIP. Không sinh nguồn, build hoặc capture lại khi input runtime không đổi.
- Các chặn tool cũ và log RED vẫn giữ riêng làm lịch sử. Không suy ra phiên khác chỉ từ chuỗi finish/report event. Thao tác package đã hoàn tất qua đường công cụ hiện hành, không đổi cấu hình MCP.

## Bước tiếp theo
1. Audit độ rõ của toàn bộ10icon Võ ở kích thước ô nhỏ và so với demo; sửa nguồn theo nhóm, không tint hoặc phóng từng icon trong UI. Không coi10/10 là art acceptance.
2. Tiếp tục icon Skill có mapping demo rõ, ưu tiên bản sắc Võ/Linh và nội dung còn sai; Hỏa Tuyến còn thiếu, Thanh Tẩy chưa thay. Không sinh chỉ theo9tên catalog hoặc tự sửa ID/cấp khi demo còn mâu thuẫn.
3. Vật phẩm, slot/category, thuộc tính Tiềm năng, portrait/petSkill là các vai trò riêng. Giữ frame/component chung nhưng đăng ký nội dung theo đúng ownership và nguồn tương ứng.
4. Chỉ bổ sung class/giới/cấp khác khi có nguồn đủ rõ và exact registration; không sửa renderer hoặc gameplay để làm cho icon có vẻ khớp.

## Evidence và bàn giao
`build/character-hub-four-item-redraw-v40/`: `prepared-sources/`, `registry.json`, `runtime-review.json`, `closure-proof.json`, `source-hashes.json`, `full-editmode.xml`, `runtime-final/`, `player-final/LinhGioiOnline.app`.
`lgo-four-item-redraw-v40-authoring.zip` +`.zip.sha256` chứa4nguồn RGBA, atlas nền v39, registry portable, demo chỉ-tham-chiếu và recipe; không có font/model/binary. Không giải nén trực tiếp vào Resources. PortablePNG tái sinh đúng byte; README có lệnh pack vào thư mục mới.
`checkpoint-receipt.json` là xác minh commit/remote sau checkpoint. Không dùng `blocked.json` cũ làm trạng thái hiện hành.
`CONTINUE / TEN_VO_STARTER_ITEM_CONTENTS / VISUAL_FIX_REQUIRED` — chưa nghiệm thu Character Hub năm tab hoặc mobile/tablet thiết bị thật.
