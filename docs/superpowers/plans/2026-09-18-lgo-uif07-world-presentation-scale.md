# LGO UIF-07 World Presentation Scale Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans task-by-task with TDD.

**Goal:** Make Map01A camera, actor/NPC screen scale, world-label density and background framing measurable and intentional independently from UI scale.

**Architecture:** Shipping `CongDongLamMap01AArtPreview` is the product authority because `GameBootstrap` attaches it directly. Introduce one pure `RuntimeWorldPresentationProfile` plus `RuntimeWorldPresentationMetrics`; bind Map01A camera/markers to that profile and write measured world metrics into normal quest manifests. Only after product behavior is green may prototype `PlayableWorldController` consume the shared profile where semantics match.

**Tech Stack:** Unity 6000.3.2f1, C#, orthographic Camera, SpriteRenderer/TextMesh, NUnit EditMode tests, existing macOS Player capture harness.

**Spec:** `build/full-ui-ux-code-audit-v1/backlog.json` task `LGO-UIF-07` and `build/full-ui-ux-code-audit-v1/audit.md`.

## Global Constraints
- No character rig/art redesign.
- Do not change UIF-03..06 UI/auth/persistence/account/world-entry/combat semantics.
- World presentation scale is independent from UI scale.
- Existing actor art is ~1.68–1.70 world units high; fixed Map01A half-height 3.8 currently implies roughly 22% vertical screen occupancy.
- Do not change camera/actor size just to make profiles different; evidence must justify geometry changes.
- Tablet/mobile Player evidence remains macOS profile/aspect simulation.
- Restore only known incidental Unity import/Graphics/Quality drift after Unity runs.
- Every production change follows verified RED -> minimal GREEN -> focused regressions.

---## File Structure
- Create: `client/Unity/Assets/Game/World/Runtime/RuntimeWorldPresentationProfile.cs` — pure profile classification, camera target, target ratio bands and label policy.
- Create: `client/Unity/Assets/Game/Foundation/Runtime/RuntimeWorldPresentationMetrics.cs` — serializable measurement snapshot for Player manifests.
- Modify: `CongDongLamMap01AArtPreview.cs` — consume profile, measure actor/NPC/background/marker data, export metrics.
- Modify: `CongDongLamRegisteredCapture.cs` — reuse shared screen-height measurement instead of owning a second implementation.
- Modify: `PlayableWorldController.cs` only after product contract is green; remove duplicate profile math only where behavior remains explicitly equivalent.
- Test: `DongMonIllustratedPreviewTests.cs` — product camera/aspect/grounding and manifest contract.
- Test: relevant foundation/world tests for pure profile/measurement behavior.

### Task 1: Shared world-presentation profile + measurement math
**Produces:**
- `RuntimeWorldPresentationProfile.FromScreen(string forcedProfile, int width, int height)`
- properties `Name`, `CameraOrthographicSize`, `CameraGroundOffsetY`, actor/NPC target ratio bounds, marker label metrics.
- `RuntimeWorldPresentationMetrics.ScreenHeightRatio(Camera, Bounds, int)`

- [ ] Add RED tests for desktop/tablet/mobile profile classification, explicit camera target, stable actor/NPC ratio bands and invalid measurement inputs.
- [ ] Run focused tests and verify they fail because the new types do not exist.
- [ ] Implement only the pure profile/measurement types. Keep Map01A camera at its currently proven 3.8 / +1.5 unless later capture proves a defect.
- [ ] Run focused GREEN and existing viewport/background ratio helpers.
- [ ] Commit: `refactor(world): centralize presentation scale authority`.### Task 2: Bind shipping Map01A camera and actor/NPC metrics
**Produces:** normal quest `manifest.json` gains `worldMetrics` containing profile name, camera half-height/Y offset, actor world height/screen ratio, representative NPC world height/screen ratio and marker policy.

- [ ] Add RED test proving Map01A uses the shared profile rather than literals `orthographicSize = 3.8f` / `GroundY + 1.5f`.
- [ ] Add RED capture-contract test requiring serializable world metrics with actor/NPC ratio values.
- [ ] Verify RED.
- [ ] Bind camera through `RuntimeWorldPresentationProfile`, calculate active actor bounds from enabled SpriteRenderers, choose a representative authored NPC renderer, and populate `RuntimeWorldPresentationMetrics`.
- [ ] Keep actor/root scale unchanged unless measured ratios leave the approved band.
- [ ] GREEN: product camera test, capture contract test, existing foot/quest regressions.
- [ ] Commit: `refactor(world): bind Map01A scale metrics`.

### Task 3: World labels + grounded composition
- [ ] Add RED tests that interaction markers use profile-owned label metrics and marker height stays bounded relative to actor/NPC scale.
- [ ] Add RED tests for background coverage and parallax/grounding invariants across desktop/tablet/mobile aspects.
- [ ] Verify RED.
- [ ] Move interaction-marker font/character size to profile; keep authored NPC/landmark world dimensions unchanged unless evidence proves out-of-band.
- [ ] Export background coverage factor and parallax delta/ground error into world metrics or existing manifest fields.
- [ ] GREEN focused tests.
- [ ] Commit: `refactor(world): normalize labels and framing metrics`.### Task 4: Reconcile prototype world profile without changing smoke semantics
- [ ] Add source/behavior tests proving `PlayableWorldController` no longer owns a duplicate nested screen classifier if it can consume the shared profile safely.
- [ ] Verify RED.
- [ ] Replace only duplicate mobile/tablet classification and shared label-policy calls; preserve its historical camera values if smoke evidence depends on them, or encode those values as a named legacy/prototype profile instead of silently changing them.
- [ ] Run M4/M5/M6 focused world/smoke EditMode regressions.
- [ ] Commit only if behavior is proven green; otherwise leave prototype authority documented and do not force convergence.

### Task 5: Exact-source Player evidence + world-scale audit
- [ ] Recheck foreign heavy WEB/AXIRO jobs and machine pressure; claim `lgo-unity-editor` only when safe.
- [ ] Run focused/static gates, then full Unity EditMode exact source.
- [ ] Build one fresh macOS Player from exact source.
- [ ] Capture PC 1600x900, tablet 1024x768, mobile-landscape 1600x720 from that binary.
- [ ] Extract `worldMetrics` per profile into a machine-readable audit table.
- [ ] Reject if actor/NPC ratios leave target bands, labels overlap/overscale, background coverage fails, ground error accumulates, or camera source is not profile-owned.
- [ ] Visually review representative arrival/dialogue/combat frames side-by-side.
- [ ] Restore incidental Unity rewrites, rerun final exact-source static gates.
- [ ] Commit/push runtime; verify local == origin/feature/2d and clean.
- [ ] Publish closure docs with exact runtime commit/evidence and simulation limitation; push docs.
- [ ] Link evidence to MCP task `T-c3fea993173e`, `v3.finish READY_REVIEW`, then continue to already-assigned UIF-08.

## Self-review
- Acceptance coverage: measured actor/NPC ranges, intentional camera framing, low-occlusion labels, grounded composition and camera/aspect manifests all have explicit tasks.
- UIF-08 texture optimization and UIF-09 decomposition are excluded.
- No rig/art redesign, no fabricated class renderer, no UI scale changes.
- A profile may intentionally retain identical Map01A 3.8 half-height across aspect classes if measured product evidence stays inside target bands; differentiation is not a goal by itself.
