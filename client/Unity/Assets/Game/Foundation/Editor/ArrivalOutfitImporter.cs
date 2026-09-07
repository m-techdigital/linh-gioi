using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    public static class ArrivalOutfitImporter
    {
        private const string Directory = "Assets/Game/Art/OnboardingCandidate/";
        private const string ModelPath = Directory + "ArrivalOutfit.fbx";
        private const string PrefabPath = Directory + "Resources/LGOArrivalOutfitCandidate.prefab";

        public static void ValidateLocomotion()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            var animator = prefab.GetComponent<Animator>();
            if (animator.runtimeAnimatorController == null)
                throw new InvalidOperationException("Arrival prefab has no locomotion controller.");
            var controller = (AnimatorController)animator.runtimeAnimatorController;
            var tree = controller.layers[0].stateMachine.states.Single(s => s.state.name == "Locomotion").state.motion as BlendTree;
            if (tree == null) throw new InvalidOperationException("Locomotion must retain its speed blend tree.");
            var clips = tree.children.Select(c => c.motion as AnimationClip).ToArray();
            if (clips.Length != 3 || clips.Any(c => c == null || !c.humanMotion || !c.isLooping || c.length <= 0f) ||
                new[] { "Idle_Loop", "Walk_Loop", "Jog_Fwd_Loop" }.Any(name => clips.Count(c => ClipNamed(c.name, name)) != 1))
                throw new InvalidOperationException("Arrival locomotion requires three looping Humanoid clips.");
            if (AnimationMode.InAnimationMode()) throw new InvalidOperationException("Finish the active animation preview before validating arrival motion.");
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var leg = instance.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                AnimationMode.StartAnimationMode();
                var idle = clips.Single(c => ClipNamed(c.name, "Idle_Loop"));
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(instance, idle, idle.length * 0.4f);
                AnimationMode.EndSampling();
                var sampledAnimator = instance.GetComponent<Animator>();
                foreach (var side in new[] { HumanBodyBones.LeftHand, HumanBodyBones.RightHand })
                {
                    var hand = sampledAnimator.GetBoneTransform(side).position;
                    var shoulder = sampledAnimator.GetBoneTransform(side == HumanBodyBones.LeftHand
                        ? HumanBodyBones.LeftUpperArm : HumanBodyBones.RightUpperArm).position;
                    Debug.Log("LGO_ARRIVAL_IDLE_POSE " + side + " hand=" + hand + " shoulder=" + shoulder);
                    if (hand.y >= shoulder.y)
                        throw new InvalidOperationException("Arrival idle must keep hands below shoulders: " + side);
                }
                foreach (var clip in clips.Where(c => !ClipNamed(c.name, "Idle_Loop")))
                {
                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(instance, clip, clip.length * 0.1f);
                    AnimationMode.EndSampling();
                    var first = leg.localRotation;
                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(instance, clip, clip.length * 0.55f);
                    AnimationMode.EndSampling();
                    var angle = Quaternion.Angle(first, leg.localRotation);
                    if (angle <= 2f) throw new InvalidOperationException("Locomotion leg is static: " + clip.name + " angle=" + angle);
                    Debug.Log("LGO_ARRIVAL_RETARGET_POSE_PASS " + clip.name + " leg_angle=" + angle);
                }
            }
            finally { AnimationMode.StopAnimationMode(); UnityEngine.Object.DestroyImmediate(instance); }
            Debug.Log("LGO_ARRIVAL_LOCOMOTION_IMPORT_PASS " + string.Join(",", clips.Select(c => c.name + ":" + c.length)));
        }

        private static void ConfigureHumanoid(string path, bool animations)
        {
            var importer = (ModelImporter)AssetImporter.GetAtPath(path);
            var names = new Dictionary<string, string>
            {
                { "Hips", "pelvis" }, { "Spine", "spine_01" }, { "Chest", "spine_02" },
                { "UpperChest", "spine_03" }, { "Neck", "neck_01" }, { "Head", "Head" }
            };
            foreach (var side in new[] { "Left", "Right" })
            {
                var suffix = side == "Left" ? "_l" : "_r";
                names.Add(side + "Shoulder", "clavicle" + suffix);
                names.Add(side + "UpperArm", "upperarm" + suffix);
                names.Add(side + "LowerArm", "lowerarm" + suffix);
                names.Add(side + "Hand", "hand" + suffix);
                names.Add(side + "UpperLeg", "thigh" + suffix);
                names.Add(side + "LowerLeg", "calf" + suffix);
                names.Add(side + "Foot", "foot" + suffix);
                names.Add(side + "Toes", "ball" + suffix);
            }
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.humanDescription = new HumanDescription
            {
                human = names.Select(pair => new HumanBone { humanName = pair.Key, boneName = pair.Value,
                    limit = new HumanLimit { useDefaultValues = true } }).ToArray(),
                // Let Unity derive the canonical T-pose; imported FBX transforms are not an Avatar reference pose.
                skeleton = Array.Empty<SkeletonBone>(),
                armStretch = 0.05f, legStretch = 0.05f, upperArmTwist = 0.5f, lowerArmTwist = 0.5f,
                upperLegTwist = 0.5f, lowerLegTwist = 0.5f, feetSpacing = 0f
            };
            importer.importAnimation = animations;
            importer.isReadable = false;
            importer.SaveAndReimport();
        }

        public static void Import()
        {
            if (!AssetDatabase.IsValidFolder(Directory + "Resources"))
                AssetDatabase.CreateFolder(Directory.TrimEnd('/'), "Resources");
            var oldPrefab = Directory + "ArrivalOutfit.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(oldPrefab) != null && AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) == null)
            {
                var error = AssetDatabase.MoveAsset(oldPrefab, PrefabPath);
                if (!string.IsNullOrEmpty(error)) throw new InvalidOperationException(error);
            }
            ConfigureHumanoid(ModelPath, false);
            Validate();
            foreach (var texture in new[] { "Skin", "Eyes" })
            {
                var settings = (TextureImporter)AssetImporter.GetAtPath(Directory + texture + ".png");
                settings.maxTextureSize = texture == "Skin" ? 512 : 128;
                settings.mipmapEnabled = true;
                settings.isReadable = false;
                settings.textureCompression = TextureImporterCompression.CompressedHQ;
                settings.SaveAndReimport();
            }
            SaveCandidatePrefab(ModelPath, PrefabPath);
        }

        public static void ImportKeeper() => NpcAppearanceBaker.BakeKeeper();

        internal static void PrepareKeeperModel()
        {
            var path = Directory + "GateKeeper.fbx";
            ConfigureHumanoid(path, false);
            ValidateModel(path, 4);
            foreach (var name in new[] { "KeeperReconstructionAlbedo", "KeeperReconstructionFace" })
            {
                var settings = (TextureImporter)AssetImporter.GetAtPath(Directory + name + ".png");
                if (settings == null) throw new InvalidOperationException("Missing keeper atlas: " + name);
                settings.maxTextureSize = name.EndsWith("Face", StringComparison.Ordinal) ? 256 : 512;
                settings.mipmapEnabled = true;
                settings.isReadable = false;
                settings.textureCompression = TextureImporterCompression.CompressedHQ;
                foreach (var platform in new[] { "Android", "iPhone" })
                {
                    var mobile = settings.GetPlatformTextureSettings(platform);
                    mobile.overridden = true;
                    mobile.maxTextureSize = settings.maxTextureSize;
                    mobile.format = TextureImporterFormat.ASTC_6x6;
                    settings.SetPlatformTextureSettings(mobile);
                }
                settings.SaveAndReimport();
            }
            var portrait = (TextureImporter)AssetImporter.GetAtPath(Directory + "Resources/LGOGateKeeperPortrait.png");
            portrait.textureType = TextureImporterType.Default;
            portrait.maxTextureSize = 128;
            portrait.mipmapEnabled = false;
            portrait.isReadable = false;
            portrait.alphaIsTransparency = true;
            portrait.textureCompression = TextureImporterCompression.CompressedHQ;
            portrait.SaveAndReimport();
            Debug.Log("LGO_KEEPER_MODEL_READY semantic_sections=4 runtime_atlases=2");
        }

        private static void SaveCandidatePrefab(string modelPath, string prefabPath)
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            try
            {
                var renderer = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                renderer.sharedMaterials = renderer.sharedMaterials.Select(CreateMaterial).ToArray();
                var animator = instance.GetComponent<Animator>();
                animator.applyRootMotion = false;
                animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Directory + "ArrivalLocomotion.controller");
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                AssetDatabase.SaveAssets();
                if (renderer.sharedMaterials.Any(m => m == null || m.shader.name != "Universal Render Pipeline/Lit"))
                    throw new InvalidOperationException("Arrival outfit has an unsupported material.");
                Debug.Log("LGO_ARRIVAL_PREFAB_READY materials=" + renderer.sharedMaterials.Length + " root_motion=false");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }

        public static void ImportLocomotion()
        {
            var clips = ImportNativeClips(true, "Idle_Loop", "Walk_Loop", "Jog_Fwd_Loop");
            var controllerPath = Directory + "ArrivalLocomotion.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            if (!controller.parameters.Any(p => p.name == "Speed")) controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            var machine = controller.layers[0].stateMachine;
            var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Locomotion") ?? machine.AddState("Locomotion");
            var tree = state.motion as BlendTree;
            if (tree == null)
            {
                tree = new BlendTree { name = "Arrival locomotion", blendType = BlendTreeType.Simple1D, blendParameter = "Speed", useAutomaticThresholds = false };
                AssetDatabase.AddObjectToAsset(tree, controller);
                state.motion = tree;
            }
            var names = new[] { "Idle_Loop", "Walk_Loop", "Jog_Fwd_Loop" };
            var speeds = new[] { 0f, 1.6f, 3.6f };
            tree.children = names.Select((name, i) => new ChildMotion
                { motion = clips.Single(c => ClipNamed(c.name, name)), threshold = speeds[i], timeScale = 1f }).ToArray();
            machine.defaultState = state;
            EditorUtility.SetDirty(tree);
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Import();
            ValidateLocomotion();
            var obsolete = Directory + "ArrivalLocomotion.fbx";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(obsolete) != null)
            {
                if (AssetDatabase.GetDependencies(PrefabPath, true).Contains(obsolete))
                    throw new InvalidOperationException("Prefab still references the rejected animation export.");
                AssetDatabase.DeleteAsset(obsolete);
            }
        }

        public static void ImportInteraction()
        {
            var clip = ImportNativeClips(false, "Interact").Single();
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Directory + "ArrivalLocomotion.controller");
            if (controller == null) throw new InvalidOperationException("Import locomotion before the interaction gesture.");
            var machine = controller.layers[0].stateMachine;
            var locomotion = machine.states.Select(s => s.state).Single(s => s.name == "Locomotion");
            var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Interact") ?? machine.AddState("Interact");
            state.motion = clip;
            var exit = state.transitions.FirstOrDefault(t => t.destinationState == locomotion) ?? state.AddTransition(locomotion);
            exit.hasExitTime = true;
            exit.exitTime = 1f;
            exit.hasFixedDuration = true;
            exit.duration = 0.1f;
            EditorUtility.SetDirty(state);
            EditorUtility.SetDirty(exit);
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            ValidateLocomotion();
            var clips = controller.animationClips.Distinct().ToArray();
            var expectedClips = controller.layers.Any(l => l.name == "Conversation") ? 5 : 4;
            if (clips.Length != expectedClips || !clip.humanMotion || clip.isLooping || clip.length <= 0f || clip.length > 3f ||
                AssetDatabase.GetDependencies(PrefabPath, true).Any(p => p.Contains("LGOAnimationImport_")))
                throw new InvalidOperationException("Gesture must be a single short non-looping Humanoid clip without a source-FBX dependency.");
            Debug.Log("LGO_ARRIVAL_GESTURE_IMPORT_PASS clips=" + clips.Length + " human=true looping=false duration=" + clip.length);
        }

        public static void ImportConversation()
        {
            var clip = ImportNativeClips(true, "Idle_Talking_Loop").Single();
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Directory + "ArrivalLocomotion.controller");
            if (controller == null) throw new InvalidOperationException("Import locomotion before conversation.");
            var baseMachine = controller.layers[0].stateMachine;
            var fullBody = baseMachine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Talking");
            if (fullBody != null) baseMachine.RemoveState(fullBody);
            if (!controller.layers.Any(l => l.name == "Conversation")) controller.AddLayer("Conversation");
            var layers = controller.layers;
            var layer = layers.Single(l => l.name == "Conversation");
            var mask = layer.avatarMask;
            if (mask == null)
            {
                mask = new AvatarMask { name = "Conversation head and arms" };
                AssetDatabase.AddObjectToAsset(mask, controller);
            }
            for (var part = AvatarMaskBodyPart.Root; part < AvatarMaskBodyPart.LastBodyPart; part++)
                mask.SetHumanoidBodyPartActive(part, part == AvatarMaskBodyPart.Head ||
                    part == AvatarMaskBodyPart.LeftArm || part == AvatarMaskBodyPart.RightArm ||
                    part == AvatarMaskBodyPart.LeftFingers || part == AvatarMaskBodyPart.RightFingers ||
                    part == AvatarMaskBodyPart.RightHandIK);
            layer.avatarMask = mask;
            layer.defaultWeight = 0f;
            layer.iKPass = true;
            layer.blendingMode = AnimatorLayerBlendingMode.Override;
            var machine = layer.stateMachine;
            var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Talking") ?? machine.AddState("Talking");
            state.motion = clip;
            machine.defaultState = state;
            var guiding = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Guiding") ?? machine.AddState("Guiding");
            guiding.motion = controller.animationClips.Single(c => ClipNamed(c.name, "Idle_Loop"));
            controller.layers = layers;
            EditorUtility.SetDirty(mask);
            EditorUtility.SetDirty(state);
            EditorUtility.SetDirty(guiding);
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            ValidateLocomotion();
            if (controller.animationClips.Distinct().Count() != 5 || !clip.humanMotion || !clip.isLooping ||
                clip.length <= 0f || clip.length > 4f ||
                AssetDatabase.GetDependencies(PrefabPath, true).Any(p => p.Contains("LGOAnimationImport_")))
                throw new InvalidOperationException("Conversation must add only one short looping Humanoid clip without a source-FBX dependency.");
            Debug.Log("LGO_KEEPER_CONVERSATION_IMPORT_PASS clips=5 human=true looping=true head_arms_mask=true default_weight=0 duration=" + clip.length);
        }

        private static AnimationClip[] ImportNativeClips(bool looping, params string[] names)
        {
            var folderName = "LGOAnimationImport_" + Guid.NewGuid().ToString("N");
            var folder = "Assets/" + folderName;
            AssetDatabase.CreateFolder("Assets", folderName);
            try
            {
                var root = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "../../.."));
                var source = System.IO.Path.Combine(root, "build/asset-staging/quaternius-animation/UAL1_Standard.fbx");
                var path = folder + "/Source.fbx";
                System.IO.File.Copy(source, path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                ConfigureHumanoid(path, true);
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                var settings = names.Select(name => importer.defaultClipAnimations.Single(c => ClipNamed(c.name, name))).ToArray();
                foreach (var clip in settings)
                {
                    clip.loopTime = looping;
                    clip.lockRootRotation = true;
                    clip.lockRootHeightY = true;
                    clip.lockRootPositionXZ = true;
                }
                importer.clipAnimations = settings;
                importer.SaveAndReimport();
                var avatar = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().FirstOrDefault();
                if (avatar == null || !avatar.isValid || !avatar.isHuman)
                    throw new InvalidOperationException("Native locomotion Avatar invalid.");
                var imported = AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview__")).ToArray();
                var result = names.Select(name =>
                {
                    var clip = imported.Single(c => ClipNamed(c.name, name));
                    var destination = Directory + name + ".anim";
                    var saved = AssetDatabase.LoadAssetAtPath<AnimationClip>(destination);
                    if (saved == null)
                    {
                        saved = UnityEngine.Object.Instantiate(clip);
                        saved.name = name;
                        saved.hideFlags = HideFlags.None;
                        AssetDatabase.CreateAsset(saved, destination);
                    }
                    else { EditorUtility.CopySerialized(clip, saved); saved.name = name; EditorUtility.SetDirty(saved); }
                    return saved;
                }).ToArray();
                AssetDatabase.SaveAssets();
                return result;
            }
            finally { AssetDatabase.DeleteAsset(folder); }
        }

        private static bool ClipNamed(string actual, string expected) => actual == expected || actual.EndsWith("|" + expected, StringComparison.Ordinal);

        private static Material CreateMaterial(Material source)
        {
            if (source == null) throw new InvalidOperationException("Missing FBX material identity.");
            var name = source.name;
            var color = Color.white;
            string texture = null;
            if (name == "Base skin") texture = "Skin";
            else if (name == "Base eyes") texture = "Eyes";
            else if (name == "Keeper Reconstruction") texture = "KeeperReconstructionAlbedo";
            else if (name == "Keeper Reconstruction Face") texture = "KeeperReconstructionFace";
            else if (name == "Arrival unbleached cloth") color = new Color(0.65f, 0.63f, 0.57f);
            else if (name == "Arrival muted blue sash") color = new Color(0.09f, 0.15f, 0.19f);
            else if (name == "Arrival charcoal trousers") color = new Color(0.06f, 0.065f, 0.06f);
            else if (name == "Arrival black hair") color = new Color(0.012f, 0.014f, 0.018f);
            // Blender shader inputs are linear; Unity material color properties use sRGB.
            else if (name == "Keeper ivory cloth") color = new Color(0.76f, 0.77f, 0.72f).gamma;
            else if (name == "Keeper ink cloth") color = new Color(0.012f, 0.027f, 0.06f).gamma;
            else if (name == "Keeper warm brass") color = new Color(0.52f, 0.32f, 0.10f).gamma;
            else if (name == "Keeper gray robe") color = new Color(0.28f, 0.29f, 0.28f);
            else if (name == "Keeper antique gold") color = new Color(0.42f, 0.34f, 0.19f);
            else if (name == "Keeper silver hair") color = new Color(0.68f, 0.70f, 0.69f);
            else throw new InvalidOperationException("Unmapped arrival material: " + name);
            var path = Directory + name.Replace(' ', '_') + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.SetColor("_BaseColor", color);
            var brass = name == "Keeper warm brass";
            material.SetFloat("_Metallic", brass ? 0.65f : 0f);
            material.SetFloat("_Smoothness", brass ? 0.5f : 0.15f);
            material.SetTexture("_BaseMap", texture == null ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(Directory + texture + ".png"));
            EditorUtility.SetDirty(material);
            return material;
        }

        public static void Validate()
        {
            ValidateModel(ModelPath);
        }

        private static void ValidateModel(string modelPath, int materialSections = 6)
        {
            AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceSynchronousImport);
            var avatar = AssetDatabase.LoadAllAssetsAtPath(modelPath).OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isValid || !avatar.isHuman)
                throw new InvalidOperationException("Arrival outfit requires a valid Humanoid Avatar.");
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            var renderers = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderers.Length != 1 || renderers[0].sharedMesh.subMeshCount != materialSections)
                throw new InvalidOperationException("Character must retain one skinned mesh and " + materialSections + " material sections.");
            var baked = new Mesh();
            Bounds bounds;
            try
            {
                // Compensate FBX renderer scale before applying its full local-to-world transform.
                renderers[0].BakeMesh(baked, true);
                var vertices = baked.vertices;
                if (vertices.Length == 0) throw new InvalidOperationException("Arrival outfit has no skinned vertices.");
                bounds = new Bounds(renderers[0].transform.TransformPoint(vertices[0]), Vector3.zero);
                foreach (var vertex in vertices) bounds.Encapsulate(renderers[0].transform.TransformPoint(vertex));
            }
            finally { UnityEngine.Object.DestroyImmediate(baked); }
            if (bounds.size.y < 1.7f || bounds.size.y > 1.95f)
                throw new InvalidOperationException("Arrival outfit scale mismatch: " + bounds);
            Debug.Log("LGO_ARRIVAL_IMPORT_PASS humanoid=true renderers=1 submeshes=" + materialSections + " bounds=" + bounds);
        }
    }
}
