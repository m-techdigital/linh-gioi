# Runtime UI Quality Debt Triage v1.0

Marker: `LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY`

## Evidence Reviewed

- `build/visual-evidence/latest/login.png`
- `build/visual-evidence/latest/character-select.png`
- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/visual-runtime-evidence-review-vi.md`
- `docs/art/RUNTIME-ASSET-WATCH-QUEUE.md`

## Triage Result

| Priority | Area | Current issue | Safe next action |
|---:|---|---|---|
| 1 | Login | Background/logo are strong, but the Gate Keeper reads pasted-on/floating and the CTA panel still feels like a tool panel over final art. | `LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0` |
| 2 | World Hub | Actors/props are readable, but the training ground remains visually flat compared with the login reference quality. | Add a lightweight procedural depth pass before importing more world art. |
| 3 | Character Hall | Layout is functional and framed, but dense glass/card nesting still feels heavier than the login reference. | Continue component-level density/panel simplification. |
| 4 | Runtime assets | Several V3B world sprites sit close to role byte budgets. | Optimize only when visual evidence shows no quality regression. |
| 5 | Evidence | `enter-world.png` and `world-hub.png` can legitimately duplicate in steady state, but review notes should keep calling this out. | Keep duplicate-frame classification as review-required, not pass/fail. |

## Decision

The next highest-value safe fix is `LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0`.

Reasons:

- Login is the first player-facing screen and sets the perceived production quality.
- The fix can be done in UI/runtime presentation without opening gameplay, protocol, schema, ADR, or design-token work.
- It uses existing V3B candidate assets and code styling, avoiding new large images or composite slicing.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` is claimed.
- No production-final art quality is claimed.
- No new gameplay, auth, DB, economy, social, liveops, protocol, GameData schema, ADR, or design-token change is included.
