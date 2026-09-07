using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.World
{
    /// <summary>
    /// SCN-002 low, approachable crystal study from the v4 prop reference.
    /// One mesh / four shared opaque materials; no texture, particle or light allocation.
    /// The caller owns the generated mesh and materials, and may reuse them on other stones.
    /// Collider, resonance seals and interaction state remain owned by the world.
    /// </summary>
    public static class TrainingStoneVisuals
    {
        private const float CrystalRadius = 0.22f;
        public static Vector3 SealNormal(bool roadFace) => roadFace ? Vector3.left : Vector3.back;
        public static Vector3 SealCenter(bool roadFace) =>
            Vector3.up * 0.61f + SealNormal(roadFace) * (CrystalRadius * Mathf.Cos(Mathf.PI / 8f) + 0.003f);

        public static MeshRenderer Create(Transform parent, Func<Color, Material> materialFactory)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (materialFactory == null) throw new ArgumentNullException(nameof(materialFactory));
            var root = new GameObject("Training stone crystal assembly");
            root.transform.SetParent(parent, false);
            root.AddComponent<MeshFilter>().sharedMesh = CreateMesh();
            var renderer = root.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = new[]
            {
                materialFactory(new Color(0.105f, 0.16f, 0.205f)),
                materialFactory(new Color(0.64f, 0.43f, 0.20f)),
                materialFactory(new Color(0.075f, 0.43f, 0.78f)),
                materialFactory(new Color(0.34f, 0.79f, 0.95f))
            };
            return renderer;
        }

        public static Mesh CreateMesh()
        {
            var shape = new Geometry();
            // Broad, shallow plinth keeps the existing 0.65 m interaction footprint.
            shape.Band(0.325f, 0f, 0.325f, 0.07f, 0, false, true);
            shape.Band(0.325f, 0.07f, 0.28f, 0.10f, 1);
            shape.Band(0.28f, 0.10f, 0.28f, 0.17f, 0);
            shape.Band(0.28f, 0.17f, 0.29f, 0.19f, 1);
            shape.Band(0.29f, 0.19f, 0.23f, 0.21f, 0, true);
            shape.Crystal(Vector3.zero, CrystalRadius, 0.20f, 0.81f, 1.10f, 0f);

            // Four sculpted bronze claws frame the blue body without covering its front seal.
            for (var i = 0; i < 4; i++)
            {
                var angle = (45f + i * 90f) * Mathf.Deg2Rad;
                var radial = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                var foot = radial * 0.265f + Vector3.up * 0.17f;
                var elbow = radial * 0.265f + Vector3.up * 0.32f;
                var shoulder = radial * 0.218f + Vector3.up * 0.46f;
                var tip = radial * 0.215f + Vector3.up * 0.54f;
                shape.Beam(foot, elbow, 0.041f, 0.026f, 1);
                shape.Beam(elbow, shoulder, 0.026f, 0.02f, 1);
                shape.Beam(shoulder, tip, 0.02f, 0f, 1);
            }

            // Attached satellites, not a huge floating ritual platform.
            for (var i = 0; i < 3; i++)
            {
                var angle = (35f + i * 125f) * Mathf.Deg2Rad;
                var center = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 0.266f;
                shape.Crystal(center, 0.047f, 0.18f, 0.34f + i * 0.035f, 0.46f + i * 0.025f, angle, 4);
            }
            return shape.Finish();
        }

        private sealed class Geometry
        {
            private readonly List<Vector3> _vertices = new List<Vector3>(1800);
            private readonly List<int>[] _triangles = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };

            private void Triangle(Vector3 a, Vector3 b, Vector3 c, int material)
            {
                var index = _vertices.Count;
                _vertices.Add(a); _vertices.Add(b); _vertices.Add(c);
                _triangles[material].Add(index);
                _triangles[material].Add(index + 1);
                _triangles[material].Add(index + 2);
            }

            private void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int material)
            {
                Triangle(a, b, c, material);
                Triangle(a, c, d, material);
            }

            private static Vector3 Ring(float radius, float height, int side, float rotation = 0f, int sides = 8)
            {
                var angle = (side + 0.5f) * Mathf.PI * 2f / sides + rotation;
                return new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
            }

            public void Band(float lowerRadius, float lowerHeight, float upperRadius, float upperHeight, int material, bool capTop = false, bool capBottom = false)
            {
                for (var side = 0; side < 8; side++)
                {
                    var a = Ring(lowerRadius, lowerHeight, side);
                    var b = Ring(upperRadius, upperHeight, side);
                    var c = Ring(upperRadius, upperHeight, side + 1);
                    var d = Ring(lowerRadius, lowerHeight, side + 1);
                    Quad(a, b, c, d, material);
                    if (capTop) Triangle(Vector3.up * upperHeight, c, b, material);
                    if (capBottom) Triangle(Vector3.up * lowerHeight, a, d, material);
                }
            }

            public void Crystal(Vector3 center, float radius, float bottom, float shoulder, float top, float rotation, int sides = 8)
            {
                for (var side = 0; side < sides; side++)
                {
                    var lower = center + Ring(radius * 0.60f, bottom, side, rotation, sides);
                    var lowerNext = center + Ring(radius * 0.60f, bottom, side + 1, rotation, sides);
                    // Vertical main body makes the two existing spiral seals sit on a true face.
                    var waist = center + Ring(radius, Mathf.Lerp(bottom, shoulder, 0.18f), side, rotation, sides);
                    var waistNext = center + Ring(radius, Mathf.Lerp(bottom, shoulder, 0.18f), side + 1, rotation, sides);
                    var upper = center + Ring(radius, shoulder, side, rotation, sides);
                    var upperNext = center + Ring(radius, shoulder, side + 1, rotation, sides);
                    var tip = center + new Vector3(radius * 0.12f, top, -radius * 0.09f);
                    var color = side == 2 || side == 5 ? 3 : 2;
                    Quad(lower, waist, waistNext, lowerNext, color);
                    Quad(waist, upper, upperNext, waistNext, color);
                    Triangle(upper, tip, upperNext, side % 3 == 0 ? 3 : 2);
                    Triangle(center + Vector3.up * bottom, lower, lowerNext, 2);
                }
            }

            public void Beam(Vector3 from, Vector3 to, float startWidth, float endWidth, int material)
            {
                var axis = (to - from).normalized;
                var horizontal = Vector3.Cross(axis, Vector3.forward).normalized;
                if (horizontal.sqrMagnitude < 0.1f) horizontal = Vector3.right;
                var depth = Vector3.Cross(axis, horizontal).normalized;
                var starts = new Vector3[4];
                var ends = new Vector3[4];
                for (var side = 0; side < 4; side++)
                {
                    var angle = (side + 0.5f) * Mathf.PI * 0.5f;
                    var radial = horizontal * Mathf.Cos(angle) + depth * Mathf.Sin(angle);
                    starts[side] = from + radial * startWidth;
                    ends[side] = to + radial * endWidth;
                }
                for (var side = 0; side < 4; side++)
                {
                    var next = (side + 1) % 4;
                    if (endWidth == 0f) Triangle(starts[side], starts[next], to, material);
                    else Quad(starts[side], starts[next], ends[next], ends[side], material);
                }
            }

            public Mesh Finish()
            {
                var mesh = new Mesh { name = "Training stone faceted crystal and bronze plinth" };
                mesh.SetVertices(_vertices);
                mesh.subMeshCount = _triangles.Length;
                for (var i = 0; i < _triangles.Length; i++) mesh.SetTriangles(_triangles[i], i);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                return mesh;
            }
        }
    }
}
