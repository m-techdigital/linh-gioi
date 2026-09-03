# P9 — S0 Project Governance Readiness

Use this prompt when a sandbox is asked to update planning, delivery rules, phase gates, and documentation quality without opening a feature milestone.

## Goal

Improve project execution clarity so future Linh Giới Online milestones can be implemented with clear progress, evidence, and handoff standards.

## Scope allowed

- `docs/execution/**`
- `docs/tasks/planned/**`
- `docs/reference-art/design-boards/**`
- root readme/status/version files when only linking or status-normalizing docs

## Scope forbidden

- Do not implement gameplay.
- Do not start M1/M2.
- Do not modify `protocol/**`, `docs/adr/**`, GameData schemas/content, or runtime contracts.
- Do not claim runtime gates from documentation changes.

## Required work

1. Update project-state/roadmap/gate docs.
2. Add or refresh handoff/evidence/revalidation templates.
3. Add risk register and owner review process.
4. Add non-production design boards/wireframes if useful.
5. Run source validation if available.
6. Package delta/full-source with SHA.

## Required final decision

Return one of:

- `HANDOFF_DONE_DOCS_READY`
- `BLOCKED_BASELINE`
- `BLOCKED_SCOPE`
- `BLOCKED_VALIDATION`
