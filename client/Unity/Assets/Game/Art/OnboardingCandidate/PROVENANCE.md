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
- Retired elder visual residual (historical): the facing greeting exposes dark trouser patches at the robe. They remain with the head/arms mask, so masking is not a garment fix and the full-body clip is not established as their root cause. Inspect the garment mesh/weights separately; do not claim the outfit is visually final.
- Clip plus meta:426885 source bytes after whitespace normalization. This is not a Player build delta or runtime-memory measurement. Foot-bone stability is sampled in runtime; no foot IK, exact sole-ground fit, lip sync or final-art claim.

## Young Gate Keeper candidate — 2026-09-07

- Identity reference: `docs/reference-art/linh-gioi-story-design-reference-pack-v0.1/images/reference_only/02_gate_keeper_character_sheet.png`, subject to the design lock and SCN-001. The young black-haired guide with a wide hat replaces the retired elder candidate. This is a simplified candidate, not approved final character art; face, tailoring and ornament fidelity remain incomplete.
- Recipe: `tools/art/build_gate_keeper_v4.py --source build/asset-staging/quaternius-base/arrival-outfit-study.blend --output <new-staging-directory>`, run through Blender 4.5.13 with `--python-exit-code 1`. Input blend SHA256: `ae218dc3528df4870ec928b1519ae0584516c085ca8f2257ec2c89db22987f66`. Preserve this input; the separate arrival generator has unrelated uncommitted experiments.
- Garments use separate front/side/back patterns in the existing rest skeleton, continuous sleeves with elbow weights, and separate waist/head accessories. The standing guide's front/side panels follow pelvis and cape blends from pelvis at the waist to spine at the shoulders; walking cape fit, body occlusion, interchangeable garment assembly and LOD are not implemented.
- FBX round-trip: 13,074 vertices / 24,764 triangles, six material sections, one exported mesh. Both arrival and guide FBXs have 65 identical bone names/rest matrices (maximum delta 0 in the offline probe). This establishes rest-skeleton compatibility of two baked outfits, not universal mesh or animation compatibility.
- Surface textures reuse Skin512/Eyes128. Three outfit material assets isolate ivory/ink/brass from the arrival player's materials; Blender linear colors are converted with Color.gamma for material color properties, and brass retains metallic0.65/smoothness0.5. Hair and dark trousers share the existing black material. Portrait128 is rendered from the same candidate, not cut from the reference board.
- Pre-v4 dirty FBX/prefab/portrait preserved locally in `build/asset-staging/gate-keeper-v4/runtime-before-v4/`; old FBX SHA256 `d87578995e02e97aea081a1b8026b75dd956aaaeb098064c1d09659f8f50ef07`. This local backup is not distributed through Git.
- Runtime evidence: `keeper-v4-final-mobile` completed SCN-001/002 with final geometry before the color conversion; `keeper-v4-color-mobile` and `keeper-v4-desktop-diagnostic` cover final-color approach/dialogue/close at 960x540 and1920x1080. Screenshots reviewed. Earlier `keeper-v4-color-desktop` failed movement during greeting; unchanged assertions passed after adding diagnostics, so the earlier failure remains unexplained rather than claimed fixed. No physical-device/final-art/visual-pass claim.
