# 03 — Technical Design Document v0.1

## 1. Architecture summary

```text
Unity Client (C#)
   | HTTPS                         | WebSocket/TCP + Protobuf
   v                               v
Spring Boot API              Java Realtime Gateway
   |                               |
   |                               v
   |                           Zone Runtime
   |                     movement/combat/AI/AOI/event
   |                               |
   +---------------+---------------+
                   v
          PostgreSQL + Redis
```

## 2. Service boundaries

### API/business backend
Modular monolith initially. Owns:
- account;
- durable character profile;
- inventory/equipment transactions;
- friends/guild durable records;
- market durable records;
- housing durable records;
- LiveOps/admin APIs.

### Realtime runtime
Separate process. Owns active session/zone simulation:
- connection/session;
- movement;
- combat;
- entity state;
- AI;
- area-of-interest replication;
- active world-event state.

Realtime runtime does not execute database queries every simulation tick.

## 3. Simulation target

Initial hypothesis:
- server simulation: 20 Hz;
- client render: 30/60 FPS;
- normal visible city players: 30;
- event visible players: 40–50 target after LOD/AOI work.

These values are budgets to validate, not promises.

## 4. Networking

Initial transport is WebSocket/TCP with protobuf payloads. UDP is explicitly deferred.

Message envelope must include:
- protocol version;
- sequence / correlation when applicable;
- message type or typed wrapper;
- server time where useful.

Client may locally predict presentation but canonical simulation remains server-owned.

## 5. Protocol source of truth

Only `protocol/*.proto` defines the wire contract.

Generated C# and Java code is disposable output and must not be manually edited.

## 6. GameData source of truth

`gamedata/**` is the canonical tunable content source.

The GameData pipeline must validate:
- globally unique IDs;
- required fields;
- numeric bounds;
- valid referenced IDs;
- schema version;
- deterministic compiled output.

## 7. Persistence

PostgreSQL owns canonical durable state.
Redis may own ephemeral state such as:
- session presence;
- zone routing;
- rate limits;
- temporary distributed locks/caches.

Redis is not canonical inventory or currency storage.

## 8. Currency and item correctness

Every important item instance has an immutable instance ID and version.
Every currency mutation has a transaction ID and ledger reason.
Server APIs must be idempotent where retry can occur.

## 9. Security baseline

Never trust client-supplied:
- damage result;
- item grant;
- currency amount mutation;
- quest completion reward;
- cooldown completion;
- unrestricted teleport position.

## 10. Observability baseline

Structured logs should include at minimum:
- trace/correlation ID;
- player/account ID when authenticated;
- zone/session ID;
- message/action type;
- result/error code.

## 11. Future extraction rule

No subsystem becomes a microservice merely for organization. Extraction requires measured independent scaling, fault-isolation or ownership need.
