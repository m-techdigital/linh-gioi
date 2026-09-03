# 06 — UI Design System v1

## Visual identity

**Neo-Asian Spirit Fantasy**

Core palette:

| Token | Hex | Role |
|---|---|---|
| `bg` | `#0B1324` | deep background |
| `surface` | `#111D32` | panels |
| `surfaceRaised` | `#182741` | raised surfaces |
| `spirit` | `#28D7C7` | primary interactive / spirit |
| `shadow` | `#9B5CFF` | corruption/world-event accent |
| `gold` | `#E6B85C` | prestige/currency |
| `danger` | `#E35D6A` | danger/HP/error |
| `text` | `#F5F2EA` | main text |
| `muted` | `#9BA7BC` | secondary text |

The generated concept images in `docs/reference-art/` are North Star references, not exact pixel specs.

## Spacing scale

Only use as defaults:
`4, 8, 12, 16, 24, 32, 48, 64`.

## Component ownership

Reusable base components are created once and consumed by feature UI. Required initial components:
- `BaseButton`;
- `IconButton`;
- `BasePanel`;
- `ModalPanel`;
- `ProgressBar`;
- `HealthBar`;
- `ManaBar`;
- `SkillButton`;
- `AvatarView`;
- `Nameplate`;
- `TabBar`;
- `Toast`;
- `CurrencyDisplay`.

Feature UI must not create visually unrelated local buttons/panels when a Base component exists.

## HUD modes

### Social HUD
Friends, chat, interact, emote, map/home access. Combat controls visually recede.

### Combat HUD
Attack, dodge, 4 skills, ultimate, spirit, target/status.

### Boss/Event HUD
Combat HUD plus boss health, phase/objective, party and contribution information.

## Mobile-first rules

- touch target target >=44 CSS/device-independent px equivalent where feasible;
- important action never depends on hover;
- controls respect safe area;
- text remains readable on common phone aspect ratios;
- desktop may bind hotkeys but must retain the same information hierarchy.
