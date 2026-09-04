# M6 Combat Readiness Spec v0.48.0

Final readiness decision: `NO_CONTRACT_CHANGE_REQUIRED_FOR_M6_V0_49_LOCAL_PROTOTYPE`.

This spec reviews the accepted M6 placeholder combat state before opening a real implementation task. It authorizes only the next narrow local combat prototype and does not implement gameplay.

## Current Accepted State

- Baseline source: latest full-source after `M6_SOURCE_GATE_CONSISTENCY_HOTFIX_CLOSED_v0.46.1`.
- v0.47 visual acceptance artifacts are not present in this repo snapshot; v0.48 treats human visual acceptance as review-ready or owner-provided context, not as a new runtime claim.
- Runtime-usable placeholder combat PNGs are source-controlled under `client/Unity/Assets/Game/Art/Combat/Placeholders/Resources/CombatPlaceholders/`.
- `CombatPlaceholderAssets.cs` exposes target dummy, target marker, hit spark, warning telegraph, cooldown, combat panel, and combat button placeholder assets through Unity `Resources`.
- `M4PlayableClientController.cs` shows Vietnamese player-facing local combat copy and uses placeholder panel/button/cooldown visuals.
- `PlayableWorldController.cs` drives local-only target readability, hit flash, cooldown ring, target marker, warning telegraph, and current `CombatIntent` preview wiring.

## Non-Claims

- Real combat is not implemented by v0.48.
- No new damage, HP, cooldown, enemy AI, loot, economy, DB/auth, social, or live-ops behavior is added.
- Placeholder PNGs are not production art.
- This spec does not claim full MMO readiness or broader M0 runtime closure.

## Combat Ownership Model

Client visual feedback:
- Owns readability only: target selection state, local hit flash, cooldown/readiness display, warning telegraph, and combat panel/button skins.
- May preview a result while clearly labeling local-only behavior.
- Must not declare authoritative damage, rewards, HP persistence, or economy changes.

Client input intent:
- May build `CombatIntent` from current actor, target, skill id, target position, sequence, and client time.
- Must keep intent copy separate from local visual feedback and preserve Vietnamese player-facing labels.

Server validation:
- Owns acceptance/rejection of `CombatIntent`, sequence handling, target validity, cooldown validation, and result emission.
- For v0.49 it may remain a local prototype harness if no server runtime gate is opened.

GameData skill config:
- Existing skill schema already includes cooldown, activation, targeting, effect placeholder amount, telegraph, damage coefficient, range, and tags.
- v0.49 may consume existing `skill.sword.wind_slash` style config without schema mutation.

Protocol messages:
- Existing `protocol/combat.proto` already defines `CombatIntent`, `CombatAccepted`, `CombatRejected`, `CombatResult`, and `CombatStateSnapshot`.
- v0.49 must not introduce private DTOs that bypass protobuf.

## Safe To Reuse In v0.49

- `CombatPlaceholderAssets` runtime asset loader.
- Existing target dummy entity id and wind slash skill id as prototype fixtures.
- Existing local-only combat button, cooldown icon, combat panel, target marker, hit spark, and warning telegraph.
- Existing protocol combat messages and GameData schema fields.
- Existing source/runtime closure scripts as regression gates.

## Missing Runtime Surfaces

- A deterministic local combat state object that owns HP/readiness for the prototype instead of scattering state across UI/world code.
- A validator that proves no private combat DTO/schema workaround was introduced.
- Runtime smoke evidence for accepted, rejected, cooldown, out-of-range, and recovery paths.
- Visual evidence focused on target dummy idle/selected/recover, hit spark, cooldown ready/cooldown, target marker, warning telegraph, button states, and panel skin.

## Required Runtime Gates For Actual Combat

- Source gates: package hygiene, protocol/GameData frozen audit, M6 combat readiness validator.
- Unity compile/player build from current source.
- Local combat smoke with deterministic pass/fail JSON.
- Visual evidence capture with review checklist.
- If server-authoritative runtime is opened later: live Java realtime server, Unity player sends real `CombatIntent`, server emits accepted/rejected/result snapshot, and invalid intent survival is proven.

## Required Validators

- `tools/validate_m6_combat_readiness_spec.py`
- `tools/validate_m6_runtime_usable_combat_asset_pack.py`
- `tools/validate_m6_unity_combat_placeholder_asset_import.py`
- `tools/validate_m6_combat_protocol_gamedata_contract.py`
- `tools/validate_m6_server_combat_contract_spec.py`
- `tools/validate_package_hygiene.py`

## Entry Criteria For M6 v0.49

- v0.46.1 source/gate consistency PASS from fresh unzip.
- v0.47 visual acceptance is review-ready or owner accepted.
- v0.48 spec and contract impact review are accepted.
- Frozen surfaces remain unchanged.
- v0.49 stays local prototype unless a separate S0 contract-change request is approved.

## Forbidden Scope For v0.49

- No protocol mutation.
- No GameData schema mutation.
- No production auth, DB persistence, economy, inventory, loot, guild, chat, market, party, live ops, enemy AI, or MMO-scale combat.
- No production art claim.
- No private DTO/schema workaround.

## Failure Classification

- `FIX_REQUIRED`: source/docs/tooling are inconsistent, gates fail, or v0.49 scope attempts to expand without approval.
- `BLOCKED_CONTRACT`: requested behavior cannot be represented by existing protocol/GameData contracts.
- `CONTRACT_CHANGE_REQUIRED`: a proposed next task needs new protobuf fields, schema fields, or durable semantic changes.
- `NO_CONTRACT_CHANGE_REQUIRED`: next task can use existing v0.40 combat protocol and current GameData schema.

## Decision Tree

- `NO_CONTRACT_CHANGE_REQUIRED`: choose this for v0.49 local combat prototype with existing protocol/schema and no authoritative persistence.
- `CONTRACT_CHANGE_REQUIRED`: choose this before adding new combat result fields, status effects, actor stats, multi-target payloads, durable HP, inventory rewards, or economy effects.
- `BLOCKED_CONTRACT`: choose this if product requirements demand real mechanics that the current contracts cannot encode and no S0 approval exists.
- `FIX_REQUIRED`: choose this if evidence, validators, or source-package hygiene are inconsistent.
