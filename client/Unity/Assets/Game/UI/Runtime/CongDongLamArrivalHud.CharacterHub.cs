using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private enum CharacterHubMode { Skills, Potential, SpiritPet }

        private VisualElement _skillsPanel, _potentialPanel, _spiritPetPanel, _hubPreviewDetailPanel, _hubSpiritPetBadges;
        private VisualElement _hubSkillActionRow, _hubPotentialActionRow, _hubSpiritPetActionRow;
        private Label _hubDetailHeader, _hubDetailName, _hubDetailMeta, _hubDetailBody, _hubDetailStatus;
        private VisualElement _hubDetailIcon;
        private Button _hubSkillUpgradeAction, _hubSkillEquipAction, _potentialAddPointAction, _potentialResetAction, _spiritPetDeployAction, _spiritPetDevelopAction;
        private Texture2D _spiritPetPreviewTexture;
        private readonly List<Button> _skillPathNodes = new List<Button>();
        private readonly List<Button> _potentialPathNodes = new List<Button>();

        private VisualElement CreateHubSurface(string name)
        {
            var panel = InventoryPanel(name);
            panel.style.flexGrow = 0;
            panel.style.flexBasis = InventoryDesktopMainColumnWidth;
            panel.style.minHeight = 0;
            return panel;
        }

        private VisualElement HubIcon(string name, string iconId, float size)
        {
            var icon = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            ApplyLgoItemIcon(icon);
            icon.style.width = size;
            icon.style.height = size;
            icon.style.marginTop = 0;
            icon.style.marginBottom = 0;
            icon.style.marginLeft = 0;
            icon.style.marginRight = 0;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            var sprite = _scene.GetMap01AHudIconSprite(iconId);
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            return icon;
        }

        private VisualElement SkillIcon(string name, string iconId, float size)
        {
            var icon = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            ApplyLgoSkillIcon(icon, size);
            var sprite = _scene.GetMap01ASkillIconSprite(iconId);
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            return icon;
        }

        private VisualElement CreateEquippedSkillSlot(string iconId, int index)
        {
            var slot = new VisualElement { name = "Map01A Equipped Skill Slot " + index };
            ApplyLgoEquippedSkillSlot(slot);
            slot.Add(SkillIcon("Map01A Equipped Skill " + iconId, iconId, 58));
            var number = LgoLabel(index.ToString(), 11, UiGold, true);
            number.name = "Map01A Equipped Skill Number " + index;
            number.style.position = Position.Absolute;
            number.style.left = 0;
            number.style.top = 0;
            number.style.paddingLeft = number.style.paddingRight = 4;
            number.style.backgroundColor = new Color(.005f, .018f, .035f, .94f);
            slot.Add(number);
            return slot;
        }

        private VisualElement PotentialIcon(string name, string iconId, float size)
        {
            var icon = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            ApplyLgoSkillIcon(icon, size);
            var sprite = _scene.GetMap01APotentialIconSprite(iconId);
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            return icon;
        }

        private Button CreateHubTile(string name, string title, string subtitle, string iconId, Action action = null)
        {
            var button = InventoryButton(action ?? (() => { }), name);
            ApplyLgoInventoryGridCell(button);
            button.style.flexBasis = new Length(29.5f, LengthUnit.Percent);
            button.style.height = 112;
            button.style.flexDirection = FlexDirection.Column;
            button.style.alignItems = Align.Center;
            button.Add(HubIcon(name + " Icon", iconId, 58));
            var label = LgoLabel(title, 12, UiText, true);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.Add(label);
            var meta = LgoLabel(subtitle, 10, UiSubText);
            meta.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.Add(meta);
            return button;
        }

        private Button CreateHubRailControl(string name, string text, string iconId, bool selected)
        {
            var button = InventoryButton(() => { }, name);
            ApplyLgoSkillCategoryCard(button, _touch);
            button.Add(SkillIcon(name + " Icon", iconId, 72));
            var label = LgoLabel(text, 14, UiText, true);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.Add(label);
            ApplyLgoSelectedTab(button, selected);
            if (!selected) ApplyLgoDisabledAction(button);
            return button;
        }

        private Button CreateHubPathNode(string name, string title, string level, string iconId, Action action,
            bool selected = false, bool useSkillArt = false)
        {
            var node = InventoryButton(action, name);
            ApplyLgoSkillNode(node);
            node.tooltip = title + " · " + level;
            node.Add(useSkillArt ? SkillIcon(name + " Icon", iconId, 88) : HubIcon(name + " Icon", iconId, 64));
            var titleLabel = LgoLabel(title, 12, UiText, true);
            titleLabel.name = name + " Title";
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            titleLabel.style.marginTop = 2;
            titleLabel.style.whiteSpace = WhiteSpace.NoWrap;
            titleLabel.style.display = useSkillArt ? DisplayStyle.None : DisplayStyle.Flex;
            node.Add(titleLabel);
            var levelLabel = LgoLabel(level, 10, new Color(.74f, .92f, 1f, .92f), true);
            levelLabel.name = name + " Level";
            levelLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            levelLabel.style.position = useSkillArt ? Position.Absolute : Position.Relative;
            if (useSkillArt)
            {
                levelLabel.style.bottom = -7;
                levelLabel.style.backgroundColor = new Color(.01f, .035f, .06f, .94f);
                levelLabel.style.paddingLeft = levelLabel.style.paddingRight = 7;
            }
            node.Add(levelLabel);
            ApplyHubPathNodeSelection(node, selected);
            return node;
        }


        private static void ApplyHubPathNodeSelection(Button node, bool selected)
        {
            ApplyLgoCharacterHubSelectionState(node, selected);
            var title = node.Q<Label>(node.name + " Title");
            if (title != null) title.style.color = selected ? UiGold : UiText;
        }

        private static VisualElement CreateHubPathConnector(string name, bool vertical = false)
        {
            var connector = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            connector.style.flexShrink = 0;
            connector.style.alignSelf = Align.Center;
            connector.style.backgroundColor = new Color(.20f, .58f, .82f, .68f);
            connector.style.width = vertical ? 2 : 44;
            connector.style.height = vertical ? 12 : 2;
            return connector;
        }

        private VisualElement CreatePotentialNode(string name, string title, string value, string iconId,
            float left, float top, bool selected = false)
        {
            var node = InventoryButton(() => SelectPotentialNode(name, title, value, iconId), name);
            ApplyLgoPotentialNode(node);
            node.Add(PotentialIcon(name + " Icon", iconId, 82));
            var titleLabel = LgoLabel(title, 12, UiText, true);
            titleLabel.name = name + " Title";
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            node.Add(titleLabel);
            var valueLabel = LgoLabel(value, 10, new Color(.74f, .92f, 1f, .92f), true);
            valueLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            node.Add(valueLabel);
            var addMarker = LgoLabel("+", 18, UiGold, true);
            addMarker.name = "Map01A Potential Node Add " + title;
            ApplyLgoPotentialAddMarker(addMarker);
            node.Add(addMarker);
            ApplyHubPathNodeSelection(node, selected);
            _potentialPathNodes.Add(node);
            node.style.position = Position.Absolute;
            node.style.left = left;
            node.style.top = top;
            return node;
        }

        private VisualElement CreateSpiritPetRosterEntry(string name, string title, string level, bool selected, int lockedIndex)
        {
            var card = InventoryButton(() => ShowHubDetail(CharacterHubMode.SpiritPet), name);
            ApplyLgoInventoryGridCell(card);
            card.style.flexBasis = new Length(23, LengthUnit.Percent);
            card.style.height = 104;
            card.style.flexDirection = FlexDirection.Column;
            card.style.alignItems = Align.Center;
            card.style.justifyContent = Justify.Center;
            var art = new VisualElement
            {
                name = selected ? "Map01A Spirit Pet Selected Roster Art" : "Map01A Spirit Pet Locked Roster " + lockedIndex,
                pickingMode = PickingMode.Ignore
            };
            ApplyLgoItemIcon(art);
            art.style.width = 58;
            art.style.height = 58;
            art.style.marginTop = 0;
            art.style.marginBottom = 2;
            art.style.unityBackgroundScaleMode = selected ? ScaleMode.ScaleAndCrop : ScaleMode.ScaleToFit;
            art.style.backgroundImage = selected && _spiritPetPreviewTexture != null
                ? new StyleBackground(_spiritPetPreviewTexture)
                : new StyleBackground(_scene.GetMap01AHudIconSprite("lock"));
            card.Add(art);
            var titleLabel = LgoLabel(title, 11, selected ? UiGold : UiSubText, true);
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(titleLabel);
            var stateLabel = LgoLabel(level, 10, selected ? new Color(.76f, 1f, .70f, .94f) : new Color(.56f, .64f, .68f, .78f));
            stateLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(stateLabel);
            ApplyLgoCharacterHubSelectionState(card, selected);
            if (!selected) ApplyLgoDisabledAction(card);
            return card;
        }

        private UnityEngine.UIElements.ProgressBar CreateSpiritPetProgress(string name, string title, float value, Color fill)
        {
            var bar = new UnityEngine.UIElements.ProgressBar { name = name, title = title, lowValue = 0, highValue = 600, value = value };
            ApplyLgoVitalBar(bar, fill);
            bar.style.marginTop = 4;
            bar.style.marginBottom = 2;
            return bar;
        }

        private void InitializeCharacterHub(VisualElement body)
        {
            InitializeSkillsView(body);
            InitializePotentialView(body);
            InitializeSpiritPetView(body);
            InitializeHubInspector(body);
            HideCharacterHubPreviewPanels();
        }

        private void InitializeSkillsView(VisualElement body)
        {
            _skillsPanel = CreateHubSurface("Map01A Skills Panel");
            var content = new VisualElement { name = "Map01A Skills Workspace" };
            content.style.flexDirection = FlexDirection.Column;
            content.style.flexGrow = 1;
            _skillsPanel.Add(content);

            var progression = new VisualElement { name = "Map01A Skills Progression Area" };
            progression.style.flexDirection = FlexDirection.Row;
            progression.style.flexGrow = 1;
            progression.style.minHeight = 0;
            content.Add(progression);

            var rail = new VisualElement { name = "Map01A Skills Category Rail" };
            rail.style.flexDirection = FlexDirection.Column;
            rail.style.width = 116;
            rail.style.marginRight = 12;
            rail.Add(CreateHubRailControl("Map01A Active Skills Category", "Chủ động", "category_active", true));
            rail.Add(CreateHubRailControl("Map01A Passive Skills Category", "Bị động", "category_passive", false));
            rail.Add(CreateHubRailControl("Map01A Method Skills Category", "Tâm pháp", "category_method", false));
            progression.Add(rail);

            var skillArea = new VisualElement { name = "Map01A Skills Grid Area" };
            skillArea.style.flexGrow = 1;
            skillArea.style.minWidth = 0;
            var path = new VisualElement { name = "Map01A Skill Progression Path" };
            path.style.flexGrow = 1;
            path.style.alignItems = Align.Center;
            var stages = new[]
            {
                new[] { ("Thiên Kiếm Quyết", "Lv.8", "thien_kiem_quyet"), ("Lăng Không Bộ", "Lv.5", "lang_khong_bo"), ("Kiếm Vũ", "Lv.4", "kiem_vu") },
                new[] { ("Hộ Thể", "Lv.3", "ho_the"), ("Song Kiếm", "Lv.6", "song_kiem"), ("Phong Trảm", "Lv.2", "phong_tram") },
                new[] { ("Kiếm Trận", "Lv.1", "kiem_tran"), ("Ngự Kiếm", "Lv.3", "ngu_kiem"), ("Vạn Kiếm", "Lv.1", "van_kiem") }
            };
            for (var stageIndex = 0; stageIndex < stages.Length; stageIndex++)
            {
                var stage = new VisualElement { name = "Map01A Skill Path Stage " + (stageIndex + 1) };
                stage.style.flexDirection = FlexDirection.Row;
                stage.style.alignItems = Align.Center;
                stage.style.justifyContent = Justify.Center;
                stage.style.width = new Length(100, LengthUnit.Percent);
                for (var nodeIndex = 0; nodeIndex < stages[stageIndex].Length; nodeIndex++)
                {
                    var skill = stages[stageIndex][nodeIndex];
                    var nodeName = "Map01A Skill Node " + skill.Item1;
                    var node = CreateHubPathNode(nodeName, skill.Item1, skill.Item2, skill.Item3,
                        () => SelectSkillNode(nodeName, skill.Item1, skill.Item2, skill.Item3),
                        stageIndex == 0 && nodeIndex == 0, true);
                    _skillPathNodes.Add(node);
                    stage.Add(node);
                    if (nodeIndex < stages[stageIndex].Length - 1)
                        stage.Add(CreateHubPathConnector("Map01A Skill Stage " + (stageIndex + 1) + " Connector " + (nodeIndex + 1)));
                }
                path.Add(stage);
                if (stageIndex < stages.Length - 1)
                    path.Add(CreateHubPathConnector("Map01A Skill Path Connector " + (stageIndex + 1), true));
            }
            skillArea.Add(path);
            var equippedHeading = LgoLabel("Kỹ năng đã trang bị", 14, UiSubText, true);
            equippedHeading.name = "Map01A Equipped Skill Heading";
            equippedHeading.style.unityTextAlign = TextAnchor.MiddleCenter;
            content.Add(equippedHeading);
            var equippedRow = InventoryRow("Map01A Equipped Skill Strip");
            equippedRow.style.alignItems = Align.Center;
            equippedRow.style.justifyContent = Justify.SpaceBetween;
            equippedRow.style.minHeight = 68;
            var equippedIconIds = new[] { "thien_kiem_quyet", "lang_khong_bo", "phong_tram", "van_kiem" };
            for (var equippedIndex = 0; equippedIndex < equippedIconIds.Length; equippedIndex++)
                equippedRow.Add(CreateEquippedSkillSlot(equippedIconIds[equippedIndex], equippedIndex + 1));
            var pointsGroup = new VisualElement { name = "Map01A Skill Points Group" };
            pointsGroup.style.flexDirection = FlexDirection.Row;
            pointsGroup.style.alignItems = Align.Center;
            var pointsLabel = LgoLabel("Điểm kỹ năng", 12, UiSubText, true);
            pointsLabel.name = "Map01A Skill Points Label";
            var pointsBadge = InventoryBadge("Map01A Skill Points Badge", "12", UiGold);
            var pointsAdd = InventoryButton(() => { }, "Map01A Skill Points Add", "+");
            pointsAdd.style.flexGrow = 0;
            pointsAdd.style.flexBasis = 34;
            pointsAdd.style.minHeight = 34;
            pointsAdd.style.marginRight = 0;
            ApplyLgoDisabledAction(pointsAdd);
            pointsGroup.Add(pointsLabel);
            pointsGroup.Add(pointsBadge);
            pointsGroup.Add(pointsAdd);
            equippedRow.Add(pointsGroup);
            progression.Add(skillArea);
            equippedRow.style.marginTop = 2;
            content.Add(equippedRow);
            body.Add(_skillsPanel);
        }

        private void InitializePotentialView(VisualElement body)
        {
            _potentialPanel = CreateHubSurface("Map01A Potential Panel");
            var diagram = new VisualElement { name = "Map01A Potential Diagram" };
            diagram.style.width = 650;
            diagram.style.height = 400;
            diagram.style.alignSelf = Align.Center;
            diagram.style.position = Position.Relative;
            diagram.style.flexShrink = 0;
            var orbit = new VisualElement { name = "Map01A Potential Orbit", pickingMode = PickingMode.Ignore };
            orbit.style.position = Position.Absolute;
            orbit.style.left = 72;
            orbit.style.top = 14;
            orbit.style.width = 506;
            orbit.style.height = 348;
            orbit.style.borderTopLeftRadius = orbit.style.borderTopRightRadius = 174;
            orbit.style.borderBottomLeftRadius = orbit.style.borderBottomRightRadius = 174;
            orbit.style.borderLeftWidth = orbit.style.borderRightWidth = 2;
            orbit.style.borderTopWidth = orbit.style.borderBottomWidth = 2;
            orbit.style.borderLeftColor = orbit.style.borderRightColor = new Color(.18f, .48f, .72f, .56f);
            orbit.style.borderTopColor = orbit.style.borderBottomColor = new Color(.78f, .60f, .23f, .72f);
            diagram.Add(orbit);

            var core = new VisualElement { name = "Map01A Potential Diagram Core", pickingMode = PickingMode.Ignore };
            core.style.position = Position.Absolute;
            core.style.left = 241;
            core.style.top = 108;
            core.style.width = 168;
            core.style.height = 168;
            core.style.alignItems = Align.Center;
            core.style.justifyContent = Justify.Center;
            core.style.borderTopLeftRadius = core.style.borderTopRightRadius = 84;
            core.style.borderBottomLeftRadius = core.style.borderBottomRightRadius = 84;
            ApplyLgoFrame(core, new Color(.025f, .11f, .19f, .94f), new Color(.24f, .66f, 1f, .82f));
            core.style.borderLeftWidth = core.style.borderRightWidth = 2;
            core.style.borderTopWidth = core.style.borderBottomWidth = 2;
            core.Add(PotentialIcon("Map01A Potential Core Icon", "core", 128));
            var coreText = LgoLabel("TÂM MẠCH", 12, UiGold, true);
            coreText.style.unityTextAlign = TextAnchor.MiddleCenter;
            core.Add(coreText);
            diagram.Add(core);
            diagram.Add(CreatePotentialNode("Map01A Potential Node Công", "Công", "120", "attack", 263, 0));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Thủ", "Thủ", "118", "defense", 16, 132));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Sinh lực", "Sinh lực", "250", "vitality", 510, 132, true));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Linh lực", "Linh lực", "96", "spirit", 138, 270));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Nhanh nhẹn", "Nhanh nhẹn", "110", "agility", 388, 270));
            _potentialPanel.Add(diagram);
            var footer = InventoryRow("Map01A Potential Footer");
            footer.style.justifyContent = Justify.SpaceBetween;
            footer.Add(InventoryBadge("Map01A Potential Recommendation", "Đề xuất Võ", new Color(.74f, .92f, 1f, .94f)));
            footer.Add(InventoryBadge("Map01A Potential Footer Points", "Điểm tiềm năng còn lại: 12", UiGold));
            _potentialPanel.Add(footer);
            body.Add(_potentialPanel);
        }

        private void InitializeSpiritPetView(VisualElement body)
        {
            _spiritPetPanel = CreateHubSurface("Map01A Spirit Pet Panel");
            _spiritPetPreviewTexture = Resources.Load<Texture2D>("LGOMaps/CongDongLamMap01ACharacterHub/spirit-fox-preview");
            var preview = new VisualElement { name = "Map01A Spirit Pet Preview Art" };
            ApplyLgoCharacterHubDetailCard(preview, 10, 8, false);
            preview.style.height = 382;
            preview.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            preview.style.backgroundImage = _spiritPetPreviewTexture == null ? StyleKeyword.None : new StyleBackground(_spiritPetPreviewTexture);
            _spiritPetPanel.Add(preview);
            var identity = LgoTitleLabel("Thanh Vân Hồ · Lv.20", 19);
            identity.style.unityTextAlign = TextAnchor.MiddleCenter;
            _spiritPetPanel.Add(identity);
            _spiritPetPanel.Add(CreateSpiritPetProgress("Map01A Spirit Pet Intimacy", "Thân mật 320/600", 320, new Color(.96f, .32f, .58f, 1f)));
            var growth = CreateSpiritPetProgress("Map01A Spirit Pet Growth", "Tăng trưởng 180/300", 180, new Color(.38f, .78f, .36f, 1f));
            growth.highValue = 300;
            _spiritPetPanel.Add(growth);
            var roster = InventoryRow("Map01A Spirit Pet Roster");
            roster.style.marginTop = 10;
            roster.style.justifyContent = Justify.SpaceBetween;
            roster.Add(CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Thanh Vân Hồ", "Thanh Vân Hồ", "Lv.20 · Đang chọn", true, 0));
            roster.Add(CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Locked 1", "Ô Linh thú II", "Chưa thức tỉnh", false, 1));
            roster.Add(CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Locked 2", "Ô Linh thú III", "Chưa thức tỉnh", false, 2));
            roster.Add(CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Locked 3", "Ô Linh thú IV", "Chưa thức tỉnh", false, 3));
            _spiritPetPanel.Add(roster);
            body.Add(_spiritPetPanel);
        }

        private void InitializeHubInspector(VisualElement body)
        {
            _hubPreviewDetailPanel = InventoryPanel("Map01A Hub Preview Detail Panel");
            _hubPreviewDetailPanel.style.flexGrow = 0;
            _hubPreviewDetailPanel.style.flexBasis = InventoryDesktopDetailColumnWidth;
            _hubPreviewDetailPanel.style.marginLeft = InventoryDesktopColumnGap;
            ApplyLgoCharacterHubDetailCard(_hubPreviewDetailPanel);
            _hubDetailHeader = LgoLabel("CHI TIẾT", 14, UiSubText, true);
            _hubDetailHeader.name = "Map01A Hub Detail Header";
            _hubDetailHeader.style.display = DisplayStyle.None;
            _hubPreviewDetailPanel.Add(_hubDetailHeader);
            var detailHero = new VisualElement { name = "Map01A Hub Preview Detail Hero" };
            detailHero.style.flexDirection = FlexDirection.Row;
            detailHero.style.alignItems = Align.Center;
            detailHero.style.flexShrink = 0;
            detailHero.style.marginTop = 12;
            detailHero.style.marginBottom = 12;
            _hubDetailIcon = HubIcon("Map01A Hub Preview Detail Icon", "skill", 96);
            ApplyLgoCharacterHubHeroIconFrame(_hubDetailIcon);
            _hubDetailIcon.style.flexShrink = 0;
            _hubDetailIcon.style.marginRight = 16;
            detailHero.Add(_hubDetailIcon);
            var detailHeroCopy = new VisualElement { name = "Map01A Hub Preview Detail Hero Copy" };
            detailHeroCopy.style.flexGrow = 1;
            detailHeroCopy.style.minWidth = 0;
            detailHeroCopy.style.flexDirection = FlexDirection.Column;
            _hubDetailName = LgoTitleLabel("", 24);
            _hubDetailName.name = "Map01A Hub Preview Detail Name";
            detailHeroCopy.Add(_hubDetailName);
            _hubDetailMeta = LgoSubtitleLabel("", 13);
            _hubDetailMeta.style.marginTop = 4;
            detailHeroCopy.Add(_hubDetailMeta);
            _hubSpiritPetBadges = InventoryRow("Map01A Spirit Pet Detail Badges");
            _hubSpiritPetBadges.style.marginTop = 8;
            _hubSpiritPetBadges.style.marginBottom = 0;
            var rarityBadge = InventoryBadge("Map01A Spirit Pet Rarity Badge", "Tinh phẩm", new Color(.88f, .62f, 1f, 1f));
            var roleBadge = InventoryBadge("Map01A Spirit Pet Role Badge", "Hỗ trợ", new Color(.72f, .90f, 1f, 1f));
            var stateBadge = InventoryBadge("Map01A Spirit Pet State Badge", "Đang xuất chiến", new Color(.55f, 1f, .65f, 1f));
            _hubSpiritPetBadges.Add(rarityBadge);
            _hubSpiritPetBadges.Add(roleBadge);
            _hubSpiritPetBadges.Add(stateBadge);
            _hubSpiritPetBadges.style.display = DisplayStyle.None;
            detailHeroCopy.Add(_hubSpiritPetBadges);
            detailHero.Add(detailHeroCopy);
            _hubPreviewDetailPanel.Add(detailHero);
            var facts = new VisualElement { name = "Map01A Hub Preview Detail Facts" };
            ApplyLgoInventoryStatsCard(facts);
            _hubDetailBody = LgoLabel("", 15, UiText);
            _hubDetailBody.style.whiteSpace = WhiteSpace.Normal;
            facts.Add(_hubDetailBody);
            _hubPreviewDetailPanel.Add(facts);
            _hubDetailStatus = LgoLabel("", 12, new Color(.76f, 1f, .70f, .94f), true);
            ApplyLgoStatusCard(_hubDetailStatus);
            _hubDetailStatus.style.marginTop = 10;
            _hubDetailStatus.style.minHeight = 48;
            _hubDetailStatus.style.flexShrink = 0;
            _hubDetailStatus.style.whiteSpace = WhiteSpace.Normal;
            _hubDetailStatus.style.unityTextAlign = TextAnchor.MiddleLeft;
            _hubPreviewDetailPanel.Add(_hubDetailStatus);

            var actionSpacer = new VisualElement { name = "Map01A Hub Preview Detail Action Spacer", pickingMode = PickingMode.Ignore };
            actionSpacer.style.flexGrow = 1;
            actionSpacer.style.minHeight = 8;
            _hubPreviewDetailPanel.Add(actionSpacer);

            _hubSkillActionRow = InventoryRow("Map01A Skill Detail Actions");
            _hubSkillActionRow.style.marginTop = 12;
            _hubSkillUpgradeAction = InventoryButton(() => { }, "Map01A Skill Upgrade Action", "Nâng cấp");
            _hubSkillEquipAction = InventoryButton(() => { }, "Map01A Skill Equip Action", "Trang bị");
            foreach (var action in new[] { _hubSkillUpgradeAction, _hubSkillEquipAction })
            {
                action.style.flexGrow = 1;
                action.style.flexBasis = 0;
                action.style.marginRight = 6;
                ApplyLgoDisabledAction(action);
                _hubSkillActionRow.Add(action);
            }
            _hubPreviewDetailPanel.Add(_hubSkillActionRow);
            _hubPotentialActionRow = InventoryRow("Map01A Potential Detail Actions");
            _hubPotentialActionRow.style.marginTop = 12;
            _potentialAddPointAction = InventoryButton(() => { }, "Map01A Potential Add Point", "Cộng 1 điểm");
            _potentialResetAction = InventoryButton(() => { }, "Map01A Potential Reset", "Đặt lại");
            foreach (var action in new[] { _potentialAddPointAction, _potentialResetAction })
            {
                action.style.flexGrow = 1;
                action.style.flexBasis = 0;
                action.style.marginRight = 6;
                ApplyLgoDisabledAction(action);
                _hubPotentialActionRow.Add(action);
            }
            _hubPreviewDetailPanel.Add(_hubPotentialActionRow);
            _hubSpiritPetActionRow = InventoryRow("Map01A Spirit Pet Detail Actions");
            _hubSpiritPetActionRow.style.marginTop = 12;
            _spiritPetDeployAction = InventoryButton(() => { }, "Map01A Spirit Pet Deploy Action", "Đang xuất chiến");
            _spiritPetDevelopAction = InventoryButton(() => { }, "Map01A Spirit Pet Develop Action", "Bồi dưỡng");
            foreach (var action in new[] { _spiritPetDeployAction, _spiritPetDevelopAction })
            {
                action.style.flexGrow = 1;
                action.style.flexBasis = 0;
                action.style.marginRight = 6;
                ApplyLgoDisabledAction(action);
                _hubSpiritPetActionRow.Add(action);
            }
            _hubPreviewDetailPanel.Add(_hubSpiritPetActionRow);
            body.Add(_hubPreviewDetailPanel);
        }

        private void HideCharacterHubPreviewPanels()
        {
            if (_skillsPanel != null) _skillsPanel.style.display = DisplayStyle.None;
            if (_potentialPanel != null) _potentialPanel.style.display = DisplayStyle.None;
            if (_spiritPetPanel != null) _spiritPetPanel.style.display = DisplayStyle.None;
            if (_hubPreviewDetailPanel != null) _hubPreviewDetailPanel.style.display = DisplayStyle.None;
        }

        private void ShowCharacterHubPreviewMode(CharacterHubMode mode)
        {
            _characterInfoOpen = false;
            _suppliesOpen = false;
            RefreshInventoryShellMode();
            _inventoryGridPanel.style.display = DisplayStyle.None;
            _inventoryHeroPanel.style.display = DisplayStyle.None;
            _inventoryDetailPanel.style.display = DisplayStyle.None;
            _inventoryFooter.style.display = DisplayStyle.None;
            HideCharacterHubPreviewPanels();
            _skillsPanel.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _potentialPanel.style.display = mode == CharacterHubMode.Potential ? DisplayStyle.Flex : DisplayStyle.None;
            _spiritPetPanel.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
            _hubPreviewDetailPanel.style.display = DisplayStyle.Flex;
            _inventoryModalTitle.text = mode == CharacterHubMode.Skills ? "KỸ NĂNG" : mode == CharacterHubMode.Potential ? "TIỀM NĂNG" : "LINH THÚ";
            _inventoryModalSubtitle.text = mode == CharacterHubMode.Skills
                ? "Kỹ năng đã lĩnh hội và bộ kỹ năng đang trang bị"
                : mode == CharacterHubMode.Potential
                    ? "Thuộc tính căn bản và hướng phát triển nhân vật"
                    : "Linh thú đồng hành, kỹ năng hỗ trợ và mức thân mật";
            ApplyHubMainTabSelection(mode);
            ShowHubDetail(mode);
            AnimateLgoCharacterHubSwap(mode == CharacterHubMode.Skills
                ? _skillsPanel
                : mode == CharacterHubMode.Potential ? _potentialPanel : _spiritPetPanel);
            AnimateLgoCharacterHubSwap(_hubPreviewDetailPanel);
        }

        private void ApplyHubMainTabSelection(CharacterHubMode? previewMode = null, bool character = false, bool storage = false)
        {
            ApplyLgoCharacterHubTabState(_characterInfoTab, character);
            ApplyLgoCharacterHubTabState(_bagTab, storage);
            ApplyLgoCharacterHubTabState(_skillsTab, previewMode == CharacterHubMode.Skills);
            ApplyLgoCharacterHubTabState(_potentialTab, previewMode == CharacterHubMode.Potential);
            ApplyLgoCharacterHubTabState(_spiritPetTab, previewMode == CharacterHubMode.SpiritPet);
        }

        private void SelectSkillNode(string nodeName, string title, string level, string iconId)
        {
            foreach (var node in _skillPathNodes) ApplyHubPathNodeSelection(node, node.name == nodeName);
            ShowSkillDetail(title, level, iconId);
        }

        private void SelectPotentialNode(string nodeName, string title, string value, string iconId)
        {
            foreach (var node in _potentialPathNodes) ApplyHubPathNodeSelection(node, node.name == nodeName);
            ShowPotentialDetail(title, value, iconId);
        }

        private void ConfigureHubDetailMode(CharacterHubMode mode)
        {
            _hubSkillActionRow.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _hubPotentialActionRow.style.display = mode == CharacterHubMode.Potential ? DisplayStyle.Flex : DisplayStyle.None;
            _hubSpiritPetActionRow.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
            _hubSpiritPetBadges.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
            _hubDetailIcon.style.width = _hubDetailIcon.style.height = mode == CharacterHubMode.Skills
                ? 116
                : mode == CharacterHubMode.Potential ? 104 : 112;
            _hubDetailIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        private void SetHubDetailIcon(string iconId)
        {
            var sprite = _scene.GetMap01AHudIconSprite(iconId);
            _hubDetailIcon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
        }

        private void ShowSkillDetail(string title, string level, string iconId)
        {
            ConfigureHubDetailMode(CharacterHubMode.Skills);
            var sprite = _scene.GetMap01ASkillIconSprite(iconId);
            _hubDetailIcon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            _hubDetailIcon.style.width = _hubDetailIcon.style.height = 116;
            _hubDetailHeader.text = "CHI TIẾT KỸ NĂNG";
            _hubDetailName.text = title;
            _hubDetailMeta.text = "Kỹ năng chủ động · " + level;
            _hubDetailBody.text = title == "Thiên Kiếm Quyết"
                ? "Vận kiếm khí thiên đạo, chém mục tiêu phía trước.\n\nSát thương  320% Công\nPhạm vi  Hình quạt trước mặt\nHồi chiêu  12 giây\nTiêu hao MP  180"
                : "Cấp hiện hành  " + level + "\n\nThông tin hiệu ứng chi tiết sẽ hiển thị khi kỹ năng được lĩnh hội đầy đủ.";
            _hubDetailStatus.text = "Nâng cấp và thay đổi bộ kỹ năng đang khóa.";
        }

        private void ShowPotentialDetail(string title, string value, string iconId)
        {
            ConfigureHubDetailMode(CharacterHubMode.Potential);
            var sprite = _scene.GetMap01APotentialIconSprite(iconId);
            _hubDetailIcon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            _hubDetailIcon.style.width = _hubDetailIcon.style.height = 124;
            _hubDetailHeader.text = "CHI TIẾT TIỀM NĂNG";
            _hubDetailName.text = title;
            _hubDetailMeta.text = "Giá trị xem trước: " + value;
            _hubDetailBody.text = title == "Sinh lực"
                ? "Tăng cường thể chất, sinh lực và khả năng phòng thủ.\n\nHiệu quả hiện tại\nSinh lực (HP)  +12.500\nPhòng thủ  +250\n\nKhi cộng 1 điểm\nSinh lực (HP)  +50\nPhòng thủ  +1"
                : "Điểm đang chọn  " + title + " · " + value + "\n\nHiệu quả hiện tại và mức tăng kế tiếp được giữ ở chế độ xem trước.";
            _hubDetailStatus.text = "Tiêu hao 1 điểm tiềm năng · thao tác đang khóa.";
        }

        private void ShowHubDetail(CharacterHubMode mode)
        {
            if (mode == CharacterHubMode.Skills)
            {
                ShowSkillDetail("Thiên Kiếm Quyết", "Lv.8", "thien_kiem_quyet");
            }
            else if (mode == CharacterHubMode.Potential)
            {
                ShowPotentialDetail("Sinh lực", "250", "vitality");
            }
            else
            {
                ConfigureHubDetailMode(CharacterHubMode.SpiritPet);
                _hubDetailIcon.style.backgroundImage = _spiritPetPreviewTexture == null
                    ? new StyleBackground(_scene.GetMap01AHudIconSprite("crest"))
                    : new StyleBackground(_spiritPetPreviewTexture);
                _hubDetailIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
                _hubDetailHeader.text = "CHI TIẾT LINH THÚ";
                _hubDetailName.text = "Thanh Vân Hồ";
                _hubDetailMeta.text = "Lv.20";
                _hubDetailBody.text = "Thuộc tính Linh thú\nHP  +8720\nTấn Công  +860\nPhòng Thủ  +430\nHồi Phục  +28%\nGiảm Sát Thương  +12%\n\nKỹ năng Linh thú\nThanh Vân Hộ Thể · Lv.1\nCửu Vĩ Linh Phong · Lv.1";
                _hubDetailStatus.text = "Tính năng bồi dưỡng đang khóa.";
            }
        }

    }
}
