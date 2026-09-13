using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinhGioi.ArchitectureProbe;
using LinhGioi.World;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

namespace LinhGioi.ArchitectureProbe.Editor
{
    public static class LgoSkeletal2DPlayerProbeBuilder
    {
        private const string TempRoot = "Assets/ArchitectureProbeTemp/Skeletal2D";
        private const string ScenePath = TempRoot + "/skeletal-2d-probe.unity";
        private const float PixelsPerUnit = 1536f / 1.7f;

        [Serializable]
        private sealed class BuildEvidence
        {
            public string status, unityVersion, output, sourceProfile;
            public int errors, warnings, spriteSkinCount, bodyCutoutPartCount, garmentRestMasterCount, poseSpecificGarmentSourceCount;
            public bool authoredGarmentWeights, runtimePromotionAllowed;
            public ulong totalSizeBytes;
            public PartRegistrationEvidence[] bodyPartRegistration;
            public JointRegistrationEvidence[] jointRegistration;
            public string jointRegistrationGateStatus;
        }

        [Serializable]
        private sealed class PartRegistrationEvidence
        {
            public string id, bone, method;
            public int textureWidth, textureHeight;
            public Vector2 topCenterPixels, bottomCenterPixels;
            public float sourceAxisLengthPixels, bindLengthWorld, uniformScale;
        }

        [Serializable]
        private sealed class JointRegistrationEvidence
        {
            public string id, partA, partB, status;
            public float partADistanceSourcePixels, partBDistanceSourcePixels, gapUpperBoundSourcePixels;
        }

        [Serializable] private sealed class RigSourceJob { public RigSourcePart[] parts; }
        [Serializable] private sealed class RigSourcePart { public string id, file; }

        public static void Build()
        {
            if (AssetDatabase.IsValidFolder(TempRoot)) AssetDatabase.DeleteAsset(TempRoot);
            Directory.CreateDirectory(ToFullPath(TempRoot));
            var rigQaPath = RequireEnvironment("LGO_SKELETAL2D_RIG_QA");
            var bodyJob = JsonUtility.FromJson<RigSourceJob>(File.ReadAllText(rigQaPath));
            if (bodyJob?.parts == null || bodyJob.parts.Length != 10)
                throw new InvalidOperationException("Neutral rig QA must contain ten isolated cutout parts");
            var rigPartRoot = Path.Combine(Path.GetDirectoryName(rigQaPath), "male");
            var bodyPaths = bodyJob.parts.Select(part => StagePngPath(Path.Combine(rigPartRoot, part.file), "body-" + part.id + ".png")).ToArray();
            var upperAPath = StagePng("LGO_SKELETAL2D_UPPER_A_SOURCE", "upper-a.png");
            var upperBPath = StagePng("LGO_SKELETAL2D_UPPER_B_SOURCE", "upper-b.png");
            var rigidPath = StagePng("LGO_SKELETAL2D_RIGID_SOURCE", "rigid-accessory.png");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (var path in bodyPaths.Concat(new[] { upperAPath, upperBPath, rigidPath })) ConfigureTexture(path);
            var bodySprites = bodyPaths.Select((path, index) => CreateCutoutSpriteAsset(path, "body-" + bodyJob.parts[index].id + ".asset", BodyPivotAtBottom(bodyJob.parts[index].id))).ToArray();
            var upperASprite = CreateSpriteAsset(upperAPath, "upper-a-sprite.asset");
            var upperBSprite = CreateSpriteAsset(upperBPath, "upper-b-sprite.asset");
            var rigidSprite = CreateSpriteAsset(rigidPath, "rigid-accessory-sprite.asset");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var actor = new GameObject("Skeletal2DReviewActor");
            var skeleton = BuildSkeleton(actor.transform);
            var definitions = skeleton.Select(item => item.Definition).ToArray();
            var binder = new TwoDRegisteredSpriteSkin(actor.transform, definitions);

            var skinnedRenderers = new List<SpriteRenderer>();
            var bodyRegistration = new List<PartRegistrationEvidence>();
            var bodyRenderers = new Dictionary<string, SpriteRenderer>();
            for (var index = 0; index < bodyJob.parts.Length; index++)
            {
                var part = bodyJob.parts[index];
                bodyRegistration.Add(AddRigPart(skeleton, part.id, bodySprites[index], out var renderer));
                bodyRenderers.Add(part.id, renderer);
            }
            var jointRegistration = MeasureJointRegistration(skeleton, bodyRenderers);
            var authoredUpper = CreateUpperMesh();
            var upperA = AddLayer(actor.transform, "UpperVariantA", upperASprite, 10);
            var upperASkin = binder.Bind(upperA, 1, 1, authoredUpper);
            var upperB = AddLayer(actor.transform, "UpperVariantB", upperBSprite, 10);
            binder.Bind(upperB, 1, 1, authoredUpper);
            upperB.gameObject.SetActive(false);
            skinnedRenderers.Add(upperA); skinnedRenderers.Add(upperB);
            foreach (var renderer in skinnedRenderers)
            {
                EditorUtility.SetDirty(renderer.sprite);
                AssetDatabase.SaveAssetIfDirty(renderer.sprite);
            }

            var rigid = AddLayer(actor.transform, "RigidAccessory", rigidSprite, 20);
            rigid.transform.SetParent(skeleton.First(item => item.Id == "pelvis").Transform, true);

            var runnerObject = new GameObject("Skeletal2DProbeRunner");
            runnerObject.AddComponent<LgoSkeletal2DPlayerProbe>().Configure(
                actor.transform,
                skeleton.First(item => item.Id == "pelvis").Transform,
                skeleton.First(item => item.Id == "head").Transform,
                skeleton.Select(item => item.Transform).ToArray(),
                upperA.gameObject, upperB.gameObject, rigid.transform, upperASkin);

            var cameraObject = new GameObject("ProbeCamera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true; camera.orthographicSize = 1.02f;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.035f, .045f, .065f);
            camera.transform.position = new Vector3(0, .82f, -10f); camera.tag = "MainCamera";

            EditorSceneManager.SaveScene(scene, ScenePath); AssetDatabase.SaveAssets();
            var output = RequireEnvironment("LGO_SKELETAL2D_PLAYER_OUTPUT");
            var build = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath }, locationPathName = output,
                target = BuildTarget.StandaloneOSX, options = BuildOptions.Development,
            });
            var summary = build.summary;
            var evidence = new BuildEvidence
            {
                status = summary.result == BuildResult.Succeeded ? "PASS" : "FAIL",
                unityVersion = Application.unityVersion, output = summary.outputPath,
                totalSizeBytes = summary.totalSize, errors = summary.totalErrors, warnings = summary.totalWarnings,
                spriteSkinCount = UnityEngine.Object.FindObjectsByType<SpriteSkin>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length,
                bodyCutoutPartCount = 10, garmentRestMasterCount = 1, poseSpecificGarmentSourceCount = 0,
                authoredGarmentWeights = true, sourceProfile = "lgo_character_canvas_1024x1536_v1",
                runtimePromotionAllowed = false, bodyPartRegistration = bodyRegistration.ToArray(),
                jointRegistration = jointRegistration,
                jointRegistrationGateStatus = jointRegistration.All(item => item.status == "PASS") ? "PASS" : "FIX_REQUIRED",
            };
            var report = RequireEnvironment("LGO_SKELETAL2D_BUILD_REPORT");
            Directory.CreateDirectory(Path.GetDirectoryName(report));
            File.WriteAllText(report, JsonUtility.ToJson(evidence, true) + Environment.NewLine);
            if (summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Skeletal 2D build failed: " + summary.result);
        }

        private readonly struct SkeletonBone
        {
            public SkeletonBone(string id, Transform transform, TwoDRegisteredSpriteSkin.Bone definition)
            { Id = id; Transform = transform; Definition = definition; }
            public string Id { get; }
            public Transform Transform { get; }
            public TwoDRegisteredSpriteSkin.Bone Definition { get; }
        }

        private static SkeletonBone[] BuildSkeleton(Transform actor)
        {
            var source = new[]
            {
                new BoneSource("pelvis", -1, new Vector2(534.5f, 719f), new Vector2(510f, 263f)),
                new BoneSource("head", 0, new Vector2(510f, 263f), new Vector2(520f, 77f)),
                new BoneSource("near_upper_arm", 0, new Vector2(403f, 322f), new Vector2(351f, 519f)),
                new BoneSource("near_forearm", 2, new Vector2(351f, 519f), new Vector2(349f, 721f)),
                new BoneSource("far_upper_arm", 0, new Vector2(585f, 342f), new Vector2(640f, 554f)),
                new BoneSource("far_forearm", 4, new Vector2(640f, 554f), new Vector2(686f, 728f)),
                new BoneSource("near_thigh", 0, new Vector2(480f, 718f), new Vector2(407f, 1002f)),
                new BoneSource("near_shin", 6, new Vector2(407f, 1002f), new Vector2(337f, 1484f)),
                new BoneSource("far_thigh", 0, new Vector2(589f, 720f), new Vector2(628f, 1017f)),
                new BoneSource("far_shin", 8, new Vector2(628f, 1017f), new Vector2(645f, 1484f)),
            };
            var result = new SkeletonBone[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                var transform = new GameObject(source[i].Id).transform;
                transform.SetParent(source[i].Parent < 0 ? actor : result[source[i].Parent].Transform, false);
                transform.position = actor.TransformPoint(TwoDRegisteredSpriteSkin.SourcePoint(source[i].Start.x, source[i].Start.y));
                result[i] = new SkeletonBone(source[i].Id, transform,
                    new TwoDRegisteredSpriteSkin.Bone(transform,
                        TwoDRegisteredSpriteSkin.SourcePoint(source[i].Start.x, source[i].Start.y),
                        TwoDRegisteredSpriteSkin.SourcePoint(source[i].End.x, source[i].End.y), source[i].Parent));
            }
            return result;
        }

        private readonly struct BoneSource
        {
            public BoneSource(string id, int parent, Vector2 start, Vector2 end) { Id = id; Parent = parent; Start = start; End = end; }
            public string Id { get; }
            public int Parent { get; }
            public Vector2 Start { get; }
            public Vector2 End { get; }
        }

        private static TwoDRegisteredSpriteSkin.AuthoredMesh CreateUpperMesh()
        {
            const int columns = 5, rows = 9; const float left = 406, right = 636, top = 226, bottom = 676;
            var vertices = new TwoDRegisteredSpriteSkin.AuthoredVertex[(columns + 1) * (rows + 1)];
            var indices = new int[columns * rows * 6];
            for (var y = 0; y <= rows; y++)
            for (var x = 0; x <= columns; x++)
            {
                var sourceY = Mathf.Lerp(top, bottom, (float)y / rows);
                var headWeight = Mathf.Clamp01((410f - sourceY) / 184f);
                vertices[y * (columns + 1) + x] = new TwoDRegisteredSpriteSkin.AuthoredVertex
                {
                    x = Mathf.Lerp(left, right, (float)x / columns), y = sourceY,
                    bone0 = 0, bone1 = 1, weight0 = 1f - headWeight, weight1 = headWeight,
                    region = sourceY < 360 ? "neck" : "torso",
                };
            }
            for (var y = 0; y < rows; y++)
            for (var x = 0; x < columns; x++)
            {
                var vertex = y * (columns + 1) + x; var index = (y * columns + x) * 6;
                indices[index] = vertex; indices[index + 1] = vertex + columns + 2; indices[index + 2] = vertex + 1;
                indices[index + 3] = vertex; indices[index + 4] = vertex + columns + 1; indices[index + 5] = vertex + columns + 2;
            }
            return new TwoDRegisteredSpriteSkin.AuthoredMesh { id = "phap_male_inner_top_skeletal_master_v1", vertices = vertices, indices = indices };
        }

        private static SpriteRenderer AddLayer(Transform actor, string name, Sprite sprite, int order)
        {
            var host = new GameObject(name); host.transform.SetParent(actor, false);
            var renderer = host.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = order;
            return renderer;
        }

        private static PartRegistrationEvidence AddRigPart(SkeletonBone[] skeleton, string id, Sprite sprite, out SpriteRenderer renderer)
        {
            var boneId = id switch
            {
                "head" => "head", "torso-hips" => "pelvis",
                "left-upper-arm" => "near_upper_arm", "left-forearm-hand" => "near_forearm",
                "right-upper-arm" => "far_upper_arm", "right-forearm-hand" => "far_forearm",
                "left-thigh" => "near_thigh", "left-shin-foot" => "near_shin",
                "right-thigh" => "far_thigh", "right-shin-foot" => "far_shin",
                _ => throw new InvalidOperationException("Unknown ten-part rig component: " + id),
            };
            var bone = skeleton.First(item => item.Id == boneId);
            var pivotAtBottom = BodyPivotAtBottom(id);
            renderer = AddLayer(bone.Transform, "Body/" + id, sprite, BodySortingOrder(id));
            var direction = bone.Definition.End - bone.Definition.Start;
            var metrics = LgoSkeletal2DPlayerProbe.MeasureCutoutAxis(
                sprite.texture.GetPixels32(), sprite.texture.width, sprite.texture.height);
            var artDirectionPixels = pivotAtBottom
                ? metrics.TopCenter - metrics.BottomCenter
                : metrics.BottomCenter - metrics.TopCenter;
            renderer.transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(artDirectionPixels, direction));
            var uniformScale = LgoSkeletal2DPlayerProbe.RegisteredUniformScale(
                artDirectionPixels.magnitude, sprite.pixelsPerUnit, direction.magnitude);
            renderer.transform.localScale = Vector3.one * uniformScale;
            return new PartRegistrationEvidence
            {
                id = id, bone = boneId, method = "alpha_endpoint_band_8pct_v1",
                textureWidth = sprite.texture.width, textureHeight = sprite.texture.height,
                topCenterPixels = metrics.TopCenter, bottomCenterPixels = metrics.BottomCenter,
                sourceAxisLengthPixels = artDirectionPixels.magnitude,
                bindLengthWorld = direction.magnitude, uniformScale = uniformScale,
            };
        }

        private static JointRegistrationEvidence[] MeasureJointRegistration(
            SkeletonBone[] skeleton, IReadOnlyDictionary<string, SpriteRenderer> renderers)
        {
            var definitions = new[]
            {
                ("neck", "torso-hips", "head", "head"),
                ("near-shoulder", "torso-hips", "left-upper-arm", "near_upper_arm"),
                ("near-elbow", "left-upper-arm", "left-forearm-hand", "near_forearm"),
                ("far-shoulder", "torso-hips", "right-upper-arm", "far_upper_arm"),
                ("far-elbow", "right-upper-arm", "right-forearm-hand", "far_forearm"),
                ("near-hip", "torso-hips", "left-thigh", "near_thigh"),
                ("near-knee", "left-thigh", "left-shin-foot", "near_shin"),
                ("far-hip", "torso-hips", "right-thigh", "far_thigh"),
                ("far-knee", "right-thigh", "right-shin-foot", "far_shin"),
            };
            return definitions.Select(definition =>
            {
                var joint = skeleton.First(item => item.Id == definition.Item4).Transform.position;
                var distanceA = AlphaDistanceSourcePixels(renderers[definition.Item2], joint);
                var distanceB = AlphaDistanceSourcePixels(renderers[definition.Item3], joint);
                var gap = distanceA + distanceB;
                return new JointRegistrationEvidence
                {
                    id = definition.Item1, partA = definition.Item2, partB = definition.Item3,
                    partADistanceSourcePixels = distanceA, partBDistanceSourcePixels = distanceB,
                    gapUpperBoundSourcePixels = gap, status = gap <= 2f ? "PASS" : "FIX_REQUIRED",
                };
            }).ToArray();
        }

        private static float AlphaDistanceSourcePixels(SpriteRenderer renderer, Vector3 worldPoint)
        {
            var sprite = renderer.sprite;
            var local = renderer.transform.InverseTransformPoint(worldPoint);
            var texturePoint = new Vector2(
                sprite.rect.x + sprite.pivot.x + local.x * sprite.pixelsPerUnit,
                sprite.rect.y + sprite.pivot.y + local.y * sprite.pixelsPerUnit);
            var textureDistance = LgoSkeletal2DPlayerProbe.DistanceToOpaqueAlpha(
                sprite.texture.GetPixels32(), sprite.texture.width, sprite.texture.height, texturePoint);
            var worldPerTexturePixel = renderer.transform.TransformVector(Vector3.right / sprite.pixelsPerUnit).magnitude;
            return textureDistance * worldPerTexturePixel * PixelsPerUnit;
        }

        private static bool BodyPivotAtBottom(string id) => id == "head" || id == "torso-hips";

        private static int BodySortingOrder(string id)
        {
            if (id.StartsWith("right-", StringComparison.Ordinal)) return 0;
            if (id == "torso-hips") return 5;
            if (id == "head") return 6;
            return 12;
        }

        private static string StagePng(string environment, string filename)
        {
            return StagePngPath(RequireEnvironment(environment), filename);
        }

        private static string StagePngPath(string source, string filename)
        {
            if (!File.Exists(source)) throw new FileNotFoundException("Source missing", source);
            var assetPath = TempRoot + "/" + filename;
            File.Copy(source, ToFullPath(assetPath), true);
            return assetPath;
        }

        private static void ConfigureTexture(string assetPath)
        {
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true; importer.isReadable = true; importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None; importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
        }

        private static Sprite CreateSpriteAsset(string texturePath, string filename)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null || texture.width != 1024 || texture.height != 1536)
                throw new InvalidOperationException("Registered texture import failed: " + texturePath);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, 52f / 1536f), PixelsPerUnit, 0, SpriteMeshType.FullRect);
            sprite.name = Path.GetFileNameWithoutExtension(filename);
            AssetDatabase.CreateAsset(sprite, TempRoot + "/" + filename);
            return sprite;
        }

        private static Sprite CreateCutoutSpriteAsset(string texturePath, string filename, bool pivotAtBottom)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null) throw new InvalidOperationException("Cutout texture import failed: " + texturePath);
            var metrics = LgoSkeletal2DPlayerProbe.MeasureCutoutAxis(
                texture.GetPixels32(), texture.width, texture.height);
            var pivotPixels = pivotAtBottom ? metrics.BottomCenter : metrics.TopCenter;
            var pivot = new Vector2(pivotPixels.x / texture.width, pivotPixels.y / texture.height);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                pivot, texture.height, 0, SpriteMeshType.FullRect);
            sprite.name = Path.GetFileNameWithoutExtension(filename);
            AssetDatabase.CreateAsset(sprite, TempRoot + "/" + filename);
            return sprite;
        }

        private static string ToFullPath(string assetPath)
        {
            return Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, assetPath));
        }

        private static string RequireEnvironment(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException("Missing environment variable " + key);
            return value;
        }
    }
}
