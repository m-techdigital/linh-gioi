# Linh Gioi Online Code Governance Contract v1.0

Decision marker: LGO_CODE_GOVERNANCE_CONTRACT_ACCEPTED_v1.0.

Main principle:

Do not only make it work; make it maintainable.

## Anti-Duplication Rules

- No duplicated gameplay logic.
- No duplicated UI state logic.
- No duplicated world interaction logic.
- No duplicated runtime command logic.
- No duplicated validators for the same concept.
- No parallel DTO/config formats for the same concept.
- No repeated marker, path, or version constants spread across unrelated places when a canonical place is safer.

When duplication is found, fix it only when the task scope and risk allow a behavior-preserving cleanup. Otherwise record a follow-up with owner, affected files, and the reason it was deferred.

## Ownership Rules

- UI state belongs in the UI/controller layer.
- World interaction state belongs in the world/runtime layer.
- Runtime config belongs in the foundation/config layer.
- Art runtime metadata belongs in the art runtime catalog and art docs.
- Validation belongs in tools/validators.
- Protocol/schema contracts remain frozen unless explicitly opened by a contract-change task.
- Server/API/realtime logic must not be duplicated in client DTOs.

## Refactor Rules

- Refactor only when it reduces risk or duplication.
- Do not perform broad risky refactors inside feature tasks.
- Keep refactors behavior-preserving unless the task explicitly changes behavior.
- Larger cleanup must become follow-up backlog with a concrete owner and validation plan.

## File And Class Cohesion

- Avoid giant monoliths.
- Avoid meaningless 1-2 line files.
- Keep cohesive components near their owning layer.
- Avoid one-off helper sprawl.
- Prefer an existing local pattern before introducing a new abstraction.

## Naming And Version Rules

- Use consistent milestone naming in docs, validators, handoffs, commits, and tags.
- Use consistent runtime marker naming and never infer runtime PASS from source inspection.
- Use consistent validator naming: `tools/validate_<milestone_or_concept>.py`.
- Player-facing copy should be Vietnamese when a production-facing localization pass opens that surface.
- English internal code/test markers are allowed.

## Validator Rules

- Do not weaken validators just to pass.
- Every new convention should have validator coverage when practical.
- Source/static failures are not environment limitations.
- Environment limitations are only for missing tools, missing executable runtimes, incompatible local host setup, or unavailable external services.
- Validators should check ownership, frozen surfaces, stale markers, generated/cache outputs, and non-claims when relevant.

## Handoff Rules

- Every handoff must include a code quality, duplication, and ownership audit.
- Every handoff must include remaining tech debt or follow-up when debt is not fixed.
- Every handoff must state which runtime claims are PASS, UNVERIFIED_ENVIRONMENT, DEFERRED, or NOT CLAIMED.
- Every handoff must list frozen surfaces checked.

## Long-Term Maintainability

- Every milestone must leave source easier or equally easy to maintain.
- Hidden technical debt is not allowed without an explicit follow-up.
- Documentation, validators, and package hygiene are part of the product source, not optional paperwork.
