using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace LinhGioi.World
{
    // Every layer samples the same weight field in character space. Cropping a
    // sprite changes its local origin, never its anatomical registration.
    public sealed class TwoDRegisteredSpriteSkin
    {
        public readonly struct Bone
        {
            public Bone(Transform transform, Vector2 start, Vector2 end, int parentIndex)
            {
                Transform = transform;
                Start = start;
                End = end;
                ParentIndex = parentIndex;
            }
            public Transform Transform { get; }
            public Vector2 Start { get; }
            public Vector2 End { get; }
            public int ParentIndex { get; }
        }

        [Serializable] public sealed class AuthoredVertex
        {
            public float x, y, weight0, weight1;
            public int bone0, bone1;
            public string region;
        }
        [Serializable] public sealed class AuthoredMesh
        {
            public string id;
            public AuthoredVertex[] vertices;
            public int[] indices;
        }
        [Serializable] public sealed class AuthoredPack { public AuthoredMesh[] meshes; }
        public static Vector2 SourcePoint(float x, float y) => new Vector2((x - 512f) * 1.7f / 1536f, (1484f - y) * 1.7f / 1536f);
        private readonly Transform _characterRoot;
        private readonly Bone[] _bones;

        public TwoDRegisteredSpriteSkin(Transform characterRoot, Bone[] bones)
        {
            _characterRoot = characterRoot != null ? characterRoot : throw new ArgumentNullException(nameof(characterRoot));
            if (bones == null || bones.Length == 0) throw new ArgumentException("A registered skeleton is required", nameof(bones));
            _bones = (Bone[])bones.Clone();
            for (var i = 0; i < _bones.Length; i++)
                if (_bones[i].Transform == null || _bones[i].ParentIndex >= i || _bones[i].ParentIndex < -1)
                    throw new ArgumentException("Bones must be valid and parent-first", nameof(bones));
        }

        public BoneWeight WeightsAt(Vector2 characterPoint)
        {
            var nearest = 0;
            var next = -1;
            var distance = float.PositiveInfinity;
            var nextDistance = float.PositiveInfinity;
            for (var i = 0; i < _bones.Length; i++)
            {
                var delta = _bones[i].End - _bones[i].Start;
                var t = delta.sqrMagnitude < .000001f ? 0 : Mathf.Clamp01(Vector2.Dot(characterPoint - _bones[i].Start, delta) / delta.sqrMagnitude);
                var d = (characterPoint - (_bones[i].Start + delta * t)).sqrMagnitude;
                if (d < distance)
                {
                    next = nearest; nextDistance = distance;
                    nearest = i; distance = d;
                }
                else if (d < nextDistance) { next = i; nextDistance = d; }
            }
            if (next < 0 || float.IsPositiveInfinity(nextDistance))
                return new BoneWeight { boneIndex0 = nearest, weight0 = 1 };
            // A shared, continuous field avoids seams caused by independently
            // assigning the body and its clothing to different bones.
            var a = 1f / (distance + .0004f);
            var b = 1f / (nextDistance + .0004f);
            return new BoneWeight { boneIndex0 = nearest, boneIndex1 = next, weight0 = a / (a + b), weight1 = b / (a + b) };
        }

        // Mutates only a Sprite owned by this renderer. The caller creates and
        // releases it (Map01A already tracks runtime-created sprites centrally).
        public SpriteSkin Bind(SpriteRenderer renderer, int columns, int rows, AuthoredMesh authored = null)
        {
            if (renderer == null || renderer.sprite == null || columns < 1 || columns > 32 || rows < 1 || rows > 40)
                throw new ArgumentException("A sprite and valid mesh density are required");
            if (renderer.GetComponent<SpriteSkin>() != null)
                throw new InvalidOperationException("Layer already has a SpriteSkin");
            var sprite = renderer.sprite;
            var bounds = sprite.bounds;
            var vertices = new Vector3[(columns + 1) * (rows + 1)];
            var uv = new Vector2[vertices.Length];
            var indices = new ushort[columns * rows * 6];
            var weights = new BoneWeight[vertices.Length];
            var tangents = new Vector4[vertices.Length];
            for (var y = 0; y <= rows; y++)
            for (var x = 0; x <= columns; x++)
            {
                var i = y * (columns + 1) + x;
                vertices[i] = new Vector2(Mathf.Lerp(bounds.min.x, bounds.max.x, (float)x / columns),
                    Mathf.Lerp(bounds.min.y, bounds.max.y, (float)y / rows));
                uv[i] = new Vector2((sprite.rect.x + sprite.rect.width * x / columns) / sprite.texture.width,
                    (sprite.rect.y + sprite.rect.height * y / rows) / sprite.texture.height);
                var characterPoint = _characterRoot.InverseTransformPoint(renderer.transform.TransformPoint(vertices[i]));
                weights[i] = WeightsAt(characterPoint);
                tangents[i] = new Vector4(1, 0, 0, -1);
            }
            for (var y = 0; y < rows; y++)
            for (var x = 0; x < columns; x++)
            {
                var v = y * (columns + 1) + x;
                var i = (y * columns + x) * 6;
                indices[i] = (ushort)v; indices[i + 1] = (ushort)(v + columns + 2); indices[i + 2] = (ushort)(v + 1);
                indices[i + 3] = (ushort)v; indices[i + 4] = (ushort)(v + columns + 1); indices[i + 5] = (ushort)(v + columns + 2);
            }
            if (authored != null)
            {
                if (authored.vertices == null || authored.vertices.Length < 3 || authored.vertices.Length > ushort.MaxValue
                    || authored.indices == null || authored.indices.Length % 3 != 0)
                    throw new ArgumentException("Invalid authored anatomy mesh");
                vertices = new Vector3[authored.vertices.Length];
                uv = new Vector2[vertices.Length]; weights = new BoneWeight[vertices.Length]; tangents = new Vector4[vertices.Length];
                indices = new ushort[authored.indices.Length];
                for (var i = 0; i < vertices.Length; i++)
                {
                    var v = authored.vertices[i];
                    if (v.bone0 < 0 || v.bone0 >= _bones.Length || v.bone1 < 0 || v.bone1 >= _bones.Length
                        || float.IsNaN(v.weight0 + v.weight1) || v.weight0 < 0 || v.weight1 < 0 || Mathf.Abs(v.weight0 + v.weight1 - 1) > .00001f)
                        throw new ArgumentException("Invalid authored bone influence");
                    vertices[i] = renderer.transform.InverseTransformPoint(_characterRoot.TransformPoint(SourcePoint(v.x, v.y)));
                    var fraction = new Vector2((vertices[i].x - bounds.min.x) / bounds.size.x, (vertices[i].y - bounds.min.y) / bounds.size.y);
                    if (fraction.x < -.0001f || fraction.y < -.0001f || fraction.x > 1.0001f || fraction.y > 1.0001f)
                        throw new ArgumentException("Authored vertex outside registered sprite crop");
                    uv[i] = new Vector2((sprite.rect.x + sprite.rect.width * fraction.x) / sprite.texture.width,
                        (sprite.rect.y + sprite.rect.height * fraction.y) / sprite.texture.height);
                    weights[i] = new BoneWeight { boneIndex0 = v.bone0, boneIndex1 = v.bone1, weight0 = v.weight0, weight1 = v.weight1 };
                    tangents[i] = new Vector4(1, 0, 0, -1);
                }
                for (var i = 0; i < indices.Length; i++)
                {
                    if (authored.indices[i] < 0 || authored.indices[i] >= vertices.Length) throw new ArgumentException("Invalid authored triangle index");
                    indices[i] = (ushort)authored.indices[i];
                }
            }
            // Match Unity 2D Animation's SpritePostProcess data path. Unlike
            // OverrideGeometry, this also works during import/EditMode setup.
            using var nativeVertices = new NativeArray<Vector3>(vertices, Allocator.Temp);
            using var nativeUv = new NativeArray<Vector2>(uv, Allocator.Temp);
            using var nativeIndices = new NativeArray<ushort>(indices, Allocator.Temp);
            using var nativeWeights = new NativeArray<BoneWeight>(weights, Allocator.Temp);
            using var nativeTangents = new NativeArray<Vector4>(tangents, Allocator.Temp);
            sprite.SetVertexCount(vertices.Length);
            sprite.SetVertexAttribute(VertexAttribute.Position, nativeVertices);
            sprite.SetVertexAttribute(VertexAttribute.TexCoord0, nativeUv);
            sprite.SetIndices(nativeIndices);
            sprite.SetVertexAttribute(VertexAttribute.BlendWeight, nativeWeights);
            sprite.SetVertexAttribute(VertexAttribute.Tangent, nativeTangents);
            var transforms = new Transform[_bones.Length];
            var definitions = new SpriteBone[_bones.Length];
            var bindPoses = new Matrix4x4[_bones.Length];
            for (var i = 0; i < _bones.Length; i++)
            {
                transforms[i] = _bones[i].Transform;
                bindPoses[i] = transforms[i].worldToLocalMatrix * renderer.transform.localToWorldMatrix;
                definitions[i] = new SpriteBone { name = transforms[i].name, parentId = _bones[i].ParentIndex,
                    position = _bones[i].ParentIndex < 0 ? renderer.transform.InverseTransformPoint(transforms[i].position)
                        : transforms[i].localPosition,
                    rotation = Quaternion.identity, length = Vector2.Distance(_bones[i].Start, _bones[i].End) };
            }
            sprite.SetBones(definitions);
            using var nativeBindPoses = new NativeArray<Matrix4x4>(bindPoses, Allocator.Temp);
            sprite.SetBindPoses(nativeBindPoses);
            var skin = renderer.gameObject.AddComponent<SpriteSkin>();
            skin.SetRootBone(transforms[0]);
            var state = skin.SetBoneTransforms(transforms);
            if (state != SpriteSkinState.Ready)
                throw new InvalidOperationException("Registered SpriteSkin not ready: " + state);
            return skin;
        }
    }
}
