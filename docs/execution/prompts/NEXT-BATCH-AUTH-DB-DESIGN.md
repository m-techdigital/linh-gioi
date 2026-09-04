# Next Batch: Auth DB Design

Before coding, read `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/LGO-MASTER-ROADMAP-v1.0.md`, and `docs/execution/LGO-MILESTONE-DEPENDENCY-MAP-v1.0.md`.

DB-0/Auth design comes first. Do not implement production DB without ERD, migration, security boundary, backup/restore, concurrency, and audit rules. Do not implement production auth without login/register UX, token/session/logout/expiry, secret handling, and dev-login migration plan.

Allowed paths: docs/execution, docs/tasks, docs/design, tools.

Forbidden paths: `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`, production DB code, production auth code, combat, economy, social, live ops.

Source gates: `git --no-pager diff --check`, governance validator, roadmap validator, project-state validator.

Runtime gates: not relevant until implementation opens after accepted design.

Package/commit/tag/push: handoff, changed-files, deletions, commit/tag/push after gates.

Non-claims: no auth implementation, DB implementation, runtime progress, production readiness, combat, production admin/player portal, or production art.

Final decision tokens: `LGO_AUTH_DB_DESIGN_READY` or `LGO_AUTH_DB_DESIGN_FIX_REQUIRED`.

Code quality: enforce no duplicated code/logic and include ownership audit.
