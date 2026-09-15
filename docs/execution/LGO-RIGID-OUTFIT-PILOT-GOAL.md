# GOAL — LGO rigid outfit pilot

## Kết quả cần chứng minh

Một male và một female mini chibi chạy trong Unity Player bằng cùng canonical skeleton. Body, outfit và weapon chỉ gồm `SpriteRenderer` cứng gắn vào bone. Mỗi item fit/equip một lần; Idle, Walk, Run, Attack, Roll và Hit chỉ đổi position/rotation của skeleton. Không SpriteSkin, mesh/vertex deformation, scale animation hoặc sprite swap theo pose/frame/góc.

## Gate hiện hành — owner review joint/layer formula v1

`NEED_HUMAN_VISUAL_REVIEW`. Kỹ thuật rigid của Final #1 đã được owner xác nhận khả dụng. Side-profile v4 và bản 3/4 quá chính diện bị bác. Candidate v5.2 chỉ xoay 12–15° ra trước từ profile và thiết kế lại outfit để che pivot bằng overlap cứng tại cổ, vai, khuỷu, cổ tay, eo/hông, gối và cổ chân; bản này thêm cuff cổ chân rõ giữa boot shaft và foot. Đai nằm tại đường eo/pelvis. Ảnh sáu pose cũ của owner là authority duy nhất cho contact A/B, high-knee A/B và jump tuck. ImageGen không được dùng làm pose authority vì ba lượt đã lặp sai chân dẫn.

Topology v2 có 16 body part + 25 outfit part theo contract `ONE_VISUAL_PART_ONE_BONE_ONE_FIXED_FORM`. Mỗi joint dùng một pivot chung nhưng có hai vùng che độc lập: body child cap phủ body parent underlap, outfit child cover phủ outfit parent underlap. Body-only phải tự nhiên trên toàn safe ROM; equipment không được chứa pixel da để vá body. Công thức và board review nằm tại `docs/art/LGO-RIGID-JOINT-AND-LAYER-AUTHORING-CONTRACT-v1.md` và `build/rigid-outfit-pilot/final-1-joint-authoring-v1/joint-contract-review.png`.

Source `rigid-source-v2` cũ chỉ cắt alpha theo đường ngang với overlap chữ nhật nên không chứng minh khớp kín khi xoay; giữ làm evidence, cấm dùng làm source authority mới. Candidate v5.2 vẫn là composite style art, chưa có hidden pixels và chưa phải layered source.

## Quy trình bắt buộc từ đây

1. Duyệt bind silhouette và topology body/outfit. Chưa duyệt góc, tỷ lệ, trục người, belt/cuff/guard thì không author layer.
2. Tạo body-only layered bind source; đo pivot và body joint width một lần trên `lgo_character_canvas_1024x1536_v1`, rồi calibrate joint profile. Không chỉnh vị trí theo pose, item hoặc frame.
3. Body parent underlap và child cap tròn phải tự kín khi tháo toàn bộ đồ. Sau đó mới author garment parent underlap/child cover theo cùng pivot; equipment không chứa body pixel.
4. Chạy body-only, từng slot-off, cặp slot giao seam và all-on qua alpha sweep trước Player; sau đó mới chạy 15 key Idle/Walk/Run/Jump/Attack/Roll.
5. Review trực tiếp toàn board và ghi một danh sách lỗi theo `SOURCE`, `BODY_CAP`, `OUTFIT_COVER`, `PIVOT`, `MOTION_RANGE`, `RENDER_ORDER`.
6. Chỉ sửa theo nhóm nguyên nhân rồi capture lại một lần. Nếu cùng lỗi cấu trúc còn xuất hiện ở hai capture liên tiếp, bác topology/source giả thuyết đó; không tiếp tục nắn số.
7. Chỉ khi outfit #1 đạt bằng mắt mới chạy clip đầy đủ và mới được mở outfit #2. Validator chỉ chặn hồi quy kỹ thuật, không quyết định visual PASS.
8. Không gọi một chuỗi state rời là Final. Final #1 bắt buộc có combo liên tục ở gameplay speed, transition được blend theo shortest rotation arc và board frame liền kề để phát hiện pop.
9. Immediate stop: thấy sai góc nhìn, body axis, tỷ lệ, đai, joint cover hoặc silhouette thì bác candidate tại đúng gate và sửa source/design. Không tách module, rig, chỉnh motion hoặc full capture trên design chưa đạt; không dùng motion/offset để che lỗi source.

## Visual acceptance của outfit #1

- Idle giữ hướng chạy phải và chỉ xoay nhẹ ra trước như bind authority đã duyệt; tỷ lệ đầu/thân/tay/chân ổn định.
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
{"activeTask":"LGO_RIGID_OUTFIT_PILOT_01","phase":"FINAL_OUTFIT1_BODY_AND_OUTFIT_JOINT_FORMULA_REVIEW","status":"NEED_HUMAN_VISUAL_REVIEW","method":"SHARED_PIVOT_DUAL_BODY_OUTFIT_CIRCULAR_CAP","sourceProfile":"lgo_character_canvas_1024x1536_v1","motionAuthority":"build/character-base-v3/male-old-base-six-pose-reference.png","currentDesignEvidence":"build/rigid-outfit-pilot/final-1-three-quarter-design-v5/slight-outward-rigid-cover-neutral-pair-v3.png","jointContract":"docs/art/LGO-RIGID-JOINT-AND-LAYER-AUTHORING-CONTRACT-v1.md","jointFormulaEvidence":"build/rigid-outfit-pilot/final-1-joint-authoring-v1/joint-contract-review.png","moduleTopology":"build/rigid-outfit-pilot/final-1-three-quarter-design-v5/outfit-module-topology-v2.json","currentSourceAuthorityValid":false,"bodyBindCalibrated":false,"layerAuthoringAllowed":false,"rigImplementationAllowed":false,"outfit2IntegrationAllowed":false,"classExpansionAllowed":false,"generatedMultiPoseAuthorityAllowed":false,"perPoseAssetsAllowed":false,"deformationAllowed":false,"animatedScaleAllowed":false}
```
