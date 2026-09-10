using System;
using System.Text;
using UnityEngine;

namespace LinhGioi.World
{
    public static class TwoDPaperDollAtlasCatalog
    {
        private const string VoLv1ResourcePath = "LGOClasses/VoLv1PaperDollAtlas";

        public static VoLv1PaperDollAtlasSource LoadVoLv1PaperDollAtlas()
        {
            var asset = Resources.Load<TextAsset>(VoLv1ResourcePath);
            if (asset == null || string.IsNullOrEmpty(asset.text)) return null;
            return JsonUtility.FromJson<VoLv1PaperDollAtlasSource>(asset.text);
        }

        public static string LoadVoLv1PaperDollAtlasSnapshot()
        {
            var asset = Resources.Load<TextAsset>(VoLv1ResourcePath);
            if (asset == null) return "VoLv1PaperDollAtlas: resource=" + VoLv1ResourcePath + " | missing";

            var text = asset.text ?? string.Empty;
            var source = LoadVoLv1PaperDollAtlas();
            var builder = new StringBuilder("VoLv1PaperDollAtlas: resource=").Append(VoLv1ResourcePath);
            builder.Append(" | bytes=").Append(text.Length);
            builder.Append(" | classId=").Append(source != null ? source.classId : "missing");
            builder.Append(" | displayName=").Append(source != null ? source.displayName : "missing");
            builder.Append(" | usage=").Append(source != null ? source.usage : "missing");
            builder.Append(" | parts=").Append(source != null && source.parts != null ? source.parts.Length : 0);
            if (source != null && source.parts != null)
            {
                for (var i = 0; i < source.parts.Length; i++)
                {
                    var part = source.parts[i];
                    builder.Append(" | part=").Append(part.id)
                        .Append(" cell=").Append(string.IsNullOrEmpty(part.cell) ? "runtime-generated-atlas-cell" : part.cell)
                        .Append(" slot=").Append(part.slot)
                        .Append(" anchor=").Append(part.anchor)
                        .Append(" shape=").Append(string.IsNullOrEmpty(part.shape) ? "rect" : part.shape);
                }
            }
            if (source != null && source.productionAtlas != null)
            {
                builder.Append(" | productionAtlas=").Append(source.productionAtlas.atlasId);
                builder.Append(" | importMode=").Append(source.productionAtlas.importMode);
                builder.Append(" | texturePolicy=").Append(source.productionAtlas.texturePolicy);
                builder.Append(" | pixelsPerUnit=").Append(source.productionAtlas.pixelsPerUnit);
                builder.Append(" | pivotPolicy=").Append(source.productionAtlas.pivotPolicy);
                AppendTokens(builder, "requiredCell", source.productionAtlas.requiredCells);
                AppendTokens(builder, "runtimeLayer", source.productionAtlas.runtimeLayers);
                AppendTokens(builder, "rigJoint", source.productionAtlas.rigJoints);
                AppendTokens(builder, "motionClip", source.productionAtlas.motionClips);
                AppendTokens(builder, "replacementGate", source.productionAtlas.replacementGate);
            }
            builder.Append(" | poseOffsets=").Append(source != null && source.poseOffsets != null ? source.poseOffsets.Length : 0);
            if (source != null && source.poseOffsets != null)
            {
                for (var i = 0; i < source.poseOffsets.Length; i++)
                {
                    var offset = source.poseOffsets[i];
                    builder.Append(" | pose=").Append(offset.pose)
                        .Append(" part=").Append(offset.partId)
                        .Append(" dx=").Append(offset.dx.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))
                        .Append(" dy=").Append(offset.dy.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            builder.Append(" | skillCues=").Append(source != null && source.skillCues != null ? source.skillCues.Length : 0);
            if (source != null && source.skillCues != null)
            {
                for (var i = 0; i < source.skillCues.Length; i++)
                {
                    var cue = source.skillCues[i];
                    builder.Append(" | skillCue=").Append(cue.id)
                        .Append(" cell=").Append(string.IsNullOrEmpty(cue.cell) ? "runtime-generated-atlas-cell" : cue.cell)
                        .Append(" slot=").Append(cue.slot)
                        .Append(" anchor=").Append(cue.anchor)
                        .Append(" shape=").Append(string.IsNullOrEmpty(cue.shape) ? "rect" : cue.shape);
                }
            }
            builder.Append(" | cellSource=runtime-generated-atlas-cell");
            builder.Append(" | safe-runtime-resource=").Append(ContainsToken(text, "safe-runtime-resource"));
            builder.Append(" | safe-no-source-image=").Append(ContainsToken(text, "safe-no-source-image"));
            builder.Append(" | safe-no-3d=").Append(ContainsToken(text, "safe-no-3d"));
            builder.Append(" | safe-local-no-backend=").Append(ContainsToken(text, "safe-local-no-backend"));
            builder.Append(" | safe-runtime-motion=").Append(ContainsToken(text, "safe-runtime-motion"));
            return builder.ToString();
        }

        private static void AppendTokens(StringBuilder builder, string label, string[] values)
        {
            if (values == null) return;
            for (var i = 0; i < values.Length; i++)
                builder.Append(" | ").Append(label).Append("=").Append(values[i]);
        }

        private static bool ContainsToken(string text, string token)
        {
            return text.IndexOf(token, StringComparison.Ordinal) >= 0;
        }
    }

    [Serializable]
    public sealed class VoLv1PaperDollAtlasSource
    {
        public string id;
        public string classId;
        public string displayName;
        public string usage;
        public string[] safety;
        public VoLv1ProductionAtlasContract productionAtlas;
        public VoLv1PaperDollAtlasPart[] parts;
        public VoLv1PaperDollPoseOffset[] poseOffsets;
        public VoLv1PaperDollAtlasPart[] skillCues;
    }

    [Serializable]
    public sealed class VoLv1ProductionAtlasContract
    {
        public string atlasId;
        public string importMode;
        public string texturePolicy;
        public int pixelsPerUnit;
        public string pivotPolicy;
        public string[] requiredCells;
        public string[] runtimeLayers;
        public string[] rigJoints;
        public string[] motionClips;
        public string[] replacementGate;
    }

    [Serializable]
    public sealed class VoLv1PaperDollPoseOffset
    {
        public string pose;
        public string partId;
        public float dx;
        public float dy;
        public float sx = 1f;
        public float sy = 1f;
    }

    [Serializable]
    public sealed class VoLv1PaperDollAtlasPart
    {
        public string id;
        public string slot;
        public string anchor;
        public string cell;
        public string shape;
        public float x;
        public float y;
        public float w;
        public float h;
        public float r;
        public float g;
        public float b;
        public float a;
        public int sortOffset;
    }
}
