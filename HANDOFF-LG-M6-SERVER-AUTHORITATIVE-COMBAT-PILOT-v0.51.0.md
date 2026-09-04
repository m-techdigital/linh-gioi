# Handoff: LGO M6 Server-Authoritative Combat Pilot v0.51.0

Decision: `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_CLOSED_LOCAL_v0.51.0`

## What Changed

The Java realtime combat validation service now supports a narrow server-authoritative pilot using existing protobuf messages. The old `validate(...)` path remains compatible with earlier smoke tests; the new `validatePilot(...)` path returns accepted/result/snapshot evidence for accepted intents and rejected evidence for invalid intents.

## Validation

Required commands were run locally and passed unless separately noted in the final assistant response. The v0.51 marker is emitted by `tools/run_m6_server_authoritative_combat_pilot.sh`.

## Frozen Surface Audit

- Protocol files were not modified.
- GameData schemas were not modified.
- ADR files were not modified.
- UI design tokens were not modified.

## Contract Change

No contract change required.

## Next Allowed Task

`M6-UNITY-JAVA-COMBAT-E2E-RUNTIME-CLOSURE-v0.52.0`
