using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed partial class CongDongLamMap01AArtPreview
    {
        private NpcDialogueSession _dialogueSession;
        private Action _onDialogueComplete;
        private bool _inventoryBeforeDialogue;
        public bool DialogueOpen => _dialogueSession != null && _dialogueSession.Active;
        public string DialogueSpeaker => _dialogueSession?.Speaker ?? "";
        public string DialogueText => _dialogueSession?.Line ?? "";
        public string DialogueProgress => _dialogueSession?.Progress ?? "";
        public bool CanReadDialogueInformation => _dialogueSession != null && _dialogueSession.CanReadInformation;
        public string DialogueActionLabel => _dialogueSession == null ? "Trò chuyện"
            : _dialogueSession.ReadingInformation ? "Quay lại"
            : _dialogueSession.HasNext ? "Tiếp tục" : _dialogueSession.CompletionAction;
        public bool CanTalkToCurrentNpc => Mathf.Abs(PlayerX - RouteNodeX[_currentRouteIndex]) <= .95f
            && (CurrentRouteNodeId == "spawn-ha-van" || CurrentRouteNodeId == "quan-thu"
                || CurrentRouteNodeId == "tong-phu" || CurrentRouteNodeId == "thanh-nhi"
                || CurrentRouteNodeId == "well-bridge" || CurrentRouteNodeId == "lao-tran");

        public bool UseNpcConversation()
        {
            if (!CanTalkToCurrentNpc) return false;
            if (DialogueOpen) return AdvanceNpcDialogue();
            var node = CurrentRouteNodeId;
            var offer = node == "spawn-ha-van" && ActiveQuestId == "Q01"
                || node == "quan-thu" && ActiveQuestId == "Q03"
                || node == "tong-phu" && ActiveQuestId == "Q04" && HasInspectedInventory && !HasStarterSupplies
                || node == "thanh-nhi" && ActiveQuestId == "Q05" && !HasAcceptedGatherQuest
                || node == "lao-tran" && ActiveQuestId == "Q06" && !HasAcceptedCombatQuest;
            var done = node == "spawn-ha-van" && HasMetHaVan
                || node == "quan-thu" && IsQuestComplete("Q03")
                || node == "tong-phu" && HasUsedHealthPotion
                || node == "thanh-nhi" && HasHarvestedSpiritHerb
                || node == "well-bridge" && HasOpenedHiddenChest
                || node == "lao-tran" && HasDefeatedFirstEnemy;
            var hint = ActiveQuestId == "COMPLETE" ? "Lối Suối Thanh Minh đã mở. Ngươi có thể tiếp tục khám phá Đông Lâm."
                : QuestName(ActiveQuestId) + "\n" + QuestObjective(ActiveQuestId);
            _dialogueSession = Map01ADialogueContent.Create(node, offer, done, hint);
            _dialogueNodeId = node;
            _onDialogueComplete = offer ? () => AcceptNpcTask(node) : (Action)null;
            _inventoryBeforeDialogue = InventoryOpen;
            InventoryOpen = false;
            _dialogueSession.Open();
            LastInteractionMessage = DialogueSpeaker + " đang trò chuyện.";
            LogDialoguePage();
            return true;
        }

        private bool AdvanceNpcDialogue()
        {
            if (!DialogueOpen || _dialogueNodeId != CurrentRouteNodeId) return false;
            _dialogueSession.Advance();
            if (DialogueOpen) LogDialoguePage();
            else
            {
                InventoryOpen = _inventoryBeforeDialogue;
                var completion = _onDialogueComplete;
                _onDialogueComplete = null;
                LastInteractionMessage = "Đã trò chuyện với " + DialogueSpeaker + ".";
                completion?.Invoke();
            }
            return true;
        }

        public bool ReadDialogueInformation()
        {
            if (_dialogueSession == null || !_dialogueSession.ReadInformation()) return false;
            LogDialoguePage();
            return true;
        }

        public void CloseNpcDialogue()
        {
            if (_dialogueSession == null || !_dialogueSession.Close()) return;
            _onDialogueComplete = null;
            InventoryOpen = _inventoryBeforeDialogue;
            LastInteractionMessage = "Đã tạm dừng trò chuyện với " + DialogueSpeaker + ".";
        }

        private void LogDialoguePage() => Debug.Log("LGO_MAP01A_DIALOGUE node=" + _dialogueNodeId
            + " page=" + DialogueProgress + " information=" + _dialogueSession.ReadingInformation);

        private void AcceptNpcTask(string node)
        {
            switch (node)
            {
                case "spawn-ha-van":
                    HasMetHaVan = true;
                    if (ActiveQuestId == "Q01") CompleteQuest("Q01", "Q02");
                    break;
                case "quan-thu":
                    if (ActiveQuestId == "Q03") CompleteQuest("Q03", "Q04");
                    break;
                case "tong-phu":
                    if (HasStarterSupplies) return;
                    HasStarterSupplies = true; HealthPotionCount += 3; ManaPotionCount += 2;
                    InventoryOpen = true;
                    LastInteractionMessage = "Nhận Bình Máu Nhỏ ×3 và Bình Linh Lực Nhỏ ×2; hãy dùng một bình máu.";
                    break;
                case "thanh-nhi":
                    HasAcceptedGatherQuest = true;
                    LastInteractionMessage = "Thanh Nhi chỉ Linh Thảo phát sáng bên giếng.";
                    break;
                case "lao-tran":
                    HasAcceptedCombatQuest = true;
                    LastInteractionMessage = "Lão Trần giao việc hạ một quái non ở rìa làng.";
                    break;
            }
        }

        private IEnumerator CaptureNpcDialoguePages(string directory, string tag, CaptureInfo evidence)
        {
            var output = Path.Combine(directory, "dialogue"); Directory.CreateDirectory(output);
            for (var page = 1; DialogueOpen && page <= 8; page++)
            {
                yield return null; yield return new WaitForEndOfFrame();
                WriteMapCaptureBmp(Path.Combine(output, tag + "-" + page + ".bmp"), evidence.width, evidence.height);
                evidence.dialogueFrames++;
                if (CanReadDialogueInformation)
                {
                    ReadDialogueInformation();
                    yield return null; yield return new WaitForEndOfFrame();
                    WriteMapCaptureBmp(Path.Combine(output, tag + "-" + page + "-info.bmp"), evidence.width, evidence.height);
                    evidence.dialogueFrames++;
                    if (!UseCurrentRouteAction()) throw new InvalidOperationException("Cannot return from dialogue information");
                }
                if (!UseCurrentRouteAction()) throw new InvalidOperationException("Cannot advance dialogue " + tag);
            }
            if (DialogueOpen) throw new InvalidOperationException("Dialogue did not terminate: " + tag);
        }

        private static void WriteMapCaptureBmp(string path, int width, int height)
        {
            var active = RenderTexture.active;
            var image = new Texture2D(width, height, TextureFormat.RGBA32, false);
            try
            {
                RenderTexture.active = null;
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
                DongMonIllustratedPreview.WriteBmp(path, image.GetPixels32(), width, height);
            }
            finally { RenderTexture.active = active; Destroy(image); }
        }
    }
}
