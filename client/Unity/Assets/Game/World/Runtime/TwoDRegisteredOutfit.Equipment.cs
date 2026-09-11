using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace LinhGioi.World
{
    public sealed partial class TwoDRegisteredOutfit
    {
        private sealed class EquipmentLayer
        {
            public RigidPart Part;
            public SpriteRenderer Sprite;
            public TwoDRegisteredCloth Cloth;
            public bool Visible { get => Sprite != null ? Sprite.enabled : Cloth.Visible; set { if (Sprite != null) Sprite.enabled = value; else Cloth.Visible = value; } }
        }
        private RigidPack _equipmentPack;
        private readonly Dictionary<string, Texture2D> _equipmentAtlases = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, TwoDRegisteredSpriteSkin.AuthoredMesh> _equipmentBodyMeshes = new Dictionary<string, TwoDRegisteredSpriteSkin.AuthoredMesh>();
        private readonly List<EquipmentLayer> _equipment = new List<EquipmentLayer>();
        private readonly Dictionary<string, SpriteRenderer> _rigidById = new Dictionary<string, SpriteRenderer>();
        public bool RegisteredEquipmentEnabled { get; private set; }
        public int VisibleEquipmentAttachments => _equipment.Count(e => e.Visible && !e.Part.kind.EndsWith("variant", StringComparison.Ordinal));
        public bool UsesEquipmentLowerBody => _equipment.Any(e => e.Visible && e.Part.kind == "joint-cloth" && e.Part.slot == "lower_garment");
        public int VisibleJointGarments => _equipment.Count(e => e.Visible && e.Cloth != null);
        public int VisibleBodyVariants => _equipment.Count(e => e.Visible && e.Part.kind == "native-body-variant");
        private void LoadEquipmentPack(string rigJson)
        {
            var parts = new List<RigidPart>();
            foreach (var gender in new[] { "male", "female" })
            {
                var path = "LGOClasses/" + (gender == "male" ? "VoRegisteredEquipmentLv1/" : "VoRegisteredFemaleEquipmentLv1/");
                var data = Resources.Load<TextAsset>(path + "manifest");
                var atlas = Resources.Load<Texture2D>(path + "equipment-atlas");
                if (data == null || atlas == null) throw new InvalidOperationException("Missing registered equipment source pack: " + gender);
                var pack = JsonUtility.FromJson<RigidPack>(data.text);
                VerifyRig(rigJson, pack.rigSha256);
                _equipmentAtlases.Add(gender, atlas);
                if (!string.IsNullOrEmpty(pack.bodyMesh?.id))
                {
                    if (pack.bodyMesh.vertices == null || pack.bodyMesh.indices == null) throw new InvalidOperationException("Incomplete equipment body mesh");
                    _equipmentBodyMeshes.Add(gender, pack.bodyMesh);
                }
                parts.AddRange(pack.parts);
            }
            _equipmentPack = new RigidPack { parts = parts.ToArray() };
        }

        private SpriteRenderer CreateEquipmentSprite(RigidPart part, List<Sprite> owned, bool native)
        {
            var host = new GameObject("Registered equipment " + part.id).transform;
            host.SetParent(native ? _skinRoot : _bones[part.gender + "_" + part.bone], false);
            host.position = _skinRoot.TransformPoint(new Vector3(part.dx, part.dy, 0));
            var sprite = Sprite.Create(_equipmentAtlases[part.gender], new Rect(part.x, part.y, part.w, part.h), Vector2.one * .5f, 100, 0, SpriteMeshType.FullRect);
            owned.Add(sprite);
            host.localScale = new Vector3(part.worldW / sprite.bounds.size.x, part.worldH / sprite.bounds.size.y, 1);
            var renderer = host.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite; renderer.sortingOrder = part.drawOrder; renderer.enabled = false;
            return renderer;
        }
        private void LoadEquipmentBodyVariants(Part source, TwoDRegisteredSpriteSkin binder,
            TwoDRegisteredSpriteSkin.AuthoredMesh mesh, List<Sprite> owned)
        {
            foreach (var part in _equipmentPack.parts.Where(p => p.gender == source.gender && p.kind == "native-body-variant"))
            {
                var bodyMesh = _equipmentBodyMeshes.TryGetValue(source.gender, out var trimmed) ? trimmed : mesh;
                if (trimmed == null)
                {
                    if (Mathf.Abs(part.worldW - source.worldW) > .000001f || Mathf.Abs(part.worldH - source.worldH) > .000001f
                        || Mathf.Abs(part.dx - source.dx) > .000001f || Mathf.Abs(part.dy - source.dy) > .000001f)
                        throw new InvalidOperationException("Equipment occlusion changed the registered body projection");
                }
                else
                {
                    var r = part.sourceCanvasRect;
                    if (r == null || r.Length != 4) throw new InvalidOperationException("Missing canonical body crop");
                    var lo = TwoDRegisteredSpriteSkin.SourcePoint(r[0], r[3]);
                    var hi = TwoDRegisteredSpriteSkin.SourcePoint(r[2], r[1]);
                    if (Vector2.Distance((lo + hi) * .5f, new Vector2(part.dx, part.dy)) > .000001f
                        || Vector2.Distance(hi - lo, new Vector2(part.worldW, part.worldH)) > .000001f)
                        throw new InvalidOperationException("Trimmed body changed canonical density");
                    foreach (var v in trimmed.vertices)
                        if (!mesh.vertices.Any(o => o.x == v.x && o.y == v.y && o.bone0 == v.bone0 && o.bone1 == v.bone1 && o.weight0 == v.weight0 && o.weight1 == v.weight1))
                            throw new InvalidOperationException("Trimmed body changed anatomy or weights");
                }
                var renderer = CreateEquipmentSprite(part, owned, true);
                binder.Bind(renderer, source.meshColumns, source.meshRows, bodyMesh);
                _equipment.Add(new EquipmentLayer { Part = part, Sprite = renderer });
            }
        }
        private void LoadEquipmentParts(List<Sprite> owned)
        {
            foreach (var part in _equipmentPack.parts.Where(p => p.kind != "native-body-variant"))
            {
                var layer = new EquipmentLayer { Part = part };
                if (part.kind == "joint-cloth")
                {
                    var host = new GameObject("Registered equipment " + part.id); host.transform.SetParent(_skinRoot, false);
                    layer.Cloth = host.AddComponent<TwoDRegisteredCloth>();
                    var template = _layers.First(p => p.Item1.gender == part.gender && p.Item1.kind == "base").Item2.sharedMaterial;
                    layer.Cloth.InitializeJoint(_equipmentAtlases[part.gender], new Rect(part.x, part.y, part.w, part.h), part.sourceCanvasRect,
                        part.jointCloth, _skinRoot, name => _bones[part.gender + "_" + name],
                        name => _boneBind[part.gender + "_" + name], template, part.drawOrder);
                }
                else layer.Sprite = CreateEquipmentSprite(part, owned, false);
                _equipment.Add(layer);
            }
        }
        private void ApplyEquipment(string gender, bool baseOnly, Func<string, bool> equipped)
        {
            if (!RegisteredEquipmentEnabled) return;
            bool Has(string slot) => !baseOnly && equipped(slot);
            var state = new HashSet<string>(_equipment.Where(e => e.Part.gender == gender && e.Part.kind == "native-body-variant")
                .SelectMany(e => e.Part.occlusionState ?? Array.Empty<string>()).Distinct().Where(Has));
            foreach (var entry in _equipment)
            {
                var p = entry.Part;
                var visible = p.gender == gender && (p.kind == "native-body-variant"
                    ? state.SetEquals(p.occlusionState ?? Array.Empty<string>()) : Has(p.slot));
                entry.Visible = visible;
                if (visible && !string.IsNullOrEmpty(p.replaces) && p.kind != "native-body-variant") _rigidById[p.replaces].enabled = false;
            }
            var supported = _equipment.Any(e => e.Part.gender == gender);
            if (supported)
            {
                foreach (var layer in _layers.Where(l => l.Item1.gender == gender))
                    layer.Item2.enabled = layer.Item1.kind == "base" && VisibleBodyVariants == 0;
                foreach (var layer in _clothLayers.Where(l => l.Item1 == gender)) layer.Item2.Visible = !Has("lower_garment");
            }
            VisibleLayers = _layers.Count(l => l.Item2.enabled) + VisibleBodyVariants
                + _equipment.Where(e => e.Visible && !e.Part.kind.EndsWith("variant", StringComparison.Ordinal)).Select(e => e.Part.slot).Distinct().Count();
            UpdateEquipmentPose();
        }
        private void UpdateEquipmentPose()
        {
            foreach (var layer in _equipment) if (layer.Visible && layer.Cloth != null) layer.Cloth.UpdatePose();
        }
        private void AppendEquipmentVertices(Transform root, List<Vector3> result)
        {
            foreach (var layer in _equipment)
            {
                if (!layer.Visible) continue;
                if (layer.Cloth != null) layer.Cloth.AppendVertices(root, result);
                else AppendRendererVertices(layer.Sprite, root, result);
            }
        }
        private static void AppendRendererVertices(SpriteRenderer renderer, Transform root, List<Vector3> result)
        {
            var sprite = renderer.sprite; var skin = renderer.GetComponent<SpriteSkin>();
            if (skin == null)
            {
                foreach (var vertex in sprite.vertices) result.Add(root.InverseTransformPoint(renderer.transform.TransformPoint(vertex)));
                return;
            }
            var positions = sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position);
            var weights = sprite.GetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight);
            var bind = sprite.GetBindPoses();
            for (var i = 0; i < positions.Length; i++)
            {
                var w = weights[i];
                var world = skin.boneTransforms[w.boneIndex0].localToWorldMatrix.MultiplyPoint3x4(bind[w.boneIndex0].MultiplyPoint3x4(positions[i])) * w.weight0
                    + skin.boneTransforms[w.boneIndex1].localToWorldMatrix.MultiplyPoint3x4(bind[w.boneIndex1].MultiplyPoint3x4(positions[i])) * w.weight1;
                result.Add(root.InverseTransformPoint(world));
            }
        }
    }
}
