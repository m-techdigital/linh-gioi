using LinhGioi.Combat;
using LinhGioi.CombatUI;
using LinhGioi.UI;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests
{
    public sealed class M1OfflineCombatHudTests
    {
        [Test]
        public void OfflineCombatHudBindsResultWithoutCombatSideEffects()
        {
            var theme = ScriptableObject.CreateInstance<ThemeTokens>();
            theme.minimumTouchTarget = 44;
            theme.spacing = new[] { 4, 8, 12, 16 };
            theme.bg = Color.black;
            theme.surface = Color.gray;
            theme.surfaceRaised = Color.gray;
            theme.spirit = Color.cyan;
            theme.shadow = Color.magenta;
            theme.gold = Color.yellow;
            theme.danger = Color.red;
            theme.text = Color.white;
            theme.muted = Color.gray;

            var view = new OfflineCombatHudView(theme);
            view.BindResult(new M1OfflineCombatRunResult
            {
                status = "PASS",
                enemyContentId = GameDataCombatCatalog.DefaultMonsterId,
                skillId = GameDataCombatCatalog.DefaultSkillId,
                targetDefeated = true,
                initialPlayerHp = 160,
                finalPlayerHp = 160,
                initialEnemyHp = 120,
                finalEnemyHp = 0,
                lastActionStatus = "Victory",
                lastActionDamage = 55
            });

            Assert.NotNull(view.SkillButton);
            Assert.AreEqual("Phong Trảm", view.SkillButton.text);
            Object.DestroyImmediate(theme);
        }
    }
}
