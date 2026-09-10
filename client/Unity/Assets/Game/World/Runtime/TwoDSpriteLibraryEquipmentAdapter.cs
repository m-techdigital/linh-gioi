using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace LinhGioi.World
{
    public static class TwoDSpriteLibraryEquipmentAdapter
    {
        private readonly struct PendingSprite
        {
            public PendingSprite(SpriteRenderer renderer, Sprite sprite)
            {
                Renderer = renderer;
                Sprite = sprite;
            }

            public SpriteRenderer Renderer { get; }
            public Sprite Sprite { get; }
        }

        public static bool RegisterItem(
            SpriteLibraryAsset library,
            TwoDEquipmentItemDefinition item,
            IReadOnlyDictionary<string, Sprite> spritesByComponent,
            out string reason)
        {
            if (library == null) throw new ArgumentNullException(nameof(library));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (spritesByComponent == null) throw new ArgumentNullException(nameof(spritesByComponent));
            if (item.FitStatus == TwoDEquipmentFitStatus.RedrawRequired)
                return Fail("ITEM_REDRAW_REQUIRED", out reason);
            if (item.FitStatus != TwoDEquipmentFitStatus.Approved)
                return Fail("ITEM_FIT_NOT_APPROVED", out reason);

            foreach (var attachment in item.Attachments)
            {
                if (!spritesByComponent.TryGetValue(attachment.ComponentId, out var sprite) || sprite == null)
                    return Fail("COMPONENT_SPRITE_MISSING:" + attachment.ComponentId, out reason);
                if (attachment.Mode == TwoDEquipmentAttachmentMode.Skinned && sprite.GetBones().Length == 0)
                    return Fail("SPRITE_SKINNING_MISSING:" + attachment.ComponentId, out reason);
            }

            foreach (var attachment in item.Attachments)
                library.AddCategoryLabel(spritesByComponent[attachment.ComponentId], attachment.ComponentId, item.ItemId);
            reason = "REGISTERED";
            return true;
        }

        public static bool TryApply(
            SpriteLibraryAsset library,
            TwoDEquipmentLoadout loadout,
            IReadOnlyDictionary<string, SpriteRenderer> renderersByComponent,
            out string reason)
        {
            if (library == null) throw new ArgumentNullException(nameof(library));
            if (loadout == null) throw new ArgumentNullException(nameof(loadout));
            if (renderersByComponent == null) throw new ArgumentNullException(nameof(renderersByComponent));

            var pending = new List<PendingSprite>();
            foreach (var slot in TwoDEquipmentCompatibilityCatalog.CanonicalSlots)
            {
                var item = loadout.GetEquippedItem(slot);
                if (item == null) continue;
                foreach (var attachment in item.Attachments)
                {
                    if (!renderersByComponent.TryGetValue(attachment.ComponentId, out var renderer) || renderer == null)
                        return Fail("COMPONENT_RENDERER_MISSING:" + attachment.ComponentId, out reason);
                    var sprite = library.GetSprite(attachment.ComponentId, item.ItemId);
                    if (sprite == null)
                        return Fail("LIBRARY_ENTRY_MISSING:" + attachment.ComponentId + ":" + item.ItemId, out reason);
                    pending.Add(new PendingSprite(renderer, sprite));
                }
            }

            foreach (var value in pending)
                value.Renderer.sprite = value.Sprite;
            reason = "APPLIED";
            return true;
        }

        private static bool Fail(string value, out string reason)
        {
            reason = value;
            return false;
        }
    }
}
