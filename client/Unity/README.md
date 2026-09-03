# Unity Client — M0 Foundation

Authoritative editor target: **Unity 6000.3.2f1 (Unity 6.3 LTS)**.

## Before opening the project

From repository root:

```bash
python3 tools/prepare_unity_protocol.py
```

This regenerates the disposable C# protobuf surface from `protocol/*.proto` into:

`client/Unity/Assets/Game/Generated/Protocol`

Do not hand-edit generated protocol code. The generated directory is ignored by source control and rebuilt from the canonical contract.

The project declares:

- URP `17.3.0`;
- Unity Test Framework `1.4.3`;
- NuGetForUnity `4.5.0`;
- Google.Protobuf NuGet `3.13.0`.

A normal network-enabled Unity workstation restores package dependencies. The current ChatGPT sandbox cannot execute Unity or restore Unity/NuGet packages, so Unity compile/runtime claims must be verified on a real Unity 6000.3.2f1 environment.

## Generated project assets

On editor load, `M0ProjectGenerator` creates reproducible assets under `Assets/Game/Generated/`:

- URP asset;
- ThemeTokens asset imported from the authoritative `Assets/Game/UI/design-tokens.json`;
- `Bootstrap.unity`;
- `UIShowcase.unity`;
- PanelSettings.

`Assets/Game/Generated/` is disposable and ignored. This avoids hand-maintained Unity YAML and GUID conflicts during M0.

## Canonical tests

```bash
UNITY_EDITOR=/path/to/Unity ./tools/unity_batch_test.sh
```

The script exits non-zero if Unity is unavailable; an unexecuted Unity test is never treated as PASS.
