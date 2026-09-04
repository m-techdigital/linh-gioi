# LGO Mobile Tablet UI Profile Hardening v1.0

Marker: `LGO_MOBILE_TABLET_UI_PROFILE_HARDENING_READY`

## Scope

Define and enforce device-profile UI and runtime-asset budgets before adding more visual polish.

Allowed:

- document shared `mobile-light`, `tablet-standard`, and `pc-standard` runtime delivery assumptions;
- validate V3B runtime candidate dimensions against profile ceilings;
- wire the profile validator into continuous local source gates;
- keep source/runtime evidence honest when Unity visual capture is environment-blocked.

Not allowed:

- No duplicate ad hoc runtime asset folders.
- No production art claim.
- No reference/mockup import as runtime asset.
- No protocol, GameData schema, ADR, design-token, auth, DB, economy, social, guild, liveops, or full combat change.

## Profile Budget

| Profile | UI sprite ceiling | Key background ceiling | VFX ceiling | Cooldown icon ceiling |
|---|---:|---:|---:|---:|
| mobile-light | 512 px | 1024 px | 256 px | 128 px |
| tablet-standard | 1024 px | 1536 px | 256 px | 128 px |
| pc-standard | 1024 px | 2048 px | 256 px | 128 px |

The shared Unity runtime candidate copy must fit these ceilings. Future platform-specific bundles can reduce import settings further, but they should derive from the same provenance manifest instead of creating unmanaged copies.

## Closure

This task closes when:

- `docs/art/RUNTIME-ASSET-WEIGHT-HYGIENE.md` names the device profiles;
- `tools/validate_lgo_device_profile_ui_budgets.py` enforces profile ceilings from the V3B runtime manifest;
- `tools/lgo_continue_dev_loop.sh` and source closure gates run the validator;
- task ledger records `LGO_MOBILE_TABLET_UI_PROFILE_HARDENING_READY`.
