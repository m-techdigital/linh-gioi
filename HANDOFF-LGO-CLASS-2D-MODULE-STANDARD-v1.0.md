# Handoff chuẩn module class 2D v1.0 — Võ

## Override hiện hành — base-first hierarchy checkpoint 2026-09-10

Update shared-state: `TwoDCharacterRuntimeState` đã lấy ownership của mode/gender/level, 10-slot loadout, locomotion hold và timed action khỏi Map01A. Evidence `build/vo-shared-state-v2/three-profiles/` đạt 66×3 và `voSharedRuntimeStateVerified=True`. Class kế tiếp phải cấp config/assets cho base này, không sao chép state machine Võ.

Update Kiếm source: đã batch-extract 80 **source candidate** Lv1/10/20/30 nam/nữ từ hai grid redraw đã tuyển vào `classes-lv001-030/kiem/generated-batch-v1/source-v4/`; manifest/hash/contact sheet đầy đủ nhưng toàn bộ `runtimeEligible=false`. Review thô đã chuyển 12 candidate nữ có da/body/tay/chân bake sang `redraw-required`; 68 món còn lại vẫn chưa được fit base. Tool chung `extract_lgo_equipment_grid.py` nhận crop plan, bắt buộc 10 slot canonical và không hardcode class.

Compatibility contract mới ở `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md`: loadout là `slotId → itemId`, cho phép phối chéo level sau unlock; level không phải compatibility key. Runtime chỉ nhận item `approved` có cùng skeleton/body profile và đủ attachment bone. Không dùng scale/offset tùy ý để cứu crop sai; chuyển sang design/redraw trên template base.

Update side-view redraw: grid cũ được giữ cho inventory/reference; không dùng làm paper-doll vì góc front/3/4 lệch base side-view. Built-in ImageGen đã tạo hai sheet mới ở `classes-lv001-030/kiem/generated-batch-v2/`, đủ 80 cell canonical; nữ v1 bị reject do dính da/tay, nữ v2 sửa đúng hai row. `source-v3` đã key/despill/crop một lượt, nhưng cả 80 vẫn `candidate`, runtime eligible bằng 0 cho tới SpriteSkin/bone-weight + motion fit.

Fit spike chỉ chọn `Lv1 weapon + Lv30 hair + Lv10 inner + Lv20 outer`. Review cận cảnh phát hiện hair và outer trong source-v3 còn ghép nhiều view; đã redraw đúng bốn file hair/outer nam/nữ, reject bản outer nam 3/4, key magenta và normalize một lần tại PPU 208. Contact `generated-batch-v2/fit-spike-v1/mixed-loadout-prefit-contact-v1.jpg` đạt idle prefit cho bước weighting; vẫn runtime eligible bằng 0.

Proof pack Unity gộp 8 item vào một atlas 512×512, PNG 173.683 byte, không resize lại từng item. Sáu sprite Skinned có bones/BlendWeight tái tạo bằng editor authoring tool; hai weapon là Rigid. Player 169.088.250 byte, tăng 0,352% so baseline và nhẹ hơn bản 8 texture rời 202.672 byte. Pack vẫn giữ `DRAFT_RUNTIME_FIT` cho tới khi có motion evidence trong Player.

Runtime fit preview dùng API opt-in riêng: production equip tiếp tục từ chối `DraftRuntimeFit`. Bone-proxy adapter giữ bind pose cục bộ của từng item rồi nhận rotation từ skeleton chung, tránh lỗi áo/hair tụt xuống chân khi bind thẳng hai hệ tọa độ. Capture 78×3 đã cho thấy 4 món mixed-level nam/nữ bám base qua idle/walk/run/jump/basic/shared-skill pose; đây chỉ là fit proof 4/10 slot, chưa phải class Kiếm hoàn chỉnh hoặc skill Kiếm production.

Update v9: base `head` nam/nữ đã bỏ tóc bake; mọi hairstyle là attachment. Capture matrix 66×3 tại `build/vo-ten-slot-matrix-v2/three-profiles/` đã review đủ 10 slot cho nam Lv1 và nữ Lv30. Trạng thái `CLASS_VO_LV1_30_VERTICAL_SLICE_PASS`; next tách character/action orchestration dùng chung, chưa nhân controller Map01A sang class khác.

Quyết định dependency: chưa cài `2D Animation`/`PSD Importer`/`Addressables` trong checkpoint này. Manifest hiện giữ contract tương đương Sprite Library `Category + Label`; migration package phải là batch riêng có benchmark và giữ nguyên evidence. Asset residency mục tiêu là map hiện tại + class common + tier hiện tại/gần kề, dùng chung cho mobile/tablet/PC.

Update v8: source garment hiện ở `generated-batch-v6/source-v1`, đủ 8 sheet cùng layout 4×3. Runtime có 112 attachment; một slot được phép có nhiều component theo bone. Tổng texture 885.234 byte, Player ba profile đã review outfit Lv30 nữ. Gate còn lại là visual matrix cởi/mặc đủ 10 slot; không quay lại cách tách delta nhỏ từ full-body.

Phần “no gameplay/runtime asset” bên dưới là lịch sử của sandbox chuẩn hóa cũ. Branch hiện hành đã có Map01A và Võ runtime. Contract mới dùng chung nằm ở `TwoDSkeletalPaperDollRig.cs`; manifest Võ v7 có 20 rig part, parent/pivot, 120 local pose profile và 96 equipment attachment cho 4 tier × 2 giới × 10 slot. Mọi slot dùng attachment path; `arm_guard/boots` có left/right, tám slot còn lại có center attachment. Tất cả chỉ tham chiếu sub-rect của 7 atlas hiện có, không tăng texture byte.

Không nhân code Võ sang class khác. Trước Kiếm/Pháp/Cơ/Linh phải tái dùng cùng base rig runtime, bone/slot IDs và action/UI base. Asset khác class chỉ được thay manifest/atlas/pose timing. Evidence kỹ thuật mới nhất phải đi cùng visual note trung thực; hierarchy đã giảm rời khớp, nhưng rig-compatible garment art vẫn cần polish trước khi ghi `CLASS-VO-01_PASS`.

Ngày 2026-09-09. Owner thu hẹp phạm vi hiện tại về class Võ; bốn spec class còn lại trong prompt ban đầu được hoãn, không tạo file giả đủ danh sách. Quy ước chung giữ 5 class ID, nhưng checker chỉ yêu cầu spec/pack Võ.

## Quyết định và hiện trạng

Đã review trực tiếp 23 PNG trong `/Users/minhdc/Projects/2D/Vo`; chọn outfit nam/nữ `22_11_44 … (2)/(3)`, grid nam/nữ `22_13_40 … (3)/(2)`, module map `22_13_40 … (1)`, mood VFX `lgo-vo-ky-nang-chien-dau-vfx.png`. Tên đầy đủ, quyết định từng file và SHA-256 ở `docs/art/classes/vo/LGO-VO-2D-MODULE-SPEC-v1.0.md`.

Không có board đạt chuẩn item rời: còn ngón tay, áo/giáp và quần/đai gộp; bản mới nhất còn lệch xanh Lv100. Chọn reference không đồng nghĩa duyệt sạch mọi ô. Thiết kế sửa: quyền khí ở bàn tay, bảo hộ ở cẳng tay; tóc không đầu; áo/giáp/đai độc lập; Võ đen–đỏ–vàng–ngà, aura cam/vàng tách khỏi item.

Sau yêu cầu mở rộng của owner, đã thêm contract base/rig/animation cho Võ: `docs/art/classes/vo/LGO-VO-2D-BASE-RIG-ANIMATION-SPEC-v1.0.md`. Contract khóa một base nam và một base nữ dùng xuyên level, anchor ổn định, test phối chéo lv001/lv050, và motion set `idle`, `walk`, `run`, `jump_start`, `jump_air`, `fall`, `land`, `basic_attack`, `skill_windup`, `skill_cast`, `skill_recover`.

Đã thêm thiết kế rương/paper doll: `docs/design/LGO-2D-VO-CHEST-PAPERDOLL-DESIGN-v0.1.md`. Màn này dùng để xếp đồ Võ, inspect item, thử đồ không mutate loadout, apply/cancel, toggle nam/nữ và xem preview motion. Không mở persistence, loot, economy, trade hoặc server inventory.

Đã sinh draft preview để review hướng, giữ trong `build/class-2d-review/generated-drafts/`:

- `vo_male_lv001_sideview_module_demo_draft_v1.png`
- `vo_female_lv001_sideview_module_demo_draft_v1.png`
- `vo_chest_paperdoll_ui_mockup_draft_v1.png`
- `vo_base_rig_motion_sheet_draft_v1.png`
- `vo_male_female_motion_compatibility_draft_v1.png`
- `vo_male_cross_level_fit_draft_v1.png`
- `vo_female_cross_level_fit_draft_v1.png`
- `vo_male_base_transparent_candidate_v1.png`
- `vo_female_base_transparent_candidate_v1.png`
- `vo_male_base_transparent_candidate_v1_white_check.png`
- `vo_female_base_transparent_candidate_v1_white_check.png`
- `vo_male_lv001_10slot_detached_sheet_candidate_v1.png`
- `vo_female_lv001_10slot_detached_sheet_candidate_v1.png`
- `vo_male_lv001_10slot_detached_sheet_extraction_failed_alpha_v1.png`

Các ảnh này chỉ là visual direction. Bản nam đọc silhouette tốt nhưng assembled preview vẫn có tay trong găng; bản nữ hợp hướng agile/pony-tail nhưng còn da trong preview mặc đồ và vài module cần hollow sạch hơn. Mockup rương thể hiện đúng grid, applied/preview, paper doll, slot quanh người và motion strip, nhưng icon/slot trong ảnh chỉ là bố cục visual, chưa phải dữ liệu item thật. Base rig/motion sheet đầu hữu ích cho anchor và pose, nhưng nữ đang lệch sang quần dài; bản motion nam/nữ thứ hai đã sửa hướng female shorts và có đủ movement/jump/punch/skill rows, nhưng vẫn chưa phải sprite sheet đo pixel. Hai cross-level fit draft đọc được `lv001`, `lv050` và mixed outfit trên cùng side-view base, đủ để duyệt hướng tỷ lệ/anchor; chưa dùng để trích xuất runtime layer. Hai base candidate nam/nữ có alpha thật và đã xem trên nền trắng, đủ làm draft thử đồ trong build evidence; vẫn cần đo anchor/scale. Hai sheet 10 slot `lv001` giúp kiểm hình dáng item rời, nhưng checkerboard bị bake vào RGB; extraction thử ra nền tối, nên item sheet fail transparent gate. Không đưa các ảnh vào source/runtime và không coi là production.

Bộ bàn giao: 5 tài liệu chuẩn chung tại `docs/art/LGO-CLASS-*` và checklist, 1 spec Võ, 1 checker, handoff này và cập nhật NEXT-ACTION. Ảnh gốc giữ nguyên ngoài repo, không đưa lại source images vào branch. Các thay đổi Unity/AGENTS/reference đang có trước batch không thuộc checkpoint này.

## Validation và giới hạn

Chạy từ repo root:

```sh
pwd
git --no-pager status --short --untracked-files=all
git --no-pager diff --check
PYTHONPYCACHEPREFIX=build/pycache python3.12 -m py_compile tools/validate_class_2d_module_spec.py
python3.12 tools/validate_class_2d_module_spec.py
python3.12 build/class-2d-review/check_validator.py
python3.12 tools/validate_class_2d_module_spec.py --require-assets
python3.12 tools/validate_2d_branch_no_source_images.py
python3.12 tools/report_lgo_change_budget.py
```

`--require-assets` phải trả khác 0 khi chưa có ảnh; đây là evidence thiếu pack, không lỗi cần che. Regression harness local kiểm tài liệu thiếu, phrase thiếu, tên/folder sai, thiếu slot/board và header PNG giả trên fixture tạm; không chứng nhận art. Không chạy Unity cho batch docs/checker. Kết quả thực lưu `build/class-2d-review/validation.log`, không commit build artifacts.

Kết quả cũ đã chạy: diff check và py_compile đạt; checker đọc 7/7 tài liệu đạt; 15 regression cases đạt; gate no-source-images đạt. Gate --require-assets trả 1 vì thiếu 226/226 ảnh như dự kiến. Sau phần base/rương cần chạy lại checker vì số tài liệu gate đã tăng. Change budget báo WARN do tổng workspace có cả thay đổi có sẵn; checkpoint này không gom Unity. Owner xác nhận tab khác đang xử lý world map/hệ thống; ghi chú NEXT-ACTION dùng chung để ngoài commit, không reset/stash thay đổi tab khác.

## Gate kế tiếp

Thiếu pack clean transparent cho 10 item rời và rig anchor đo được. Kế tiếp nên sinh từng item riêng cho `lv001` nam/nữ thay vì sinh sheet, vì sheet hay bake checkerboard; sau đó làm `lv050`, full-equip preview, mixed-level preview, motion compatibility sheet và mockup rương/paper doll. Chỉ khi hai mốc khớp anchor mới mở rộng 11 level/220 item. Sau khi Võ đạt gate này, owner yêu cầu làm tiếp class Cơ và Linh theo folder tương tự `/Users/minhdc/Projects/2D/Vo`. Gate ingest riêng: `tools/validate_2d_branch_no_source_images.py` hiện cấm ảnh trong source, không tắt hoặc nới gate tự động. Chưa có runtime/production asset để import.

## Non-claims

- no gameplay implementation
- no production art claim
- no runtime asset claim
- no protocol changes
- no GameData schema changes

Không sửa server runtime, `protocol/**`, `gamedata/schemas/**`, `docs/adr/**` hoặc `client/Unity/Assets/Game/UI/design-tokens.json` trong batch. Commit chỉ tài liệu/checker thuộc batch, không push ngoài supervisor.
