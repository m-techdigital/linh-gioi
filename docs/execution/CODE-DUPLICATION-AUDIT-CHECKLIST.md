# Linh Gioi Online Code Duplication Audit Checklist v1.0

Decision marker: LGO_CODE_GOVERNANCE_CONTRACT_ACCEPTED_v1.0.

Use this checklist before final handoff.

- Duplicate labels/copy.
- Duplicate state machines.
- Duplicate interaction logic.
- Duplicate config paths.
- Duplicate smoke marker strings.
- Duplicate package exclusions.
- Duplicate error handling.
- Duplicate validation logic.
- Inconsistent Vietnamese text.
- Stale milestone markers.
- Hardcoded paths repeated across scripts.
- UI logic copied between states.
- World logic copied between smoke/manual paths.
- Parallel DTO/config formats for the same concept.
- Server/API/realtime logic mirrored in client code instead of consumed through the contract.

Result format:

```text
CODE_DUPLICATION_AUDIT
result = PASS | FOLLOW_UP_REQUIRED | FIX_REQUIRED
notes =
follow_up =
```
