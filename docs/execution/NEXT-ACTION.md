# NEXT ACTION — Character Hub / độ rõ và nội dung đúng demo

## Active — tiếp tục chỉnh mỹ thuật từ nguồn đã đối chiếu, giữ base chung
- Worktree `/Users/minhdc/Projects/LinhGioiOnline/.worktrees/character-hub-v22`, branch `codex/character-hub-v22`, upstream `feature/2d`. Không reset, restore hoặc đổi branch/worktree.
- SID `S-LGO-SKILL-20260916-C9B4`, task `T-69b4b27c4b39`. Check pause/recovery/job và claim trước batch; không tác động phiên khác. Cuối lượt dùng `v3.finish` và WAITING_USER/BLOCKED đúng kết quả.
- Canonical UI: `redesign-v4-five-tabs`. Demo class là căn cứ hình ảnh, không tự thay ID/tên/cấp/gameplay. Actor/pose/wardrobe/renderer và frozen surfaces không thuộc batch.

## v41 — nâng sáng vùng tối của nguồn, đóng gói gọn cả bộ
- Cùng mười vật phẩm Võ–nam–Lv1, không thêm item hoặc mở biến thể. Tái dựng từ nguồn RGBA384 đã sửa và pin hash ở v38/v39/v40, không từ pixel atlas đã nén hoặc ảnh Player.
- Một công thức chung chỉ nâng vùng có peak RGB dưới128, giữ các pixel sáng hơn và tỉ lệ kênh màu trong sai số làm tròn; alpha, hình và tọa độ nguồn giữ nguyên. Không tint/scale/offset trong UI, không thêm viền/glow để che nguồn.
- Đã loại bản gamma toàn dải vì làm đổi màu vùng kim loại sáng; PNG bản đó vẫn vượt250000byte sau thử nén lossless. Bản chọn bảo vệ highlight và phù hợp hơn với mục tiêu làm rõ vải/tóc, không phải đổi palette theo cảm tính.
- Mười nguồn hiện hành đóng một lượt trên base slot đã pin: atlas640×512 thay640×640, giảm20% sốpixel nhưng không đo bộ nhớGPU thật. PNG249142byte, tăng459byte so v40, vẫn dưới250000byte.
- Hai mươi module giữ alpha chính xác; mười module reference cũ giữ nguyên RGBA; mười binding và profile/công thức chiếu nội dung không đổi. Gỡ hàng trống không có nghĩa scale từng sprite.
- Packer có tùy chọn nén PNG lossless, mặc định giữ bytes cũ. Trên atlas v41 tùy chọn này tiết kiệm0byte; không lấy nó làm lời giải thích cho việc vừa ngân sách. RGBround4 vẫn là encoding cũ với sai số≤2/255, không gọi nguồn RGB lossless.
- Graphics EditMode313/313 không fail/skip; Python78/78. Một build0error/0warning, một capture132ảnh. Đã xem15ảnh Nhân vật/Rương đồ/tóc/áo trong/phụ kiện trên ba viewport; không thấy cắt/chồng hoặc notice lỗi mới.
- Tóc và tua đen vẫn nhỏ/tối ở tablet; hình mới/cũ chưa đồng nhất hoàn toàn với demo hoặc renderer. Đây là cải thiện độ rõ, không nghiệm thu toàn mỹ thuật hay thiết bị mobile/tablet thật.

## Bước tiếp theo
1. Không áp lại đường nâng sáng lên nguồn đã xử lý hoặc tiếp tục tăng gamma để coi cả bộ đạt. Giữ original-sources làm đầu vào; chỉnh nét/hình tác dụng hoặc ánh sáng vẽ chỉ khi có đối chiếu demo và lợi ích nhìn thấy rõ.
2. Tiếp tục Skill có mapping demo rõ, ưu tiên bản sắc Võ/Linh; Hỏa Tuyến còn thiếu và Thanh Tẩy chưa được thay. Giữ vòng ngoài/component chung, nội dung skill riêng; không dùng portrait/item icon làm phép hoặc tự sửa tên/cấp còn mâu thuẫn.
3. Đăng ký vật phẩm class/giới/cấp khác chỉ khi có nguồn đủ rõ và đúng item ownership. Không làm cho icon có vẻ khớp bằng sửa renderer hoặc mượn bộ Võ.
4. Bất kỳ batch nguồn mới phải kiểm demo → nguồn rời → Player; giữ ảnh lỗi và kiểm regression theo vùng bị ảnh hưởng, không chỉ đếm đủ icon.

## Evidence và bàn giao
`build/character-hub-item-readability-v41/`: `original-sources/`, `prepared-reviewed/`, `registry-reviewed.json`, `art-review-reviewed.json`, `source-lighting-reviewed.png`, `lighting-proof-reviewed.json`, `import-proof.json`, `runtime-review.json`, `full-editmode.xml`, `runtime-final/`, `player-final/LinhGioiOnline.app`.
`lgo-item-readability-v41-authoring.zip` +`.zip.sha256`: nguồn trước/sau, base slot và snapshot v40, registry portable, demo chỉ-tham-chiếu, hai script và README. Tái tạo nguồn RGBA và PNG portable đúng byte; không chứa font/model/binary, không unzip vào Resources.
`checkpoint-receipt.json` sau push xác minh commit/remote. `CONTINUE / SOURCE_READABILITY_REFINED / VISUAL_FIX_REQUIRED` — chưa nghiệm thu toàn Character Hub.
