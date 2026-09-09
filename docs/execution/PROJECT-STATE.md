# PROJECT STATE — Linh Giới Online 2D

## Current branch

`feature/2d`

## Current direction

Scenario production spine mới: `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md`.

Linh Giới Online tiếp tục theo North Star Social Action MMORPG, nhưng branch này khóa hướng **2D Side-Scrolling Social Action MMORPG**: HD 2D anime / illustrated character, không pixel-art, side view, map parallax nhiều lớp, combat nhanh có walk/run/jump/dash/skill, hub xã hội đông người. Khóa kịch bản mới đã được đưa vào `docs/02-GDD.md`: giữ thế giới/class/progression/story/backend, đổi cách hiện thực sang sprite layer, skeleton 2D, anchor, atlas và runtime visual evidence. Linh Thành vẫn là hub xã hội; Đông Môn tutorial là lát cắt ưu tiên đầu: Người Giữ Cổng, Bia/Đá Luyện, movement/jump/dash, skill class, Shadow Slime, quay NPC và mở Linh Thành.

## Current source state

Đã dọn pipeline/source/asset/tool cũ liên quan hướng dựng nhân vật/cảnh 3D khỏi `feature/2d` và loại bỏ toàn bộ ảnh thiết kế/source cũ khỏi source tree để tránh kéo lại hướng art đã bỏ. Runtime 2D nhập môn hiện có một slice player-visible: di chuyển bằng WASD/phím mũi tên, focus NPC, mở thoại, nhận hướng dẫn tới Bia Luyện Khí, kích hoạt bia, hoàn tất chuỗi Jump/Dash/ClassSkill, đánh tan Shadow Slime bằng kỹ năng Võ Lv1 và hoàn tất nhập môn. Visual hiện là sprite/layer procedural gọn để kiểm flow, có HUD world-space, minimap/route overlay, snapshot base character nam/nữ, modular equipment và locomotion animation được camera capture; chưa phải art final. Kịch bản mới yêu cầu production map đi theo Zone Network và Chapter 1 `Vết Nứt Đông Môn`, không biến sân luyện thành toàn game và không triển khai map bằng cách crop/dán board.

## Validation spine

- `python3.12 tools/validate_2d_branch_no_3d.py` bảo vệ branch khỏi việc kéo lại pipeline cũ.
- `python3.12 tools/validate_2d_branch_no_source_images.py` bảo vệ branch khỏi ảnh thiết kế/source cũ; evidence trong `build/` được bỏ qua.
- Unity batch compile/test là gate trước khi báo checkpoint.
- `tools/run_lgo_2d_onboarding_smoke.sh` kiểm flow nhập môn bằng Unity Editor command-line.
- `python3.12 tools/lgo_runtime_smoke_matrix.py --phase two-d` kiểm evidence runtime 2D đã sinh: onboarding smoke JSON, macOS Player build log và visual manifest có `runtimeTilemapSnapshot`/`ChunkFlow`.
- `python3.12 tools/lgo_visual_evidence_matrix.py --verify-current` kiểm các frame visual 2D trọng yếu: initial, gate focus, skill ready, inventory try và inventory applied; mọi claim vẫn là runtime evidence, không phải production art.
- `python3.12 tools/validate_lgo_crash_error_reporting_plan.py` bảo vệ crash/error plan local: phân loại `FIX_REQUIRED`, `UNVERIFIED_ENVIRONMENT`, `CONTRACT_CHANGE_REQUIRED`, `HUMAN_REVIEW_REQUIRED` mà không tích hợp production service/telemetry.
- Player smoke và visual capture trong `build/2d-onboarding-player/` + `build/2d-onboarding-visual/` là evidence runtime thật cho slice 2D, gồm HUD snapshot trong manifest và ảnh PNG review.

## Latest checkpoint evidence

- Cleanup commit: `d97a3c8 Remove 3D asset pipeline from 2D branch`.
- Meshy/service trace cleanup commit: `6bf905e Remove obsolete 3D service traces from 2D branch`.
- Source image cleanup commit: `646492e Remove legacy images and keep 2D HUD procedural`.
- Procedural Đông Môn blockout commit: `eeaf19d Improve 2D onboarding map blockout details`.
- Direction/map catalog checkpoint: `1fa3231 Lock 2D social action direction and map catalog`.
- Character base checkpoint: `0d31cc8 Add 2D character base catalog`.
- Modular equipment checkpoint: `091a2e3 Add 2D modular equipment runtime spine`.
- Võ Lv1 seed checkpoint: `81000ed Add 2D Vo Lv1 starter outfit seed`.
- Locomotion animation checkpoint: `c04e40f Add 2D locomotion animation spine`.
- Tutorial movement/skill checkpoint: Bia Luyện Khí mở chuỗi Jump → Dash → ClassSkill → Complete; visual capture 8 frame và manifest kiểm `screenshotCount=8`, `finalStep=Complete`, animation tokens Jump/Dash/ClassSkill/TrainingCompletePose.
- Shadow Slime combat micro-slice checkpoint: sau Dash, Shadow Slime xuất hiện ở `LearnClassSkill`; ClassSkill đánh tan slime, manifest có `runtimeCombatSnapshot=ShadowSlimeVisible=False ShadowSlimeDefeated=True`, ảnh review `07-skill-ready.png`/`08-complete.png`.
- Map layer budget checkpoint: `TwoDMapDesignCatalog` expose `LayerBudget: Chapter 1: Vết Nứt Đông Môn` với Sky/Fog, Far, Mid, Near, Gameplay Plane, Foreground; minimap overlay hiển thị Chapter/Layer marker và manifest `runtimeMapSnapshot` chứa layer budget.
- Route progress checkpoint: `TwoDOnboardingState.CurrentRouteNodeId` bám các node spawn → gatekeeper → training-stone/movement → jump → dash → shadow-slime → return-gate; minimap hiển thị `Node:` hiện tại và manifest có `runtimeRouteProgressSnapshot`.
- Kiếm Lv1 module checkpoint: catalog có module áo Kiếm nam/nữ, quần/đai/găng/boots, vũ khí kiếm starter với `class=Kiem`, `level=1`, `sword_trail_seed`; loadout mix được áo/kiếm Kiếm với quần Võ qua cùng fit profile. Runtime Đông Môn có rack preview `KIẾM LV1` để chứng minh module class thứ hai đi vào scene mà không cần Meshy/3D hay ảnh source.
- Đông Môn landmark checkpoint: `DongMonLandmarks` khóa Cổng Linh Thành, Bia Luyện Khí, Cầu Gỗ, Thác Nước, Sóng Linh và Rừng Ngoại Thành với layer/route node; runtime scene có silhouette cầu/rừng/thác/sóng linh và manifest chứa `Landmarks: Chapter 1`.
- Inventory try-on checkpoint: runtime Đông Môn có strip `HÀNH TRANG` cho icon Võ/Kiếm/kiếm starter; `RuntimeInventoryTryOnSnapshot` và visual manifest khóa flow `select_icon -> inspect_item -> try_on -> cancel_or_apply`, selected `top_kiem_lv1_male`, preview `status=TRYING_ON`.
- Scenario production spine checkpoint: `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md` đưa kịch bản game mới của owner vào source chính: HD 2D anime/illustrated, không pixel-art, Zone Network, Chapter 1/2/3, class identity, item/map pipeline và roadmap 2D-00 → 2D-12.
- Terrain collision spine checkpoint: Đông Môn có contract `DongMonCollisionBands` cho ground-main, training-platform, jump-gap, dash-lane và slime-arena; runtime có cue `JUMP GAP`/`DASH LANE`, visual manifest chứa `runtimeTerrainCollisionSnapshot` và ảnh review không che HUD/action chính.
- Inventory input spine checkpoint: panel `HÀNH TRANG` hỗ trợ `I` mở/đóng, `Tab` chọn, `T` thử, `Y` áp dụng, `Esc` hủy; `RuntimeInventoryInputSnapshot` ghi state Applied/Trying/Cancelled và visual capture có frame `09-inventory-try`, `10-inventory-applied`.
- Đông Môn tilemap runtime spine checkpoint: map catalog có `DongMonTileDefinitions` cho ground grass/stone, wood platform, jump gap, dash lane và slime arena; runtime scene render tile chunks Gate/Training/Jump/Dash/Slime cùng platform/gap/dash/slime cues, visual manifest ghi `runtimeTilemapSnapshot` + `ChunkFlow`, Player capture 10 frame đã review không che HUD/action chính.
- Runtime smoke matrix 2D checkpoint: `tools/lgo_runtime_smoke_matrix.py --phase two-d` báo `LGO_RUNTIME_SMOKE_MATRIX_2D_PASS`, xác minh smoke JSON, Player build log và visual manifest của onboarding 2D hiện tại.
- Visual evidence matrix 2D checkpoint: `tools/lgo_visual_evidence_matrix.py --verify-current` báo `LGO_VISUAL_EVIDENCE_MATRIX_2D_CURRENT_PASS`, xác minh 5 screenshot evidence quan trọng và các manifest field tương ứng, đồng thời giữ non-claim production art.
- Crash/error reporting plan checkpoint: `tools/validate_lgo_crash_error_reporting_plan.py` báo `LGO_CRASH_ERROR_REPORTING_PLAN_VALIDATION_PASS`; `tools/lgo_error_report_summary.py` phân loại local closure summary thiếu là `UNVERIFIED_ENVIRONMENT`, chưa thêm production crash/telemetry service.
- Quảng Trường plaza shell checkpoint: map catalog expose `LinhThanhPlazaShellSnapshot` với `PlazaShell: district=plaza`, `social-spawn`, `event-board`, `guild-bulletin-preview`, `safe-no-trade-backend`; runtime scene có preview marker social/event/guild trong nền; Player build/capture mới PASS và smoke matrix 2D bắt PlazaShell.
- Linh Thành hub shell checkpoint: map catalog expose `LinhThanhHubShellSnapshot` với `HubShell: linh-thanh`, `district=plaza`, `district=academy`, `district=market`; runtime scene thêm marker/silhouette hub phía sau Đông Môn; Player build/capture mới PASS, smoke matrix 2D bắt buộc token HubShell.
- World/Linh Thành zone-network checkpoint: map catalog expose `ZoneNetworkSnapshot` với `WorldMapNetwork: hub=linh-thanh` và `LinhThanhHubRuntime`; runtime minimap overlay hiển thị node LT ↔ Đông Vực/Âm Giới; Player build/capture mới PASS, smoke matrix 2D giờ bắt buộc `runtimeMapSnapshot` giữ zone network.
- Linh Thành unlock presentation checkpoint: sau Shadow Slime/Complete, `LinhThanhUnlocked=true`, HUD hiện “Mở Linh Thành: Quảng Trường”, runtime scene bật banner/path local preview và manifest có `runtimeLinhThanhUnlockSnapshot` với `unlock=plaza`, `safe-local-no-teleport`; Player build/capture mới PASS và ảnh complete/inventory đã review không che HUD/minimap/inventory.
- Quảng Trường hub runtime preview checkpoint: sau unlock Đông Môn, runtime expose `RuntimeLinhThanhPlazaHubSnapshot`/manifest field với `PlazaHubRuntime`, `npc=gate-guide`, `board=event-local-preview`, `guild-bulletin=locked`, `safe-local-no-backend`; scene bật cụm NPC/board/label sau unlock và frame initial đã review không hiện sớm.
- Đông Môn parallax/foreground polish checkpoint: runtime map catalog có `ParallaxDepthSnapshot` cho `Layer5=Sky/Fog:cloud-drift`, `Layer4=Far Background:mountain-silhouette`, `Layer0=Foreground:grass-leaf-motes`; Player build/capture mới PASS, manifest 10 frame chứa `ParallaxDepth`, ảnh review không che HUD/combat/inventory; app build thực tế khoảng 109MB dù Unity log có bước prepared ~36GB.
- Alpha/Beta/Live checklist checkpoint: `tools/validate_lgo_release_checklist.py` báo `LGO_RELEASE_CHECKLIST_VALIDATION_PASS`; release state vẫn là pre-alpha development, không claim alpha/beta/live hoặc production art.

## Next

Tiếp tục roadmap 2D bằng parallax spacing/foreground polish cho Đông Môn hoặc chuyển chunk procedural sang authored Unity Tilemap asset khi asset sạch sẵn sàng hoặc nâng inventory panel sang inspect detail/icon grid đẹp hơn sau khi có art asset sạch. Map work phải bám `docs/02-GDD.md`, `docs/design/LGO-2D-SOCIAL-ACTION-DIRECTION-LOCK-v0.1.md`, `docs/design/LGO-2D-MAP-AZ-DESIGN-v0.1.md` và không mở HP/loot/economy/server-authoritative combat khi chưa có gate riêng.
## 2026-09-10 — Quảng Trường board interaction + truthful Unity test gate

`LGO_LINHTHANH_PLAZA_BOARD_INTERACTION_READY`: sau khi hoàn tất Đông Môn/Shadow Slime và unlock Linh Thành, state runtime cho phép mở preview local-only của `Bảng sự kiện Quảng Trường`. HUD chuyển đúng sang `Khu vực: Quảng Trường`, visual manifest ghi `runtimeLinhThanhPlazaHubSnapshot` với `interaction=board-preview-open`, và visual capture có frame `11-plaza-board-preview`. Inventory panel được ẩn mặc định và chỉ hiện khi người chơi mở hành trang để board preview không bị che.

Đồng thời sửa `tools/unity_batch_test.sh`: bỏ `-quit` khỏi `-runTests`, bắt buộc có XML results, `total>0`, `passed>0`, `failed=0` trước khi in `UNITY_EDITMODE_PASS`. Evidence mới: Unity EditMode thật `total=98 passed=97 failed=0`, Editor smoke PASS, macOS Player build PASS (`totalSize=114657763`), `tools/lgo_runtime_smoke_matrix.py --phase two-d` PASS và `tools/lgo_visual_evidence_matrix.py --verify-current` PASS. Scope vẫn local-only: chưa mở teleport/shop/giao dịch/bang hội/backend.

## LGO_LINHTHANH_PLAZA_NPC_INTERACTION_READY — 2026-09-10

Quảng Trường có NPC interaction local-only sau unlock Đông Môn: `TryTalkPlazaGateGuide()` và `TryTalkPlazaMerchantPreview()` tách `DialogueSpeaker` khỏi `DialogueLine`, visual frame `12-plaza-npc-preview`, manifest `runtimeLinhThanhPlazaHubSnapshot` có `npc=merchant-preview`, `interaction=npc-merchant-preview`, `safe-local-no-shop-backend`. Đây vẫn là social hub preview an toàn: chưa mở shop/economy, teleport, giao dịch, bang hội hoặc backend.

## LGO_LINHTHANH_PLAZA_TARGET_SELECTOR_READY — 2026-09-10

Quảng Trường sau unlock Đông Môn nay có target selector runtime local-only: người chơi dùng `P` để đổi mục tiêu giữa `Bảng Sự Kiện`, `Người Giữ Cổng` và `Thương Nhân`, dùng `E/Enter` để tương tác mục tiêu đang chọn. Manifest visual có `runtimePlazaHubInputSnapshot` với `controls=P select, E interact`, `selected=merchant-preview`, `interaction=npc-merchant-preview`, `safe-local-no-shop-backend`; visual capture tăng lên 13 frame, gồm `12-plaza-target-selector` và `13-plaza-npc-preview`. Scope vẫn an toàn: chưa mở teleport, shop/economy, giao dịch, bang hội hoặc backend.

## LGO_LINHTHANH_PLAZA_SOCIAL_LAYOUT_READY — 2026-09-10

Quảng Trường selector được nới thành layout `spaced-social-triangle`: Người Giữ Cổng ở trái, Bảng Sự Kiện ở giữa/phía sau, Thương Nhân ở phải để giảm cảm giác chắp cụm và giúp frame selector/NPC dễ đọc hơn. Manifest `runtimePlazaHubInputSnapshot` bắt `layout=spaced-social-triangle`; visual matrix vẫn giữ scope local-only, chưa mở shop/economy/teleport/bang hội/backend.

## LGO_LINHTHANH_HUB_TRANSITION_PREVIEW_READY — 2026-09-10

Đã thêm transition shell local-only `Đông Môn → Quảng Trường`: controller expose `RuntimeHubTransitionSnapshot`, input/runtime method `PreviewEastGateToPlazaTransition()`, visual capture có frame `14-plaza-transition-preview`, manifest bắt `HubTransition: unlocked=True`, `from=east-gate`, `to=plaza`, `mode=local-route-preview`, `safe-local-no-teleport-backend`. Đây là route preview để map A-Z có xương sống đi từ tutorial sang hub; chưa mở teleport thật, map streaming, server travel hoặc backend xã hội.

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

`LGO_LINHTHANH_PLAZA_LAYOUT_ANCHORS_READY`: runtime Quảng Trường sau unlock Đông Môn đã có `PlazaHubLayout` với 5 anchor social-spawn, event-board, gate-guide, merchant-preview và guild-locked. Đây là layout/social hub local-only cho Player capture, chưa phải map production art và chưa mở backend event/shop/guild.
## 2026-09-10 — Quảng Trường anchor detail inspect

`LGO_LINHTHANH_PLAZA_ANCHOR_DETAIL_READY`: runtime Quảng Trường đã expose `PlazaHubDetail` cho target đang chọn và visual manifest/matrix bắt merchant preview `role=starter-gear-preview`, `detail=try-before-shop`. Đây là inspect local-only, chưa mở shop/event/guild/backend.
## LGO_LINHTHANH_DISTRICT_PREVIEW_READY — 2026-09-10

`feature/2d` đã có district preview rail local-only cho Linh Thành sau khi hoàn tất Đông Môn. Controller/manifest xuất `runtimeLinhThanhDistrictPreviewSnapshot`; Player capture có 15 frame, trong đó `15-district-preview-rail` hiển thị ring/label Học Viện và guard `safe-no-district-backend`. Trạng thái vẫn là pre-alpha blockout map, chưa production map toàn thế giới.
## LGO_LINHTHANH_DISTRICT_DETAIL_READY — 2026-09-10

`feature/2d` đã có `RuntimeLinhThanhDistrictDetailSnapshot` và Player capture 16 frame. Frame `16-district-market-preview` cho thấy district rail chọn Thương Phố sau Học Viện, manifest guard `safe-no-trade-backend`, `safe-no-economy-backend`, `safe-no-district-backend`. Map tổng thể vẫn pre-alpha blockout, chưa production-complete.

## 2026-09-10 — Linh Thành Đền Linh preview rail

- Marker: `LGO_LINHTHANH_SPIRIT_TEMPLE_PREVIEW_READY`.
- Trạng thái: runtime 2D đã có Player capture riêng `17-district-spirit-temple-preview` cho Đền Linh trong district preview rail; manifest selected `spirit-temple`, route `plaza->spirit-temple`, detail `altar-local-only`, next `quest-buff-gate`.
- Giới hạn: preview local-only, chưa phải production map/art hoàn thiện, chưa mở buff/story/backend/teleport.
- Next: tiếp tục map-safe bằng polish visual/readability Linh Thành hoặc nâng Đông Môn Tilemap/atlas sạch.

## 2026-09-10 — Linh Thành district rail coverage

- Marker: `LGO_LINHTHANH_DISTRICT_RAIL_COVERAGE_READY`.
- Trạng thái: Player visual capture đã tăng lên 20 frame và cover rail Học Viện, Thương Phố, Đền Linh, Khu Rèn, Khu Bang Hội, Cảng Linh Thuyền.
- Evidence cuối giữ Cảng: `selected=harbor`, `label=Cảng Linh Thuyền`, `route=plaza->harbor`, `detail=spirit-boat-locked`, `next=world-route-gate`.
- Giới hạn: vẫn là runtime blockout/local-only; chưa mở crafting, guild, travel, teleport hoặc district backend.
- Next: polish visual density/layout Linh Thành hoặc nâng Đông Môn Tilemap/atlas sạch.

## 2026-09-10 — Linh Thành district rail readability

- Marker: `LGO_LINHTHANH_DISTRICT_RAIL_READABILITY_READY`.
- Trạng thái: district preview callout/backplate đi theo selected node, manifest có `DistrictRailReadability` và Player visual capture 20 frame đã review frame Khu Rèn/Bang Hội/Cảng.
- Giới hạn: polish UX cho runtime blockout local-only; chưa thay production art, chưa mở district backend.
- Next: polish minimap/district rail density hoặc nâng Đông Môn Tilemap/atlas sạch.

## 2026-09-10 — 2D minimap compact readability

- Trạng thái: `LGO_MINIMAP_COMPACT_READABILITY_READY`; overlay/minimap giảm mật độ chữ, route `Đông Môn → Linh Thành`, district chips `HV TP ĐL KR BH CẢ`, world links ngắn và selected-node progress.
- Evidence cần giữ: `runtimeMinimapReadabilitySnapshot` trong visual manifest, Player capture frame `01-initial` và `20-district-harbor-preview`, smoke/visual matrix PASS.
- Giới hạn: local-only, chưa mở teleport/travel/backend; map chưa production-complete toàn bộ A-Z.
- Next: Đông Môn authored Tilemap/atlas sạch hoặc mở chi tiết khu Linh Thành tiếp theo với guard local-only.
