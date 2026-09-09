# NEXT ACTION — Linh Giới Online 2D

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

1. Commit/push checkpoint inventory input runtime spine sau khi source validators và visual review pass.
2. Tiếp roadmap: tilemap authoring Đông Môn theo scenario spine hoặc nâng inventory panel sang inspect detail/icon grid khi có art asset sạch.
3. Không mở HP bar, loot/economy/server-authoritative combat trước khi có task/gate riêng.

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
