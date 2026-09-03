# Protocol v1

Canonical source: `.proto` files in this directory.

Initial generation contract:

```text
protoc -> C# generated output for Unity
protoc -> Java generated output for server
```

Exact build tooling/version pin is a deliverable of S5 in M0 Batch 01 so it can be reproducible in CI rather than depending on a developer-global `protoc` installation.
