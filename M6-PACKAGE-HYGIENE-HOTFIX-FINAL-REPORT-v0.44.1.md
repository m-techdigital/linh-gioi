# M6 Package Hygiene Hotfix Final Report v0.44.1

Final decision: `M6_PACKAGE_HYGIENE_HOTFIX_SOURCE_CLOSED_v0.44.1`

Root cause:
The existing package hygiene validator only inspected older package versions and did not explicitly reject tracked Python cache artifacts across all handoff zips. The source tree also contained tracked `__pycache__/*.pyc` files, so a full-source package could include stale cache artifacts if the packaging rules drifted.

Fix:
- Removed tracked Python cache artifacts from source control.
- Removed tracked legacy v0.35 zip artifacts from source control so source checkouts no longer carry stale packaged binaries.
- Added `.pyc` to package exclusion rules.
- Changed full-source packaging to use the git index instead of all files found by filesystem walk, preventing unrelated untracked files from entering a package.
- Updated package hygiene validation to inspect every root-level zip package.
- Added a v0.44.1 validator that checks source status, package contents, deletion manifest format, frozen surfaces, and required v0.44.1 package artifacts.
- Added the v0.44.1 validator to the playable closure source/package gate and cleaned Python compile cache after source validation.
- Updated M6 validators in the closure path so files staged for deletion are not misreported as present cache artifacts.

Frozen surface audit:
- `protocol/**`: unchanged.
- `gamedata/schemas/**`: unchanged.
- `docs/adr/**`: unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json`: unchanged.

Runtime statement:
No new runtime PASS is claimed by this package hygiene hotfix. Runtime gates remain governed by the existing playable closure scripts.
