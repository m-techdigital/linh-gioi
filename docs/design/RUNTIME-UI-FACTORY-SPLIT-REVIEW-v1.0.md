# Runtime UI Factory Split Review v1.0

Status: `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`

## Decision

Do not split `M4PlayableClientController` into screen-level factories yet.

The controller still owns several connected flows: login, Character Hall, world HUD, NPC dialogue, session menu, local settings, and combat-preview presentation. Splitting those screens before stronger behavior coverage would move state coupling into new files without reducing real complexity.

## Evidence

- `M4PlayableClientController.cs`: about 2.1k lines after recent skin adoption work.
- `RuntimeUiSkin.cs`: owns reusable visual styling roles.
- Remaining candidate code is mostly leaf UI construction, responsive placement, state visibility, and texture assignment.

## Approved Next Split

Create a small leaf-level factory only for stateless reusable UI construction:

- section title labels;
- muted body labels;
- status labels;
- readibility rows;
- button rows;
- preview/panel shells only if the call site stays simple.

Do not move:

- login/lobby/world/session flow logic;
- async account/character operations;
- world state refresh;
- local combat intent semantics;
- responsive profile decisions;
- visual evidence controls.

## Rules

- A factory method must be stateless or receive all required inputs explicitly.
- A factory method must not know about account state, selected character, world controller state, or task progression.
- A split must reduce controller line count or repeated construction without hiding behavior.
- Runtime visual style remains in `RuntimeUiSkin`; construction factories should call skin helpers, not duplicate colors.
- Validators must protect the split from drifting into a parallel style system.

## Follow-Up

Proceed with `LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0`: introduce a small reusable factory for safe leaf widgets and migrate a limited set of private static builders.
