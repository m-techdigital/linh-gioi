# Login Gate Entry Visual v1

Marker: `LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1`

## Scope

Upgrade the playable login screen to a polished gate-entry presentation using separated runtime-candidate assets from V3B-A.

## Allowed

- `client/Unity/Assets/Game/Art/Runtime/V3BA/**`
- `client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3BA.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- docs and validators directly related to login visual integration

## Forbidden

- production auth
- DB/session persistence
- economy/social/liveops
- protocol changes
- GameData schema changes
- ADR changes
- design-token changes

## Acceptance

- Login opens as a final-looking gate-entry shell.
- Runtime-ready assets are mapped and loaded through a registry.
- Reference-only images are not imported.
- Validation and runtime smoke pass locally when Unity is available.
