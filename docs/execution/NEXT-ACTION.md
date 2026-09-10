> **Gate Map01A playable visual slice — 2026-09-10:** Cổng Đông Lâm đã đủ tuyến Q01–Q09 trong Player, NPC/interactable/enemy/loot, gather/chest, ba hit combat, portal unlock, minimap route, inventory, dùng Bình Máu và equip Hộ Uyển Võ Tân Thủ. Capture cuối có 27 trạng thái trên từng profile mobile/tablet/PC tại `build/map01a-functional-ui/three-profiles-v2/`; panel action đã tự xuống dòng và ảnh inventory/potion/equip/portal đã review trực tiếp. Trạng thái `MAP01A_PLAYABLE_VISUAL_SLICE_PASS`.
>
> **Next:** chuyển sang `CLASS-VO-01` đúng thứ tự Goal. Kế thừa atlas nam/nữ Lv1/10/20/30 và 10 slot hiện có; làm một batch articulated rig/attachment theo đầu-ngực-hông-tay-chân, rồi nối idle/walk/run/jump/basic attack/`Liên Quyền` sao cho trang bị đang mặc đi đúng pose. Chỉ test/build/capture ba profile sau khi trọn batch đã nối; chưa mở Kiếm/Pháp/Cơ/Linh. Map pass không phải chứng nhận thiết bị vật lý, persistence/backend hoặc scene Map01B.

> **Checkpoint Võ modular motion alignment — 2026-09-10:** base/modular và các tier Lv10/20/30 giữ nguyên toàn bộ layer đang mặc khi walk/skill bằng một root pose chung; full-frame motion chỉ dùng cho `Lv1 + full`. Capture mobile/tablet/PC đã chứng minh nữ modular sau khi tháo `inner_top` vẫn chỉ còn `base + 9 slot`, slot đã tháo không xuất hiện lại; frame skill vẫn có hit `100→65 HP`. Evidence: `build/map01a-art/vo-aligned-modular-final-three-profiles-v2/`. Đây là compatibility fix, chưa phải limb rig production hoặc chứng nhận thiết bị vật lý.
>
> **Next:** làm một batch rig/attachment Võ theo các anchor đầu-ngực-hông-tay-chân, rồi nối idle/walk/run/jump/basic attack/`Liên Quyền` cho outfit Lv1–30. Chỉ capture lại sau khi trọn batch asset + motion + equipment đã nối; chưa mở class thứ hai.

> **Checkpoint Võ combat hit — 2026-09-10:** `Liệt Phong Kích` chỉ kích hoạt khi mục tiêu còn sống và trong 2,4 world unit; hit được áp tại key timing, trừ 35 HP, tăng hit counter, đổi tint mục tiêu và ghi feedback lên HUD. Capture 12 trên cả mobile/tablet/PC xác nhận `100→65 HP`; spam trong active window bị chặn. Đây là một skill runtime kiểm chứng được, chưa phải combo `Liên Quyền`, AI/knockback hoặc combat production.
>
> **Next:** đóng phần motion/trang bị khớp nhau cho Võ Lv10/20/30. Ưu tiên rig/attachment theo limb để item tùy chọn đi cùng pose; chỉ dùng full-frame tier sheet nếu batch QA chứng minh không làm nổ tổ hợp asset. Sau gate này mới cân nhắc class thứ hai.

> **Checkpoint Võ progression Lv1/10/20/30 — 2026-09-10:** một sheet chung đã được tách theo batch thành bốn tier, hai giới và 10 slot/tier. Runtime có 96 paper-doll part trong bốn atlas 1024²; motion Lv1 có sáu pose thật cho mỗi giới trong hai atlas riêng. Tổng sáu PNG 643.722 byte. Player cho đổi tier bằng L/touch; capture 18 trạng thái × 3 profile đã technical pass và review trực tiếp frame female walk + Lv10/20/30.
>
> **Next:** hoàn thiện Võ class gate bằng motion khớp từng tier (hoặc rig attachment production) và combat `Liên Quyền` có hit timing/target feedback. Hiện Lv10/20/30 cố ý giữ paper-doll tĩnh khi walk/skill để không tráo ngược sang bộ Lv1; vì vậy chưa claim class Lv1–30 hoàn chỉnh. Gom asset/motion/combat rồi test ba profile một lượt, chưa mở class thứ hai.

> **Checkpoint Võ Lv1 — equipment + motion — 2026-09-10:** runtime hiện có Võ nam/nữ cùng art direction, `base/full` và 10 slot paper-doll có thể chọn/bật/tắt trong Player. Hai atlas 1024² chứa 24 part tĩnh (91.553 byte) và 6 frame Võ nam (124.385 byte); walk và `Liệt Phong Kích` dùng pose thật thay transform giả. Capture cuối có 42 ảnh (14 trạng thái × mobile/tablet/PC) và đã review trực tiếp. EditMode `178 total / 177 passed / 0 failed / 1 skipped`; build macOS `162.751.299` byte, 0 error/13 warning; smoke, matrix và branch guards đều pass.
>
> **Next coherent batch:** tạo theo lô một sheet tiến cấp Võ Lv10/Lv20/Lv30 cho cả nam/nữ và một sheet motion nữ, cùng canvas/foot anchor hiện tại; sau đó nối selector cấp và frame animation rồi mới chạy một lượt test/build/capture ba profile. Đây mới là checkpoint Lv1: chưa claim đủ progression 1–30, motion nữ hoặc combat class hoàn chỉnh; chưa mở Kiếm/Pháp/Cơ/Linh.

> **Checkpoint Võ Lv1–30 trên Map 01A — 2026-09-10:** PC cyan đã được thay trong preview bằng hình Võ nam gold/black kế thừa từ WIP đã audit. Một atlas 512²/23.370 byte chứa base, full, ba slot `inner_top/arm_guard/main_weapon` và slash VFX; cùng canvas/ground anchor nên `full/base/modular` đổi bằng C hoặc touch mà không lệch chân. Walk có transform motion và `Liệt Phong Kích` dùng X/touch. Capture macOS Player 36 ảnh (12 trạng thái × mobile/tablet/PC) đã xem trực tiếp; đây là **workflow proof**, chưa phải animation frame/skeletal production hoặc Võ đủ 10 slot.
>
> **Next:** tiếp tục `CLASS-VO-01` theo một batch asset lớn: tạo/tách đủ bảy slot còn thiếu và bộ nữ trên cùng canvas, rồi làm sprite-frame hoặc skeletal motion thật cho idle/walk/run/jump/basic attack/`Liên Quyền`. Giữ atlas theo cấp dùng lại, target 512–1024, ASTC 6×6 mobile; capture lại ba profile một lần khi cả asset + motion đã nối xong. Không mở class thứ hai hoặc map 01B.

## Batch source props + phủ nền — 2026-09-10

Goal vẫn Map01A Cổng Đông Lâm trước, sau đó một class Lv1–30; không dùng wording Đông Môn/Người Giữ Cổng cũ để mở scope khác. Batch trước là tiến triển thực: cắt89 mảnh/hash/pixel. Batch này cleanup16 props cùng sheet, ghép4 vào opt-in Player bằng atlas256²/33.209 byte và sửa phủ nền cả hai chiều sau parallax.

Evidence `build/map01a-art/source-props-three-profiles/`: 12 frame/3 aspect technical pass; đã xem ba frame village-lane, hết viền đen trên tablet/PC, props đúng vị trí. EditMode167 pass/0 fail/1 skip sau sửa import. Có3 lần test lỗi trước đó (log riêng),1 macOS build159.293.123 byte/0 errors/13 warnings,1 capture batch3profile; không giấu retry. Baseline smoke +20frame capture + matrices pass chỉ chứng minh regression prototype cũ, không chứng nhận Map01A Q01–Q09. Đã xem baseline01/02/03/20 bằng contact sheet, vẫn blockout.

Map **VISUAL_FIX_REQUIRED**: skyline quá nổi và phóng lớn, ground lặp rõ, player primitive, mới arrival/thoại Hạ Vân; thiếu route đầy đủ/NPC/quest/items/combat. Chưa push/claim map pass hoặc mở class. WIP code/main khác giữ nguyên; hai GUID settings tự sinh đã lưu patch rồi restore trong worktree này.

Đã rà contact sheet toàn bộ35 ảnh Đông Lâm: `build/map01a-art/source-audit/index.json`, page01/02. Sheet parallax ảnh26 đúng bộ xanh–vàng nhưng các lớp trong board chỉ vài trăm pixel, không upscale thành nền full-screen. Board34 có sáu NPC toàn thân nhưng nét cartoon/kiểu cổng khác nên chưa trộn. Tiếp theo hoàn thiện nguồn parallax/terrain/NPC thống nhất ở đúng pixel budget rồi mở route Map01A theo contract; giữ group crop/cleanup/atlas, không tạo từng asset lẻ tùy ý. Bảng alpha nhóm16 vẫn có matte dưới bàn trà/trong kệ/chậu: giữ ngoài ingest, không claim sạch cả16.

> Chỉ đạo nguồn mới nhất: dùng bộ đồng nhất, không tin tên canonical cũ. Đã batch-crop 89 mảnh từ sheet08/06; nguồn hiện hành `/Users/minhdc/Projects/Design/LGO-Extracted-2D-Items-v1/map-01a-reviewed`, plan `docs/art/LGO-MAP01A-BATCH-CROP-PLAN.json`. 04/07 bị giữ ngoài batch do sai chức năng/khác style. Tiếp theo cleanup alpha/duplicate theo nhóm, chọn NPC đúng bộ, rồi ghép Map01A; không tự tạo lại từng item. Runtime WIP vẫn VISUAL_FIX_REQUIRED (cover nền tablet/PC + player placeholder); chỉ chạy bộ Unity/build/3-profile capture khi gom xong batch runtime.

> Quy tắc batch và bài học đã commit tại `57b83f0` (AGENTS.md + docs/art/LGO-MAP01A-ASSET-OPTIMIZATION-LESSONS.md). Batch scale/downsample đã chạy 1 EditMode + 1 build + 1 capture 12 ảnh sau khi gom thay đổi cuối. Capture kỹ thuật pass; review ảnh `build/map01a-art/reduced-three-profiles/` phát hiện hở nền phía trên ở tablet/PC: cover nền mới tính theo width, chưa bao cả height và camera offset. Gom sửa này cùng các lỗi visual tiếp theo; không báo visual PASS, không lặp build chỉ để đổi tài liệu.

> Quy tắc thực thi mới: đọc mục “Quy tắc owner — làm theo batch, tránh vòng lặp nhỏ” trong `AGENTS.md`. Chốt kết quả/batch trước sửa; gom lỗi review; chỉ rerun gate theo trigger cụ thể; lưu số lượt và lý do cuối batch.

> Bắt buộc đọc trước batch art/scale: `docs/art/LGO-MAP01A-ASSET-OPTIMIZATION-LESSONS.md`. Owner yêu cầu runtime giảm chất lượng/dung lượng, chia tile dùng lại và lưu bài học. Batch đang gom camera/HUD scale + downsample/tile; budget mới 4 MiB PNG, chưa dùng số 7 MiB cũ làm mục tiêu.

## Owner Goal Override — Cổng Đông Lâm Map 01A — 2026-09-10

Audit sandbox “Chuẩn hóa module 2D class” đã hoàn tất trước khi triển khai map. Không cherry-pick nguyên nhánh vì diff có 48 file xóa và nhánh hiện hành đã đi trước 54 commit riêng. Đã giữ bundle/patch, chọn 26 source chi tiết, xác nhận 28 unit test PASS và phân loại rõ tool/contract/WIP tại `docs/art/LGO-CLASS-STANDARDIZATION-REUSE-AUDIT-v1.md`. Class code chỉ port chọn lọc sau gate Map 01A.

`MAP01A-01` đã khóa bằng runtime JSON: 10 khu, 12 parallax/render layer, 6 NPC, 4 quái chỉ ở combat edge, Q01–Q09, UI safe area và portal Suối Thanh Minh; Player manifest có `runtimeCongDongLamMap01AContractSnapshot`. Review ảnh xác nhận prototype hiện vẫn tối/rectangle và còn label Đông Môn/Linh Thành cũ, nên đây là contract PASS chứ chưa phải visual Map 01A PASS.

Nguồn sản phẩm mới nhất thay các mục Đông Môn/Linh Thành cũ bên dưới:

- Việc đang làm: **Cổng Đông Lâm = Map 01A**, tutorial chung Lv1–3 sau năm class intro riêng.
- Source hình canonical: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/map-01a-cong-dong-lam/01..10`; catalog và giới hạn dùng tại `docs/art/LGO-SELECTED-2D-SOURCE-CATALOG-v1.md`.
- World flow khóa: Class Intro → Cổng Đông Lâm → Suối Thanh Minh → Rừng Ngoại Vi → Đồi Phong Linh → Miếu Linh Sơn → Sơn Thạch Thủ → Linh Thành. Batch này chỉ Map 01A; Linh Thành chỉ là silhouette xa.
- Không tiếp tục lấy primitive hiện có làm art direction. Giữ state machine, input, anchor, Tilemap/atlas contract và evidence tooling nếu phù hợp; thay nội dung/cảnh theo source canonical.
- Không đưa nguyên design board vào runtime. Tách/redraw/crop thành asset riêng theo layer với provenance, alpha/pivot/anchor/sort và review Player.
- Công sức class cũ không bị bỏ: source pack giữ 12 file Võ WIP gồm base nam/nữ đã căn ground, mask plan, ba slot alpha và atlas/toggle review. Chưa import runtime vì còn lỗi cổ tay/alpha và thiếu slot/motion.

### Batch đa màn hình / tối ưu asset — đã kiểm kỹ thuật

Đã gom importer desktop BC1/BC3 + Android/iOS ASTC 6x6, hai texture dùng chung, terrain lặp sprite, camera, grounding, HUD safe-area, joystick có sẵn và hội thoại Hạ Vân. Input legacy bị suspend khi mở preview; trò chuyện này không chạy quest Shadow Slime/Linh Thành cũ. Pack PNG 6.764.005 byte trong budget 7 MiB; GPU byte trong manifest là **ước tính**, chưa đo thiết bị thật. Build Player mới 161.426.371 byte, trước nén 177.317.779 byte.

Một lượt tích hợp và một lượt sửa lỗi review: EditMode cuối 168 total/167 pass/0 fail/1 skipped; build 0 errors/13 warnings. `tools/capture_lgo_map01a_art.py --profile all` tạo 12 ảnh (arrival/dialogue/gate/village × mobile 1600×720, tablet 1024×768, PC 1280×720), manifest xác nhận grounding/parallax và mở/đóng thoại. Evidence: `build/map01a-art/batch-final-three-profiles/`, log `build/map01a-art/review/batch-final-*`. Đã xem ba ảnh dialogue: marker đúng vị trí, hộp thoại không che NPC. Đây là mô phỏng tỷ lệ macOS, chưa kiểm touch/notch/GPU mobile thật. PC vẫn placeholder; map chưa đạt visual/product gate.

Next batch: mở rộng Map01A theo route/quest source với Quan Thủ và tương tác chung, tiếp tục giữ atlas dùng chung và budget; dùng base/slot WIP đã audit khi cần thay PC placeholder, không phát triển cả năm class. Gom đủ một đoạn gameplay rồi test/build/capture chung, không build sau từng chỉnh nhỏ. Bổ sung các trạng thái cần thiết vào cùng capture; chưa chuyển sang hoàn thiện class trước khi đóng map.

### Actions theo thứ tự

1. **MAP01A-01 — World strip contract:** thay route cũ bằng 10 khu `Spawn/Hạ Vân → Đại Cổng → Quan Thủ → Quảng trường → Tổng Phú → Thanh Nhi → Giếng/Cầu → Lão Trần → Combat edge → Portal Suối Thanh Minh`; khóa 12 parallax/render layer, collision và UI safe area từ source.
2. **MAP01A-02 — Visible art slice:** dựng một màn gameplay 16:9 từ đúng palette/architecture/terrain/vegetation source; loại cảm giác rectangle blockout ở góc spawn–đại cổng; Player và NPC đứng đúng ground.
3. **MAP01A-03 — Lv1–3 playable flow:** Q01–Q09, NPC chính, inventory/potion/gather/loot/equip/chest, bốn quái chỉ ở rìa làng và portal Map 01B. Không boss, không monetization, không skill thứ hai.
4. **MAP01A-04 — Runtime gate:** Unity EditMode, smoke, macOS Player build/capture, matrix, no-3D/no-source-image, frozen audit và review ảnh so với 10 source canonical.
5. **CLASS-VO-01 — sau khi map pass:** tiếp tục WIP Võ, không làm lại; hoàn thiện Võ nam/nữ Lv1–30 với 10 slot, thay đồ, idle/walk/run/jump/basic attack và `Liên Quyền` trong Player.
6. Chỉ sau CLASS-VO-01 mới nhân cùng contract sang Kiếm/Pháp/Cơ/Linh Lv1–30.

MAP01A-02 đã có pack draft tái tạo bằng `tools/pack_lgo_map01a_art.py` (Pillow venv class cũ). Nguồn và SHA ở `build/map01a-art/source/manifest.json`; atlas/nền ở `build/map01a-art/pack/`. Cổng và Hạ Vân có alpha thật. Terrain bị bake checkerboard trong cả hai lần export, nên chỉ dùng vùng đá đặc `(0,330,2172,724)` của bản v1; không coi ảnh đó là alpha sprite. Script kiểm hash nguồn/reference và alpha trước pack.

Đã bổ sung `village-midground-draft-v1.png` (alpha thật, review trên trắng/xanh tối), pack bốn part `gate/ha-van/terrain/village` và năm layer placement có order/parallax. Review mới `build/map01a-art/review/authored-layer-assembly-v2.png` dựng trực tiếp từ manifest: chân cổng đã chạm mặt đường, lớp làng che phần khoảng trống dưới cổng. Đây vẫn là composite, chưa Player evidence. `build/map01a-art/review/pack-qa.txt` xác nhận rect bounded/nonoverlap, alpha RGBA giữ nguyên sau resize, source hash mutation bị từ chối; Python compile PASS.

Renderer `CongDongLamMap01AArtPreview` đã tích hợp Resource riêng qua `--lgo-map01a-art-preview`; atlas bốn part/năm layer, parallax và khôi phục renderer/camera được kiểm bằng EditMode. Lượt đầu phát hiện OnDestroy EditMode chưa restore, đã sửa bằng ExecuteAlways; test cuối 168 total, 167 passed, 0 failed, 1 skipped. Build macOS thành công (0 errors, 13 warnings); ảnh cửa sổ Player thật `build/map01a-art/review/player-arrival-camera.png` xác nhận camera đã hết cắt mái. Log ở `unity-editmode-camera.log`, `player-build-camera.log`, `player-camera.log`. Đây là preview art, **không phải Map01A playable/visual PASS**.

Next action ngay: căn PC foot/contact shadow trên lane (ảnh hiện vẫn PC blockout sát mép), nối marker/HUD và tương tác Map01A đúng Hạ Vân/Quan Thủ/route. Preview hiện ẩn HUD cũ; chưa thay flow state cũ. Không dùng Editor smoke Shadow Slime/Linh Thành để claim Q01–Q09. Bổ sung capture tự động cho preview mới, kiểm movement/parallax trong Player; sau đó tiếp Map01A-03. Class WIP giữ nguyên đến gate map.

## Current Owner Goal Override — 2026-09-10

- Map đầu Đông Môn là vertical-slice grounding/proof surface; không mở map thứ hai.
- Class workflow chỉ xử lý **một class trước: Võ Lv1-30**. Không làm đồng loạt 5 class, không mở nội dung 31+.
- Việc tiếp theo sau checkpoint cell-map: refine alpha matte/pivot/scale/head-arms-legs coverage và pose/frame motion Võ Lv1-30 trong Player; kiểm chứng thay đồ, tách layer, skill cue, contact shadow và route grounding cùng scene.
- Chỉ khi Võ Lv1-30 có runtime evidence đủ rõ mới nhân pattern sang Kiếm/Pháp/Cơ/Linh Lv1-30.
- Không Meshy/3D, không dùng ảnh thiết kế cũ làm runtime asset, không sửa frozen surfaces.

`LGO_VO_LV1_30_SKILL_CUE_ART_READY`: Skill cue Võ Lv1-30 đã có approved runtime art cho trail/edge/impact (`cells=13`); tiếp theo refine alpha/pivot/body-head-arms coverage và frame motion Võ Lv1-30 trước khi nhân class.

`LGO_VO_LV1_30_CELLMAP_COVERAGE_READY`: Cell-map approved runtime art Võ Lv1-30 đã mở rộng từ 8 lên 11 cell, thêm torso underlay và hai mảnh pants để giảm fallback primitive trong Player. Evidence nằm ở checkpoint commit tương ứng; đây vẫn là art/probe giai đoạn đầu, chưa phải production final.

# NEXT ACTION — Linh Giới Online 2D

## Goal kỹ thuật hiện tại — 2026-09-10

### Base-first override mới nhất

Checkpoint mới nhất: manifest v9 dùng base head không bake tóc; evidence `build/vo-ten-slot-matrix-v2/three-profiles/` đạt 66 frame × 3 profile và đã review đủ 20 trạng thái tháo từng slot cho nam Lv1/nữ Lv30. Gate `CLASS_VO_LV1_30_VERTICAL_SLICE_PASS` chỉ xác nhận map/avatar/equip/action local trong Player, chưa phải production-final. Next: tách loadout, rig pose và action orchestration khỏi `CongDongLamMap01AArtPreview` thành character base dùng chung; giữ Map01A làm consumer và chạy lại cùng evidence trước khi mở class thứ hai.

Checkpoint mới hơn: manifest Võ v8 đã thay delta trang phục nhỏ bằng batch 8 sheet rig-compatible, đóng 112 attachment vào 7 atlas/885.234 byte. Player mobile/tablet/PC tại `build/vo-garment-v1/three-profiles/` đã review: Lv30 nữ có đủ silhouette trong run/jump/basic/Liên Quyền. Next duy nhất trước khi đóng `CLASS-VO-01`: thêm capture matrix cởi/mặc đủ 10 slot cho nam/nữ ở Lv1 và Lv30, kiểm layer order/pivot; không tạo thêm artwork nếu matrix không chỉ ra lỗi cụ thể.

Gate: `VO_LV1_30_GARMENT_BATCH_PLAYER_PASS / TEN_SLOT_VISUAL_MATRIX_PENDING`. Sau gate này mới tách orchestration/controller base khỏi Map01A và chỉ sau đó mới mở class thứ hai.

Đã bỏ hướng mỗi slot tự dịch/xoay. Võ hiện là consumer đầu tiên của `TwoDSkeletalPaperDollRig`: skeleton cha-con, local pose và 96 attachment metadata dùng chung. Next không được copy `CongDongLamMap01AArtPreview` sang class khác. Việc tiếp theo của `CLASS-VO-01` là tạo/chuẩn hóa garment art tương thích chính skeleton này, kiểm full outfit và equip/unequip từng slot qua idle/run/jump/basic/`Liên Quyền`; chỉ khi visual đạt mới tách controller orchestration ra khỏi Map01A và nhân manifest sang class thứ hai.

Gate hình ảnh hiện tại: `VO_BASE_FIRST_RIG_TECHNICAL_PASS / GARMENT_ART_VISUAL_FIX_REQUIRED`. Evidence mới nhất `build/vo-base-first-v3/three-profiles/`; không dùng capture split-only hoặc independent-pivot trước đó để claim. Quy tắc/lỗi đã ghi tại `LGO-2D-PRODUCTION-WORKFLOW-RESEARCH-v0.1.md`.

Owner bổ sung: mỗi class chỉ xử lý dải **Lv1-30** theo giai đoạn phát triển; không mở skill/trang bị high-tier 31-100 trong batch class hiện tại.

Owner đã giao Codex tự quyết định hướng kỹ thuật cho task này. Quyết định hiện tại: **đóng map đầu Đông Môn thành vertical slice 2D có thể kiểm chứng trong Player trước, rồi mới chuyển sang hoàn thiện 5 class**. Các checkpoint Linh Thành/Quảng Trường/district bên dưới là lịch sử runtime preview local-only; không dùng chúng làm lý do mở map thứ hai hoặc tiếp hub trước khi Đông Môn đạt gate.

Hướng kỹ thuật map đầu:

1. Đông Môn dùng authored Tilemap/Grid + Sprite Atlas/prop atlas/parallax layer, đọc từ Resource JSON khi còn ở giai đoạn draft. Không tiếp tục polish bằng rectangle primitive vô hạn.
2. Scene phải phục vụ flow kiểm chứng thật: spawn -> Người Giữ Cổng/thoại -> Bia Luyện Khí -> jump/dash/class skill -> Shadow Slime -> quay lại/hoàn tất. Mỗi trạng thái có capture Player và manifest, không claim từ Editor-only.
3. Art runtime phải là asset mới, có provenance/hash/allowlist rõ ràng. Không dùng ảnh thiết kế cũ, không crop board/reference, không Meshy/3D.
4. NPC/props/terrain đi theo data-driven placement, atlas rect, anchor/pivot và layer order để sau này thay asset production mà không đổi state machine.
5. Gate map đầu là owner nhìn được trong Player: hình có chiều sâu, silhouette đọc được, không chồng HUD/label/player, collision/route khớp hình, thao tác E/I/Tab/T/Y và movement không bị lệch.

Hướng kỹ thuật sau gate map đầu:

1. Tiếp nhận luồng 5 class từ tab riêng bằng commit/asset đã rõ ownership; không ghi đè worktree hoặc file đang dở của tab đó.
2. Chuẩn hóa base chung nam/nữ, item slot alpha thật, anchor/pivot, sorting layer, compatibility rules và snapshot trang bị.
3. Mỗi class Võ/Kiếm/Pháp/Cơ/Linh phải kiểm được trong Player: chọn class, mặc/tháo từng slot, phối bộ tương thích, idle/walk/run/jump và đòn/skill preview trong scope hiện có.
4. Gate 5 class là nhìn thấy đồ/vũ khí đi theo thân khi đổi hướng/chuyển động, không hở cổ tay/chân/thân, không sai layer, không dùng ảnh source/reference làm runtime asset.

Next action ngay: tiếp tục **Võ Lv1-30** làm class mẫu theo function probe vừa thêm. Probe đã chứng minh trong Player manifest: paper-doll slots, try-on/apply, motion states, skill target và grounding Đông Môn đều đi qua cùng một contract. Bước kế tiếp là refine runtime art cell-map thật: làm sạch alpha matte, tách thêm cells đầu/tay/chân/boots/waist/weapon từ `VoLv1ApprovedRuntimeArt`, chỉnh world scale/pivot/sort để nhìn rõ trong Player ở các ảnh 07/08/09/10, rồi mới nhân pipeline sang Kiếm/Pháp/Cơ/Linh Lv1-30. Map đầu vẫn cần art/parallax/prop pass riêng để tiến gần reference; không dùng checkpoint grounding/probe này để claim map hoặc class đã production-final.

## Đông Môn illustrated draft — 2026-09-10

Đã tạo art mới và pack skyline + atlas cổng/NPC/terrain; preview opt-in trong Player qua `--lgo-dongmon-art-preview`, không sửa controller/state/5 class hoặc frozen surfaces. `tools/capture_lgo_dongmon_art.py` capture 5 trạng thái vào thư mục riêng. EditMode 139 pass, 0 fail, 1 skipped; guard 5 test pass; smoke/build/baseline 20 frame và art 5 frame đã chạy, ảnh đã review. Art vẫn DRAFT, player còn placeholder; không claim giống hoàn toàn ảnh owner.

Next/gate: owner xem capture `build/dongmon-art/player-final/03-dialogue.png` (bản copy bền ở `build/dongmon-art-checkpoint/`) và duyệt bố cục/palette/tỷ lệ trước khi nhân rộng. Sau duyệt chỉ hoàn thiện map đầu Đông Môn; xong gate map mới chuyển sang tích hợp và hoàn thiện 5 class từ tab riêng. Hướng/plan/evidence: `docs/design/dong-mon-illustrated/DESIGN.md`. Không tiếp polish primitive; không ghi đè checkout chính hoặc source tab 5 class. Không có blocker runtime; gate còn lại là duyệt mỹ thuật.


## Gate Keeper silhouette — 2026-09-10

Đã kiểm Người Giữ Cổng 13 part đọc từ JSON với `rect/ellipse/diamond/tapered`; shape lạ fallback rectangle, cache dùng lại texture, snapshot đọc style từ source. Capture tiếp cận NPC ở khoảng cách 0.85 trong FocusRange 1.15 để không chồng silhouette. Không đổi gameplay 5 class hoặc frozen surfaces.

Evidence trong `build/gatekeeper-polish-checkpoint/`: EditMode 138 pass, 0 fail, 1 skipped (pointer capture cần UI panel); onboarding smoke Complete; macOS build Succeeded (0 errors, 13 warning API UI cũ); Player capture 20 BMP; smoke/visual matrix, no-3D/no-source-image và frozen diff audit pass. Đã xem ảnh 01/02/03/20: mũ, áo thuôn, bóng ellipse, gậy/ngọc đọc được; player đứng riêng khi thoại. Đây chỉ là checkpoint silhouette blockout, chưa đạt chuẩn illustrated owner gửi.

macOS: sau launch từ Contents/MacOS, Player có thể chờ foreground do runInBackground=0; dùng `open <đúng app vừa build>` để kích hoạt app đang chạy, không mở ảnh giữa capture. Worktree cũ được giữ vì có thay đổi đồng thời ngoài lượt này; chỉ dọn worktree riêng của batch. Tab 5 class giữ ownership source của họ.

Next: design/demo draft góc Đông Môn bằng art mới theo phần hiệu chỉnh owner trong workflow research; capture Player và duyệt mỹ thuật trước khi nhân rộng. Không tiếp tục polish primitive vô hạn.


## Ưu tiên owner mới — 2026-09-09: chuẩn hóa art Võ

Scope hiện tại chỉ Võ, docs/reference/checker; không tiếp gameplay hoặc triển khai class khác từ batch này. Đã review 23 PNG và chọn nguồn trong `docs/art/classes/vo/LGO-VO-2D-MODULE-SPEC-v1.0.md`; handoff: `HANDOFF-LGO-CLASS-2D-MODULE-STANDARD-v1.0.md`. Các mục runtime phía dưới là trạng thái trước yêu cầu này, không phải quyền mở rộng batch art.

Việc tiếp theo: tạo pack clean transparent cho Võ `lv001` + `lv050`: dùng base candidate nam/nữ đã có trong `build/class-2d-review/generated-drafts/`, sinh từng item rời thay vì sheet checkerboard, lắp đủ/tháo từng slot trên base chung, phối chéo level, motion sheet idle/walk/run/jump/basic_attack/skill_preview và mockup rương/paper doll để thử đồ. Draft cross-level đã chứng minh hướng phối `lv001`/`lv050` trên cùng base; hai base candidate có alpha thật và white-check, nhưng sheet 10 slot `lv001` bị bake checkerboard nên chưa đạt transparent item gate. Gate asset hiện thiếu: `python3.12 tools/validate_class_2d_module_spec.py --require-assets` báo thiếu 220 item + 6 board. Sau khi Võ đạt gate này, owner yêu cầu xử lý tiếp class Cơ và Linh theo folder tương tự `/Users/minhdc/Projects/2D/Vo`. Evidence: `build/class-2d-review/validation.log`; lỗi trực quan từng nguồn và SHA-256 lưu trong spec Võ.

Ingest vào Unity còn bị gate `tools/validate_2d_branch_no_source_images.py` chặn ảnh source; cần task asset/ingest riêng giải quyết gate, không tự tắt validator, không import/crop board. Bản chọn vẫn REFERENCE_ONLY; chưa production/runtime art.

## Trạng thái

Branch hiện tại: `feature/2d`. Owner đã khóa hướng mới qua `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md`: **2D Side-Scrolling Social Action MMORPG**, HD 2D anime / illustrated character, không pixel-art, không Meshy/3D, side-view parallax, social hub + action combat. Kịch bản mới đã được đưa vào `docs/02-GDD.md` để làm nguồn chính trước khi design map: giữ thế giới/class/progression/story/backend, chuyển pipeline sang base sprite/cutout body, layer trang phục 2D, skeleton 2D, anchor point, sprite atlas và runtime test. Cleanup 3D đã commit/push ở `d97a3c8`, cleanup trace dịch vụ 3D ở `6bf905e`, xoá ảnh source cũ ở `646492e`, Đông Môn procedural blockout ở `eeaf19d`, direction/map catalog ở `1fa3231`. Tutorial Jump/Dash/Skill đã checkpoint ở `e330c58`; Shadow Slime combat micro-slice ở `ad9cd52`; map layer budget ở `4bb77f0`; route progress/minimap ở `986b0e5`; 2D-04 Kiếm Lv1 module catalog, Đông Môn landmark/parallax và inventory try-on strip đã checkpoint; batch hiện tại sẵn sàng chuyển sang tileset/terrain collision hoặc panel inventory input thật. Roadmap khóa: 2D-00 Direction Lock → 2D-01 Male/Female Base Character → 2D-02 Modular Runtime → 2D-03 Võ Lv1 → 2D-04 Kiếm Lv1 → 2D-05 Pháp Lv1 → 2D-06 Cơ Lv1 → 2D-07 Linh Lv1 → 2D-08 Animation Foundation → 2D-09 Linh Thành Map → 2D-10 Gate Keeper Tutorial → 2D-11 Shadow Slime Combat → 2D-12 Vertical Slice, trong Zone Network thay vì open-world liên tục.

## Gate hiện tại

- `python3.12 tools/validate_2d_branch_no_3d.py` phải pass trước khi tiếp tục gameplay 2D.
- `python3.12 tools/validate_2d_branch_no_source_images.py` phải pass để bảo đảm ảnh thiết kế/source cũ đã bị loại khỏi source tree.
- Unity batch compile/test phải chạy thật sau các thay đổi source.
- Player smoke phải chứng minh flow hoàn tất trong Player, không chỉ Editor.
- Runtime visual capture phải có manifest và ảnh đã xem bằng mắt nếu có thay đổi player-visible.
- Không sửa frozen surfaces nếu chưa có owner decision riêng.

## Việc tiếp theo

Chỉ đạo owner mới nhất 2026-09-10: **map đầu trước → hoàn thiện 5 class sau**, ưu tiên chức năng owner tự kiểm chứng trong game. Không mở map thứ hai.

Cập nhật Goal kỹ thuật: map Đông Môn đã là mặt phẳng kiểm chứng anchor/grounding; batch hiện tại tiếp tục Võ Lv1 trước để chứng minh paper-doll slot, tách đồ, pose/frame motion và skill có thể chạy trong Player thật trên cùng scene. Chưa nhân sang 5 class khi Võ chưa có runtime evidence đủ rõ.

Batch tiếp theo của Võ Lv1 đang khóa theo hướng slot compatibility trước: mọi item runtime phải báo selected slot/anchor/fit/profile/provenance trong manifest để owner kiểm thay đồ không lệch trước khi nâng art atlas/spritesheet.

Batch Võ Lv1 đã có anchor/pivot gizmo, runtime fit contract và production atlas contract trong Player: owner có thể kiểm Chest/Hips/Hand/Foot, slot bounds/sort, skill Hand_R, required atlas cells, rig joints và replacement gates trên mặt phẳng Đông Môn. Bước kế tiếp là tạo/nhập spritesheet hoặc layered PSB sạch cho Võ Lv1 theo contract này, rồi mới nhân pattern sang Kiếm/Pháp/Cơ/Linh.

1. Owner xem góc Đông Môn illustrated trong capture Player `03-dialogue.png`; bản này mới là draft góc cổng, chưa coi cả map đầu hoàn thiện.
2. Hoàn thiện Đông Môn theo kịch bản đã có: cảnh/parallax/terrain, đường đi và tương tác không lệch hình; kiểm spawn → Người Giữ Cổng/thoại → Bia Luyện Khí → hướng dẫn hiện có. Bàn giao Player chạy được, phím điều khiển và bằng chứng trước/sau từng thao tác. Không mở hệ thống mới hoặc map tiếp theo.
3. Sau gate map đầu, tiếp nhận commit/asset từ tab 5 class rồi triển khai theo base/slot chung: Võ, Kiếm, Pháp, Cơ, Linh; không ghi đè worktree hoặc source chưa commit của tab đó. Mỗi class có nguồn item rời alpha thật, anchor/pivot, thứ tự layer và quy tắc tương thích rõ ràng.
4. Gate kiểm chứng 5 class trong Player: chọn class → mặc/tháo từng slot → phối các bộ tương thích → idle/walk/run/jump và đòn/skill preview trong scope đã có. Kiểm đồ đi theo chuyển động, không lệch tay/chân/vũ khí, không hở/cắt thân hoặc sai layer khi đổi hướng; snapshot trang bị phải khớp phần đang thấy. Đây là task tiếp theo, chưa phải chức năng đã hoàn thiện.
5. Bàn giao một bản game với hướng dẫn thao tác và expected result ngắn cho từng chức năng; test/capture thực, owner nhìn và thao tác được. Không dùng ảnh concept hoặc test xanh thay bằng chứng gameplay.

## Blocker

Chưa có blocker. Visual capture hiện bắt được world stage, HUD world-space, minimap/route overlay, scene-beat metadata và character base snapshot bằng camera render; manifest lưu `hudSnapshot`, `productionSceneBeatSnapshot`, `runtimeMapSnapshot`, `runtimeCharacterBaseSnapshot`, `runtimeEquipmentSnapshot`, `runtimeAnimationSnapshot` để kiểm copy/trạng thái. Ảnh source cũ đã bị loại khỏi source tree và có validator riêng để ngăn tái nhập nhầm. Shadow Slime micro-slice, map layer budget, route progress, Kiếm Lv1 module runtime, Đông Môn landmark pass và inventory try-on/input runtime hiện được kiểm bằng catalog check, controller snapshot, smoke runner và visual manifest; chưa có blocker.

## Map runtime checkpoint — 2026-09-09

Song song với luồng class Võ, map Đông Môn vừa có tilemap runtime spine riêng: `DongMonTileDefinitions`, runtime `RuntimeTilemapSnapshot`, manifest `runtimeTilemapSnapshot`, Player capture 10 frame đã review. Next map-safe action: nâng procedural tile strip thành authored tilemap chunks/parallax spacing cho Đông Môn, nhưng chỉ làm khi không đụng scope class/art đang xử lý ở tab song song.

## Map chunk checkpoint — 2026-09-09

Đông Môn đã có `ChunkFlow` runtime: `chunk_gate_entry -> chunk_training_stone -> chunk_jump_bridge -> chunk_dash_lane -> chunk_slime_arena`, visual capture PASS. Next map-safe action: parallax spacing/foreground polish hoặc authored Unity Tilemap asset khi asset sạch sẵn sàng; tránh chạm luồng class/art ở tab song song.

## Runtime smoke matrix checkpoint — 2026-09-09

`LGO-TASK-047` đã có closure `LGO_RUNTIME_SMOKE_MATRIX_READY`: `python3.12 tools/lgo_runtime_smoke_matrix.py --phase two-d` kiểm onboarding smoke JSON, macOS Player build log và visual manifest hiện tại, gồm `runtimeTilemapSnapshot` + `ChunkFlow`. Next safe action: tiếp roadmap map/runtime hoặc task advisor kế tiếp, vẫn tránh chạm luồng class/art ở tab song song.

## Visual evidence matrix checkpoint — 2026-09-09

`LGO-TASK-048` đã có closure `LGO_VISUAL_EVIDENCE_MATRIX_READY`: `python3.12 tools/lgo_visual_evidence_matrix.py --verify-current` kiểm 5 frame visual 2D hiện tại và manifest fields, marker `LGO_VISUAL_EVIDENCE_MATRIX_2D_CURRENT_PASS`. Next safe action: tiếp task advisor kế tiếp hoặc map/runtime nhỏ, không claim production art từ evidence này.

## Crash/error reporting checkpoint — 2026-09-09

`LGO-TASK-049` đã có closure `LGO_CRASH_REPORTING_PLAN_READY`: local plan/validator PASS, không thêm production crash service hoặc telemetry backend. Next safe action: task advisor kế tiếp trong docs/tools hoặc map/runtime nhỏ khi không đụng luồng class/art.

## Release checklist checkpoint — 2026-09-09

`LGO-TASK-050` đã có closure `LGO_RELEASE_CHECKLIST_READY`: release checklist validator PASS; trạng thái vẫn pre-alpha, không mở implementation hoặc production release claim. Next safe action: kiểm advisor mới, ưu tiên task không xung đột tab class hoặc map/runtime 2D nhỏ có evidence thật.

## Đông Môn parallax/foreground checkpoint — 2026-09-09

`LGO_DONGMON_PARALLAX_POLISH_READY`: runtime Đông Môn đã có `ParallaxDepthSnapshot` và visual capture mới PASS 10 frame với cloud-drift, mountain-silhouette, mist/foreground grass thấp; ảnh đã review không che HUD, combat, minimap hoặc inventory. Build app thực tế khoảng 109MB, nhưng Unity Player build log có bước `prepared ~36238M`, nên next safe action ưu tiên kiểm/tối ưu nguyên nhân build prepared-size hoặc tiếp map/runtime nhỏ không đụng luồng class/art đang mở ở tab song song.

## World/Linh Thành zone-network checkpoint — 2026-09-09

`LGO_WORLD_ZONE_NETWORK_RUNTIME_READY`: runtime catalog và minimap overlay đã có World Map/Linh Thành network (`WorldMapNetwork: hub=linh-thanh`, `LinhThanhHubRuntime`), Player capture mới PASS 10 frame, smoke matrix 2D đã bắt token này. Map tổng thể vẫn chưa production-complete; next safe map action là Linh Thành hub shell/Quảng Trường hoặc authored Đông Môn Tilemap khi có asset sạch, tránh đụng luồng class/art tab song song.

## Linh Thành hub shell checkpoint — 2026-09-09

`LGO_LINHTHANH_HUB_SHELL_RUNTIME_READY`: runtime catalog/scene đã có `HubShell: linh-thanh` với Đông Môn, Quảng Trường, Học Viện và Thương Phố; Player capture mới PASS 10 frame và smoke matrix 2D đã bắt token HubShell. Map tổng thể vẫn chưa production-complete; next safe map action là mở Quảng Trường hub shell chi tiết hơn hoặc authored Đông Môn Tilemap khi asset sạch, tránh đụng luồng class/art tab song song.

## Quảng Trường plaza shell checkpoint — 2026-09-09

`LGO_LINHTHANH_PLAZA_SHELL_RUNTIME_READY`: runtime catalog/scene đã có `PlazaShell: district=plaza`, social-spawn/event-board/guild-bulletin preview và guard `safe-no-trade-backend`; Player capture mới PASS 10 frame và smoke matrix 2D đã bắt PlazaShell. Map tổng thể vẫn chưa production-complete; next safe map action là hub runtime riêng cho Quảng Trường hoặc authored Đông Môn Tilemap khi asset sạch.

## Linh Thành unlock presentation checkpoint — 2026-09-09

`LGO_LINHTHANH_UNLOCK_PRESENTATION_READY`: sau khi người chơi hoàn tất Đông Môn/Shadow Slime, runtime state bật `LinhThanhUnlocked=true`, HUD đổi mục tiêu sang `Mở Linh Thành: Quảng Trường`, scene bật banner/path local preview và visual manifest ghi `runtimeLinhThanhUnlockSnapshot` với `unlock=plaza`, `safe-local-no-teleport`. Đây là unlock presentation local-only: chưa mở teleport, shop, giao dịch, bang hội hoặc backend xã hội. Map tổng thể vẫn chưa production-complete; next safe map action là làm Quảng Trường hub runtime riêng với NPC/board local-only hoặc chuyển Đông Môn procedural chunk sang authored Tilemap khi asset sạch sẵn sàng.

## Quảng Trường hub runtime preview checkpoint — 2026-09-09

`LGO_LINHTHANH_PLAZA_HUB_RUNTIME_READY`: Quảng Trường đã có runtime preview local-only sau unlock Đông Môn: `RuntimeLinhThanhPlazaHubSnapshot`/visual manifest chứa `PlazaHubRuntime`, `npc=gate-guide`, `board=event-local-preview`, `guild-bulletin=locked`, `safe-local-no-backend`; scene bật NPC/board/label sau unlock và không hiện sớm ở frame initial. Next map-safe action: thêm interaction local-only cho bảng sự kiện/NPC Quảng Trường hoặc authored Đông Môn Tilemap khi asset sạch sẵn sàng.
## Next after Quảng Trường board interaction — 2026-09-10

Map/runtime checkpoint mới đã có board interaction local-only cho Quảng Trường sau unlock Đông Môn, visual frame `11-plaza-board-preview`, inventory panel ẩn mặc định và Unity EditMode wrapper đã được sửa để không pass giả khi thiếu XML. Next map-safe action: thêm NPC local interaction cho Người Giữ Cổng/Thương Nhân ở Quảng Trường hoặc authored Đông Môn Tilemap asset khi có asset sạch; vẫn tránh đụng luồng class/art tab song song và không mở teleport/shop/giao dịch/bang hội/backend nếu chưa có gate riêng.
## Next after Quảng Trường NPC interaction — 2026-09-10

`LGO_LINHTHANH_PLAZA_NPC_INTERACTION_READY`: Quảng Trường đã có NPC interaction local-only cho Người Giữ Cổng và Thương Nhân preview sau unlock Đông Môn, visual frame `12-plaza-npc-preview`, dialogue speaker đúng NPC và manifest bắt `safe-local-no-shop-backend`. Next map-safe action: chuyển Đông Môn procedural chunk sang authored Tilemap asset sạch hoặc thêm Plaza NPC selector/input thật; chưa mở shop/economy, teleport, giao dịch, bang hội hoặc backend nếu chưa có gate riêng.

## Quảng Trường target selector checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_TARGET_SELECTOR_READY`: Quảng Trường sau unlock Đông Môn có selector runtime local-only: `P` đổi mục tiêu giữa Bảng Sự Kiện, Người Giữ Cổng, Thương Nhân; `E/Enter` tương tác mục tiêu đang chọn. Evidence cần giữ khi resume: `runtimePlazaHubInputSnapshot`, frame `12-plaza-target-selector`, frame `13-plaza-npc-preview`, smoke/matrix PASS.

## Next map-safe action

1. Nâng Quảng Trường social layout để ba mục tiêu hub có spacing/độ đọc tốt hơn và chuẩn bị route vào Học Viện/Thương Phố/Bang Hội ở mức shell local-only; hoặc
2. Chuyển Đông Môn procedural chunks sang authored Tilemap/tileset sạch nếu asset/source 2D đã sẵn; hoặc
3. Thêm transition shell `Đông Môn -> Linh Thành/Quảng Trường` không mở teleport backend, chỉ preview route local.

Không mở shop/economy, teleport thật, giao dịch, bang hội, HP/loot/server-authoritative combat hoặc frozen surfaces trước gate riêng.

## Quảng Trường social layout checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_SOCIAL_LAYOUT_READY`: selector Quảng Trường đã có layout token `spaced-social-triangle`, spacing ba target hub rõ hơn trong runtime capture. Debt còn lại: blockout procedural vẫn có text nhỏ/chồng nhẹ ở marker xa, nên batch map tiếp theo nên ưu tiên authored Tilemap/label layout hoặc transition shell thay vì mở chức năng xã hội thật.

## Hub transition checkpoint — 2026-09-10

`LGO_LINHTHANH_HUB_TRANSITION_PREVIEW_READY`: sau khi hoàn tất Đông Môn, runtime có preview tuyến `Đông Môn -> Quảng Trường` ở mức local-only, evidence `runtimeHubTransitionSnapshot` và frame `14-plaza-transition-preview`. Next map-safe action: authored Đông Môn Tilemap/tileset sạch, hoặc Quảng Trường label/readability pass; không mở teleport/backend thật.

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
## Quảng Trường layout anchor checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_LAYOUT_ANCHORS_READY`: Quảng Trường sau unlock Đông Môn có runtime `PlazaHubLayout` gồm 5 anchor social-spawn/event-board/gate-guide/merchant-preview/guild-locked, được scene render local-only và visual manifest/matrix bắt token. Next map-safe action: mở inspect detail cho từng anchor bằng UI local-only hoặc tiếp tục tile/atlas production khi có asset sạch; không mở event/shop/guild/backend.
## Quảng Trường anchor detail checkpoint — 2026-09-10

`LGO_LINHTHANH_PLAZA_ANCHOR_DETAIL_READY`: Quảng Trường sau unlock có `runtimePlazaHubDetailSnapshot` và dòng detail world-space cho target đang chọn, gồm merchant preview `try-before-shop` local-only. Next map-safe action: polish readability/spacing Plaza hoặc chuyển asset sạch sang atlas/tile khi có gate ingest riêng; không mở shop/event/guild/backend.
## Checkpoint — LGO_LINHTHANH_DISTRICT_PREVIEW_READY — 2026-09-10

Đã thêm preview rail chọn khu Linh Thành sau unlock Đông Môn: Học Viện là target đầu, route `plaza->academy`, control `M select-district`, frame visual `15-district-preview-rail`. Next map-safe action: polish readability/density của Linh Thành shell hoặc mở district preview chi tiết tiếp theo local-only; không mở teleport/shop/skill/crafting/guild/travel backend nếu chưa có gate riêng.
## Checkpoint — LGO_LINHTHANH_DISTRICT_DETAIL_READY — 2026-09-10

Đã thêm district detail snapshot và visual frame `16-district-market-preview`: rail Linh Thành cycle từ Học Viện sang Thương Phố, giữ local-only và không mở shop/trade/economy. Next map-safe action: polish density/visual label cho hub shell hoặc thêm detail preview cho Đền Linh/Khu Rèn/Guild/Harbor theo cùng guard, vẫn không mở backend.
## Đền Linh district preview checkpoint — 2026-09-10

`LGO_LINHTHANH_SPIRIT_TEMPLE_PREVIEW_READY`: Linh Thành district preview rail đã có frame Player riêng cho Đền Linh sau cycle Học Viện → Thương Phố → Đền Linh. Runtime manifest giữ `selected=spirit-temple`, `label=Đền Linh`, `route=plaza->spirit-temple`, detail `altar-local-only`, next gate `quest-buff-gate`, guard `safe-no-buff-backend`/`safe-no-district-backend`. Evidence: Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 17 frame, visual matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là polish visual/readability Linh Thành hoặc chuyển thêm Đông Môn procedural strip sang Tilemap/atlas sạch.

## Linh Thành district rail coverage checkpoint — 2026-09-10

`LGO_LINHTHANH_DISTRICT_RAIL_COVERAGE_READY`: district preview rail Linh Thành đã có Player visual frames cho Khu Rèn, Khu Bang Hội và Cảng Linh Thuyền sau các frame Học Viện/Thương Phố/Đền Linh. Manifest cuối ở `selected=harbor`, route `plaza->harbor`, detail `spirit-boat-locked`, next `world-route-gate`, guard `safe-no-travel-backend`/`safe-no-teleport-backend`/`safe-no-district-backend`. Evidence: Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 20 frame, visual matrix PASS. Map tổng thể vẫn chưa production-complete; next safe map action là polish visual density/layout Linh Thành hoặc nâng Đông Môn Tilemap/atlas sạch.

## Linh Thành district rail readability checkpoint — 2026-09-10

`LGO_LINHTHANH_DISTRICT_RAIL_READABILITY_READY`: district preview rail đã polish callout để label/backplate đi theo node đang chọn, tăng size nhẹ và expose `runtimeLinhThanhDistrictReadabilitySnapshot` với `label-follows-selected=True`, `backplate=follows-selected`, `callout-size=readable`, `avoids-hud-overlap`. Evidence: Unity EditMode PASS, Editor smoke PASS, macOS Player build PASS, Player visual capture PASS 20 frame và visual review frame Khu Rèn/Bang Hội/Cảng. Map tổng thể vẫn chưa production-complete; next safe map action là polish minimap/district rail density hoặc nâng Đông Môn Tilemap/atlas sạch.

## Next after minimap compact readability — 2026-09-10

`LGO_MINIMAP_COMPACT_READABILITY_READY`: world/minimap overlay đã rút route text, dùng district chips ngắn `HV TP ĐL KR BH CẢ`, world links dạng ngắn và manifest bắt `runtimeMinimapReadabilitySnapshot`. Map chưa xong production A-Z; nền tảng hiện có world map/Linh Thành/Đông Môn/district preview chạy trong Player. Next map-safe action: nâng Đông Môn authored Tilemap/atlas sạch hoặc mở layout chi tiết tiếp theo cho một khu Linh Thành theo local-only gate; không mở teleport/travel/shop/guild/backend.

## Next after Đông Môn authored detail resource — 2026-09-10

`LGO_DONG_MON_AUTHORED_DETAILS_RESOURCE_READY`: Đông Môn đã có resource detail pass riêng `DongMonAuthoredDetails.json` cho moss/step/rope/spirit-dust/rune, runtime đọc resource và manifest/matrix bắt snapshot. Next map-safe action: chuyển các props/NPC blockout tiếp theo sang atlas/sprite pipeline sạch hoặc nâng một khu Linh Thành detail layout bằng resource tương tự; không quay lại hardcode/dán tay, không mở backend.

## Next after Gate Keeper NPC sprite source — 2026-09-10

`LGO_DONG_MON_GATEKEEPER_NPC_SPRITE_SOURCE_READY`: Người Giữ Cổng đã có resource sprite-part riêng `DongMonNpcSprites.json` và runtime render từ slot/anchor. Next map-safe action: áp cùng pattern cho Training Stone/Shadow Slime/NPC hub hoặc thay slot bằng atlas sprite thật khi art sạch sẵn sàng; không mở backend/social/shop.
- LGO_2D_MACOS_PLAYER_CWD_CAPTURE_LESSON: khi chạy macOS Player evidence cho 2D onboarding, dùng `tools/capture_lgo_2d_onboarding_visual.py` hoặc launch executable từ `Contents/MacOS`; chạy trực tiếp từ repo root có thể init engine rồi thoát trước khi `GameBootstrap` ghi manifest/screenshot.
- LGO_2D_PRODUCTION_WORKFLOW_RESEARCH_READY: đã ghi `docs/execution/LGO-2D-PRODUCTION-WORKFLOW-RESEARCH-v0.1.md`; batch sau ưu tiên pipeline Tilemap/Sprite Atlas/paper-doll, dùng `tools/capture_lgo_2d_onboarding_visual.py`, không polish bằng rectangle primitive kéo dài.
## Đông Môn interaction marker + PC grounding checkpoint — 2026-09-10

`LGO_DONG_MON_PC_GROUNDING_MARKERS_READY`: Đông Môn map đầu có thêm runtime resource `DongMonInteractionMarkers.json` cho 5 mốc route `gatekeeper -> training-stone -> jump -> dash -> shadow-slime`, render marker/label trực tiếp trong Player để kiểm điểm tương tác. Runtime manifest/matrix bắt `runtimeDongMonInteractionMarkerSourceSnapshot` và `runtimeDongMonPlayerSceneFitSnapshot`; PC được kiểm theo `footAnchor=bottom-center`, contact shadow, ground band, sort order và route anchors để batch class tiếp theo ghép nhân vật/đồ/motion trên cùng hệ tọa độ, không chỉnh tay theo ảnh chụp. Evidence: Unity EditMode `total=143 passed=142 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS. Visual vẫn là blockout kỹ thuật, chưa phải art giống concept; next action là làm một class đầu tiên có base/paper-doll/tách đồ/thay đồ/motion/skill trong Player thật trước khi nhân rộng sang 5 class.
## Võ Lv1 one-class runtime slice checkpoint — 2026-09-10

`LGO_VO_LV1_ONE_CLASS_RUNTIME_SLICE_READY`: Class đầu tiên để kiểm chứng workflow là Võ Lv1, chưa nhân rộng sang 5 class. Thêm runtime resource `LGOClasses/VoLv1ClassSlice.json` mô tả base nam/nữ, 5 paper-doll slots, anchor/fit profile, motion Idle/Walk/Jump/Dash/ClassSkill và skill `vo_lv1_first_skill` đánh `shadow-slime`. Controller expose `runtimeVoLv1ClassSliceSnapshot` trong visual manifest tại frame `08-complete` trước khi thử đồ inventory, nên snapshot giữ loadout Võ `top_vo_lv1_male`, motion `TrainingCompletePose` và `runtimeSkillCue=True`; inventory applied vẫn là snapshot riêng để kiểm thử thay đồ. Runtime scene có cue `VÕ Q`/trail nhỏ để owner thấy skill class đầu tiên đã đi qua Player thật, nhưng đây vẫn là cue blockout, chưa phải VFX production. Evidence: Unity EditMode `total=145 passed=144 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS. Next action: thay primitive cue/slot màu bằng sprite atlas/paper-doll asset sạch cho Võ trước, rồi mới nhân pattern sang Kiếm/Pháp/Cơ/Linh.

`LGO_VO_LV1_PAPERDOLL_MOTION_POSE_READY`: Võ Lv1 paper-doll atlas đã có `poseOffsets` data-driven cho `vo_idle`, `vo_jump_lift`, `vo_dash_stretch`, `vo_skill_cast`, `vo_lv1_training_complete`; controller áp pose theo animation intent và expose `currentPose` trong `runtimeVoLv1PaperDollAtlasSnapshot`. Overlay Võ hiện từ lesson Bia Luyện Khí để Player capture kiểm được đồ đi theo Jump/Dash/Skill/Complete, skill cue chỉ bật lúc cast/complete. Đây vẫn là primitive/blockout atlas để kiểm chứng anchor/pivot/motion và thay đồ runtime, chưa phải art production giống concept. Evidence: Unity EditMode `total=149 passed=148 failed=0 skipped=1`, onboarding smoke PASS, macOS Player build Succeeded, Player visual capture PASS 20 frame, runtime smoke matrix PASS, visual evidence matrix PASS, no-3D/no-source-image PASS, frozen diff audit PASS. Next action sau checkpoint: thay primitive atlas Võ bằng bitmap/spritesheet approved hoặc nâng locomotion frame sampling trước khi nhân pattern sang Kiếm/Pháp/Cơ/Linh.
> **Checkpoint CLASS-VO-01A — motion Võ Lv1 hai giới — 2026-09-10:** nguồn mới được tạo theo đúng identity Võ đã audit, tách thành hai sheet 4×2, gate tự động loại sheet chạm ranh giới ô và chuẩn hóa alpha/margin theo cả batch. Runtime hiện có 14 frame cho mỗi giới: idle/walk/dash/punch legacy cùng run/jump/basic attack/`Liên Quyền`; HUD có nút touch và phím Shift/J/Z/X. Sáu atlas 1024² tổng 700.563 byte, tăng khoảng 57 KB dù thêm 16 frame. EditMode `180/179/0/1`; build macOS 166.998.867 byte; capture `build/vo-motion-v8/three-profiles/` có 38 frame/profile và đã review, không chồng action bar hoặc che portal.
>
> **Next:** tiếp tục `CLASS-VO-01B` trong cùng worktree: làm attachment/rig cho 10 slot để trang bị tùy chọn đi đúng pose khi run/jump/basic/`Liên Quyền`, ưu tiên Lv1 làm proof rồi áp metadata chung cho Lv10/20/30. Không mở class thứ hai. Trạng thái hiện tại là `VO_LV1_FULL_FRAME_MOTION_PASS / ANIMATED_PAPER_DOLL_ATTACHMENT_INCOMPLETE`; full-frame đã chạy thật nhưng modular/tier cao vẫn chỉ dùng root pose.

> **Checkpoint CLASS-VO-01B1 — base skeletal rig — 2026-09-10:** hai sheet rig nam/nữ 5×2 được tạo từ identity Võ đã tuyển, tách theo connected component thành 20 segment head/torso/arms/legs, đóng thêm một atlas 1024² 24.232 byte. Manifest v6 có 120 rig-pose profile và 120 equipment-attachment profile dùng chung Lv1/10/20/30. Runtime modular thay base phẳng bằng 10 segment, xoay quanh joint pivot cho idle/walk/run/jump/basic/`Liên Quyền`; capture hiện tại `build/vo-rig-v2/three-profiles/` có 46 ảnh/profile.
>
> **Next:** `CLASS-VO-01B2` tách các slot nhiều component (`arm_guard`, `boots`, và phần trang bị hai bên) rồi bind component vào hand/foot/thigh/torso bone. Review B1 xác nhận khớp thân đã chuyển động, nhưng slot đôi còn là sprite chung và Võ nữ Lv30 chưa khớp trang bị đủ sạch; không claim paper-doll hoàn chỉnh và chưa mở class thứ hai.
## Sau checkpoint Võ v9 — shared character/action base

Tách loadout, rig pose và action orchestration khỏi `CongDongLamMap01AArtPreview` sang runtime base dùng chung, không đổi hình ảnh/output đã capture. Giữ manifest contract tương đương Sprite Library `Category + Label`; chưa thêm package Unity mới. Sau extraction, build/capture lại cùng ma trận 66×3 để chứng minh không regression rồi mới bắt đầu class thứ hai. Asset loading về sau chia theo map/class/tier residency, không load toàn bộ 5 class hoặc tạo bộ atlas riêng cho ba profile.

## Sau shared character runtime state — Kiếm Lv1–30 source batch

`SHARED_CHARACTER_RUNTIME_STATE_PLAYER_PASS`: state mode/gender/level/loadout/action đã rời Map01A vào `TwoDCharacterRuntimeState`; Player v22 và capture 66×3 giữ nguyên Võ/Q01–Q09, manifest có `voSharedRuntimeStateVerified=True`. Next: audit toàn bộ source Kiếm đã tuyển trong `LGO-Selected-2D-Source-v1` và backup sandbox chuẩn hóa cũ, chọn một identity nam/nữ đồng nhất, lập một crop/attachment plan đủ Lv1/10/20/30 × 10 slot rồi mới pack/nối base chung trong một batch. Không tạo lại từng item, không mở Pháp/Cơ/Linh trước khi Kiếm có Player evidence.

## Sau Kiếm source-v4 — base-fit/redraw gate trước atlas

Đã tách một lượt 80 ô thành source-v4 với đúng 10 slot canonical, nhưng không mặc định crop là item sạch: 12 crop nữ đã xác định `redraw-required`, 68 còn lại `candidate`, runtime eligible bằng 0. Next: dựng template canvas/attachment cho base nam/nữ, sửa 12 món dính body và fit theo lô; sau đó chạy mixed-level pairwise matrix qua idle/walk/run/jump/basic attack/class skill. Chỉ item `approved` mới được pack atlas 512–1024 và nối shared runtime state. Không mở Pháp/Cơ/Linh trước Player evidence Kiếm.

## Sau Kiếm side-view redraw source-v3 — SpriteSkin fit spike

Đã tạo lại theo base side-view hai sheet nam/nữ đủ 80 cell; nữ v1 lỗi dính body/tay được giữ làm evidence, nữ v2 đã sửa theo row, source-v3 đã crop/despill sạch theo batch. Tất cả vẫn candidate. Next: cài `com.unity.2d.animation` 13.x trong một spike có benchmark, weight một bộ mixed-level tối thiểu `Lv1 weapon + Lv30 hair + Lv10 inner + Lv20 outer` lên skeleton chung nam và nữ, chạy sáu motion state và capture ba profile. Chỉ giữ package nếu compile/build-size/runtime evidence đạt; nếu SpriteSkin không phù hợp thì quay về split component từ cùng source-v3, không sinh lại art.

## Sau Sprite Library compatibility spike — weight/fit batch đầu

Package 13.0.0 compile/test PASS và Player delta khoảng 0,188%, nên giữ. Adapter đã chứng minh loadout `Lv1 weapon + Lv30 hair + Lv10 inner + Lv20 outer` resolve độc lập qua Category/Label và chặn Skinned sprite chưa có bones. Next: fit cùng bốn món trên template nam/nữ, để weapon là Rigid, weight hair/inner/outer theo skeleton, kiểm occlusion và sáu state `idle/walk/run/jump/basic_attack/class_skill`, rồi mới nâng đúng các item đã pass lên approved và capture ba profile. Chưa import cả 80 candidate, chưa mở Pháp/Cơ/Linh.

## Sau mixed-loadout idle prefit — import/weight/motion

Prefit đã sửa hair/outer multi-view, normalize đúng PPU 208 và ghép idle nam/nữ thành công; weapon/inner được kế thừa. Next: import đúng 8 file của proof vào draft runtime pack, weight hair/inner/outer trên skeleton chung và giữ weapon Rigid; đăng ký qua Sprite Library, nối một opt-in Kiếm fit preview, chạy/capture sáu motion state trên mobile/tablet/PC. Chỉ các item qua motion/occlusion mới đổi thành `approved`; toàn bộ 72 candidate còn lại vẫn ở ngoài runtime.

## Sau Kiếm proof asset authoring — opt-in Player motion preview

8 proof item đã vào một draft atlas 512×512 ở PPU 208; 6 Skinned có bone/weight thật, 2 weapon Rigid. Atlas giữ nguyên cell đã normalize và guard khóa hash/provenance. Next: dựng component preview dùng base/rig chung, Sprite Library mixed loadout và SpriteSkin transforms; expose gender + `idle/walk/run/jump/basic_attack/class_skill`, rồi mới build/capture một lượt ba profile và review occlusion/grounding. Chưa nâng `approved` trước evidence này.

## Sau Kiếm shared-rig fit preview — hoàn thiện theo batch, không chỉnh từng món

Fit proof 4 slot đã chạy trong Player qua bone proxy và 78×3 capture; production vẫn chặn draft. Next: xử lý một lượt 6 slot Kiếm còn thiếu cho Lv1/10/20/30 nam/nữ từ source-v3, redraw theo cùng side-view base khi crop không phù hợp, pack theo atlas 512–1024 có byte budget; sau đó nối skill Kiếm Lv1 riêng và mới chạy lại full test/build/capture ba profile. Không nâng `approved` hoặc mở Pháp/Cơ/Linh trước khi đủ 10 slot, tháo/mặc và visual motion review.

48 crop của 6 slot còn thiếu đã được pack thành hai atlas candidate ngoài runtime tại `generated-batch-v2/remaining-six-slot-batch-v1/`, không resize và eligible 0. Next cụ thể: chọn `Lv1 lower_body + Lv10 waist_belt + Lv20 arm_guard + Lv30 footwear + Lv20 shoulder_chest_guard + Lv30 class_accessory` cho cả nam/nữ; ghép contact 10-slot với proof hiện tại, sửa lệch theo base rồi author bone proxy trái/phải. Chỉ sau contact pass mới import hai atlas fit đã chọn.
