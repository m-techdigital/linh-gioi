# NEXT ACTION — Character Hub / tiếp tục hoàn thiện artwork Skill

## Active
- Giữ worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`. Kiểm pause, claims, job và tác dụng phụ trước batch; không tác động phiên khác. Cuối lượt gọi `v3.finish` với CONTINUE khi còn bước hợp lệ.
- Canonical vẫn `redesign-v4-five-tabs`. Giữ shared base, vòng/content độc lập, exact ownership, actor và frozen surfaces. Không đổi tên/cấp/gameplay theo nhãn demo.

## v45 — hai nguồn được thay, chưa nghiệm thu toàn mỹ thuật
- Hỏa Tuyến `co_skill_8`: thay nét hình học bằng các dải hỏa lực có nét lửa và tia sáng; loại nền vuông trung tính bằng alpha, không tăng gamma hoặc sửa RGB nguồn.
- Thanh Tẩy `linh_skill_4`: ba linh thể có nét vẽ và áo linh lực, thay ba hình khối cũ. Loại vòng sáng sinh kèm bằng mask theo tọa độ native 384 px; giữ các đầu, thân và áo. Không dùng vòng sinh kèm làm frame thứ hai.
- Bản mask đầu sai tọa độ làm mất một phần mặt và giữ vòng bên phải đã bị loại. Kiểm điểm ảnh tái hiện lỗi rồi pass sau khi sửa. Bản Hỏa Tuyến còn nền vuông cũng bị loại trước build.
- Liên Kích mới bị loại vì chỉ có một bàn tay, mất ảnh quyền liên hoàn. Hai variant bị model checker chặn không được lưu, nhập hoặc retry. Giữ nguồn Liên Kích v43 và Chấn Kình cũ, không nhận đã sửa hai món đó.
- Atlas 1024×1024, 880.771 byte; vẫn 49 module và 45 skill có hình. Chỉ hai nội dung trên đổi; 47 module khác, frame, năm exact designBindings, skill library, item atlas và code runtime giữ nguyên.
- Source native 384 px, padding chung 32 px vào 448 px; không bbox-fit, scale/offset riêng trong UI hoặc chỉnh gamma. Alpha và PNG portable tái dựng từ raw được kiểm riêng; không hứa model sinh lại đúng pixel.
- Một Player build, hai lượt chọn node 3 và 7 trên cùng binary để xem cả Thanh Tẩy/Hỏa Tuyến ở bảng chi tiết. Tổng 264 ảnh; đã xem 10 ảnh, gồm hai skill trên ba viewport và Võ/Rương đồ/Tiềm năng/Linh thú PC.
- Bộ Võ/Linh/Cơ vẫn chưa đồng nhất mỹ thuật. Hỏa Tuyến còn một đường cong và thiếu dấu hiệu cơ giới rõ; các chi tiết nhỏ của Thanh Tẩy vẫn cần so với toàn bộ demo. Không dùng số lượng sprite hoặc test xanh để gọi hoàn thành.

## Bước tiếp theo cụ thể
1. Tiếp tục nhóm Võ còn sai hình tác động: đối chiếu Bộ Pháp/Đột Kích/Quyền Ý với các vùng Bước Thần/Lướt áp sát/Ý Chí Võ Đạo trong demo Võ. Khóa visual-only mapping trước, không gán alias gameplay; nguồn mới phải thể hiện chuyển động/quyền khí, không vật phẩm hoặc portrait.
2. Liên Kích/Chấn Kình vẫn còn việc. Không dùng các bản bàn tay đơn/giáp/huy hiệu bị loại hoặc lặp lại các lần model đã bị chặn để lấp chỗ.
3. Tiếp tục kiểm độ nhất quán của Hỏa Tuyến/Thanh Tẩy với các icon còn lại ở 64–128 px; sửa artwork khi có sai lệch cụ thể, không thêm vòng gamma hoặc phóng UI riêng.
4. Chỉ chuyển READY_REVIEW khi toàn bộ năm tab thực sự sát canonical và có audit Player đầy đủ. Kết thúc batch không phải nghiệm thu task.

## Evidence và nguồn
`build/character-hub-skill-paint-v45/`: `runtime-review.json`, `source-comparison-final.png` (nguồn, không phải Player), `final-sources/`, `artwork-registry-final.json`, `import-final-proof.json`, `full-editmode.xml`, `runtime-reviewed/{cleanse,fireline}/`, `player-reviewed/LinhGioiOnline.app`.
`lgo-skill-paint-v45-authoring.zip` cùng `.zip.sha256` và `package-proof.json`: nguồn tích lũy gồm cả Linh Thuẫn v44; raw được chọn, mask, recipe và registry portable. Không import ZIP/reference board vào Resources; không có font/model/Player trong gói.
`CONTINUE / PARTIAL_SKILL_PAINT_ALIGNMENT / VISUAL_FIX_REQUIRED` — không nghiệm thu toàn class, Character Hub hoặc thiết bị mobile/tablet thật.
