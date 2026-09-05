# LGO Post-Login Visual Evidence Upload Packaging v1.0

Status: `LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY`

## Scope

This pass adds a repeatable lightweight package step for sharing current visual runtime evidence with human or AI review tools without uploading Unity caches, build players, PID files, or source-control noise.

## Package Command

```bash
python3.12 tools/package_lgo_visual_evidence_upload.py
```

Optional logs:

```bash
python3.12 tools/package_lgo_visual_evidence_upload.py --include-logs
```

The ZIP, manifest, and checksum are written to `build/chatgpt-handoff/`.

## Included Evidence

- desktop/tablet/mobile screenshots for login, character hall, enter world, world hub, near-object prompts, NPC dialogue, session menu, and target dummy state;
- profile index and profile review log;
- runtime manifests and heuristic reports;
- optional player/API/Unity logs only when requested.

## Excluded

- Unity `Library`, `Temp`, player binaries, package caches, PID files, and source ZIPs;
- protocol or GameData generated outputs;
- production art claims.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay mechanic change.
- No new runtime art import.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Upload the generated ZIP for review when useful, then continue with the next safe runtime UI/UX polish task from `docs/execution/NEXT-ACTION.md`.
