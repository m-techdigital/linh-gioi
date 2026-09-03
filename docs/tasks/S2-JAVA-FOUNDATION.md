# LG-M0-S2 — Java Backend + Realtime Foundation

## Goal
Create reproducible Java 25 server foundation with separate API and realtime process boundaries.

## Allowed paths
- `server/**`

## Forbidden paths
- `protocol/**`
- `gamedata/**`
- `client/**`
- architecture ADRs

## Required work
1. Create one documented build system at server root (Maven or Gradle; do not create competing builds).
2. Target Java 25.
3. API module: Spring Boot 4.x with `/health` or equivalent runtime health endpoint.
4. Realtime module: Netty process with WebSocket/TCP endpoint.
5. Integrate generated Java protobuf messages from canonical `protocol/**` via build/codegen wiring coordinated with S5; do not copy DTO definitions manually.
6. Realtime accepts `ClientHello`, validates version placeholders/config, and replies `ServerHello` with server time.
7. No persistence/business features beyond minimal configuration/health.
8. Structured logging contains connection/session context where available.

## Acceptance
- clean build from documented command;
- API boots and health endpoint passes;
- realtime boots;
- handshake automated integration test passes;
- malformed/unsupported handshake produces controlled rejection, not crash.

## Explicit non-goals
Combat, DB schema, login product flow, inventory, guild, market, Redis cluster, microservices.
