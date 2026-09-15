# GOAL — LGO rigid outfit pilot

## Kết quả duy nhất

Chứng minh trong Unity Player một male và một female mini chibi dùng cùng canonical skeleton semantics, hai outfit cùng họ và một weapon. Body/equipment là các sprite cứng fit/equip một lần; Idle, Walk, Run, Jump, Attack, Roll, Hit và một animation mới chỉ đổi position/rotation của skeleton. Final #1 chỉ được gọi là đạt sau khi xem trực tiếp clip gameplay-speed và contact sheet.

## Kiến trúc khóa

- Source profile: `lgo_character_canvas_1024x1536_v1`, canvas `1024×1536`, origin X `512`, ground Y `1484`, `u=1.70/1536`.
- Blender `.blend` là editable source duy nhất; Python tạo body/outfit object cứng và render PNG RGBA từ cùng camera.
- Candidate v9 chỉ được chuyển thành silhouette/landmark QA reference; mask từ ảnh phẳng không được trở thành body hoặc equipment source.
- Mỗi visual part có một `partId`, một bone, một sprite form, một pivot và một render role cố định.
- Unity dùng `SpriteRenderer` con của bone và một `SortingGroup` ở root.
- Cấm SpriteSkin, mesh/weight/vertex deformation, warp, pose/frame/angle sprite swap, animated scale, per-pose offset và outfit-specific animation.
- Outfit #2 dùng lại interface/fingerprint fit của outfit #1; không đo lại body hoặc thêm offset riêng.
- Candidate v9 khóa identity, tỷ lệ và bind stance. Bộ sáu pose cũ của owner khóa silhouette chuyển động.

## Năm gate theo đúng thứ tự

1. `DESIGN_SOURCE_GATE`: Blender tạo neutral male/female đạt numeric + Codex visual review; `.blend` mở lại được và body object có hidden geometry.
2. `BODY_MOTION_GATE`: body-only qua alpha sweep và board 15 key; không hở khớp, cục tròn lộ, đổi tỷ lệ hoặc sai dáng.
3. `OUTFIT_01_GATE`: outfit #1 qua all-on, từng slot-off, seam-pair và continuous Player clip cho cả hai giới. Đây là Final #1 và phải dừng chờ owner review.
4. `REUSE_GATE`: outfit #2 khác design nhưng dùng lại toàn bộ body/interface fit; không remeasure và không thêm transform riêng.
5. `MIX_ANIMATION_GATE`: mix slot giữa hai outfit và animation mới giữ nguyên sprite/fit/scale, rồi mới quyết định pilot khả thi hay cần sửa.

## Quy tắc dừng và sửa

- Sai identity, camera, tỷ lệ, stance, belt hoặc silhouette: sửa source design trước; không rig/capture tiếp.
- Body hở: sửa body cap/underlap. Outfit hở: sửa garment cover/underlap. Một sprite cần hai bone: tách lại asset.
- Hai revision liên tiếp lặp cùng lỗi cấu trúc: bác interface/topology đó; không nắn số hoặc pixel lần ba.
- Validator xanh chỉ cho phép mở visual review. Không dùng report, layer count hoặc checkpoint để tuyên bố hình ảnh đạt.
- Không mở class/level/migration trước khi năm gate hoàn tất. Không đưa ETA tủ đồ trước khi có thời gian đo thật của outfit #1 và #2.

## Đường bị khóa

Không tái sử dụng dưới tên mới: six-pose/per-frame outfit art, composite slicing, flat-card/capsule/mannequin, vector trace RGB, silhouette width inference, ImageGen atlas/alpha cleanup, Blender weighted mesh, SpriteSkin hoặc pose correction sprite. Không coi SAM2/Grounded-SAM mask hay inpaint output là hidden geometry/source production. Evidence v7–v10 chỉ dùng để tránh lặp lỗi.

## Đầu ra

- `.blend` body nam+nữ và outfit #1/#2 mở lại được, có hierarchy/hash/provenance.
- PNG RGBA parts, bind/interface profile và source manifests.
- Unity v3 pilot, invariant tests, runtime-evidence JSON.
- Body, slot-off, seam, 15-key và continuous MP4/contact-sheet evidence.
- `source-motion-report.json` ghi thời gian thực đo; `FEASIBILITY-REVIEW.md` ghi source, hành vi Player, reuse, lỗi còn lại và bước migration.

## Execution authority

Thực hiện tuần tự theo `docs/superpowers/plans/2026-09-15-rigid-outfit-pilot-production.md`. Đánh giá quyết định công cụ nằm tại `docs/art/LGO-SINGLE-IMAGE-MODULAR-RIG-ASSESSMENT-2026-09-15.md`.

```json
{"activeTask":"LGO_RIGID_OUTFIT_PILOT_01","phase":"DESIGN_SOURCE_AUTHORING","status":"CONTINUE","method":"BLENDER_VOLUMETRIC_RIGID_SOURCE_TO_UNITY_SPRITES","sourceProfile":"lgo_character_canvas_1024x1536_v1","designAuthoritySha256":"b8d27811164ca7f190d2796f3231528a7a91ca7727ae26e8ed591e7890fa05fb","currentGate":"DESIGN_SOURCE_GATE_REVISION_01","runtimePromotionAllowed":false,"outfit2Allowed":false,"classExpansionAllowed":false}
```
