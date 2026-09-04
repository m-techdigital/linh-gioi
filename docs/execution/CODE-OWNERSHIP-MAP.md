# Linh Gioi Online Code Ownership Map v1.0

Decision marker: LGO_CODE_GOVERNANCE_CONTRACT_ACCEPTED_v1.0.

| Surface | Canonical owner | Rules |
|---|---|---|
| UI runtime | `client/Unity/Assets/Game/UI/**` | Owns UI state, panel flow, labels, local HUD controls, and player-facing presentation wiring. |
| World runtime | `client/Unity/Assets/Game/World/**` | Owns player marker, local world state, interaction state, landmarks, camera/world presentation, smoke hooks, and the world/runtime layer. |
| Art runtime catalog | `client/Unity/Assets/Game/Art/**` and `LinhGioi.Art` | Owns runtime-safe placeholder art references, palette materials, and art metadata. |
| Bootstrap | `client/Unity/Assets/Game/Bootstrap/**` | Owns startup composition and command-line smoke entry. |
| Foundation/config | `client/Unity/Assets/Game/Foundation/**` | Owns runtime config loading, shared client configuration, and stable local setup behavior. |
| Tools/validators | `tools/**` | Owns source validators, closure scripts, package hygiene, runtime harnesses, and generated-output cleanup. |
| Docs/tasks | `docs/tasks/**` | Owns task-specific scope, status, validation, non-claims, and handoff guidance. |
| Protocol | `protocol/**` | Frozen contract surface unless explicitly opened by S0 contract change. |
| GameData | `gamedata/**` and `gamedata/schemas/**` | Content and schema ownership; schemas are frozen unless explicitly opened. |
| Server/API/realtime | `server/**` | Owns Java API, persistence prototype, realtime session authority, backend tests, and server/API/realtime boundaries. |
| Runtime smoke harness | `tools/run_*`, `tools/*_runtime.py` | Owns executable evidence flow and runtime PASS marker production. |
| Packaging/handoff | Root handoff/report files, `tools/package_source.py`, package validators | Owns changed-files manifests, deletion records, SHA summaries, and source/delta package hygiene. |
| Visual/reference art | `docs/reference-art/**`, `docs/art/**` | Owns reference-only art packs, translation rules, provenance notes, and non-production art boundaries. |
| Future web/player/admin surfaces | Future `web/**`, `admin/**`, or agreed app path | Public website, player portal, admin-dev, and admin-prod must stay separate in ownership and UX intent. |

Cross-owner changes require the task to name the reason, gate, validator, and rollback path.
