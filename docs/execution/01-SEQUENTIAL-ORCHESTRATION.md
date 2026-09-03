# 01 — Sequential Orchestration

## Objective

Turn the static M0 specification into a real reproducible runtime foundation while minimizing early cross-lane rework.

## Execution graph

```text
M0 SPEC v0.1
    |
    v
P0 S0 preflight
    |
    v
P1 S5-A protocol/codegen tooling
    |
    v
P2 S2 Java API + realtime
    |
    v
P3 S1 Unity bootstrap + handshake client
    |
    v
P4 S4 deterministic GameData pipeline
    |
    v
P5 S3 reusable UI foundation
    |
    v
P6 S5-B CI + integrated runtime closure
    |
    v
M0 RUNTIME PASS
```

## Phase status vocabulary

Use only:

- `READY`
- `IN_PROGRESS`
- `VERIFY`
- `ACCEPTED`
- `BLOCKED`

A phase is `ACCEPTED` only after its output has been applied to the current integration baseline and its required verification has passed.

## P0 — S0 preflight

Purpose:

- verify original ZIP checksum;
- unpack to a clean root;
- run static foundation validation;
- apply this execution overlay;
- record current integration baseline manifest.

No production implementation.

Exit:

- original package hash matches;
- static M0 validation PASS;
- no frozen-contract drift.

## P1 — S5-A protocol tooling bootstrap

Purpose:

- choose/pin the protobuf compiler/toolchain within the existing M0 contract;
- create canonical C# and Java generation commands/paths;
- compile/lint current proto sources;
- prove deterministic generation;
- do not claim full CI/Unity/server PASS yet.

Why first:

S1 and S2 should consume one generated-contract path from day one.

## P2 — S2 Java foundation

Purpose:

- Java 25 build;
- Spring Boot API health process;
- Netty realtime process;
- consume generated Java protobuf types;
- implement `ClientHello -> ServerHello` including controlled rejection.

Exit:

server is a stable target for Unity.

## P3 — S1 Unity foundation

Purpose:

- real Unity 6.3 LTS project;
- URP;
- asmdef/module boundaries;
- bootstrap scene;
- consume generated C# protobuf types;
- realtime client abstraction;
- connect to accepted S2 server and complete handshake.

Exit:

real client/server runtime handshake works.

## P4 — S4 GameData foundation

Purpose:

- deterministic schema validation;
- duplicate ID validation;
- starter reference validation;
- deterministic manifest/hash;
- positive + negative fixtures.

Exit:

content pipeline is safe enough for M1 data-driven combat content.

## P5 — S3 UI foundation

Purpose:

- implement reusable primitives on top of the accepted Unity project;
- consume the one canonical design-token source;
- responsive/safe-area showcase;
- no gameplay-screen implementation.

Exit:

M1 can build combat HUD using stable UI primitives rather than one-off widgets.

## P6 — S5-B closure

Purpose:

- integrate current S5-A tooling with accepted S2/S1/S4 outputs;
- root canonical validation commands;
- server build/test;
- Unity batch compile/test where executable;
- GameData tests;
- handshake smoke;
- CI pipeline and diagnostic artifacts.

Do not mark an unavailable lane PASS. Record environmental limitations explicitly.

## M0 final exit gate

All must be verified on the same integrated source/provenance:

- Unity 6.3 LTS project opens and bootstrap runs.
- Java 25 API boots.
- Java realtime boots.
- protobuf C# generation compiles.
- protobuf Java generation compiles.
- Unity sends `ClientHello` and receives accepted `ServerHello`.
- valid GameData passes.
- invalid GameData fails.
- CI/automation executes the intended contract validations/tests, or any environment-only limitation is explicitly documented and not called PASS.

Only then create/declare logical milestone:

`lg-m00-foundation`
