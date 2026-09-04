# M6 Package Hygiene Hotfix v0.44.1

Status: source package hygiene hotfix.

Scope:
- Remove tracked Python cache artifacts from source control.
- Regenerate the v0.44 handoff source packages as v0.44.1 with cache/build/generated exclusions.
- Strengthen package validators so future handoff zips reject `__pycache__`, `*.pyc`, build output, generated Unity protocol output, and nested package artifacts.

Non-goals:
- No gameplay changes.
- No combat mechanic expansion.
- No protocol, gamedata schema, ADR, or UI design token changes.
- No production art claim.

Acceptance marker:
`M6_PACKAGE_HYGIENE_HOTFIX_PASS_v0.44.1`
