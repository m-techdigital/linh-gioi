using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D;

namespace LinhGioi.Foundation.Editor
{
    public static class KiemSpriteSkinAssetAuthoring
    {
        private const string AssetRoot = "Assets/Game/World/Runtime/Resources/LGOClasses/KiemMixedLoadoutFitPreview";
        private const string AtlasPath = AssetRoot + "/kiem-mixed-loadout-fit-atlas.png";
        private const string ManifestPath = AssetRoot + "/manifest.json";

        [Serializable]
        private sealed class ProofManifest
        {
            public int pixelsPerUnit;
            public ProofItem[] items;
        }

        [Serializable]
        private sealed class ProofItem
        {
            public string itemId;
            public string slotId;
            public string attachmentMode;
            public int[] atlasRect;
        }

        public static void AuthorProofPack()
        {
            var manifest = JsonUtility.FromJson<ProofManifest>(File.ReadAllText(ManifestPath));
            if (manifest == null || manifest.items == null || manifest.items.Length != 8)
                throw new InvalidOperationException("Expected eight Kiếm proof items");
            if (manifest.pixelsPerUnit != 208)
                throw new InvalidOperationException("Kiếm proof atlas must use 208 PPU");

            ConfigureAtlas(manifest);
            var skinned = AuthorSkinning(manifest);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log($"LGO_KIEM_SPRITESKIN_AUTHORING_PASS atlas=1 sprites={manifest.items.Length} skinned={skinned} ppu={manifest.pixelsPerUnit} max=512");
        }

        private static void ConfigureAtlas(ProofManifest manifest)
        {
            var importer = AssetImporter.GetAtPath(AtlasPath) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("TextureImporter missing: " + AtlasPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = manifest.pixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 512;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();

            var provider = Provider(importer);
            var existingIds = provider.GetSpriteRects().ToDictionary(rect => rect.name, rect => rect.spriteID);
            var rects = manifest.items.Select(item =>
            {
                if (item.atlasRect == null || item.atlasRect.Length != 4)
                    throw new InvalidOperationException("Invalid atlasRect: " + item.itemId);
                return new SpriteRect
                {
                    name = item.itemId,
                    spriteID = existingIds.TryGetValue(item.itemId, out var id) ? id : GUID.Generate(),
                    rect = new Rect(item.atlasRect[0], item.atlasRect[1], item.atlasRect[2], item.atlasRect[3]),
                    alignment = SpriteAlignment.Custom,
                    pivot = new Vector2(.5f, .5f)
                };
            }).ToArray();
            provider.SetSpriteRects(rects);
            var nameProvider = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            if (nameProvider == null) throw new InvalidOperationException("Sprite name provider missing: " + AtlasPath);
            nameProvider.SetNameFileIdPairs(rects.Select(rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)));
            provider.Apply();
            importer.SaveAndReimport();
        }

        private static int AuthorSkinning(ProofManifest manifest)
        {
            var importer = AssetImporter.GetAtPath(AtlasPath);
            var provider = Provider(importer);
            var rects = provider.GetSpriteRects().ToDictionary(rect => rect.name, StringComparer.Ordinal);
            var boneProvider = provider.GetDataProvider<ISpriteBoneDataProvider>();
            var meshProvider = provider.GetDataProvider<ISpriteMeshDataProvider>();
            if (boneProvider == null || meshProvider == null)
                throw new InvalidOperationException("Skinning data providers missing: " + AtlasPath);

            var skinned = 0;
            foreach (var item in manifest.items.Where(item => item.attachmentMode == "Skinned"))
            {
                if (!rects.TryGetValue(item.itemId, out var rect))
                    throw new InvalidOperationException("Sprite rect missing: " + item.itemId);
                var width = rect.rect.width;
                var height = rect.rect.height;
                var profile = item.slotId == "head_hair" ? "hair" : item.slotId == "outer_top" ? "outer" : "inner";
                boneProvider.SetBones(rect.spriteID, BuildBones(profile, width, height));
                var vertices = meshProvider.GetVertices(rect.spriteID);
                if (vertices.Length < 3)
                {
                    vertices = new[]
                    {
                        new Vertex2DMetaData { position = new Vector2(0, 0) },
                        new Vertex2DMetaData { position = new Vector2(width, 0) },
                        new Vertex2DMetaData { position = new Vector2(width, height) },
                        new Vertex2DMetaData { position = new Vector2(0, height) }
                    };
                    meshProvider.SetIndices(rect.spriteID, new[] { 0, 2, 1, 0, 3, 2 });
                    meshProvider.SetEdges(rect.spriteID, new[]
                    {
                        new Vector2Int(0, 1), new Vector2Int(1, 2),
                        new Vector2Int(2, 3), new Vector2Int(3, 0)
                    });
                }
                for (var i = 0; i < vertices.Length; i++)
                {
                    var x = width <= 0 ? .5f : vertices[i].position.x / width;
                    var y = height <= 0 ? .5f : vertices[i].position.y / height;
                    vertices[i].boneWeight = Weight(profile, x, y);
                }
                meshProvider.SetVertices(rect.spriteID, vertices);
                skinned++;
            }
            provider.Apply();
            importer.SaveAndReimport();
            return skinned;
        }

        private static ISpriteEditorDataProvider Provider(UnityEngine.Object importer)
        {
            var factories = new SpriteDataProviderFactories();
            factories.Init();
            var provider = factories.GetSpriteEditorDataProviderFromObject(importer);
            if (provider == null) throw new InvalidOperationException("Sprite data provider missing: " + AtlasPath);
            provider.InitSpriteEditorDataProvider();
            return provider;
        }

        private static List<SpriteBone> BuildBones(string profile, float width, float height)
        {
            if (profile == "inner")
                return new List<SpriteBone> { Bone("torso", width * .5f, height * .5f, height * .45f, -1) };
            if (profile == "outer")
                return new List<SpriteBone>
                {
                    Bone("torso", width * .65f, height * .55f, height * .4f, -1),
                    Bone("arm_near_upper", -width * .2f, height * .12f, height * .35f, 0),
                    Bone("arm_near_forearm", 0, -height * .3f, height * .3f, 1)
                };
            return new List<SpriteBone>
            {
                Bone("head", width * .72f, height * .62f, height * .25f, -1),
                Bone("hair_mid", -width * .28f, -height * .08f, height * .32f, 0),
                Bone("hair_tip", -width * .28f, -height * .24f, height * .3f, 1)
            };
        }

        private static SpriteBone Bone(string name, float x, float y, float length, int parent)
        {
            return new SpriteBone
            {
                name = name,
                position = new Vector3(x, y, 0),
                rotation = Quaternion.identity,
                length = length,
                parentId = parent
            };
        }

        private static BoneWeight Weight(string profile, float x, float y)
        {
            var index = 0;
            if (profile == "hair") index = x < .34f ? 2 : x < .68f ? 1 : 0;
            else if (profile == "outer") index = x < .55f && y < .5f ? 2 : x < .62f ? 1 : 0;
            return new BoneWeight { boneIndex0 = index, weight0 = 1 };
        }
    }
}
