# 00 — Baseline Lock

## Authority

All M0 Batch 01 work starts from the content of:

`linh-gioi-m0-foundation-v0.1.zip`

SHA256:

`2b8e30bb4e1206c5e6364615c17980d274bee97c065f04272b4e44779a347421`

Logical baseline identifier:

`lg-m00-spec-v01`

Until Git is initialized, **the ZIP hash is the source-provenance anchor**.

## Contracts that are frozen for feature lanes

The following are S0-owned hot areas:

- `protocol/**`
- `docs/adr/**`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/03-TDD.md`
- `docs/04-NETWORK-CONTRACT.md`
- `docs/05-GAMEDATA-CONTRACT.md`
- GameData schema semantics under `gamedata/schemas/**`
- global UI token semantics under `client/Unity/Assets/Game/UI/design-tokens.json`

Feature/tool lanes must not silently mutate these contracts.

## Contract defect rule

If a sandbox cannot complete its task without changing a frozen contract, it must stop that slice and produce:

`CONTRACT_CHANGE_REQUEST.md`

with:

- exact blocking requirement;
- current contract evidence;
- minimal proposed change;
- affected consumers;
- migration/compatibility impact;
- why a local workaround would create divergence.

S0 decides the change before implementation continues.

## Baseline drift rule

Because Git is deferred, every handoff must state:

- source baseline identifier;
- source baseline ZIP SHA256;
- all previously applied accepted overlays;
- output ZIP SHA256.

A sandbox with unknown provenance is **BLOCKED**, not assumed compatible.
