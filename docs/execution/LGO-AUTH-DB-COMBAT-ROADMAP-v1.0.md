# Linh Gioi Online Auth DB Combat Roadmap v1.0

Decision marker: LGO_MASTER_ROADMAP_ACCEPTED_v1.0.

Code governance applies through `docs/execution/CODE-GOVERNANCE-CONTRACT.md`.

## Combat M6

- Contract review first.
- Protocol needs: combat intent, authoritative result, rejection, and versioning if existing messages are insufficient.
- GameData needs: skill, effect, target, and balance data shape if schemas open.
- Server-authoritative direction: client sends intent, server validates, client presents result.
- Marker: server-authoritative.
- Local prototype: one narrow path only.
- Runtime smoke: accepted intent, rejected intent, inherited playable no-regression.
- Rollback/failure rules: no tag or closure if Unity build or runtime marker fails.

## Auth

- Dev login migration.
- Login/register UX.
- Session, token, logout, expiry.
- Security boundary and secret handling.
- Rate limiting/account recovery later.

## Database

- DB-0 ERD/domain design.
- Account/player/character tables.
- Progression/training records.
- Inventory/equipment later.
- Migrations.
- Backup/restore.
- Concurrency/audit.

No combat/auth/DB implementation is performed by this roadmap.
