using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    /// <summary>
    /// One immutable Potential geometry template. Class profiles bind only the
    /// icon/title/value overlays created by the owning screen.
    /// </summary>
    internal sealed class CharacterHubPotentialTopology : VisualElement
    {
        internal const float CanvasWidth = 600f;
        internal const float CanvasHeight = 520f;
        internal static readonly Vector2[] NodePositions =
        {
            new Vector2(238, 0), new Vector2(38, 168), new Vector2(438, 168),
            new Vector2(128, 348), new Vector2(368, 348)
        };

        internal static int PrebuiltNodeFrameCount => NodePositions.Length;
        internal static int PrebuiltValueFrameCount => NodePositions.Length;
        internal static int PrebuiltAddFrameCount => NodePositions.Length;
        internal static int PrebuiltAddGlyphCount => NodePositions.Length;
        internal static int PrebuiltMeridianAnchorCount => 5;

        internal CharacterHubPotentialTopology(Texture2D artwork)
        {
            name = "Map01A Potential Topology Base";
            pickingMode = PickingMode.Ignore;
            AddToClassList("lgo-potential-topology");
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.width = CanvasWidth;
            style.height = CanvasHeight;

            var artworkElement = new VisualElement
            {
                name = "Map01A Potential Topology Artwork",
                pickingMode = PickingMode.Ignore
            };
            artworkElement.style.position = Position.Absolute;
            artworkElement.style.left = 0;
            artworkElement.style.top = 0;
            artworkElement.style.right = 0;
            artworkElement.style.bottom = 0;
            artworkElement.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            artworkElement.style.backgroundImage = artwork == null
                ? StyleKeyword.None
                : new StyleBackground(artwork);
            Add(artworkElement);
        }
    }
}
