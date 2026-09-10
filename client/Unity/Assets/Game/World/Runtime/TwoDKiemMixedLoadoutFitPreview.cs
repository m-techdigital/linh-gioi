using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace LinhGioi.World
{
    public sealed class TwoDKiemMixedLoadoutFitPreview : IDisposable
    {
        private const string Resource = "LGOClasses/KiemMixedLoadoutFitPreview/";
        private readonly Transform _root;
        private readonly TwoDSkeletalPaperDollRig _rig;
        private readonly SpriteLibraryAsset _library;
        private readonly TwoDEquipmentCompatibilityCatalog _catalog;
        private readonly Dictionary<string, SpriteRenderer> _renderers = new Dictionary<string, SpriteRenderer>();
        private readonly Dictionary<string, SpriteSkin> _skins = new Dictionary<string, SpriteSkin>();
        private readonly Dictionary<string, Transform[]> _proxyBones = new Dictionary<string, Transform[]>();
        private readonly HashSet<string> _readySkins = new HashSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, TwoDEquipmentItemDefinition> _items = new Dictionary<string, TwoDEquipmentItemDefinition>();
        private string _gender = "male";
        private string _motion = "idle";

        [Serializable]
        private sealed class Manifest
        {
            public string id;
            public string status;
            public int runtimeEligibleCount;
            public ProofItem[] items;
        }

        [Serializable]
        private sealed class ProofItem
        {
            public string itemId;
            public string gender;
            public string slotId;
            public string attachmentMode;
        }

        public TwoDKiemMixedLoadoutFitPreview(Transform parent, TwoDSkeletalPaperDollRig rig)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            _rig = rig ?? throw new ArgumentNullException(nameof(rig));
            _root = new GameObject("Map01A Kiếm mixed-loadout draft fit preview").transform;
            _root.SetParent(parent, false);
            _library = ScriptableObject.CreateInstance<SpriteLibraryAsset>();

            var manifestAsset = Resources.Load<TextAsset>(Resource + "manifest");
            var sprites = Resources.LoadAll<Sprite>(Resource + "kiem-mixed-loadout-fit-atlas")
                .ToDictionary(sprite => sprite.name, StringComparer.Ordinal);
            if (manifestAsset == null || sprites.Count != 8)
                throw new InvalidOperationException("Missing Kiếm mixed-loadout proof pack");
            var manifest = JsonUtility.FromJson<Manifest>(manifestAsset.text);
            if (manifest == null || manifest.id != "kiem-mixed-loadout-fit-preview-v2"
                || manifest.status != "DRAFT_RUNTIME_FIT" || manifest.runtimeEligibleCount != 0
                || manifest.items == null || manifest.items.Length != 8)
                throw new InvalidOperationException("Invalid Kiếm mixed-loadout proof manifest");

            foreach (var source in manifest.items)
            {
                if (!sprites.TryGetValue(source.itemId, out var sprite))
                    throw new InvalidOperationException("Missing Kiếm proof sprite: " + source.itemId);
                var mode = source.attachmentMode == "Skinned"
                    ? TwoDEquipmentAttachmentMode.Skinned : TwoDEquipmentAttachmentMode.Rigid;
                var bone = BoneForSlot(source.slotId);
                var item = new TwoDEquipmentItemDefinition(source.itemId, source.slotId,
                    source.slotId == "head_hair" ? 30 : source.slotId == "outer_top" ? 20 : source.slotId == "inner_top" ? 10 : 1,
                    "lgo_humanoid_2d_v1", new[] { "common_" + source.gender + "_v1" }, new[] { "kiem" },
                    TwoDEquipmentFitStatus.DraftRuntimeFit,
                    new[] { new TwoDEquipmentAttachmentDefinition(source.slotId, bone, SortForSlot(source.slotId), mode) },
                    new string[0], new string[0]);
                if (!TwoDSpriteLibraryEquipmentAdapter.RegisterDraftFitPreviewItem(_library, item,
                    new Dictionary<string, Sprite> { [source.slotId] = sprite }, out var reason))
                    throw new InvalidOperationException("Cannot register Kiếm proof item: " + reason);
                _items.Add(source.itemId, item);
            }
            _catalog = new TwoDEquipmentCompatibilityCatalog(_items.Values.ToArray());
            CreateTargets();
            SetActive(false, "male", "idle");
        }

        public bool Active => _root.gameObject.activeSelf;
        public int VisibleItemCount => _renderers.Values.Count(renderer => renderer.enabled && renderer.sprite != null);
        public int ValidSpriteSkinCount => _readySkins.Count;
        public string Snapshot => "KiemMixedLoadoutFitPreview: status=DRAFT_RUNTIME_FIT"
            + " | runtimeEligibleCount=0 | atlas=512x512 | items=" + VisibleItemCount
            + " | spriteSkins=" + ValidSpriteSkinCount + "/3 | gender=" + _gender + " | motion=" + _motion
            + " | sharedSkeleton=lgo_humanoid_2d_v1 | productionEquipAllowed=False";

        public void SetActive(bool active, string gender, string motion)
        {
            if (gender != "male" && gender != "female") throw new ArgumentException("Unknown gender", nameof(gender));
            _gender = gender;
            _motion = motion ?? "idle";
            _root.gameObject.SetActive(active);
            if (!active) return;

            var profile = new TwoDCharacterFitProfile("common_" + gender + "_v1", "lgo_humanoid_2d_v1",
                new[] { "head", "torso-hips", "right-forearm-hand" });
            var loadout = new TwoDEquipmentLoadout(profile, "kiem");
            foreach (var slot in new[] { "main_weapon", "head_hair", "inner_top", "outer_top" })
            {
                var itemId = _items.Keys.Single(id => id.Contains("-" + gender + "-") && _items[id].SlotId == slot);
                if (!_catalog.TryEquipDraftFitPreview(loadout, itemId, 30, out var reason))
                    throw new InvalidOperationException("Cannot equip Kiếm proof item: " + reason);
            }
            if (!TwoDSpriteLibraryEquipmentAdapter.TryApply(_library, loadout, _renderers, out var applyReason))
                throw new InvalidOperationException("Cannot apply Kiếm proof loadout: " + applyReason);
            BindToSharedRig();
        }

        private void CreateTargets()
        {
            foreach (var slot in new[] { "inner_top", "outer_top", "head_hair", "main_weapon" })
            {
                var host = new GameObject("Map01A Kiếm proof " + slot);
                host.transform.SetParent(_root, false);
                var renderer = host.AddComponent<SpriteRenderer>();
                renderer.sortingOrder = SortForSlot(slot);
                _renderers.Add(slot, renderer);
                if (slot != "main_weapon") _skins.Add(slot, host.AddComponent<SpriteSkin>());
            }
        }

        private void BindToSharedRig()
        {
            _readySkins.Clear();
            Place("inner_top", new Vector2(0, 1.12f), Vector3.one * .88f, 0);
            Place("outer_top", new Vector2(0, 1.10f), Vector3.one * .90f, 0);
            Place("head_hair", new Vector2(.01f, 1.52f), Vector3.one * .76f, 0);

            var weapon = _renderers["main_weapon"].transform;
            weapon.SetParent(_rig.Bone(_gender + "_right-forearm-hand"), false);
            weapon.localPosition = new Vector3(.16f, -.04f, 0);
            weapon.localRotation = Quaternion.Euler(0, 0, -32f);
            weapon.localScale = Vector3.one * .82f;

            foreach (var pair in _skins)
            {
                var spriteBones = _renderers[pair.Key].sprite.GetBones();
                var transforms = BuildOrRefreshProxyBones(pair.Key, spriteBones);
                pair.Value.SetRootBone(transforms[0]);
                if (pair.Value.SetBoneTransforms(transforms) == SpriteSkinState.Ready)
                    _readySkins.Add(pair.Key);
                pair.Value.enabled = true;
            }
        }

        private Transform[] BuildOrRefreshProxyBones(string slot, SpriteBone[] spriteBones)
        {
            if (!_proxyBones.TryGetValue(slot, out var transforms) || transforms.Length != spriteBones.Length)
            {
                transforms = new Transform[spriteBones.Length];
                for (var i = 0; i < spriteBones.Length; i++)
                {
                    transforms[i] = new GameObject("Kiếm bind proxy " + slot + " " + spriteBones[i].name).transform;
                    var parent = spriteBones[i].parentId < 0
                        ? _renderers[slot].transform : transforms[spriteBones[i].parentId];
                    transforms[i].SetParent(parent, false);
                }
                _proxyBones[slot] = transforms;
            }
            for (var i = 0; i < spriteBones.Length; i++)
            {
                transforms[i].localPosition = spriteBones[i].position;
                transforms[i].localScale = Vector3.one;
                transforms[i].localRotation = spriteBones[i].rotation * SharedBone(spriteBones[i].name).localRotation;
            }
            return transforms;
        }

        private void Place(string slot, Vector2 position, Vector3 scale, float rotation)
        {
            var target = _renderers[slot].transform;
            target.SetParent(_root, false);
            target.localPosition = position;
            target.localRotation = Quaternion.Euler(0, 0, rotation);
            target.localScale = scale;
        }

        private Transform SharedBone(string spriteBone)
        {
            if (spriteBone == "torso") return _rig.Bone(_gender + "_torso-hips");
            if (spriteBone == "arm_near_upper") return _rig.Bone(_gender + "_right-upper-arm");
            if (spriteBone == "arm_near_forearm") return _rig.Bone(_gender + "_right-forearm-hand");
            return _rig.Bone(_gender + "_head");
        }

        private static string BoneForSlot(string slot)
        {
            if (slot == "head_hair") return "head";
            if (slot == "main_weapon") return "right-forearm-hand";
            return "torso-hips";
        }

        private static int SortForSlot(string slot)
        {
            if (slot == "inner_top") return 12;
            if (slot == "outer_top") return 13;
            if (slot == "main_weapon") return 14;
            return 15;
        }

        public void Dispose()
        {
            if (_library != null)
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(_library);
                else UnityEngine.Object.DestroyImmediate(_library);
            }
        }
    }
}
