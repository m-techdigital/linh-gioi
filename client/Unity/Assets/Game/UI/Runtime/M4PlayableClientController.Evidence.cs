using System.Threading.Tasks;
using System;
using System.Collections;
using LinhGioi.Account;
using LinhGioi.Art;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class M4PlayableClientController
    {
        internal IEnumerator CaptureEvidenceTouchMovement()
        {
            var focusDeadline = Time.realtimeSinceStartup + 15f;
            while (!Application.isFocused && Time.realtimeSinceStartup < focusDeadline)
                yield return null;
            var pad = _worldTouchMovementPad as RuntimeTouchMovementPad;
            if (_worldTouchControlsOverlay.resolvedStyle.display != DisplayStyle.Flex)
                throw new InvalidOperationException("World controls must be visible on " + _lastLayoutProfile);
            if (_worldTouchMenuButton.parent == _worldTouchActionCluster)
                throw new InvalidOperationException("Session navigation must not occupy a combat action slot.");
            if (pad == null || _world == null || !Application.isFocused)
                throw new InvalidOperationException("Touch movement prerequisites: pad=" + (pad != null) + " world=" + (_world != null) + " focused=" + Application.isFocused);
            var start = _world.CurrentPosition;
            var yaw = _world.CurrentYawDegrees;
            var point = pad.worldBound.center + Vector2.right * pad.contentRect.width * 0.35f;
            try
            {
                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                {
                    down.target = pad;
                    pad.SendEvent(down);
                }
                var initialInput = pad.Value;
                var initialBounds = pad.worldBound;
                var startedAt = Time.realtimeSinceStartup;
                for (var i = 0; i < 12; i++) yield return null;
                var distance = Vector3.Distance(start, _world.CurrentPosition);
                if (distance < 0.1f || pad.Value.x <= 0f)
                    throw new InvalidOperationException("Touch movement evidence failed: distance=" + distance
                        + " initialInput=" + initialInput + " finalInput=" + pad.Value
                        + " focused=" + Application.isFocused + " elapsed=" + (Time.realtimeSinceStartup - startedAt)
                        + " initialBounds=" + initialBounds + " finalBounds=" + pad.worldBound);

                using (var up = PointerUpEvent.GetPooled(new Event { type = EventType.MouseUp, button = 0, mousePosition = point }))
                {
                    up.target = pad;
                    pad.SendEvent(up);
                }
                yield return null;
                yield return null;
                var released = _world.CurrentPosition;
                for (var i = 0; i < 4; i++) yield return null;
                if (pad.Value != Vector2.zero || Vector3.Distance(released, _world.CurrentPosition) > 0.001f)
                    throw new InvalidOperationException("Touch movement evidence failed after release: pad=" + pad.Value
                        + " worldInput=" + _world.TouchMovement + " distance=" + Vector3.Distance(released, _world.CurrentPosition)
                        + " keyboard=" + Input.GetAxisRaw("Horizontal") + "," + Input.GetAxisRaw("Vertical")
                        + " focused=" + Application.isFocused);

                using (var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = point }))
                {
                    down.target = pad;
                    pad.SendEvent(down);
                }
                yield return null;
                SetSessionMenuVisible(true);
                yield return null;
                yield return null;
                var paused = _world.CurrentPosition;
                for (var i = 0; i < 4; i++) yield return null;
                if (pad.Value != Vector2.zero || Vector3.Distance(paused, _world.CurrentPosition) > 0.001f)
                    throw new InvalidOperationException("Touch movement evidence failed: movement continued in menu.");
                AssertCenteredSurfaceForEvidence(_sessionMenuPanel);
                foreach (var button in _sessionActions.Query<Button>().ToList())
                {
                    var bounds = button.worldBound;
                    var container = _sessionActions.worldBound;
                    if (bounds.xMin < container.xMin - 1 || bounds.xMax > container.xMax + 1)
                        throw new InvalidOperationException("Session action exceeds shared column: button=" + bounds + " column=" + container);
                }
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
            if (_characterNameError != null)
                throw new InvalidOperationException("A valid name must clear the inline error before submission.");
            _classId.value = DefaultClassId;
            await CreateCharacterAsync();
        }

        internal async Task CaptureEvidenceInvalidCharacterNameAsync()
        {
            var selected = _selectedCharacter;
            var count = _characters.Length;
            _characterName.value = "A!";
            await CreateCharacterAsync();
            if (_characterNameError == null || _selectedCharacter != selected || _characters.Length != count)
                throw new InvalidOperationException("Invalid name must remain in the form without creating a character.");
            _characterName.Blur();
            Debug.Log("LGO_CHARACTER_NAME_REJECTED_LOCALLY_PASS");
        }

        internal void AssertInvalidCharacterNameForEvidence()
        {
            if (_characterNameError == null || _createHint.text != _characterNameError
                || _createHint.resolvedStyle.display != DisplayStyle.Flex || CharacterNameRules.IsValid(_characterName.value))
                throw new InvalidOperationException("Invalid-name capture state changed before screenshot.");
            AssertCharacterFormBoundsForEvidence();
        }

        internal void CaptureEvidenceCorrectCharacterName()
        {
            _characterName.value = "EvidenceHero";
            if (_characterNameError != null)
                throw new InvalidOperationException("Editing a valid name did not clear the inline error.");
        }

        internal async Task CaptureEvidenceSecondCharacterAsync()
        {
            OnCreateCharacterAction();
            _characterName.value = "EvidenceTwin";
            await CreateCharacterAsync();
            if (_characters.Length != 2 || _selectedCharacter == null || _selectedCharacter.name != "EvidenceTwin")
                throw new InvalidOperationException("Creating the second character must select it: actual=" + _selectedCharacter?.name);
            var createdId = _selectedCharacter.characterId;
            await RefreshCharactersAsync();
            if (_selectedCharacter?.characterId != createdId)
                throw new InvalidOperationException("Refreshing the roster lost the selected character.");
            Debug.Log("LGO_SECOND_CHARACTER_SELECTED_PASS");
        }

        internal void AssertSelectedRosterForEvidence()
        {
            var selectedCount = 0;
            foreach (var button in _characterList.Query<Button>(className: "lgo-list-item").ToList())
            {
                var expected = Equals(button.userData, _selectedCharacter.characterId);
                if (button.ClassListContains("lgo-list-selected") != expected
                    || (button.resolvedStyle.borderTopColor == RuntimeArtCatalog.Gold) != expected)
                    throw new InvalidOperationException("Roster highlight does not match the selected character.");
                if (expected) selectedCount++;
            }
            if (selectedCount != 1 || _selectedName.text != _selectedCharacter.name)
                throw new InvalidOperationException("Roster and character preview must agree on one selection.");
        }

        internal async Task CaptureEvidenceLongRosterAsync()
        {
            for (var index = 3; index <= 3; index++)
            {
                OnCreateCharacterAction();
                _characterName.value = "Evidence" + index.ToString("D2");
                await CreateCharacterAsync();
                if (_characters.Length != index)
                    throw new InvalidOperationException("Long roster fixture could not create character " + index);
            }
        }

        internal void AssertSelectedRosterVisibleForEvidence()
        {
            AssertSelectedRosterForEvidence();
            var scroll = _characterList as ScrollView;
            var selected = _characterList.Q<Button>(className: "lgo-list-selected");
            if (scroll == null || selected == null) throw new InvalidOperationException("Roster scroll or selection missing.");
            var viewport = scroll.contentViewport.worldBound;
            var row = selected.worldBound;
            if (row.yMin < viewport.yMin - 1 || row.yMax > viewport.yMax + 1 || row.xMin < viewport.xMin - 1 || row.xMax > viewport.xMax + 1)
                throw new InvalidOperationException("Selected roster row is outside its scroll viewport: row=" + row + " viewport=" + viewport);
            if (_characterList.Query<Button>(className: "lgo-character-slot").ToList().Count != 3)
                throw new InvalidOperationException("Character Hall must always have exactly three slots.");
            if (scroll.verticalScroller.highValue > 0 && scroll.verticalScroller.resolvedStyle.display != DisplayStyle.Flex)
                throw new InvalidOperationException("Overflowing roster must expose the shared scrollbar.");
            var scroller = scroll.verticalScroller;
            var pixelTolerance = Mathf.Abs(RuntimePanelUtils.ScreenToPanel(scroll.panel, Vector2.right).x
                - RuntimePanelUtils.ScreenToPanel(scroll.panel, Vector2.zero).x);
            Debug.Log("LGO_ROSTER_SCROLLER_METRICS width=" + scroller.resolvedStyle.width + " minWidth=" + scroller.resolvedStyle.minWidth
                + " slider=" + scroller.slider.worldBound);
            if (scroller.resolvedStyle.width > 8 + pixelTolerance || scroller.slider.worldBound.xMax > scroller.worldBound.xMax + pixelTolerance)
                throw new InvalidOperationException("Shared compact scrollbar exceeds its width budget.");
            Debug.Log("LGO_ROSTER_SELECTED_VISIBLE_PASS count=" + _characters.Length + " offset=" + scroll.scrollOffset.y);
        }

        internal IEnumerator CaptureEvidenceScrollRosterToStart()
        {
            var scroll = (ScrollView)_characterList;
            scroll.verticalScroller.value = 0;
            for (var frame = 0; frame < 12; frame++)
            {
                Debug.Log("LGO_ROSTER_SCROLL_TRACE frame=" + frame + " offset=" + scroll.scrollOffset.y
                    + " slider=" + scroll.verticalScroller.value + " viewport=" + scroll.contentViewport.layout
                    + " content=" + scroll.contentContainer.layout);
                yield return null;
            }
            if (scroll.scrollOffset.y > 1)
                throw new InvalidOperationException("Roster selection must not undo manual scrolling.");
            Debug.Log("LGO_ROSTER_MANUAL_SCROLL_PASS input=scroller-value");
            yield return CaptureEvidenceReselectFirstCharacter();
            for (var frame = 0; frame < 6; frame++) yield return null;
            AssertSelectedRosterVisibleForEvidence();
        }

        internal IEnumerator CaptureEvidenceReselectFirstCharacter()
        {
            var firstId = _characters[0].characterId;
            var button = _characterList.Query<Button>(className: "lgo-list-item").ToList().Find(row => Equals(row.userData, firstId));
            if (button == null) throw new InvalidOperationException("First character row is missing.");
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = button;
                button.SendEvent(submit);
            }
            yield return null;
            if (_selectedCharacter.characterId != firstId)
                throw new InvalidOperationException("Submitting the roster row did not select the first character.");
            AssertSelectedRosterForEvidence();
            Debug.Log("LGO_ROSTER_SELECTION_SWITCH_PASS input=navigation-submit");
        }

        internal async Task CaptureEvidenceEnterWorldAsync()
        {
            await EnterWorldAsync();
            _evidenceState = RuntimeUiEvidenceState.EnterWorldTransition;
            RefreshWorldLoopLabels();
        }

        internal void CaptureEvidenceExpandCharacterForm()
        {
            OnCreateCharacterAction();
        }

        internal void CaptureEvidenceLongCharacterName(bool restore)
        {
            _selectedName.text = restore ? _selectedCharacter.name : new string('W', 16);
        }

        internal void CaptureEvidenceCloseCharacterForm()
        {
            var selected = _selectedCharacter;
            OnEnterWorldOrCancelCreateAction();
            if (_createFormExpanded || _selectedCharacter != selected || IsDisplayed(_worldHud))
                throw new InvalidOperationException("Cancel character form must preserve selection without entering the world.");
            Debug.Log("LGO_CHARACTER_CREATE_CANCEL_PASS selection=preserved world=not-entered");
        }

        internal void CaptureEvidenceReapplyCharacterLayout()
        {
            ApplyResponsiveLayoutProfile(true);
        }

        internal void AssertCharacterFormBoundsForEvidence()
        {
            AssertCenteredSurfaceForEvidence(_lobbyPanel);
            var backdrop = LinhGioi.Art.LgoVisualAssetRegistryV3B.LinhThanhNightBackground;
            if (backdrop == null || _root.resolvedStyle.backgroundImage.texture != backdrop)
                throw new InvalidOperationException("Character Hall must load the approved night-city background, not silently use the old fallback.");
            var headingFont = Resources.Load<Font>("LGOUI/HeadingSerif");
            var heading = _lobbyHeaderBlock.Q<Label>();
            if (headingFont == null || heading == null || heading.resolvedStyle.unityFontDefinition.font != headingFont)
                throw new InvalidOperationException("Character Hall title must use the shared bundled heading font.");
            if (_lobbyHeaderBlock.resolvedStyle.display != DisplayStyle.Flex)
                throw new InvalidOperationException("Character selection must preserve its shared shell header.");
            var slots = _characterList.Query<Button>(className: "lgo-character-slot").ToList();
            if (slots.Count != 3) throw new InvalidOperationException("Character Hall must always show three slots.");
            foreach (var slot in slots)
            {
                var avatar = slot.Q<VisualElement>("LGO Character Slot Avatar");
                if (avatar == null || avatar.resolvedStyle.backgroundImage.texture == null)
                    throw new InvalidOperationException("Every character slot must have a loaded portrait or empty silhouette.");
                if (avatar.worldBound.xMin < slot.worldBound.xMin - 1 || avatar.worldBound.xMax > slot.worldBound.xMax + 1
                    || avatar.worldBound.yMin < slot.worldBound.yMin - 1 || avatar.worldBound.yMax > slot.worldBound.yMax + 1)
                    throw new InvalidOperationException("Character slot avatar exceeds its button bounds.");
            }
            var bodyFont = Resources.Load<Font>("LGOUI/BodySans");
            if (bodyFont == null || _root.resolvedStyle.unityFontDefinition.font != bodyFont)
                throw new InvalidOperationException("Runtime UI must use its bundled body font, not an OS font.");
            var viewport = _root.worldBound;
            var form = _createPanel.worldBound;
            foreach (var element in new VisualElement[] { _createPanel, _characterName, _createHint, _createButton, _enterWorldButton })
            {
                if (element.resolvedStyle.display == DisplayStyle.None) continue;
                var bounds = element.worldBound;
                var container = element == _createPanel ? viewport : form;
                if (bounds.width <= 0 || bounds.height <= 0 || bounds.xMin < container.xMin - 1 || bounds.xMax > container.xMax + 1
                    || bounds.yMin < container.yMin - 1 || bounds.yMax > container.yMax + 1)
                    throw new InvalidOperationException("Character form resize overflow: " + element.name + " bounds=" + bounds + " container=" + container);
            }
            Debug.Log("LGO_CHARACTER_FORM_RESIZE_BOUNDS_PASS screen=" + Screen.width + "x" + Screen.height + " form=" + form);
        }

        private void AssertCenteredSurfaceForEvidence(VisualElement surface)
        {
            var bounds = surface.worldBound;
            var viewport = _root.worldBound;
            if (Mathf.Abs(bounds.center.x - viewport.center.x) > 1 || Mathf.Abs(bounds.center.y - viewport.center.y) > 1)
                throw new InvalidOperationException("Shared centered surface is off-center: " + surface.name
                    + " bounds=" + bounds + " viewport=" + viewport);
            if (bounds.xMin < viewport.xMin || bounds.xMax > viewport.xMax
                || bounds.yMin < viewport.yMin || bounds.yMax > viewport.yMax)
                throw new InvalidOperationException("Centered surface exceeds its viewport: " + surface.name);
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
            if (_isMobileProfile)
            {
                TriggerWorldTouchPrimaryAction();
                if (!_world.DialogueActive) throw new InvalidOperationException("Mobile primary action did not open Gate Keeper dialogue.");
            }
            else _world.TriggerInteractionForSmoke();
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
            if (_isMobileProfile)
            {
                TriggerWorldTouchPrimaryAction();
                if (!_world.InteractionAcknowledged) throw new InvalidOperationException("Mobile primary action did not complete training.");
                Debug.Log("LGO_MOBILE_GUIDED_ACTIONS_PASS keeper=dialogue stone=completed");
            }
            else _world.TriggerInteractionForSmoke();
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
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = _worldTouchWindSlashButton;
                _worldTouchWindSlashButton.SendEvent(submit);
            }
            if (!_world.LocalCombatCoolingDown)
                throw new InvalidOperationException("World Wind Slash action must execute combat, not only preview VFX.");
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
