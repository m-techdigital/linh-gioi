using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.World
{
    // Own garment topology and UVs; never reuses body cut masks or changes its bones.
    public sealed class TwoDRegisteredCloth : MonoBehaviour
    {
        [Serializable] public sealed class Vertex { public float x, y; public int panel; public float[] influences; }
        [Serializable] public sealed class Panel
        {
            public string bone, secondBone;
            public float pivotX, pivotY, gain, blendStartY, blendEndY;
        }
        [Serializable] public sealed class Data { public Vertex[] vertices; public int[] indices; public Panel[] panels; }
        [Serializable] public sealed class JointData
        {
            public Vertex[] vertices; public int[] indices;
            public string upstream, downstream;
            public float pivotX, pivotY, axisX, axisY, blendStart, blendEnd, maxRelativeDegrees;
        }
        private JointData _joint;
        private Transform _upstream, _downstream;
        private Matrix4x4 _upBind, _downBind;
        private Data _data;
        public void InitializeJoint(Texture2D atlas, Rect atlasRect, int[] sourceRect, JointData data,
            Transform root, Func<string, Transform> bone, Func<string, Matrix4x4> bind, Material template, int order)
        {
            if (data == null || data.blendEnd <= data.blendStart || new Vector2(data.axisX, data.axisY).sqrMagnitude < .000001f)
                throw new ArgumentException("Invalid registered joint garment");
            _joint = data; _upstream = bone(data.upstream); _downstream = bone(data.downstream);
            _upBind = bind(data.upstream); _downBind = bind(data.downstream);
            Initialize(atlas, atlasRect, sourceRect, new Data { vertices = data.vertices, indices = data.indices, panels = Array.Empty<Panel>() },
                root, _upstream, _upBind, bone, template, order);
        }
        public static Vector2 BendJointPoint(Vector2 source, JointData data, float sourceRadians)
        {
            var q = source - new Vector2(data.pivotX, data.pivotY);
            var axis = new Vector2(data.axisX, data.axisY).normalized;
            var t = Vector2.Dot(q, axis);
            var blend = Mathf.Clamp01((t - data.blendStart) / (data.blendEnd - data.blendStart));
            var limit = data.maxRelativeDegrees > 0 ? data.maxRelativeDegrees * Mathf.Deg2Rad : Mathf.PI;
            var theta = Mathf.Clamp(sourceRadians, -limit, limit) * blend; var c = Mathf.Cos(theta); var s = Mathf.Sin(theta);
            return new Vector2(data.pivotX + c * q.x - s * q.y, data.pivotY + s * q.x + c * q.y);
        }
        private Transform _root, _torso;
        private Matrix4x4 _torsoBind;
        private Transform[] _firstBones, _secondBones;
        private Vector3[] _positions;
        private Mesh _mesh;
        private Material _material;
        private MeshRenderer _renderer;
        public bool Visible { get => _renderer.enabled; set => _renderer.enabled = value; }
        public Bounds WorldBounds => _renderer.bounds;
        public void Initialize(Texture2D atlas, Rect atlasRect, int[] sourceRect, Data data,
            Transform root, Transform torso, Matrix4x4 torsoBind,
            Func<string, Transform> bone, Material template, int order)
        {
            if (data?.vertices == null || data.panels == null || data.indices == null || data.indices.Length % 3 != 0)
                throw new ArgumentException("Missing garment topology");
            _data = data; _root = root; _torso = torso; _torsoBind = torsoBind;
            _firstBones = new Transform[data.panels.Length]; _secondBones = new Transform[data.panels.Length];
            for (var i = 0; i < data.panels.Length; i++)
            {
                var p = data.panels[i];
                if (!string.IsNullOrEmpty(p.bone)) _firstBones[i] = bone(p.bone);
                if (!string.IsNullOrEmpty(p.secondBone)) _secondBones[i] = bone(p.secondBone);
            }
            _positions = new Vector3[data.vertices.Length];
            var uv = new Vector2[_positions.Length];
            var colors = new Color32[_positions.Length];
            for (var i = 0; i < _positions.Length; i++)
            {
                var v = data.vertices[i];
                colors[i] = new Color32(255, 255, 255, 255);
                if (_joint == null && (v.influences == null || v.influences.Length != data.panels.Length)) throw new ArgumentException("Missing sewn garment influences");
                if (_joint == null && (v.panel < 0 || v.panel >= data.panels.Length)) throw new ArgumentException("Invalid cloth panel");
                _positions[i] = TwoDRegisteredSpriteSkin.SourcePoint(v.x, v.y);
                uv[i] = new Vector2((atlasRect.x + atlasRect.width * (v.x - sourceRect[0]) / (sourceRect[2] - sourceRect[0])) / atlas.width,
                    (atlasRect.y + atlasRect.height * (sourceRect[3] - v.y) / (sourceRect[3] - sourceRect[1])) / atlas.height);
            }
            foreach (var index in data.indices) if (index < 0 || index >= _positions.Length) throw new ArgumentException("Invalid cloth triangle");
            _mesh = new Mesh { name = "Registered garment panels" }; _mesh.MarkDynamic();
            _mesh.vertices = _positions; _mesh.colors32 = colors; _mesh.uv = uv; _mesh.triangles = data.indices;
            gameObject.AddComponent<MeshFilter>().sharedMesh = _mesh;
            _material = new Material(template) { name = "Registered garment atlas", mainTexture = atlas };
            _renderer = gameObject.AddComponent<MeshRenderer>(); _renderer.sharedMaterial = _material;
            _renderer.sortingOrder = order; _renderer.enabled = false;
        }
        public void UpdatePose()
        {
            if (_joint != null)
            {
                var up = _root.worldToLocalMatrix * _upstream.localToWorldMatrix * _upBind;
                var down = _root.worldToLocalMatrix * _downstream.localToWorldMatrix * _downBind;
                var relative = up.inverse * down;
                var radians = -Mathf.Atan2(relative.m10, relative.m00); // Source Y points down.
                for (var i = 0; i < _positions.Length; i++)
                {
                    var v = _data.vertices[i]; var point = BendJointPoint(new Vector2(v.x, v.y), _joint, radians);
                    _positions[i] = up.MultiplyPoint3x4(TwoDRegisteredSpriteSkin.SourcePoint(point.x, point.y));
                }
                _mesh.vertices = _positions; _mesh.RecalculateBounds();
                return;
            }
            var deformation = _root.worldToLocalMatrix * _torso.localToWorldMatrix * _torsoBind;
            for (var i = 0; i < _positions.Length; i++)
            {
                var v = _data.vertices[i]; var point = Vector3.zero;
                for (var panel = 0; panel < _data.panels.Length; panel++)
                {
                    var influence = v.influences[panel];
                    if (influence == 0) continue;
                    var p = _data.panels[panel];
                    var angle = _firstBones[panel] == null ? 0 : Mathf.DeltaAngle(0, _firstBones[panel].localEulerAngles.z);
                    if (_secondBones[panel] != null) angle += Mathf.DeltaAngle(0, _secondBones[panel].localEulerAngles.z);
                    var weight = p.blendEndY > p.blendStartY ? Mathf.Clamp01((v.y - p.blendStartY) / (p.blendEndY - p.blendStartY)) : 1;
                    var radians = -angle * p.gain * weight * Mathf.Deg2Rad;
                    var cos = Mathf.Cos(radians); var sin = Mathf.Sin(radians);
                    var dx = v.x - p.pivotX; var dy = v.y - p.pivotY;
                    point += influence * (Vector3)TwoDRegisteredSpriteSkin.SourcePoint(p.pivotX + cos * dx - sin * dy, p.pivotY + sin * dx + cos * dy);
                }
                _positions[i] = deformation.MultiplyPoint3x4(point);
            }
            _mesh.vertices = _positions; _mesh.RecalculateBounds();
        }
        public void AppendVertices(Transform root, List<Vector3> output)
        {
            if (!Visible) return;
            foreach (var point in _positions) output.Add(root.InverseTransformPoint(transform.TransformPoint(point)));
        }
        private void OnDestroy()
        {
            if (Application.isPlaying) { Destroy(_mesh); Destroy(_material); }
            else { DestroyImmediate(_mesh); DestroyImmediate(_material); }
        }
    }
}
