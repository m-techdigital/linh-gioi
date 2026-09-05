# LGO Runtime Asset Watch Queue

Marker: `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`

| Priority | Role | Size | Budget | Margin | Source Max | Standalone | Android | iPhone | Action |
|---:|---|---:|---:|---:|---:|---:|---:|---:|---|
| 1 | `world_player_male_cultivator` | 173.7 KB | 180.0 KB | 6.3 KB | 512 | 512 | 512 | 512 | next optimization candidate |
| 2 | `world_rock_moss` | 48.0 KB | 55.0 KB | 7.0 KB | 256 | 256 | 256 | 256 | next optimization candidate |
| 3 | `world_tree_pine` | 82.4 KB | 90.0 KB | 7.6 KB | 256 | 256 | 256 | 256 | next optimization candidate |
| 4 | `world_tree_cherry` | 79.3 KB | 90.0 KB | 10.7 KB | 256 | 256 | 256 | 256 | next optimization candidate |
| 5 | `world_bridge_wood` | 78.1 KB | 90.0 KB | 11.9 KB | 512 | 512 | 512 | 512 | next optimization candidate |
| 6 | `world_spirit_gate` | 295.3 KB | 320.0 KB | 24.7 KB | 512 | 512 | 512 | 512 | keep current source; enforce platform max texture |
| 7 | `login_background` | 444.6 KB | 512.0 KB | 67.4 KB | 2048 | 2048 | 1024 | 1536 | keep JPEG source; rely on 1024/1536 mobile/tablet max texture |

## Policy

- Do not recompress transparent PNGs blindly; compare runtime screenshots before replacement.
- No runtime art replacement is performed by this report.
- Do not add animation frames for WATCH character/prop roles without a per-frame budget.
- Prefer platform import profiles before adding duplicate mobile image folders.
- V3B remains runtime candidate art, not production final art.
- Priority is sorted by smallest budget margin first, not visual importance.
