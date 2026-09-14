# LGO Spine production proof 01

Date: 2026-09-14

Task: `LGO-SPINE-PRODUCTION-PROOF-01`

Current phase: `EDITOR_AUTHORING_NOT_STARTED`
Current feasibility decision: `API_ATTACHMENT_COMPATIBILITY_ONLY`
Current production decision: `BLOCKED_SPINE_TOOLING`

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

## Owner scope correction — 2026-09-14

The required proof starts in **Spine Editor** with the actual LGO character and full Pháp Lv1 equipment. It must create an editable Spine project, save it, reopen it, run the authored idle/run/jump animations in Spine, export with the official exporter, then run that export in the real Unity Player.

Three experiments made after the reference reproduction are rejected as `WRONG_TEST_SCOPE_OWNER_REJECTED`:

- converting two LGO atlas sprites into `RegionAttachment` objects on the official sample skeleton;
- generating a ten-part LGO cutout `SkeletonData` through spine-csharp in Unity;
- generating a whole-base weighted `MeshAttachment` through spine-csharp in Unity.

These experiments only exercised runtime APIs. They did not prove Spine Editor authoring, editable source, weights, deformation, export, LGO body motion or a full equipped character. At the owner's request, their code, Unity project/cache, Player builds, images, logs and generated source pixels were deleted on 2026-09-14. Only the written lesson, tombstones and cleanup manifest remain; no later session may recreate or promote them as the production proof.

The only valid resume path is an activated Spine Professional 4.3.x editor plus an accepted, reopenable LGO male/female and sleeved Pháp Lv1 source package. Trial 4.3.26 can open the official sample but cannot save or export this proof, so implementation stops at `BLOCKED_SPINE_TOOLING` until that capability exists.

## API attachment compatibility evidence — insufficient for the required proof

After the owner reduced the immediate scope to proving applicability, a bounded Player probe used the already verified official skeleton and two actual Pháp Lv1 components from the current LGO atlas: `waist_belt` and `main_weapon`. Unity imported them as sprites, converted them at runtime to Spine `RegionAttachment` objects, and placed them on the official `body-dress` and `hand-front` animated slots.

The Player executed six captured states: official base, both LGO items equipped at idle, two `walk` phases, both items removed during `walk`, and both items restored while `walk` continued. Visual review confirms that both items appear only in equipped states and move with their separate slots. The motion log records belt-bone movement of 1.0681/0.2718 units and weapon-bone movement of 2.4534/0.2905 units across the captured sequence. Build result: 0 errors, 0 warnings, 2.730754 seconds.

The detailed captures, logs and runtime fixture were purged after owner rejection. Cleanup record: `build/lgo-spine-production-proof-01/wrong-scope-cleanup-2026-09-14.json`.

This records only `API_ATTACHMENT_COMPATIBILITY_ONLY`: spine-unity accepted two RGBA textures and its slot API could remove and restore them. It is insufficient for the requested LGO Spine proof and must not be described as actual-LGO feasibility. The sample mismatch is a rejected test-scope result, not a production candidate.

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
| LGO texture/API attachment compatibility | LIMITED | two textures attached to the official sample at runtime; `WRONG_TEST_SCOPE_OWNER_REJECTED` as an LGO production proof |
| Actual LGO Spine Editor authoring | BLOCKED | no LGO `.spine` project created/saved/reopened; Trial cannot save/export |
| LGO project integration | BLOCKED | license required before adding Spine Runtimes to `client/Unity` |
| LGO male source admission | FAIL | old 12-layer KRA reconstructs the rejected cutout body; two newly authored candidates also failed visual/source audit and remain `runtimeEligible=false` |
| LGO female source admission | FAIL | old six-pose PNGs have no editable layered source; two newly authored candidates also failed visual/source audit and remain `runtimeEligible=false` |
| Pháp Lv1 source admission | FAIL | no accepted sleeved source exists; the new ORA candidates cannot prove detachable belt/armor or riggable sleeve topology |
| Existing Unity Pháp atlas | FAIL AS SPINE INPUT | RGBA/alpha is valid, but manifest is `DRAFT_RUNTIME_FIT`, `runtimeEligibleCount=0`, and the assets are static paper-doll parts |

Machine-readable evidence: `build/lgo-spine-production-proof-01/toolchain-preflight.json`, `build/lgo-spine-production-proof-01/reference-evaluation.json` and `build/lgo-spine-production-proof-01/lgo-source-admission.json`. Visual board: `build/lgo-spine-production-proof-01/reference-automated/contact-sheet.png`.

The source-admission precheck was run against the real LGO trees and current Unity Pháp atlases. At that stage no LGO source was admitted as a body/rig and no LGO body Player test was executed. The later bounded attachment probe used two atlas components only; it does not alter the failed body/source admission. Reusing the old male KRA would restore the body/cutout result already rejected by the owner; treating the complete current paper-doll atlas as a rig source would only reproduce the stopped static-fit architecture.

After the owner authorized creating missing test source, two bounded male/female attempts were authored and audited before any runtime use:

- `spine-production-proof-source-v1`: `VISUAL_REJECTED_MANNEQUIN_NOT_LGO`; the reopenable layered ORA exists, but the anatomy/rendering does not preserve LGO identity.
- `spine-production-proof-source-v2`: `VISUAL_REJECTED_STRUCTURAL_MASK_LOSS`; the attempted segmented masks remove rectangular areas from the male neck/hair and female ponytail, leave incomplete female leg coverage, and cannot demonstrate a genuinely detachable belt/armor or deformable sleeved topology from the flat generated board.

Both generated source trees and their build copies were deleted at the owner's request. Small `DO-NOT-USE` tombstones remain at the former active paths so source selection cannot silently rediscover or recreate them. Cleanup record: `build/lgo-spine-production-proof-01/wrong-scope-cleanup-2026-09-14.json`. This closes the question of whether simply generating an ORA or splitting a flat board is sufficient: it is not. It does not close the accepted-source gate.

The proof still has two independent prerequisites: purchase/provision and activate one valid Spine Professional 4.3.x seat, and supply or author an owner-accepted reopenable layered male/female base plus the Pháp Lv1 sleeved source. The failed generated candidates do not satisfy the second prerequisite. The activation code must not be pasted into chat, source control, logs or evidence. After both exist, author and visually check the full LGO character in Spine Editor first, save/reopen it, export it, and only then integrate that export into `client/Unity` for Player review.

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
