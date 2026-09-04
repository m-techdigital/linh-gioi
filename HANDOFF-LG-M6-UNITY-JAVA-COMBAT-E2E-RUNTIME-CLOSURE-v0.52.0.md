# Handoff: LGO M6 Unity-Java Combat E2E Runtime Closure v0.52.0

Decision: `M6_UNITY_JAVA_COMBAT_E2E_CLOSED_LOCAL_v0.52.0`

## What Changed

Unity now has a dedicated v0.52 E2E runner for server-path combat intent. The Java smoke server emits multiple existing protobuf messages for non-local-preview intents while keeping the older single-response local preview behavior intact.

## Validation

The runtime evidence marker is `M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0`.

## Frozen Surface Audit

Frozen contract surfaces were not modified.

## Next Allowed Task

`M6-COMBAT-UX-READABILITY-POLISH-v0.53.0`
