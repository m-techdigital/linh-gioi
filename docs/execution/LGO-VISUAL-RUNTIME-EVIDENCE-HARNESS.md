# Visual Runtime Evidence Harness

Marker: `LGO_VISUAL_RUNTIME_EVIDENCE_HARNESS_READY`

Run from repo root:

```bash
./tools/lgo_visual_runtime_review.sh
```

The harness builds and launches the real macOS Unity Player at `1920x1080` without `-nographics`, starts the local dev API, drives the actual playable UI flow, and writes evidence to:

```text
build/visual-evidence/latest/
```

## Iteration Modes

For fast UI/visual iteration after a nearby full validation already passed:

```bash
LGO_VISUAL_RUNTIME_SOURCE_GATES=fast \
LGO_VISUAL_RUNTIME_SERVER_BUILD=skip \
LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 \
./tools/lgo_visual_runtime_review.sh
```

For checkpoint closure or broader changes:

```bash
LGO_VISUAL_RUNTIME_SOURCE_GATES=full \
LGO_VISUAL_RUNTIME_SERVER_BUILD=full \
LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=1 \
./tools/lgo_visual_runtime_review.sh
```

Supported source gate modes: `fast`, `full`, `skip`.
Supported server build modes: `fast`, `full`, `skip`.
`skip` requires the API jar to already exist and must not be used for closure claims.

For desktop/tablet/mobile evidence in one command:

```bash
./tools/lgo_visual_runtime_review_profiles.sh
```

Profile screenshots are written to:

```text
build/visual-evidence/profiles/desktop/
build/visual-evidence/profiles/tablet/
build/visual-evidence/profiles/mobile/
```

## Captured Checkpoints

- `login.png`
- `character-lobby.png`
- `character-select.png`
- `enter-world.png`
- `world-hub.png`
- `npc-dialogue.png`
- `session-menu.png`

Each run writes `visual-runtime-evidence-manifest.json` and `visual-runtime-evidence-review.md` with reference mapping. Build/capture success is not a `VISUAL_RUNTIME_PASS`; screenshots must still be inspected for layout, scale, spacing, sharpness, hierarchy, readability, asset quality, and similarity to reference.

## Boundaries

- Evidence output stays under `build/` and is not source-controlled.
- The player-facing UI remains Vietnamese.
- The harness does not add gameplay mechanics.
- It does not modify protocol, GameData schemas, ADRs, or design tokens.
