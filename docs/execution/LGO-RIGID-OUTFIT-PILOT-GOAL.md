# GOAL — LGO rigid outfit pilot

## Kết quả cần chứng minh

Một male và một female mini chibi chạy trong Unity Player bằng cùng canonical skeleton. Body, outfit và weapon chỉ gồm `SpriteRenderer` cứng gắn vào bone. Mỗi item fit/equip một lần; Idle, Walk, Run, Attack, Roll và Hit chỉ đổi position/rotation của skeleton. Không SpriteSkin, mesh/vertex deformation, scale animation hoặc sprite swap theo pose/frame/góc.

## Gate hiện hành — owner review Final #1

`NEED_HUMAN_VISUAL_REVIEW`. Final #1 review candidate đã chạy thật trong Unity Player bằng rigid source v2. Contact sheet sạch và clip 150 frame cho thấy Run đủ bốn pha contact/pass đối xứng, Jump có anticipation/takeoff/apex/landing, Attack có windup/impact và Roll quay tới với core/limb articulation. Đây là self-review PASS để owner kiểm, chưa phải owner acceptance và chưa mở outfit #2/class/level.

Source v2 tách cổ tay/cổ chân bằng cùng canvas/registration và overlap cố định; fit distal part được suy ra từ bind coordinates thay vì offset theo pose. Run/Jump dùng quỹ đạo chân chung + nghiệm two-bone IK trên canonical limb lengths để khóa ground contact. HUD bị loại khỏi capture để không che nhân vật.

## Quy trình bắt buộc từ đây

1. Chốt một rigid-native bind source: torso/neck/head, upper/lower arms, upper/lower legs và garment covers có pivot/overlap được khai báo trong cùng hệ tọa độ `lgo_character_canvas_1024x1536_v1`.
2. Đo anchor từ source một lần. Không chỉnh vị trí theo pose, item hoặc frame.
3. Chạy keyframe rẻ cho Idle/Walk/Run/Jump/Attack/Roll trước. Review trực tiếp toàn board và ghi một danh sách lỗi theo `SOURCE`, `PIVOT`, `OVERLAP`, `MOTION_RANGE`, `RENDER_ORDER`.
4. Chỉ sửa theo nhóm nguyên nhân rồi capture lại một lần. Nếu cùng lỗi cấu trúc còn xuất hiện ở hai capture liên tiếp, bác topology/source giả thuyết đó; không tiếp tục nắn số.
5. Chỉ khi outfit #1 đạt bằng mắt mới chạy clip đầy đủ và mới được mở outfit #2. Validator chỉ chặn hồi quy kỹ thuật, không quyết định visual PASS.

## Visual acceptance của outfit #1

- Idle giữ đúng true side-view, tỷ lệ đầu/thân/tay/chân ổn định.
- Walk/Run có bốn pha khác nhau, tay và chân đối nghịch, gối nhấc hợp lý, chân không trông rời cơ thể.
- Jump có anticipation, takeoff, air và land; trục đầu–ngực–hông và tỷ lệ chi giữ đúng, tiếp đất có hấp thụ lực.
- Attack có anticipation và impact rõ; bàn tay giữ vũ khí đúng hướng.
- Roll có anticipation/tuck/open/land, không quay `CharacterRoot`, không hở cổ/vai/hông/gối/cổ chân.
- Không mất chi tiết, xuyên sai layer, thay sprite, kéo hình hoặc thay scale.
- Male/female dùng cùng motion semantics nhưng vẫn đọc được là hai nhân vật riêng.

## Đường bị khóa

- Không quay lại six-pose/per-frame art, SpriteSkin, weighted mesh, capsule limb, flat-card body hoặc pose correction sprite.
- Không dùng source Blender weighted-mesh hiện tại như bằng chứng rigid production.
- Không tích hợp bộ giáp đỏ Phase 8 đã sinh trước khi outfit #1 đạt visual; giữ nó ở build evidence với marker `REJECTED-NOT-INTEGRATED`.
- Không mở class/level/migration trước khi male + female, hai outfit, mix-and-match, animation mới, invariants, visual và performance đều đạt.

## Active state

```json
{"activeTask":"LGO_RIGID_OUTFIT_PILOT_01","phase":"FINAL_OUTFIT1_CANDIDATE_READY_FOR_OWNER_REVIEW","status":"NEED_HUMAN_VISUAL_REVIEW","method":"UNITY_PARENTED_SPRITERENDERER_RIGID_WITH_TWO_BONE_FOOT_TARGET_IK","sourceProfile":"lgo_character_canvas_1024x1536_v1","requiredMotion":["Idle","Walk","Run","Jump","Attack","Roll"],"currentPlayerEvidence":"build/rigid-outfit-pilot/final-1-review","currentSourceAuthorityValid":true,"technicalInvariantStatus":"PASS_17_OF_17","visualStatus":"FINAL_1_SELF_REVIEW_PASS_OWNER_REVIEW_REQUIRED","stopCondition":"OWNER_REVIEW_FINAL_OUTFIT1","redesignOutfit1Allowed":true,"outfit2IntegrationAllowed":false,"classExpansionAllowed":false,"perPoseAssetsAllowed":false,"deformationAllowed":false,"animatedScaleAllowed":false}
```
