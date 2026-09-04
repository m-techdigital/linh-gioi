# Next Batch: M6 Contract Review And Combat Foundation

Before coding, read `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/LGO-MASTER-ROADMAP-v1.0.md`, and `docs/execution/LGO-MILESTONE-DEPENDENCY-MAP-v1.0.md`.

Do not start combat blindly. First create a `CONTRACT_CHANGE_REQUEST` if protocol or GameData changes are needed.

Allowed paths: docs/execution, docs/tasks, tools, and explicitly approved protocol/GameData/client/server paths after owner approval.

Forbidden paths before approval: `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`, production auth, DB, inventory, loot, economy, social, live ops.

Source gates: `git --no-pager diff --check`, governance validator, roadmap validator, project-state validator, relevant M4/M5/M6 validators.

Runtime gates if implementation opens: Unity build plus combat foundation runtime smoke and inherited playable smoke markers.

Package/commit/tag/push: produce handoff, changed-files, deletions; commit only after gates pass; tag only after accepted decision.

Non-claims: no production combat, auth, DB, economy, social, production art, production admin/player portal, or runtime PASS without marker.

Final decision tokens: `LGO_M6_CONTRACT_REVIEW_READY`, `LGO_M6_COMBAT_FOUNDATION_RUNTIME_CANDIDATE`, or `LGO_M6_COMBAT_FOUNDATION_FIX_REQUIRED`.

Code quality: enforce no duplicated code/logic and include ownership audit.
