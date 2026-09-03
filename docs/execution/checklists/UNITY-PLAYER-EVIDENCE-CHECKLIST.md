# Linh Giới Online — Unity Player Evidence Checklist

## External Unity machine / CI

- [ ] Source artifact/SHA recorded.
- [ ] Runtime closure delta/SHA recorded.
- [ ] Unity Editor path recorded.
- [ ] Unity Editor version is exactly `6000.3.2f1`.
- [ ] Linux Build Support installed.
- [ ] `tools/prepare_unity_protocol.py` executed.
- [ ] C# protobuf generated under `Assets/Game/Generated/Protocol`.
- [ ] Unity project import/generator executed.
- [ ] Bootstrap scene generated.
- [ ] UI generated foundation available.
- [ ] Unity EditMode tests executed.
- [ ] EditMode result XML contains test cases.
- [ ] Linux player smoke build executed.
- [ ] Player archive created.
- [ ] Evidence ZIP created.
- [ ] SHA256 files created.
- [ ] Unity Editor, `Library/`, `Temp/`, `Logs/`, JDK, Maven cache, and secrets excluded from artifacts.

## Sandbox verification

- [ ] Source full v0.3.4 or successor extracted.
- [ ] Server runtime kit installed.
- [ ] Java 25 verified.
- [ ] Maven 3.9.16 verified.
- [ ] Server build/test PASS or valid existing evidence reused.
- [ ] Player archive SHA256 PASS.
- [ ] Editor evidence ZIP SHA256 PASS.
- [ ] Evidence ZIP includes Unity version, import, EditMode, build, generated file list, player file list.
- [ ] Evidence proves Unity `6000.3.2f1`.
- [ ] Evidence XML includes test cases; no zero-test PASS.
- [ ] Java realtime server starts in sandbox.
- [ ] Linux Unity player runs in sandbox.
- [ ] Player result JSON reports `status=PASS` and `accepted=true`.
- [ ] Server log records `realtime_started`.
- [ ] Server log records `realtime_handshake_accepted`.
- [ ] Server log records `realtime_stopped`.
- [ ] No orphan server process remains.
- [ ] Report explicitly distinguishes external Editor evidence from in-sandbox player smoke.
