# LGO Runtime Asset Watch Queue Import Profile Polish v1.0

Status: `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`

## Scope

This pass adds a focused watch-queue report for runtime candidates that are close to their role budgets. It keeps optimization decisions tied to platform import profiles instead of ad hoc duplicate image folders.

## Changes

- Added `tools/report_lgo_runtime_asset_watch_queue.py`.
- Added `docs/art/RUNTIME-ASSET-WATCH-QUEUE.md`.
- Added validator coverage for watch-queue marker, report output, and closure integration.

## Non-Claims

- No production art claim.
- No runtime art replacement or recompression.
- No gameplay change.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

When adding the next character frame, prop pack, or large login/world image, consult this queue first and either keep within role budget or add profile evidence explaining the cost.
