# M3-B Unity Account / Character Integration Closure Checklist

## Input provenance

- [ ] Full-source ZIP SHA matches `6110c4d1043eae7650df72b8386b9784bf91f1fd9523490ddd26f5c0308ab968`.
- [ ] Corrected M3 delta is used only if applying from v0.6.2; stale `dd0e...` delta is not used.
- [ ] M3 final decision is understood as server/API persistence closure only.

## Source gates

- [ ] `ClientRuntimeConfig` includes API base URL and timeout validation.
- [ ] `LinhGioi.Account` asmdef exists and does not create an asmdef cycle.
- [ ] Unity account API client uses real `UnityWebRequest` HTTP calls.
- [ ] Unity client covers login, list, create, load, and save position.
- [ ] Unity smoke runner has an `--lgo-m3b-expect-existing` restart path.
- [ ] EditMode/source tests cover API config and character list JSON parsing.
- [ ] M3-B validator PASS.

## Runtime gates

- [ ] Java 25 real runtime PASS.
- [ ] Maven 3.9.16 real runtime PASS.
- [ ] Server build PASS.
- [ ] Server tests PASS with executed count > 0 and skipped = 0.
- [ ] M3 API persistence restart smoke PASS.
- [ ] Current Unity Linux player is built from this exact M3-B source.
- [ ] Unity M3-B player smoke first pass PASS.
- [ ] API restart performed.
- [ ] Unity M3-B player smoke restart pass PASS.
- [ ] Raw `m3b-unity-dev-key` is not persisted.
- [ ] No Java server orphan remains after smoke.

## Frozen contract / hygiene

- [ ] `protocol/**` unchanged.
- [ ] `gamedata/schemas/**` unchanged.
- [ ] `docs/adr/**` unchanged.
- [ ] product/GDD/TDD/network/GameData/design-token contracts unchanged.
- [ ] Delta has no parent wrapper.
- [ ] Delta excludes `Library/`, `Temp/`, `Logs/`, `target/`, `build/`, `.env`, toolchains, secrets, and generated disposable Unity assets.

## Final decision vocabulary

- `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_CLOSED`
- `M3B_SERVER_ONLY_SOURCE_CLOSED_UNITY_ENV_LIMITED`
- `FIX_REQUIRED`
- `BLOCKED_CONTRACT`
