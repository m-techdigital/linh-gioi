# Codex Default Code Governance Rules

Copy-paste this section into future long-running prompts:

```text
Before final handoff, read and obey docs/execution/CODE-GOVERNANCE-CONTRACT.md, docs/execution/CODE-OWNERSHIP-MAP.md, and docs/execution/CODE-QUALITY-GATES.md.

Do not only make it work; make it maintainable.
Do not duplicate gameplay logic, UI state logic, world interaction logic, runtime command logic, validators for the same concept, DTO/config formats, or marker/path/version constants when a canonical owner is safer.
Do not weaken validators just to pass.
Every handoff must include a code quality, duplication, ownership, frozen surface, runtime claim, and technical debt audit.
```
