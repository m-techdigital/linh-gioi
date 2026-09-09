# Vo 2D Base Rig And Animation Spec v1.0

Date: 2026-09-09. Scope: class Vo only, male and female. This document defines the base body and motion contract used to test Vo equipment across levels. It is a draft art/rig contract, not a runtime implementation and not a production sprite claim.

## Goal

Vo equipment must fit one stable male base and one stable female base across every level from `lv001` to `lv100`. The base proportions, joint positions, side-view camera, and slot anchors stay fixed. Cross-level mixing is allowed: any item from any Vo level must be testable on the same gender base without moving body landmarks.

## Base Character Contract

Both bases use side-view facing right as the primary production view. The neutral pose has feet on the same ground line, head upright, arms relaxed beside the body, and hands visible enough to check gauntlet and arm-guard separation. The base keeps modest grey training clothes when unequipped so remove-all equipment is still presentable in SCN-001/002.

Male base: athletic martial build, not bulky, short black default hair, grey sleeveless training top, loose grey cropped martial pants, low simple shoes or socks depending on rig test. Female base: agile athletic build, high ponytail default hair, grey/black training top, practical shorts/lower garment, low closed footwear or socks depending on rig test. No level changes body size, age, height, posture, or face.

Equipment overrides default visible clothing by slot, but does not replace the skeleton. When a slot is empty, the base fallback remains visible. `head_hair` may replace the default hair layer; all other equipment must avoid hair, face, skin, torso, fingers, and feet.

## Anchor Contract

Every equipment candidate must be checked against these stable anchors:

- `head_hair`: scalp cap, front bang zone, back hair zone, ponytail swing root for female.
- `inner_top`: neck opening, chest center, rib side seam, waist hem.
- `outer_top`: shoulder seam, chest overlap, side hem, rear cloth overlap.
- `lower_body`: hip line, crotch clearance, knee bend allowance, ankle hem.
- `waist_belt`: pelvis center, left sash tail root, right sash tail root, charm hang point.
- `shoulder_chest_guard`: near shoulder cap, far shoulder hint, chest strap center, rear strap exit.
- `arm_guard`: elbow-side opening, forearm center, wrist-side opening before the hand.
- `main_weapon`: wrist joint, knuckle center, palm clearance, left/right mirror check.
- `footwear`: heel point, toe point, ankle collar, sole ground clearance.
- `class_accessory`: belt charm point or chest emblem point; one active mount per item.

Item art may include front/back pieces inside the same slot file or sheet, but the ownership still belongs to one slot. Sorting order is a rig decision, not the slot ID number.

## Cross-Level Fit Rule

All Vo level items share the same bounding landmarks per gender. Higher levels may add trim, plates, cloth tails, and aura-ready silhouette, but they cannot move the anchor point or require a wider base stance. Cloth tails may extend beyond the base silhouette if they do not hide feet, hands, face, or action readability.

Cross-level smoke for each gender must include:

1. `lv001` full set on base.
2. `lv050` full set on the same base.
3. Mixed loadout A: `lv001` inner/lower/footwear with `lv050` weapon, arm guard, belt, shoulder/chest guard, and accessory.
4. Mixed loadout B: `lv050` inner/lower/footwear with `lv001` weapon, arm guard, belt, shoulder/chest guard, and accessory.
5. Empty slot pass: remove each slot one at a time and confirm fallback base/clothing remains coherent.

If a higher-level item only fits by scaling the body or moving joints, the item fails and must be redrawn.

## Motion Test Set

Before runtime import, every approved Vo item set must be previewed on a small side-view motion sheet:

- `idle`: neutral stance, breathing overlap, hair and sash rest pose.
- `walk`: one contact frame and one passing frame.
- `run`: compressed torso, rear leg push, front leg reach, arm counter-swing.
- `jump_start`: knees bent, fists ready, sash/hair lag down.
- `jump_air`: body extended, legs tucked lightly, hair/sash trailing.
- `fall`: downward read, feet clear, belt/accessory not covering legs.
- `land`: crouch impact, boots on ground line, shoulder/chest guard clear.
- `basic_attack`: short punch extension, main weapon follows hand, arm guard remains on forearm.
- `skill_windup`: grounded charge pose, no baked aura on equipment.
- `skill_cast`: punch or palm strike silhouette with VFX as a separate layer.
- `skill_recover`: return to stance, cloth and ponytail settle.

This motion sheet proves attachment and readability only. It does not define damage, cooldown, combo timing, server authority, combat balance, or unlock level.

## Draft Evidence From This Batch

Generated preview drafts were created as visual direction only and kept under build output:

- `build/class-2d-review/generated-drafts/vo_male_lv001_sideview_module_demo_draft_v1.png`
- `build/class-2d-review/generated-drafts/vo_female_lv001_sideview_module_demo_draft_v1.png`
- `build/class-2d-review/generated-drafts/vo_base_rig_motion_sheet_draft_v1.png`
- `build/class-2d-review/generated-drafts/vo_male_female_motion_compatibility_draft_v1.png`
- `build/class-2d-review/generated-drafts/vo_male_cross_level_fit_draft_v1.png`
- `build/class-2d-review/generated-drafts/vo_female_cross_level_fit_draft_v1.png`
- `build/class-2d-review/generated-drafts/vo_male_base_transparent_candidate_v1.png`
- `build/class-2d-review/generated-drafts/vo_female_base_transparent_candidate_v1.png`
- `build/class-2d-review/generated-drafts/vo_male_base_transparent_candidate_v1_white_check.png`
- `build/class-2d-review/generated-drafts/vo_female_base_transparent_candidate_v1_white_check.png`
- `build/class-2d-review/generated-drafts/vo_male_lv001_10slot_detached_sheet_candidate_v1.png`
- `build/class-2d-review/generated-drafts/vo_female_lv001_10slot_detached_sheet_candidate_v1.png`
- `build/class-2d-review/generated-drafts/vo_male_lv001_10slot_detached_sheet_extraction_failed_alpha_v1.png`

Visual read: the male/female level-001 drafts improve side-view readability and level-001 silhouette. They are not clean production modules. The assembled characters still show body parts inside worn gauntlets because they are outfit previews, and some detached module shapes need stricter hollow openings before production extraction. The first base rig/motion draft is useful for anchor and motion discussion, but the female base drifts toward long pants instead of the preferred agile shorts/lower garment. The second motion compatibility draft corrects the female shorts direction and shows male/female rows for movement, jump, punch, and skill poses, but it is still not a measured sprite sheet. The male/female cross-level fit drafts show `lv001`, `lv050`, and mixed silhouettes on a stable side-view base; they are useful for approving proportions and anchor direction, not for extracting runtime layers. The male/female base candidates contain alpha and pass a white-background visual check as draft cutouts, but they still need measured anchors, matching scale, and detachable equipment before runtime import. The male/female `lv001` ten-slot sheets are useful shape references, but both baked a checkerboard background; the attempted extraction produced a dark background, so these sheets fail the transparent item gate. Keep all drafts as art direction references, not import candidates.

## Acceptance Gate

The next clean asset gate is a transparent-background candidate pack for male/female `lv001` and `lv050`: base body, ten detached equipment slots, full-equip preview, two mixed-level previews, and the motion test set above. A reviewer must inspect hand/forearm, hair/head, inner/outer/shoulder, lower/belt, boot/foot, and accessory ownership before expanding to all 11 levels.

Non-claims:

- no gameplay implementation
- no production art claim
- no runtime asset claim
- no protocol changes
- no GameData schema changes
