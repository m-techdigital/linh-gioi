# 10 — Integration and Sandbox Rules

## Roles

- S0: Architect / control tower.
- S1: Unity client/gameplay/world/network client.
- S2: Java API/realtime/backend.
- S3: UI/UX and reusable UI library.
- S4: GameData/content.
- S5: QA/tooling/CI/contracts/load harness.

## Baseline rule

Every parallel batch starts from the same exact baseline commit/tag.

Example:

```text
develop @ ABC123
  |- sbx/client-m01
  |- sbx/server-m01
  |- sbx/ui-m01
  |- sbx/content-m01
  `- sbx/qa-m01
```

A sandbox may not silently rebase itself onto a different functional baseline and hand off as if unchanged.

## Hot files

`protocol/**`, schema files, architecture docs, production scenes/prefabs and database migration history are hot files.

Only their designated owner may mutate them inside a batch.

## Scene/prefab rule

One production scene or root prefab has one owner per batch. Other lanes deliver composable prefabs/assets; the scene owner integrates them.

Smart/YAML merge is fallback conflict resolution, not a normal multi-owner workflow.

## Contract change workflow

1. Feature lane records required contract change.
2. S0 decides/updates contract and ADR if necessary.
3. Generated outputs/fixtures refresh.
4. Consumer lanes implement against the new contract.

## Merge sequence default

1. contract/foundation;
2. backend/server;
3. client core;
4. UI;
5. content;
6. QA/tools;
7. integrated verification.

If there is no shared-file dependency, independent lanes may merge in another order after S0 review.

## Handoff

Every lane includes:
- task ID;
- baseline commit;
- commits;
- changed/add/delete files;
- contracts consumed/changed;
- migrations;
- tests executed/results;
- manual verification;
- known limitations;
- integration notes.
