# LGO Runtime Asset Watch Queue Prioritization v1.0

Status: `LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY`

## Scope

This task makes the runtime asset watch queue explicitly ordered, so continuous development optimizes high-risk assets before adding more visual variants.

## Implemented

- `tools/report_lgo_runtime_asset_watch_queue.py` now prints a `Priority` column sorted by smallest budget margin first.
- `docs/art/RUNTIME-ASSET-WATCH-QUEUE-PRIORITY.md` records the priority rule and current high-risk roles.
- `tools/validate_lgo_runtime_asset_watch_queue_priority.py` protects the priority report and docs.

## Validation

- `python3.12 tools/validate_lgo_runtime_asset_watch_queue_priority.py`
- `python3.12 tools/report_lgo_runtime_asset_watch_queue.py`

## Follow-Up

Continue with `LGO-RUNTIME-ASSET-WATCH-QUEUE-EVIDENCE-REFRESH-v1.0`: refresh the generated watch queue report after adding the priority column.
