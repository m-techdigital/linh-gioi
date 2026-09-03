# 11 — Risk Register

This is the active project risk register. Update it when a risk changes owner, severity, trigger, or mitigation.

| ID | Risk | Severity | Trigger | Mitigation | Owner |
|---|---|---:|---|---|---|
| R-001 | Unity Editor cannot run inside hosted sandbox. | High | Unity runtime gate pending. | Use external Unity `6000.3.2f1` evidence + sandbox Linux player replay; keep status honest. | S0/S1/S5 |
| R-002 | Source-only evidence accidentally promoted to runtime PASS. | High | Handoff claims runtime without logs. | Evidence standard requires runtime class and exact command/exit code. | S0/S5 |
| R-003 | Frozen protocol/GameData drift during gameplay work. | High | Combat/session needs new fields. | Create contract-change request before implementation. | S0 |
| R-004 | Scope jumps from foundation into MMO features too early. | Medium | M1/M2 starts before M0 closed. | Phase gates and owner override record. | Owner/S0 |
| R-005 | Unity package/DLL restore instability. | Medium | Network timeout or dependency mismatch. | Pin versions; cache evidence; include dependency logs. | S1/S5 |
| R-006 | Performance budget ignored in early prototypes. | Medium | Feature prototype adds heavy loops/assets. | Check `docs/11-PERFORMANCE-BUDGET.md` during handoff. | S1/S2 |
| R-007 | Legal/publishing requirements discovered late. | Medium | Public alpha/monetization planning starts. | Add legal readiness checklist before M7. | Owner |
| R-008 | Content authoring becomes code-defined and non-deterministic. | Medium | New items/skills added manually in client/server. | GameData compiler and deterministic manifest required. | S4/S5 |
| R-009 | Handoffs become conversation-dependent. | Medium | New sandbox cannot reproduce result. | Mandatory handoff templates and evidence manifest. | All lanes |
| R-010 | Player-facing UX lacks early visual direction. | Low | Implementation starts without screen flow. | Maintain design boards and UX acceptance notes. | S3/S0 |

## Risk update rule

Any handoff that leaves a known limitation must update this file or explicitly state why no risk entry changed.
