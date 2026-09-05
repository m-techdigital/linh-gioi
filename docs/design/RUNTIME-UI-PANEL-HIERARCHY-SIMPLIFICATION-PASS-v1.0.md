# Runtime UI Panel Hierarchy Simplification Pass v1.0

Status: `LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY`

## Purpose

Reduce visible nested-frame noise in the playable UI while keeping the existing login, Character Hall, world, dialogue, and session behavior intact.

## Runtime Decision

- Login CTA backing keeps its glass-panel role but now uses softer edge emphasis so the V3B logo and gold CTA remain the visual focus.
- Character Hall nested surfaces now share `RuntimeUiSkin.ApplySubtleNestedFrame`.
- Character list, selected profile, create panel, and empty character card no longer each hand-roll a different high-contrast edge recipe.
- Parent panel framing remains stronger than child panel framing, so the hierarchy reads as shell first, nested content second.

## Non-Goals

- No gameplay or account/character flow change.
- No new asset import.
- No production art claim.
- No protocol, GameData, ADR, or design-token change.

## Follow-Up

Refresh runtime screenshots with `LGO-RUNTIME-UI-PANEL-HIERARCHY-EVIDENCE-REFRESH-v1.0` and review login/Character Hall panel balance on real player output.
