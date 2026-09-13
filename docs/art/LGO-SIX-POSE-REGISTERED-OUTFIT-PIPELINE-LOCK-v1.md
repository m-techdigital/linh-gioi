# LGO six-pose registered outfit pipeline lock v1

Date: 2026-09-13
Scope: `feature-2d-latest` pose-matched clothing sandbox after owner rejected the current skeletal cutout result.

Resume guard: `docs/execution/STOPPED-PATHS-AND-RESUME-GUARDS.md`.

## Active path

Use the six-pose registered outfit pipeline as the active production route for this sandbox. The six existing pose sprites are the runtime/body/motion authority for now:

- `idle`
- `run_contact_a`
- `run_a`
- `run_contact_b`
- `run_b`
- `jump_tuck`

All outfit work must stay on the project source-space profile: canvas 1024x1536, `originX=512`, `groundY=1484`, `u=1.70/1536`. Do not normalize each item or pose by bbox, and do not use runtime offsets to hide source mismatch.

## Stopped path

Do not continue the current skeletal 2D generated-cutout path in this sandbox. The stopped artifacts remain evidence only:

- `bind-authority-candidate-v1` is `OWNER_REJECTED_VISUAL` and has `DO-NOT-PACK.md`.
- `skeletal_2d` planner status is `AUTHOR_SKELETAL_2D_SOURCE_BLUEPRINT`, but no accepted blueprint exists.
- `neutral-training-body-native-v1/v2/v3`, canonical ORA and `jump-anatomy-candidate-v1` are not enough to resume skeletal runtime work.

Only reopen skeletal 2D if a new accepted neutral layered body/rig blueprint exists and passes `LGO-SKELETAL-2D-SOURCE-BLUEPRINT-SPEC-v1.md`. Do not create another Player probe, animation tweak, overlap fix or garment fit on the rejected cutout source.

## Required result for the six-pose path

The next player-checkable result is a registered outfit review pack, not another isolated source candidate. It must show:

- Pháp Lv1 outfit pieces registered across all six poses.
- Mixed equipment levels/variants on the same six-pose body without pose-local scale or runtime offset.
- At minimum: `inner_top`, `outer_top`, `waist_belt`, `shoulder_chest_guard` and one rigid accessory/item.
- A/B surface or level variant reuse only when it shares the same accepted phom/template; otherwise it is a new family and needs its own fit gate.
- Off-slot and mixed-loadout boards before Player pack.
- Player capture only after source coverage, visual source review and pack gate pass.

## Current source state

Reusable evidence:

- `inner_top` and `class_accessory` have six-pose A/B native exports.
- Krita round-trip and hash/provenance mechanics have useful evidence.
- Old six-pose runtime remains a visual/motion baseline.

Blocking source gaps:

- `outer_top` is idle-only/current review candidate; collar, hem and ownership remain unresolved.
- `waist_belt` has idle plus four run draft poses, but no accepted A/B six-pose source.
- `shoulder_chest_guard` is idle-only/current review candidate.
- `jump_tuck` anatomy and outfit fit remain high-risk and must be reviewed before pack.

## Guardrails

Do not return to these stopped loops:

- skeletal generated-cutout runtime probe;
- per-pose pixel nudging without source review;
- polygon/body-mask garment candidates that already failed as floating panels;
- AI/composite geometry authority;
- claiming technical test/capture pass as visual acceptance.

If the six-pose path also fails to produce a coherent source board after a bounded batch, record the root cause and switch to a higher-level product decision. Do not open another architecture/tool branch just to keep moving.
