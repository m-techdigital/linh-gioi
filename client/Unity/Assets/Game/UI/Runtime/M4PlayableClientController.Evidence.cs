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
            if (_characters.Length == 0 && (!string.IsNullOrEmpty(_characterName.value)
                || _characterName.textEdition.placeholder != "Danh xưng"))
                throw new InvalidOperationException("An empty account must show the name placeholder, not a prefilled character name.");
        }

        internal async Task CaptureEvidenceCreateCharacterIfNeededAsync(string characterName)
        {
            if (_selectedCharacter != null) return;
            var emptySlot = _characterList.Query<Button>(className: "lgo-list-item").ToList()
                .Find(button => Equals(button.userData, "empty-slot-2"));
            if (emptySlot == null) throw new InvalidOperationException("Third empty character slot is missing.");
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = emptySlot;
                emptySlot.SendEvent(submit);
            }
            if (_selectedSlot != 3 || !emptySlot.ClassListContains("lgo-list-selected"))
                throw new InvalidOperationException("Selecting an empty slot must select its creation destination.");
            _characterName.value = Required(characterName, "EvidenceHero");
            if (_characterNameError != null)
                throw new InvalidOperationException("A valid name must clear the inline error before submission.");
            _classId.value = DefaultClassId;
            await CreateCharacterAsync();
            if (_selectedCharacter?.slot != 3) throw new InvalidOperationException("Creation ignored the chosen character slot.");
            var createdId = _selectedCharacter.characterId;
            await RefreshCharactersAsync();
            if (_selectedCharacter?.characterId != createdId || _selectedCharacter.slot != 3)
                throw new InvalidOperationException("Roster refresh must preserve the chosen slot and character.");
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
            if (!string.IsNullOrEmpty(_characterName.value))
                throw new InvalidOperationException("Create another character must open a blank name draft.");
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
            if (_root.Q<VisualElement>("LGO Character Hall V3B Cultivator Portrait").style.unityBackgroundImageTintColor.value != Color.black)
                throw new InvalidOperationException("An empty creation slot must show the unassigned silhouette.");
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
            if (_selectedSlot != selected.slot
                || _root.Q<VisualElement>("LGO Character Hall V3B Cultivator Portrait").style.unityBackgroundImageTintColor.value != Color.white)
                throw new InvalidOperationException("Cancelling must restore the occupied slot and its portrait.");
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

        internal void CaptureEvidenceNearTrainingStonePrompt(bool approaching = false)
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _skillPreviewActive = false;
            _world.SetSmokePositionNearGateKeeper();
            if (!_world.DialogueCompleted)
            {
                RefreshWorldLoopLabels();
                using (var submit = NavigationSubmitEvent.GetPooled())
                {
                    submit.target = _worldTouchPrimaryActionButton;
                    _worldTouchPrimaryActionButton.SendEvent(submit);
                }
                if (!_world.DialogueActive) throw new InvalidOperationException("Primary button did not open Gate Keeper dialogue.");
            }
            for (var line = 0; line < 3 && _world.DialogueActive; line++)
            {
                _dialogueLineScroll.scrollOffset = new Vector2(0f, 20f);
                using (var submit = NavigationSubmitEvent.GetPooled())
                {
                    submit.target = _dialogueContinueButton;
                    _dialogueContinueButton.SendEvent(submit);
                }
                if (_world.DialogueActive && _dialogueLineScroll.scrollOffset.y != 0f)
                    throw new InvalidOperationException("Next dialogue line must reset reading position.");
            }
            if (!_world.DialogueCompleted || _world.DialogueActive)
                throw new InvalidOperationException("Dialogue buttons must complete all three lines.");
            if (approaching) _world.SetSmokePosition(-1.1f, 0.25f, 2.3f, 0f);
            else _world.SetSmokePositionNearTrainingStone();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
            if (approaching && (_world.CanInteract || !_interactionHint.text.Contains("đường đá")))
                throw new InvalidOperationException("Approach must show the stone route before interaction becomes available.");
            if (approaching)
            {
                if (_worldTouchPrimaryActionButton.enabledSelf || _worldTouchPrimaryActionButton.text != "Luyện")
                    throw new InvalidOperationException("Guided action must retain its purpose and disable outside interaction range.");
                var targetStatus = _world.TargetDummyStatusText;
                TriggerWorldTouchPrimaryAction();
                if (_world.TargetDummyStatusText != targetStatus || _world.InteractionAcknowledged)
                    throw new InvalidOperationException("Out-of-range guided action must not attack or complete the objective.");
            }
            if (!approaching && (!_world.CanInteract || !_interactionHint.text.Contains("Chọn Luyện")))
                throw new InvalidOperationException("Near stone guidance must name the available on-screen action.");
            if (!approaching && !_worldTouchPrimaryActionButton.enabledSelf)
                throw new InvalidOperationException("Guided action must re-enable inside interaction range.");
        }

        internal void CaptureEvidenceTrainingComplete()
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _skillPreviewActive = false;
            _world.SetSmokePositionNearTrainingStone();
            RefreshWorldLoopLabels();
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = _worldTouchPrimaryActionButton;
                _worldTouchPrimaryActionButton.SendEvent(submit);
            }
            if (!_world.InteractionAcknowledged) throw new InvalidOperationException("Primary button did not complete training.");
            if (_isMobileProfile) Debug.Log("LGO_MOBILE_GUIDED_ACTIONS_PASS keeper=dialogue stone=completed");
            RefreshWorldLoopLabels();
            if (!_world.DialogueCompleted || !_world.InteractionAcknowledged)
                throw new InvalidOperationException("Training completion requires finished dialogue and acknowledged stone.");
            var completedObjective = _world.ObjectiveText;
            if (_worldTouchPrimaryActionButton.enabledSelf || _worldTouchPrimaryActionButton.text != "Đã xong")
                throw new InvalidOperationException("Completed interaction must not turn into a duplicate combat action.");
            var combatBefore = _world.LastLocalCombatOutcome;
            TriggerWorldTouchPrimaryAction();
            if (!Equals(combatBefore, _world.LastLocalCombatOutcome))
                throw new InvalidOperationException("Completed interaction callback must not issue a combat attempt.");
            if (_world.TriggerInteractionForSmoke() || _world.ContinueDialogue() || _world.CloseDialogue() ||
                _world.ObjectiveText != completedObjective || !_world.InteractionAcknowledged)
                throw new InvalidOperationException("Repeated interaction must not restart completed guidance.");
            RefreshCombatAssetUiState();
        }

        internal async Task CaptureEvidenceSaveFromMenuAsync()
        {
            var savedPosition = _world.CurrentPosition;
            var characterId = _selectedCharacter.characterId;
            try
            {
                _selectedCharacter.characterId = "evidence-missing-character";
                using (var failedSave = NavigationSubmitEvent.GetPooled())
                {
                    failedSave.target = _sessionSaveButton;
                    _sessionSaveButton.SendEvent(failedSave);
                }
                for (var attempt = 0; attempt < 200 && !_sessionSaveButton.enabledSelf; attempt++)
                    await Task.Delay(50);
                if (!_sessionSaveButton.enabledSelf || _sessionSaveButton.text != "Thử lưu lại" ||
                    !_sessionBackButton.enabledSelf || !_status.text.Contains("lưu vị trí"))
                    throw new InvalidOperationException("Rejected save must show retry and restore menu controls.");
            }
            finally
            {
                _selectedCharacter.characterId = characterId;
            }
            SetSessionMenuVisible(false);
            SetSessionMenuVisible(true);
            if (_sessionSaveButton.text != "Lưu vị trí" || _sessionSaveButton.tooltip != "Lưu vị trí hiện tại của nhân vật.")
                throw new InvalidOperationException("Reopened menu must clear stale save rejection feedback.");
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = _sessionSaveButton;
                _sessionSaveButton.SendEvent(submit);
            }
            for (var attempt = 0; attempt < 200 && !_sessionSaveButton.enabledSelf; attempt++)
                await Task.Delay(50);
            if (!_sessionSaveButton.enabledSelf || _sessionSaveButton.text != "Đã lưu vị trí")
                throw new InvalidOperationException("Menu save must finish with visible success feedback.");
            using (var back = NavigationSubmitEvent.GetPooled())
            {
                back.target = _sessionBackButton;
                _sessionBackButton.SendEvent(back);
            }
            if (_worldHud.style.display != DisplayStyle.None)
                throw new InvalidOperationException("Menu back action did not return to character hall.");
            await EnterWorldAsync();
            if (Vector3.Distance(savedPosition, _world.CurrentPosition) > 0.01f)
                throw new InvalidOperationException("Re-entry did not restore the saved server position.");
            SetSessionMenuVisible(true);
            if (_sessionSaveButton.text != "Lưu vị trí" || _sessionSaveButton.tooltip != "Lưu vị trí hiện tại của nhân vật.")
                throw new InvalidOperationException("New menu visit must not retain previous save success feedback.");
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
            using (var close = NavigationSubmitEvent.GetPooled())
            {
                close.target = _dialogueCloseButton;
                _dialogueCloseButton.SendEvent(close);
            }
            if (_world.DialogueActive || _world.DialogueCompleted)
                throw new InvalidOperationException("Closing early must dismiss dialogue without completing the objective.");
            _world.SetSmokePositionNearTrainingStone();
            if (_world.TriggerInteractionForSmoke())
                throw new InvalidOperationException("Training stone unlocked after cancelling dialogue.");
            _world.SetSmokePositionNearGateKeeper();
            if (!_world.TriggerInteractionForSmoke() || !_world.DialogueActive || _world.DialogueProgress != "1/3")
                throw new InvalidOperationException("Cancelled dialogue must reopen at its first line.");
            RefreshWorldLoopLabels();
            if (_dialoguePanel.childCount != 3 || _dialoguePanel[0] != _dialogueSpeakerHeader ||
                _dialoguePanel[1] != _dialogueBody || _dialoguePanel[2] != _dialogueFooter)
                throw new InvalidOperationException("Dialogue must contain one speaker header, body and footer without duplicate shell titles.");
        }

        internal void CaptureEvidenceOpenLongDialogue()
        {
            CaptureEvidenceOpenDialogue();
            if (_dialogueLine == null || _dialogueProgress == null) return;
            _dialogueLine.text =
                "Linh Môn mở ra không chỉ để con bước qua, mà để thử xem tâm thức có giữ được nhịp thở giữa gió mạnh hay không. " +
                "Khi lời dẫn kéo dài, khung thoại phải giữ nguyên hình dáng, phần chữ tự cuộn trong vùng đọc, còn tiến trình và nút hành động vẫn nằm đúng vị trí để người chơi không bị mất điều khiển.";
            _dialogueProgress.text = "Đối thoại dài: kiểm tra cuộn";
        }

        internal IEnumerator CaptureEvidenceDialogueScrollAndReopen()
        {
            var paragraph = _dialogueLine.text;
            _dialogueLine.text = paragraph + "\n\n" + paragraph + "\n\n" + paragraph;
            yield return null;
            yield return null;
            var scroll = _dialogueLineScroll;
            if (scroll.verticalScroller.highValue <= 1f)
                throw new InvalidOperationException("Long dialogue must exceed its reading viewport for this scroll test.");
            scroll.verticalScroller.value = scroll.verticalScroller.highValue;
            yield return null;
            if (scroll.scrollOffset.y <= 1f)
                throw new InvalidOperationException("Dialogue scrollbar must move actual reading content.");
            if (_dialogueFooter.worldBound.yMax > _dialoguePanel.worldBound.yMax + 1f)
                throw new InvalidOperationException("Dialogue footer escaped its shell while reading long content.");
            using (var close = NavigationSubmitEvent.GetPooled())
            {
                close.target = _dialogueCloseButton;
                _dialogueCloseButton.SendEvent(close);
            }
            RefreshWorldLoopLabels();
            using (var open = NavigationSubmitEvent.GetPooled())
            {
                open.target = _worldTouchPrimaryActionButton;
                _worldTouchPrimaryActionButton.SendEvent(open);
            }
            yield return null;
            if (!_world.DialogueActive || scroll.scrollOffset.y > 0.5f || _world.DialogueProgress != "1/3")
                throw new InvalidOperationException("Reopened dialogue must start at its first line and reading origin.");
            Debug.Log("LGO_DIALOGUE_SCROLL_REOPEN_PASS input=scrollbar-programmatic reopen=button");
        }

        internal void AssertCombatPanelClearsMovementPad()
        {
            if (_localCombatPanel.worldBound.yMax + 4f > _worldTouchMovementPad.worldBound.yMin)
                throw new InvalidOperationException("Combat feedback must leave a gap above movement controls.");
            if (_localCombatPanel.Q<Button>() != null)
                throw new InvalidOperationException("Combat HUD must not expose a duplicate cast button.");
        }

        internal void CaptureEvidenceCombatOutOfRange()
        {
            _evidenceState = RuntimeUiEvidenceState.CombatPanelFocus;
            _skillPreviewActive = false;
            _world.SetSmokePosition(-4f, 0.25f, -2f, 0f);
            RefreshWorldLoopLabels();
            var hpBefore = _world.TargetDummyStatusText;
            if (_world.LocalCombatTargetInRange || _world.LocalCombatCoolingDown || !_worldTouchWindSlashButton.enabledSelf)
                throw new InvalidOperationException("Out-of-range fixture must be ready but too far from its target.");
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = _worldTouchWindSlashButton;
                _worldTouchWindSlashButton.SendEvent(submit);
            }
            var outcome = _world.LastLocalCombatOutcome;
            if (outcome == null || outcome.Accepted || outcome.RejectedReason != "OUT_OF_RANGE" ||
                hpBefore != _world.TargetDummyStatusText || _world.LocalCombatCoolingDown)
                throw new InvalidOperationException("Out-of-range cast must reject without damage or cooldown.");
            if (!_worldTouchWindSlashButton.text.Contains("Lại gần") || !_combatFeedback.text.Contains("Ngoài tầm"))
                throw new InvalidOperationException("Out-of-range feedback must explain the next action on button and HUD.");
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
            if (_worldTouchWindSlashButton.enabledSelf || !_worldTouchWindSlashButton.text.Contains("\n"))
                throw new InvalidOperationException("Wind Slash button must show cooldown and disable after casting.");
        }

        internal IEnumerator CaptureEvidenceCooldownRecoversAtRest()
        {
            var position = _world.CurrentPosition;
            var first = _world.LastLocalCombatOutcome;
            var deadline = Time.realtimeSinceStartup + 10f;
            while (_world.LocalCombatCoolingDown && Time.realtimeSinceStartup < deadline)
                yield return null;
            yield return null;
            if (_world.LocalCombatCoolingDown || !_worldTouchWindSlashButton.enabledSelf || _worldTouchWindSlashButton.text != "Chém")
                throw new InvalidOperationException("Wind Slash must become ready without movement or forced cooldown reset.");
            if (Vector3.Distance(position, _world.CurrentPosition) > 0.01f)
                throw new InvalidOperationException("Cooldown recovery fixture must remain stationary.");
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = _worldTouchWindSlashButton;
                _worldTouchWindSlashButton.SendEvent(submit);
            }
            var second = _world.LastLocalCombatOutcome;
            if (second == null || !second.Accepted || Equals(first, second) || !_world.LocalCombatCoolingDown)
                throw new InvalidOperationException("Ready button must execute another accepted cast.");
            Debug.Log("LGO_WIND_SLASH_RECOVERY_PASS idle=true time=natural second_cast=accepted");
        }

        internal void CaptureEvidenceShadowBindPreview()
        {
            if (_world == null) return;
            _evidenceState = RuntimeUiEvidenceState.None;
            _world.ResetLocalCombatPreviewStateForSmoke();
            PreviewSkill("Shadow Bind", "Trói Bóng");
            RefreshCombatAssetUiState();
        }

        internal IEnumerator CaptureEvidenceOpenSessionMenu()
        {
            if (_world.DialogueActive) CloseDialogue();
            SetSessionMenuVisible(true);
            var position = _world.CurrentPosition;
            var yaw = _world.BuildSaveRequest().yawDegrees;
            // SendMessage would also tick the UI controller on this GameObject and overwrite injected input.
            var worldTick = _world.GetType().GetMethod("Update", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var outcome = _world.LastLocalCombatOutcome;
            _world.TouchMovement = Vector2.one;
            worldTick.Invoke(_world, null);
            TriggerLocalCombat();
            if (Vector3.Distance(position, _world.CurrentPosition) > 0.001f || !Equals(outcome, _world.LastLocalCombatOutcome))
                throw new InvalidOperationException("Menu focus must block movement and combat callbacks.");
            SetSessionMenuVisible(false);
            if (_world.TouchMovement != Vector2.zero)
                throw new InvalidOperationException("Closing menu must discard stale movement.");
            _world.TouchMovement = Vector2.right;
            worldTick.Invoke(_world, null);
            if (Vector3.Distance(position, _world.CurrentPosition) > 0.001f)
                throw new InvalidOperationException("Closing frame must not leak input into the world.");
            _world.TouchMovement = Vector2.zero;
            yield return null;
            yield return null;
            _world.TouchMovement = Vector2.right;
            worldTick.Invoke(_world, null);
            _world.TouchMovement = Vector2.zero;
            if (Vector3.Distance(position, _world.CurrentPosition) <= 0.001f)
                throw new InvalidOperationException("Fresh movement must resume after closing menu.");
            _world.SetSmokePosition(position.x, position.y, position.z, yaw);
            SetSessionMenuVisible(true);
            Debug.Log("LGO_MENU_INPUT_FOCUS_PASS movement=injected combat=callback");
            Debug.Log("LGO_MENU_INPUT_RESUME_PASS stale=cleared closing_frame=blocked fresh_input=moved");
            var focusCallback = _world.GetType().GetMethod("OnApplicationFocus", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            foreach (var focused in new[] { false, true })
            {
                _world.TouchMovement = Vector2.one;
                focusCallback.Invoke(_world, new object[] { focused });
                if (_world.TouchMovement != Vector2.zero)
                    throw new InvalidOperationException("World focus callback must clear movement without waiting for Update.");
                _world.TouchMovement = Vector2.one;
                OnApplicationFocus(focused);
                if (_world.TouchMovement != Vector2.zero)
                    throw new InvalidOperationException("UI focus callback must clear pending movement without waiting for Update.");
            }
            Debug.Log("LGO_APPLICATION_FOCUS_RESET_PASS callback=simulated loss=cleared gain=cleared");
        }
    }
}
