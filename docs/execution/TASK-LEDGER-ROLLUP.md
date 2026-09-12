## Map01A — mốc mặt terrain và thoại tablet — 2026-09-12

Dùng mốc source-pixel chung cho bốn loại terrain để mặt đi thực sự khớp GroundY trên cả 12 instance; giữ nguyên PNG và actor/camera/scale. Nút hành trang/combat ẩn trong thoại, hiện lại khi tiếp tục. RED 2 lỗi tái hiện; GREEN 45 test. Player build 0 error/7 warning, Q01–Q09 đủ 18 frame × ba profile và ảnh đã xem tại `build/map01a-grounded-player/quest-capture/`. Next là phục hồi nguồn landmarks để sửa object bị cắt ngang ô; chưa nghiệm thu toàn map hoặc character.

## Ưu tiên hiện hành — ổn định code, chuyển Map01A — 2026-09-12

Owner dừng mở rộng character để hoàn thiện map. Đã giữ source/base/div4, sửa class catalog thiếu giới để không bật renderer cũ, khôi phục đổi đủ năm class, sửa hành trang dùng bình và chống tràn. Python 37 test, Unity 26 test; Player Q01–Q09 18 ảnh × ba tỷ lệ, ảnh UI đã review. Chi tiết `build/map01a-stable-player/quest-ui-verified`, cuối PC `quest-ui-labels-pc`. Chưa nghiệm thu toàn bộ art/Map01A. Next: audit mặt terrain alpha so với GroundY và thoại tablet; không tự tiếp task character trong lịch sử dưới đây.

## Map01A functional UI + playable gate — 2026-09-10

Đã hoàn thiện phần còn thiếu sau Q01–Q09: minimap mở từ Q02 và bám node route; Q04 mở panel hành trang, nhận 3 Bình Máu/2 Bình Linh Lực và buộc dùng một bình để HP 60→100; Q07 nhận Hộ Uyển Võ Tân Thủ và buộc equip trước Q09. UI Toolkit dùng chung ba tỷ lệ, hàng action tự wrap để không tràn panel. EditMode cuối `179/178/0/1`; macOS Player build 0 compiler error; capture `build/map01a-functional-ui/three-profiles-v2/` có 27 frame/profile technical pass và các frame inventory/supplies/potion/loot/equip/portal đã review.

Map gate: `MAP01A_PLAYABLE_VISUAL_SLICE_PASS`. Next là một batch Võ Lv1–30 articulated rig/attachment + idle/walk/run/jump/basic attack/`Liên Quyền`; chưa mở class khác. Non-claim giữ nguyên: chưa persistence/backend, physical-device certification hoặc Map01B scene.

## Map01A Q01–Q09 playable flow — 2026-09-10

Đã nối một state machine local đủ 9 quest vào Cổng Đông Lâm, gồm dialogue Hạ Vân/Quan Thủ/Tổng Phú/Thanh Nhi/Lão Trần, inventory inspect, consumable reward, gather, chest tùy chọn, combat ba hit, loot và portal unlock. Herb/chest/enemy/portal có phản hồi world thay vì chỉ đổi text. EditMode gần nhất `179 total / 178 pass / 0 fail / 1 ignored`; build macOS `166.984.083` byte, 0 error/13 warning. Evidence v4 có 24 frame cho mỗi profile mobile/tablet/PC; cả ba manifest hoàn tất 9/9 và ảnh trọng yếu đã review.

Next: một batch functional UI cho inventory/potion/class-item equip/minimap unlock và capture chứng minh thao tác. Chưa chuyển Võ rig cho tới khi gate Map01A này xong; chưa claim physical-device certification, persistence hoặc Map01B transition.

## Võ modular motion alignment — 2026-09-10

Sửa lỗi correctness làm trang bị đã tháo có thể xuất hiện lại khi movement dùng full-frame. `base/modular` và tier cao giữ đúng layer đang mặc trong root pose; `Lv1 + full` mới dùng motion frame. RED capture bắt `voFemaleMotionVerified=false` do gate cũ; GREEN sau khi gate kiểm đúng `walk + aligned paper-doll + base/9 slot + inner_top off`. EditMode `179/178/0/1`, build macOS 0 error, 54 ảnh mobile/tablet/PC technical pass và đã review. Next: batch rig/attachment + đủ locomotion/combat Võ Lv1–30; chưa mở class thứ hai.

# TASK LEDGER ROLLUP — 2D Pivot

## Võ skill hit proof — 2026-09-10

Nâng skill từ cue sang hit runtime: range 2,4, active 0,42 giây, damage 35 tại key timing, HP/counter/tint/HUD feedback và chặn cast lại trong active window. EditMode `179/178/0/1`; build macOS pass; frame 12 của ba profile đã review. Đây là một-skill proof, chưa phải combat loop đầy đủ. Next vẫn là tier-matched modular motion cho Võ Lv10/20/30.

## Võ Lv1/10/20/30 visual progression — 2026-09-10

Đã tạo một sheet tiến cấp đồng nhất cho sáu nhân vật nam/nữ ở Lv10/20/30, batch-key/căn về base và tách 10 slot/tier. Runtime có 96 part, bốn atlas tier và hai atlas motion Lv1 hai giới, tổng 643.722 byte. Selector L/touch và 54 ảnh Player ba profile chứng minh đổi cấp/giới/slot và motion nữ. Chưa đóng class: Lv10/20/30 giữ paper-doll tĩnh khi chuyển động để tránh hiển thị sai outfit Lv1; next là tier-matched motion/rig và `Liên Quyền` có hit feedback.

## Võ Lv1 equipment + real motion — 2026-09-10

Batch kế thừa WIP đã hoàn thiện checkpoint Lv1 cho cả nam/nữ: 10 lớp paper-doll, đổi giới tính, chọn và bật/tắt từng slot trong Player. Võ nam có sheet sáu pose thật cho idle/walk/dash/punch; hai atlas indexed 1024² tổng 215.938 byte PNG. 42 ảnh mobile/tablet/PC đã review; EditMode `178/177/0/1`, build macOS 0 error, smoke/matrix/guards pass. Hạn chế còn công khai: nữ chưa có motion frame, mới một bộ Lv1, chưa có progression Lv10/20/30 và combat class đầy đủ. Next là một batch sheet tiến cấp hai giới + motion nữ, không mở class khác.

## Võ Lv1–30 map avatar workflow proof — 2026-09-10

Kế thừa trực tiếp Võ WIP từ sandbox đã dừng: base/full chung bbox `(275,16)–(801,1484)` và ba slot alpha `inner_top`, `arm_guard`, `main_weapon`. Packer mới tạo atlas 512²/23.370 byte, manifest giữ SHA nguồn, canvas, offset, ground và non-claim. Map 01A thay PC cyan bằng art Võ; C/touch đổi full/base/modular, movement có walk transform, X/touch chạy slash VFX `Liệt Phong Kích` tại combat edge.

Evidence cuối: EditMode 177/176/0/1, macOS Player 160.896.915 byte với 0 error, 36 ảnh Map 01A mobile/tablet/PC đã review, baseline smoke/build/capture/matrix và hai branch guard pass. Chưa claim đủ class: hiện 3/10 slot, chỉ có bản nam, motion transform dùng làm proof; next là một batch bảy slot + nữ + frame/skeletal animation thật trước khi mở Kiếm/Pháp/Cơ/Linh.

## Source selection + stopped class task recovery — 2026-09-10

MAP01A-01: thêm Resource contract Cổng Đông Lâm với route 10 khu, L0–L11, 6 NPC, 4 quái combat-edge-only, Q01–Q09, collision và UI safe area; controller/capture/matrix expose contract từ Player. Visual review 01/02/03/20 xác nhận runtime cũ chưa giống source: nền tối, rectangle và label cũ còn nhiều. Không claim visual pass; next MAP01A-02 thay màn spawn–đại cổng bằng art source-grounded.

Audit sâu sandbox class hoàn tất: nhánh local có 30 commit riêng và lệch origin 54 commit; bundle đầy đủ + patch uncommitted đã lưu. Năm commit cuối được phân loại để port chọn lọc; ground alignment giữ, chroma-key chỉ pre-process, procedural preview chỉ fixture QA. Unit test 28/28 PASS, validator tài liệu 13/13 PASS nhưng không được dùng để claim art/rig/runtime.

Source pack được mở rộng từ 61 lên 87 entry bằng 26 ảnh chi tiết đã review: module map, grid nam/nữ, outfit/silhouette và VFX mood của năm class. Tất cả mang `REDRAW_SOURCE_ONLY`; board Cơ trùng byte trong thư mục Linh bị loại. Audit: `docs/art/LGO-CLASS-STANDARDIZATION-REUSE-AUDIT-v1.md`.

Đã review bộ Cổng Đông Lâm và sheet hệ thống/identity/base/module Võ–Kiếm–Pháp–Cơ–Linh; tạo source pack chuẩn hóa ngoài repo tại `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1`. Pack có 10 map source canonical theo đúng 10 chức năng triển khai, 37 class source canonical và 12 file Võ WIP được thu hồi từ batch đã dừng. Manifest giữ đường dẫn gốc, SHA-256, purpose và status; ảnh gốc không đổi tên hoặc sửa.

Audit Võ WIP: base nam/nữ đã căn chung ground; mask plan và ba mảnh alpha `inner_top`, `arm_guard`, `main_weapon` có trim/atlas round-trip lossless và toggle review. Chưa được runtime approval vì lỗi tiếp xúc cổ tay/alpha edge, thiếu bảy slot, thiếu motion và female equipment. Phần này được giữ để tiếp tục sau gate Map 01A, không làm lại và không dùng primitive cũ làm art direction.

Next: MAP01A-01 khóa route 10 khu, 12 layer, collision/trigger/UI safe-area theo catalog; sau đó dựng visible art slice và Player evidence trước class Võ Lv1–30.

## Đông Môn illustrated draft — 2026-09-10

Đã tạo art mới và pack skyline + atlas cổng/NPC/terrain; preview opt-in trong Player qua `--lgo-dongmon-art-preview`, không sửa controller/state/5 class hoặc frozen surfaces. `tools/capture_lgo_dongmon_art.py` capture 5 trạng thái vào thư mục riêng. EditMode 139 pass, 0 fail, 1 skipped; guard 5 test pass; smoke/build/baseline 20 frame và art 5 frame đã chạy, ảnh đã review. Art vẫn DRAFT, player còn placeholder; không claim giống hoàn toàn ảnh owner.

Next/gate: owner xem capture `build/dongmon-art/player-final/03-dialogue.png` (bản copy bền ở `build/dongmon-art-checkpoint/`) và duyệt bố cục/palette/tỷ lệ trước khi nhân rộng. Sau duyệt mới mở thêm foreground/prop và ghép asset 5 class đã được tab riêng chuẩn hóa. Hướng/plan/evidence: `docs/design/dong-mon-illustrated/DESIGN.md`. Không tiếp polish primitive; không ghi đè checkout chính hoặc source tab 5 class. Không có blocker runtime; gate còn lại là duyệt mỹ thuật.


## Gate Keeper silhouette — 2026-09-10

Đã kiểm Người Giữ Cổng 13 part đọc từ JSON với `rect/ellipse/diamond/tapered`; shape lạ fallback rectangle, cache dùng lại texture, snapshot đọc style từ source. Capture tiếp cận NPC ở khoảng cách 0.85 trong FocusRange 1.15 để không chồng silhouette. Không đổi gameplay 5 class hoặc frozen surfaces.

Evidence trong `build/gatekeeper-polish-checkpoint/`: EditMode 138 pass, 0 fail, 1 skipped (pointer capture cần UI panel); onboarding smoke Complete; macOS build Succeeded (0 errors, 13 warning API UI cũ); Player capture 20 BMP; smoke/visual matrix, no-3D/no-source-image và frozen diff audit pass. Đã xem ảnh 01/02/03/20: mũ, áo thuôn, bóng ellipse, gậy/ngọc đọc được; player đứng riêng khi thoại. Đây chỉ là checkpoint silhouette blockout, chưa đạt chuẩn illustrated owner gửi.

macOS: sau launch từ Contents/MacOS, Player có thể chờ foreground do runInBackground=0; dùng `open <đúng app vừa build>` để kích hoạt app đang chạy, không mở ảnh giữa capture. Worktree cũ được giữ vì có thay đổi đồng thời ngoài lượt này; chỉ dọn worktree riêng của batch. Tab 5 class giữ ownership source của họ.

Next: design/demo draft góc Đông Môn bằng art mới theo phần hiệu chỉnh owner trong workflow research; capture Player và duyệt mỹ thuật trước khi nhân rộng. Không tiếp tục polish primitive vô hạn.


## 2026-09-09 — Branch 2D cleanup

Owner yêu cầu dừng hướng dựng nhân vật/cảnh cũ và chuyển sang game 2D trên `feature/2d`. Batch cleanup đã dọn source, asset source, tooling thử nghiệm, generated staging/evidence và cache nặng liên quan hướng cũ; thêm validator `tools/validate_2d_branch_no_3d.py` để ngăn kéo lại pipeline đó. Commit/push: `d97a3c8 Remove 3D asset pipeline from 2D branch`.

## 2026-09-09 — Runtime 2D onboarding slice

Đang đóng gói slice SCN-001/002 đầu tiên: Cổng Linh Thành, Người Giữ Cổng, thoại nhập môn, đường dẫn tới Bia Luyện Khí, kích hoạt bia và trạng thái hoàn tất. Evidence đã chạy qua Editor smoke, Player smoke và visual capture 5 frame trong `build/2d-onboarding-visual/`.

## Next

Commit/push runtime 2D slice, sau đó nâng art/UI 2D cho cùng flow thay vì mở rộng hệ thống mới.


## 2026-09-09 — HUD capture cho 2D onboarding

Chuyển HUD nhập môn từ IMGUI tạm sang text/sprite world-space để Player visual capture thấy được title, khu vực, mục tiêu, gợi ý, action, feedback và thoại NPC. Visual manifest bổ sung `hudLineCount`/`hudSnapshot`; ảnh review `03-dialogue.png` và `05-complete.png` đã được xem bằng mắt.

## Next after HUD

Nâng art 2D cho cùng flow SCN-001/002 và giữ capture runtime thật trước checkpoint.

## 2026-09-09 — Xoá ảnh source cũ cho branch 2D

Owner yêu cầu loại bỏ toàn bộ ảnh thiết kế cũ vì không dùng được nữa. Batch hiện tại xoá ảnh source/reference/runtime placeholder cũ khỏi source tree, giữ evidence trong `build/` ngoài kiểm soát source, thêm `tools/validate_2d_branch_no_source_images.py`, và xác minh runtime 2D onboarding vẫn chạy bằng procedural sprites + HUD world-space.

## Next after source image cleanup

Commit/push checkpoint, sau đó tạo lại hướng art 2D mới cho SCN-001/002 bằng design mới đã duyệt thay vì tái dùng ảnh cũ.

## 2026-09-09 — Đông Môn procedural blockout detail pass

Nâng runtime 2D onboarding theo hướng map Đông Môn tutorial: thêm biển Cổng Linh Thành, viền/ấn ngọc, lồng đèn, lối ngọc, đường kẻ sân, label Bia Luyện Khí, đốm linh khí và silhouette nhân vật nhiều lớp hơn. Thêm scene-beat snapshot/count vào controller và visual manifest để gate không chỉ dựa vào cảm giác. Evidence mới: Unity EditMode, Editor smoke, macOS Player build và Player visual capture 5 frame.

## Next after Đông Môn blockout

Triển khai design map A-Z từ board mới owner gửi: world map tổng thể → Linh Thành hub → Đông Môn tutorial → layer/tileset/ký hiệu/flow, bắt đầu bằng spec text và runtime blockout không dùng ảnh cũ.

## 2026-09-09 — 2D direction lock + map catalog realignment

Owner cung cấp kịch bản game mới trước khi design. Đã khóa lại hướng: 2D Side-Scrolling Social Action MMORPG, HD anime/stylized, không pixel-art, không quay lại Meshy/3D, giữ backend/world/class/progression/story. Thứ tự ưu tiên mới: 2D-00 Direction Lock → 2D-01 Male/Female Base Character → 2D-02 Character Modular Runtime → class Lv1 → animation → Linh Thành/Đông Môn map → Shadow Slime combat → vertical slice. Runtime map catalog bắt đầu phản ánh World/Linh Thành/Đông Môn tutorial mới và visual manifest có map snapshot để tránh làm mò.

## Next after direction lock

Sau khi checkpoint này pass/push, bắt đầu 2D-01 Male/Female Base Character; chỉ triển khai map production lớn sau khi character base/modular runtime đủ spine.

## 2026-09-09 — 2D-01 Male/Female Base Character

Đã bắt đầu spine base character theo kịch bản mới: `male_base` và `female_base`, default hair/innerwear xám, layer order từ Shadow đến UIAnchor, anchor tối thiểu cho body/equipment/hair/weapon/pet. Runtime controller expose `runtimeCharacterBaseSnapshot` vào visual manifest để các batch sau không tự chế slot/layer ngoài form chung. Đây vẫn là procedural/blockout base, chưa phải art final.

## Next after 2D-01

Sau khi commit/push checkpoint này, chuyển sang 2D-02 Character Modular Runtime: registry item/layer, equip/unequip, inventory preview flow đã được duyệt, và slot/id riêng cho tóc, mắt, trang bị, vũ khí, pet/spirit.

## 2026-09-09 — 2D-02 Character Modular Runtime

Đã thêm spine module/equipment cho hướng 2D: catalog module starter, slot riêng cho tóc trước/sau, mắt, áo, quần, giày, vũ khí và pet/spirit; loadout hỗ trợ flow hành trang đã duyệt `select_icon -> inspect_item -> try_on -> cancel_or_apply`. Runtime manifest có `runtimeEquipmentSnapshot` để kiểm slot/id/fit profile trước khi tạo đồ Võ Lv1 thật.

## Next after 2D-02

2D-03 Võ Lv1 outfit/module cơ bản cho nam/nữ, giữ form đơn giản theo level đầu và khai báo qua slot/fit profile chung thay vì hardcode từng món.

## 2026-09-09 — 2D-03 Võ Lv1 starter outfit seed

Đã thêm seed outfit Võ Lv1 qua module runtime: top riêng nam/nữ, quần, đai, găng và boots dùng fit profile chung. Khi hoàn tất Bia Luyện Khí, runtime apply loadout Võ Lv1 để capture thấy đổi trang bị trên player; manifest xác nhận `top=top_vo_lv1_male`, `waist=waist_vo_lv1_unisex`, `gloves=gloves_vo_lv1_unisex`, `boots=boots_vo_lv1_unisex`, `class=Vo`, `level=1`.

## Next after 2D-03

Tiếp theo nên làm 2D-05 Animation Locomotion để slice nhập môn bớt tĩnh, hoặc 2D-04 Kiếm Lv1 nếu cần mở thêm class module trước.

## 2026-09-09 — 2D-05 Animation Locomotion spine

Đã thêm animation profile side-scroll cho character 2D: Idle, Walk, Run, Jump, Dash, ClassSkill và TrainingCompletePose. Runtime controller expose `runtimeAnimationSnapshot`; procedural pose hiện đổi nhẹ tay/chân/đầu khi di chuyển và phóng nhẹ ở completion pose sau Bia Luyện Khí. Player visual capture kiểm được frame `02-gate-focus.png` và `05-complete.png`.

## Next after 2D-05

Ưu tiên mở tutorial Jump/Dash/Skill để các animation state có trigger gameplay thật, hoặc 2D-04 Kiếm Lv1 nếu muốn mở thêm class catalog song song.

## 2026-09-09 — Tutorial Jump/Dash/ClassSkill flow

Đã mở rộng Đông Môn tutorial: sau Bia Luyện Khí, người chơi phải qua LearnJump, LearnDash và LearnClassSkill trước khi Complete. Runtime input hỗ trợ Space/J cho Jump, Shift/K cho Dash, Q/L cho skill; HUD đổi mục tiêu/hint/action theo từng bước. Visual capture tăng lên 8 frame và manifest xác nhận `screenshotCount=8`, `finalStep=Complete`, animation tokens Jump/Dash/ClassSkill/TrainingCompletePose.

## Next after movement/skill tutorial

Nối `ClassSkill` vào Shadow Slime combat micro-slice để có mục tiêu/quái thật, hoặc mở 2D-04 Kiếm Lv1 nếu cần thêm class module.

## 2026-09-09 — Khóa kịch bản 2D mới vào GDD

Owner cung cấp kịch bản game mới trước khi design map. Đã cập nhật `docs/02-GDD.md` với scenario lock 2D: Social Action MMORPG side-scrolling, HD anime/stylized, Zone Network, opening cinematic, tutorial Đông Môn 10 bước, Chapter 1-3, item/character/map pipeline 2D và roadmap 2D-00 → 2D-12. Cập nhật direction lock/execution docs để các batch sau không quay lại Meshy/3D, không crop/dán board, và không làm map production lệch khỏi GDD.

## Next after scenario lock

Tiếp tục Shadow Slime combat micro-slice đang có test đỏ, sau đó chọn 2D-04 Kiếm Lv1 hoặc map production theo roadmap; mọi design/map mới phải kiểm `docs/02-GDD.md` trước khi implement.
## 2026-09-09 — Shadow Slime combat runtime checkpoint

Nối `ClassSkill` trong tutorial Đông Môn vào mục tiêu thật: Shadow Slime xuất hiện sau Dash, HUD yêu cầu dùng kỹ năng Võ Lv1, skill đánh tan slime và hoàn tất nhập môn. Thêm `ShadowSlimeVisible`/`ShadowSlimeDefeated`, `runtimeCombatSnapshot`, smoke assertion và visual manifest field để kiểm trạng thái bằng test + runtime evidence. Scope vẫn nhỏ: chưa mở HP, loot, reward/economy hoặc combat server-authoritative.

## Next after Shadow Slime runtime checkpoint

Đóng checkpoint sau validation; tiếp theo chọn 2D-04 Kiếm Lv1 hoặc map production Linh Thành/Đông Môn theo GDD mới.
## 2026-09-09 — Map layer budget Chapter 1 runtime spine

Bám kịch bản/GDD mới trước khi design map production: `TwoDMapDesignCatalog` thêm `LayerBudgetSnapshot` cho Chapter 1 `Vết Nứt Đông Môn` với Sky/Fog, Far Background, Mid Background, Near Background, Gameplay Plane và Foreground. Runtime minimap overlay thêm nhãn Chapter/Layer; visual manifest `runtimeMapSnapshot` chứa layer budget để future session không làm map mò hoặc làm phẳng.

## Next after map layer budget

Đóng checkpoint sau validation; tiếp theo chọn 2D-04 Kiếm Lv1 hoặc nâng map production Linh Thành/Đông Môn bằng landmark/route/parallax rõ hơn.
## 2026-09-09 — Đông Môn route progress runtime spine

Thêm route progress cho tutorial/Chapter 1: state expose `CurrentRouteNodeId` từ `spawn` tới `return-gate`, controller expose `RuntimeRouteProgressSnapshot`, visual manifest ghi route progress và minimap hiển thị dòng `Node:` hiện tại. Đây là bước map-runtime nhỏ nhưng player-visible, giúp các batch map sau biết người chơi đang ở node nào thay vì chỉ có route tĩnh.

## Next after route progress

Đóng checkpoint sau validation; tiếp theo chọn 2D-04 Kiếm Lv1 hoặc nâng map production Linh Thành/Đông Môn bằng landmark/parallax/route nodes rõ hơn.

## 2026-09-09 — 2D-04 Kiếm Lv1 starter modules

Bổ sung class module thứ hai sau Võ Lv1: áo Kiếm nam/nữ dùng fit profile torso riêng, quần/đai/găng/boots dùng fit profile chung, vũ khí `weapon_kiem_lv1_starter` có anchor `WeaponAnchor` và VFX seed `sword_trail_seed`. Loadout kiểm được mix áo/kiếm Kiếm với quần Võ để bảo vệ hướng auto-fit/module chung thay vì fit thủ công từng tổ hợp. Runtime Đông Môn thêm rack procedural `KIẾM LV1` ở góc phải dưới để player-visible rằng module class thứ hai đã đi vào scene.

## Next after 2D-04

Ưu tiên production map pass cho Linh Thành/Đông Môn: thêm landmark/parallax/route nodes rõ hơn theo GDD 2D mới, giữ scope nhỏ và kiểm runtime visual thật.

## 2026-09-09 — Đông Môn landmark/parallax pass

Thêm `DongMonLandmarks` vào map catalog cho Chapter 1 `Vết Nứt Đông Môn`: Cổng Linh Thành, Bia Luyện Khí, Cầu Gỗ, Thác Nước, Sóng Linh và Rừng Ngoại Thành, kèm layer index + route node. Runtime scene thêm silhouette cầu, thác, rừng và sóng linh để map đọc được nhiều mốc hơn nhưng không che HUD/NPC/combat. Visual manifest ghi `Landmarks: Chapter 1` trong `runtimeMapSnapshot`.

## Next after landmark pass

Tiếp tục map production theo hướng tileset/terrain collision và route node cho jump/dash, hoặc mở inventory try-on UI 2D nhẹ nếu cần kiểm trang bị Võ/Kiếm từ hành trang.

## 2026-09-09 — 2D inventory try-on runtime spine

Đưa flow hành trang đã duyệt vào runtime 2D ở mức nhẹ: scene Đông Môn có strip `HÀNH TRANG` với icon Võ/Kiếm/kiếm starter, selected item `top_kiem_lv1_male`, preview text và snapshot `RuntimeInventoryTryOnSnapshot`. Visual manifest thêm `runtimeInventoryTryOnSnapshot`, bảo vệ flow `select_icon -> inspect_item -> try_on -> cancel_or_apply` và loadout preview `status=TRYING_ON`.

## Next after inventory try-on spine

Tiếp tục bằng tileset/terrain collision pass cho Đông Môn hoặc nâng inventory từ strip preview sang panel inspect/try/apply/cancel có input thật.


## 2026-09-09 — 2D scenario production spine + terrain collision checkpoint

Owner cung cấp kịch bản game mới để khóa lại trước design: 2D Side-Scrolling Social Action MMORPG, HD 2D anime/illustrated, không pixel-art, Zone Network, Chapter 1 Vết Nứt Đông Môn, class identity Võ/Kiếm/Pháp/Cơ/Linh, pipeline item/map và roadmap 2D-00 → 2D-12. Đã đưa vào `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md` và cập nhật GDD/NEXT-ACTION để batch sau đọc trực tiếp. Terrain collision Đông Môn hoàn tất ở mức spine bằng `DongMonCollisionBands`, runtime cues `JUMP GAP`/`DASH LANE`, visual manifest `runtimeTerrainCollisionSnapshot` và visual review.


## 2026-09-09 — 2D inventory input runtime spine

Nâng inventory từ strip preview tĩnh sang input runtime local: `I` mở panel, `Tab` chọn item, `T` thử, `Y` áp dụng, `Esc` hủy. Controller expose `RuntimeInventoryInputSnapshot`; visual capture tăng lên 10 frame với `09-inventory-try` và `10-inventory-applied`, chứng minh áo Kiếm Lv1 có thể thử/áp dụng trên base Võ Lv1 qua cùng loadout/layer mechanism. Không mở economy, loot hoặc persistence thật.

## 2026-09-09 — Đông Môn tilemap runtime spine

Đã thêm `DongMonTileDefinitions` vào map catalog cho ground grass/stone, wood platform, jump gap, dash lane và slime arena. Runtime onboarding vẽ procedural tile strip/platform/gap/dash/slime cues có thể nhìn thấy trong Player capture, expose `RuntimeTilemapSnapshot` và ghi `runtimeTilemapSnapshot` vào visual manifest. Scope là spine runtime để thay bằng authored tileset/sprite atlas sau; không dùng ảnh source, không mở biome mới và không đổi frozen surfaces.

## Next after tilemap runtime spine

Nâng Đông Môn bằng parallax spacing/foreground polish hoặc chuyển chunk procedural sang authored Unity Tilemap asset khi asset sạch sẵn sàng; tránh đụng luồng class/art đang chạy song song.

## 2026-09-09 — Runtime smoke matrix 2D evidence gate

LGO_RUNTIME_SMOKE_MATRIX_READY. Đã nâng `tools/lgo_runtime_smoke_matrix.py` với phase `two-d` để kiểm evidence runtime hiện tại mà không chạy lại Unity: onboarding smoke JSON, macOS Player build log và visual capture manifest. Gate `two_d_visual_capture` yêu cầu `status=PASS`, `screenshotCount>=10`, `finalStep=Complete`, `runtimeTilemapSnapshot` chứa `ChunkFlow` và inventory input đã applied. Validator `tools/validate_lgo_runtime_smoke_matrix.py` cũng kiểm list phase 2D để task LGO-TASK-047 không lặp lại. Evidence: `python3.12 tools/lgo_runtime_smoke_matrix.py --phase two-d` in `LGO_RUNTIME_SMOKE_MATRIX_2D_PASS`.

## Next after runtime smoke matrix

Tiếp tục task roadmap an toàn kế tiếp từ advisor; nếu làm map/runtime player-visible thì vẫn chạy Unity compile, smoke, Player build/capture và matrix 2D sau khi sinh evidence mới.

## 2026-09-09 — Visual evidence matrix 2D current views

LGO_VISUAL_EVIDENCE_MATRIX_READY. Đã nâng `tools/lgo_visual_evidence_matrix.py` với `TWO_D_ONBOARDING_VIEWS` và mode `--verify-current`: kiểm manifest `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`, 5 screenshot runtime trọng yếu (`01-initial`, `02-gate-focus`, `07-skill-ready`, `09-inventory-try`, `10-inventory-applied`) và các field HUD/route/base/combat/animation/inventory. Output xác nhận `LGO_VISUAL_EVIDENCE_MATRIX_2D_CURRENT_PASS`; mọi view giữ non-claim như not production art/combat/inventory.

## Next after visual evidence matrix

Tiếp tục task advisor kế tiếp; nếu mở runtime/player-visible mới thì sinh lại screenshot/manifest rồi chạy smoke matrix và visual matrix tương ứng.

## 2026-09-09 — Crash/error reporting plan closure

LGO_CRASH_REPORTING_PLAN_READY. Đã xác minh plan local crash/error reporting hiện có bằng `python3.12 tools/validate_lgo_crash_error_reporting_plan.py`: PASS. Tool `tools/lgo_error_report_summary.py` phân loại missing closure summary là `UNVERIFIED_ENVIRONMENT`, và plan giữ rõ các class `FIX_REQUIRED`, `UNVERIFIED_ENVIRONMENT`, `CONTRACT_CHANGE_REQUIRED`, `HUMAN_REVIEW_REQUIRED`. Không tích hợp production crash-reporting service, telemetry backend, analytics SDK, auth/DB/economy/social/live-ops hoặc frozen surface.

## Next after crash/error plan

Tiếp tục task advisor kế tiếp trong docs/tools hoặc quay lại map/runtime 2D khi không xung đột tab class.

## 2026-09-09 — Alpha/Beta/Live checklist closure

LGO_RELEASE_CHECKLIST_READY. Đã xác minh checklist release bằng `python3.12 tools/validate_lgo_release_checklist.py`: PASS. Checklist giữ phân loại hiện tại là pre-alpha development, yêu cầu alpha/beta/live gate riêng và stop lines không mở production auth/DB/economy/social/live ops, không claim production art, không đổi frozen contracts. Không có implementation trong batch này.

## Next after release checklist

Tiếp tục task advisor kế tiếp hoặc quay lại cải thiện runtime/player-visible 2D khi scope an toàn và không xung đột tab class.

## 2026-09-09 — Đông Môn parallax/foreground polish

LGO_DONGMON_PARALLAX_POLISH_READY. Đã thêm `ParallaxDepthSnapshot` vào map catalog và render polish procedural cho Đông Môn: cloud drift ở Sky/Fog, mountain silhouette ở Far Background, mist veil/cỏ foreground thấp để runtime bớt phẳng nhưng không che HUD, combat hoặc inventory. Evidence mới: Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS (`totalSize=114184847`, app thực tế `109M`), Player visual capture PASS 10 frame; manifest chứa `ParallaxDepth: Layer5=Sky/Fog:cloud-drift | Layer4=Far Background:mountain-silhouette | Layer0=Foreground:grass-leaf-motes`. Unity log có bước `prepared ~36238M` nhưng output app không phình tương ứng; cần follow-up tối ưu/điều tra incremental build cache nếu build tiếp tục chậm.

## Next after parallax polish

Tiếp tục roadmap 2D không xung đột tab class: hoặc điều tra/tối ưu macOS Player build prepared-size, hoặc nâng inventory inspect/icon grid bằng procedural UI, hoặc chuyển Đông Môn sang authored Tilemap asset khi có asset sạch hợp lệ.

## 2026-09-09 — World/Linh Thành zone-network runtime overlay

LGO_WORLD_ZONE_NETWORK_RUNTIME_READY. Đã đưa World Map/Linh Thành hub vào runtime spine: `MapZoneConnection[]`, `ZoneNetworkSnapshot`, `WorldMapNetwork: hub=linh-thanh`, `LinhThanhHubRuntime` cho Đông Môn/Quảng Trường/Học Viện/Đền Linh/Khu Dân Cư. Minimap overlay trong Player hiện có node nhỏ LT ↔ Đông Vực/Âm Giới để Đông Môn không còn là sân luyện cô lập. Evidence: RED `build/tdd-red/check_zone_network_overlay_red.py` fail rồi PASS, Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS (`totalSize=114649059`, errors=0), Player visual capture PASS 10 frame; runtime smoke matrix 2D đã nâng để bắt `runtimeMapSnapshot` chứa `WorldMapNetwork` và `LinhThanhHubRuntime`.

## Next after zone-network overlay

Map chưa xong toàn bộ. Next map-safe action: mở dần Linh Thành hub shell/Quảng Trường hoặc authored Đông Môn tilemap khi asset sạch sẵn sàng; nếu tiếp tục gặp cold build `prepared ~36GB`, ưu tiên tối ưu URP/package/build profile riêng trước các vòng visual lớn.

## 2026-09-09 — Linh Thành hub shell runtime checkpoint

LGO_LINHTHANH_HUB_SHELL_RUNTIME_READY. Đã thêm `LinhThanhHubShellSnapshot` vào map catalog và runtime scene: hub shell giữ district Đông Môn, Quảng Trường, Học Viện, Thương Phố; visual overlay phía sau cổng cho thấy Đông Môn nối vào Linh Thành xã hội thay vì đứng riêng như sân luyện. Evidence: RED `build/tdd-red/check_linhthanh_hub_shell_red.py` fail rồi PASS, Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS (`totalSize=114650595`, errors=0), Player visual capture PASS 10 frame; ảnh `01-initial` và `09-inventory-try` đã review không che HUD/minimap/inventory. Smoke matrix 2D đã nâng để bắt `HubShell: linh-thanh`, `district=plaza`, `district=market`.

## Next after Linh Thành hub shell

Map chưa xong toàn bộ. Next safe action: mở Quảng Trường hub shell chi tiết hơn hoặc tiếp tục authored Đông Môn Tilemap khi asset sạch sẵn sàng; không mở shop/giao dịch/bang hội/backend khi chưa có gate riêng.

## 2026-09-09 — Quảng Trường plaza shell runtime checkpoint

LGO_LINHTHANH_PLAZA_SHELL_RUNTIME_READY. Đã thêm `LinhThanhPlazaShellSnapshot` và runtime preview cho Quảng Trường: social spawn local-safe, event board preview, guild bulletin preview, kèm contract `safe-no-trade-backend` để không mở giao dịch/economy/bang hội backend. Evidence: RED `build/tdd-red/check_linhthanh_plaza_shell_red.py` fail rồi PASS, Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS (`totalSize=114652131`, errors=0), Player visual capture PASS 10 frame; ảnh `01-initial` và `09-inventory-try` đã review không che HUD/minimap/inventory. Smoke matrix 2D đã nâng để bắt `PlazaShell: district=plaza` và `safe-no-trade-backend`.

## Next after Quảng Trường plaza shell

Map chưa xong toàn bộ. Next safe action: phát triển Quảng Trường thành hub runtime riêng với NPC/board local-only hoặc authored Đông Môn Tilemap khi asset sạch sẵn sàng; không mở shop/giao dịch/bang hội/backend khi chưa có gate riêng.

## 2026-09-09 — Linh Thành unlock presentation runtime checkpoint

LGO_LINHTHANH_UNLOCK_PRESENTATION_READY. Đã nối flow Đông Môn với Linh Thành ở mức runtime local-only: sau `TryUseClassSkill()` đánh tan Shadow Slime, `TwoDOnboardingState.LinhThanhUnlocked` bật true, Objective/HUD chuyển sang “Mở Linh Thành: Quảng Trường”, runtime scene hiện banner/path `LGO 2D Linh Thanh Unlock Banner` + `LGO 2D Plaza Unlock Path`, và visual manifest ghi `runtimeLinhThanhUnlockSnapshot=LinhThanhUnlock: unlocked=True | unlock=plaza | source=shadow-slime-complete | route=return-gate->plaza | safe-local-no-teleport`. Evidence: RED compile fail vì thiếu `LinhThanhUnlocked`/`RuntimeLinhThanhUnlockSnapshot`, Unity EditMode PASS, Editor smoke PASS với `linhThanhUnlocked=true`, macOS Player build PASS, Player visual capture PASS 10 frame, smoke matrix 2D PASS; ảnh `08-complete`/`09-inventory-try` đã review, banner không che HUD/minimap/inventory.

## Next after Linh Thành unlock presentation

Map chưa xong toàn bộ. Next safe action: phát triển Quảng Trường hub runtime riêng có NPC/board local-only hoặc chuyển Đông Môn procedural chunk sang authored Tilemap/Tile palette khi asset sạch sẵn sàng; không mở teleport thật, shop/giao dịch/bang hội/backend khi chưa có gate riêng.

## 2026-09-09 — Quảng Trường hub runtime preview local-only

LGO_LINHTHANH_PLAZA_HUB_RUNTIME_READY. Đã nâng Quảng Trường từ shell marker thành runtime preview sau unlock Đông Môn: map catalog có `LinhThanhPlazaHubRuntimeSnapshot`, controller expose `RuntimeLinhThanhPlazaHubSnapshot`, visual manifest ghi `runtimeLinhThanhPlazaHubSnapshot=PlazaHubRuntime: district=plaza | npc=gate-guide | npc=wandering-student | board=event-local-preview | guild-bulletin=locked | social-spawn=local-safe | safe-local-no-backend | unlocked=True`. Runtime scene bật cụm NPC hướng dẫn, học viên lang thang, bảng sự kiện local preview và bảng bang hội locked sau khi `LinhThanhUnlocked=true`; label đã được parent vào root để không hiện sớm ở frame initial. Evidence: RED compile fail vì thiếu snapshot catalog/controller, Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 10 frame, smoke matrix 2D PASS; ảnh `01-initial` và `08-complete` đã review.

## Next after Quảng Trường hub runtime preview

Map vẫn chưa production-complete. Next safe action: thêm interaction local-only cho bảng sự kiện/NPC Quảng Trường hoặc chuyển Đông Môn procedural chunk sang authored Tilemap/Tile palette khi asset sạch sẵn sàng; không mở teleport thật, shop/giao dịch/bang hội/backend khi chưa có gate riêng.
## 2026-09-10 — Quảng Trường board interaction + truthful Unity test gate

`LGO_LINHTHANH_PLAZA_BOARD_INTERACTION_READY`: sau khi hoàn tất Đông Môn/Shadow Slime và unlock Linh Thành, state runtime cho phép mở preview local-only của `Bảng sự kiện Quảng Trường`. HUD chuyển đúng sang `Khu vực: Quảng Trường`, visual manifest ghi `runtimeLinhThanhPlazaHubSnapshot` với `interaction=board-preview-open`, và visual capture có frame `11-plaza-board-preview`. Inventory panel được ẩn mặc định và chỉ hiện khi người chơi mở hành trang để board preview không bị che.

Đồng thời sửa `tools/unity_batch_test.sh`: bỏ `-quit` khỏi `-runTests`, bắt buộc có XML results, `total>0`, `passed>0`, `failed=0` trước khi in `UNITY_EDITMODE_PASS`. Evidence mới: Unity EditMode thật `total=98 passed=97 failed=0`, Editor smoke PASS, macOS Player build PASS (`totalSize=114657763`), `tools/lgo_runtime_smoke_matrix.py --phase two-d` PASS và `tools/lgo_visual_evidence_matrix.py --verify-current` PASS. Scope vẫn local-only: chưa mở teleport/shop/giao dịch/bang hội/backend.

## 2026-09-10 — Quảng Trường NPC local interaction

Checkpoint `LGO_LINHTHANH_PLAZA_NPC_INTERACTION_READY`: Quảng Trường có NPC local preview cho Người Giữ Cổng/Thương Nhân sau unlock Đông Môn, capture `12-plaza-npc-preview`, manifest bắt `npc=merchant-preview`, `interaction=npc-merchant-preview`, `safe-local-no-shop-backend`. Next map-safe action: authored Đông Môn Tilemap hoặc nâng Plaza NPC selector/input thật, vẫn không mở shop/backend.

## 2026-09-10 — Quảng Trường target selector/input checkpoint

`LGO_LINHTHANH_PLAZA_TARGET_SELECTOR_READY`: sau khi unlock Linh Thành, Quảng Trường có selector local-only cho ba mục tiêu hub: Bảng Sự Kiện, Người Giữ Cổng và Thương Nhân. Input runtime: `P` đổi mục tiêu, `E/Enter` tương tác mục tiêu đang chọn; manifest ghi `runtimePlazaHubInputSnapshot`, visual capture có frame `12-plaza-target-selector` và `13-plaza-npc-preview`. Gate đã chạy: Unity EditMode, Editor smoke, macOS Player build/capture, runtime smoke matrix và visual evidence matrix. Next map-safe action: tiếp tục A-Z map bằng Quảng Trường social layout/Đông Môn authored Tilemap hoặc hub transition shell, vẫn không mở shop/economy/teleport/bang hội/backend khi chưa có gate riêng.

## 2026-09-10 — Quảng Trường social layout spacing

`LGO_LINHTHANH_PLAZA_SOCIAL_LAYOUT_READY`: sau selector checkpoint, Quảng Trường được chỉnh spacing thành `spaced-social-triangle` để Gate Guide, Bảng Sự Kiện và Thương Nhân không dính vào nhau khi capture. TDD RED/GREEN bắt `runtimePlazaHubInputSnapshot` có layout token; Player build/capture/matrix PASS với 13 frame. Next map-safe action: nâng hub transition/local route hoặc authored Đông Môn Tilemap; vẫn không mở backend/frozen surfaces.

## 2026-09-10 — Đông Môn → Quảng Trường transition shell

`LGO_LINHTHANH_HUB_TRANSITION_PREVIEW_READY`: sau unlock Linh Thành, runtime có preview tuyến `east-gate -> plaza` local-only với snapshot `runtimeHubTransitionSnapshot` và frame `14-plaza-transition-preview`. Gate đã chạy: test RED compile fail vì API chưa có, GREEN Unity EditMode `102 total / 101 passed / 0 failed / 1 skipped`, Editor smoke PASS, macOS Player build PASS, visual capture 14 frame, runtime/visual matrix PASS. Next map-safe action: làm authored Đông Môn Tilemap hoặc giảm chồng label Quảng Trường; chưa mở teleport/backend.

## Quảng Trường label readability checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_LABEL_READABILITY_READY`: Quảng Trường đã giảm mật độ chữ trong world-space sau unlock Đông Môn: nhãn chi tiết quanh NPC/board được thay bằng chip `01/02/03`, banner unlock lớn tự ẩn khi tutorial đã `Complete`, manifest có `runtimePlazaReadabilitySnapshot` với `mode=label-rail`, `world-label-density=reduced`, `target-chips=event-board,gate-guide,merchant-preview`. Evidence mới: Unity EditMode `total=102 passed=101 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame và visual matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là mở Học Viện/Thương Phố shell hoặc authored Đông Môn Tilemap khi asset sạch sẵn sàng, tránh đụng luồng class/art tab song song.

## Học Viện shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_ACADEMY_SHELL_RUNTIME_READY`: Linh Thành có thêm Học Viện shell local-only trong catalog/scene/runtime manifest. `AcademyShell: district=academy` ghi `skill-hall=preview-only`, `class-trainer=locked`, `lecture-board=local-preview`, `safe-no-skill-backend`, `safe-local-no-backend`; scene có silhouette Học Viện/skill board/trainer locked ở hậu cảnh, không mở skill backend hoặc class progression mới. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=104 passed=103 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là Thương Phố shell hoặc authored Đông Môn Tilemap khi asset sạch sẵn sàng, vẫn tránh đụng luồng class/art tab song song.

## Thương Phố shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_MARKET_SHELL_RUNTIME_READY`: Linh Thành có thêm Thương Phố shell local-only trong catalog/scene/runtime manifest. `MarketShell: district=market` ghi `vendor-row=preview-only`, `auction-board=locked`, `item-stall=local-preview`, `safe-no-trade-backend`, `safe-no-economy-backend`, `safe-local-no-backend`; scene có awning/stall/auction-board locked ở hậu cảnh trái, không mở shop, trade, economy hoặc backend. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=106 passed=105 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap hoặc Đền Linh/Khu Dân Cư shell nếu tiếp tục mở Linh Thành theo zone network.

## Đền Linh shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_SPIRIT_TEMPLE_SHELL_RUNTIME_READY`: Linh Thành có thêm Đền Linh shell local-only trong catalog/scene/runtime manifest. `SpiritTempleShell: district=spirit-temple` ghi `blessing-altar=preview-only`, `story-shrine=locked`, `incense-vfx=local-preview`, `safe-no-buff-backend`, `safe-no-story-backend`, `safe-local-no-backend`; scene có shrine/altar/incense VFX ở hậu cảnh phải, không mở buff, story quest backend hoặc progression mới. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=108 passed=107 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là Khu Dân Cư shell để mở social residential spine hoặc authored Đông Môn Tilemap khi asset sạch.

## Khu Dân Cư shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_RESIDENTIAL_SHELL_RUNTIME_READY`: Linh Thành có thêm Khu Dân Cư shell local-only trong catalog/scene/runtime manifest. `ResidentialShell: district=residential` ghi `npc-home-row=preview-only`, `social-chat-node=locked`, `ambient-citizen=local-preview`, `safe-no-housing-backend`, `safe-no-social-backend`, `safe-local-no-backend`; scene có nhà dân/NPC ambient/chat node locked ở hậu cảnh, không mở housing backend, social backend hoặc player-owned home. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=110 passed=109 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc Khu Rèn shell nếu tiếp tục mở Linh Thành theo zone network.

## Khu Rèn shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_FORGE_SHELL_RUNTIME_READY`: Linh Thành có thêm Khu Rèn shell local-only trong catalog/scene/runtime manifest. `ForgeShell: district=forge` ghi `anvil-row=preview-only`, `craft-board=locked`, `forge-glow=local-preview`, `safe-no-crafting-backend`, `safe-no-upgrade-economy`, `safe-local-no-backend`; scene có workshop/anvil/forge glow/craft board locked ở hậu cảnh, không mở crafting backend, upgrade economy hoặc item mutation. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=112 passed=111 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc Khu Bang Hội shell khi cần mở trục cộng đồng.

## Khu Bang Hội shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_GUILD_SHELL_RUNTIME_READY`: Linh Thành có thêm Khu Bang Hội shell local-only trong catalog/scene/runtime manifest. `GuildShell: district=guild` ghi `guild-hall=preview-only`, `guild-banner=local-preview`, `notice-board=locked`, `safe-no-guild-backend`, `safe-no-membership-backend`, `safe-local-no-backend`; scene có guild hall/banner/notice board locked ở hậu cảnh phải, không mở guild backend, membership, chat hoặc guild progression. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=114 passed=113 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc gate/harbor shell để mở travel affordance local-only.


## Cảng Linh Thuyền shell runtime checkpoint — 2026-09-10

`LGO_LINHTHANH_HARBOR_SHELL_RUNTIME_READY`: Linh Thành có thêm Cảng Linh Thuyền shell local-only trong catalog/scene/runtime manifest. `HarborShell: district=harbor` ghi `spirit-boat=preview-only`, `travel-board=locked`, `dock-lantern=local-preview`, `safe-no-travel-backend`, `safe-no-teleport-backend`, `safe-local-no-backend`; scene có dock/spirit boat/travel board locked ở vùng thấp, không mở travel backend, teleport hoặc world-route execution. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=116 passed=115 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Đông Môn Tilemap/collision polish hoặc gate shell để mở travel affordance local-only.


## Đông Môn authored detail pass runtime checkpoint — 2026-09-10

`LGO_DONG_MON_AUTHORED_DETAIL_PASS_READY`: Đông Môn có thêm authored detail pass bám route/collision thay vì trang trí ngẫu nhiên. Catalog/controller/visual manifest expose `runtimeDongMonAuthoredPassSnapshot` với `DongMonAuthoredPass`, `route-segments=5`, `collision-bands=5`, `detail-density=readable`, `collision-boundaries=from-bands`, `foreground-fringe=controlled`, `landmark-silhouettes=anchored`, `no-random-decoration`, `safe-no-procedural-spam`; runtime thêm moss/stone-step/bridge-rope/dash-dust/slime-rune nhỏ theo chunk và collision band để cải thiện hình ảnh mà không che HUD/player/combat. Evidence: RED Unity compile fail đúng vì thiếu snapshot, GREEN Unity EditMode `total=118 passed=117 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là authored Tilemap asset/palette thật hoặc gate shell local-only khi không đụng luồng class/art song song.


## Đông Môn tile palette runtime checkpoint — 2026-09-10

`LGO_DONG_MON_TILE_PALETTE_RUNTIME_READY`: Đông Môn có runtime tile palette contract cho 6 tile chính, chuẩn bị thay procedural chunk bằng authored Tilemap asset/palette thật mà không đổi gameplay flow. Catalog/controller/visual manifest expose `runtimeDongMonTilePaletteSnapshot` với `DongMonTilePalette`, `tile_ground_grass:earth-green:soft-grass-edge`, `tile_ground_stone:spirit-cyan:stone-step-line`, `tile_platform_wood:warm-wood:rope-rail`, `tile_gap_marker:void-shadow:negative-space`, `tile_dash_lane:spirit-cyan:wind-streak`, `tile_slime_arena:violet-corruption:rune-boundary`, `safe-no-source-image`, `safe-runtime-palette-contract`; runtime thêm dải swatch thấp để kiểm màu/role mà không che HUD/player/combat. Evidence: RED Unity compile fail đúng vì thiếu palette snapshot, GREEN Unity EditMode `total=120 passed=119 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là chuyển palette contract này thành authored Tilemap asset/palette Unity thật hoặc gate shell local-only khi không đụng luồng class/art song song.
## Đông Môn authored source asset runtime checkpoint — 2026-09-10

`LGO_DONG_MON_AUTHORED_SOURCE_ASSET_READY`: Đông Môn có authored runtime source asset `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonTilePalette.json` được đóng gói qua Unity Resources và load trong Editor/Player bằng `TwoDMapDesignCatalog.LoadDongMonTilePaletteSourceSnapshot()`. Runtime manifest expose `runtimeDongMonTilePaletteSourceSnapshot` với `DongMonTilePaletteSource`, `resource=LGOMaps/DongMonTilePalette`, `tile_dash_lane=True`, `safe-no-source-image=True`, `safe-runtime-resource=True`; scene beat ghi nguồn palette để future Tilemap asset/palette thay thế không đổi gameplay flow. Evidence: RED Unity compile fail đúng vì thiếu source snapshot API, GREEN Unity EditMode `total=122 passed=121 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là parse JSON này thành authored chunk placement hoặc tạo Unity Tilemap/Tile palette asset thật khi không đụng luồng class/art song song.
## Đông Môn authored chunk placement runtime checkpoint — 2026-09-10

`LGO_DONG_MON_AUTHORED_CHUNK_PLACEMENT_READY`: Đông Môn đã có authored runtime chunk placement source `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonChunkPlacement.json`. Runtime load data bằng `TwoDMapDesignCatalog.LoadDongMonAuthoredChunkPlacements()` và render các chunk tile từ Resource thay vì giữ toàn bộ vị trí/count hardcoded trong scene builder. Visual manifest expose `runtimeDongMonChunkPlacementSourceSnapshot` với `DongMonChunkPlacementSource`, `resource=LGOMaps/DongMonChunkPlacement`, `chunk_gate_entry@-3.70,-2.02x4`, `chunk_dash_lane@1.82,-1.02x4`, `chunk_slime_arena@3.25,-1.36x2`, `authored-placement=True`, `safe-runtime-resource=True`, `safe-no-3d=True`. Evidence: RED Unity compile fail đúng vì thiếu chunk placement API/snapshot, GREEN Unity EditMode `total=124 passed=123 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 14 frame, runtime smoke matrix PASS và visual evidence matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là tách placement này thành Tilemap/Tile palette asset thật hoặc thêm label/layout pass cho Đông Môn/Quảng Trường khi không đụng luồng class/art song song.

## Đông Môn route label rail checkpoint — 2026-09-10

`LGO_DONG_MON_ROUTE_LABEL_RAIL_READY`: Đông Môn có readability pass player-visible cho route label: world-space collision labels `JUMP GAP`/`DASH LANE` được giảm thành chip `03`/`04`, scene thêm rail route với `01 Cổng`, `02 Bia`, `03 Jump`, `04 Dash`, `05 Slime`, và manifest expose `runtimeDongMonReadabilitySnapshot` với `mode=route-label-rail`, `world-label-density=reduced`, `chips=gate,stone,jump,dash,slime`, `avoids-hud-overlap`, `safe-local-no-backend`. Evidence: RED Unity compile fail đúng vì thiếu `RuntimeDongMonReadabilitySnapshot`, GREEN Unity EditMode `total=125 passed=124 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, 2D Player visual capture PASS 14 frame không dùng `-nographics`, runtime smoke matrix PASS và visual evidence matrix PASS. Visual vẫn là blockout/prototype art, chưa production-complete; next safe map action là chuyển placement/palette thành Unity Tilemap asset thật hoặc tiếp tục polish layout Đông Môn/Linh Thành.

## Đông Môn Unity Tilemap runtime checkpoint — 2026-09-10

`LGO_DONG_MON_UNITY_TILEMAP_RUNTIME_READY`: Đông Môn đã có Unity `Grid` + `Tilemap` + `TilemapRenderer` runtime layer thật, lấy cell từ `Resources/LGOMaps/DongMonChunkPlacement.json` và palette role từ `Resources/LGOMaps/DongMonTilePalette.json`. Visual layer này đang là underlay mảnh dưới procedural strip để chứng minh pipeline Tilemap sạch trước khi thay toàn bộ blockout art; manifest expose `runtimeDongMonUnityTilemapSnapshot` với `DongMonUnityTilemap`, `renderer=TilemapRenderer`, `grid=Grid`, `source=LGOMaps/DongMonChunkPlacement`, `cells=16`, `safe-no-source-image`, `safe-no-3d`. Evidence: RED Unity compile fail đúng vì thiếu `RuntimeDongMonUnityTilemapSnapshot`, GREEN Unity EditMode `total=126 passed=125 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, 2D Player visual capture PASS 14 frame không dùng `-nographics`, runtime smoke matrix PASS và visual evidence matrix PASS. Visual vẫn là blockout/prototype art; next safe map action là chuyển dần procedural strip sang Tilemap/atlas đẹp hơn hoặc polish Linh Thành layout, không mở shop/economy/teleport/backend.
## 2026-09-10 — Quảng Trường layout anchors

Checkpoint `LGO_LINHTHANH_PLAZA_LAYOUT_ANCHORS_READY`: thêm snapshot/manifest `runtimePlazaHubLayoutSnapshot` và marker scene cho social-spawn/event-board/gate-guide/merchant-preview/guild-locked trong Quảng Trường local preview. Next: inspect UI local-only cho anchor hoặc production tile/atlas khi asset sạch.
## 2026-09-10 — Quảng Trường anchor detail inspect

Checkpoint `LGO_LINHTHANH_PLAZA_ANCHOR_DETAIL_READY`: thêm detail snapshot + world-space detail line cho Plaza target đang chọn, giúp frame board/selector/NPC đọc rõ chức năng anchor. Scope local-only.
## 2026-09-10 — LGO_LINHTHANH_DISTRICT_PREVIEW_READY

Hoàn tất checkpoint map/runtime nhỏ: Linh Thành district preview rail sau unlock Đông Môn, control `M`, route `plaza->academy`, safe local-only. Evidence: Unity EditMode, smoke, macOS Player build và visual capture 15 frame.
## 2026-09-10 — LGO_LINHTHANH_DISTRICT_DETAIL_READY

Hoàn tất checkpoint district cycle/detail: thêm detail snapshot theo khu và visual frame Thương Phố. Evidence gồm RED compile fail, EditMode pass, smoke pass, macOS Player build và visual capture 16 frame.

## 2026-09-10 — Linh Thành Đền Linh district preview runtime checkpoint

`LGO_LINHTHANH_SPIRIT_TEMPLE_PREVIEW_READY`: thêm Player visual frame `17-district-spirit-temple-preview` cho district preview rail Linh Thành. Flow capture chuyển Học Viện → Thương Phố → Đền Linh, manifest giữ `DistrictPreviewRail selected=spirit-temple label=Đền Linh route=plaza->spirit-temple` và `DistrictDetail role=story-blessing-preview detail=altar-local-only next=quest-buff-gate`. Evidence: Unity EditMode `total=131 passed=130 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 17 frame, visual evidence matrix PASS. Giới hạn: local-only, chưa mở buff/story/district backend và chưa phải production art map hoàn chỉnh.

## 2026-09-10 — Linh Thành district rail coverage runtime checkpoint

`LGO_LINHTHANH_DISTRICT_RAIL_COVERAGE_READY`: mở rộng Player visual capture cho district preview rail từ Đền Linh tới Khu Rèn, Khu Bang Hội và Cảng Linh Thuyền. Capture mới có `18-district-forge-preview`, `19-district-guild-preview`, `20-district-harbor-preview`; manifest cuối giữ `DistrictPreviewRail selected=harbor label=Cảng Linh Thuyền route=plaza->harbor` và `DistrictDetail role=travel-preview detail=spirit-boat-locked next=world-route-gate`. Evidence: Unity EditMode `total=132 passed=131 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, visual capture PASS 20 frame, visual evidence matrix PASS. Giới hạn: local-only, chưa mở crafting/guild/travel/teleport/backend và chưa phải production art map hoàn chỉnh.

## 2026-09-10 — Linh Thành district rail readability runtime checkpoint

`LGO_LINHTHANH_DISTRICT_RAIL_READABILITY_READY`: polish district preview rail để callout/backplate đi theo node đang chọn thay vì nằm cố định ở đáy màn; thêm snapshot `DistrictRailReadability` và gate yêu cầu `label-follows-selected=True`, `backplate=follows-selected`, `callout-size=readable`, `avoids-hud-overlap`. Evidence: Unity EditMode `total=133 passed=132 failed=0 skipped=1`, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 20 frame; đã review frame Khu Rèn/Bang Hội/Cảng sau polish.

## Next after minimap compact readability

`LGO_MINIMAP_COMPACT_READABILITY_READY`: minimap/runtime overlay được rút gọn để dễ đọc hơn trong Player: route text ngắn, district chips `HV TP ĐL KR BH CẢ`, world links dạng ngắn, manifest có `runtimeMinimapReadabilitySnapshot`/`MinimapReadability`. Map chưa xong production toàn bộ; checkpoint này chốt readability của foundation map hiện tại. Next: authored Đông Môn Tilemap/atlas sạch hoặc mở district detail kế tiếp local-only.

## Next after Đông Môn authored detail resource

`LGO_DONG_MON_AUTHORED_DETAILS_RESOURCE_READY`: detail pass Đông Môn được data hóa qua `DongMonAuthoredDetails.json` thay vì đặt từng sprite trong controller. Runtime manifest/matrix bắt `runtimeDongMonAuthoredDetailSourceSnapshot` và 7 detail role. Next: atlas/sprite pipeline hoặc data hóa layout props/NPC khu tiếp theo.

## Next after Gate Keeper NPC sprite source

`LGO_DONG_MON_GATEKEEPER_NPC_SPRITE_SOURCE_READY`: Người Giữ Cổng được dựng từ resource `DongMonNpcSprites.json` với 13 sprite parts/slot thay vì blockout generic; manifest/matrix bắt snapshot. Next: atlas sprite thật hoặc data hóa NPC/props còn lại.
- LGO_2D_PRODUCTION_WORKFLOW_RESEARCH_READY: đã ghi `docs/execution/LGO-2D-PRODUCTION-WORKFLOW-RESEARCH-v0.1.md`; batch sau ưu tiên pipeline Tilemap/Sprite Atlas/paper-doll, dùng `tools/capture_lgo_2d_onboarding_visual.py`, không polish bằng rectangle primitive kéo dài.
## Võ Lv1 run/jump/basic/Liên Quyền — 2026-09-10

Thêm 16 frame mới từ hai sheet nam/nữ đã qua alpha/cell-margin QA, nâng runtime lên 28 motion frame. HUD có action bar bốn nút và keyboard Shift/J/Z/X; sửa capture chạy nền để ba profile không kẹt khi mất focus. EditMode `180/179/0/1`, build 0 lỗi, capture `build/vo-motion-v8/three-profiles/` đạt `38×3` technical pass và đã review trực tiếp các pose cùng tablet/mobile/PC. Atlas tổng 700.563 byte; portal/NPC không bị control panel che sau layout cuối.

Checkpoint chỉ đóng full-frame Lv1. Next làm animated attachment cho 10 slot và tier Lv10/20/30; không nhân sang class khác trước gate đó.

## Võ base skeletal rig — 2026-09-10

Tạo nguồn rig nam/nữ theo ảnh Võ đã chọn, component-QA và pack 20 body segment vào một atlas 24.232 byte. Manifest v6 cấp 120 rig pose + 120 attachment profile; runtime modular dùng segment và joint pivot cho sáu state trên cả hai giới/tier. EditMode `181/180/0/1`; build và Player capture `build/vo-rig-v2/three-profiles/` đạt 46×3 technical pass. Visual review giữ trạng thái chưa hoàn chỉnh vì slot `boots`/`arm_guard` và các phần hai bên chưa tách để bind từng bone; next `CLASS-VO-01B2`, chưa mở class khác.
## Võ base-first rig + equipment attachment — 2026-09-10

TDD bắt lỗi combined slot, thiếu hierarchy và world/local rotation. Runtime chung `TwoDSkeletalPaperDollRig` nay quản lý parent bones, pivot và attachment; Võ dùng 96 metadata attachment mà không tăng 724.795 byte texture. EditMode 180 pass/0 fail/1 ignored; capture 46×3 technical pass. Visual còn `GARMENT_ART_VISUAL_FIX_REQUIRED`, đặc biệt Võ nữ Lv30; next sửa source garment theo rig rồi mới claim class pass hoặc mở class thứ hai.

## Võ garment source/atlas batch v8 — 2026-09-10

Tạo đủ 8 source sheet theo một layout 4×3 và pack một lượt thành 112 attachment; các slot áo giáp/hạ trang/găng/giày có nhiều component theo bone. Tổng 7 atlas 885.234 byte. Player v18 capture `build/vo-garment-v1/three-profiles/` đạt 46×3 và visual Lv30 nữ đã rõ outfit/action. Next capture ma trận 10 slot nam/nữ Lv1/Lv30; chưa đóng `CLASS-VO-01`.

## Võ ten-slot visual matrix v9 — 2026-09-10

Thêm matrix 20 ảnh tháo từng slot và bắt lỗi tóc còn bake trong base rig. Thay hai base head không tóc, giữ hairstyle thành attachment. EditMode 181 pass/0 fail/1 ignored; Player v20 capture 66×3 pass và review PC/mobile/tablet. `CLASS_VO_LV1_30_VERTICAL_SLICE_PASS`; next refactor orchestration base trước class thứ hai.
## Shared character runtime state — 2026-09-10

`TwoDCharacterRuntimeState` hiện sở hữu presentation selection, loadout 10 slot, locomotion hold và exclusive action timer/progress; Map01A chỉ còn render rig và xử lý quest/damage. TDD RED/GREEN và toàn bộ EditMode đạt `184/183/0/1`; Player v22 + evidence 66×3 xác nhận shared state, equip, action/hit và Q01–Q09 không regression. Next audit/cắt theo lô source Kiếm Lv1–30 nam/nữ rồi nối cùng base; không sao chép controller Võ.
## Kiếm Lv1–30 source candidate extraction + compatibility — 2026-09-10

Chọn grid redraw nam/nữ Kiếm cùng art direction và tách một lượt 80 source candidate vào external `source-v4`; tool kiểm hash, đúng 10 slot canonical và luôn chặn runtime eligibility. Visual review ghi 12 crop nữ `redraw-required`, 68 crop còn lại `base-fit-unverified`. Thêm compatibility runtime/test cho mix chéo level theo item/slot, chặn candidate/redraw, class/skeleton/body/bone sai và hợp thành coverage/occlusion. TDD cuối `191/190/0/1`, Player build 0 error và onboarding smoke PASS. Next fit/redraw trước atlas; chưa mở class thứ ba.

## Kiếm side-view redraw batch — 2026-09-10

Không ép 80 crop front/3/4 cũ lên side-view base. Tạo batch-v2 nam/nữ đúng 4 level × 10 slot; nữ v1 fail body/tay và được sửa đúng hai row thành v2. Nâng extractor bằng TDD cho magenta feather/despill; source-v3 external có 80 PNG, contact/provenance/audit, runtime eligible bằng 0. Next benchmark Unity 2D Animation 13.x và SpriteSkin mixed-level fit trước atlas/runtime.

## Kiếm Sprite Library compatibility spike — 2026-09-10

Cài Unity 2D Animation 13.0.0 và thêm adapter Category=`componentId`, Label=`itemId`. Mixed-level resolve độc lập, candidate bị chặn, apply preflight tránh thay đồ nửa chừng; attachment `Skinned` bắt buộc sprite bone data còn `Rigid` theo transform. EditMode `195/194/0/1`; Player benchmark cuối tăng 317.079 byte (0,188%). Next weight/fit bốn món mixed-level nam/nữ và chạy motion evidence; chưa nâng source-v3 lên approved.

## Kiếm mixed-loadout idle prefit — 2026-09-10

Review cận cảnh bắt hair/outer source-v3 còn ghép nhiều view. Giữ weapon Lv1/inner Lv10, chỉ redraw hair Lv30/outer Lv20 nam nữ; hai failure ImageGen được lưu, output key magenta và normalize một lần tại PPU 208. Contact idle nam/nữ đã review PASS cho bước weighting, nhưng runtime eligible vẫn 0. Next import 8 proof asset, weight/skinning và capture sáu motion × ba profile.

## Kiếm proof asset authoring — 2026-09-10

Import đúng 8 item proof vào một atlas 512×512/PPU 208, không scale lại cell. Unity authoring tool ghi bones/weights cho 6 Skinned sprite, giữ 2 kiếm Rigid; validator khóa atlas/hash/provenance và không thêm PSD Importer. EditMode `204/203/0/1`; Player 169.088.250 byte/0 error, delta 0,352% so baseline và nhẹ hơn bản texture rời 202.672 byte. Pack vẫn draft/eligible=0; next opt-in Player motion preview ba profile.

## Kiếm mixed-loadout shared-rig fit — 2026-09-10

Thêm `DraftRuntimeFit` với API preview opt-in; production equip vẫn chặn. Direct bind sai hệ tọa độ đã được thay bằng bone proxy giữ item-local bind pose và theo rotation skeleton chung. EditMode `206/205/0/1`, Player 169.099.226 byte/0 error; Map01A capture 78×3 cho thấy weapon/hair/inner/outer nam/nữ đi cùng idle/walk/run/jump/basic/shared-skill pose, UI ghi DRAFT. Chưa claim full Kiếm: mới 4/10 slot, chưa có skill/VFX riêng và eligible vẫn 0.

## Kiếm remaining six-slot source batch — 2026-09-10

Pack một lượt 48 crop của 6 slot còn thiếu × 4 mốc level × 2 giới vào hai atlas candidate 1024 ngoài runtime; không resize, manifest có source/atlas rect/hash và eligible 0. Review xác nhận identity chung; arm/boot phải weight hai phía. Next ghép một mixed-level 10-slot contact và chỉ nhập 12 item đã fit, không kéo cả candidate batch vào Player.
