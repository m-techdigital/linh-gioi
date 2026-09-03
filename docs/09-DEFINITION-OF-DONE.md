# 09 — Definition of Done

A task is not DONE merely because source compiles.

Required when applicable:

- implementation matches authoritative contract;
- unit tests;
- integration/contract tests;
- client/server error states;
- mobile interaction/layout;
- desktop interaction/layout;
- server authority respected;
- reconnect/retry considered;
- telemetry/logging added for important player actions;
- performance budget checked for impacted path;
- docs/ADR updated if contract changed;
- no generated code manually patched;
- handoff lists changed/deleted files and known limitations;
- integrated against current milestone baseline;
- playable verification completed.

## Task states

`BACKLOG -> READY -> IN_PROGRESS -> VERIFY -> INTEGRATE -> DONE`

`DONE` is only valid after integration into the authoritative baseline.
