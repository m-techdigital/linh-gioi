# M1 Offline Combat Entry Checklist

M1 should not start until `M0_RUNTIME_CLOSED`.

Owner override exception: the project owner may explicitly allow starting M1 from `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED` or `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED`, but this must be written in the task prompt and must acknowledge that Unity runtime gates are still unverified.

Required before normal M1 entry:

- [ ] M0 final decision is `M0_RUNTIME_CLOSED`.
- [ ] Unity Editor 6000.3.2f1 probe PASS.
- [ ] Unity import PASS.
- [ ] Unity compile PASS.
- [ ] Unity asmdef compile PASS.
- [ ] Unity EditMode tests PASS.
- [ ] Bootstrap scene PASS.
- [ ] UI foundation PASS.
- [ ] Java server handshake PASS.
- [ ] Protocol deterministic hash stable.
- [ ] GameData hash stable.
- [ ] Frozen contracts have no drift.
- [ ] No open `FIX_REQUIRED` or `BLOCKED_CONTRACT` item.

M1 allowed scope after entry:
- Offline combat loop prototype only.
- GameData-driven local skill/combat fixtures.
- Minimal HUD/combat visualization using existing UI foundation.

M1 forbidden scope:
- No online persistence.
- No economy/marketplace.
- No guild/social production systems.
- No large content expansion.
- No protocol/schema edits without contract request.

## External Unity evidence override path

M1 may be opened from external Unity evidence only if the project owner explicitly accepts this written condition:

`OWNER_ACCEPTS_EXTERNAL_UNITY_EDITOR_EVIDENCE_FOR_M1_ENTRY`

Required supporting evidence:

- [ ] Unity Editor 6000.3.2f1 evidence ZIP verified.
- [ ] Unity EditMode test count > 0.
- [ ] Linux player smoke artifact verified.
- [ ] Unity player to Java realtime handshake PASS in sandbox.
- [ ] No open `FIX_REQUIRED` remains in M0 source/tooling.
