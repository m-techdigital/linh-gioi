# Linh Gioi Online Next 50 Tasks Backlog v1.0

Decision marker: LGO_MASTER_ROADMAP_ACCEPTED_v1.0.

Code governance: every task requires code governance audit from `docs/execution/CODE-GOVERNANCE-CONTRACT.md`.

| ID | Purpose | Allowed paths | Forbidden paths | Validators | Runtime gates | Dependency | Closure |
|---|---|---|---|---|---|---|---|
| LGO-TASK-001 | Combat contract review | docs, tools | client/server/protocol/gamedata unless opened | code governance, master roadmap | not claimed | v0.32 accepted | LGO_COMBAT_CONTRACT_REVIEW_READY |
| LGO-TASK-002 | Protocol change request for combat if needed | docs/protocol request only | direct schema mutation without approval | governance | not claimed | 001 | LGO_COMBAT_PROTOCOL_REQUEST_READY |
| LGO-TASK-003 | GameData combat request if needed | docs/gamedata request only | schemas without approval | governance | not claimed | 001 | LGO_COMBAT_GAMEDATA_REQUEST_READY |
| LGO-TASK-004 | First combat smoke design | tools/docs | gameplay implementation | governance | not claimed | 001 | LGO_COMBAT_SMOKE_DESIGN_READY |
| LGO-TASK-005 | Combat foundation implementation | approved paths only | unapproved frozen paths | source validators | Unity/runtime required | 001-004 | LGO_COMBAT_FOUNDATION_RUNTIME_CANDIDATE |
| LGO-TASK-006 | Combat rejection path | approved paths only | economy/inventory | source validators | runtime required | 005 | LGO_COMBAT_REJECTION_PATH_CLOSED |
| LGO-TASK-007 | Combat no-regression playable smoke | tools | gameplay expansion | closure validators | runtime required | 005 | LGO_COMBAT_PLAYABLE_REGRESSION_PASS |
| LGO-TASK-008 | Auth/DB ERD | docs | implementation paths | governance | not claimed | roadmap | LGO_AUTH_DB_ERD_READY |
| LGO-TASK-009 | Auth security boundary | docs | production auth code | governance | not claimed | 008 | LGO_AUTH_SECURITY_BOUNDARY_READY |
| LGO-TASK-010 | Dev login migration plan | docs/tools | production auth code | governance | not claimed | 009 | LGO_DEV_LOGIN_MIGRATION_PLAN_READY |
| LGO-TASK-011 | Login/register UX spec | docs/design | client implementation | governance | visual review deferred | 009 | LGO_AUTH_UX_SPEC_READY |
| LGO-TASK-012 | Token/session/logout design | docs | production auth code | governance | not claimed | 009 | LGO_AUTH_SESSION_DESIGN_READY |
| LGO-TASK-013 | Auth implementation slice | approved auth paths | DB without ERD | source validators | runtime required | 008-012 | LGO_AUTH_RUNTIME_CANDIDATE |
| LGO-TASK-014 | DB migration foundation | approved DB paths | gameplay | source validators | runtime required | 008 | LGO_DB_MIGRATION_FOUNDATION_READY |
| LGO-TASK-015 | Account/player/character persistence | approved DB/API paths | inventory/economy | source validators | runtime required | 014 | LGO_DB_CHARACTER_RUNTIME_CANDIDATE |
| LGO-TASK-016 | Progression/training persistence | approved DB/API paths | economy/social | source validators | runtime required | 015 | LGO_TRAINING_PERSISTENCE_READY |
| LGO-TASK-017 | Backup/restore rehearsal | tools/docs | gameplay | validators | runtime required | 014 | LGO_DB_BACKUP_RESTORE_READY |
| LGO-TASK-018 | Concurrency/audit design | docs/server tests | production admin | validators | runtime deferred | 014 | LGO_DB_AUDIT_DESIGN_READY |
| LGO-TASK-019 | Item catalog spec | docs/gamedata request | schemas without approval | governance | not claimed | DB design | LGO_ITEM_CATALOG_SPEC_READY |
| LGO-TASK-020 | Inventory shell spec | docs/UI | implementation | governance | not claimed | 019 | LGO_INVENTORY_SHELL_SPEC_READY |
| LGO-TASK-021 | Equipment spec | docs | implementation | governance | not claimed | 020 | LGO_EQUIPMENT_SPEC_READY |
| LGO-TASK-022 | Rewards spec | docs | economy implementation | governance | not claimed | 019 | LGO_REWARDS_SPEC_READY |
| LGO-TASK-023 | Economy boundary spec | docs | economy implementation | governance | not claimed | Auth/DB | LGO_ECONOMY_BOUNDARY_READY |
| LGO-TASK-024 | Profile display spec | docs/UI | social implementation | governance | visual deferred | Auth | LGO_PROFILE_DISPLAY_SPEC_READY |
| LGO-TASK-025 | Presence model spec | docs/server | implementation | governance | runtime deferred | Auth/DB | LGO_PRESENCE_MODEL_READY |
| LGO-TASK-026 | Friend list spec | docs | implementation | governance | runtime deferred | 025 | LGO_FRIEND_LIST_SPEC_READY |
| LGO-TASK-027 | Chat safety envelope spec | docs | chat implementation | governance | runtime deferred | 025 | LGO_CHAT_ENVELOPE_SPEC_READY |
| LGO-TASK-028 | Party lifecycle spec | docs | party implementation | governance | runtime deferred | 025 | LGO_PARTY_SPEC_READY |
| LGO-TASK-029 | Guild shell spec | docs | guild implementation | governance | runtime deferred | 026 | LGO_GUILD_SPEC_READY |
| LGO-TASK-030 | Leaderboard/event board spec | docs | live ops implementation | governance | runtime deferred | DB/content | LGO_LEADERBOARD_SPEC_READY |
| LGO-TASK-031 | Content taxonomy | docs | gamedata schemas without approval | governance | not claimed | roadmap | LGO_CONTENT_TAXONOMY_READY |
| LGO-TASK-032 | Map/zone model spec | docs | implementation | governance | not claimed | 031 | LGO_ZONE_MODEL_READY |
| LGO-TASK-033 | NPC/dialogue content pipeline | docs/tools | production DB | validators | runtime deferred | 031 | LGO_DIALOGUE_PIPELINE_READY |
| LGO-TASK-034 | Skill/effect content pipeline | docs/tools | combat without contract | validators | runtime deferred | 001 | LGO_SKILL_EFFECT_PIPELINE_READY |
| LGO-TASK-035 | World event/boss event spec | docs | live ops implementation | governance | not claimed | 031 | LGO_WORLD_EVENT_SPEC_READY |
| LGO-TASK-036 | Content validation admin-dev plan | docs | admin-prod | governance | not claimed | 031 | LGO_CONTENT_ADMIN_DEV_PLAN_READY |
| LGO-TASK-037 | Public website spec | docs/web plan | auth/portal implementation | governance | visual evidence | roadmap | LGO_PUBLIC_WEBSITE_SPEC_READY |
| LGO-TASK-038 | Public website implementation | web/site paths | Auth/DB/admin-prod | validators | visual evidence | 037 | LGO_PUBLIC_WEBSITE_CANDIDATE |
| LGO-TASK-039 | Player portal spec | docs | implementation | governance | not claimed | Auth/DB | LGO_PLAYER_PORTAL_SPEC_READY |
| LGO-TASK-040 | Admin-dev console spec | docs | admin-prod | governance | visual deferred | content roadmap | LGO_ADMIN_DEV_SPEC_READY |
| LGO-TASK-041 | Admin-prod console spec | docs | implementation | governance | not claimed | Auth/DB/content/economy | LGO_ADMIN_PROD_SPEC_READY |
| LGO-TASK-042 | Asset provenance rules | docs/art | runtime art replacement | governance | visual deferred | roadmap | LGO_ASSET_PROVENANCE_READY |
| LGO-TASK-043 | Sprite sheet import plan | docs/art | production claim | governance | visual deferred | 042 | LGO_SPRITE_IMPORT_PLAN_READY |
| LGO-TASK-044 | Animation direction plan | docs/art | implementation | governance | visual deferred | 043 | LGO_ANIMATION_DIRECTION_READY |
| LGO-TASK-045 | UI atlas/import settings | docs/art/UI | design tokens | governance | visual deferred | 042 | LGO_UI_ATLAS_PLAN_READY |
| LGO-TASK-046 | Telemetry event schema plan | docs/tools | production analytics code | governance | runtime deferred | QA roadmap | LGO_TELEMETRY_SCHEMA_PLAN_READY |
| LGO-TASK-047 | Runtime smoke matrix | docs/tools | gameplay implementation | validators | runtime deferred | governance | LGO_RUNTIME_SMOKE_MATRIX_READY |
| LGO-TASK-048 | Visual evidence matrix | docs/tools | production art claim | validators | visual evidence | governance | LGO_VISUAL_EVIDENCE_MATRIX_READY |
| LGO-TASK-049 | Crash/error reporting plan | docs/tools | production service integration | governance | runtime deferred | QA roadmap | LGO_CRASH_REPORTING_PLAN_READY |
| LGO-TASK-050 | Alpha/beta/live checklist | docs | implementation | governance | not claimed | production roadmap | LGO_RELEASE_CHECKLIST_READY |
