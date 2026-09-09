# NEXT ACTION — Linh Giới Online 2D

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
