# Login Gate Entry Visual v1

Marker: `LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1`

## Scope

Upgrade the playable login screen to a polished gate-entry presentation using V3B runtime-candidate assets only.

## Allowed

- `client/Unity/Assets/Game/Art/Runtime/V3B/**`
- `client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3B.cs`
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
- Runtime-ready V3B assets are mapped and loaded through a registry.
- V3BA and FinalLogin runtime candidate folders are not used by the login runtime.
- Reference-only images are not imported.
- Validation and runtime smoke pass locally when Unity is available.
