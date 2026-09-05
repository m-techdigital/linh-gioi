# LGO World Responsive Evidence Refresh v1.0

Marker: `LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY`

## Goal

Refresh desktop/tablet/mobile runtime evidence after World Hub depth work and tighten mobile HUD sizing where screenshots show the HUD covering too much of the scene.

## Changes

- Reduced the mobile World HUD width budget to a proportional viewport range.
- Lowered mobile HUD padding and key status text size while keeping objective and interaction copy readable.
- Preserved tablet/desktop HUD sizing and existing player-facing flow.
- Captured profile evidence under `build/visual-evidence/profiles/**`.

## Evidence Targets

- `build/visual-evidence/profiles/desktop/world-hub.png`
- `build/visual-evidence/profiles/tablet/world-hub.png`
- `build/visual-evidence/profiles/mobile/world-hub.png`
- `build/visual-evidence/profiles/mobile/login.png`

## Non-Claims

- No gameplay change.
- No protocol, GameData schema, ADR, or design-token change.
- No new runtime image import.
- No `VISUAL_RUNTIME_PASS` claim from capture alone.

## Decision

`LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY`
