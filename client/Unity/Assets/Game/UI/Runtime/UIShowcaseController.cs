using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class UIShowcaseController : MonoBehaviour
    {
        [SerializeField] private ThemeTokens theme;

        public void Configure(ThemeTokens value) => theme = value;

        private void OnEnable()
        {
            if (theme == null)
            {
                Debug.LogError("[LinhGioi] UI showcase is missing ThemeTokens.");
                return;
            }
            UIThemeContext.Set(theme);
            var document = GetComponent<UIDocument>();
            var root = document.rootVisualElement;
            root.Clear();
            root.style.backgroundColor = theme.bg;
            root.style.paddingLeft = 24;
            root.style.paddingRight = 24;
            root.style.paddingTop = 24;
            root.style.paddingBottom = 24;

            var safeRoot = new SafeAreaRoot();
            safeRoot.style.flexGrow = 1;
            safeRoot.ApplySafeArea(Screen.safeArea, new Vector2(Screen.width, Screen.height));
            root.Add(safeRoot);

            var title = new Label("LINH GIỚI — UI FOUNDATION");
            title.style.color = theme.text;
            title.style.fontSize = 24;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 16;
            safeRoot.Add(title);

            var panel = new BasePanel(theme);
            panel.style.maxWidth = 760;
            safeRoot.Add(panel);

            var identity = new VisualElement();
            identity.style.flexDirection = FlexDirection.Row;
            identity.style.alignItems = Align.Center;
            identity.Add(new AvatarView(theme, "LG"));
            var nameplate = new Nameplate(theme, "LinhGiới123", "Thiên Mệnh Giả");
            nameplate.style.marginLeft = 12;
            identity.Add(nameplate);
            panel.Add(identity);

            var hp = new HealthBar(theme); hp.SetValue(0.82f); hp.style.marginTop = 16; panel.Add(hp);
            var mp = new ManaBar(theme); mp.SetValue(0.64f); mp.style.marginTop = 8; panel.Add(mp);

            var tabs = new TabBar(theme); tabs.style.marginTop = 16;
            tabs.AddTab("Bạn Bè"); tabs.AddTab("Bang Hội"); tabs.AddTab("Sự Kiện");
            panel.Add(tabs);

            var actions = new VisualElement();
            actions.style.flexDirection = FlexDirection.Row;
            actions.style.flexWrap = Wrap.Wrap;
            actions.style.marginTop = 8;
            actions.Add(new SkillButton(theme, "Q"));
            actions.Add(new SkillButton(theme, "W"));
            actions.Add(new SkillButton(theme, "E"));
            actions.Add(new IconButton(theme) { text = "✦" });
            panel.Add(actions);

            var currency = new CurrencyDisplay(theme, "Linh Thạch", 12680); currency.style.marginTop = 16; panel.Add(currency);
            var toast = new Toast(theme); toast.Show("Cổng Âm Giới sắp mở — tập hợp tổ đội!"); toast.style.marginTop = 16; panel.Add(toast);
        }
    }
}
