# LG-M0-S4 — GameData Foundation

## Goal
Implement a deterministic, validated content pipeline around the canonical schema/source files in `gamedata/**`.

## Allowed paths
- `gamedata/**` except schema semantic changes require S0 approval/request.
- lane-local helper code may be proposed under `tools/gamedata/**` only if S5 has not claimed the same files; coordinate ownership before edit.

## Forbidden paths
- client/server production code;
- protocol;
- architecture ADRs.

## Required work
1. Validate YAML source documents against JSON Schema.
2. Add global duplicate-ID validation.
3. Add starter reference validation (`class_id`, map IDs or equivalent registry mechanism where references exist).
4. Produce deterministic compiled manifest/output with `gamedata_version` and content hash.
5. Provide positive fixtures and deliberately invalid fixtures.
6. Fail with actionable diagnostics including file and field/ID.

## Acceptance
- current sample data validates;
- duplicate ID fails;
- malformed skill bounds fail;
- invalid reference fails once reference registry is wired;
- identical source produces identical compiled hash/output.

## Non-goal
Do not invent the full skill/item/economy balancing model in M0.
