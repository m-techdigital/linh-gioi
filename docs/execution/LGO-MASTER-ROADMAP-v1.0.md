# Linh Gioi Online Master Roadmap v1.0

Decision marker: LGO_MASTER_ROADMAP_ACCEPTED_v1.0.

Code governance: this roadmap is governed by `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/CODE-OWNERSHIP-MAP.md`, and `docs/execution/CODE-QUALITY-GATES.md`.

## Current Completed State

- M0 runtime foundation: closed.
- M1 offline combat prototype: closed as a local deterministic slice.
- M2 online session scaffold: source/runtime candidate remains separate; do not overclaim full closure.
- M3 account/character API persistence: runtime smoke closed.
- M3-B Unity account/character integration: covered by dedicated runtime smoke.
- M4 playable client, visual placeholder foundation, UI redesign, art quality pass, stabilization, and visible UI review harness: implemented.
- M5 playable loop, visual reference pack, guided training loop, session feedback, world hub readability, art direction pack ingestion, pose placeholders, UI skinning, VFX feedback, NPC dialogue, session menu, local settings, and API error resilience: implemented across accepted stages.
- M6 pre-combat readiness: skill preview sandbox, target dummy readability, and combat readiness spec are closed without implementing real combat.

## Near-Term Sequence

1. Contract review for real combat.
2. Auth and DB design before production implementation.
3. Public website can begin as a separate presentation surface.
4. Player portal waits for Auth/DB.
5. Admin-dev console waits for content/roadmap ownership.
6. Admin-prod waits for Auth/DB/content/economy.
7. Production art waits for provenance, import settings, and review.

## Roadmap Domains

- Combat M6: contract review, protocol needs, GameData needs, server-authoritative direction, local prototype, runtime smoke, rollback/failure rules.
- Auth: dev login migration, login/register UX, session/token/logout/expiry, security boundary, rate limiting/account recovery later.
- Database: DB-0 ERD/domain design, account/player/character, progression/training, inventory/equipment later, migrations, backup/restore, concurrency/audit.
- Inventory/economy/progression: item catalog, inventory shell, equipment, rewards, economy later.
- Social/MMO: display profile, presence, friend list, chat, party, guild, leaderboard/event board.
- Content/live event: content taxonomy, map/zone model, NPC/dialogue content, skill/effect content, world events/boss events, admin/dev content validation.
- Web: public player-facing landing website, player portal after Auth/DB, internal admin/dev console, production admin after Auth/DB/content/economy, player website game-style, admin console ops-first with Linh Gioi skin.
- Asset/art/animation: reference art packs, runtime placeholders, production sprite sheets, animation direction, UI atlas/import settings, provenance/license rules.
- QA/CI/runtime/visual evidence: one-command closure, source gates, runtime smoke matrix, visual evidence, telemetry/event schema, crash/error reporting, alpha/beta/live checklist.

## Production Readiness

- Alpha criteria: stable account/character flow, accepted first combat foundation, DB design accepted, crash/error reporting path, and owner-visible playable build.
- Beta criteria: persistence migrations, auth session lifecycle, basic inventory/progression, content validation, smoke matrix, and rollback rehearsals.
- Live criteria: monitoring, backups, security review, moderation/admin controls, production art provenance, release checklist, and live rollback plan.

## Anti-Patchwork Rules

- No duplicated logic.
- No parallel DTO/config.
- No source inspection as runtime PASS.
- No production claims without gates.
- No large feature without ownership, validator, smoke, handoff, and technical debt audit.

Real combat, production auth, DB implementation, production admin/player portal, economy/social/live ops, and production art are not implemented by this roadmap.
