# Linh Gioi Online Code Quality Gates v1.0

Decision marker: LGO_CODE_GOVERNANCE_CONTRACT_ACCEPTED_v1.0.

Future tasks must classify these gates before handoff:

- Source validators: run relevant static/source validators and new validators added by the task.
- Duplicate/ownership audit: apply `docs/execution/CODE-DUPLICATION-AUDIT-CHECKLIST.md` and `docs/execution/CODE-OWNERSHIP-MAP.md`.
- Package hygiene: exclude build/generated/cache/temp/local outputs and verify packages when produced.
- Frozen surface audit: confirm `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, and `client/Unity/Assets/Game/UI/design-tokens.json` are unchanged unless explicitly opened.
- Runtime evidence classification: classify PASS, UNVERIFIED_ENVIRONMENT, DEFERRED, and NOT CLAIMED separately.
- Visual evidence classification: if UI/art changed, classify generated screenshots, manual review, accepted reference, and production-art non-claims.
- Handoff quality section: include code quality, duplication, ownership, validation, package, and non-claim evidence.
- Technical debt/follow-up section: record remaining cleanup when it is not fixed in scope.

Validators must not be weakened to pass. Source/static failures are fix-required, not environment limitations.
