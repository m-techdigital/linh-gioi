using System;
using LinhGioi.Foundation;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    // Runtime layout/input/evidence orchestration. Hierarchy construction and product data binding stay in sibling partials.
    public sealed partial class CongDongLamArrivalHud
    {
        public static float CalculateInventoryShellHeight(Rect inventoryRect, bool touch, bool compactShell)
        {
            return Mathf.Min(inventoryRect.height, InventoryCanonicalShellHeight);
        }

        public static string QuestInteractionMessageForDisplay(string message, string objective, string progress)
        {
            var normalized = (message ?? "").Trim();
            if (normalized.Length == 0) return "";
            if (string.Equals(normalized, (objective ?? "").Trim(), StringComparison.Ordinal)
                || string.Equals(normalized, (progress ?? "").Trim(), StringComparison.Ordinal)) return "";
            return normalized;
        }

        public static bool ShouldBlockWorldInput(bool entryOpen, bool menuOpen, bool inventoryOpen,
            bool dialogueOpen, bool characterSelectOpen, bool serverSelectOpen = false)
        {
            return entryOpen || menuOpen || inventoryOpen || dialogueOpen || characterSelectOpen || serverSelectOpen;
        }

        public static bool ShouldEnableReviewHotkeysForArgs(string[] args)
        {
            return args != null && Array.IndexOf(args, "--lgo-map01a-review-hotkeys") >= 0;
        }

        private void HandleEscape()
        {
            if (_passwordRecoveryOpen) ClosePasswordRecovery();
            else if (_registerOpen) CloseRegister();
            else if (_serverSelectOpen) CloseServerSelect(false);
            else if (_menuOpen) CloseMenu();
            else if (_characterSelectOpen) CloseCharacterSelect();
            else if (_scene.DialogueOpen) _scene.CloseNpcDialogue();
            else if (_scene.InventoryOpen) _scene.ToggleInventory();
        }

        public static Rect CalculateInventoryModalRect(Rect safePanelRect, bool touch,
            float maximumShellHeight = InventoryCanonicalShellHeight)
        {
            const float minimumMargin = 24f;
            const float opticalVerticalOffset = 18f;
            var availableWidth = Mathf.Max(0f, safePanelRect.width - minimumMargin * 2f);
            var availableHeight = Mathf.Max(0f, safePanelRect.height - minimumMargin * 2f);
            var profileScale = InventoryCanonicalShellHeight <= 0f
                ? 0f
                : Mathf.Clamp(maximumShellHeight / InventoryCanonicalShellHeight, 0f, 1f);
            var fitScale = Mathf.Min(
                InventoryCanonicalShellWidth <= 0f ? 0f : availableWidth / InventoryCanonicalShellWidth,
                InventoryCanonicalShellHeight <= 0f ? 0f : availableHeight / InventoryCanonicalShellHeight);
            var shellScale = Mathf.Min(profileScale, fitScale);
            var width = InventoryCanonicalShellWidth * shellScale;
            var height = InventoryCanonicalShellHeight * shellScale;
            var x = safePanelRect.x + (safePanelRect.width - width) * .5f;
            var centeredY = (safePanelRect.height - height) * .5f + opticalVerticalOffset;
            var y = safePanelRect.y + Mathf.Clamp(centeredY, minimumMargin,
                Mathf.Max(minimumMargin, safePanelRect.height - height - minimumMargin));
            return new Rect(x, y, width, height);
        }

        public static Vector2 CalculateInventoryDesktopColumnWidths()
        {
            return new Vector2(InventoryDesktopMainColumnWidth, InventoryDesktopDetailColumnWidth);
        }

        public static Vector3 CalculateInventoryColumnWidths(float shellWidth)
        {
            var available = Mathf.Max(0f, shellWidth - InventoryCanonicalBodyHorizontalInset);
            var canonical = InventoryDesktopMainColumnWidth + InventoryDesktopDetailColumnWidth + InventoryDesktopColumnGap;
            var scale = canonical <= 0f ? 0f : Mathf.Min(1f, available / canonical);
            return new Vector3(
                InventoryDesktopMainColumnWidth * scale,
                InventoryDesktopDetailColumnWidth * scale,
                InventoryDesktopColumnGap * scale);
        }

        internal static Vector3 CalculateInventoryColumnWidths(
            float shellWidth, RuntimeCharacterHubLayoutVariant variant)
        {
            return variant.CalculateColumns(shellWidth);
        }

        private void Layout()
        {
            _metrics = RuntimeViewportMetrics.FromRoot(_root, _forcedLayoutProfile);
            var layout = RuntimeUiLayoutProfile.FromViewport(_metrics);
            var r = _metrics.SafePanelRect;
            Place(_safe, r.x, null, r.y, null); _safe.style.width = r.width; _safe.style.height = r.height;
            LayoutEntryScreen(layout, r);
            var gameplayHud = RuntimeGameplayHudLayout.Calculate(new Rect(0, 0, r.width, r.height), layout);
            ApplyLgoGameplayHudRect(_playerHudCluster, gameplayHud.PlayerStatus);
            ApplyLgoGameplayHudRect(_rightHudCluster, gameplayHud.RightInfo);
            ApplyLgoGameplayHudRect(_combatBar, gameplayHud.Combat);
            ApplyLgoGameplayHudRect(_talk, gameplayHud.Context);
            ApplyLgoGameplayHudRect(_npcTalk, gameplayHud.Context);
            ApplyLgoGameplayHudRect(_pad, gameplayHud.TouchPad);
            ApplyLgoGameplayHudRect(_productShortcutActions, gameplayHud.SecondaryNav);
            _vitals.style.width = gameplayHud.PlayerStatus.width;
            var rightColumnWidth = gameplayHud.RightInfo.width;
            _quest.style.width = rightColumnWidth;
            _questTabs.style.width = rightColumnWidth;
            _minimap.style.width = rightColumnWidth;
            ApplyLgoGameplayHudRect(_dialogue, gameplayHud.Dialogue);
            var inventoryRect = CalculateInventoryModalRect(
                new Rect(0, 0, r.width, r.height), _touch, layout.CharacterHubShellMaxHeight);
            _inventory.style.left = inventoryRect.x;
            _inventory.style.right = StyleKeyword.Auto;
            _inventory.style.top = inventoryRect.y;
            _inventory.style.bottom = StyleKeyword.Auto;
            _inventory.style.width = inventoryRect.width;
            _inventory.style.height = CalculateInventoryShellHeight(inventoryRect, _touch, IsInventoryCompactShellActive());
            PublishRuntimeUiMetrics(layout, inventoryRect, gameplayHud);
            _talk.style.fontSize = layout.WorldTalkFontSize;
            if (_inventoryHeroPanel != null)
            {
                var body = _inventory.Q("Map01A Inventory Body");
                var variant = layout.CharacterHubVariant;
                var columns = CalculateInventoryColumnWidths(inventoryRect.width, variant);
                body.style.flexDirection = FlexDirection.Row;
                ApplyLgoCharacterHubMainTabsLayout(
                    _inventory.Q("Map01A Inventory Main Tabs"), inventoryRect.width, variant);
                _inventoryHeroPanel.style.flexBasis = columns.x;
                _inventoryHeroPanel.style.marginRight = 0;
                _inventoryHeroPanel.style.marginBottom = 0;
                _inventoryGridPanel.style.flexBasis = columns.x;
                _inventoryGridPanel.style.marginRight = 0;
                _inventoryGridPanel.style.marginBottom = 0;
                _inventoryDetailPanel.style.flexBasis = columns.y;
                _inventoryDetailPanel.style.marginLeft = columns.z;
                _inventoryDetailPanel.style.marginRight = 0;
                _inventoryDetailPanel.style.marginBottom = 0;
                if (_skillsPanel != null)
                {
                    _skillsPanel.style.flexBasis = columns.x;
                    _skillsPanel.style.marginBottom = 0;
                    _potentialPanel.style.flexBasis = columns.x;
                    _potentialPanel.style.marginBottom = 0;
                    _spiritPetPanel.style.flexBasis = columns.x;
                    _spiritPetPanel.style.marginBottom = 0;
                    _hubPreviewDetailPanel.style.flexBasis = columns.y;
                    _hubPreviewDetailPanel.style.marginLeft = columns.z;
                    _hubPreviewDetailPanel.style.marginBottom = 0;
                }
            }
        }
        private void PublishRuntimeUiMetrics(RuntimeUiLayoutProfile layout, Rect characterHubShellRect,
            RuntimeGameplayHudLayout gameplayHud)
        {
            if (_scene == null) return;
            var authority = Application.isMobilePlatform
                ? "device-runtime-unverified"
                : layout.InputClass == "touch" ? "macos-aspect-simulation" : "macos-player";
            _scene.SetRuntimeUiMetrics(RuntimeUiEvidenceMetrics.CreateGameplaySnapshot(
                _metrics.ScreenPixelWidth, _metrics.ScreenPixelHeight, _metrics.PanelWidth, _metrics.PanelHeight,
                _metrics.SafePanelRect, characterHubShellRect, layout.Name, layout.InputClass,
                RuntimePanelSettingsProvider.Describe(_ownedPanel), authority,
                RuntimeUiTheme.Current.minimumTouchTarget, gameplayHud));
        }

        private void Update()
        {
            if (_scene == null || _root == null) return;
            var metrics = RuntimeViewportMetrics.FromRoot(_root, _forcedLayoutProfile);
            if (!_metrics.LayoutEquals(metrics)) Layout();
            UpdatePasswordRecoveryCooldown();
            if (!_scene.IsCapturing)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    HandleEscape();

                var worldInputBlocked = ShouldBlockWorldInput(
                    _entryOpen, _menuOpen, _scene.InventoryOpen, _scene.DialogueOpen, _characterSelectOpen,
                    _serverSelectOpen);
                if (!worldInputBlocked)
                {
                    var keyboard = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
                        - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                    if (!_touch) _scene.SetCharacterRun(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    _scene.MoveOnLane(Mathf.Abs(_pad.Value.x) > .01f ? _pad.Value.x : keyboard, Time.deltaTime);
                    if (Input.GetKeyDown(KeyCode.E)) _scene.UseCurrentRouteAction();
                    if (_reviewHotkeysEnabled)
                    {
                        if (!_scene.IsSourcePoseReviewActive && Input.GetKeyDown(KeyCode.C)) _scene.CycleCharacterAvatarMode();
                        if (Input.GetKeyDown(KeyCode.L)) _scene.CycleCharacterLevel();
                        if (Input.GetKeyDown(KeyCode.G)) _scene.CycleCharacterGender();
                        if (Input.GetKeyDown(KeyCode.V)) _scene.CycleEquipmentSlot();
                        if (Input.GetKeyDown(KeyCode.M)) _scene.CycleSelectedEquipmentItemLevel();
                        if (Input.GetKeyDown(KeyCode.B)) _scene.ToggleEquipmentSlot();
                        if (Input.GetKeyDown(KeyCode.F)) _scene.CycleSourcePoseClass();
                    }
                    if (Input.GetKeyDown(KeyCode.X)) _scene.TriggerCharacterSkill();
                    _scene.SetCharacterJumpHeld(_touchJumpHeld || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));
                    if (Input.GetKeyDown(KeyCode.Z)) _scene.TriggerCharacterBasicAttack();
                    if (Input.GetKeyDown(KeyCode.I)) _scene.ToggleInventory();
                    if (Input.GetKeyDown(KeyCode.H)) _scene.UseHealthPotion();
                    if (Input.GetKeyDown(KeyCode.K)) _scene.UseManaPotion();
                    if (Input.GetKeyDown(KeyCode.R)) _scene.EquipClassReward();
                }
                else
                {
                    if (!_touch) _scene.SetCharacterRun(false);
                    _scene.SetCharacterJumpHeld(false);
                }
            }
            _questTitle.text = _scene.QuestDisplayTitle;
            _questObjective.text = _scene.QuestObjectiveText;
            _questProgress.text = _scene.QuestProgressText;
            _questMessage.text = QuestInteractionMessageForDisplay(
                _scene.LastInteractionMessage, _scene.QuestObjectiveText, _scene.QuestProgressText);
            _questMessage.style.display = _scene.DialogueOpen || string.IsNullOrEmpty(_questMessage.text)
                ? DisplayStyle.None
                : DisplayStyle.Flex;
            _talk.SetEnabled(_scene.CanUseCurrentRouteAction);
            _talk.text = _scene.CurrentActionLabel + (_touch ? "" : " · E");
            _outfit.text = "Trang bị " + _scene.AvatarClassLabel + ": " + _scene.CharacterAvatarMode + (_touch ? "" : " · C");
            _level.text = "Cấp đồ: " + _scene.EquipmentLevelLabel + (_touch ? "" : " · L");
            _gender.text = "Thân: " + _scene.CharacterGender + (_touch ? "" : " · G");
            _slot.text = "Slot: " + _scene.EquipmentSlotLabel + (_touch ? "" : " · V");
            _itemLevel.text = "Đổi cấp item" + (_touch ? "" : " · M");
            _toggleSlot.text = (_scene.EquippedSlotCount == 10 ? "Cởi slot" : "Mặc/cởi") + (_touch ? "" : " · B");
            _run.text = _touch ? "" : "SHIFT";
            _jump.text = _touch ? "" : "W";
            _basic.text = _touch ? "" : "Z";
            _skill.text = ActiveCharacterHubProfile.Skills[0].Name + (_touch ? "" : " · X");
            _skill.SetEnabled(_scene.CanTriggerCharacterSkill);
            _minimapTitle.text = _scene.MinimapUnlocked ? "BẢN ĐỒ ĐÔNG LÂM" : "BẢN ĐỒ KHU VỰC";
            _minimapStatus.text = _scene.MinimapUnlocked ? "Đang ở: " + _scene.CurrentRouteNodeLabel : "Hoàn thành Q02 để mở tuyến đường";
            _minimapCurrentMarker.style.display = _scene.MinimapUnlocked ? DisplayStyle.Flex : DisplayStyle.None;
            _minimapCurrentMarker.style.left = new Length(7f + 86f * Mathf.Clamp01(_scene.CurrentRouteProgress), LengthUnit.Percent);
            _minimap.style.opacity = _scene.MinimapUnlocked ? 1f : .74f;
            _minimap.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _inventory.style.display = _scene.InventoryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            if (_inventoryBackdrop != null)
                _inventoryBackdrop.style.display = _scene.InventoryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateCharacterHubOpenAnimation(_scene.InventoryOpen);
            _quest.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _questTabs.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            foreach (var control in new[] { _outfit, _level, _gender, _slot, _itemLevel, _toggleSlot })
                control.style.display = DisplayStyle.None;
            _inventorySummary.text = _scene.InventorySummaryText;
            _inventoryHeroTitle.text = _scene.ActiveEquipmentClassLabel + " · " + (_scene.CharacterGender == "female" ? "Nữ" : "Nam");
            _inventoryHeroMeta.text = "HP " + _scene.PlayerHealth + "/100  ·  MP " + _scene.PlayerMana + "/100";
            _equipmentTitle.text = "TRANG BỊ · " + _scene.EquippedSlotCount + "/10 món đang mặc";
            if (_suppliesEmptyState != null)
                _suppliesEmptyState.text = _scene.HealthPotionCount <= 0 && _scene.ManaPotionCount <= 0 && !_scene.HasClassRewardItem
                    ? "Chưa nhận vật phẩm nhiệm vụ. Hoàn thành Q04 để nhận bình máu, bình linh lực và hộ uyển tân thủ."
                    : "Chọn vật phẩm để dùng hoặc trang bị. Vật phẩm chưa đủ điều kiện sẽ tạm khóa nhưng vẫn đọc rõ trạng thái.";
            _questItemActions.style.display = DisplayStyle.Flex;
            RefreshInventoryEquipmentTiles();
            RefreshInventoryDetailCard();
            var selectedSlot = _scene.SelectedEquipmentSlot;
            var selectedEquipped = _scene.IsEquipmentSlotEquipped(selectedSlot);
            var equipToggleText = selectedEquipped ? "Tháo" : "Trang bị";
            if (!_suppliesOpen)
            {
                _inventoryDetailPrimaryAction.text = equipToggleText;
                _equipmentToggle.text = equipToggleText;
                var hasVariant = _scene.HasEquipmentItemVariant(_scene.SelectedEquipmentSlot);
                _equipmentVariant.text = hasVariant ? "Đổi cấp món" : "";
                _equipmentVariant.style.display = hasVariant ? DisplayStyle.Flex : DisplayStyle.None;
                _equipmentVariant.SetEnabled(hasVariant);
            }
            _equipmentClass.style.display = _scene.CanCycleCharacterHubClass ? DisplayStyle.Flex : DisplayStyle.None;
            _equipmentClass.text = "Class · " + _scene.ActiveEquipmentClassLabel + "  ›";
            _equipmentClass.SetEnabled(_scene.CanCycleCharacterHubClass);
            RefreshCharacterHubClassProfile();
            _inventoryGender.style.display = _scene.CanCycleSourcePoseGender ? DisplayStyle.Flex : DisplayStyle.None;
            _inventoryGender.text = "Đổi giới · "
                + (_scene.CharacterGender == "female" ? "Nữ" : "Nam") + (_touch ? "" : " · G");
            _inventoryGender.SetEnabled(_scene.CanCycleSourcePoseGender);
            _vitals.style.display = _scene.InventoryOpen || _scene.DialogueOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _vitalsName.text = string.IsNullOrWhiteSpace(_loadedProductCharacterName) ? "LụcThiên" : _loadedProductCharacterName;
            _vitalsMeta.text = _scene.ActiveEquipmentClassLabel + " · " + (_scene.CharacterGender == "female" ? "Nữ" : "Nam") + "  ·  Lv.1";
            var playerPortrait = _scene.GetCharacterAvatarThumbnailSprite();
            _vitalsPortrait.style.backgroundImage = playerPortrait == null ? StyleKeyword.None : new StyleBackground(playerPortrait);
            _health.value = _scene.PlayerHealth; _health.title = "HP " + _scene.PlayerHealth + "/100";
            _mana.value = _scene.PlayerMana; _mana.title = "MP " + _scene.PlayerMana + "/100";
            foreach (var supplyAction in new[] { _healthPotion, _manaPotion, _equipReward })
                supplyAction.SetEnabled(true);
            RefreshInventorySupplyRows();
            _dialogue.style.display = _scene.DialogueOpen ? DisplayStyle.Flex : DisplayStyle.None;
            var hudBlocked = _scene.DialogueOpen || _scene.InventoryOpen || _characterSelectOpen;
            _talk.style.display = hudBlocked || !_scene.CanUseCurrentRouteAction ? DisplayStyle.None : DisplayStyle.Flex;
            _productShortcutActions.style.display = hudBlocked ? DisplayStyle.None : DisplayStyle.Flex;
            _combatBar.style.display = hudBlocked ? DisplayStyle.None : DisplayStyle.Flex;
            _pad.style.display = _touch && !hudBlocked ? DisplayStyle.Flex : DisplayStyle.None;
            _dialogueContinue.style.display = _scene.DialogueOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _dialogueContinue.SetEnabled(_scene.CanUseCurrentRouteAction);
            _dialogueContinue.text = "Tiếp tục" + (_touch ? "" : " · E");
            _dialogueInformation.style.display = _scene.CanReadDialogueInformation ? DisplayStyle.Flex : DisplayStyle.None;
            _npcTalk.style.display = !_scene.DialogueOpen && _scene.CanTalkToCurrentNpc
                && _scene.CurrentRouteNodeId == "well-bridge" && _scene.CurrentActionLabel != "Trò chuyện"
                ? DisplayStyle.Flex : DisplayStyle.None;
            _talk.style.width = _scene.DialogueOpen ? new StyleLength(260) : new StyleLength(StyleKeyword.Auto);
            _dialogueSpeaker.text = _scene.DialogueSpeaker;
            _dialogueQuestContext.text = DialogueQuestContextText();
            _dialogueLine.text = _scene.DialogueText;
            var dialoguePortrait = _scene.GetCurrentDialogueNpcSprite();
            _dialoguePortrait.style.backgroundImage = dialoguePortrait == null ? StyleKeyword.None : new StyleBackground(dialoguePortrait);
            _marker.text = "!\n" + _scene.CurrentRouteNodeLabel;
            _marker.style.display = _scene.DialogueOpen || _scene.InventoryOpen || _characterSelectOpen ? DisplayStyle.None : DisplayStyle.Flex;
            UpdateHudShellVisibility();
            var camera = Camera.main;
            if (camera != null)
            {
                var v = camera.WorldToViewportPoint(_scene.CurrentInteractionPosition);
                _marker.style.left = v.x * _metrics.PanelWidth - 120;
                _marker.style.top = (1-v.y) * _metrics.PanelHeight - 64;
            }
        }

    }
}
