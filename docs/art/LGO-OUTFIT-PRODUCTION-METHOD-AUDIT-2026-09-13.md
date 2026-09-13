# LGO Outfit Production Method Audit — 2026-09-13

## Verdict

The current Pháp Lv1 six-pose repair batch has useful measurement and source-board tooling, but the work loop is not yet production-scalable. Continuing to make visual candidates by image/mask heuristics per pose would repeat the rejected manual pixel loop. It may close one board after many attempts, but it does not create a reusable production system for 10 items, multiple levels, variants, classes, or additional poses.

As of this audit, completed final items remain `0`. Current useful evidence is source-layer coverage, not final art acceptance: `outer_top`, `waist_belt`, and `shoulder_chest_guard` can all be present as 24/24 A/B front/back six-pose layers, and mixed/off-slot boards can be generated without missing layers. The visual blocker is still model-level: the idle Pháp `outer_top` source includes white sleeves while the run/jump body authority is bare-arm, so fitting or deleting sleeve pixels is treating a body/slot ownership mismatch as an art cleanup problem.

## Evidence from the local attempts

- Sleeve-add candidate v1: rejected. It used skin-region masks and produced large rectangular white patches; slot envelope failed 5/5 run/jump targets.
- Sleeve capsule candidate v2: rejected. It improved shape but still read as detached white bands and failed the current `outer_top` envelope 5/5.
- Sleeveless unified v2: build-only visual looked more consistent, but it was still created by heuristic idle pixel removal rather than a declared product/body authority decision. It was restored out of the active repair source and preserved only as evidence at `build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/outer-top-sleeveless-unified-style-v2/visual-method-review-v1.json`.

These attempts show that the bottleneck is not tool availability. The bottleneck is missing semantic contract: which surface owns sleeves, which body pixels are allowed to be hidden, and whether Pháp Lv1 is visually sleeved or sleeveless across locomotion.

## External pipeline comparison and applied rules

| Source checked | What the source supports | Applied LGO rule | Enforced by |
| --- | --- | --- | --- |
| Spine mix-and-match skins: skins reuse one skeleton animation with different attachment sets, runtime can combine item skins, and template placeholders are recommended for customizable attachments. Source: https://esotericsoftware.com/spine-unity-mix-and-match | Equipment is not a loose PNG; it is a named attachment/skin entry under slots/placeholders. | Every outfit item family must declare `slotId`, `familyType`, ownership and route before source authoring. | `tools/validate_lgo_outfit_surface_contract.py` rejects unknown slot/family and missing ownership. |
| Unity 2D Animation Sprite Swap: Sprite Library uses Category/Label, Sprite Resolver requests by Category/Label, and skeletal sprite swaps require an identical skeleton. Source: https://docs.unity3d.com/Packages/com.unity.2d.animation@15.0/manual/SpriteSwapIntro.html | Runtime swap is category/label/skeleton driven; mixing renderer sprite animation and resolver animation can conflict. | LGO must not pack a source candidate until the source-space profile, pose set and route are declared. | Contract requires the fixed 1024×1536 profile, six-pose set and `runtimePromotionAllowed=false`. |
| Live2D deformers: manual vertex-by-vertex work is expensive; deformers move groups together, but large warp/rotation can shrink or distort under the wrong interpolation. Source: https://docs.live2d.com/en/cubism-editor-manual/deformer/ | Deformation must be part of the model hierarchy; preview warping is not reliable proof. | `cloth_body` items such as sleeves require explicit body-part ownership/masks before drawing; no pixel-polish loop. | Contract route `SLEEVED_PHAP_LV1` fails without `upper_arm_cloth` ownership and per-pose masks. |
| Godot 2D skeletons: parts are arranged in a node hierarchy and assigned to a `Skeleton2D`/`Bone2D`; visual order must be handled explicitly. Source: https://docs.godotengine.org/en/stable/tutorials/animation/2d_skeletons.html | Motion reuse depends on named hierarchy and draw order, not just image dimensions. | LGO must treat shoulder guard, torso cloth, waist plate and arm cloth as separate ownership surfaces. | Contract separates `outer_top`, `waist_belt` and `shoulder_chest_guard` family rules; Player pack stays blocked until contract PASS. |

These are official tool/game-engine documentation lessons, not a claim that LGO should immediately migrate to Spine, Unity SpriteSkin, Live2D or Godot. The actual applied decision is narrower: stop asset generation until LGO has a contract that gives the same kind of slot/attachment/ownership structure.

## Scalable direction for LGO

Before making another item candidate, define a `Character Outfit Surface Contract` for the six-pose path:

1. Body authority per class/pose: the exact body image or rig source, its visible skin/clothing base, and whether the base already contains inner clothing.
2. Slot ownership: for each slot, declare whether it owns torso, shoulder, upper arm, forearm, waist, hip cloth, back cloth, rigid ornament, weapon, hair, or accessory.
3. Occlusion masks: body regions that a slot may cover, must not cover, or must replace. Sleeves require `upper_arm_cloth` ownership; they should not be squeezed into a torso-only `outer_top` envelope.
4. Item family type:
   - `rigid`: one source plus transform/attachment points, e.g. gem, badge, weapon, charm.
   - `part-rigid`: a few pieces attached to shoulders/waist/limbs, e.g. guard, belt plate.
   - `cloth-body`: requires body-part masks/occlusion and likely per-pose silhouettes, e.g. shirt, sleeves, pants overlay.
   - `pose-authored`: only for items whose silhouette changes strongly per pose.
5. Gate order: contract → generated source-board from contract → mixed/off-slot board → review-only Player. No Player pack from source count alone.

## Production estimate

If we keep the current per-pose visual-candidate loop, one item can take days to a week and still be rejected; 10 items can take months; more poses multiply the work. That path is not acceptable.

If the contract above is created first, a realistic target for early production is:

- Rigid/part-rigid item: hours after the attachment/mask contract exists.
- Cloth-body item: roughly 0.5–2 days per item family for a first class because masks and ownership need source review.
- New level variant with same family: much faster, usually palette/material/detail changes plus board review.
- New pose: one shared body/slot/mask update, then regenerate boards for all affected items; no manual resize loop per item should be allowed.

This only becomes true after the contract and generators exist. Until then, claiming item completion from board count would be a false pass.

## Decision gate before further implementation

Do not continue `outer_top` pixel/mask polish. The next valid task is to write and test the surface contract for Pháp Lv1, then choose one of two explicit product routes:

- `SLEEVELESS_PHAP_LV1`: body authority remains bare-arm across all six poses; idle outer source is edited from a native/source decision, not heuristic pixel deletion.
- `SLEEVED_PHAP_LV1`: add `upper_arm_cloth` ownership and masks/attachments for every pose before drawing sleeves.

Either route must produce a board that can be reviewed as a design result, not as a technical pass.
