# Chuẩn module class 2D v1.0

Cập nhật kiểm soát không gian 2026-09-11; scope triển khai hiện tại là Võ Lv1 trước, rồi Lv10. Nội dung Lv20–100 là chuẩn mở rộng, chưa phải nội dung đã hoàn thiện.

Ngày 2026-09-09. Scope đang áp dụng: **Võ (`vo`)**, male và female. Owner thu hẹp batch hiện tại về Võ; chưa tạo spec triển khai hoặc art cho Kiếm/Pháp/Cơ/Linh. Chuẩn tài liệu này không đổi equipment contract runtime.

Nguồn ưu tiên: yêu cầu owner → `docs/02-GDD.md` và `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md` → north-star lock → visual reference usage guide → ảnh. Hai base nam/nữ dùng chung giữa các class; tháo đồ vẫn có tóc cơ bản, đồ xám, shorts và socks. Không đổi tỷ lệ body theo level hoặc dùng board để thay skeleton.

## Định danh ổn định

Class registry dùng cho tên reference: `vo`, `kiem`, `phap`, `co`, `linh`. Đây không phải ID protocol/GameData. Chỉ `vo` được kiểm độ đầy đủ trong batch này; không yêu cầu asset của bốn class còn lại.

Gender: `male`, `female`.

Level: `lv001`, `lv010`, `lv020`, `lv030`, `lv040`, `lv050`, `lv060`, `lv070`, `lv080`, `lv090`, `lv100`.

Thứ tự slot cố định, không đồng nghĩa sorting layer:

1. `main_weapon` — Vũ khí chính / Class weapon
2. `head_hair` — Đầu / Tóc / Mũ / Băng trán
3. `inner_top` — Áo trong
4. `outer_top` — Áo ngoài / Chiến y / Robe / Jacket
5. `lower_body` — Quần / Váy / Hạ y
6. `waist_belt` — Đai lưng
7. `arm_guard` — Bảo hộ tay / Cẳng tay / Găng phụ trợ
8. `footwear` — Giày / Ủng
9. `shoulder_chest_guard` — Giáp vai / Ngực nhẹ
10. `class_accessory` — Phụ kiện / Linh ấn / Trang sức / Class emblem

## Quy tắc bắt buộc

Equipment phải **không dính da thịt**: không body, tay, chân, mặt, torso hoặc base character. Ngoại lệ về tóc duy nhất: `head_hair` được chứa tóc rời, không đầu người; các slot khác không chứa tóc. Đồ hở ngón có lỗ trong suốt, không vẽ da để lấp lỗ.

- Không gộp `main_weapon` với `arm_guard`.
- Không gộp `inner_top` với `outer_top`.
- Không gộp `lower_body` với `waist_belt`.
- Không gộp `outer_top` với `shoulder_chest_guard`.

Mỗi pixel chi tiết thuộc đúng một slot: vạt may vào áo thuộc áo, tua treo từ đai thuộc đai, charm tháo riêng thuộc accessory. Tóc front/back, đôi giày, trái/phải của quyền khí là các phần của một slot, không tạo slot thứ 11. Base và VFX không phải equipment slot.

## Folder và naming

`assets/reference/classes/{class_id}/{gender}/equipment/{slot_id}/{class_id}_{gender}_{slot_id}_{level}.png`

Ví dụ: `assets/reference/classes/vo/male/equipment/main_weapon/vo_male_main_weapon_lv050.png`.

Folder vật lý đi theo class/gender/equipment/slot; level nằm trong filename, **không thêm thư mục level** làm sai convention. Một PNG canonical là preview item rời ở một góc chuẩn, không phải gói đầy đủ cho rig/animation. Side/back, các mảnh cutout, anchors, masks và VFX cần task/pipeline riêng trước runtime; không tự thêm suffix vào convention này.

**Đường dẫn là quy ước đích, chưa tạo ảnh tại đó.** Gate hiện hành `tools/validate_2d_branch_no_source_images.py` cấm source image trong branch. Giữ ảnh upload ngoài repo; không copy board vào các folder trên để giả đủ bộ. Khi có task ingest riêng phải giải quyết gate này minh bạch, không tắt validator.

## Base, anchor và motion contract

Base nhân vật không đổi theo level. Mọi item Võ phải khớp cùng một skeleton nam và một skeleton nữ qua `lv001` đến `lv100`; level chỉ thay trang bị, không thay chiều cao, tỉ lệ, khớp, thế đứng hoặc camera side-view. Cross-level mixing là gate bắt buộc: item cấp thấp/cao có thể dùng chéo trên cùng base mà không scale body hoặc dịch anchor.

Anchor tối thiểu phải ổn định cho tóc, áo trong, áo ngoài, hạ y, đai, giáp vai/ngực, cẳng tay, quyền khí, giày và phụ kiện. Motion test tối thiểu gồm `idle`, `walk`, `run`, `jump_start`, `jump_air`, `fall`, `land`, `basic_attack`, `skill_windup`, `skill_cast`, `skill_recover`. VFX skill là layer riêng, không bake vào equipment.

Chi tiết đang áp dụng cho Võ ở `docs/art/classes/vo/LGO-VO-2D-BASE-RIG-ANIMATION-SPEC-v1.0.md`. Màn rương/paper doll để thử đồ nằm ở `docs/design/LGO-2D-VO-CHEST-PAPERDOLL-DESIGN-v0.1.md`.

## Board phải có cho Võ

Prefix: `assets/reference/classes/vo/boards/`.

- `vo_module_map_v1.png`: mannequin reference tách vùng với bảng item; đủ 10 nhãn, chỉ vị trí/quan hệ, không xem thứ tự slot là z-order.
- `vo_male_equipment_grid_v1.png`: 10 hàng slot × 11 cột level, item rời.
- `vo_female_equipment_grid_v1.png`: cùng cấu trúc nam, item rời.
- `vo_male_outfit_progression_v1.png`: 11 nhân vật cùng base, pose, camera, tỷ lệ.
- `vo_female_outfit_progression_v1.png`: cùng tiêu chí.
- `vo_skill_vfx_progression_v1.png`: nghiên cứu visual, không chốt skill/unlock/damage.

Đối với class khác thay prefix `vo` bằng class_id khi được giao. Image boards là reference/design source, **không tự động là runtime assets**. Equipment grid chỉ có detached item assets; mỗi hàng đúng một slot, mỗi cột đúng một level; không mannequin, chân dung hoặc body trong ô item. Full outfit progression chỉ để visual review, **không dùng để trích xuất item**, không crop/slice board vào Unity.

## Cổng nghiệm thu

1. Spec: chạy `python3.12 tools/validate_class_2d_module_spec.py`; kiểm tài liệu/ID/quy tắc, không đánh giá ảnh.
2. Pack reference Võ đầy đủ: chạy cùng lệnh với `--require-assets --asset-root /path/to/assets/reference/classes`. Phải đủ 220 item PNG và 6 board; thiếu ảnh trả mã khác 0, không tạo placeholder.
3. Visual separation: người review mở từng ảnh theo checklist, ghi nguồn và lỗi. Checker tên file không phát hiện da, slot gộp hoặc alpha giả.
4. Runtime: chỉ sau clean sprite, rig, import và screenshot review; chưa chạy trong batch spec.

Tài liệu chi tiết: equipment slots, progression rules, image prompt templates, asset separation checklist và `classes/vo/LGO-VO-2D-MODULE-SPEC-v1.0.md` cùng thư mục này.

Hợp đồng sâu cho fit state, phối chéo level, attachment bone, coverage/occlusion và atlas residency nằm ở `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md`. Mọi class sau phải dùng chung contract này; không tạo loadout theo nguyên bộ level hoặc tự cấp runtime status từ crop.


## Kiểm soát kích thước và tọa độ — bắt buộc từ 2026-09-11

### Sự cố và nguyên nhân đã xác minh

Owner phát hiện nhân vật đổi kích thước/vị trí và tách bộ phận trước–sau hành động. Test cũ đếm slot và so attachment trong avatar-local space nên loại root transform khỏi phép đo; kết quả xanh không chứng minh base ổn định. Code từng scale root lúc đứng thở/đi/chạy/skill và nghiêng root tại chân. Mode full đổi giữa static và motion source được normalize khác nhau. Rig/garment tách rời lại fit chiều cao từng mảnh, lấy center bone làm center item, có da/body trong garment. Đây là các lỗi pipeline, không phải đặc tính thiết kế.

Bằng chứng tại worktree `/private/tmp/lgo-vo-lv1-30-FaSFxE`: root RED 2/2 fail (`build/vo-base-scale-red.xml`) → GREEN 2/2 pass sau bỏ root squash/lean (`build/vo-base-scale-green.xml`). Pack hiện hành bị fit gate reject: silhouette IoU nam 0,537, nữ 0,382; diện tích tương ứng 1,808× và 2,351× reference (`build/vo-bind-fit-rejection-v1/report.json`). Không gọi các con số này là phần trăm chất lượng. Static base nam cao 1,6247 unit, nữ 1,5572; motion idle cùng bị đặt 1,70; rig cả hai 1,71. Sửa root hiện còn WIP trong worktree; checkpoint tài liệu/tooling không đồng nghĩa đã tích hợp root fix hoặc Player pass. Chưa được gọi base hiện tại là hoàn thiện.

### Một công thức không gian, một profile có phiên bản

Nguồn code duy nhất cho phép chiếu hiện tại: `project_canvas_rect()` trong `tools/pack_lgo_vo_lv1_map_avatar.py`; cả hai packer Võ dùng lại hàm này. Profile `lgo_character_canvas_1024x1536_v1` gồm canvas 1024×1536, gốc ngang 512, groundY 1484, hệ số `u = 1,70 / 1536` world unit/source pixel. **1,70 là độ cao canvas, không phải chiều cao mọi nhân vật hoặc mọi pose.** Base nam/nữ có thể khác chiều cao thiết kế; mỗi base giữ nguyên tỷ lệ qua level và hành động.

Với rect nguồn top-left `[l,t,r,b]`:

```text
worldW = (r-l) * u
worldH = (b-t) * u
dx = ((l+r)/2 - 512) * u
dy = (1484 - (t+b)/2) * u
worldPoint(x,y) = ((x-512)*u, (1484-y)*u)
atlasTop = atlasHeight - atlasY - atlasRectHeight
rigidAttachmentOffset = worldPoint(authoredAnchor) - boneBindPivot
```

Trim chỉ bỏ pixel rỗng, vẫn giữ `sourceCanvasRect`. Atlas X/Y, padding, độ phân giải PNG, nén texture không tham gia công thức world. Downsample đồng nhất theo budget; không fit từng item về một chiều cao riêng. Cùng điểm nguồn trên base và áo phải ra cùng world point và cùng trường trọng số bone. Pivot là khớp/điểm gắn đã đăng ký, không phải tâm ảnh hoặc tâm bone mặc định. Nếu artwork mới khác canvas/pose, phải đăng ký vào profile hoặc làm lại phần không tương thích trước đóng gói; không thêm offset trong controller để bù.

Mỗi base có danh tính/profile và reference/hash ổn định. Thay tỷ lệ skeleton là thay phiên bản base và phải kiểm lại các item phụ thuộc; tăng level, tháo đồ, đổi mode, đổi thiết bị không được đổi base. Metadata `sourceSpaceProfile` và `sourceCanvasRect` là bắt buộc cho output mới, nhưng **chỉ có metadata chưa đủ**: `registration_errors()` tính lại world bounds để bắt scale/offset tự sửa. Source chưa đăng ký bị reject, không tự gán profile để hợp thức hóa.

### Quy tắc runtime và source mới

- Root giữ scale chuẩn và rotation chuẩn; route/physics quyết định vị trí. Jump có độ cao hợp lệ; grounded action giữ mốc chân. Animation tác động bone, không squash/stretch cả root. Facing thuộc visual child; camera zoom không thay scale base.
- Một nhân vật dùng cùng base/skeleton cho idle và action. Bộ ảnh full-frame có registration khác không được tự thay vào lúc bắt đầu/dừng action. Bounding box nhỏ hơn khi cúi/nhảy là hợp lệ; không ép chiều cao mọi pose bằng nhau.
- Base, garment, rigid weapon, VFX là vai trò riêng. Gear không chứa da/bàn tay/chân; ảnh full chỉ là reference của outfit. Sheet chia ô/alpha đẹp không chứng minh item phù hợp.
- Kế thừa `TwoDSkeletalPaperDollRig`, SpriteSkin/Sprite Library và authoring đã có; mọi class/level dùng cùng cơ chế. Không sao chép controller/UI hoặc nhân các công thức theo item. Chỉ mở profile/class mới sau gate class hiện tại.
- Asset mới phải có source/hash, profile, rect nguồn, bone/anchor, sort/occlusion, semantics slot và budget. Kiểm trọn bộ + từng món tháo + phối cấp đã được đăng ký trước runtime promotion. Candidate không fit được giữ riêng, không xóa WIP.

### Gate và phạm vi bằng chứng

| Gate | Kiểm gì | Đã có hay còn thiếu |
|---|---|---|
| Source registration | World bounds tính lại từ source rect/profile; chặn thiếu profile hoặc tự đổi scale/offset | Đã có hàm chung và test; record rig/garment cũ chưa migrate |
| Bind outfit fit | So full outfit cùng pose/tọa độ, không normalize riêng bbox; IoU≥0,90, area ratio 0,90–1,10, RGB MAE≤22 | Đã có trong repacker; pack hiện tại bị reject |
| Root/action regression | Scale/rotation root, route/ground và return-to-bind qua action nam/nữ | Đã có test focused; không chứng minh limb fit |
| Animation/gear | Bộ phận khớp trước/trong/sau action; trang bị tháo không xuất hiện lại; 10 slot và phối tier hợp lệ | Phải hoàn thiện/review Player; chưa PASS |
| UI/UX đa màn hình | Cùng tỷ lệ world, safe-area, layout, tương tác, không che chủ thể/hộp thoại | Bắt buộc capture/review; chưa có gate tự động đầy đủ |

Ngưỡng fit là tiêu chí reject kỹ thuật ban đầu cho **cùng outfit ở bind pose**, không phải thuật toán phê duyệt mỹ thuật và không dùng so hai outfit hoặc hai pose khác nhau. Không hạ ngưỡng để hợp thức hóa pack hỏng. Mỗi lần thay tiêu chí phải có nguyên nhân và đối chứng; visual review vẫn bắt buộc. Gate repack hiện chưa chặn mọi đường build/deploy: build chẩn đoán pack cũ vẫn có thể chạy, nhưng không được đánh dấu runtime-approved/checkpoint visual PASS khi gate này đang fail.

Lệnh nhẹ trước Unity (Python có Pillow; môi trường hiện tại là `python3`):

```sh
PYTHONPYCACHEPREFIX=build/pycache python3 -m unittest discover -s tools -p test_review_lgo_paper_doll_pack.py
PYTHONPYCACHEPREFIX=build/pycache python3 tools/review_lgo_paper_doll_pack.py --pack-dir <runtime-pack> --out-dir <new-evidence-dir> --require-fit
```

Lệnh thứ hai trả exit 2 khi fit/registration fail, giữ báo cáo/ảnh để sửa; `pack_lgo_vo_lv1_30_avatar.py` cũng từ chối báo thành công nếu fail. Thư mục evidence mới bắt buộc để không đè kết quả cũ.

### Kiểm UI/UX và cách kết thúc batch

Ba profile macOS hiện có: mobile 1600×720, tablet 1024×768, PC 1280×720. Cùng camera orthographic có `pixelsPerWorldUnit = viewportHeight / (2 * orthographicSize)`; khác tỷ lệ màn hình chủ yếu đổi vùng nhìn ngang, không sửa sprite scale để bù. Nếu thiết kế yêu cầu camera khác phải có profile rõ và review cả ba, không offset tùy màn. UI dùng base layout/safe-area chung; kiểm text, anchor, overflow, hộp thoại, inventory, slot toggle và nút thao tác. Profile capture không thay chứng nhận thiết bị mobile thật.

Gom thay đổi phụ thuộc, kiểm nguồn/registration/full outfit trước, rồi một lượt EditMode/build/capture. Capture phải có idle trước action → windup/impact/recover → idle sau action, cận cảnh khớp và toàn màn hình UI. Phải xem ảnh; kiểm manifest/state/count xanh chỉ là technical evidence. Chạy lại gate tốn thời gian khi có thay đổi liên quan hoặc lỗi mới được xác minh, ghi lý do. Kết luận tách riêng logic, registration, visual và device; fail một gate cần thiết thì giữ `FIX_REQUIRED` và chưa mở class/tier tiếp theo. Lưu lỗi, nguồn sai, công thức sửa và evidence vào tài liệu này/NEXT-ACTION để phiên sau không nghiên cứu lại từ đầu.

### Gate registered đã có evidence — 2026-09-11

Pack legacy bị reject ở phần lịch sử không phải pack registered hiện hành. `BoundOutfitVerticesMatchCanonicalTextureCoordinates` trong EditMode equipment tests kiểm geometry sau bind và xuất dump khi có `LGO_REGISTERED_BIND_DUMP=/absolute/new-directory`. Dump khóa input code/rig/packages và atlas/manifest; thay input phải capture dump mới.

```sh
PYTHONPYCACHEPREFIX=build/pycache build/rig-authoring-venv/bin/python tools/review_lgo_paper_doll_pack.py --registered-bind-dump <dump>/male-bind.json --out-dir <new-evidence>/male --require-fit
PYTHONPYCACHEPREFIX=build/pycache build/rig-authoring-venv/bin/python tools/review_lgo_paper_doll_pack.py --registered-bind-dump <dump>/female-bind.json --out-dir <new-evidence>/female --require-fit
```

Reference là source canonical theo rect sở hữu; candidate dùng vertices/UV/indices Unity thực và atlas, trên cùng canvas256×384, không normalize bbox. Gate giữ ngưỡng cũ; sorting/layer selection kế thừa runtime nên cần Player visual review độc lập. Không gọi software raster này là screenshot Player. Final `build/vo-registered-bind-fit-final` đạt nam IoU1,00000/area1,00000/MAE0,016 và nữ0,99898/0,99898/0,129; đã xem ảnh đối chứng. Kết quả này chỉ kiểm bind ở trạng thái đứng và đã stale sau khi sửa runtime. Owner đã bác motion retarget v5/v7; không đủ đóng fit Lv1 hoặc tiếp Lv10/mặc chéo. Chuẩn motion là whole-pose v3 div4 từ sandbox cũ; registered wardrobe vẫn là candidate cần khớp theo chuẩn đó.
# Runtime wardrobe invariant cho nhiều class và nhiều tier — 2026-09-12

- Mỗi giới dùng đúng một body/action authority. Mọi item của mọi class/tier phải dùng chung canvas, pivot, pose IDs và fit family tương ứng; runtime không tạo actor, camera, scale hoặc animation clock riêng theo class/level.
- Resolver duy nhất chọn `slotId -> itemId -> components`. Một item có thể gồm nhiều attachment như tay áo trước/sau, nhưng việc đổi class/tier/loadout phải resolve trọn bộ rồi swap trên cùng actor. Candidate lỗi không được hiển thị song song với candidate cũ.
- Source full-outfit chỉ được dùng làm donor. Trước khi đóng atlas phải phân vùng theo canonical slot anchor của từng pose, giữ full-compose invariant, và review hai chiều: full set cùng từng trạng thái tháo. Một slot không được sở hữu anatomy hoặc phần lớn trang phục của slot khác.
- Gate bắt buộc gồm đủ sáu pose, 10 slot, full Lv1/full tier đích, phối chéo level, 16 tổ hợp bốn garment core, actor bounds đúng presentation, root scale 1 và chuỗi chạy riêng `contact A → run A → contact B → run B`. Count/test xanh không thay cho review ảnh Player.
- Khi số tier tăng, catalog/manifest chứa class, gender, level, fit family, body hashes và component order; atlas được load/release theo loadout. Không hardcode thêm controller hay renderer tree cho từng class.
