# M6 Combat Entry Checklist v0.48.0

Use this checklist before starting v0.49.

## Required Before Implementation

- [ ] v0.46.1 full-source is the working baseline.
- [ ] v0.47 visual acceptance is review-ready or owner accepted.
- [ ] v0.48 readiness spec is accepted.
- [ ] No frozen surfaces are dirty.
- [ ] Existing combat protocol and GameData schema are sufficient for the selected task.
- [ ] v0.49 scope is local prototype only.

## Frozen Surface Guard

- [ ] Do not edit `protocol/**`.
- [ ] Do not edit `gamedata/schemas/**`.
- [ ] Do not edit `docs/adr/**`.
- [ ] Do not edit `client/Unity/Assets/Game/UI/design-tokens.json`.

## Runtime Gates

- [ ] Source gates PASS.
- [ ] Package hygiene PASS.
- [ ] Unity compile/player build PASS when environment is available.
- [ ] Local combat smoke PASS.
- [ ] Visual evidence captured or marked `UNVERIFIED_ENVIRONMENT`.

## Failure Classification

- `NO_CONTRACT_CHANGE_REQUIRED`: proceed with existing contracts.
- `CONTRACT_CHANGE_REQUIRED`: stop and create S0 request before implementation.
- `BLOCKED_CONTRACT`: stop because requested behavior cannot be represented.
- `FIX_REQUIRED`: repair source, docs, validator, or package consistency first.
