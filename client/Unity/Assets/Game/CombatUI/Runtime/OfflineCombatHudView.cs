using LinhGioi.Combat;
using LinhGioi.UI;
using UnityEngine.UIElements;

namespace LinhGioi.CombatUI
{
    public sealed class OfflineCombatHudView : VisualElement
    {
        private readonly ThemeTokens _theme;
        private readonly HealthBar _playerHealth;
        private readonly HealthBar _enemyHealth;
        private readonly SkillButton _skillButton;
        private readonly Toast _combatLog;
        private readonly Label _title;
        private readonly Label _enemyName;

        public OfflineCombatHudView(ThemeTokens theme)
        {
            _theme = theme;
            style.flexGrow = 1;
            style.paddingLeft = 16;
            style.paddingRight = 16;
            style.paddingTop = 16;
            style.paddingBottom = 16;
            style.justifyContent = Justify.SpaceBetween;

            var topPanel = new BasePanel(theme);
            _title = new Label("M1 Offline Combat Prototype");
            _title.style.color = theme.gold;
            _title.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            _enemyName = new Label("monster.shadow.slime");
            _enemyName.style.color = theme.text;
            _enemyHealth = new HealthBar(theme);
            topPanel.Add(_title);
            topPanel.Add(_enemyName);
            topPanel.Add(_enemyHealth);

            var bottomPanel = new BasePanel(theme);
            _playerHealth = new HealthBar(theme);
            _skillButton = new SkillButton(theme, "Phong Trảm");
            _combatLog = new Toast(theme);
            _combatLog.Show("Sẵn sàng giao chiến.");
            bottomPanel.Add(new Nameplate(theme, "Kiếm tu tập sự", "class.sword"));
            bottomPanel.Add(_playerHealth);
            bottomPanel.Add(_skillButton);
            bottomPanel.Add(_combatLog);

            Add(topPanel);
            Add(bottomPanel);
        }

        public void BindResult(M1OfflineCombatRunResult result)
        {
            if (result == null)
            {
                _enemyHealth.SetValue(1f);
                _playerHealth.SetValue(1f);
                _combatLog.Show("Chưa có kết quả combat.");
                return;
            }

            _enemyName.text = result.enemyContentId;
            var enemyMaxHp = result.initialEnemyHp <= 0 ? result.finalEnemyHp : result.initialEnemyHp;
            var enemyRatio = enemyMaxHp <= 0 ? 0f : (float)result.finalEnemyHp / enemyMaxHp;
            _enemyHealth.SetValue(enemyRatio);
            var playerMaxHp = result.initialPlayerHp <= 0 ? result.finalPlayerHp : result.initialPlayerHp;
            var playerRatio = playerMaxHp <= 0 ? 0f : (float)result.finalPlayerHp / playerMaxHp;
            _playerHealth.SetValue(playerRatio);
            _combatLog.Show($"{result.skillId}: {result.lastActionStatus}, damage={result.lastActionDamage}, enemyHp={result.finalEnemyHp}/{enemyMaxHp}");
        }

        public SkillButton SkillButton => _skillButton;
    }
}
