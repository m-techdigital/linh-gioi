# LGO skeletal 2D source blueprint spec v1

Date: 2026-09-13
Scope: common male body foundation for the `skeletal_2d` architecture benchmark. This spec is review-only until a source package passes the gates below.

## Why this spec exists

The previous generated cutout bind candidate was rejected by owner visual review. Its technical metrics were not enough because the Player result showed detached limbs, wrong proportions and unusable jump/flip anatomy. The next skeletal attempt must start from a better source body, not from more animation or garment tweaks.

## Required source package

A candidate package must contain:

- Native layered file: `.kra`, `.psb`, `.psd` or equivalent layer-preserving source.
- Full composite PNG on the project canvas: 1024x1536, top-left origin, `originX=512`, `groundY=1484`, `u=1.70/1536`.
- Neutral rest body with no baked optional outfit. Immutable base items, if any, must be declared explicitly.
- Six motion reference targets aligned to the existing gameplay set: `idle`, `run_contact_a`, `run_a`, `run_contact_b`, `run_b`, `jump_tuck`.
- Explicit joint-center JSON for crown, chin, neck, shoulders, elbows, wrists, hips, knees, ankles, heel/toe or foot endpoint policy.
- Body-part layers with real alpha, not checkerboard RGB: head/neck, torso/hips, near/far upper arm, near/far forearm/hand, near/far thigh, near/far shin/foot, hair/face if separated.
- Hidden surfaces where clothing or limb order can reveal them: under-arm torso edge, sleeve underlap area, inside leg overlaps, lower garment contact zones and foot/sole alternative surfaces.
- Draw order and ownership table stating which layer may own each visible region.

## Admission rejects

Reject before rig if any condition is true:

- Canvas, origin or ground differs from the source-space profile.
- A layer has no alpha channel, all opaque pixels, all transparent pixels, or baked checkerboard background.
- A body layer contains another slot's ownership, such as belt/robe/weapon pixels inside torso or limb layers.
- Rest body cannot visually match the accepted LGO body proportions at 1:1 source review.
- Jump pose requires per-pose scaling to look the same character.
- Candidate is only a dressed composite with reverse-cut inferred body parts.

## Measured gates

Before Unity runtime probe:

- Source admission report: every required layer passes canvas, alpha and ownership checks.
- Proportion board: idle/run/jump compared at the same source scale; head, torso, upper arm, forearm, thigh, shin and foot ratios stay within reviewed style bounds.
- Joint board: all joint centers visible or marked inferred with review radius; no shoulder/hip/knee/ankle outlier like the previous `run_b` collapsed shoulder.
- Bind fit report: root scale drift 0, body bone length drift <= 0.001 during normal gameplay motion, rigid foot/shin policy declared.
- Visual body review: body-only Player or equivalent sequence at real speed and slow review is not visibly detached, warped or proportionally wrong.

Passing these gates does not approve clothing. It only permits the skeletal benchmark to continue to garment weights, rigid attachments, unseen item and Player visual gates.

## Long-term use of measurements

The measurements are useful only when stored as reusable source contracts. They feed these later steps:

- shared skeleton and bind profile;
- garment family masks, weights and hidden-surface checks;
- SpriteLibrary or equivalent resolver categories;
- rigid socket validation for weapons, belt plates and ornaments;
- unseen-item cost comparison after template/tooling lock.

They must not become per-item offsets, bbox normalization, pose-specific scale correction, or manual pixel nudge instructions. If one new item needs fresh body measurements from scratch, the family/template method has failed and must be recorded before continuing.
