# LGO pose-matched clothing pipeline decision v1

Date: 2026-09-13
Scope: Pháp nam Lv1 sandbox in `feature-2d-latest`, review-only until gates pass.

Method selection is now locked in `docs/art/LGO-POSE-MATCHED-CLOTHING-METHOD-SELECTION-v1.md`: use registered template stack as the next effective method. Do not resume asset work through AI-first, runtime-first, polygon, body-mask, or pixel-nudge loops.

## Problem observed in this sandbox

The first implementation loop produced useful provenance and validators, but not a clear player-facing result. `outer-top-run-four-material-v1/v2` both failed visually because hand-placed polygons looked like floating armor plates instead of cloth attached to the torso. `outer-top-run-four-material-v3-bodymask` attached better to the source silhouette, but generated sleeve/collar artifacts. These are not acceptable as production artwork or as proof that the pipeline works.

The main issue is workflow shape: fixing one slot or one pose at a time hides the real dependency between body, outer garment, belt, shoulder guard, and occlusion. The next pipeline step must work at stack level.

Owner feedback on 2026-09-13 also exposed a measurement problem: repeated pixel nudges cannot converge if the pose landmarks and body proportions are not measured first. The pipeline now treats anchor measurement as a required source gate before more garment candidates are created.

## Sources checked

Internal:

- `docs/art/LGO-CLASS-2D-MODULE-STANDARD-v1.0.md`
- `docs/art/LGO-2D-EQUIPMENT-COMPATIBILITY-CONTRACT-v1.md`
- `docs/07-ART-BIBLE.md`
- `docs/02-GDD.md`
- owner prompt attachments for the Krita/Comfy/Spine pipeline

External primary sources:

- Unity 2D Animation Sprite Swap/Sprite Library docs: swap works through matching Category/Label parts and Sprite Resolver setup; it is a runtime/library mechanism, not a generator of missing garment art. Source: <https://docs.unity3d.com/Packages/com.unity.2d.animation@10.0/manual/ex-sprite-swap.html>.
- Spine skins docs: wardrobe systems reuse animations by prepared skins/attachments and can mix multiple item/body-part skins; setup efficiency comes from placeholders, naming, linked meshes, and programmatic skins, not from per-pixel runtime correction. Source: <https://esotericsoftware.com/spine-skins>.
- Spine linked mesh docs: linked meshes share vertices, UVs, weights, and optionally deformation timelines, so reuse only works when the item has the same structure.
- Spine runtime repacking docs: combining skins can increase materials/draw calls; repacking has texture/import constraints and generated assets must be managed.
- Godot cutout docs: cutout/skeletal deformation helps animate prepared pieces; it does not solve ownership, occlusion, or missing art by itself. Source: <https://docs.godotengine.org/en/stable/tutorials/animation/cutout_animation.html>.
- Krita Python docs confirm scripting is an intended automation route, but each destructive/non-destructive document operation still needs save/reopen/export verification in our installed version. Source: <https://docs.krita.org/en/user_manual/python_scripting.html>.
- MapleStory Worlds creator docs were checked as a production reference for avatar item registration/action discipline, but public docs do not prove its internal art pipeline.

## Decision

Use a stack-first template pipeline:

1. **Template contract first.** Define per slot what geometry family it belongs to: torso cloth, belt wrap, rigid chest/shoulder guard, rigid accessory. Each family has canonical anchors, occlusion rules, and pose coverage. Variants can reuse only within the same family.
2. **Measured anchors before pixel edits.** Use `tools/measure_lgo_pose_garment_anchors.py` on the shared landmark guide to produce neck, shoulder, hip, torso-axis, chest, waist, belt-line, and shoulder-line measurements. The output is review data only; it must not be used as automatic warp, per-item runtime offset, or source approval while guide status remains draft.
3. **Outer_top drives the run stack.** For the four run poses, no belt or guard adjustment is accepted until `outer_top` is readable as cloth on the body. If outer_top fails, downstream fit work is discarded or held as body-only draft.
4. **Four-pose stack board is the minimum working batch.** The next review artifact must show `run_contact_a`, `run_a`, `run_contact_b`, and `run_b` with body + outer_top + waist_belt + shoulder_chest_guard in one board. Single-layer boards are diagnostic only.
5. **Jump remains excluded until anatomy is accepted.** Do not author final clothing for `jump_tuck` while the body proportion/candidate issue remains open.
6. **AI is surface assistance, not geometry authority.** AI can fill texture/trim inside masks after phom/lineart/occlusion is accepted. It must not decide collar, sleeve, belt, or shoulder placement.
7. **Runtime remains Unity's existing review path.** Do not switch runtime to Spine or Godot. Spine may be explored later as an offline authoring tool only if template/linked-mesh reuse has a working small proof.

## Rejected working patterns

- Drawing large garment polygons directly on body poses and iterating by eye.
- Nudging individual pixels/contours before landmark measurements are sane.
- Judging a slot by a body-only preview when that slot is layered over another item.
- Treating Krita KRA/export pass as visual acceptance.
- Treating one idle board as proof of pose-matched clothing.
- Creating more validators or candidate folders when the next question is visual stack coherence.

## Anti-repeat gate

Before creating another garment candidate, the agent must state which failed method family it is avoiding and what new evidence makes the next attempt different. The blocked method families in this sandbox are:

- hand-drawn torso polygons adjusted by eye;
- large body-mask fills that inherit silhouette without garment structure;
- whole-patch AI/composite outputs that change canvas placement or bring background/glow into alpha;
- contour or pixel nudges before source-space anchors pass sanity.

If the next action is still inside one of those families, stop asset generation. The only acceptable continuation is a measurement/body-guide fix, a stack-level board based on reviewed anchors, or a different authoring proof with its own measurable contract. This rule exists because repeated small candidates already consumed time without producing a player-checkable result.

Each batch must also record an efficiency audit: intended review artifact, candidate count by method family, failures grouped by root cause, reusable evidence, gates rerun and why, and the condition that proves the next attempt is not a repeat. This audit is part of the pipeline, not an optional report after the owner notices wasted work.

## Planning gate

Run `tools/plan_lgo_pose_pipeline_next_action.py` before any asset candidate. It combines measurement sanity and method-repeat audit:

- `FIX_GUIDE_BEFORE_ASSET`: stop asset authoring, fix/review landmark or body source, then rerun measurement overlay.
- `CHANGE_METHOD_BEFORE_ASSET`: stop asset authoring because the proposed method repeats a locked failure family.
- `RUN_STACK_BOARD_ALLOWED`: create the four-pose stack board, then review grouped failures.

The current sandbox evidence returns `FIX_GUIDE_BEFORE_ASSET` because `run_b` shoulder measurement remains an outlier. This is expected and protects the pipeline from resuming pixel tweaks under a new folder name.

## Next concrete batch

Produce **one run stack review batch**:

- Inputs: current body authority, reviewed anchor measurements, `outer_top` idle v2 as color/style reference only, `waist_belt` run draft v1 as provisional overlay, `shoulder_chest_guard` idle v4 as shape reference only.
- Output: one board for four run poses with body + outer_top + belt + guard, plus a report listing grouped failures.
- Acceptance for the batch: the board clearly shows whether the method can produce cloth attached to the torso using measured anchors. If the guide still has anomalies such as the current `run_b` shoulder width outlier, fix the guide/body source first. If outer_top still reads as floating or body-mask artifact after measurement sanity, stop asset generation and choose a different authoring method, such as manual Krita source-paint over pose screenshots or a Spine offline mesh proof.

This decision is a guardrail against endless small candidate loops. Future sessions should not resume by creating `outer_top` v4/v5 from the same polygon/body-mask assumptions.

## Superseded by character source gate — 2026-09-13

After the skeletal Player probe, owner rejected the generated cutout body result visually. The run stack batch above remains useful as source-space garment evidence, but it is no longer the next action while the body foundation is rejected. Current planner status is `AUTHOR_SKELETAL_2D_SOURCE_BLUEPRINT`; garment fitting resumes only after a new neutral layered body/rig blueprint passes anatomy/proportion/source admission gates. Do not use the old six-pose baseline or the rejected skeletal cutout as proof of the new method.
