# LGO Build Size Budget

Marker: `LGO_BUILD_SIZE_BUDGET_READY`

## Boundary

The project separates runtime payload from repository-only reference/tooling weight.

- `client/Unity/Assets/**` is the main Unity runtime source budget.
- `client/Unity/Assets/Game/Art/Runtime/**` is the runtime art budget.
- `docs/reference-art/**` is reference art and review material, not runtime payload.
- `.git`, `server/**/target/**`, and `tools/protobuf/**` are repo/tooling weight, not player-facing art payload.

## Current Budget Targets

- Unity runtime source: keep `client/Unity/Assets/**` under 16 MB until a real content milestone requires more.
- Runtime art source: keep `client/Unity/Assets/Game/Art/Runtime/**` under 8 MB.
- V3B runtime candidates: keep `client/Unity/Assets/Game/Art/Runtime/V3B/**` under 4 MB while this is still a vertical slice.
- mobile: rely on platform import settings, smaller max texture sizes, and compressed build targets instead of shipping full reference boards.

## Rules

- Do not import reference boards, screen mockups, or composite sheets as runtime assets.
- Do not crop composite sheets into final runtime source.
- Prefer role-sized assets: fullscreen backgrounds can be compressed JPEG when opaque, sprites/icons/panels use transparent PNG only when alpha is needed.
- Keep source validators focused on runtime payload growth; reference art can be large but must stay outside Unity runtime folders.
- No dependency-bearing deletion without validation.

## Evidence

Run:

```bash
python3.12 tools/report_lgo_build_size_budget.py
python3.12 tools/validate_lgo_build_size_budget.py
```
