using UnityEngine;

namespace LinhGioi.UI
{
    internal readonly struct RuntimeGameplayHudLayout
    {
        internal readonly Rect PlayerStatus;
        internal readonly Rect RightInfo;
        internal readonly Rect Combat;
        internal readonly Rect Context;
        internal readonly Rect SecondaryNav;
        internal readonly Rect TouchPad;
        internal readonly Rect Dialogue;

        private RuntimeGameplayHudLayout(Rect playerStatus, Rect rightInfo, Rect combat,
            Rect context, Rect secondaryNav, Rect touchPad, Rect dialogue)
        {
            PlayerStatus = playerStatus;
            RightInfo = rightInfo;
            Combat = combat;
            Context = context;
            SecondaryNav = secondaryNav;
            TouchPad = touchPad;
            Dialogue = dialogue;
        }
        internal static RuntimeGameplayHudLayout Calculate(Rect safeRect, RuntimeUiLayoutProfile profile)
        {
            var edge = Mathf.Max(0f, profile.GameplayHudEdgeInset);
            var gap = Mathf.Max(0f, profile.GameplayHudDockGap);
            var maxWidth = Mathf.Max(0f, safeRect.width - edge * 2f);
            var maxHeight = Mathf.Max(0f, safeRect.height - edge * 2f);

            var playerWidth = Mathf.Min(profile.GameplayHudPlayerStatusWidth, maxWidth);
            var playerHeight = Mathf.Min(profile.GameplayHudPlayerStatusHeight, maxHeight);
            var playerStatus = new Rect(safeRect.xMin + edge, safeRect.yMin + edge,
                playerWidth, playerHeight);

            var combatWidth = Mathf.Min(profile.GameplayHudCombatWidth, maxWidth * .48f);
            var combatHeight = Mathf.Min(profile.GameplayHudCombatHeight, maxHeight);
            var combat = new Rect(safeRect.xMax - edge - combatWidth,
                safeRect.yMax - edge - combatHeight, combatWidth, combatHeight);

            var navWidth = Mathf.Min(profile.GameplayHudSecondaryNavWidth, combatWidth);
            var navHeight = Mathf.Min(profile.GameplayHudSecondaryNavHeight, maxHeight);
            var contextWidth = Mathf.Min(profile.GameplayHudContextWidth,
                Mathf.Max(0f, combatWidth - navWidth - gap));
            var contextHeight = Mathf.Min(profile.GameplayHudContextHeight, maxHeight);
            var actionRowHeight = Mathf.Max(navHeight, contextHeight);
            var actionRowY = Mathf.Max(safeRect.yMin + edge,
                combat.yMin - gap - actionRowHeight);
            var context = new Rect(combat.xMin, actionRowY, contextWidth, contextHeight);
            var secondaryNav = new Rect(combat.xMax - navWidth, actionRowY, navWidth, navHeight);

            var rightWidth = Mathf.Min(profile.WorldRightColumnWidth, maxWidth);
            var rightTop = safeRect.yMin + edge;
            var rightAvailableHeight = Mathf.Max(0f, actionRowY - gap - rightTop);
            var rightHeight = Mathf.Min(profile.GameplayHudRightInfoPreferredHeight, rightAvailableHeight);
            var rightInfo = new Rect(safeRect.xMax - edge - rightWidth, rightTop,
                rightWidth, rightHeight);

            var touchSize = Mathf.Min(profile.GameplayHudTouchPadSize,
                Mathf.Min(maxWidth * .35f, maxHeight));
            var touchPad = touchSize <= 0f
                ? new Rect(safeRect.xMin + edge, safeRect.yMax - edge, 0f, 0f)
                : new Rect(safeRect.xMin + edge, safeRect.yMax - edge - touchSize,
                    touchSize, touchSize);

            var dialogueWidth = Mathf.Min(profile.DialogueOverlayWidth, maxWidth);
            var dialogueHeight = Mathf.Min(profile.DialoguePanelMaxHeight, maxHeight);
            var dialogue = new Rect(
                safeRect.xMin + Mathf.Max(edge, (safeRect.width - dialogueWidth) * .5f),
                safeRect.yMax - edge - dialogueHeight,
                dialogueWidth,
                dialogueHeight);

            return new RuntimeGameplayHudLayout(playerStatus, rightInfo, combat, context,
                secondaryNav, touchPad, dialogue);
        }
    }
}
