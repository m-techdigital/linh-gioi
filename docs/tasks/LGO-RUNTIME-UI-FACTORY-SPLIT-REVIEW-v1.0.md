# LGO Runtime UI Factory Split Review v1.0

Status: `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`

## Scope

This pass reviews whether the playable UI controller should be split after RuntimeUiSkin adoption.

## Decision

- No broad screen-level split yet.
- Leaf-level stateless UI factory split is allowed next.
- Runtime styling remains owned by `RuntimeUiSkin`.

## Non-Claims

- No gameplay change.
- No runtime image payload change.
- No production art claim.
- No visual runtime PASS claim.

## Follow-Up

Continue with `LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0`.
