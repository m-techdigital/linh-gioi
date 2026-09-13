using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _menuOverlay;
        private bool _menuOpen;

        private void BuildMenu()
        {
            _menuOverlay = new VisualElement { name = "Map01A Menu Overlay" };
            _menuOverlay.style.position = Position.Absolute;
            _menuOverlay.style.left = 0;
            _menuOverlay.style.right = 0;
            _menuOverlay.style.top = 0;
            _menuOverlay.style.bottom = 0;
            _menuOverlay.style.backgroundColor = new Color(.006f, .020f, .040f, .28f);
            _menuOverlay.style.justifyContent = Justify.FlexEnd;
            _menuOverlay.style.alignItems = Align.FlexEnd;
            _menuOverlay.style.paddingRight = 18;
            _menuOverlay.style.paddingBottom = 94;

            var panel = new VisualElement { name = "Map01A Menu Panel" };
            panel.style.width = 370;
            ApplyLgoModalShell(panel, 16);
            ApplyLgoLayeredFrame(panel);
            _menuOverlay.Add(panel);

            var header = new VisualElement { name = "Map01A Menu Header" };
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems = Align.Center;
            var title = LgoTitleLabel("MENU", 21);
            title.style.flexGrow = 1;
            header.Add(title);
            var close = new Button(CloseMenu) { name = "Map01A Menu Close", text = "×" };
            ApplyLgoModalCloseButton(close, _touch);
            header.Add(close);
            panel.Add(header);

            var location = LgoSubtitleLabel("Cổng Đông Lâm · truy cập nhanh", 13);
            location.style.marginTop = 2;
            location.style.marginBottom = 12;
            panel.Add(location);
            panel.Add(LgoDivider("Map01A Menu Divider"));

            var navigation = new VisualElement { name = "Map01A Menu Navigation" };
            navigation.style.flexDirection = FlexDirection.Row;
            navigation.style.flexWrap = Wrap.Wrap;
            navigation.style.marginTop = 12;
            navigation.Add(MenuAction("Map01A Menu Character Action", "Nhân vật", "character-info"));
            navigation.Add(MenuAction("Map01A Menu Bag Action", "Rương đồ", "bag"));
            navigation.Add(MenuAction("Map01A Menu Skills Action", "Kỹ năng", "skills"));
            navigation.Add(MenuAction("Map01A Menu Potential Action", "Tiềm năng", "potential"));
            navigation.Add(MenuAction("Map01A Menu Spirit Pet Action", "Linh thú", "spirit-pet"));
            panel.Add(navigation);

            var help = new VisualElement { name = "Map01A Menu Help" };
            ApplyLgoStatusCard(help, 10, 8);
            help.style.marginTop = 6;
            help.Add(LgoLabel("ĐIỀU KHIỂN", 12, UiGold, true));
            var helpText = LgoSubtitleLabel("Di chuyển: Shift  ·  Nhảy: W  ·  Đánh: Z  ·  Tương tác: E", 12);
            helpText.style.whiteSpace = WhiteSpace.Normal;
            helpText.style.marginTop = 4;
            help.Add(helpText);
            panel.Add(help);

            var resume = new Button(CloseMenu) { name = "Map01A Menu Resume", text = "Tiếp tục hành trình" };
            ApplyLgoMenuAction(resume, true);
            resume.style.flexBasis = StyleKeyword.Auto;
            resume.style.width = Length.Percent(100);
            resume.style.height = 48;
            resume.style.marginTop = 10;
            panel.Add(resume);

            _root.Add(_menuOverlay);
            UpdateMenu();
        }

        private Button MenuAction(string name, string text, string destination)
        {
            var button = new Button(() => OpenMenuDestination(destination)) { name = name, text = text };
            ApplyLgoMenuAction(button);
            return button;
        }

        private void OpenMenuDestination(string destination)
        {
            CloseMenu();
            OpenInventoryReviewMode(destination);
        }

        private void ToggleMenu()
        {
            _menuOpen = !_menuOpen;
            UpdateMenu();
        }

        private void CloseMenu()
        {
            _menuOpen = false;
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            if (_menuOverlay != null)
                _menuOverlay.style.display = _menuOpen ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
