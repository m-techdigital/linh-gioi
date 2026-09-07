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
            var clips = animator.runtimeAnimatorController.animationClips.Distinct().Where(c => c.isLooping).ToArray();
            if (clips.Length != 3 || clips.Any(c => !c.humanMotion || !c.isLooping || c.length <= 0f))
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
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            try
            {
                var renderer = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                renderer.sharedMaterials = renderer.sharedMaterials.Select(CreateMaterial).ToArray();
                var animator = instance.GetComponent<Animator>();
                animator.applyRootMotion = false;
                animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Directory + "ArrivalLocomotion.controller");
                PrefabUtility.SaveAsPrefabAsset(instance, PrefabPath);
                AssetDatabase.SaveAssets();
                if (renderer.sharedMaterials.Any(m => m == null || m.shader.name != "Universal Render Pipeline/Lit"))
                    throw new InvalidOperationException("Arrival outfit has an unsupported material.");
                Debug.Log("LGO_ARRIVAL_PREFAB_READY materials=6 skin=512 eyes=128 root_motion=false");
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
            if (clips.Length != 4 || !clip.humanMotion || clip.isLooping || clip.length <= 0f || clip.length > 3f ||
                AssetDatabase.GetDependencies(PrefabPath, true).Any(p => p.Contains("LGOAnimationImport_")))
                throw new InvalidOperationException("Gesture must be a single short non-looping Humanoid clip without a source-FBX dependency.");
            Debug.Log("LGO_ARRIVAL_GESTURE_IMPORT_PASS clips=4 human=true looping=false duration=" + clip.length);
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
            else if (name == "Arrival unbleached cloth") color = new Color(0.65f, 0.63f, 0.57f);
            else if (name == "Arrival muted blue sash") color = new Color(0.09f, 0.15f, 0.19f);
            else if (name == "Arrival charcoal trousers") color = new Color(0.06f, 0.065f, 0.06f);
            else if (name == "Arrival black hair") color = new Color(0.012f, 0.014f, 0.018f);
            else throw new InvalidOperationException("Unmapped arrival material: " + name);
            var path = Directory + name.Replace(' ', '_') + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0.15f);
            material.SetTexture("_BaseMap", texture == null ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(Directory + texture + ".png"));
            EditorUtility.SetDirty(material);
            return material;
        }

        public static void Validate()
        {
            AssetDatabase.ImportAsset(ModelPath, ImportAssetOptions.ForceSynchronousImport);
            var avatar = AssetDatabase.LoadAllAssetsAtPath(ModelPath).OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isValid || !avatar.isHuman)
                throw new InvalidOperationException("Arrival outfit requires a valid Humanoid Avatar.");
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            var renderers = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderers.Length != 1 || renderers[0].sharedMesh.subMeshCount != 6)
                throw new InvalidOperationException("Arrival outfit must retain one skinned mesh and six material sections.");
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
            Debug.Log("LGO_ARRIVAL_IMPORT_PASS humanoid=true renderers=1 submeshes=6 bounds=" + bounds);
        }
    }
}
