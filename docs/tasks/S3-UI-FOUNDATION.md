# LG-M0-S3 — UI Foundation

## Goal
Implement the reusable UI design-system foundation matching `docs/06-UI-DESIGN-SYSTEM.md` and reference art, without building feature screens.

## Allowed paths
- `client/Unity/Assets/Game/UI/**`

## Forbidden paths
- gameplay/network/server/protocol/gamedata implementation
- production gameplay scenes owned by S1

## Required work
1. Convert `design-tokens.json` into Unity-consumable theme assets without duplicating a second independent palette.
2. Implement reusable primitives: BaseButton, IconButton, BasePanel, ModalPanel, ProgressBar, HealthBar, ManaBar, SkillButton placeholder, AvatarView placeholder, Nameplate, TabBar, Toast, CurrencyDisplay.
3. Create a UI showcase/test scene or prefab collection owned entirely by S3; do not edit S1 production bootstrap scene.
4. Demonstrate mobile safe-area handling and desktop responsive sizing.
5. Provide an initial typography/icon strategy with replaceable asset references rather than hardcoded proprietary assets.
6. Tests/checks for token/component loading where feasible.

## Acceptance
- showcase renders all required primitives;
- no local arbitrary palette contradicts tokens;
- components work at representative 16:9 desktop and tall phone aspect;
- no direct dependency on combat implementation.
