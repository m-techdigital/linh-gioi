using System;
using System.Text;
using UnityEngine;

namespace LinhGioi.World
{
    public static class TwoDClassSliceCatalog
    {
        private const string VoLv1ResourcePath = "LGOClasses/VoLv1ClassSlice";

        public static VoLv1ClassSliceSource LoadVoLv1ClassSlice()
        {
            var asset = Resources.Load<TextAsset>(VoLv1ResourcePath);
            if (asset == null || string.IsNullOrEmpty(asset.text)) return null;
            return JsonUtility.FromJson<VoLv1ClassSliceSource>(asset.text);
        }

        public static string LoadVoLv1ClassSliceSnapshot()
        {
            var asset = Resources.Load<TextAsset>(VoLv1ResourcePath);
            if (asset == null) return "VoLv1ClassSlice: resource=" + VoLv1ResourcePath + " | missing";

            var text = asset.text ?? string.Empty;
            var source = LoadVoLv1ClassSlice();
            var builder = new StringBuilder("VoLv1ClassSlice: resource=").Append(VoLv1ResourcePath);
            builder.Append(" | bytes=").Append(text.Length);
            builder.Append(" | classId=").Append(source != null ? source.classId : "missing");
            builder.Append(" | displayName=").Append(source != null ? source.displayName : "missing");
            builder.Append(" | bases=").Append(source != null && source.baseIds != null ? string.Join(",", source.baseIds) : "missing");
            builder.Append(" | slots=").Append(source != null && source.slots != null ? source.slots.Length : 0);
            if (source != null && source.slots != null)
            {
                for (var i = 0; i < source.slots.Length; i++)
                    builder.Append(" | ").Append(source.slots[i].slot).Append(':').Append(source.slots[i].moduleId).Append('@').Append(source.slots[i].anchor);
                builder.Append(" | anchorSet=").Append(BuildAnchorSet(source.slots));
                for (var i = 0; i < source.slots.Length; i++)
                    builder.Append(" | compat=").Append(source.slots[i].fitProfile).Append("->").Append(source.slots[i].slot);
            }
            builder.Append(" | motions=");
            if (source != null && source.motions != null)
            {
                for (var i = 0; i < source.motions.Length; i++)
                {
                    if (i > 0) builder.Append(',');
                    builder.Append(source.motions[i].state).Append(':').Append(source.motions[i].pose);
                }
            }
            else
            {
                builder.Append("missing");
            }
            builder.Append(" | skillId=").Append(source != null && source.skill != null ? source.skill.id : "missing");
            builder.Append(" | skillAnchor=").Append(source != null && source.skill != null ? source.skill.anchor : "missing");
            builder.Append(" | hitTarget=").Append(source != null && source.skill != null ? source.skill.hitTarget : "missing");
            builder.Append(" | safe-runtime-resource=").Append(ContainsToken(text, "safe-runtime-resource"));
            builder.Append(" | safe-no-source-image=").Append(ContainsToken(text, "safe-no-source-image"));
            builder.Append(" | safe-no-3d=").Append(ContainsToken(text, "safe-no-3d"));
            builder.Append(" | safe-local-no-backend=").Append(ContainsToken(text, "safe-local-no-backend"));
            builder.Append(" | source=runtime-authored-json");
            builder.Append(" | runtimeArtPolicy=replace-primitive-with-approved-spritesheet");
            return builder.ToString();
        }

        private static string BuildAnchorSet(VoLv1ClassSlotEntry[] slots)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < slots.Length; i++)
            {
                var anchors = (slots[i].anchor ?? string.Empty).Split(',');
                for (var j = 0; j < anchors.Length; j++)
                {
                    var anchor = anchors[j].Trim();
                    if (string.IsNullOrEmpty(anchor) || ContainsAnchor(builder, anchor)) continue;
                    if (builder.Length > 0) builder.Append(',');
                    builder.Append(anchor);
                }
            }
            return builder.ToString();
        }

        private static bool ContainsAnchor(StringBuilder builder, string anchor)
        {
            var text = builder.ToString();
            return text == anchor || text.StartsWith(anchor + ",", StringComparison.Ordinal) || text.EndsWith("," + anchor, StringComparison.Ordinal) || text.IndexOf("," + anchor + ",", StringComparison.Ordinal) >= 0;
        }

        private static bool ContainsToken(string text, string token)
        {
            return text.IndexOf(token, StringComparison.Ordinal) >= 0;
        }
    }

    [Serializable]
    public sealed class VoLv1ClassSliceSource
    {
        public string id;
        public string classId;
        public string displayName;
        public string[] baseIds;
        public string usage;
        public string[] safety;
        public VoLv1ClassSlotEntry[] slots;
        public VoLv1ClassMotionEntry[] motions;
        public VoLv1SkillEntry skill;
    }

    [Serializable]
    public sealed class VoLv1ClassSlotEntry
    {
        public string slot;
        public string moduleId;
        public string anchor;
        public string fitProfile;
        public int sortBand;
    }

    [Serializable]
    public sealed class VoLv1ClassMotionEntry
    {
        public string state;
        public string pose;
        public string rootMotion;
        public string anchorPolicy;
    }

    [Serializable]
    public sealed class VoLv1SkillEntry
    {
        public string id;
        public string displayName;
        public string input;
        public string anchor;
        public string hitTarget;
        public string vfxCue;
    }
}
