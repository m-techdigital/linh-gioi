## Current Owner Goal Override — 2026-09-10

- Map đầu Đông Môn là vertical-slice grounding/proof surface; không mở map thứ hai.
- Class workflow chỉ xử lý **một class trước: Võ Lv1-30**. Không làm đồng loạt 5 class, không mở nội dung 31+.
- Việc tiếp theo sau checkpoint cell-map: refine alpha matte/pivot/scale/head-arms-legs coverage và pose/frame motion Võ Lv1-30 trong Player; kiểm chứng thay đồ, tách layer, skill cue, contact shadow và route grounding cùng scene.
- Chỉ khi Võ Lv1-30 có runtime evidence đủ rõ mới nhân pattern sang Kiếm/Pháp/Cơ/Linh Lv1-30.
- Không Meshy/3D, không dùng ảnh thiết kế cũ làm runtime asset, không sửa frozen surfaces.

`LGO_VO_LV1_30_CELLMAP_COVERAGE_READY`: Cell-map approved runtime art Võ Lv1-30 đã mở rộng từ 8 lên 11 cell, thêm torso underlay và hai mảnh pants để giảm fallback primitive trong Player. Evidence nằm ở checkpoint commit tương ứng; đây vẫn là art/probe giai đoạn đầu, chưa phải production final.

# NEXT ACTION — Linh Giới Online 2D

## Goal kỹ thuật hiện tại — 2026-09-10

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
