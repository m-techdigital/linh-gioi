# Next Batch: Web Landing And Admin Spec

Before coding, read `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/LGO-MASTER-ROADMAP-v1.0.md`, and `docs/execution/LGO-MILESTONE-DEPENDENCY-MAP-v1.0.md`.

Public website can start earlier as a player-facing game-style landing surface. Player portal starts after Auth/DB. Admin-dev console starts after roadmap/content ownership. Admin-prod starts after Auth/DB/content/economy and security review. Admin must be ops-first with Linh Gioi skin.

Allowed paths: docs/execution, docs/design, docs/art, docs/tasks, tools, future web/site paths only if explicitly opened.

Forbidden paths: `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`, production auth, DB, combat, economy, social, live ops.

Source gates: `git --no-pager diff --check`, governance validator, roadmap validator, project-state validator; visual validator if UI/site implementation opens.

Runtime gates if relevant: local site smoke or visual evidence, but no game runtime PASS claim.

Package/commit/tag/push: handoff, changed-files, deletions, commit/tag/push after gates.

Non-claims: no player portal/admin-prod implementation, no production auth/DB, no gameplay/combat, no production art.

Final decision tokens: `LGO_WEB_LANDING_ADMIN_SPEC_READY`, `LGO_PUBLIC_WEBSITE_CANDIDATE`, or `LGO_WEB_ADMIN_SPEC_FIX_REQUIRED`.

Code quality: enforce no duplicated code/logic and include ownership audit.
