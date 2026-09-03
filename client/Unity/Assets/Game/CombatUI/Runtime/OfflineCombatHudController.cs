using LinhGioi.Combat;
using LinhGioi.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.CombatUI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class OfflineCombatHudController : MonoBehaviour
    {
        [SerializeField] private ThemeTokens theme;
        [SerializeField] private TextAsset compiledGameDataManifest;
        private OfflineCombatHudView _view;

        private void OnEnable()
        {
            var document = GetComponent<UIDocument>();
            var activeTheme = theme != null ? theme : UIThemeContext.Require();
            _view = new OfflineCombatHudView(activeTheme);
            document.rootVisualElement.Clear();
            document.rootVisualElement.Add(_view);
            _view.SkillButton.clicked += RunPrototype;
            RunPrototype();
        }

        private void OnDisable()
        {
            if (_view != null) _view.SkillButton.clicked -= RunPrototype;
        }

        public void Configure(ThemeTokens themeAsset, TextAsset manifest)
        {
            theme = themeAsset;
            compiledGameDataManifest = manifest;
        }

        public void RunPrototype()
        {
            if (_view == null) return;
            if (compiledGameDataManifest == null)
            {
                _view.BindResult(null);
                return;
            }
            var catalog = GameDataCombatCatalog.FromCompiledManifestJson(compiledGameDataManifest.text);
            _view.BindResult(M1OfflineCombatScenario.RunDeterministicDuel(catalog));
        }
    }
}
