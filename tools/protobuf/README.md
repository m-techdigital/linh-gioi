# Protobuf compiler pin for M0 S5-A

Canonical compiler version for M0 Batch 01 P1/S5-A:

```text
libprotoc 3.13.0
```

The repository carries the accepted Linux x86_64 compiler at:

```text
tools/protobuf/linux-x86_64/protoc
```

Its exact SHA256 is recorded in `tools/protobuf/linux-x86_64/SHA256`. The canonical wrapper verifies both the binary SHA256 and the exact `protoc --version` result before use.

## Compiler override

`PROTOC_BIN` is supported for a different local compiler binary, including another host platform, but an override is accepted only when **both** identity checks are supplied and pass:

```bash
PROTOC_BIN=/absolute/path/to/protoc \
PROTOC_SHA256=<exact-64-hex-sha256> \
./tools/protocol_codegen.sh version
```

The override must:

- point to an executable file;
- match `PROTOC_SHA256` byte-for-byte;
- report exactly `libprotoc 3.13.0`.

There is intentionally no fallback to an ambient `protoc` from `PATH`, because that would make the accepted toolchain depend on hidden workstation/CI state.

## Generated output

Generated code is disposable build output and is not hand-edited or checked in by S5-A. Canonical default paths are:

```text
build/generated/protocol/csharp
build/generated/protocol/java
```

S1 generates/consumes C# with:

```bash
./tools/protocol_codegen.sh generate --language csharp
```

S2 generates/consumes Java with:

```bash
./tools/protocol_codegen.sh generate --language java
```

Full S5-A verification is:

```bash
./tools/protocol_codegen.sh verify
```

For adversarial/determinism clean-room work, `generate` also accepts `--output-root`. If the resolved path is inside the repository it is restricted to `build/**`; source-owned and frozen paths such as `protocol/**`, `client/**`, `server/**`, `gamedata/**`, and `docs/**` are rejected. External temporary directories are allowed.

This verification uses the real selected protobuf compiler for descriptor compilation, C# generation, Java generation, deterministic repeated generation, and a controlled negative compile fixture.

## Portability evidence boundary

Execution evidence for the bundled compiler in S5-A is Linux x86_64 only. The bundled ELF is dynamically linked, so a compatible Linux dynamic loader/system C/C++ runtime remains a host prerequisite; it is not a fully static or universally portable binary.

macOS/Windows and other architectures are **not runtime-PASS claims** in S5-A. They require a separately obtained local `protoc` 3.13.0 whose exact binary SHA256 is explicitly supplied through `PROTOC_SHA256` and then independently executed on that host.

The canonical protocol tooling itself has no PyPI/network dependency; it uses Python standard library plus the selected local compiler binary. This statement does not remove the host OS shared-library requirements of that compiler binary.
