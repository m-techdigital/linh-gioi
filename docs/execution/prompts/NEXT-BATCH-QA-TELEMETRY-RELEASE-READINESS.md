# Next Batch: QA Telemetry Release Readiness

Before coding, read `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/LGO-MASTER-ROADMAP-v1.0.md`, and `docs/execution/LGO-MILESTONE-DEPENDENCY-MAP-v1.0.md`.

Define telemetry event schema, runtime smoke matrix, visual evidence matrix, crash/error reporting plan, and alpha/beta/live checklists.

Allowed paths: docs/execution, docs/tasks, tools, tests only if explicitly opened for validator/smoke harness work.

Forbidden paths: `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`, production analytics service integration, gameplay, combat, auth, DB, economy, social, live ops.

Source gates: `git --no-pager diff --check`, governance validator, roadmap validator, project-state validator, new QA validators if added.

Runtime gates if relevant: smoke matrix commands must classify PASS, UNVERIFIED_ENVIRONMENT, DEFERRED, and NOT CLAIMED.

Package/commit/tag/push: handoff, changed-files, deletions, commit/tag/push after gates.

Non-claims: no release readiness, production monitoring, production auth/DB, gameplay/combat, or production art unless accepted gates exist.

Final decision tokens: `LGO_QA_TELEMETRY_RELEASE_READINESS_READY` or `LGO_QA_TELEMETRY_FIX_REQUIRED`.

Code quality: enforce no duplicated code/logic and include ownership audit.
