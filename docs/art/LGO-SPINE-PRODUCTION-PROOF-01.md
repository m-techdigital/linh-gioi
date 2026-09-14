# LGO Spine production proof 01

Date: 2026-09-14

Task: `LGO-SPINE-PRODUCTION-PROOF-01`

Current phase: `TOOLCHAIN_PREFLIGHT`
Current decision: `BLOCKED_SPINE_TOOLING`

## Decision and verified basis

Spine is a credible production candidate for LGO, but it is not accepted merely from documentation. The official material verifies the mechanisms needed by this proof:

- spine-unity 4.3 supports Unity 2017.1 through 6000.4; LGO uses Unity 6000.3.2f1;
- the official `Mix and Match Skins` example combines separate item skins on one skeleton and animation set;
- the official template-attachment workflow is intended for large or downloadable equipment catalogs and requires real item images to fit a compatible template family;
- Spine Professional provides meshes, weights and IK; Essential cannot save/export projects that use Professional features;
- a valid Spine license is required to integrate Spine Runtimes into a product.

Official references:

- https://esotericsoftware.com/spine-unity-download/
- https://esotericsoftware.com/spine-unity-installation
- https://esotericsoftware.com/spine-unity-mix-and-match
- https://esotericsoftware.com/spine-examples-mix-and-match
- https://esotericsoftware.com/spine-purchase

These facts prove capability and version compatibility. They do not prove that the current LGO character source, Pháp Lv1 sleeves or run silhouette will deform acceptably. That conclusion requires the staged Player proof below.

## Scope

Unity remains responsible for gameplay, input, network, collision, UI and scene presentation. Spine/spine-unity is evaluated only as the character visual, animation and modular-equipment authoring/runtime layer.

The proof is limited to:

- official Mix-and-Match reference reproduction;
- Pháp Lv1 male and female;
- shared semantic skeleton and motion timeline;
- one rigid item, one segmented item and sleeved `outer_top`;
- idle, complete run cycle, jump/tuck, transitions, stress motion;
- equip, unequip and one mixed loadout in a real Unity Player.

No other class, level or complete ten-slot catalog may be converted before acceptance.

## Lifecycle gates

1. `DISCOVER`: classify current source/runtime/evidence.
2. `REFERENCE_REPRODUCTION`: run the official sample unchanged in LGO Unity.
3. `LGO_RIG_FOUNDATION`: create one shared semantic skeleton in licensed Spine.
4. `LGO_BODY_PROOF`: substitute accepted Pháp male, then female.
5. `EQUIPMENT_PROOF`: prove rigid, segmented and sleeved surfaces.
6. `MOTION_PROOF`: build smooth run, jump, IK contact and stress animation.
7. `RUNTIME_VISUAL_AUDIT`: capture Player evidence and review frames.
8. `AUTOMATION_PROOF`: automate deterministic import/mapping/capture checks.
9. `HANDOFF`: report only the allowed final status and non-claims.

Failure at a phase blocks dependent phases. A compiler PASS, object count or screenshot manifest cannot advance a visual gate.

## Current preflight

| Capability | Result | Evidence |
|---|---|---|
| Unity project/editor | PASS | `client/Unity`, editor 6000.3.2f1 exists |
| URP | PASS | package 17.3.0 |
| Spine Professional editor | BLOCKED | no `Spine.app` or executable found |
| Valid Spine license | BLOCKED | no locally usable licensed installation detected |
| spine-csharp/spine-unity 4.3 | BLOCKED | absent from Packages and Assets |
| Official Spine Examples | BLOCKED | absent from project |
| Spine skeleton/atlas import | NOT EXECUTED | depends on runtime installation |
| Reference Player proof | NOT EXECUTED | depends on licensed/runtime preflight |

Machine-readable evidence: `build/lgo-spine-production-proof-01/toolchain-preflight.json`.

Smallest unblock action: install and activate one valid Spine Professional 4.3.x seat on this machine and provide the `Spine.app` path. The activation code must not be pasted into chat, source control, logs or evidence. After that, install official spine-csharp, spine-unity and examples from the matching 4.3 line and reproduce the official sample before touching LGO assets.

## Legacy classification

### `LEGACY_KEEP_FOR_REFERENCE`

- six pose images and pose-registration guides, used only as motion/silhouette anchors;
- all rejected candidate trees and visual evidence with their provenance;
- surface ownership findings and Pháp sleeve failure analysis.

### `LEGACY_REUSABLE_VALIDATOR`

- source/rejected-selection boundary and true-alpha checks;
- surface ownership, draw-order, silhouette/envelope and unexpected-exposure checks where they can evaluate rendered Spine output;
- mixed/off-slot scenario definitions;
- male/female phase checks;
- Player build, capture and human visual review rules.

### `LEGACY_REPLACE`

- `TwoDRegisteredOutfit` as the production character renderer;
- Unity SpriteSkin/Unity 2D IK character assembly in the rejected prototype;
- `TwoDPaperDollPoseSampler` full-pose swapping as production locomotion;
- per-pose full-item layers as the main equipment architecture.

### `LEGACY_DELETE_ONLY_AFTER_ACCEPTANCE`

- existing runtime classes, atlases, manifests and Player routes still used by the current playable game.

Nothing in this proof authorizes deletion before an accepted Spine Player replacement exists.

## Rig and equipment contract

The shared skeleton semantics are `root`, `pelvis`, `spine`, `chest`, `neck`, `head`, left/right clavicle, upper arm, forearm, hand, thigh, shin and foot. Male/female may use different art and setup positions, while preserving the same semantic motion phases.

Every rendered surface declares one mode:

- `RIGID`: one bone/slot, transform only;
- `SEGMENTED`: separate upper/lower components with a controlled joint bridge;
- `SKINNED`: minimal influences only where bending is necessary;
- `SECONDARY`: dedicated chain for hair, cape, tails or ribbons;
- `CORRECTIVE`: keyed attachment replacement for a limited extreme range.

Normal locomotion keeps limb and head scale at 1.0. Full-body item sprites, per-pose pixel repair, body-part card reuse from rejected prototypes and arbitrary runtime fit offsets are forbidden.

## Motion contract

The existing six-pose semantics become anchors rather than runtime sprite frames:

- `idle`;
- right leg forward with left arm forward;
- right leg high with left arm high;
- left leg forward with right arm forward;
- left leg high with right arm high;
- `jump_tuck`.

The production run interpolates through contact, down, passing and up phases for both sides. Planted feet use Spine IK/contact control. Thigh, shin, arm and head scales remain constant. Male and female use equivalent timestamps and phase opposition.

## Acceptance

Final status is one of:

- `LGO_SPINE_PROOF_ACCEPTED`;
- `LGO_SPINE_PROOF_FIX_REQUIRED`;
- `BLOCKED_SPINE_TOOLING`;
- `BLOCKED_AUTHORITATIVE_ASSET`.

`LGO_SPINE_PROOF_ACCEPTED` requires official reference reproduction, real spine-unity Player execution, accepted Pháp male/female source, synchronized run/jump, stable proportions, rigid/segmented/sleeved equipment, equip/unequip, mixed loadout, stress poses and reviewed runtime evidence. Any missing mandatory condition keeps promotion false.
