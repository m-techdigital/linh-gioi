# Handoff — LGO Spine Production Proof 01

Date: 2026-09-14

FINAL DECISION: BLOCKED_SPINE_TOOLING

REFERENCE REPRODUCTION:
FAIL — the required reproduction inside `client/Unity` remains license-blocked.

ISOLATED REFERENCE EVALUATION:
PASS

UNITY RUNTIME:
BLOCKED

LGO MALE:
FAIL

LGO FEMALE:
FAIL

RIGID ITEM:
FAIL

SEGMENTED ITEM:
FAIL

SLEEVED OUTER_TOP:
FAIL

RUN CYCLE:
FAIL

JUMP:
FAIL

EQUIP / UNEQUIP:
FAIL

MIX AND MATCH:
FAIL

PER-POSE PIXEL REPAIR USED:
NO

VISUAL PRODUCTION GATE:
FAIL

VISUAL EVIDENCE: `build/lgo-spine-production-proof-01/reference-automated/contact-sheet.png` for the isolated official evaluation. Rejected LGO source boards exist in the quarantine named by `lgo-created-source-audit.json`; no accepted LGO runtime visual evidence exists.

MAIN REMAINING VISUAL DEFECTS: no accepted LGO male/female body, Pháp equipment, run cycle or jump has been authored or rendered in Spine. The two generated source attempts were rejected before runtime: v1 does not match LGO visual quality; v2 loses rectangular neck/hair/ponytail regions, has incomplete female leg coverage and cannot prove detachable/riggable equipment topology.

LGO SOURCE ADMISSION:
FAIL — `build/lgo-spine-production-proof-01/lgo-source-admission.json` records `spineLgoRuntimeTestExecuted=false`. Old sources remain ineligible. `build/lgo-spine-production-proof-01/lgo-created-source-audit.json` records two additional rejected creation attempts; neither may enter Spine or runtime.

## Verified environment

- Unity project: `client/Unity`
- Unity Editor: `6000.3.2f1`
- Unity executable: `/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity`
- URP: `17.3.0`
- Spine Trial: launcher 4.3.08/editor 4.3.26 installed at `~/Applications/SpineTrial.app`; official Mix-and-Match source opens
- Spine Professional license: no usable licensed installation detected
- spine-csharp/spine-unity evaluation: 4.3.39/4.3.107 at commit `51aad49f3e5db76e91c1c7f1800b0e7536bad11b`
- isolated Unity evaluation: build PASS, 0 errors, 0 warnings, graphics Player PASS for eight reference states
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
- created and visually audited two bounded male/female source attempts after owner authorization; v1 failed LGO identity/finish and v2 failed silhouette reconstruction/modular-topology proof. Both were moved out of work-in-progress into rejected evidence with tombstones, and no LGO Player claim was made.
- hardened the source-selection audit so a rejected tombstone or malformed selection returns `SOURCE_STAGING_SELECTION_REJECTED` instead of crashing or being treated as reviewable; both v1/v2 tombstones now fail closed in machine evidence.

Exact evaluation commands, run from the repository root:

```sh
curl -fL --retry 3 --connect-timeout 20 https://jp.esotericsoftware.com/launcher/mac-arm -o build/toolchains/spine-trial/SpineTrial-ARM.dmg
hdiutil attach build/toolchains/spine-trial/SpineTrial-ARM.dmg -readonly -nobrowse -mountpoint "$PWD/build/toolchains/spine-trial/mount"
ditto 'build/toolchains/spine-trial/mount/Spine Trial.pkg' 'build/toolchains/spine-trial/Spine Trial.pkg'
hdiutil detach "$PWD/build/toolchains/spine-trial/mount"
installer -pkg 'build/toolchains/spine-trial/Spine Trial.pkg' -target CurrentUserHomeDirectory -verboseR
git clone --depth 1 --branch 4.3 https://github.com/EsotericSoftware/spine-runtimes.git build/toolchains/spine-runtimes-4.3
open -a "$HOME/Applications/SpineTrial.app" "$PWD/build/toolchains/spine-runtimes-4.3/examples/mix-and-match/mix-and-match-pro.spine"
"/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -createProject "$PWD/build/lgo-spine-production-proof-01/unity-evaluation" -logFile "$PWD/build/lgo-spine-production-proof-01/unity-evaluation-create.log"
"/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$PWD/build/lgo-spine-production-proof-01/unity-evaluation" -executeMethod SpineEvaluationBuilder.BuildOfficialMixAndMatch -logFile "$PWD/build/lgo-spine-production-proof-01/unity-evaluation-build-driver.log"
LGO_SPINE_EVAL_EVIDENCE="$PWD/build/lgo-spine-production-proof-01/reference-automated" "$PWD/build/lgo-spine-production-proof-01/unity-evaluation/Build/SpineMixAndMatchEvaluation.app/Contents/MacOS/Spine Mix And Match Evaluation" -screen-width 1280 -screen-height 720 -screen-fullscreen 0 -logFile "$PWD/build/lgo-spine-production-proof-01/reference-automated/player.log"
```

The evaluation-only builder and runtime driver are retained under `build/lgo-spine-production-proof-01/unity-evaluation/Assets/`; they were not copied into `client/Unity`.

## Explicit non-claims

The isolated official reference evaluation passed. The newly created LGO candidates failed source/visual admission. Neither result authorizes runtime integration into `client/Unity` or proves any LGO body, equipment category, run/jump quality, performance target or production automation route.

## Requested production report

1. **Source created/reopened:** v1 male/female and v2 male/female ORA files were created. v1 was visually rejected as mannequin-like. An earlier male v2 revision reopened and exported through isolated Krita in 22.5 seconds, but later v2 edits invalidated that round-trip evidence; the final v2 is rejected and was not promoted on the stale export. No accepted source currently exists.
2. **Behavior run in Player:** only the unmodified official Mix-and-Match evaluation ran: base, bag, backpack, remove, restore, combined skin and two walk phases. No LGO character, LGO garment, run or jump ran in Player.
3. **Second item reuse:** v2 item B reused item A alpha geometry and changed palette without per-item offset. This is a source-level observation only; because the underlying masks are structurally invalid, it is not an accepted reuse result and no Player behavior was demonstrated.
4. **Remaining faults:** male neck/hair rectangle loss; female ponytail rectangle loss and incomplete leg coverage; generated flat board has no valid hidden material under detachable belt/armor and does not establish sleeve topology; no accepted male/female body; no licensed LGO Spine export.
5. **Measured time:** official isolated Unity build took 5.163 seconds; the earlier isolated Krita male reopen/export took 22.5 seconds. The source attempts were not instrumented as production tasks, so there is no defensible `create item` or `add second item` time. Reporting their file timestamp span as production throughput would be misleading. Review waiting time was zero because both candidates failed self-audit before owner review.

NEXT ALLOWED STEP: provision Spine Professional 4.3.x and an owner-accepted reopenable layered LGO male/female + sleeved Pháp Lv1 source package, then run the incremental substitution in `client/Unity`.
