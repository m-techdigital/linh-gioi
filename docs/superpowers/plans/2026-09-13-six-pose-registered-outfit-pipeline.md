# Six-Pose Registered Outfit Pipeline Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Continue Pháp Lv1 outfit production on the existing six-pose body/motion authority, with level/variant mixing verified before Player promotion.

**Architecture:** Keep the current six full-body pose sprites as body/motion authority and register outfit layers to that source-space canvas. Source coverage, off-slot boards and mixed-loadout boards gate runtime packing; Player capture happens only after source gates pass.

**Tech Stack:** Python 3.12 source validators/composers, Krita 5.3.3 for native layer round-trip when editing source, existing Unity review packer/player capture path.

**Spec:** `docs/art/LGO-SIX-POSE-REGISTERED-OUTFIT-PIPELINE-LOCK-v1.md`

**Current execution status, 2026-09-13:** This is the active character/outfit path for `feature-2d-latest`. The current skeletal generated-cutout path is stopped and must not be resumed without a new accepted body blueprint.

## Global Constraints

- Canvas/profile stays 1024x1536, `originX=512`, `groundY=1484`, `u=1.70/1536`.
- Do not normalize each item or pose by bbox.
- Do not add runtime offsets or pose-scale corrections to hide source mismatch.
- Do not use the rejected skeletal generated-cutout source for garment fitting.
- Do not claim source coverage, technical tests or Player capture as visual acceptance.
- Keep runtime/UI/frozen surfaces unchanged until source gates pass.

---

### Task 1: Rebuild Current Coverage Snapshot

**Files:**
- Read: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/class-work-in-progress/phap-lv001/pose-matched-layer-authoring-v1/source-review-v1/pose-layer-coverage-v3.json`
- Write: `build/pose-matched-layer-authoring-v1/six-pose-active-coverage-v1.json`
- Modify: `docs/execution/NEXT-ACTION.md`

**Interfaces:**
- Consumes: existing Pháp source root and current body root.
- Produces: one active coverage report naming reusable slots and blocking gaps.

- [x] **Step 1: Run coverage audit on the existing source root**

Run:

```bash
PYTHONPYCACHEPREFIX=build/pycache python3.12 tools/audit_lgo_pose_layer_authoring_coverage.py \
  /Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/class-work-in-progress/phap-lv001/pose-matched-layer-authoring-v1 \
  --body-root /Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/class-work-in-progress/vo-lv001/legacy-base-run-contact-jump-v3-div4 \
  --output build/pose-matched-layer-authoring-v1/six-pose-active-coverage-v1.json
```

Observed: `SOURCE_COVERAGE_INCOMPLETE`, with `inner_top` and `class_accessory` reusable, and `outer_top`, `waist_belt`, `shoulder_chest_guard`, `jump_tuck` listed as blockers.

- [x] **Step 2: Compare with previous coverage**

Run:

```bash
python3.12 -m json.tool build/pose-matched-layer-authoring-v1/six-pose-active-coverage-v1.json >/dev/null
```

Observed: JSON parses; blockers match the previous coverage and `NEXT-ACTION.md` points at the active coverage evidence.

### Task 2: Source Repair Batch For Missing Outfit Slots

**Files:**
- External source root: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/class-work-in-progress/phap-lv001/pose-matched-layer-authoring-v1`
- Modify only within new candidate directories under that root.
- Write evidence under: `build/pose-matched-layer-authoring-v1/`

**Interfaces:**
- Consumes: six-pose body authority, current reusable `inner_top` and `class_accessory`, current idle candidates for `outer_top`, `waist_belt`, `shoulder_chest_guard`.
- Produces: source-review board for all six poses, with grouped failures and no runtime pack.

- [x] **Step 1: Create one batch brief before editing**

The brief must name the candidate directories, slot ownership to change, and the single board to review. It must explicitly avoid skeletal generated-cutout, polygon/body-mask floating panels, AI geometry authority and per-pose pixel nudging.

Observed 2026-09-13: `tools/plan_lgo_six_pose_source_repair_batch.py` generated `SOURCE_REPAIR_BATCH_READY` from active coverage. Evidence:

- `build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/repair-batch-plan.json`
- `build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/repair-batch-brief.md`
- External source brief mirror: `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1/class-work-in-progress/phap-lv001/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/repair-batch-brief.md`

Candidate directories are fixed as `outer-top-six-pose-source-repair-v1`, `waist-belt-six-pose-source-repair-v1` and `shoulder-chest-guard-six-pose-source-repair-v1`. Plan payload hash: `718db23e15c361781f7e828f802b5a232f8f14b2c97a3437cabd9b26d50970a3`.

- [ ] **Step 2: Repair source by slot group**

Edit source/native layers only enough to complete all six poses for `outer_top`, `waist_belt` and `shoulder_chest_guard`. Keep variants A/B within one accepted phom/template; if a level changes silhouette, declare it a new family instead of forcing reuse.

Prep evidence 2026-09-13: `tools/inspect_lgo_source_png_inventory.py` wrote `build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/current-input-image-inventory.json`. Existing idle/run candidates can be reused only as references: `preview-on-*` and review-board PNGs are full-alpha/RGB/composite preview files, not clean layer source. Do not pack these files directly; export new A/B front/back layer PNGs in the fixed candidate directories.

- [ ] **Step 3: Export and reopen/check native source**

Use Krita automation when native files are edited. Required evidence: save/reopen/export hashes, alpha checks and source path provenance. Expected: no source PNG has wrong canvas, missing alpha, fully opaque alpha or background leakage.

### Task 3: Mixed-Level And Off-Slot Review Board

**Files:**
- Use existing board/composer tools where possible.
- Write: `build/pose-matched-layer-authoring-v1/six-pose-mixed-loadout-review-v1/`

**Interfaces:**
- Consumes: accepted source exports from Task 2.
- Produces: review boards for full outfit, off-slot states and mixed level/variant combinations.

- [ ] **Step 1: Compose source boards before Player**

The board must include all six poses and enough combinations to reveal level/slot conflicts: full Lv1, A/B variant, removed `outer_top`, removed `waist_belt`, removed `shoulder_chest_guard`, and at least one mixed-level or mixed-variant combination.

- [ ] **Step 2: Record visual findings**

Write grouped findings: body/anatomy, outer, waist, guard, accessory, jump, level-mix. Expected: either `SOURCE_VISUAL_REVIEW_READY_FOR_PACK` or `SOURCE_VISUAL_FIX_REQUIRED`; no technical-only pass.

### Task 4: Review-Only Pack And Player Capture

**Files:**
- Existing packer/player capture path only after source board passes.
- Write evidence under: `build/pose-matched-layer-authoring-v1/six-pose-player-review-v1/`

**Interfaces:**
- Consumes: source board status `SOURCE_VISUAL_REVIEW_READY_FOR_PACK`.
- Produces: Player evidence for PC/tablet/mobile profile review without promotion.

- [ ] **Step 1: Pack review-only atlas**

Run the current packer with source manifests that have no reject marker and no runtime offset/pose-scale correction. Expected: review atlas/manifest records source hashes and `runtimePromotionAllowed=false`.

- [ ] **Step 2: Run targeted tests and Player capture**

Run only gates affected by this outfit path, then one Player build/capture batch. Required: EditMode targeted tests, JSON parse, no frozen surface diff, Player screenshots/video reviewed manually.

- [ ] **Step 3: Update state and checkpoint**

Update `NEXT-ACTION.md`, `PROJECT-STATE.md`, `TASK-LEDGER-ROLLUP.md` and `build/codex-autopilot/status.json` with result, build/capture counts, evidence paths, failures and next action. Commit only after verification.
