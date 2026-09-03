# M0 Runtime Bootstrap / Verification

## Server runtime

M0 server runtime is pinned to:

- Java 25 LTS; bootstrap default: Eclipse Temurin `25.0.4.1+1` on Linux x86_64.
- Maven `3.9.16`.
- Spring Boot `4.1.1`.
- Netty `4.2.17.Final`.
- Protobuf compiler/runtime line `3.13.0` for the frozen M0 protocol contract.

On a Linux x86_64 machine with Internet access:

```bash
./tools/bootstrap_m0_server_toolchain.sh
./tools/with_m0_server_toolchain.sh ./server/build.sh
./tools/with_m0_server_toolchain.sh ./server/test.sh
./tools/with_m0_server_toolchain.sh ./server/scripts/runtime-smoke.sh
```

Or:

```bash
./tools/bootstrap_and_validate_m0_server.sh
```

The bootstrap installs under `.toolchains/`, which is local/ignored and is not part of project source.
It verifies the Maven archive with the pinned Apache SHA-512. The JDK bootstrap verifies the exact release archive against the release SHA-256 sidecar, or an explicitly supplied `M0_JDK_SHA256`.

Offline/pre-downloaded use:

```bash
M0_JDK_ARCHIVE_PATH=/path/to/OpenJDK25U-jdk_x64_linux_hotspot_25.0.4.1_1.tar.gz \
M0_JDK_SHA256=<exact_sha256> \
M0_MAVEN_ARCHIVE_PATH=/path/to/apache-maven-3.9.16-bin.tar.gz \
./tools/bootstrap_m0_server_toolchain.sh
```

## Unity runtime

M0 client is pinned to Unity `6000.3.2f1` (Unity 6.3 LTS) with URP `17.3.0`.

Install/activate the Unity Editor outside the repository using an authorized Unity installation. Then expose the editor binary:

```bash
export UNITY_EDITOR=/absolute/path/to/Unity
./tools/unity_batch_test.sh
```

`tools/unity_batch_test.sh` prepares disposable C# Protobuf output, opens the project in batch mode, and runs EditMode tests. It never treats a missing editor as PASS.

## Full M0 runtime gate

When Java 25, Maven, dependencies and Unity are available:

```bash
./tools/validate_m0_runtime.sh
```

Source validation remains separate:

```bash
./tools/validate_m0_source.sh
```

A source PASS does not imply runtime PASS.
