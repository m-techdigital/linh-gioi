# Runtime Asset Watch Queue

Marker: `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`

## Purpose

This document turns near-budget runtime candidates into an explicit queue so the project does not keep adding beautiful but heavy images without profile-aware delivery rules.

## Current Watch Roles

| Role | Reason | Current Decision |
|---|---|---|
| `login_background` | large by design, 444.6 KB / 512 KB | keep JPEG source; rely on Android 1024 and iPhone 1536 max texture profiles |
| `world_spirit_gate` | 295.3 KB / 320 KB | keep current source; next candidate for PNG optimization when visual comparison is available |
| `world_player_male_cultivator` | 173.7 KB / 180 KB | do not add animation frames without stricter per-frame budgets |
| `world_tree_pine` | 82.4 KB / 90 KB | reuse sparingly; avoid multiplying prop variants before quantization evidence |
| `world_tree_cherry` | 79.3 KB / 90 KB | reuse sparingly; avoid multiplying prop variants before quantization evidence |
| `world_bridge_wood` | 78.1 KB / 90 KB | keep as scenic prop, not repeated tile asset |
| `world_rock_moss` | 48.0 KB / 55 KB | keep as sparse prop; optimize if duplicated heavily |

## Import Profile Policy

- Standalone may keep current runtime max texture for desktop review.
- Android should use smaller max texture settings from the role profile.
- iPhone/tablet should use the middle profile where it preserves readability.
- No runtime art replacement or recompression should happen without screenshot comparison.
- No production art claim; V3B assets remain runtime candidates.

## Commands

```bash
python3.12 tools/report_lgo_runtime_asset_watch_queue.py
python3.12 tools/validate_lgo_runtime_asset_watch_queue_import_profile.py
```
