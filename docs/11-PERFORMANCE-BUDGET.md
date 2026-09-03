# 11 — Performance Budget v0.1

These are initial design budgets to measure and refine, not marketing guarantees.

## Client

- low/mid mobile target: 30 FPS;
- capable devices: 60 FPS option;
- normal city visible-player target: 30;
- signature event target: 40–50 visible after LOD/AOI optimization;
- no unbounded particle spawning;
- no unbounded instantiated UI/list growth;
- content loading should migrate toward Addressables as production content grows.

## Server

- realtime simulation initial target: 20 Hz;
- no DB call per entity per tick;
- AOI limits replication to relevant nearby entities;
- load harness must simulate clients before real user scale tests.

## Budget invalidation

A feature that significantly changes entity count, animation/VFX cost, message frequency or memory must re-check its affected performance slice.
