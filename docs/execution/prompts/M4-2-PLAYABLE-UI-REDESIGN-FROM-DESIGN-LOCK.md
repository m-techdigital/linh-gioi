# Future Prompt: M4-2 Playable UI Redesign From Design Lock

You are working on Linh Gioi Online. Start only after `docs/design/LGO-DESIGN-DIRECTION-LOCK-v0.11.0.md` is accepted by the project owner.

This is an implementation task for the existing M4 playable shell only.

Rules:

- Do not add gameplay systems.
- Do not touch protocol, GameData schemas, ADR, or design tokens.
- Use existing Unity UI Toolkit and existing M4 account/character/world flow.
- Keep M4-0 smoke passing.
- Keep M4-1 visual smoke passing.
- Preserve existing API client and DTOs.
- Do not claim final production UI.

Goal:

Improve the login, character lobby, and world HUD layout using the v0.11.0 design lock and wireframe spec. The result should feel like an early Vietnamese 2D/2.5D online RPG shell rather than a plain form or SaaS dashboard.

Required reads:

- `docs/design/LGO-DESIGN-DIRECTION-LOCK-v0.11.0.md`
- `docs/design/LGO-PLAYABLE-UI-WIREFRAME-SPEC-v0.11.0.md`
- `docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs`

Validation:

- `python3.12 tools/validate_m4_playable_source.py`
- `python3.12 tools/validate_m4_visual_foundation.py`
- When environment allows, build the macOS player and run M3-B, M4-0, and M4 visual smokes.

Expected non-claims:

- production auth not claimed
- DB persistence not claimed
- full MMO gameplay not claimed
- final production art not claimed
