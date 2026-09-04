# LGO Runtime Asset Weight Hygiene v1.0

Marker: `LGO_RUNTIME_ASSET_WEIGHT_HYGIENE_READY`

## Scope

Keep runtime art candidates and local handoff artifacts from making the project heavy before gameplay content exists.

Allowed:

- optimize Unity runtime candidate copies by role;
- allow JPEG for opaque login/background runtime copies;
- keep transparent sprites as PNG;
- move large handoff full-source ZIPs outside repo root;
- add validation for runtime asset file size budgets.
- define device delivery profiles for mobile/tablet/PC before large art expansion.

Not allowed:

- No production art claim.
- No composite sheet slicing.
- No reference/mockup import as runtime asset.
- No protocol, GameData schema, ADR, design-token, auth, DB, economy, social, or live ops change.

## Closure

This task closes when:

- V3B runtime candidate manifest reflects optimized Unity runtime copies;
- runtime Resources file sizes are budgeted by role;
- closure gates include `validate_lgo_runtime_asset_weight.py`;
- task ledger records `LGO_RUNTIME_ASSET_WEIGHT_HYGIENE_READY`.
