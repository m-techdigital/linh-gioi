using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    // Export the actual retargeted poses, so garment previews use the same motion as the game.
    public static class WardrobePoseExporter
    {
        [Serializable] private sealed class Bone { public string name; public float[] rest; public float[] posed; }
        [Serializable] private sealed class Pose { public string name; public float speed; public Bone[] bones; }
        [Serializable] private sealed class Library { public Pose[] poses; }
        private static float[] Values(Matrix4x4 matrix) => Enumerable.Range(0, 16).Select(i => matrix[i / 4, i % 4]).ToArray();

        public static void Export()
        {
            const string root = "Assets/Game/Art/OnboardingCandidate/";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(root + "Resources/LGOArrivalOutfitCandidate.prefab");
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var mesh = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                var poses = new System.Collections.Generic.List<Pose>();
                AnimationMode.StartAnimationMode();
                foreach (var clipName in new[] { "Idle_Loop", "Walk_Loop", "Jog_Fwd_Loop" })
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(root + clipName + ".anim");
                    if (clip == null || !clip.humanMotion) throw new InvalidOperationException("Missing Humanoid clip: " + clipName);
                    for (var frame = 0; frame < (clipName == "Idle_Loop" ? 1 : 8); frame++)
                    {
                        AnimationMode.BeginSampling();
                        AnimationMode.SampleAnimationClip(instance, clip, clip.length * frame / 8f);
                        AnimationMode.EndSampling();
                        poses.Add(new Pose { name = clipName + "-" + frame, speed = clipName == "Idle_Loop" ? 0f : clipName == "Walk_Loop" ? 1.6f : 3.6f,
                            bones = mesh.bones.Select((bone, index) => new Bone { name = bone.name,
                                rest = Values(mesh.sharedMesh.bindposes[index].inverse),
                                posed = Values(mesh.transform.worldToLocalMatrix * bone.localToWorldMatrix) }).ToArray() });
                    }
                }
                var output = Path.GetFullPath("../../build/asset-staging/arrival-wardrobe/runtime-poses.json");
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output, JsonUtility.ToJson(new Library { poses = poses.ToArray() }, true));
                Debug.Log("LGO_WARDROBE_POSES_EXPORTED poses=" + poses.Count + " path=" + output);
            }
            finally { AnimationMode.StopAnimationMode(); UnityEngine.Object.DestroyImmediate(instance); }
        }
    }
}
