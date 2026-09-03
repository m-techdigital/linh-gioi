# 12 — Design Preproduction Plan

This plan prepares the next milestones before implementation starts. It is intentionally non-functional: no scenes, prefabs, production art, content balancing, or combat code are added here.

## Design goals

- Keep Linh Giới Online friendly, readable, mobile-first, and lightweight.
- Make each milestone visually understandable before coding.
- Avoid implementing UI twice by defining screen intent, not production components.
- Preserve design tokens in `docs/06-UI-DESIGN-SYSTEM.md` and `client/Unity/Assets/Game/UI/design-tokens.json` as the source of truth.

## Preproduction deliverables

| Area | Prepared artifact | When it becomes implementation |
|---|---|---|
| Roadmap | `roadmap-flow.svg` | Never; planning only. |
| Gate flow | `m0-to-m1-gate.svg` | Never; planning only. |
| Core loop | `core-gameplay-loop.svg` | M1 combat prototype. |
| HUD | `hud-wireframe.svg` | M1 UI prototype. |
| World hub | `world-hub-wireframe.svg` | M2/M3 navigation prototype. |
| Production board | `production-board.svg` | All future handoff planning. |

## M1 visual intent

M1 should prove a small offline combat loop:

1. enter safe prototype scene;
2. spawn player and simple enemy fixture;
3. basic attack/cooldown/resource feedback;
4. HP state changes;
5. encounter ends;
6. return to safe state.

No final art, monetization, live economy, guild, marketplace, or production world expansion belongs in M1.

## UX acceptance notes

- Mobile HUD must keep primary controls thumb-reachable.
- Desktop controls may exist but must not define mobile layout.
- Text must be readable at small screen sizes.
- Loading/error/reconnect states must be represented before online expansion.
- Every visible runtime state should map to a server/client truth source once online work starts.

## Reference board locations

See `docs/reference-art/design-boards/README.md`.
