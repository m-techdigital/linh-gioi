# Arrival Outfit Candidate

Draft for the isolated onboarding preview, not final character art.
Reference: `docs/reference-ui/lgo-arrival-outfit-turnaround-draft-v1.jpg`.

- Base body, eyes and eyebrows: Quaternius Universal Base Characters Standard, CC0.
- Publisher: https://quaternius.itch.io/universal-base-characters
- Included license: `License_Quaternius.txt`.
- Clothing, hair study and export adaptations: `tools/art/build_arrival_outfit_candidate.py`.
- Source inputs stay in `build/asset-staging/quaternius-base/`; do not import the whole archive.
- Run the recipe with Blender 4.5.13 `--background --disable-autoexec --python-exit-code 1 --python tools/art/build_arrival_outfit_candidate.py`.
- Unity configuration/prefab: `LinhGioi.Foundation.Editor.ArrivalOutfitImporter.Import`.
- Skin 512px, eyes 128px; no normal/roughness textures. No release-memory budget claim.
- Imported Humanoid mapping is verified separately from animation and visual quality.
- Unity derives the canonical T-pose instead of copying raw FBX transforms. The latter produced raised arms during retargeting; an idle regression now verifies hands below shoulders as well as moving-leg checks.
- Idle/walk/jog: Quaternius Universal Animation Library Standard, CC0, from https://quaternius.itch.io/universal-animation-library . Included license: `License_Animation.txt`.
- Locomotion extraction: `ArrivalOutfitImporter.ImportLocomotion` imports the publisher's Unity FBX from `build/asset-staging/quaternius-animation/UAL1_Standard.fbx` into a unique temporary Assets folder, saves only three native `.anim` clips here, then removes that temporary source.
- No source mannequin or combat clips are retained. A Blender re-export attempt was rejected after runtime/pose checks found static walk/jog tracks; the native Unity route replaces it.
- The prefab is in Resources for the opt-in blockout; its dependencies contribute to Player builds even when the main scene does not instantiate it. No claim of zero payload cost.
- Main player presentation remains unchanged. The preview uses CharacterController velocity, not animation root motion, to drive locomotion.
- Greeting/conversation candidate: `Idle_Talking_Loop`, from the same CC0 Universal Animation Library source. `ArrivalOutfitImporter.ImportConversation` retains one native looping Humanoid clip (2.933333s); no source mannequin, texture or model is added.
- The existing controller is shared with the arrival player. `Conversation.Talking` uses an embedded head/arms Avatar Mask with default weight0 so torso/legs retain locomotion idle. Only the guide raises this layer weight, blended over0.15s. Approach gestures last at most2.5s and never lock player movement or open dialogue automatically; closing dialogue fades the layer out.
- Visual residual: the facing greeting exposes dark trouser patches at the robe. They remain with the head/arms mask, so masking is not a garment fix and the full-body clip is not established as their root cause. Inspect the garment mesh/weights separately; do not claim the outfit is visually final.
- Clip plus meta:426885 source bytes after whitespace normalization. This is not a Player build delta or runtime-memory measurement. Foot-bone stability is sampled in runtime; no foot IK, exact sole-ground fit, lip sync or final-art claim.
