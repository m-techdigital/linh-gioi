using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;

namespace LinhGioi.World
{
    // Opt-in draft. One registered body and ten layers share native skinning.
    public sealed partial class TwoDRegisteredOutfit
    {
        [Serializable] private sealed class Pack { public Part[] parts; public Joint[] bones; }
        [Serializable] private sealed class Part
        {
            public string id, gender, kind, slot;
            public int x, y, w, h, order, meshColumns, meshRows;
            public float dx, dy, worldW, worldH;
        }
        [Serializable] private sealed class RigidPack { public string rigSha256; public RigidPart[] parts; public TwoDRegisteredSpriteSkin.AuthoredMesh bodyMesh; }
        [Serializable] private sealed class RigidPart
        {
            public string id, gender, bone, kind, slot, replaces;
            public string[] occlusionState;
            public TwoDRegisteredCloth.JointData jointCloth;
            public int drawOrder;
            public int[] sourceCanvasRect;
            public TwoDRegisteredCloth.Data cloth;
            public int x, y, w, h, order;
            public float dx, dy, worldW, worldH;
        }
        private readonly List<Tuple<string, SpriteRenderer>> _rigidLayers = new List<Tuple<string, SpriteRenderer>>();
        public bool ClosedFarArmsEnabled { get; private set; }
        public bool ClosedBodyEnabled { get; private set; }
        private readonly List<Tuple<string, TwoDRegisteredCloth>> _clothLayers = new List<Tuple<string, TwoDRegisteredCloth>>();
        public int VisibleClothAttachments => _clothLayers.Count(p => p.Item2.Visible);
        public int VisibleRigidAttachments => _rigidLayers.Count(p => p.Item2.enabled)
            + _equipment.Count(e => e.Visible && (e.Part.kind == "rigid-body-variant" || (e.Part.kind == "rigid" && !string.IsNullOrEmpty(e.Part.replaces))));
        public Bounds VisibleWorldBounds()
        {
            var hasBounds = false;
            var bounds = new Bounds();
            void Include(Bounds candidate)
            {
                if (!hasBounds) { bounds = candidate; hasBounds = true; }
                else bounds.Encapsulate(candidate);
            }
            foreach (var layer in _layers) if (layer.Item2.enabled) Include(layer.Item2.bounds);
            foreach (var layer in _rigidLayers) if (layer.Item2.enabled) Include(layer.Item2.bounds);
            foreach (var layer in _clothLayers) if (layer.Item2.Visible) Include(layer.Item2.WorldBounds);
            foreach (var layer in _equipment)
            {
                if (!layer.Visible) continue;
                if (layer.Cloth != null) Include(layer.Cloth.WorldBounds);
                else Include(layer.Sprite.bounds);
            }
            return hasBounds ? bounds : new Bounds(_skinRoot.position, Vector3.zero);
        }
        [Serializable] private sealed class Joint
        {
            public string id, gender;
            public int parent;
            public float startX, startY, endX, endY;
        }
        private readonly Dictionary<string, Transform> _bones = new Dictionary<string, Transform>();
        private readonly List<Tuple<Part, SpriteRenderer>> _layers = new List<Tuple<Part, SpriteRenderer>>();
        private readonly Dictionary<string, Matrix4x4> _boneBind = new Dictionary<string, Matrix4x4>();
        private readonly Dictionary<string, Vector3> _boneRest = new Dictionary<string, Vector3>();
        private readonly Dictionary<string, Vector3> _ankleRest = new Dictionary<string, Vector3>();
        private readonly Dictionary<string, Transform> _targets = new Dictionary<string, Transform>();
        private readonly Dictionary<string, LimbSolver2D> _solvers = new Dictionary<string, LimbSolver2D>();
        private readonly Dictionary<string, Transform> _handTips = new Dictionary<string, Transform>();
        private void Rotate(string gender, string bone, float angle) => _bones[gender + "_" + bone].localRotation = Quaternion.Euler(0, 0, angle);
        private Transform _facingRoot, _rollRoot, _skinRoot;
        public Transform FacingRoot => _facingRoot;
        public Transform BindSpace => _skinRoot;
        public float RollDegrees { get; private set; }
        private TwoDPoseTransition _transition;
        public string CurrentRunPose { get; private set; } = "idle";
        public float RunCycle { get; private set; }
        // Restore the pre-retarget male gait; female boot recovery keeps its tested cadence.
        public static float RunCyclesPerSecond(string gender) => gender == "male" ? 2.5f : 1.5f;
        public void Advance(float seconds)
        {
            _transition?.Advance(seconds);
        }
        public void ApplyMovement(string gender, string motion, float phase, float progress, int facing)
        {
            CurrentRunPose = motion;
            RunCycle = motion == "run" ? Mathf.Repeat(phase * RunCyclesPerSecond(gender), 1f) : 0;
            SampleMovement(gender, motion, phase, progress, facing);
            _transition?.Blend(gender + "_" + motion);
            foreach (var layer in _clothLayers) if (layer.Item2.Visible) layer.Item2.UpdatePose();
            UpdateEquipmentPose();
        }
        private void SampleMovement(string gender, string motion, float phase, float progress, int facing)
        {
            _facingRoot.localScale = new Vector3(facing < 0 ? -1 : 1, 1, 1);
            RollDegrees = motion == "jump" ? TwoDLocomotionCurves.SomersaultDegrees(progress) : 0;
            var jumpLean = motion == "jump" ? Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI) : 0;
            _rollRoot.localPosition = new Vector3(jumpLean * .18f, Point(512, 820).y + jumpLean * .04f, 0);
            _rollRoot.localRotation = Quaternion.Euler(0, 0, RollDegrees - jumpLean * 16f);
            if (motion == "jump")
            {
                var tuck = TwoDLocomotionCurves.Tuck(progress);
                Rotate(gender, "torso-hips", (gender == "male" ? -25 : -35) * tuck);
                if (gender == "male") _bones[gender + "_torso-hips"].localPosition = _boneRest[gender + "_torso-hips"]
                    + (Vector3)(Point(400, 900) - Point(505, 710)) * tuck;
                Rotate(gender, "head", -15 * tuck);
                foreach (var side in new[] { "left", "right" })
                {
                    Rotate(gender, side + "-thigh", 145 * tuck);
                    Rotate(gender, side + "-shin-foot", -150 * tuck);
                    Rotate(gender, side + "-foot", 45 * tuck);
                    Rotate(gender, side + "-upper-arm", 0);
                    Rotate(gender, side + "-forearm-hand", 0);
                    if (tuck > 0)
                    {
                        var key = gender + "_" + side + "-hand";
                        var hand = _handTips[key];
                        var knee = _bones[gender + "_" + side + "-shin-foot"];
                        var rest = _skinRoot.InverseTransformPoint(hand.position);
                        var folded = _skinRoot.InverseTransformPoint(knee.position) + Vector3.up * (gender == "male" ? -.065f : .1f);
                        _targets[key].localPosition = Vector3.Lerp(rest, folded, tuck);
                        // A target returning to rest can still choose the other
                        // elbow solution. Fade the solve itself so both branches
                        // converge to FK at the held-jump restart boundary.
                        _solvers[key].flip = gender == "male";
                        _solvers[key].UpdateIK(tuck);
                    }
                }
                return;
            }
            if (motion != "walk" && motion != "run") return;
            var run = motion == "run";
            var frequency = run ? RunCyclesPerSecond(gender) : 2f;
            var cycle = phase * frequency;
            var swing = Mathf.Sin(cycle * Mathf.PI * 2f);
            Rotate(gender, "torso-hips", run ? -14 : -4);
            Rotate(gender, "head", run ? 10 : 3);
            Rotate(gender, "left-upper-arm", swing * (run ? 42 : 25));
            Rotate(gender, "right-upper-arm", -swing * (run ? 42 : 25));
            Rotate(gender, "left-forearm-hand", (run ? 55 : 20) - swing * 12);
            Rotate(gender, "right-forearm-hand", (run ? 55 : 20) + swing * 12);
            var hip = _bones[gender + "_torso-hips"];
            hip.localPosition = _boneRest[gender + "_torso-hips"] + Vector3.down * (run ? .17f : .1f);
            for (var side = 0; side < 2; side++)
            {
                var key = gender + (side == 0 ? "_left-foot" : "_right-foot");
                var stridePhase = Mathf.Repeat(cycle + side * .5f, 1f);
                var offset = TwoDLocomotionCurves.FootOffset(run, stridePhase);
                var target = _targets[key];
                var center = _ankleRest[key];
                var thigh = _bones[gender + (side == 0 ? "_left-thigh" : "_right-thigh")];
                center.x = _skinRoot.InverseTransformPoint(thigh.position).x;
                target.localPosition = center + (Vector3)offset;
                target.localRotation = Quaternion.identity;
                _solvers[key].UpdateIK(1);
                if (gender == "female" && run && offset.y > 0)
                {
                    // A recovering foot follows the shin instead of staying world-flat
                    // through extreme ankle rotation. Planted feet keep the existing target.
                    var rotation = Quaternion.identity;
                    for (var bone = _bones[gender + (side == 0 ? "_left-shin-foot" : "_right-shin-foot")]; bone != _skinRoot; bone = bone.parent)
                        rotation = bone.localRotation * rotation;
                    var shinAngle = Mathf.DeltaAngle(0, rotation.eulerAngles.z);
                    var lift = Mathf.SmoothStep(0, 1, Mathf.Clamp01(offset.y / .1f));
                    var footAngle = (shinAngle - Mathf.Clamp(shinAngle, -60, 60)) * lift;
                    SetBindRotation(key, footAngle);
                    target.localRotation = Quaternion.Euler(0, 0, footAngle);
                }
            }
        }
        private void SetBindRotation(string key, float degrees)
        {
            // Accumulate bone rotations below bind space; facing reflection is outside it.
            var bone = _bones[key];
            var parentRotation = Quaternion.identity;
            for (var parent = bone.parent; parent != _skinRoot; parent = parent.parent)
                parentRotation = parent.localRotation * parentRotation;
            bone.localRotation = Quaternion.Inverse(parentRotation) * Quaternion.Euler(0, 0, degrees);
        }
        private static Vector2 Point(float x, float y) => TwoDRegisteredSpriteSkin.SourcePoint(x, y);
        private Transform CreateLimbSolver(string key, Transform effector, Transform root, bool constrainRotation)
        {
            var target = new GameObject("Registered target " + key).transform;
            target.SetParent(root, false);
            target.position = effector.position;
            _targets.Add(key, target);
            var solver = new GameObject("Registered limb solver " + key).AddComponent<LimbSolver2D>();
            solver.transform.SetParent(root, false);
            solver.GetChain(0).effector = effector;
            solver.GetChain(0).target = target;
            solver.solveFromDefaultPose = false;
            solver.constrainRotation = constrainRotation;
            solver.Initialize();
            if (!solver.isValid) throw new InvalidOperationException("Invalid limb chain " + key);
            _solvers.Add(key, solver);
            return target;
        }
        public TwoDRegisteredOutfit(Transform root, List<Sprite> ownedSprites, bool smoothTransitions = false, bool anatomical = false, bool closedFarArms = false, bool closedBody = false, bool registeredEquipment = false)
        {
            if (closedBody && !anatomical) throw new ArgumentException("Closed body requires explicit anatomy");
            if (registeredEquipment && !closedBody) throw new ArgumentException("Registered equipment requires closed body");
            RegisteredEquipmentEnabled = registeredEquipment;
            ClosedBodyEnabled = closedBody;
            if (closedFarArms && !anatomical) throw new ArgumentException("Closed arms require explicit anatomy ownership");
            ClosedFarArmsEnabled = closedFarArms && !closedBody;
            _facingRoot = new GameObject("Registered facing").transform;
            _facingRoot.SetParent(root, false);
            _rollRoot = new GameObject("Registered somersault pivot").transform;
            _rollRoot.SetParent(_facingRoot, false);
            _rollRoot.localPosition = Vector3.up * Point(512, 820).y;
            _skinRoot = new GameObject("Registered bind space").transform;
            _skinRoot.SetParent(_rollRoot, false);
            _skinRoot.localPosition = Vector3.down * Point(512, 820).y;
            root = _skinRoot;
            const string path = "LGOClasses/VoRegisteredLv1/";
            var data = Resources.Load<TextAsset>(path + "manifest");
            var atlas = Resources.Load<Texture2D>(path + "registered-outfit-atlas");
            if (data == null || atlas == null) throw new InvalidOperationException("Missing registered outfit draft");
            var pack = JsonUtility.FromJson<Pack>(data.text);
            RigidPack bodyPack = null;
            Texture2D bodyAtlas = null;
            if (closedBody)
            {
                var bodyData = Resources.Load<TextAsset>("LGOClasses/VoClosedBodyLv1/manifest");
                bodyAtlas = Resources.Load<Texture2D>("LGOClasses/VoClosedBodyLv1/closed-body-atlas");
                if (bodyData == null || bodyAtlas == null) throw new InvalidOperationException("Missing closed body pack");
                bodyPack = JsonUtility.FromJson<RigidPack>(bodyData.text);
                VerifyRig(data.text, bodyPack.rigSha256);
            }
            if (registeredEquipment) LoadEquipmentPack(data.text);
            var authored = new Dictionary<string, TwoDRegisteredSpriteSkin.AuthoredMesh>();
            if (anatomical)
            {
                var meshData = Resources.Load<TextAsset>(path + "anatomical-meshes");
                if (meshData == null) throw new InvalidOperationException("Missing authored anatomy");
                foreach (var mesh in JsonUtility.FromJson<TwoDRegisteredSpriteSkin.AuthoredPack>(meshData.text).meshes)
                    authored.Add(mesh.id, mesh);
            }
            foreach (var gender in new[] { "male", "female" })
            {
                var definitions = Array.FindAll(pack.bones, b => b.gender == gender);
                var palette = new TwoDRegisteredSpriteSkin.Bone[definitions.Length];
                for (var i = 0; i < definitions.Length; i++)
                {
                    var d = definitions[i];
                    var start = Point(d.startX, d.startY);
                    var bone = new GameObject("Registered " + gender + " " + d.id).transform;
                    bone.SetParent(d.parent < 0 ? root : palette[d.parent].Transform, false);
                    bone.localPosition = d.parent < 0 ? start : start - palette[d.parent].Start;
                    palette[i] = new TwoDRegisteredSpriteSkin.Bone(bone, start, Point(d.endX, d.endY), d.parent);
                    _bones.Add(gender + "_" + d.id, bone);
                    _boneBind.Add(gender + "_" + d.id, bone.worldToLocalMatrix * root.localToWorldMatrix);
                    _boneRest.Add(gender + "_" + d.id, bone.localPosition);
                }
                foreach (var side in new[] { "left", "right" })
                {
                    var key = gender + "_" + side + "-foot";
                    var ankle = _bones[key];
                    var target = CreateLimbSolver(key, ankle, root, true);
                    _ankleRest.Add(key, target.localPosition);
                    var forearm = Array.Find(definitions, d => d.id == side + "-forearm-hand");
                    var hand = new GameObject("Registered handtip " + gender + "_" + side).transform;
                    hand.SetParent(_bones[gender + "_" + forearm.id], false);
                    hand.localPosition = Point(forearm.endX, forearm.endY) - Point(forearm.startX, forearm.startY);
                    var handKey = gender + "_" + side + "-hand";
                    _handTips.Add(handKey, hand);
                    CreateLimbSolver(handKey, hand, root, false);
                }
                var binder = new TwoDRegisteredSpriteSkin(root, palette);
                foreach (var p in pack.parts)
                {
                    if (p.gender != gender || p.kind == "full") continue;
                    var host = new GameObject("Registered " + p.id);
                    host.transform.SetParent(root, false);
                    host.transform.localPosition = new Vector3(p.dx, p.dy, 0);
                    var bodyPart = closedBody && p.kind == "base" ? bodyPack.parts.Single(a => a.gender == gender && a.kind == "native-body") : null;
                    if (bodyPart != null && (Mathf.Abs(bodyPart.worldW - p.worldW) > .000001f || Mathf.Abs(bodyPart.worldH - p.worldH) > .000001f || Mathf.Abs(bodyPart.dx - p.dx) > .000001f || Mathf.Abs(bodyPart.dy - p.dy) > .000001f))
                        throw new InvalidOperationException("Closed body changed the registered base rect");
                    var sprite = Sprite.Create(bodyPart == null ? atlas : bodyAtlas,
                        bodyPart == null ? new Rect(p.x, p.y, p.w, p.h) : new Rect(bodyPart.x, bodyPart.y, bodyPart.w, bodyPart.h), Vector2.one * .5f, 100, 0, SpriteMeshType.FullRect);
                    ownedSprites.Add(sprite);
                    host.transform.localScale = new Vector3(p.worldW / sprite.bounds.size.x, p.worldH / sprite.bounds.size.y, 1);
                    var renderer = host.AddComponent<SpriteRenderer>();
                    renderer.sprite = sprite;
                    renderer.sortingOrder = closedBody ? p.order * (registeredEquipment ? 100 : 10) : p.order;
                    renderer.enabled = false;
                    var mesh = anatomical ? authored[p.id] : null;
                    if (closedFarArms && !closedBody && p.kind == "base") mesh = WithoutFarArm(mesh);
                    binder.Bind(renderer, p.meshColumns, p.meshRows, mesh);
                    _layers.Add(Tuple.Create(p, renderer));
                    if (registeredEquipment && p.kind == "base") LoadEquipmentBodyVariants(p, binder, mesh, ownedSprites);
                }
            }
            if (closedBody)
            {
                LoadRigidParts(bodyPack.parts.Where(a => a.kind == "rigid"), bodyAtlas, ownedSprites);
                foreach (var part in bodyPack.parts.Where(a => a.kind == "fallback-cloth"))
                {
                    var host = new GameObject("Registered cloth " + part.gender); host.transform.SetParent(_skinRoot, false);
                    var cloth = host.AddComponent<TwoDRegisteredCloth>();
                    var key = part.gender + "_torso-hips";
                    var template = _layers.First(a => a.Item1.gender == part.gender && a.Item1.kind == "base").Item2.sharedMaterial;
                    cloth.Initialize(bodyAtlas, new Rect(part.x, part.y, part.w, part.h), part.sourceCanvasRect, part.cloth,
                        _skinRoot, _bones[key], _boneBind[key], name => _bones[part.gender + "_" + name], template, part.drawOrder * (registeredEquipment ? 10 : 1));
                    _clothLayers.Add(Tuple.Create(part.gender, cloth));
                }
            }
            else if (closedFarArms) LoadRigidArms(data.text, ownedSprites);
            if (registeredEquipment) LoadEquipmentParts(ownedSprites);
            if (smoothTransitions) _transition = new TwoDPoseTransition(_bones.Values.ToArray());
        }
        private static TwoDRegisteredSpriteSkin.AuthoredMesh WithoutFarArm(TwoDRegisteredSpriteSkin.AuthoredMesh source)
        {
            var vertices = new List<TwoDRegisteredSpriteSkin.AuthoredVertex>();
            var indices = new List<int>();
            var remap = new Dictionary<int, int>();
            for (var i = 0; i < source.indices.Length; i += 3)
            {
                if (source.vertices[source.indices[i]].region == "far-arm"
                    || source.vertices[source.indices[i + 1]].region == "far-arm"
                    || source.vertices[source.indices[i + 2]].region == "far-arm") continue;
                for (var j = 0; j < 3; j++)
                {
                    var old = source.indices[i + j];
                    if (!remap.TryGetValue(old, out var mapped))
                    { mapped = vertices.Count; vertices.Add(source.vertices[old]); remap.Add(old, mapped); }
                    indices.Add(mapped);
                }
            }
            return new TwoDRegisteredSpriteSkin.AuthoredMesh { id = source.id, vertices = vertices.ToArray(), indices = indices.ToArray() };
        }
        private void LoadRigidArms(string rigJson, List<Sprite> ownedSprites)
        {
            const string path = "LGOClasses/VoClosedFarArmsLv1/";
            var data = Resources.Load<TextAsset>(path + "manifest");
            var atlas = Resources.Load<Texture2D>(path + "closed-far-arm-atlas");
            if (data == null || atlas == null) throw new InvalidOperationException("Missing closed arm source pack");
            var pack = JsonUtility.FromJson<RigidPack>(data.text);
            VerifyRig(rigJson, pack.rigSha256);
            LoadRigidParts(pack.parts, atlas, ownedSprites);
        }
        private static void VerifyRig(string rigJson, string expectedHash)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var hash = BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rigJson))).Replace("-", "").ToLowerInvariant();
                if (hash != expectedHash) throw new InvalidOperationException("Closed arm source does not match the locked rig");
            }
        }
        private void LoadRigidParts(IEnumerable<RigidPart> parts, Texture2D atlas, List<Sprite> ownedSprites)
        {
            foreach (var p in parts)
            {
                var bone = _bones[p.gender + "_" + p.bone];
                var host = new GameObject("Closed arm " + p.id).transform;
                host.SetParent(bone, false);
                // Source rect projection is absolute in bind space. Attachment
                // offset is computed once, then inherited from the shared bone.
                host.position = _skinRoot.TransformPoint(new Vector3(p.dx, p.dy, 0));
                var sprite = Sprite.Create(atlas, new Rect(p.x, p.y, p.w, p.h), Vector2.one * .5f, 100, 0, SpriteMeshType.FullRect);
                ownedSprites.Add(sprite);
                host.localScale = new Vector3(p.worldW / sprite.bounds.size.x, p.worldH / sprite.bounds.size.y, 1);
                var renderer = host.gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite; renderer.sortingOrder = (string.IsNullOrEmpty(p.kind) ? p.order : p.drawOrder) * (RegisteredEquipmentEnabled ? 10 : 1); renderer.enabled = false;
                _rigidLayers.Add(Tuple.Create(p.gender, renderer));
                _rigidById[p.id] = renderer;
            }
        }
        // Read the actual native bind matrices, bone palette and vertex weights.
        // This checks deformation math; screenshots check the rendered result.
        // Authoring diagnostics only: map canonical character coordinates to
        // the sampled pose. Same palette order as the manifest, including IK.
        public Vector3 JointWorldPosition(string gender, string part) => _bones[gender + "_" + part].position;
        public Vector3 HandWorldPosition(string gender, string side) => _handTips[gender + "_" + side + "-hand"].position;
        public Matrix4x4[] SnapshotBoneMatrices(string gender, Transform root)
        {
            var result = new List<Matrix4x4>();
            foreach (var bone in _bones)
                if (bone.Key.StartsWith(gender + "_", StringComparison.Ordinal))
                    result.Add(root.worldToLocalMatrix * bone.Value.localToWorldMatrix * _boneBind[bone.Key]);
            return result.ToArray();
        }
        public Vector3[] SnapshotVertices(Transform root)
        {
            var result = new List<Vector3>();
            foreach (var layer in _layers)
            {
                var renderer = layer.Item2;
                if (!renderer.enabled) continue;
                AppendRendererVertices(renderer, root, result);
            }
            foreach (var layer in _rigidLayers)
                if (layer.Item2.enabled) AppendRendererVertices(layer.Item2, root, result);
            foreach (var layer in _clothLayers) layer.Item2.AppendVertices(root, result);
            AppendEquipmentVertices(root, result);
            return result.ToArray();
        }
        public float MaxFootTargetError(string gender)
        {
            var error = 0f;
            foreach (var side in new[] { "left", "right" })
            {
                var key = gender + "_" + side + "-foot";
                error = Mathf.Max(error, Vector3.Distance(_bones[key].position, _targets[key].position));
            }
            return error;
        }
        public int VisibleLayers { get; private set; }
        public void SetPresentationVisible(bool visible)
        {
            foreach (var renderer in _skinRoot.GetComponentsInChildren<Renderer>(true))
                renderer.forceRenderingOff = !visible;
        }

        public void Apply(string gender, bool baseOnly, Func<string, bool> equipped, Func<string, float> angle)
        {
            foreach (var b in _bones)
            {
                b.Value.localPosition = _boneRest[b.Key];
                b.Value.localRotation = Quaternion.Euler(0, 0, b.Key.StartsWith(gender + "_", StringComparison.Ordinal)
                    ? angle(b.Key.Substring(gender.Length + 1)) : 0);
            }
            foreach (var layer in _rigidLayers) layer.Item2.enabled = layer.Item1 == gender;
            foreach (var layer in _clothLayers) { layer.Item2.Visible = layer.Item1 == gender; if (layer.Item2.Visible) layer.Item2.UpdatePose(); }
            VisibleLayers = 0;
            foreach (var layer in _layers)
            {
                var p = layer.Item1;
                layer.Item2.enabled = p.gender == gender && (p.kind == "base" || (!baseOnly && equipped(p.slot)));
                if (layer.Item2.enabled) VisibleLayers++;
            }
            ApplyEquipment(gender, baseOnly, equipped);
        }
    }
}
