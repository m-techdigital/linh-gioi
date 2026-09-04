# Handoff — LG M6 Source/Gate Consistency Hotfix v0.46.1

Decision: `M6_SOURCE_GATE_CONSISTENCY_HOTFIX_CLOSED_v0.46.1`

## What Changed

- Decoupled source hygiene validation from historical release ZIP requirements.
- Added explicit artifact validation modes for release-package checks.
- Prevented Python validation bytecode writes during playable closure orchestration.
- Included the M2 v0.6.2 local runtime candidate runbook in tracked source so fresh unzip validation keeps the required markers.

## Review Files

- `M6-SOURCE-GATE-CONSISTENCY-HOTFIX-FINAL-REPORT-v0.46.1.md`
- `LGO-M6-SOURCE-GATE-CONSISTENCY-HOTFIX-v0.46.1-CHANGED-FILES.txt`
- `LGO-M6-SOURCE-GATE-CONSISTENCY-HOTFIX-v0.46.1-DELETIONS.txt`
- `LGO-M6-SOURCE-GATE-CONSISTENCY-HOTFIX-v0.46.1-ARTIFACTS-SHA256.txt`

## Gates

Source-only and package-ready gates pass locally after this hotfix. Fresh unzip validation passes without historical ZIPs in the source root.

## Frozen Surface Audit

Unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

## Non-Claims

No gameplay, combat mechanic, production art, DB/auth, economy, social, live-ops, or full MMO runtime closure is claimed.
