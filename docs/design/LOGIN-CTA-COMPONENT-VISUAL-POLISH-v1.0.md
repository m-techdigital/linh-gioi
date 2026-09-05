# Login CTA Component Visual Polish v1.0

Status: `LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY`

## Purpose

Runtime screenshot review showed the login background, logo, and Gate Keeper are strong enough to expose the CTA/server stack as the weakest first-screen element. This pass improves that stack without importing new images or increasing runtime asset weight.

## Changes

- Increased the responsive login CTA backing opacity from a near-invisible glass wash to a clearer dark stage panel.
- Raised login card min-height and padding slightly on desktop/tablet/mobile so the CTA stack reads as one intentional component.
- Strengthened the lightweight gold/spirit frame on the login CTA backing.
- Strengthened the server selector frame so it feels related to the primary CTA instead of a flat black strip.
- Added `LGO Login CTA Component Visual Polish v1` marker for future validators and evidence review.

## Lightweight Rule

This is UI Toolkit styling only. It does not add PNGs, atlases, textures, audio, scenes, or gameplay systems.

## Non-Claims

- No production art claim.
- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay behavior change.
- No protocol, GameData, ADR, or design-token JSON change.

## Follow-Up

Continue with `LGO-LOGIN-CTA-COMPONENT-EVIDENCE-REFRESH-v1.0`: capture and review Login plus post-login checkpoints to confirm the CTA component still scales correctly and does not block the login flow.
