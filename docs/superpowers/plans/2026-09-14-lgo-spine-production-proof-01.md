# LGO Spine Production Proof 01 Implementation Plan

> **For Codex:** Execute phases in order. Stop at the first unmet gate. Do not substitute custom skeletal code or mock Spine data.

**Goal:** Prove in a real Unity Player that official Spine Mix-and-Match can drive an authoritative Pháp Lv1 male/female character with rigid, segmented and sleeved modular equipment through idle, smooth run and jump without per-pose pixel repair.

**Architecture:** Unity keeps game systems and hosts the official spine-unity runtime. A licensed Spine 4.3 project owns the skeleton, skins, constraints, animations and attachment templates. LGO runtime code maps existing item identities to Spine skins/attachments only after the unmodified official reference sample passes.

**Tech Stack:** Unity 6000.3.2f1, URP 17.3.0, licensed Spine Professional 4.3.x, official spine-csharp/spine-unity/examples 4.3.

**Spec:** `docs/art/LGO-SPINE-PRODUCTION-PROOF-01.md`

## Global Constraints

- Keep `protocol/**`, `gamedata/schemas/**`, `docs/adr/**` and `client/Unity/Assets/Game/UI/design-tokens.json` unchanged.
- Do not install or integrate Spine Runtimes without a valid Spine license.
- Editor and runtime export versions must match the 4.3 line.
- Do not alter current production routes before the proof passes Player visual review.
- Do not use full-item per-pose pixel/mask repair, split-body cards, fake skeleton JSON or a homemade Spine parser.
- Do not expand beyond Pháp Lv1 male/female and the three required equipment categories.

---

### Task 1: Toolchain preflight

**Files:**
- Create: `build/lgo-spine-production-proof-01/toolchain-preflight.json`
- Modify: `docs/execution/NEXT-ACTION.md`
- Modify: `docs/execution/PROJECT-STATE.md`

**Interfaces:**
- Consumes: local applications, Unity project version, package manifest, official Spine compatibility/license pages.
- Produces: one of `REFERENCE_REPRODUCTION_READY` or `BLOCKED_SPINE_TOOLING` with exact blockers.

- [x] Inspect Unity editor/project version and URP package.
- [x] Search local applications/installers and Unity packages for Spine editor/runtime/examples.
- [x] Verify current compatibility, Mix-and-Match behavior and license requirements against official documentation.
- [x] Write machine-readable preflight evidence.
- [x] Add a regression test so the next-task advisor honors `BLOCKED_SPINE_TOOLING` instead of selecting an unrelated backlog task.
- [x] Stop implementation because Spine Professional, spine-unity and official examples are unavailable.

Resume command after owner installation:

```sh
find /Applications "$HOME/Applications" -maxdepth 5 -type f -path '*Spine*.app/Contents/MacOS/*' -print
```

Expected: a licensed Spine 4.3.x editor executable is found. Do not print activation data.

---

### Task 2: Install matching official runtime and reproduce vendor imports

**Files:**
- Modify: `client/Unity/Packages/manifest.json`
- Modify: `client/Unity/Packages/packages-lock.json`
- Create: `build/lgo-spine-production-proof-01/reference-import/`

**Interfaces:**
- Consumes: licensed Spine 4.3.x and official 4.3 Git package URLs.
- Produces: official `Spine`, `Spine.Unity` and example assemblies/assets that import without LGO adapters.

- [ ] Add official 4.3 spine-csharp, spine-unity and examples packages using the URLs published at `https://esotericsoftware.com/spine-unity-installation`.
- [ ] Open Unity 6000.3.2f1 in batch mode and record package resolution/import logs.
- [ ] Run the vendor example EditMode tests if supplied; otherwise compile/import the vendor example scenes without adding an LGO test facade.
- [ ] Verify URP rendering with the official 4.3 URP shader package only if the reference material requires it.

Run:

```sh
/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity -batchmode -quit -projectPath client/Unity -logFile build/lgo-spine-production-proof-01/reference-import/unity-import.log
```

Expected: exit 0, no C# compiler errors, Spine skeleton assets imported by the official importer.

---

### Task 3: Official Mix-and-Match runtime reproduction

**Files:**
- Create: `client/Unity/Assets/Game/Tests/EditMode/LgoSpineReferenceProofTests.cs`
- Create: `client/Unity/Assets/Game/Character/SpineProof/Scenes/LgoSpineReferenceProof.unity`
- Create: `client/Unity/Assets/Game/Character/SpineProof/Runtime/LgoSpineReferenceProofController.cs`
- Create: `build/lgo-spine-production-proof-01/reference-runtime/`

**Interfaces:**
- Consumes: official `Mix and Match Skins` sample skeleton, animations, skins and `Spine.Unity.SkeletonAnimation`.
- Produces: Player sequence `base → clothing A → clothing B → remove A → restore A → combined loadout → locomotion`.

- [ ] Write an EditMode test that fails because `LgoSpineReferenceProofController` does not yet expose the required ordered proof states.
- [ ] Run the focused test and confirm the expected missing-type/API failure.
- [ ] Implement the smallest controller around official Skin API calls; do not copy or rewrite the runtime.
- [ ] Run the focused test and full affected assembly tests.
- [ ] Build and run a graphics-enabled Player, capture every equipment state during locomotion, and visually review it.

Gate: `REFERENCE_REPRODUCTION_PASS` only when skeleton, animation, runtime skin combination, equip/unequip and Player capture all pass.

---

### Task 4: Authoritative LGO source admission

**Files:**
- Create externally: `LGO-Selected-2D-Source-v1/class-work-in-progress/phap-lv001/spine-production-proof-01/`
- Create: `build/lgo-spine-production-proof-01/source-admission/`

**Interfaces:**
- Consumes: owner-accepted Pháp Lv1 male/female layered source and six-pose silhouette references.
- Produces: accepted source manifest with body proportions, visible/hidden surfaces, equipment ownership and hashes.

- [ ] Reject every source tree marked owner-rejected, withdrawn or `DO-NOT-SELECT`.
- [ ] Check that male/female source can be opened and edited, has true transparency, complete hidden surfaces and no baked checkerboard.
- [ ] Create one idle setup comparison against the owner-approved design before rigging all motion.
- [ ] Stop with `BLOCKED_AUTHORITATIVE_ASSET` if either gender lacks accepted source.

No Unity or Spine LGO integration starts until this gate passes.

---

### Task 5: Shared LGO skeleton and body proof

**Files:**
- Create externally: `phap-lv001/spine-production-proof-01/lgo-phap-lv1.spine`
- Create: `client/Unity/Assets/Game/Character/SpineProof/Art/PhapLv1/`
- Create: `client/Unity/Assets/Game/Tests/EditMode/LgoSpineRigContractTests.cs`

**Interfaces:**
- Consumes: admitted male/female source and official Spine 4.3 exporter.
- Produces: matching 4.3 skeleton data/atlas with shared semantic bone names and `skin/base/male`, `skin/base/female`.

- [ ] Write a failing test for required bones, base skins, setup scale 1.0 and idle animation.
- [ ] Author the minimum shared skeleton in licensed Spine without reusing rejected split-card geometry.
- [ ] Export with Spine 4.3 and import through official spine-unity.
- [ ] Pass the contract test, then render and compare male/female idle in Player.

---

### Task 6: Three-category equipment proof

**Files:**
- Create: `client/Unity/Assets/Game/Character/SpineProof/Runtime/LgoSpineEquipmentDefinition.cs`
- Create: `client/Unity/Assets/Game/Character/SpineProof/Runtime/LgoSpineLoadoutComposer.cs`
- Create: `client/Unity/Assets/Game/Tests/EditMode/LgoSpineEquipmentProofTests.cs`
- Modify externally: `phap-lv001/spine-production-proof-01/lgo-phap-lv1.spine`

**Interfaces:**
- Consumes: existing item IDs, accepted source surfaces and shared skeleton.
- Produces: runtime skin composition for one rigid item, one segmented item and sleeved `outer_top`.

- [ ] Write failing tests for item-to-skin mapping, missing skin rejection, deterministic draw order and removal/restoration without changing animation identity.
- [ ] Implement data definitions and Skin API composition using existing item IDs.
- [ ] Author rigid, segmented and sleeved surfaces with declared deformation modes; use corrective attachments only at named extreme ranges.
- [ ] Pass focused tests and capture base, each item, all-on, outer-off and one mixed loadout.

---

### Task 7: Run, jump, IK and stress proof

**Files:**
- Create: `client/Unity/Assets/Game/Tests/EditMode/LgoSpineMotionContractTests.cs`
- Modify externally: `phap-lv001/spine-production-proof-01/lgo-phap-lv1.spine`
- Create: `build/lgo-spine-production-proof-01/motion-proof/`

**Interfaces:**
- Consumes: canonical LGO arm/leg opposition and shared rig.
- Produces: idle/run/jump/stress animations with constant limb/head scale and planted-foot contact.

- [ ] Write failing tests that sample both genders at contact/passing A/B and reject wrong opposition or bone scale drift.
- [ ] Author contact, down, passing and up phases plus jump/tuck and transitions in Spine.
- [ ] Add foot IK targets for planted phases and a stress animation covering shoulder, elbow, hip, knee and torso ranges.
- [ ] Pass numeric motion tests, then review Player frames for sliding, gaps and deformation.

---

### Task 8: Player visual audit and performance sanity

**Files:**
- Create: `build/lgo-spine-production-proof-01/player/`
- Create: `build/lgo-spine-production-proof-01/visual-review.json`

**Interfaces:**
- Consumes: reference and LGO proof scenes.
- Produces: male/female boards for base, full proof loadout, outer-only, mixed loadout and stress frames.

- [ ] Build one graphics-enabled Player after the complete batch is ready.
- [ ] Capture equivalent timestamps for male/female and all required loadouts.
- [ ] Inspect the images for every visual failure in the spec; technical capture success is insufficient.
- [ ] Record skeleton components, active attachments, materials, practical draw calls and repeated-swap allocations.

Gate: any visual defect keeps `LGO_SPINE_PROOF_FIX_REQUIRED`.

---

### Task 9: Automation proof and handoff

**Files:**
- Create: `docs/execution/HANDOFF-LGO-SPINE-PRODUCTION-PROOF-01.md`
- Modify: `docs/execution/PROJECT-STATE.md`
- Modify: `docs/execution/NEXT-ACTION.md`
- Modify: `docs/execution/TASK-LEDGER-ROLLUP.md`

**Interfaces:**
- Consumes: accepted source, export/import logs, tests and visual evidence.
- Produces: one allowed final status, changed/deleted file lists, commands, versions, non-claims and next action.

- [ ] Demonstrate deterministic source naming, template resolution, item mapping, import and capture without automating subjective approval.
- [ ] Run focused tests, required repo guards, Player build/capture and visual review.
- [ ] Write the handoff using the exact decision format from the task specification.
- [ ] Delete legacy production paths only in a separately approved cleanup after `LGO_SPINE_PROOF_ACCEPTED`.
