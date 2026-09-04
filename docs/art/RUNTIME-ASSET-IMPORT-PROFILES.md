# Runtime Asset Import Profiles

Status: `LGO_RUNTIME_ASSET_IMPORT_PROFILES_READY`

Date: 2026-09-05

## Purpose

Linh Giới Online should keep one clean runtime source asset per visual role, then let Unity import settings adapt texture size and compression by build target. This prevents ad hoc duplicate folders while keeping PC/tablet/mobile builds lighter.

## Policy

- V3B files are runtime candidates, not production final art.
- Do not crop composite/reference boards.
- Keep source provenance in the V3B manifest.
- Use Unity `TextureImporter.platformSettings` for device-specific delivery.
- Keep no duplicate ad hoc runtime asset folders for mobile/tablet/desktop.

## Build Targets

| Build target | Intent | Texture rule |
|---|---|---|
| `DefaultTexturePlatform` | Shared editor fallback | Match role budget from manifest. |
| `Standalone` | PC/macOS desktop | Match role budget from manifest for current 1080p evidence. |
| `Android` | Mobile-light default | Cap login background at 1024, normal sprites at 512, VFX at 256, cooldown rings at 128. |
| `iPhone` | iOS/mobile/tablet default | Cap login background at 1536, normal sprites at 768, VFX at 256, cooldown rings at 128. |

## Operating Notes

- Opaque login backgrounds may stay JPEG at source, with target compression decided by platform import.
- Transparent sprites remain PNG at source so alpha, glow, and edge quality are preserved.
- If a future build pipeline needs true split bundles, derive them from the manifest rather than manually copying random images into `Resources`.
- If platform screenshots show blur or aliasing, adjust the profile ceiling by role and rerun the import-profile validator.
