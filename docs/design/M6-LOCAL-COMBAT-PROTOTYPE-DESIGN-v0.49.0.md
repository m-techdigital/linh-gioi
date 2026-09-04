# M6 Local Combat Prototype Design v0.49.0

Decision: `M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0`

## Design Intent

v0.49 converts the previous local target dummy feedback into a deterministic local-only combat prototype. It remains a readable training-yard slice, not production combat.

## State Model

`LocalCombatPrototypeState` owns:

- target id: `2001`;
- selected state;
- range validity;
- cooldown readiness;
- target visual state: idle, selected, hit, recover;
- last accepted sequence;
- last rejected reason;
- placeholder effect amount;
- target HP/readiness for prototype evidence only.

## Contract Use

The prototype uses existing protocol messages:

- `CombatIntent`
- `CombatAccepted`
- `CombatRejected`
- `CombatResult`
- `CombatStateSnapshot`

It uses current GameData values from `skill.sword.wind_slash` and `monster.shadow.slime` semantics without schema changes:

- cooldown: `6000ms`;
- range: `4.5m`;
- placeholder effect amount: `12`;
- dummy max HP/readiness: `120`.

## Accepted Path

When the target is selected, in range, and skill is ready, the local state accepts Wind Slash, emits accepted/result/snapshot objects, reduces local placeholder HP/readiness, triggers hit feedback, and starts cooldown.

## Rejected Paths

- No target selected: `NO_TARGET`.
- Target out of range: `OUT_OF_RANGE`.
- Skill cooldown active: `COOLDOWN_ACTIVE`.
- Invalid target or unknown skill remain guarded in the state module.

## Visual Mapping

- Idle: target dummy idle sprite.
- Selected: target marker and selected dummy sprite.
- Hit: impact spark and target hit sprite.
- Recover/cooldown: cooldown ring and recover dummy sprite.
- Warning/telegraph: existing warning telegraph sprite stays available for readability evidence.
- Button/panel: existing combat placeholder UI textures.

## Non-Claims

No server-authoritative combat, production damage, enemy AI, loot/reward, DB/auth, economy, social, live ops, production art, full MMO readiness, or broader runtime closure is claimed.
