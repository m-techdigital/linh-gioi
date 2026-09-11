using UnityEngine;

namespace LinhGioi.World
{
    public static class TwoDPaperDollPoseSampler
    {
        public static float Sample(string motion, float phase, float progress)
        {
            switch (motion)
            {
                case "walk": return .72f * Mathf.Sin(phase * Mathf.PI * 2f);
                case "run": return .90f * Mathf.Sin(phase * Mathf.PI * 2f);
                case "jump": return .78f * Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
                case "basic_attack": return .65f * Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
                case "skill": return .62f * Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
                default: return 0f;
            }
        }
    }
}
