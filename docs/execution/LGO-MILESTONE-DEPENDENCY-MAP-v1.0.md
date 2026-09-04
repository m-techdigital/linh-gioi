# Linh Gioi Online Milestone Dependency Map v1.0

Decision marker: LGO_MASTER_ROADMAP_ACCEPTED_v1.0.

Code governance: dependencies must obey `docs/execution/CODE-GOVERNANCE-CONTRACT.md`.

## Before Combat

- Accept M6 combat contract review.
- Approve protocol ownership if new wire messages are needed.
- Approve GameData ownership if combat data shape changes.
- Define server-authoritative result, rejection, rollback, and runtime smoke markers.

## Before Auth

- Define security boundary, login/register UX, token/session lifecycle, logout, expiry, and dev-login migration.
- Decide storage boundary and secret handling before production auth code.

## Before DB

- Complete DB-0 ERD/domain design for account/player/character and progression/training.
- Define migrations, backup/restore, concurrency, audit, and rollback.

## Before Economy

- Finish item catalog, inventory shell, equipment rules, rewards, and consistency tests.
- Production economy waits for Auth/DB and admin-prod controls.

## Before Social/Guild/Chat

- Finish account identity, presence model, moderation/safety boundary, and persistence.
- Chat/party/guild require abuse handling and runtime smoke coverage.

## Before Web/Admin/Player Portal

- Public website may start with no auth.
- Player portal waits for Auth/DB.
- Admin-dev waits for roadmap/content tooling.
- Admin-prod waits for Auth/DB/content/economy and security review.

## Before Production Art

- Accept reference/provenance/license rules.
- Define sprite sheets, animation import settings, UI atlas settings, and visual review evidence.

## Before Release

- Source gates, runtime smoke matrix, visual evidence, telemetry event schema, crash/error reporting, alpha/beta/live checklists, monitoring, and rollback must be accepted.
