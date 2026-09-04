# LINH GIỚI ONLINE — M6 Visual Reference Pack v0.36.0 — CODEX USAGE GUIDE

This ZIP is not just loose images. It is an input artifact for Codex tasks after `M6_COMBAT_FOUNDATION_RUNTIME_CLOSED_LOCAL_v0.35.0`.

Use this file as the first instruction when giving the pack to Codex.

## 0. What this pack is for

Primary use: support `v0.37 — M6 Combat Visual Image Ingest + Readability Polish`.

It must be used as visual reference only:

- Do not claim production art.
- Do not implement production combat.
- Do not implement server-authoritative combat.
- Do not implement auth, DB, inventory, loot, economy, guild, chat, market, party, live ops, or production admin/player portal.
- Do not change `protocol/**`.
- Do not change `gamedata/schemas/**`.
- Do not change `docs/adr/**`.
- Do not modify `client/Unity/Assets/Game/UI/design-tokens.json`.
- Do not copy English labels from images into runtime UI.
- Player-facing runtime UI must be Vietnamese.

## 1. Install command for project owner

Run from local machine after downloading this ZIP:

```bash
cd "$HOME/Projects/LinhGioiOnline"
cp "$HOME/Downloads/lgo-m6-visual-reference-pack-v0.36.0-codex-ready.zip" .
shasum -a 256 lgo-m6-visual-reference-pack-v0.36.0-codex-ready.zip
unzip -o lgo-m6-visual-reference-pack-v0.36.0-codex-ready.zip -d .
find docs/reference-art/v0.36.0 -maxdepth 1 -type f | sort
find docs/reference-art/future-reference-v0.36.0 -maxdepth 1 -type f | sort
```

Expected main folder after unzip:

```text
docs/reference-art/v0.36.0/
```

Expected future-only folder after unzip:

```text
docs/reference-art/future-reference-v0.36.0/
```

## 2. Which images Codex may use now

For v0.37, Codex may use ONLY this folder:

```text
docs/reference-art/v0.36.0/
```

Files:

```text
lgo-m6-combat-readability-board-v0360.png
lgo-m6-target-dummy-state-sheet-v0360.png
lgo-m6-skill-feedback-sheet-v0360.png
lgo-m6-combat-hud-mockup-v0360.png
lgo-m6-enemy-telegraph-sheet-v0360.png
lgo-m6-hit-cooldown-feedback-sheet-v0360.png
lgo-m6-combat-reference-composite-v0360.png
```

Role of each image:

| File | Role | Use in v0.37 |
|---|---|---|
| `lgo-m6-combat-readability-board-v0360.png` | Overall readability rules | target contrast, impact clarity, UI priority |
| `lgo-m6-target-dummy-state-sheet-v0360.png` | Dummy states | normal/selected/hit/recover/disabled visual language |
| `lgo-m6-skill-feedback-sheet-v0360.png` | Skill feedback | local-only skill flash/impact/cooldown styling |
| `lgo-m6-combat-hud-mockup-v0360.png` | Combat HUD reference | target label, skill button, cooldown, feedback text |
| `lgo-m6-enemy-telegraph-sheet-v0360.png` | Future telegraph reference | document only; do not implement enemy attack yet |
| `lgo-m6-hit-cooldown-feedback-sheet-v0360.png` | Hit/cooldown states | “Trúng mục tiêu”, cooldown, ready/recover display |
| `lgo-m6-combat-reference-composite-v0360.png` | Combined overview | reviewer reference; do not use as scope expansion |

## 3. Which images are future-only

Do not use this folder to expand the v0.37 task:

```text
docs/reference-art/future-reference-v0.36.0/
```

These images are saved for later roadmap phases:

| File | Future phase |
|---|---|
| `lgo-extra-title-login-server-flow-v0360.png` | future Auth/Login UI shell |
| `lgo-extra-character-inventory-ui-v0360.png` | future inventory/equipment phase |
| `lgo-extra-world-light-combat-hud-v0360.png` | future broader HUD polish |
| `lgo-extra-npc-quest-shop-ui-v0360.png` | future NPC/quest/shop/content phase |
| `lgo-extra-environment-concepts-v0360.png` | future world/content/environment phase |
| `lgo-extra-character-monster-animation-reference-v0360.png` | future character/monster animation phase |
| `lgo-extra-item-skill-vfx-reference-v0360.png` | future item/skill/VFX production-readiness phase |
| `lgo-extra-player-portal-admin-dashboard-v0360.png` | future web/player portal/admin roadmap |
| `lgo-extra-art-direction-overview-v0360.png` | future art direction reference |

## 4. Copy-paste prompt for Codex after installing this ZIP

```text
# LINH GIỚI ONLINE — M6 v0.37 — USE VISUAL REFERENCE PACK AND POLISH COMBAT READABILITY

You are working on Linh Giới Online.

The project owner has installed the visual reference pack:

docs/reference-art/v0.36.0/
docs/reference-art/future-reference-v0.36.0/

This is not loose art. Treat it as an input artifact for M6 combat visual readability polish.

CURRENT STATE
- M6 combat foundation local runtime closed at v0.35.0.
- Existing local-only target dummy combat exists.
- Visual evidence was previously captured but human visual acceptance was pending.
- Current combat is local/prototype only.
- Production/server-authoritative combat is not implemented.

READ FIRST
- docs/execution/CODE-GOVERNANCE-CONTRACT.md
- docs/execution/CODE-OWNERSHIP-MAP.md
- docs/execution/CODE-QUALITY-GATES.md
- docs/execution/LGO-MASTER-ROADMAP-v1.0.md
- docs/art/LGO-COMBAT-VISUAL-REFERENCE-PACK-v0.36.0.md if present
- docs/design/LGO-COMBAT-READABILITY-RULES-v0.36.0.md if present
- docs/reference-art/v0.36.0/README.md
- docs/reference-art/v0.36.0/CODEX-USAGE.md if present
- docs/reference-art/future-reference-v0.36.0/README.md
- docs/tasks/M6-COMBAT-UX-FEEDBACK-POLISH-v0.35.0.md
- HANDOFF-LG-M6-COMBAT-UX-FEEDBACK-POLISH-v0.35.0.md
- M6-COMBAT-UX-FEEDBACK-POLISH-FINAL-REPORT-v0.35.0.md

USE THESE IMAGES NOW
Only use files under:

docs/reference-art/v0.36.0/

DO NOT USE FOR v0.37 SCOPE EXPANSION
Do not use files under:

docs/reference-art/future-reference-v0.36.0/

Those are only saved for future Auth/Login, inventory, web/admin, environment, NPC/shop, and asset pipeline phases.

HARD RULES
- Do not implement new combat mechanics.
- Do not implement server-authoritative combat.
- Do not implement production art.
- Do not implement auth, DB, inventory, loot, economy, guild, chat, market, party, live ops, or production admin/player portal.
- Do not touch protocol/**.
- Do not touch gamedata/schemas/**.
- Do not touch docs/adr/**.
- Do not modify client/Unity/Assets/Game/UI/design-tokens.json.
- Do not copy English labels from images into runtime UI.
- Player-facing copy must be Vietnamese.
- Do not use image details to justify scope expansion.
- No "|| true".
- Do not use git diff --cached --check.
- Use git --no-pager.

ALLOWED PATHS
- docs/reference-art/v0.36.0/**
- docs/reference-art/future-reference-v0.36.0/** only for README/manifest/reference classification, not feature scope
- docs/art/**
- docs/design/**
- docs/tasks/**
- client/Unity/Assets/Game/World/Runtime/**
- client/Unity/Assets/Game/UI/Runtime/**
- client/Unity/Assets/Game/Foundation/**
- client/Unity/Assets/Game/Tests/**
- tools/**
- HANDOFF*
- M6*
- LGO*

TASK GOAL
Implement v0.37 as a narrow visual readability polish of the existing local-only target dummy combat loop, using the v0.36 images only as reference.

ALLOWED IMPLEMENTATION
- clearer target dummy highlight
- clearer selected target state
- clearer hit flash/impact feedback
- clearer cooldown/ready state display
- clearer local-only prototype label
- clearer Vietnamese combat tooltip/help text
- visual polish using existing placeholder/material/UI patterns
- update validator/smoke only if it verifies existing local-only behavior

FORBIDDEN IMPLEMENTATION
- no real damage balancing
- no production HP/death progression
- no projectile system
- no multiple enemy combat
- no loot/reward
- no XP/level
- no inventory mutation
- no protocol/GameData change
- no server combat path
- no production art claim

VIETNAMESE COPY ANCHORS
Use natural Vietnamese, such as:
- Tấn công thử
- Mục tiêu luyện tập
- Trúng mục tiêu
- Hồi chiêu
- Sẵn sàng
- Đang hồi chiêu
- Mô phỏng cục bộ
- Chưa phải chiến đấu thật

CREATE/UPDATE
- docs/tasks/M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md
- tools/validate_m6_combat_visual_readability.py
- M6-COMBAT-VISUAL-READABILITY-POLISH-FINAL-REPORT-v0.37.0.md
- HANDOFF-LG-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md
- LGO-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0-CHANGED-FILES.txt
- LGO-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0-DELETIONS.txt

VALIDATOR REQUIREMENTS
tools/validate_m6_combat_visual_readability.py must check:
- required v0.36 image files exist
- future-reference files are not used to justify current scope expansion
- Vietnamese local-only combat copy exists
- local-only/prototype label exists
- non-claims exist
- no protocol/GameData/schema/ADR/design-token drift
- no server combat path added
- no inventory/loot/economy/auth/DB files added
- no build/generated/cache artifacts present
- code quality/duplication/ownership audit appears in handoff

VALIDATION COMMANDS
pwd
test "$(basename "$PWD")" = "LinhGioiOnline"
git --no-pager status --short --untracked-files=all
git --no-pager diff --check
python3.12 -m py_compile tools/validate_m6_combat_visual_readability.py
python3.12 tools/validate_m6_combat_visual_reference_pack.py
python3.12 tools/validate_m6_combat_visual_readability.py
python3.12 tools/validate_m6_minimal_local_combat.py
python3.12 tools/validate_m6_combat_ux_feedback.py
python3.12 tools/validate_code_governance.py
python3.12 tools/validate_master_roadmap.py
python3.12 tools/validate_project_state.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_playable_closure_check.sh --package-ready
./tools/lgo_playable_closure_check.sh --runtime
./tools/lgo_playable_closure_check.sh --visual-evidence

RUNTIME EVIDENCE MUST INCLUDE
- M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS
- LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS

VISUAL EVIDENCE
- If screenshot captured, mark screenshotStatus=CAPTURED.
- Keep humanVisualAcceptancePending=true unless a human review is explicitly documented.
- Do not claim production visual acceptance.

DELETION FILE
If no deletion, write exactly:
DELETED
none

COMMIT
Stage only accepted source/report files. Do not stage Unity Library/Temp/Logs/build/cache/generated artifacts.

git add docs/reference-art/v0.36.0
git add docs/reference-art/future-reference-v0.36.0/README.md
git add docs/art
git add docs/design
git add docs/tasks/M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md
git add client/Unity/Assets/Game/World/Runtime
git add client/Unity/Assets/Game/UI/Runtime
git add client/Unity/Assets/Game/Foundation
git add client/Unity/Assets/Game/Tests
git add tools/validate_m6_combat_visual_readability.py
git add tools/lgo_playable_closure_check.sh
git add -f M6-COMBAT-VISUAL-READABILITY-POLISH-FINAL-REPORT-v0.37.0.md
git add -f HANDOFF-LG-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md
git add -f LGO-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0-CHANGED-FILES.txt
git add -f LGO-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0-DELETIONS.txt
git --no-pager diff --cached --name-status
git commit -m "feat: polish M6 combat visual readability"
git push

git tag -a lgo-m6-combat-visual-readability-polish-v0.37.0 -m "LGO M6 combat visual readability polish v0.37.0"
git push origin lgo-m6-combat-visual-readability-polish-v0.37.0

FINAL DECISION
Use exactly one:
- M6_COMBAT_VISUAL_READABILITY_RUNTIME_CLOSED_LOCAL_v0.37.0
- M6_COMBAT_VISUAL_READABILITY_SOURCE_CLOSED_RUNTIME_UNVERIFIED_ENVIRONMENT_v0.37.0
- M6_COMBAT_VISUAL_READABILITY_FIX_REQUIRED_v0.37.0
- BLOCKED_CONTRACT

FINAL RESPONSE MUST INCLUDE
- final decision
- commit hash
- tag
- files changed
- validation commands run
- runtime markers observed
- visual evidence status
- frozen surface audit
- code duplication/ownership audit
- exact non-claims
- exact line:
Please send these artifacts/report/logs to ChatGPT for independent review and next execution selection.
```

## 5. After v0.37 passes

Then proceed in this order:

```text
v0.38 — M6 Combat Input / Feedback Stability
v0.39 — M6 Server-Authoritative Combat Contract Spec
```

Do not start v0.39 server-authoritative implementation directly. v0.39 is still contract/spec only.
