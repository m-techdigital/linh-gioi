# Handoff — LGO Spine Production Proof 01

Date: 2026-09-14

FINAL DECISION: BLOCKED_SPINE_TOOLING

REFERENCE REPRODUCTION:
FAIL

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

VISUAL EVIDENCE: none; implementation correctly stopped before reference reproduction because the legal/editor/runtime preflight is incomplete.

MAIN REMAINING VISUAL DEFECTS: no Spine runtime result exists yet, so no visual claim is possible.

## Verified environment

- Unity project: `client/Unity`
- Unity Editor: `6000.3.2f1`
- Unity executable: `/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity`
- URP: `17.3.0`
- Spine Editor: not installed
- Spine Professional license: no usable licensed installation detected
- spine-csharp/spine-unity/examples: not installed
- candidate after unblock: matching Spine Editor/runtime/examples 4.3 line, which officially supports Unity through 6000.4

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
- no Unity Player/build was executed because the mandatory Spine preflight failed.

## Explicit non-claims

The official capability is not an LGO production proof. No reference sample, LGO body, equipment category, motion, Player result, performance result or automation route has passed.

NEXT ALLOWED STEP: install and activate a valid Spine Professional 4.3.x seat on this machine, then provide the local `Spine.app` path without sharing the activation code.
