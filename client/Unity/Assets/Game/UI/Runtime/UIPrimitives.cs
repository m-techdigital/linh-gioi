using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public class BaseButton : Button
    {
        public BaseButton() : this(UIThemeContext.Require()) { }
        public BaseButton(ThemeTokens theme)
        {
            style.minHeight = theme.minimumTouchTarget;
            style.paddingLeft = theme.SpaceL;
            style.paddingRight = theme.SpaceL;
            style.backgroundColor = theme.spirit;
            style.color = theme.text;
            style.unityFontStyleAndWeight = FontStyle.Bold;
            style.borderTopLeftRadius = RuntimeUiSizing.BaseButtonRadius;
            style.borderTopRightRadius = RuntimeUiSizing.BaseButtonRadius;
            style.borderBottomLeftRadius = RuntimeUiSizing.BaseButtonRadius;
            style.borderBottomRightRadius = RuntimeUiSizing.BaseButtonRadius;
        }
    }

    public sealed class IconButton : BaseButton
    {
        public IconButton(ThemeTokens theme) : base(theme)
        {
            style.minWidth = theme.minimumTouchTarget;
            style.paddingLeft = theme.SpaceS;
            style.paddingRight = theme.SpaceS;
        }
    }

    public class BasePanel : VisualElement
    {
        public BasePanel(ThemeTokens theme)
        {
            style.backgroundColor = theme.surface;
            RuntimeUiSkin.ApplyPadding(this, theme.SpaceL, theme.SpaceL);
            style.borderTopLeftRadius = RuntimeUiSizing.BasePanelRadius;
            style.borderTopRightRadius = RuntimeUiSizing.BasePanelRadius;
            style.borderBottomLeftRadius = RuntimeUiSizing.BasePanelRadius;
            style.borderBottomRightRadius = RuntimeUiSizing.BasePanelRadius;
        }
    }

    public sealed class ModalPanel : BasePanel
    {
        public ModalPanel(ThemeTokens theme) : base(theme)
        {
            style.backgroundColor = theme.surfaceRaised;
            style.maxWidth = RuntimeUiSizing.ModalMaxWidth;
        }
    }

    public class ProgressBar : VisualElement
    {
        private readonly VisualElement _fill;
        private readonly Label _label;
        public float Value { get; private set; }

        public ProgressBar(ThemeTokens theme, Color fillColor)
        {
            style.height = RuntimeUiSizing.ProgressBarHeight;
            style.backgroundColor = theme.bg;
            style.borderTopLeftRadius = RuntimeUiSizing.ProgressBarRadius;
            style.borderTopRightRadius = RuntimeUiSizing.ProgressBarRadius;
            style.borderBottomLeftRadius = RuntimeUiSizing.ProgressBarRadius;
            style.borderBottomRightRadius = RuntimeUiSizing.ProgressBarRadius;
            _fill = new VisualElement();
            _fill.style.position = Position.Absolute;
            _fill.style.left = 0;
            _fill.style.top = 0;
            _fill.style.bottom = 0;
            _fill.style.backgroundColor = fillColor;
            _fill.style.borderTopLeftRadius = RuntimeUiSizing.ProgressBarRadius;
            _fill.style.borderTopRightRadius = RuntimeUiSizing.ProgressBarRadius;
            _fill.style.borderBottomLeftRadius = RuntimeUiSizing.ProgressBarRadius;
            _fill.style.borderBottomRightRadius = RuntimeUiSizing.ProgressBarRadius;
            _label = new Label();
            _label.style.unityTextAlign = TextAnchor.MiddleCenter;
            _label.style.color = theme.text;
            _label.style.position = Position.Absolute;
            _label.style.left = 0;
            _label.style.right = 0;
            _label.style.top = 0;
            _label.style.bottom = 0;
            Add(_fill);
            Add(_label);
            SetValue(1f);
        }

        public void SetValue(float normalized)
        {
            Value = Mathf.Clamp01(normalized);
            _fill.style.width = Length.Percent(Value * 100f);
            _label.text = $"{Mathf.RoundToInt(Value * 100f)}%";
        }
    }

    public sealed class HealthBar : ProgressBar
    {
        public HealthBar(ThemeTokens theme) : base(theme, theme.danger) { }
    }

    public sealed class ManaBar : ProgressBar
    {
        public ManaBar(ThemeTokens theme) : base(theme, theme.spirit) { }
    }

    public sealed class SkillButton : BaseButton
    {
        public SkillButton(ThemeTokens theme, string label = "Skill") : base(theme)
        {
            text = label;
            style.minWidth = RuntimeUiSizing.SkillButtonSize;
            style.minHeight = RuntimeUiSizing.SkillButtonSize;
            style.backgroundColor = theme.shadow;
        }
    }

    public sealed class AvatarView : VisualElement
    {
        private readonly Label _initials;
        public AvatarView(ThemeTokens theme, string initials = "LG")
        {
            style.width = RuntimeUiSizing.AvatarSize;
            style.height = RuntimeUiSizing.AvatarSize;
            style.borderTopLeftRadius = RuntimeUiSizing.AvatarRadius;
            style.borderTopRightRadius = RuntimeUiSizing.AvatarRadius;
            style.borderBottomLeftRadius = RuntimeUiSizing.AvatarRadius;
            style.borderBottomRightRadius = RuntimeUiSizing.AvatarRadius;
            style.backgroundColor = theme.surfaceRaised;
            _initials = new Label(initials);
            _initials.style.color = theme.gold;
            _initials.style.unityTextAlign = TextAnchor.MiddleCenter;
            _initials.style.flexGrow = 1;
            Add(_initials);
        }
        public void SetInitials(string value) => _initials.text = value;
    }

    public sealed class Nameplate : VisualElement
    {
        private readonly Label _name;
        private readonly Label _subtitle;
        public Nameplate(ThemeTokens theme, string displayName, string subtitle = "")
        {
            _name = new Label(displayName);
            _name.style.color = theme.text;
            _name.style.unityFontStyleAndWeight = FontStyle.Bold;
            _subtitle = new Label(subtitle);
            _subtitle.style.color = theme.muted;
            Add(_name);
            Add(_subtitle);
        }
    }

    public sealed class TabBar : VisualElement
    {
        private readonly ThemeTokens _theme;
        public TabBar(ThemeTokens theme)
        {
            _theme = theme;
            style.flexDirection = FlexDirection.Row;
            style.flexWrap = Wrap.Wrap;
        }
        public BaseButton AddTab(string label, Action action = null)
        {
            var button = new BaseButton(_theme) { text = label };
            if (action != null) button.clicked += action;
            button.style.marginRight = _theme.SpaceS;
            button.style.marginBottom = _theme.SpaceS;
            Add(button);
            return button;
        }
    }

    public sealed class Toast : BasePanel
    {
        private readonly Label _label;
        public Toast(ThemeTokens theme) : base(theme)
        {
            _label = new Label();
            _label.style.color = theme.text;
            Add(_label);
        }
        public void Show(string message) => _label.text = message;
    }

    public sealed class CurrencyDisplay : VisualElement
    {
        private readonly Label _label;
        public CurrencyDisplay(ThemeTokens theme, string currencyName, long amount)
        {
            style.flexDirection = FlexDirection.Row;
            _label = new Label();
            _label.style.color = theme.gold;
            Add(_label);
            Set(currencyName, amount);
        }
        public void Set(string currencyName, long amount) => _label.text = $"{currencyName}: {amount:N0}";
    }

    public sealed class SafeAreaRoot : VisualElement
    {
        public void ApplySafeArea(Rect safeArea, Vector2 screenSize)
        {
            if (screenSize.x <= 0 || screenSize.y <= 0) return;
            var left = safeArea.xMin;
            var right = screenSize.x - safeArea.xMax;
            var bottom = safeArea.yMin;
            var top = screenSize.y - safeArea.yMax;
            style.paddingLeft = left;
            style.paddingRight = right;
            style.paddingBottom = bottom;
            style.paddingTop = top;
        }
    }
}
