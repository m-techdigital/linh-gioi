# Handoff — LGO Spine Production Proof 01

Date: 2026-09-14

FEASIBILITY DECISION: NOT PROVEN FOR LGO

LIMITED RESULT: API_ATTACHMENT_COMPATIBILITY_ONLY

PRODUCTION DECISION: BLOCKED_SPINE_TOOLING

REFERENCE REPRODUCTION:
FAIL — the required reproduction inside `client/Unity` remains license-blocked.

ISOLATED REFERENCE EVALUATION:
PASS

ISOLATED UNITY RUNTIME WITH TWO LGO TEXTURES:
LIMITED — the textures attached to the official sample, but this is `WRONG_TEST_SCOPE_OWNER_REJECTED` as a character/rig proof.

LGO SPINE EDITOR SOURCE CREATED / SAVED / REOPENED:
FAIL — no LGO `.spine` source exists; Trial cannot save.

LGO SPINE EDITOR ANIMATION REVIEW:
FAIL — no actual LGO full-character idle/run/jump was authored or run in Spine Editor.

OFFICIAL SPINE EXPORT OF LGO SOURCE:
FAIL — Trial cannot export.

LGO MALE:
FAIL

LGO FEMALE:
FAIL

RIGID ATTACHMENT API:
LIMITED — two textures followed slots on the official sample; it is not proof of LGO rigging or fit.

SEGMENTED ITEM:
FAIL

SLEEVED OUTER_TOP:
FAIL

RUN CYCLE:
FAIL

JUMP:
FAIL

EQUIP / UNEQUIP API:
LIMITED — runtime API behavior on the official sample only.

MIX AND MATCH:
FAIL

PER-POSE PIXEL REPAIR USED:
NO

VISUAL PRODUCTION GATE:
FAIL

VISUAL EVIDENCE RETAINED: official reference board at `build/lgo-spine-production-proof-01/reference-automated/contact-sheet.png`. API compatibility captures and rejected programmatic fixtures were purged after owner rejection; cleanup record is `build/lgo-spine-production-proof-01/wrong-scope-cleanup-2026-09-14.json`.

MAIN REMAINING VISUAL DEFECTS: no accepted LGO male/female body, Pháp equipment, run cycle or jump has been authored or rendered in Spine. The two generated source attempts were rejected before runtime: v1 does not match LGO visual quality; v2 loses rectangular neck/hair/ponytail regions, has incomplete female leg coverage and cannot prove detachable/riggable equipment topology.

LGO SOURCE ADMISSION:
FAIL — `build/lgo-spine-production-proof-01/lgo-source-admission.json` records `spineLgoRuntimeTestExecuted=false`. Old sources remain ineligible. The two additional rejected generated source trees were deleted; their failure reasons remain in this handoff and the stopped-path guard.

## Verified environment

- Unity project: `client/Unity`
- Unity Editor: `6000.3.2f1`
- Unity executable: `/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity`
- URP: `17.3.0`
- Spine Trial: launcher 4.3.08/editor 4.3.26 installed at `~/Applications/SpineTrial.app`; official Mix-and-Match source opens
- Spine Professional license: no usable licensed installation detected
- spine-csharp/spine-unity evaluation: 4.3.39/4.3.107 at commit `51aad49f3e5db76e91c1c7f1800b0e7536bad11b`
- isolated Unity evaluation: build PASS, 0 errors, 0 warnings, graphics Player PASS for eight reference states
- LGO texture/API attachment evaluation: technically ran in Unity, but owner rejected it as proof of LGO character/rig feasibility
- programmatic LGO cutout/weighted-mesh fixtures: `WRONG_TEST_SCOPE_OWNER_REJECTED`; preserved only as failure evidence
- `client/Unity`: unchanged; no unlicensed runtime integration
- real LGO source admission: FAIL; no accepted male/female + sleeved Pháp source can legally enter the proof

## Current architecture audit

- `TwoDRegisteredOutfit`, Unity SpriteSkin/2D IK assembly and `TwoDPaperDollPoseSampler` are `LEGACY_REPLACE`, while remaining reachable until an accepted replacement exists.
- Source-selection/alpha, surface ownership, silhouette/envelope, mixed/off-slot, phase and Player evidence checks are `LEGACY_REUSABLE_VALIDATOR` where adapted to rendered Spine output.
- Rejected source and pose evidence is `LEGACY_KEEP_FOR_REFERENCE` and cannot be promoted.
- Runtime classes, atlases and manifests are `LEGACY_DELETE_ONLY_AFTER_ACCEPTANCE`; no deletion occurred.

FILES CHANGED:

- `docs/art/LGO-SPINE-PRODUCTION-PROOF-01.md`
- `docs/superpowers/plans/2026-09-14-lgo-spine-production-proof-01.md`
- `docs/execution/HANDOFF-LGO-SPINE-PRODUCTION-PROOF-01.md`
- `docs/execution/NEXT-ACTION.md`
- `docs/execution/PROJECT-STATE.md`
- `docs/execution/STOPPED-PATHS-AND-RESUME-GUARDS.md`
- `docs/execution/TASK-LEDGER-ROLLUP.md`
- `tools/lgo_next_task.py`
- `tools/test_lgo_next_task.py`

DELETIONS:

None.

COMMANDS / RUNTIME TESTS:

- inspected local Spine applications/installers, Unity packages and skeleton assets;
- read `client/Unity/ProjectSettings/ProjectVersion.txt` and `Packages/manifest.json`;
- confirmed Unity editor executable exists;
- checked official Spine documentation and 4.3 runtime branches;
- wrote and parsed `build/lgo-spine-production-proof-01/toolchain-preflight.json`;
- added and ran the active-state advisor regression test; the advisor now stops at `BLOCKED_SPINE_TOOLING` instead of routing to combat;
- downloaded and installed the official Apple Silicon Spine Trial; launcher/editor versions 4.3.08/4.3.26;
- opened `examples/mix-and-match/mix-and-match-pro.spine` in the real Trial GUI;
- cloned official spine-runtimes branch 4.3 at commit `51aad49f3e5db76e91c1c7f1800b0e7536bad11b`;
- imported the official packages/examples into an isolated Unity 6000.3.2f1 evaluation project;
- built the final automated official `Mix and Match Skins` scene in 5.163 seconds: 0 errors, 0 warnings;
- ran a graphics Player through base, bag, backpack, remove, restore, combined skin and two walk phases;
- reviewed all eight Player captures; evidence is `build/lgo-spine-production-proof-01/reference-evaluation.json` and `reference-automated/contact-sheet.png`.
- audited the actual LGO male/female/Pháp source trees and current Unity Pháp atlas; evidence is `build/lgo-spine-production-proof-01/lgo-source-admission.json`; no rejected asset was copied, renamed, rigged or rendered.
- created and visually audited two bounded male/female source attempts after owner authorization; v1 failed LGO identity/finish and v2 failed silhouette reconstruction/modular-topology proof. Their generated pixels and authoring scripts were deleted; only small tombstones and written lessons remain.
- hardened the source-selection audit so a rejected tombstone or malformed selection returns `SOURCE_STAGING_SELECTION_REJECTED` instead of crashing or being treated as reviewable; both v1/v2 tombstones now fail closed in machine evidence.
- copied the current Pháp Lv1 belt/weapon atlas regions into isolated evaluation resources and attached them to the official sample at runtime; the resulting fixture and captures were later purged as wrong-scope output;
- stopped and quarantined the later ten-part cutout and whole-base runtime mesh attempts because they bypassed Spine Editor authoring/export and repeated an owner-rejected architecture.

The disposable isolated Unity project, its builder/driver code, caches and Player builds were deleted. The exact recreation commands were removed from this active handoff so a later worker cannot mistake that fixture for the valid resume path. The official Spine 4.3 source checkout and official reference screenshots remain available.

## Explicit non-claims

The isolated official reference evaluation passed. The belt/weapon probe records only that the runtime API accepts two LGO textures; owner review rejected it as evidence of LGO character feasibility. The newly created body/outfit source candidates still fail admission. None of this authorizes runtime integration into `client/Unity` or proves an accepted LGO body, sleeve deformation, run/jump quality, performance target or production automation route.

## Requested production report

1. **Source created/reopened:** v1 male/female and v2 male/female ORA files were created. v1 was visually rejected as mannequin-like. An earlier male v2 revision reopened and exported through isolated Krita in 22.5 seconds, but later v2 edits invalidated that round-trip evidence; the final v2 is rejected and was not promoted on the stale export. No accepted source currently exists.
2. **Behavior run in Player:** the unmodified official Mix-and-Match evaluation ran first. A second isolated Player run attached two LGO textures to that official sample. The animation continued through the swap, but this is API evidence only. No LGO body rig, sleeve or jump ran in Spine Editor or from a Spine Editor export.
3. **Second item reuse:** v2 item B reused item A alpha geometry and changed palette without per-item offset. This is a source-level observation only; because the underlying masks are structurally invalid, it is not an accepted reuse result and no Player behavior was demonstrated.
4. **Remaining faults:** male neck/hair rectangle loss; female ponytail rectangle loss and incomplete leg coverage; generated flat board has no valid hidden material under detachable belt/armor and does not establish sleeve topology; no accepted male/female body; no licensed LGO Spine export.
5. **Measured time:** official isolated Unity build took 5.163 seconds; the wrong-scope API attachment rebuild took 2.730754 seconds and its Player sequence took 11.6 seconds. The earlier isolated Krita male reopen/export took 22.5 seconds. These times do not estimate Spine production. No valid LGO Spine authoring task or reusable second item has completed, so there is no defensible production `create item` time.

NEXT ALLOWED STEP: provision Spine Professional 4.3.x and an owner-accepted reopenable layered LGO male/female + sleeved Pháp Lv1 source package. Author the full character, equipment and idle/run/jump in Spine Editor; save/reopen and review it there; export officially; then test that export in `client/Unity`.
