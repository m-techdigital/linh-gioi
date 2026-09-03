# 08 — Coding Standards v1

## General

- Prefer explicit domain names over abbreviations.
- No `New`, `Final2`, `Temp`, `Manager2` style production naming.
- Keep public contracts small and documented.
- Avoid hidden cross-module singleton dependencies.
- New architectural dependency requires review when it crosses module boundaries.

## C# / Unity

- Gameplay code is organized by module and assembly definition.
- `MonoBehaviour` should primarily bind Unity lifecycle/presentation; durable domain logic should be testable outside scene glue where practical.
- Do not use scene lookup as a service locator (`FindObjectOfType` etc.) in core production paths without explicit justification.
- Do not directly edit generated protobuf source.

## Java

- Package by domain/capability, not one giant controller/service/repository layer.
- Durable mutations should define transaction/idempotency boundaries.
- Realtime loop avoids blocking DB/network calls on simulation thread.
- Do not directly edit generated protobuf source.

## Tests

Tests must state the invariant they protect. A green test with no meaningful assertion does not count as evidence.
