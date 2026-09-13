# LGO Map01A Item Icon Source Audit v0.1

Status: `SOURCE_AUDIT_CURRENT`, 2026-09-13. Scope: Map01A UI item/icon presentation only. This audit does not reopen class/wardrobe/source-pose work.

## Current conclusion

A deterministic five-item atlas now exists at `CongDongLamMap01AItems/map01a-item-icons.png` for HP potion, MP potion, starter equipment fragment, dumpling and coin. Each crop retains source path, source rectangle and SHA-256 provenance; the pack remains `DRAFT_RUNTIME_REVIEW` / `runtimeApproved=false`, so it is valid review art but not owner-approved final art.

No approved dedicated UI icon set exists for the ten equipment slots. Their current source-layer thumbnails remain dark and temporary. Do not reopen class/wardrobe work or fill those slots with emoji, generic geometry or random generated icons.

## Sources checked

- Unity runtime resources under `client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps` and `LGOClasses`.
- External selected 2D source under `/Users/minhdc/Projects/Design/LGO-Selected-2D-Source-v1` using filename patterns for `icon`, `item`, `potion`, `chest`, `gem`, `crystal`, `scroll`, and related terms.
- Owner UI reference images in `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/`, used as visual direction only, not runtime assets.
- Extracted candidates and provenance manifest in `/Users/minhdc/Projects/Design/LGO-Extracted-2D-Items-v1/map-01a/`; item candidates are review sources, not runtime-ready assets.

## Usable but not sufficient assets

- `CongDongLamMap01AArt/combat-layout.json` includes real map props such as `small-herb-spirit`, `common-chest`, and `young-spirit-herb` in the combat atlas.
- These are Map01A world props, not inventory icons for HP/MP potions. They can inform future quest collectible or chest UI, but should not be renamed visually into potion icons.
- Current equipment thumbnails come from complete detached-slot sprites in the runtime Võ atlas. They preserve real item ownership better than selecting a single limb component, but remain temporary evidence because the source resolution and contrast are too weak for final inventory icons.

## Required before wiring final item icons

1. Create or receive a dedicated 2D item icon board that covers the current Map01A items at minimum: HP potion, MP potion, starter wrist guard/reward, chest/reward, herb/quest collectible.
2. Record provenance for each icon source: source path, intended item id, resolution, transparent background status, and whether it is owner-approved or draft.
3. Import only the approved icon atlas or cutouts into Unity; do not put source boards or large design references into runtime resources.
4. Wire icons through the existing Map01A inventory UI and keep `Button.text` empty for image-bearing rows so UI Toolkit does not draw duplicate text over art.

## Guardrail

Until a provenance-backed icon set exists, the correct runtime state is text/card presentation plus clear `TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED` evidence. A prettier but fake icon is a regression for this project.

## Deterministic candidate atlas — 2026-09-13

`tools/pack_lgo_map01a_item_icons.py` now verifies every source-crop hash, removes only pale matte connected to the crop border, and packs five native-resolution icons into `CongDongLamMap01AItems/map01a-item-icons.png`. The manifest retains the source board, SHA-256 and source rectangle for HP, MP, starter equipment fragment, dumpling and coin. The dark/light alpha board at `build/map01a-item-icons-review-v1/alpha-review.png` has been visually reviewed: silhouettes and interior highlights remain intact, with no generated/redrawn pixels. Runtime status remains `DRAFT_RUNTIME_REVIEW`; owner acceptance is still pending.
