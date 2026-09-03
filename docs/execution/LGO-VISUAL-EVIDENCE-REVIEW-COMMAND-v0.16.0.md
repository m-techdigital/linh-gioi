# LGO Visual Evidence Review Command v0.16.0

Use from repo root `LinhGioiOnline`.

Run through the playable closure wrapper:

```bash
./tools/lgo_playable_closure_check.sh --visual-evidence
```

Run the visual evidence command directly:

```bash
./tools/run_m5_visual_evidence_review.sh --rebuild
```

Open existing built player:

```bash
./tools/run_m5_visual_evidence_review.sh --open-existing
```

Review outputs:

```text
build/visual-evidence/
```

Expected marker:

```text
LGO_PLAYABLE_VISUAL_EVIDENCE_READY
```

If screenshot capture is unavailable:

```text
LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE
```

This command produces review artifacts. It does not claim explicit human visual acceptance by itself.
