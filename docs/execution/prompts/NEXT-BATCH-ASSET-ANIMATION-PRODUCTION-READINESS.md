# Next Batch: Asset Animation Production Readiness

Before coding, read `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/LGO-MASTER-ROADMAP-v1.0.md`, and `docs/execution/LGO-MILESTONE-DEPENDENCY-MAP-v1.0.md`.

Use accepted image packs as references. Define sprite sheets, animation direction, UI atlas/import settings, provenance, and license rules. Do not claim production art without provenance, import settings, visual evidence, and owner acceptance.

Allowed paths: docs/art, docs/reference-art, docs/design, docs/execution, docs/tasks, tools.

Forbidden paths: `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, `client/Unity/Assets/Game/UI/design-tokens.json`, gameplay, combat, auth, DB, economy, social, live ops.

Source gates: `git --no-pager diff --check`, governance validator, roadmap validator, project-state validator, art/reference validators if added.

Runtime gates if relevant: visual evidence only; no gameplay runtime PASS claim.

Package/commit/tag/push: handoff, changed-files, deletions, artifact SHA if image packages are produced, commit/tag/push after gates.

Non-claims: no production art unless provenance/import/owner gates pass, no gameplay/combat/auth/DB implementation.

Final decision tokens: `LGO_ASSET_ANIMATION_PRODUCTION_READINESS_READY` or `LGO_ASSET_ANIMATION_FIX_REQUIRED`.

Code quality: enforce no duplicated code/logic and include ownership audit.
