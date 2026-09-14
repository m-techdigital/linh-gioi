# LGO Spine production proof 01

Date: 2026-09-14

Task: `LGO-SPINE-PRODUCTION-PROOF-01`

Current phase: `LICENSE_GATE_BEFORE_LGO_INTEGRATION`
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
| Spine Trial editor | PASS | official macOS Apple Silicon Trial installed at `~/Applications/SpineTrial.app`; launcher 4.3.08 loaded editor 4.3.26 |
| Spine Professional editor | BLOCKED | Trial cannot save/export; no activated Professional seat is available |
| Valid Spine license | BLOCKED | no locally usable licensed installation detected |
| spine-csharp/spine-unity 4.3 evaluation | PASS | official commit `51aad49f3e5db76e91c1c7f1800b0e7536bad11b`; packages 4.3.39/4.3.107 in isolated build workspace |
| Official Spine Examples evaluation | PASS | official `Mix and Match Skins` source opened in Trial and imported into isolated Unity evaluation project |
| Spine skeleton/atlas import evaluation | PASS | official 4.3 export imported without compiler/import errors |
| Reference Player evaluation | PASS | graphics Player captured base, bag, backpack, remove, restore, combined skin and two walk phases |
| LGO project integration | BLOCKED | license required before adding Spine Runtimes to `client/Unity` |
| LGO male source admission | FAIL | only reopenable 12-layer KRA reconstructs the owner-rejected cutout/body authority; `runtimeEligible=false` |
| LGO female source admission | FAIL | six pose PNGs are `SOURCE_REVIEW_REQUIRED`; no editable layered source exists |
| Pháp Lv1 source admission | FAIL | active authoring root is `SOURCE_EMPTY_AWAITING_NATIVE_AUTHORING`, `sourceArtifactAccepted=false`, and contains zero garment art files |
| Existing Unity Pháp atlas | FAIL AS SPINE INPUT | RGBA/alpha is valid, but manifest is `DRAFT_RUNTIME_FIT`, `runtimeEligibleCount=0`, and the assets are static paper-doll parts |

Machine-readable evidence: `build/lgo-spine-production-proof-01/toolchain-preflight.json`, `build/lgo-spine-production-proof-01/reference-evaluation.json` and `build/lgo-spine-production-proof-01/lgo-source-admission.json`. Visual board: `build/lgo-spine-production-proof-01/reference-automated/contact-sheet.png`.

The source-admission precheck was run against the real LGO trees and current Unity Pháp atlases. No LGO asset was admitted into Spine and no LGO Spine Player test was executed. Reusing the old male KRA would restore the body/cutout result already rejected by the owner; treating the current paper-doll atlas as a rig source would only reproduce the stopped static-fit architecture.

The proof now has two independent prerequisites: purchase/provision and activate one valid Spine Professional 4.3.x seat, and supply or author an owner-accepted reopenable layered male/female base plus the Pháp Lv1 sleeved source. The activation code must not be pasted into chat, source control, logs or evidence. After both exist, use the already verified 4.3 runtime line in `client/Unity`, reproduce the same Player sequence there, and substitute the admitted LGO sources incrementally.

The evaluation PASS proves the official workflow and local Unity/tool compatibility. It does not prove LGO body quality, Pháp sleeves, run/jump proportions or permission to ship/integrate the runtime.

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
