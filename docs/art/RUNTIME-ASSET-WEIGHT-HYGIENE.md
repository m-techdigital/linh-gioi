# Linh Gioi Runtime Asset Weight Hygiene

Marker: `LGO_RUNTIME_ASSET_WEIGHT_HYGIENE_READY`

## Purpose

The playable game should stay light while the source grows. Reference art can be large, but Unity runtime copies must be role-sized and optimized.

## Current Policy

- Reference/demo/mockup images remain visual direction only.
- Unity `Resources` folders should contain optimized runtime copies, not full-resolution review sources.
- Opaque fullscreen/login backgrounds may use JPEG when alpha is not required.
- Transparent UI, character, world, combat, and VFX assets remain PNG.
- Small UI/icons/VFX should normally be a few KB to a few dozen KB.
- Large backgrounds can be hundreds of KB, but should not casually become multi-MB runtime textures.
- Full-source ZIP handoff archives should live outside repo root, with only SHA/provenance retained in source when needed.

## V3B Runtime Copy Budget

| Role | Runtime copy target | Max file size |
|---|---:|---:|
| login background JPEG | 1920x1080 | 512 KB |
| login panel PNG | 384-512 px major axis | 200 KB |
| enter world button PNG | 384-512 px major axis | 120 KB |
| login NPC PNG | 384-512 px major axis | 220 KB |
| world spirit gate PNG | 384-512 px major axis | 320 KB |
| training stone PNG | 384-512 px major axis | 180 KB |
| target dummy PNG | 384-512 px major axis | 180 KB |
| VFX frame PNG | 192-256 px major axis | 90 KB |
| cooldown ring PNG | 128 px major axis | 45 KB |

## Build Size Note

The local macOS player can still appear large because Unity includes engine frameworks, managed assemblies, and build data. That is a platform/runtime baseline issue, not approval to ship oversized art. Release-size work must later add platform-specific stripping, compression, and packaging checks.

## Non-Claims

This hygiene pass does not claim production art quality, final visual acceptance, mobile store size readiness, or final release packaging.
