# TASK LEDGER — 2D Branch

## 2026-09-09 — 2D pivot cleanup

- Branch: `feature/2d`.
- Owner yêu cầu loại bỏ hướng dựng nhân vật/cảnh cũ và chuyển sang game 2D.
- Đã dọn source Unity, asset source, tooling thử nghiệm, generated staging/evidence và cache nặng liên quan hướng cũ.
- Đã thêm validator `tools/validate_2d_branch_no_3d.py`.
- Frozen surfaces không đổi.

## Next

Dựng runtime 2D SCN-001/002 có thể chơi và capture thật.

## 2026-09-09 — 2D onboarding runtime slice

- Branch: `feature/2d`.
- Dựng slice nhập môn 2D bám SCN-001/002: player bắt đầu ở Cổng Linh Thành, tới Người Giữ Cổng, mở thoại, nhận hướng dẫn tới Bia Luyện Khí, kích hoạt bia và hoàn tất nhập môn.
- Thêm state machine, controller sprite/layer placeholder, smoke runner Editor/Player, visual capture runner 5 trạng thái và edit-mode coverage cho flow.
- Evidence runtime: `build/2d-onboarding-player/player-smoke.json`, `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`, ảnh review `build/2d-onboarding-visual/05-complete.png`.
- Giới hạn đã biết: visual capture camera-render chưa bắt HUD OnGUI; HUD cần chuyển sang UI runtime ở batch tiếp theo.

## Next after runtime slice

Nâng art/UI 2D cho SCN-001/002, ưu tiên sprite 2D đẹp hơn và HUD capture được, không mở rộng combat/reward/frozen contracts.


## 2026-09-09 — 2D onboarding HUD capture

- Thay HUD OnGUI tạm bằng HUD world-space để visual capture camera-render bắt được nội dung player-facing.
- Thêm test `RuntimeControllerMaintainsCameraCapturedHudText`; RED đã fail vì thiếu `WorldHudSnapshot`/`WorldHudLineCount`, GREEN qua Unity compile/test sau implementation.
- Visual manifest bổ sung `hudLineCount` và `hudSnapshot`.
- Evidence: `build/2d-onboarding-visual/03-dialogue.png`, `build/2d-onboarding-visual/05-complete.png`, `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`.

## Next after HUD capture

Thay dần sprite placeholder bằng art 2D đẹp hơn cho Cổng Linh Thành, NPC, player và Bia Luyện Khí; không quay lại pipeline 3D cũ.

## 2026-09-09 — Remove obsolete source images for 2D branch

- Owner yêu cầu bỏ hết ảnh thiết kế/source cũ vì không còn dùng được cho hướng 2D mới.
- Đã xoá ảnh source cũ khỏi Unity Art/UI Resources và docs/reference/design cũ; evidence runtime trong `build/` không thuộc source tree.
- Thêm validator `tools/validate_2d_branch_no_source_images.py` để phát hiện ảnh source lẻn lại.
- Runtime 2D onboarding vẫn dùng procedural sprites/HUD world-space nên không phụ thuộc các ảnh đã xoá.

## Next after image cleanup

Thiết kế lại art 2D cho SCN-001/002 theo mẫu mới được duyệt, bắt đầu từ Cổng Linh Thành, Người Giữ Cổng, player và Bia Luyện Khí.

## 2026-09-09 — Đông Môn procedural blockout detail pass

Nâng runtime 2D onboarding theo hướng map Đông Môn tutorial: thêm biển Cổng Linh Thành, viền/ấn ngọc, lồng đèn, lối ngọc, đường kẻ sân, label Bia Luyện Khí, đốm linh khí và silhouette nhân vật nhiều lớp hơn. Thêm scene-beat snapshot/count vào controller và visual manifest để gate không chỉ dựa vào cảm giác. Evidence mới: Unity EditMode, Editor smoke, macOS Player build và Player visual capture 5 frame.

## Next after Đông Môn blockout

Triển khai design map A-Z từ board mới owner gửi: world map tổng thể → Linh Thành hub → Đông Môn tutorial → layer/tileset/ký hiệu/flow, bắt đầu bằng spec text và runtime blockout không dùng ảnh cũ.

## 2026-09-09 — 2D direction lock + map catalog realignment

Owner cung cấp kịch bản game mới trước khi design. Đã khóa lại hướng: 2D Side-Scrolling Social Action MMORPG, HD anime/stylized, không pixel-art, không quay lại Meshy/3D, giữ backend/world/class/progression/story. Thứ tự ưu tiên mới: 2D-00 Direction Lock → 2D-01 Male/Female Base Character → 2D-02 Character Modular Runtime → class Lv1 → animation → Linh Thành/Đông Môn map → Shadow Slime combat → vertical slice. Runtime map catalog bắt đầu phản ánh World/Linh Thành/Đông Môn tutorial mới và visual manifest có map snapshot để tránh làm mò.

## Next after direction lock

Sau khi checkpoint này pass/push, bắt đầu 2D-01 Male/Female Base Character; chỉ triển khai map production lớn sau khi character base/modular runtime đủ spine.

## 2026-09-09 — 2D-01 Male/Female Base Character

- Bám kịch bản game mới: 2D Side-Scrolling Social Action MMORPG, HD anime/stylized, không Meshy/3D, class đến từ equipment/layer chứ không tách body riêng.
- Thêm design doc `docs/design/LGO-2D-CHARACTER-BASE-DESIGN-v0.1.md` để khóa layer order, anchors và default outfit trước khi sản xuất modular gear.
- Thêm runtime catalog `TwoDCharacterBaseCatalog` với `male_base`/`female_base`, layer order chuẩn, anchor tối thiểu và snapshot dùng trong visual manifest.
- Runtime onboarding expose `runtimeCharacterBaseSnapshot` và minimap label Base để capture có bằng chứng base nam/nữ đang nằm trong spine runtime.
- Evidence: Unity EditMode, onboarding smoke, Player visual capture manifest `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json`, ảnh review `03-dialogue.png` và `05-complete.png`.

## Next after 2D-01

2D-02 Character Modular Runtime: item/layer registry, equip/unequip, preview hành trang đã duyệt, giữ tóc/mắt/trang bị/vũ khí/pet có id riêng và dùng form chung để tránh fit/chắp vá từng món.

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

- Cập nhật kịch bản game mới của owner thành source chính: `docs/design/LGO-2D-SCENARIO-PRODUCTION-SPINE-v0.1.md`.
- Khóa các nguyên tắc: giữ thế giới/class/progression/story/backend, chỉ chuyển hiện thực sang 2D; HD 2D anime/illustrated, không pixel-art; Zone Network; Chapter 1/2/3; class identity; item/map pipeline; roadmap 2D-00 → 2D-12.
- Bắt đầu/hoàn thiện WIP terrain collision cho Đông Môn: ground-main, training-platform, jump-gap, dash-lane, slime-arena, runtime snapshot và visual manifest.
- Trạng thái: Unity compile, onboarding smoke, Player build/capture, source validators và visual review đã chạy cho checkpoint.


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
