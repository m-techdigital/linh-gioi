using System.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class M4PlayableClientController
    {
        internal IEnumerator CaptureEvidenceTouchMovement()
        {
            if (!_isMobileProfile) yield break;
            var pad = _worldTouchMovementPad as RuntimeTouchMovementPad;
            if (pad == null || _world == null || !Application.isFocused)
                throw new InvalidOperationException("Touch movement evidence requires the focused mobile world and movement pad.");
            var start = _world.CurrentPosition;
            var yaw = _world.CurrentYawDegrees;
            var point = pad.worldBound.center + Vector2.right * pad.contentRect.width * 0.35f;
            try
            {
                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                    pad.SendEvent(down);
                for (var i = 0; i < 12; i++) yield return null;
                var distance = Vector3.Distance(start, _world.CurrentPosition);
                if (distance < 0.1f || pad.Value.x <= 0f)
                    throw new InvalidOperationException("Touch movement evidence failed: pointer did not move the player.");

                using (var up = PointerUpEvent.GetPooled(new Event { type = EventType.MouseUp, button = 0, mousePosition = point }))
                    pad.SendEvent(up);
                yield return null;
                yield return null;
                var released = _world.CurrentPosition;
                for (var i = 0; i < 4; i++) yield return null;
                if (pad.Value != Vector2.zero || Vector3.Distance(released, _world.CurrentPosition) > 0.001f)
                    throw new InvalidOperationException("Touch movement evidence failed: movement continued after release.");

                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                    pad.SendEvent(down);
                yield return null;
                SetSessionMenuVisible(true);
                yield return null;
                yield return null;
                var paused = _world.CurrentPosition;
                for (var i = 0; i < 4; i++) yield return null;
                if (pad.Value != Vector2.zero || Vector3.Distance(paused, _world.CurrentPosition) > 0.001f)
                    throw new InvalidOperationException("Touch movement evidence failed: movement continued in menu.");
                Debug.Log("LGO_TOUCH_MOVEMENT_PLAYER_PASS pointer=simulated moved=" + distance.ToString("F3", System.Globalization.CultureInfo.InvariantCulture) + " release=stopped menu=stopped");
            }
            finally
            {
                pad.ResetInput();
                _world.TouchMovement = Vector2.zero;
                SetSessionMenuVisible(false);
                _world.SetSmokePosition(start.x, start.y, start.z, yaw);
            }
        }

        internal async Task CaptureEvidenceLoginAsync()
        {
            await LoginAsync();
        }

        internal async Task CaptureEvidenceCreateCharacterIfNeededAsync(string characterName)
        {
            if (_selectedCharacter != null) return;
            _characterName.value = Required(characterName, "EvidenceHero");
            _classId.value = DefaultClassId;
            await CreateCharacterAsync();
        }

        internal async Task CaptureEvidenceEnterWorldAsync()
        {
            await EnterWorldAsync();
            _evidenceState = RuntimeUiEvidenceState.EnterWorldTransition;
            RefreshWorldLoopLabels();
        }

        internal void CaptureEvidenceWorldHub()
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _skillPreviewActive = false;
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceNearGateKeeperPrompt()
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _skillPreviewActive = false;
            _world.SetSmokePositionNearGateKeeper();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceNearTrainingStonePrompt()
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _skillPreviewActive = false;
            _world.SetSmokePositionNearGateKeeper();
            _world.TriggerInteractionForSmoke();
            while (_world.DialogueActive)
                _world.ContinueDialogue();
            _world.SetSmokePositionNearTrainingStone();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceTrainingComplete()
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _skillPreviewActive = false;
            _world.SetSmokePositionNearTrainingStone();
            _world.TriggerInteractionForSmoke();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceOpenDialogue()
        {
            if (_world == null || _selectedCharacter == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _skillPreviewActive = false;
            _world.Enter(_selectedCharacter);
            _world.SetSmokePositionNearGateKeeper();
            _world.TriggerInteractionForSmoke();
            RefreshWorldLoopLabels();
        }

        internal void CaptureEvidenceOpenLongDialogue()
        {
            CaptureEvidenceOpenDialogue();
            if (_dialogueLine == null || _dialogueProgress == null) return;
            _dialogueLine.text =
                "Người Giữ Cổng: Linh Môn mở ra không chỉ để con bước qua, mà để thử xem tâm thức có giữ được nhịp thở giữa gió mạnh hay không. " +
                "Khi lời dẫn kéo dài, khung thoại phải giữ nguyên hình dáng, phần chữ tự cuộn trong vùng đọc, còn tiến trình và nút hành động vẫn nằm đúng vị trí để người chơi không bị mất điều khiển.";
            _dialogueProgress.text = "Đối thoại dài: kiểm tra cuộn";
        }

        internal void CaptureEvidenceTargetDummyState()
        {
            if (_world == null) return;
            // LGO Combat Button Mobile Responsive Evidence v1: the compact HUD normally hides combat chrome,
            // but the target-dummy checkpoint must expose it so mobile/tablet screenshots prove button fit.
            _evidenceState = RuntimeUiEvidenceState.CombatPanelFocus;
            _skillPreviewActive = false;
            _world.SetSmokePositionNearTargetDummy();
            _world.TriggerLocalCombatForSmoke();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceShadowBindPreview()
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _world.ResetLocalCombatPreviewStateForSmoke();
            PreviewSkill("Shadow Bind", "Trói Bóng");
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceOpenSessionMenu()
        {
            SetSessionMenuVisible(true);
        }
    }
}
