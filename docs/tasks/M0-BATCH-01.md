# M0 Batch 01 — Runtime Foundation

## Baseline

This package is the specification baseline. Before starting parallel implementation, initialize Git and tag/record the exact baseline commit. All five lane sandboxes start from that exact commit.

## Lane graph

```text
                   S0 CONTRACT BASELINE
                           |
          +----------------+----------------+
          |        |        |        |       |
         S1       S2       S3       S4      S5
       Unity     Java      UI     GameData  QA/CI
          |        |        |        |       |
          +--------+--------+--------+-------+
                           |
                    M0 INTEGRATION
```

## Hot-file policy for this batch

S1/S2/S3/S4/S5 must NOT alter `protocol/**`, `docs/adr/**`, or core contract docs. Contract defects become S0 change requests.

## Integration target

The integrated M0 build must prove:
1. Unity bootstrap scene starts.
2. API `/health` works.
3. realtime endpoint starts.
4. generated C# and Java protobuf types compile.
5. client sends `ClientHello` and receives accepted `ServerHello`.
6. GameData validator has a positive and negative fixture test.
7. CI can reproduce the validations.
