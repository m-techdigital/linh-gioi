# Linh Gioi UI Atlas Import Settings

Marker: `LGO_UI_ATLAS_PLAN_READY`

## Purpose

UI art should be imported by role, not by oversized source habit. Buttons, icons, panels, and HUD pieces need clear texture budgets before the project gains hundreds of assets.

## Atlas Groups

| Group | Asset roles | Preferred source size | Max texture size |
|---|---|---:|---:|
| `ui_core` | common panels, button skins, dividers | 256-1024 px | 1024 |
| `ui_icons` | account/menu/settings/quest/status icons | 64-256 px | 256 |
| `ui_hud` | HP/MP frames, avatar frames, objective panels | 128-1024 px | 1024 |
| `combat_ui` | cooldown rings, target markers, combat buttons | 128-512 px | 512 |
| `vfx_ui` | small impact/spark/ring overlays | 256-512 px | 512 |

## Import Settings

- `textureType: Sprite` for UI sprites.
- `spriteMode: Single` unless a reviewed atlas workflow opens.
- `alphaIsTransparency: true` when alpha is required.
- `Read/Write Enabled: false` by default.
- `Generate Mip Maps: false` for pixel-stable UI.
- `Max Size` must match the smallest role-safe budget.
- Compression should be visually checked on transparent glows and borders.

## Runtime Rules

- Use 9-slice panels/buttons instead of very large rectangular textures.
- Use one shared frame skin where layout can stretch cleanly.
- Avoid duplicate near-identical button states unless color/state contrast is meaningful.
- Do not import reference boards, contact sheets, or mockups into runtime UI paths.
- Do not bake Vietnamese UI copy into button sprites; text remains runtime text.

## Future Atlas Criteria

Before creating a production atlas:

- group by screen/update frequency;
- document packing source;
- preserve readable edges and glow;
- verify mobile and desktop screenshots;
- record atlas dimensions and total memory budget;
- keep provenance notes for every included asset.
