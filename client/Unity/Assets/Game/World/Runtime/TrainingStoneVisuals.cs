using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.World
{
    /// <summary>
    /// SCN-002 low crystal, carved bronze cradle and stepped antique plinth.
    /// One mesh / four shared opaque materials; no texture, particle or light allocation.
    /// The caller owns the generated mesh and materials, and may reuse them on other stones.
    /// Collider, resonance seals and interaction state remain owned by the world.
    /// </summary>
    public static class TrainingStoneVisuals
    {
        // Whole carved assembly budget, replacing the earlier 300-triangle blockout.
        public const int TriangleBudget = 2048;
        private const float CrystalRadius = 0.22f;
        public static Vector3 SealNormal(bool roadFace) => roadFace ? Vector3.left : Vector3.back;
        public static Vector3 SealCenter(bool roadFace) =>
            Vector3.up * 0.61f + SealNormal(roadFace) * (CrystalRadius * Mathf.Cos(Mathf.PI / 8f) + 0.003f);

        public static MeshRenderer Create(Transform parent, Func<Color, Material> materialFactory, bool landmark = false)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (materialFactory == null) throw new ArgumentNullException(nameof(materialFactory));
            var root = new GameObject("Training stone crystal assembly");
            root.transform.SetParent(parent, false);
            root.AddComponent<MeshFilter>().sharedMesh = landmark ? CreateLandmarkMesh() : CreateMesh();
            var renderer = root.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = new[]
            {
                materialFactory(new Color(0.105f, 0.16f, 0.205f)),
                materialFactory(new Color(0.64f, 0.43f, 0.20f)),
                materialFactory(new Color(0.075f, 0.43f, 0.78f)),
                materialFactory(new Color(0.34f, 0.79f, 0.95f))
            };
            if (landmark)
            {
                var shader=Resources.Load<Shader>("LGOTrainingStoneSurface");
                if(shader==null || !shader.isSupported) throw new InvalidOperationException("Landmark surface shader unavailable.");
                var surfaces=renderer.sharedMaterials;
                for(var i=0;i<surfaces.Length;i++)
                {
                    var tint=i==0 ? new Color(.35f,.36f,.34f) : surfaces[i].GetColor("_BaseColor");surfaces[i].shader=shader;
                    surfaces[i].SetColor("_BaseColor",tint);surfaces[i].SetFloat("_SurfaceKind",Mathf.Min(i,2));
                    surfaces[i].SetFloat("_Metallic",0f);
                }
            }
            renderer.sharedMaterials[1].SetFloat("_Metallic", 0.65f);
            renderer.sharedMaterials[1].SetFloat("_Smoothness", 0.38f);
            for (var i = 2; i < 4; i++)
            {
                var material = renderer.sharedMaterials[i];
                material.SetFloat("_Smoothness", 0.72f);
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", new Color(0.025f, 0.15f, 0.27f));
            }
            return renderer;
        }

        public static Mesh CreateLandmarkMesh()
        {
            var shape = new Geometry();
            shape.Band(2.5f,0f,2.5f,.16f,0,false,true);
            shape.Band(2.5f,.16f,2.3f,.22f,1);
            shape.Band(2.3f,.22f,2.3f,.65f,0);
            shape.Band(2.3f,.65f,2.4f,.75f,1);
            shape.Band(2.4f,.75f,2.05f,.8f,0,true);
            shape.Band(1.35f,.8f,1.35f,1.25f,0);
            shape.Band(1.35f,1.25f,1.15f,1.4f,1,true);
            shape.Crystal(Vector3.zero,.9f,1.4f,5.1f,6.5f,0f);
            for(var i=0;i<4;i++)
            {
                var angle=(45f+i*90f)*Mathf.Deg2Rad;
                shape.Leaf(angle,1.18f,.8f,3f,.34f);
                var direction=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
                shape.Beam(direction*1.65f+Vector3.up*.8f,direction*1.25f+Vector3.up*1.8f,.18f,.13f,1);
            }
            for(var i=0;i<6;i++)
            {
                var angle=(i*60f+15f)*Mathf.Deg2Rad;
                var center=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*1.4f;
                var bottom=2.5f+(i%3)*.55f;
                shape.Crystal(center,.18f,bottom,bottom+.65f,bottom+1.05f,angle,5);
            }
            for(var i=0;i<8;i++)
            {
                var angle=(i+1)*Mathf.PI/4f;
                var normal=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
                shape.Diamond(normal*2.14f+Vector3.up*.43f,
                    new Vector3(-normal.z,0,normal.x)*.22f,Vector3.up*.18f,normal*.05f,1);
            }
            return shape.Finish();
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

            // Continuous leaf-shaped bronze supports, broad at the shoulder and pointed above.
            // Their diagonal placement leaves both existing seals fully exposed.
            for (var i = 0; i < 4; i++)
            {
                var angle = (45f + i * 90f) * Mathf.Deg2Rad;
                shape.Leaf(angle);
            }
            // Asymmetric side shards read as part of the crystal assembly at gameplay scale.
            for (var i = 0; i < 4; i++)
            {
                var angle = (0f + i * 90f) * Mathf.Deg2Rad;
                var center = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 0.276f;
                shape.Crystal(center, 0.048f, 0.68f, 0.82f + (i % 2) * 0.04f,
                    0.96f + (i % 2) * 0.04f, angle, 5);
            }
            // Inlaid lozenges and raised molding on every face of the stone base.
            for (var i = 0; i < 8; i++)
            {
                var angle = (i + 1) * Mathf.PI / 4f;
                var normal = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                var across = new Vector3(-normal.z, 0f, normal.x);
                var center = normal * 0.262f + Vector3.up * 0.135f;
                shape.Diamond(center, across * 0.031f, Vector3.up * 0.024f, normal * 0.009f, 1);
            }
            return shape.Finish();
        }

        private sealed class Geometry
        {
            private readonly List<Vector3> _vertices = new List<Vector3>(1800);
            private readonly List<int>[] _triangles = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };

            private void Triangle(Vector3 a, Vector3 b, Vector3 c, int material)
            {
                foreach (var vertex in new[] { a, b, c })
                    if (float.IsNaN(vertex.sqrMagnitude) || float.IsInfinity(vertex.sqrMagnitude))
                        throw new InvalidOperationException("Training stone contains a non-finite vertex.");
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
                    // A shallow ridge breaks the long flat prism into irregular mineral facets.
                    var ridge = Vector3.Lerp(waist, upperNext, side % 2 == 0 ? 0.39f : 0.63f);
                    var outward = Vector3.ProjectOnPlane(ridge - center, Vector3.up).normalized;
                    ridge += outward * radius * (side % 2 == 0 ? 0.045f : 0f);
                    Triangle(waist, upper, ridge, color);
                    Triangle(upper, upperNext, ridge, side % 3 == 0 ? 3 : 2);
                    Triangle(upperNext, waistNext, ridge, 2);
                    Triangle(waistNext, waist, ridge, color);
                    // Thin mineral seam, restricted to diagonal faces away from the spiral inlays.
                    if (sides == 8 && side % 2 == 0)
                    {
                        var offset = outward * 0.0015f;
                        Beam(waist + offset, ridge + offset, 0.0025f, 0.003f, 3);
                        Beam(ridge + offset, upperNext + offset, 0.003f, 0.0015f, 3);
                    }
                    Triangle(upper, tip, upperNext, side % 3 == 0 ? 3 : 2);
                    Triangle(center + Vector3.up * bottom, lower, lowerNext, 2);
                }
            }

            public void Diamond(Vector3 center, Vector3 across, Vector3 up, Vector3 raised, int material)
            {
                var tip = center + raised;
                Triangle(center - across, center + up, tip, material);
                Triangle(center + up, center + across, tip, material);
                Triangle(center + across, center - up, tip, material);
                Triangle(center - up, center - across, tip, material);
            }

            public void Leaf(float angle, float radius = .24f, float bottom = .17f, float height = .5f, float width = .076f)
            {
                var normal = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                var across = new Vector3(-normal.z, 0f, normal.x);
                const int steps = 12;
                Vector3 Center(float t) => normal * (radius + height * .09f * Mathf.Sin(t * Mathf.PI))
                    + Vector3.up * (bottom + height * t);
                float Width(float t) => width * .1842105f * (1f - t) + width * Mathf.Pow(Mathf.Max(0f, Mathf.Sin(t * Mathf.PI)), 1.3f);
                for (var i = 0; i < steps; i++)
                {
                    var t = i / (float)steps;
                    var next = (i + 1f) / steps;
                    var c = Center(t); var d = Center(next);
                    var a = c - across * Width(t); var b = c + across * Width(t);
                    var e = d - across * Width(next); var f = d + across * Width(next);
                    // Back, two folded front surfaces, and closed gold rim.
                    var ridge = normal * (width * .237f);
                    Quad(a, e, d + ridge, c + ridge, 1);
                    Quad(c + ridge, d + ridge, f, b, 1);
                    Quad(b, f, e, a, 0);
                    Beam(a + normal * 0.003f, e + normal * 0.003f, 0.006f, 0.005f, 1);
                    Beam(b + normal * 0.003f, f + normal * 0.003f, 0.006f, 0.005f, 1);
                }
                // Dark inset with raised center jewel gives a carved, layered bronze surface.
                Diamond(Center(0.56f) + normal * (width * (.02f/.076f)), across * (width * .592f),
                    Vector3.up * (height * .142f), normal * 0.006f, 0);
                Diamond(Center(0.56f) + normal * (width * (.028f/.076f)), across * (width * .197f),
                    Vector3.up * (height * .07f), normal * 0.009f, 3);
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
