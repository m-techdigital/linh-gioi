# LG-M0-S1 — Unity Foundation

## Goal
Create the real Unity 6.3 LTS client foundation without redesigning M0 contracts.

## Allowed paths
- `client/Unity/**` except `Assets/Game/UI/**` implementation owned by S3.

## Forbidden paths
- `protocol/**`
- `gamedata/**`
- `server/**`
- `docs/adr/**`

## Required work
1. Initialize/open Unity 6.3 LTS project in `client/Unity`.
2. Use URP appropriate for mobile/PC stylized 3D.
3. Create bootstrap scene and a minimal `GameBootstrap` composition root.
4. Create assembly-definition boundaries for Foundation, GameData, Networking, Character, Combat, World, Social, Inventory and Tests where dependency relationships make sense.
5. Integrate generated protobuf C# output without manually editing generated classes.
6. Implement minimal realtime client abstraction sufficient to send `ClientHello` and parse `ServerHello` when S2 is available.
7. Expose connection state to UI through an interface/event, not direct UI dependency.
8. Add edit/play mode tests for bootstrap/network serialization where practical.

## Explicit non-goals
- no combat;
- no character controller production implementation;
- no world event;
- no guild/market/housing;
- no custom UDP.

## Acceptance
- Unity project opens cleanly;
- bootstrap scene runs with no exception;
- assemblies compile without circular references;
- ClientHello round-trip serialization test passes;
- connection integration can be exercised against S2 after merge.

## Handoff
List Unity version, packages added, assembly graph, scenes/prefabs created and exact tests run.
