# ADR-0002 — Server-authoritative game state

**Status:** Accepted

Canonical movement validation, combat results, health, progression, inventory, currency and trade are server authoritative.

Client is responsible for input, prediction/presentation and rendering, but cannot declare durable outcomes.
