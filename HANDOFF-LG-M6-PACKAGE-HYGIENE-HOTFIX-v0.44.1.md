# Handoff: LG M6 Package Hygiene Hotfix v0.44.1

Decision: `M6_PACKAGE_HYGIENE_HOTFIX_SOURCE_CLOSED_v0.44.1`

Changed area:
- Package creation hygiene.
- Package hygiene validators.
- Tracked Python cache artifact cleanup.
- v0.44.1 source package handoff artifacts.

Validation gates:
- `python3.12 -m py_compile tools/validate_m6_package_hygiene_hotfix.py`
- `git --no-pager diff --check`
- `python3.12 tools/validate_m6_package_hygiene_hotfix.py`
- `python3.12 tools/validate_package_hygiene.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `./tools/lgo_playable_closure_check.sh --package-ready`

Package artifacts:
- `linh-gioi-m6-server-authoritative-combat-foundation-v0.44.1-full-source.zip`
- `linh-gioi-m6-server-authoritative-combat-foundation-v0.44.1-delta.zip`
- `LGO-M6-PACKAGE-HYGIENE-HOTFIX-v0.44.1-ARTIFACTS-SHA256.txt`

Notes:
- This hotfix does not add gameplay or combat behavior.
- This hotfix does not claim production art.
- This hotfix does not close full M0 runtime.
