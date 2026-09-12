using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _characterSelectOverlay;
        private bool _characterSelectOpen;

        private void BuildCharacterSelect()
        {
            _characterSelectOverlay = new VisualElement { name = "Map01A Character Select Overlay" };
            _characterSelectOverlay.style.position = Position.Absolute;
            _characterSelectOverlay.style.left = 0;
            _characterSelectOverlay.style.right = 0;
            _characterSelectOverlay.style.top = 0;
            _characterSelectOverlay.style.bottom = 0;
            _characterSelectOverlay.style.justifyContent = Justify.Center;
            _characterSelectOverlay.style.alignItems = Align.Center;
            _characterSelectOverlay.style.backgroundColor = new Color(.006f, .020f, .040f, .82f);

            var panel = new VisualElement { name = "Map01A Character Select Panel" };
            panel.style.width = Length.Percent(62);
            panel.style.minWidth = 760;
            panel.style.maxWidth = 980;
            ApplyLgoModalShell(panel, 20);
            _characterSelectOverlay.Add(panel);

            var title = LgoLabel("Chọn Nhân Vật", 30, UiGold, true);
            title.name = "Map01A Character Select Title";
            title.style.unityTextAlign = TextAnchor.MiddleCenter;
            panel.Add(title);
            var scope = LgoLabel("review local: chọn class để kiểm tra UI/pose hiện có; chưa mở tạo nhân vật hoặc đổi tài khoản thật.", 14, UiSubText);
            scope.name = "Map01A Character Select Scope";
            scope.style.unityTextAlign = TextAnchor.MiddleCenter;
            scope.style.marginBottom = 14;
            panel.Add(scope);

            var cards = new VisualElement { name = "Map01A Character Select Cards" };
            cards.style.flexDirection = FlexDirection.Row;
            cards.style.flexWrap = Wrap.Wrap;
            cards.style.justifyContent = Justify.Center;
            panel.Add(cards);
            foreach (var label in new[] { "Võ", "Kiếm", "Pháp", "Cơ", "Linh" })
                cards.Add(MakeCharacterCard(label));

            var actions = new VisualElement { name = "Map01A Character Select Actions" };
            actions.style.flexDirection = FlexDirection.Row;
            actions.style.justifyContent = Justify.Center;
            actions.style.marginTop = 16;
            var close = new Button(CloseCharacterSelect) { name = "Map01A Character Select Close", text = "Vào Cổng Đông Lâm" };
            close.style.minWidth = 220;
            ApplyLgoButton(close, true);
            actions.Add(close);
            panel.Add(actions);

            _root.Add(_characterSelectOverlay);
            _characterSelectOpen = ShouldShowCharacterSelectOnLaunch();
            UpdateCharacterSelectScreen();
        }

        private bool ShouldShowCharacterSelectOnLaunch()
        {
            return System.Array.IndexOf(System.Environment.GetCommandLineArgs(), "--lgo-map01a-character-select-capture") >= 0;
        }

        private Button MakeCharacterCard(string label)
        {
            var card = new Button(() => SelectCharacterCard(label)) { name = "Map01A Character Card " + label, text = label + "\nLv review · " + ClassRole(label) };
            card.style.flexGrow = 0;
            card.style.flexBasis = new Length(30.5f, LengthUnit.Percent);
            card.style.height = 116;
            card.style.marginRight = 8;
            card.style.marginBottom = 8;
            card.style.fontSize = 19;
            card.style.whiteSpace = WhiteSpace.Normal;
            card.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoButton(card);
            return card;
        }

        private static string ClassRole(string label)
        {
            switch (label)
            {
                case "Võ": return "quyền cước";
                case "Kiếm": return "kiếm khí";
                case "Pháp": return "pháp thuật";
                case "Cơ": return "cơ quan";
                default: return "linh thú";
            }
        }

        private void SelectCharacterCard(string label)
        {
            if (_scene.CanCycleSourcePoseClass)
            {
                for (var i = 0; i < 5 && _scene.ActiveEquipmentClassLabel != label; i++)
                    _scene.CycleSourcePoseClass();
            }
        }

        private void OpenCharacterSelect()
        {
            if (_scene.InventoryOpen) _scene.ToggleInventory();
            if (_scene.DialogueOpen) _scene.CloseNpcDialogue();
            _characterSelectOpen = true;
            UpdateCharacterSelectScreen();
        }

        private void CloseCharacterSelect()
        {
            _characterSelectOpen = false;
            UpdateCharacterSelectScreen();
        }

        private void UpdateCharacterSelectScreen()
        {
            if (_characterSelectOverlay == null) return;
            _characterSelectOverlay.style.display = _characterSelectOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateHudShellVisibility();
        }

        private void UpdateHudShellVisibility()
        {
            var hide = _entryOpen || _characterSelectOpen;
            if (_safe != null) _safe.style.display = hide ? DisplayStyle.None : DisplayStyle.Flex;
            if (_marker != null) _marker.style.display = hide ? DisplayStyle.None : _marker.style.display.value;
        }
    }
}
