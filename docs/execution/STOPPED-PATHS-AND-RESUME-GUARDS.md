# Stopped paths and resume guards — feature-2d-latest

Date: 2026-09-13

This file is a resume guard for the current `feature-2d-latest` sandbox. Read it together with `docs/execution/NEXT-ACTION.md` before doing character, outfit, rig, motion, Krita, Blender, GarmentCode, Comfy or Player work.

## Current allowed path

The only active character route is `LGO_RIGID_OUTFIT_PILOT_01`:

- Unity Transform hierarchy + ordinary rigid `SpriteRenderer`; one fixed FitProfile per body variant/item.
- Current gate: `FINAL_OUTFIT1_CANDIDATE_READY_FOR_OWNER_REVIEW / NEED_HUMAN_VISUAL_REVIEW`.
- Final #1 self-review evidence is `build/rigid-outfit-pilot/final-1-review`; owner acceptance is still required.
- Stop here for owner review; do not mutate source/motion or start another capture without a concrete owner finding.
- Outfit #2, mix-and-match, class/level expansion and migration remain locked until outfit #1 visual PASS.

Recurrence guard:

- Do not resume the Blender weighted-mesh source as rigid authority; its report explicitly says `armSkinning=two_bone_weighted_mesh`.
- Do not fix the rejected Player by per-frame art, per-pose offsets, sprite swap, scale, SpriteSkin, mesh or capsule/flat-card proxies.
- Do not treat validator/checkpoint PASS as visual acceptance.
- Review the whole board, batch issues by source/pivot/overlap/motion/render order, and make one grouped correction. A structural error repeated in two captures rejects that topology/source hypothesis.
- The generated red armor sheet is `REJECTED-NOT-INTEGRATED` because Phase 8 started before outfit #1 passed; do not copy it into Resources or rename it into an active outfit.

## Stopped path 0: automated base redraw without identity and pose lock

Status: stopped after bounded visual failures on 2026-09-14.

Do not continue:

- Blender mannequin renders as final Linh Giới body art;
- ImageGen whole-sheet redraw from the old base;
- ImageGen per-frame “conservative edit” claimed as identity preservation;
- renaming either rejected batch into a new character-base candidate.

Reason:

- Blender preserved numeric segment lengths but failed the established 2D design, silhouette and joint appearance;
- whole-sheet ImageGen repeated A/B limb phases and changed the character identity;
- direct single-frame ImageGen changed face, stance and anatomy even with the original frame as its only reference.

Resume condition:

- demonstrate one `idle` frame with the old design identity and source proportions locked by an artist-controlled native edit or verified structure-control plus identity-conditioning workflow;
- compare the actual image visually before any remaining pose generation;
- evidence paths: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/rejected-evidence/2026-09-14/common-character-blender-mannequin-six-pose-v1/` and `common-character-imagegen-base-removal-drift-v1/`.

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

### Rejection extension — Blender flat-card/skinned body rig, 2026-09-13

Status: stopped by owner. This is the same rejected split-body premise, not a new production route.

Do not continue or rename:

- `OUTFIT_BODY_RIG_SOURCE_PROTOTYPE`
- Blender body cards driven by rigid bones or skinned meshes
- a Player probe assembled from separate head, torso, arm and leg cards
- garment reuse claims measured on that body

Reason:

- both the rigid and skinned variants displayed detached body regions and unstable proportions
- changing vertex weights did not correct the source topology; it only changed how the same cut pieces deformed
- owner explicitly returned this sandbox to whole-body six-pose outfit authoring

Rejected evidence to retain:

- `build/outfit-body-rig-prototype-2026-09-13/runtime-resynced-v2/`
- `build/outfit-body-rig-prototype-2026-09-13/runtime-skinned-v2/`
- `build/outfit-body-rig-prototype-2026-09-13/blender-action-renders-skinned-v2/`

This path has no automatic resume condition. Reopening it requires a new explicit owner architecture task; the existing prototype, mesh, weights and Player scene remain evidence only.

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

## Stopped path 5: ImageGen export or rejected design as source

Status: stopped as a production-source route.

Do not continue:

- treating an ImageGen PNG as a registered garment layer, including requests for a transparent background;
- repairing an RGB/checkerboard output into source through alpha cleanup, pixel deletion or background removal;
- selecting any file below `rejected-evidence` or a source tree marked `REJECTED`/`WITHDRAWN`;
- copying or renaming rejected pixels into a new candidate directory.

Reason:

- the jump-layer capability check returned a 1024×1536 RGB file with checkerboard baked into the pixels;
- prompt wording did not guarantee a true alpha channel or editable layer ownership;
- rejected and active WIP previously shared one discoverable tree, so `runtimeEligible=false` alone did not prevent later source reuse.

Enforcement:

- final rejected/withdrawn trees are preserved outside selectable roots under `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/rejected-evidence/2026-09-13/`, with per-file hashes in `quarantine-manifest.json`;
- former paths contain only `DO-NOT-SELECT.md` and an authoring-selection tombstone;
- `pack_lgo_pose_review_atlas.py` rejects rejected/withdrawn ancestors, quarantine paths, PNGs without explicit alpha and fully opaque PNGs at its mandatory pack boundary;
- `audit_lgo_source_staging_selection.py` reads both material provenance and authoring-selection status.

Resume condition:

- no resume as a direct source route. A concept may be redrawn in one native layered authoring source and must pass reopen/export verification before it enters staging.

## Evidence that must not be promoted

The following evidence may be read, but not promoted to runtime-ready status:

- skeletal `bind-authority-candidate-v1`
- Blender/GarmentCode flat-panel direct-fit overlays
- Krita CLI export timeout result
- KRA archive mergedimage extraction
- `waist-belt-jump-imagegen-candidate-v1`
- source boards or guide boards marked authoring aid, review-only, partial, `SOURCE_VISUAL_FIX_REQUIRED`, or `runtimePromotionAllowed=false`

## Current native-authoring capability blocker

Krita 5.3.3 GUI launches, but this Codex sandbox has no desktop/Scripter control. Bounded automation checks are closed:

- user-resource/PYTHONPATH `kritarunner` could not import the module;
- bundle injection required modifying a signed app and macOS blocked the copied runner/signing path;
- the documented CLI export form timed out after 30 seconds and wrote no PNG while another Krita GUI process was active.

Do not continue trying module locations, writable app copies, ad-hoc signing or longer export waits. Resume native authoring only with a controllable Krita GUI/Scripter session or a newly supplied layered source that opens and saves in Krita. This blocker concerns source creation; it does not weaken the alpha, quarantine or visual Player gates.

## Resume checklist

Before changing character/outfit work in this sandbox:

1. Read `docs/execution/NEXT-ACTION.md`.
2. Check this file for stopped paths.
3. Verify the work matches the six-pose plan or an explicitly opened proof gate.
4. Record the evidence path and status before any Player pack.
5. Never claim technical pass, executed tests, export count, or capture completion as visual acceptance.
