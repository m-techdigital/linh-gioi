# Runtime Asset Size Budget

Marker: `LGO_RUNTIME_ASSET_SIZE_BUDGET_READY_v1.0`

## Purpose

Linh Gioi Online will have many images. Runtime asset files must be sized for their actual role, then optimized so the project stays light while remaining sharp in Unity.

## Classification

- `REFERENCE_ONLY`: overview boards, moodboards, screen mockups, posters, and direction sheets. Never import into Unity runtime folders.
- `EXPERIMENTAL_SOURCE_ONLY`: sheet crops or experiments kept for review/evidence. Never claim as runtime-approved or production art.
- `STRUCTURAL_RUNTIME_PLACEHOLDER_V2`: separated placeholder assets usable for mapping, validators, and temporary UI layout proof.
- `RUNTIME_CANDIDATE_SIZE_BUDGETED`: generated/imported candidate assets that have role-sized dimensions, alpha checks, import settings, and size budget evidence.
- `PRODUCTION_FINAL_REVIEW_REQUIRED`: high-quality separated assets that still need human visual acceptance before production-final claim.

## Runtime Source Size Targets

| Role | Source size | Unity maxTextureSize | File budget |
|---|---:|---:|---:|
| Fullscreen login/background | 1920x1080 | 2048 | 150-500 KB preferred |
| Large world object/gate | up to 1024x1024 | 1024 | 50-250 KB |
| Tall NPC/character portrait | 512x768 or 768x1024 | 1024 | 40-180 KB |
| In-world small actor/prop | 256-512 px major axis | 512 | 10-100 KB |
| Large reusable panel | 512x256 to 1024x512 | 1024 | 20-120 KB |
| Button | 256x64 to 512x128 | 512 | 5-50 KB |
| HUD/status icon | 64x64 to 128x128 | 128 or 256 | 3-25 KB |
| Skill icon | 256x256 | 256 or 512 | 10-60 KB |
| VFX frame | 256x256 to 512x512 | 512 | 5-80 KB |
| Cooldown/selection ring | 256x256 to 512x512 | 512 | 5-60 KB |

## Generation Rules

- Generate or request each asset separately for its runtime role.
- Do not crop composite sheets into final runtime assets.
- Do not bake player-facing text into sprites except approved logo/brand assets.
- Prefer clean alpha for sprites, panels, icons, and VFX.
- Prefer non-alpha optimized formats for fullscreen backgrounds when Unity import/runtime allows it.
- Keep `Read/Write Enabled` off unless code explicitly needs CPU-side texture reads.
- Set `maxTextureSize` to the smallest role-safe value.
- Keep source dimensions and Unity import budgets recorded in a manifest.

## Optimization Rules

Use lossless or visually safe compression after generation:

- strip metadata;
- quantize PNG only when alpha edges and glow remain clean;
- keep separate source/reference copies out of Unity runtime paths when not needed;
- reject assets that are visually blurry at display size;
- reject assets that exceed budget without a documented reason.

## Required Evidence

Every runtime candidate pack must provide:

- manifest with role, dimensions, file size, alpha requirement, classification, source, and SHA-256;
- import mapping;
- rejected-assets notes;
- optimization report;
- validator coverage for dimensions, file size, frozen surfaces, and no reference/composite imports.
