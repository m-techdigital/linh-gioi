# LGO Map01A Item Icon Source Audit v0.1

Status: `SOURCE_AUDIT_CURRENT`, 2026-09-13. Scope: Map01A UI item/icon presentation only. This audit does not reopen class/wardrobe/source-pose work.

## Current conclusion

No approved dedicated UI icon set for `Bình Máu Nhỏ`, `Bình Linh Lực Nhỏ`, or the Map01A starter reward was found in the Unity project or the selected 2D source folder during the current audit. The inventory UI should therefore keep readable text/count/state cards and must not fill the gap with emoji, generic geometry, random generated icons, or class wardrobe crops presented as item icons.

## Sources checked

- Unity runtime resources under `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps` and `LGOClasses`.
- External selected 2D source under `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1` using filename patterns for `icon`, `item`, `potion`, `chest`, `gem`, `crystal`, `scroll`, and related terms.
- Owner UI reference images in `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/`, used as visual direction only, not runtime assets.

## Usable but not sufficient assets

- `CongDongLamMap01AArt/combat-layout.json` includes real map props such as `small-herb-spirit`, `common-chest`, and `young-spirit-herb` in the combat atlas.
- These are Map01A world props, not inventory icons for HP/MP potions. They can inform future quest collectible or chest UI, but should not be renamed visually into potion icons.
- Current equipment thumbnails come from runtime class atlases. They are acceptable as temporary runtime evidence for equipped gear, but they are not final item icon art and should not be extended to unrelated consumables.

## Required before wiring final item icons

1. Create or receive a dedicated 2D item icon board that covers the current Map01A items at minimum: HP potion, MP potion, starter wrist guard/reward, chest/reward, herb/quest collectible.
2. Record provenance for each icon source: source path, intended item id, resolution, transparent background status, and whether it is owner-approved or draft.
3. Import only the approved icon atlas or cutouts into Unity; do not put source boards or large design references into runtime resources.
4. Wire icons through the existing Map01A inventory UI and keep `Button.text` empty for image-bearing rows so UI Toolkit does not draw duplicate text over art.

## Guardrail

Until a provenance-backed icon set exists, the correct runtime state is text/card presentation plus clear `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED` evidence. A prettier but fake icon is a regression for this project.
