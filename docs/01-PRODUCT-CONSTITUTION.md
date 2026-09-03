# 01 — Product Constitution

This document prevents parallel implementation lanes from redefining the product while coding.

## Immutable v1 principles

### P1 — Server authority
Client sends intent. Server owns canonical movement validation, health, damage, rewards, inventory, currency, progression and marketplace mutation.

### P2 — One persistent identity
The business model must not require periodic full character resets.

### P3 — Social crosses systems
Social is not a standalone menu. Party, friend, guild, trade, profile, housing visit and cooperative event participation should connect to core progression.

### P4 — Depth before breadth
One excellent city and one excellent world event are preferred over many shallow maps/events.

### P5 — Mobile-first usability
Every gameplay and UI feature must have a valid phone layout and touch workflow. Desktop may add shortcuts, not a separate design language.

### P6 — Fair power economy
Monetization may sell cosmetics and convenience bounded by explicit rules, but must not create exclusive combat power unavailable through gameplay.

### P7 — Data-driven content
Skills, items, monsters, loot, quests, recipes and events must not require code edits for ordinary tuning.

### P8 — Contract before implementation
A lane may not invent protocol/schema contracts because another lane has not implemented them yet.

### P9 — Playable milestone
Every milestone ends in a playable integrated build, not merely merged source.

### P10 — Measured performance
Performance budgets are treated as architecture constraints and are measured on representative devices/builds.

## Change control

A violation or change to P1–P10 requires an ADR under `docs/adr/`, reviewed by S0 before implementation.
