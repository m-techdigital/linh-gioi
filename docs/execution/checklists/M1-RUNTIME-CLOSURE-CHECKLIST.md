# M1 Runtime Closure Checklist

## Entry

- [ ] Source is `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip` or verified successor.
- [ ] M0 final decision remains `M0_RUNTIME_CLOSED`.
- [ ] No open M1 `FIX_REQUIRED` item.
- [ ] Unity Editor is exactly `6000.3.2f1`.
- [ ] Linux Build Support is installed.

## Source gates

- [ ] `./tools/validate_m1_source.sh` PASS.
- [ ] `PROJECT STATE VALIDATION PASS` included.
- [ ] Protocol files unchanged.
- [ ] GameData schemas unchanged.
- [ ] ADR/design-token contracts unchanged.
- [ ] No generated/cache/build artifacts in source delta.

## Unity editor gates

- [ ] `prepare-unity-protocol` PASS.
- [ ] `unity-import-generate` PASS.
- [ ] `unity-editmode` PASS.
- [ ] `unity-editmode-results.xml` exists.
- [ ] Unity EditMode total tests > 0.
- [ ] Unity EditMode failed = 0.
- [ ] Unity EditMode skipped = 0.
- [ ] Required M1 combat/HUD test names are present.
- [ ] `unity-linux-player-build` PASS.
- [ ] Player archive has `LinhGioiM0PlayerSmoke.x86_64`.

## Offline combat smoke gates

- [ ] Evidence ZIP SHA256 PASS.
- [ ] Player archive SHA256 PASS.
- [ ] `M1_EVIDENCE_BUNDLE_VERIFY_PASS`.
- [ ] `run_m1_offline_combat_smoke.sh` executes the Linux player.
- [ ] Offline combat smoke JSON has `status=PASS`.
- [ ] Offline combat smoke JSON has `targetDefeated=true`.
- [ ] Offline combat smoke JSON has `skillId=skill.sword.wind_slash`.
- [ ] Offline combat smoke JSON has `enemyContentId=monster.shadow.slime`.
- [ ] Offline combat smoke JSON has `actionsExecuted > 0`.

## Exit decision

- [ ] `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` only if every required row above is PASS.
- [ ] Otherwise use `M1_RUNTIME_ENVIRONMENT_LIMITED`, `FIX_REQUIRED`, or `BLOCKED_CONTRACT`.
- [ ] Do not start M2 until this checklist closes or owner records an explicit override.
