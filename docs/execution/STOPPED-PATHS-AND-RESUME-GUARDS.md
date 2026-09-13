# Stopped paths and resume guards — feature-2d-latest

Date: 2026-09-13

This file is a resume guard for the current `feature-2d-latest` sandbox. Read it together with `docs/execution/NEXT-ACTION.md` before doing character, outfit, rig, motion, Krita, Blender, GarmentCode, Comfy or Player work.

## Current allowed path

The active character/outfit route is the six-pose registered outfit pipeline:

- spec: `docs/art/LGO-SIX-POSE-REGISTERED-OUTFIT-PIPELINE-LOCK-v1.md`
- plan: `docs/superpowers/plans/2026-09-13-six-pose-registered-outfit-pipeline.md`
- body/motion authority: existing six poses `idle`, `run_contact_a`, `run_a`, `run_contact_b`, `run_b`, `jump_tuck`
- source-space profile: 1024×1536 canvas, `originX=512`, `groundY=1484`, `u=1.70/1536`

Next valid implementation work is source authoring for the 11 missing Pháp Lv1 slot/pose targets from `missing-source-authoring-brief-v1`, then grouped source board review. Do not pack Player before source coverage, provenance, alpha/canvas and visual source gates pass.

## Stopped path 1: current skeletal generated-cutout Player branch

Status: stopped.

Do not continue:

- `bind-authority-candidate-v1` Player probe
- animation tweaks on that cutout
- garment fitting on that cutout
- another capture that presents the same cutout as a candidate result

Reason:

- owner rejected the visual result because limbs/proportions read detached and unusable
- source discovery found no accepted neutral layered body/rig blueprint
- current artifacts are evidence for failure analysis only

Resume condition:

- a new neutral layered body/rig blueprint must exist, match Linh Giới proportions, include hidden surfaces/overlap ownership/joint evidence, and pass `docs/art/LGO-SKELETAL-2D-SOURCE-BLUEPRINT-SPEC-v1.md`
- if this condition is not met, do not reopen skeletal work in this sandbox

## Stopped path 2: flat 2D pattern/panel direct fit

Status: stopped as a production path.

Do not continue:

- direct affine fitting of flat GarmentCode/Blender panels onto six pose bodies
- treating exported panel PNG counts as production proof
- skin-color heuristic clipping as semantic occlusion authority

Reason:

- technical bridge proof ran, but visual/occlusion gate failed across all pose variants
- jump and limb/body overlap remain unresolved without semantic body-part masks, depth/rig ownership or garment weights

Resume condition:

- reopen only as `LGO_GARMENT_FAMILY_COMPILER_PILOT_01`, with a real LGO garment family, semantic occlusion/body-part mask or weights, A/B surface reuse, C geometry parameter change, and measured manual intervention
- it must remain a separate proof gate until Player review passes

## Stopped path 3: per-pixel garment nudging loop

Status: stopped.

Do not continue:

- repeated one-pixel or bbox-only tweaks per pose
- generating more garment candidates from the same failed assumption
- using runtime offsets or per-pose scale to hide source mismatch

Reason:

- owner feedback: this loop is slow, hard to audit and can discard days of work
- measurements are useful only when they become reusable source-space guides, gates or family templates

Resume condition:

- any repair must start from source-space body/slot measurements, accepted source/design evidence, and grouped board review
- after two repeated failures on the same assumption, record the root cause before making another candidate

## Stopped path 4: raw image/prompt candidate without numeric slot envelope

Status: stopped as a repeatable production method.

Do not continue:

- generating another outfit piece from a raw prompt/reference image and placing it directly on the six-pose body
- using alpha cleanup or background removal as proof that a generated layer is source-ready
- asking for owner review of a candidate that has not first passed a measured overlay check against the slot envelope

Reason:

- the `waist_belt/jump_tuck` ImageGen candidate produced cleaner style than the line/knot draft, but the overlay failed visual review because the belt scale and ribbons were much too large for the jump body
- alpha cleanup solved only the RGB/checkerboard artifact; it did not solve body-space fit, occlusion or long-term reuse
- repeating prompts without measured bounds returns to the slow trial loop the owner explicitly rejected

Resume condition:

- before creating or staging another generated/painted candidate, define the target slot envelope in the 1024×1536 source profile from body authority plus accepted neighboring pose/source evidence
- the candidate must pass an overlay board against that envelope before it can enter the repair source directories
- if the envelope cannot be defined for a slot/pose, keep that slot missing and move to a source-authoring method that can be measured

## Evidence that must not be promoted

The following evidence may be read, but not promoted to runtime-ready status:

- skeletal `bind-authority-candidate-v1`
- Blender/GarmentCode flat-panel direct-fit overlays
- Krita CLI export timeout result
- KRA archive mergedimage extraction
- `waist-belt-jump-imagegen-candidate-v1`
- source boards or guide boards marked authoring aid, review-only, partial, `SOURCE_VISUAL_FIX_REQUIRED`, or `runtimePromotionAllowed=false`

## Resume checklist

Before changing character/outfit work in this sandbox:

1. Read `docs/execution/NEXT-ACTION.md`.
2. Check this file for stopped paths.
3. Verify the work matches the six-pose plan or an explicitly opened proof gate.
4. Record the evidence path and status before any Player pack.
5. Never claim technical pass, executed tests, export count, or capture completion as visual acceptance.
