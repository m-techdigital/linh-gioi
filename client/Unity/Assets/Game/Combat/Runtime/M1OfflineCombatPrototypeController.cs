using UnityEngine;

namespace LinhGioi.Combat
{
    public sealed class M1OfflineCombatPrototypeController : MonoBehaviour
    {
        [SerializeField] private TextAsset compiledGameDataManifest;
        [SerializeField] private bool runOnStart = true;

        public M1OfflineCombatRunResult LastResult { get; private set; }

        private void Start()
        {
            if (!runOnStart || compiledGameDataManifest == null) return;
            Run(compiledGameDataManifest.text);
        }

        public M1OfflineCombatRunResult Run(string compiledManifestJson)
        {
            var catalog = GameDataCombatCatalog.FromCompiledManifestJson(compiledManifestJson);
            LastResult = M1OfflineCombatScenario.RunDeterministicDuel(catalog);
            Debug.Log($"[LinhGioi] M1 offline combat result={LastResult.status} actions={LastResult.actionsExecuted} finalEnemyHp={LastResult.finalEnemyHp}");
            return LastResult;
        }
    }
}
