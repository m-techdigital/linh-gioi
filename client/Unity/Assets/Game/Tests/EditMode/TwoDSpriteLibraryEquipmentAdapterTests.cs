using System.Collections.Generic;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace LinhGioi.Tests.EditMode
{
    public sealed class TwoDSpriteLibraryEquipmentAdapterTests
    {
        private readonly List<Object> _owned = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var value in _owned)
                Object.DestroyImmediate(value);
            _owned.Clear();
        }

        [Test]
        public void RegistersAndAppliesMixedLevelItemsByComponentAndItemId()
        {
            var weapon = Item("kiem_lv001_weapon", "main_weapon", "weapon_r");
            var hair = Item("kiem_lv030_hair", "head_hair", "head");
            var inner = Item("kiem_lv010_inner", "inner_top", "torso");
            var outer = Item("kiem_lv020_outer", "outer_top", "torso");
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[] { weapon, hair, inner, outer });
            var loadout = new TwoDEquipmentLoadout(Profile(), "kiem");
            foreach (var item in new[] { weapon, hair, inner, outer })
                Assert.That(catalog.TryEquip(loadout, item.ItemId, 30, out _), Is.True);

            var library = Own(ScriptableObject.CreateInstance<SpriteLibraryAsset>());
            var targets = new Dictionary<string, SpriteRenderer>();
            foreach (var item in new[] { weapon, hair, inner, outer })
            {
                var sprite = CreateSprite();
                Assert.That(TwoDSpriteLibraryEquipmentAdapter.RegisterItem(
                    library, item, new Dictionary<string, Sprite> { [item.Attachments[0].ComponentId] = sprite }, out var reason), Is.True, reason);
                targets[item.Attachments[0].ComponentId] = Own(new GameObject(item.ItemId)).AddComponent<SpriteRenderer>();
            }

            Assert.That(TwoDSpriteLibraryEquipmentAdapter.TryApply(library, loadout, targets, out var applyReason), Is.True, applyReason);
            foreach (var item in new[] { weapon, hair, inner, outer })
            {
                var component = item.Attachments[0].ComponentId;
                Assert.That(targets[component].sprite, Is.SameAs(library.GetSprite(component, item.ItemId)));
            }
        }

        [Test]
        public void CandidateCannotBeRegisteredAsRuntimeSprite()
        {
            var item = Item("candidate", "outer_top", "torso", TwoDEquipmentFitStatus.Candidate);
            var component = item.Attachments[0].ComponentId;
            Assert.That(TwoDSpriteLibraryEquipmentAdapter.RegisterItem(
                Own(ScriptableObject.CreateInstance<SpriteLibraryAsset>()), item,
                new Dictionary<string, Sprite> { [component] = CreateSprite() }, out var reason), Is.False);
            Assert.That(reason, Is.EqualTo("ITEM_FIT_NOT_APPROVED"));
        }

        [Test]
        public void DraftFitRequiresExplicitPreviewOptInForEquipAndSpriteRegistration()
        {
            var item = Item("draft-fit", "outer_top", "torso", TwoDEquipmentFitStatus.DraftRuntimeFit);
            var profile = Profile();
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[] { item });
            var productionLoadout = new TwoDEquipmentLoadout(profile, "kiem");
            Assert.That(catalog.TryEquip(productionLoadout, item.ItemId, 30, out var productionReason), Is.False);
            Assert.That(productionReason, Is.EqualTo("ITEM_FIT_NOT_APPROVED"));

            var sprite = CreateSprite();
            var library = Own(ScriptableObject.CreateInstance<SpriteLibraryAsset>());
            Assert.That(TwoDSpriteLibraryEquipmentAdapter.RegisterItem(
                library, item, new Dictionary<string, Sprite> { ["draft-fit_part"] = sprite }, out _), Is.False);

            var previewLoadout = new TwoDEquipmentLoadout(profile, "kiem");
            Assert.That(catalog.TryEquipDraftFitPreview(previewLoadout, item.ItemId, 30, out var equipReason), Is.True, equipReason);
            Assert.That(TwoDSpriteLibraryEquipmentAdapter.RegisterDraftFitPreviewItem(
                library, item, new Dictionary<string, Sprite> { ["draft-fit_part"] = sprite }, out var registerReason), Is.True, registerReason);
        }

        [Test]
        public void DeformingAttachmentRequiresSpriteBoneWeights()
        {
            var item = Item("outer", "outer_top", "torso", attachmentMode: TwoDEquipmentAttachmentMode.Skinned);
            var component = item.Attachments[0].ComponentId;
            Assert.That(TwoDSpriteLibraryEquipmentAdapter.RegisterItem(
                Own(ScriptableObject.CreateInstance<SpriteLibraryAsset>()), item,
                new Dictionary<string, Sprite> { [component] = CreateSprite() }, out var reason), Is.False);
            Assert.That(reason, Is.EqualTo("SPRITE_SKINNING_MISSING:" + component));
        }

        [Test]
        public void MissingLibraryEntryLeavesEveryRendererUnchanged()
        {
            var weapon = Item("weapon", "main_weapon", "weapon_r");
            var hair = Item("hair", "head_hair", "head");
            var catalog = new TwoDEquipmentCompatibilityCatalog(new[] { weapon, hair });
            var loadout = new TwoDEquipmentLoadout(Profile(), "kiem");
            Assert.That(catalog.TryEquip(loadout, weapon.ItemId, 30, out _), Is.True);
            Assert.That(catalog.TryEquip(loadout, hair.ItemId, 30, out _), Is.True);

            var library = Own(ScriptableObject.CreateInstance<SpriteLibraryAsset>());
            var weaponSprite = CreateSprite();
            var original = CreateSprite();
            Assert.That(TwoDSpriteLibraryEquipmentAdapter.RegisterItem(library, weapon,
                new Dictionary<string, Sprite> { [weapon.Attachments[0].ComponentId] = weaponSprite }, out _), Is.True);
            var weaponRenderer = Own(new GameObject("weapon")).AddComponent<SpriteRenderer>();
            weaponRenderer.sprite = original;
            var targets = new Dictionary<string, SpriteRenderer>
            {
                [weapon.Attachments[0].ComponentId] = weaponRenderer,
                [hair.Attachments[0].ComponentId] = Own(new GameObject("hair")).AddComponent<SpriteRenderer>()
            };

            Assert.That(TwoDSpriteLibraryEquipmentAdapter.TryApply(library, loadout, targets, out var reason), Is.False);
            Assert.That(reason, Is.EqualTo("LIBRARY_ENTRY_MISSING:" + hair.Attachments[0].ComponentId + ":" + hair.ItemId));
            Assert.That(weaponRenderer.sprite, Is.SameAs(original));
        }

        private TwoDEquipmentItemDefinition Item(string id, string slot, string bone,
            TwoDEquipmentFitStatus status = TwoDEquipmentFitStatus.Approved,
            TwoDEquipmentAttachmentMode attachmentMode = TwoDEquipmentAttachmentMode.Rigid)
        {
            return new TwoDEquipmentItemDefinition(id, slot, 1, "lgo_humanoid_2d_v1",
                new[] { "common_male_v1" }, new[] { "kiem" }, status,
                new[] { new TwoDEquipmentAttachmentDefinition(id + "_part", bone, 100, attachmentMode) },
                new string[0], new string[0]);
        }

        private static TwoDCharacterFitProfile Profile()
        {
            return new TwoDCharacterFitProfile("common_male_v1", "lgo_humanoid_2d_v1",
                new[] { "head", "torso", "weapon_r" });
        }

        private Sprite CreateSprite()
        {
            var texture = Own(new Texture2D(2, 2));
            return Own(Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.one * 0.5f, 2));
        }

        private T Own<T>(T value) where T : Object
        {
            _owned.Add(value);
            return value;
        }
    }
}
