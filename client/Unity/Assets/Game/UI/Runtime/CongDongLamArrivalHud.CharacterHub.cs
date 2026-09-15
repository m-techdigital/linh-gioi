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
        private VisualElement _hubPotentialFacts;
        private VisualElement _hubSkillActionRow, _hubPotentialActionRow, _hubSpiritPetActionRow;
        private Label _hubDetailHeader, _hubDetailName, _hubDetailMeta, _hubDetailBody, _hubDetailStatus;
        private Label _hubPotentialSummary, _hubPotentialCurrentLevel, _hubPotentialCurrentEffect, _hubPotentialNextEffect, _hubPotentialCost;
        private VisualElement _hubDetailIcon;
        private Button _hubSkillUpgradeAction, _hubSkillEquipAction, _potentialAddPointAction, _potentialResetAction, _spiritPetDeployAction, _spiritPetDevelopAction;
        private Texture2D _spiritPetPreviewTexture, _spiritPetPortraitTexture;
        private readonly List<Button> _skillPathNodes = new List<Button>();
        private readonly List<VisualElement> _skillPathIcons = new List<VisualElement>();
        private readonly List<Label> _skillPathTitles = new List<Label>();
        private readonly List<Label> _skillPathLevels = new List<Label>();
        private readonly List<VisualElement> _equippedSkillIcons = new List<VisualElement>();
        private readonly List<Button> _potentialPathNodes = new List<Button>();
        private readonly List<VisualElement> _potentialPathIcons = new List<VisualElement>();
        private readonly List<Label> _potentialPathTitles = new List<Label>();
        private readonly List<Label> _potentialPathValues = new List<Label>();
        private Label _potentialRecommendation;
        private VisualElement _spiritPetPreview, _spiritPetSelectedRosterArt;
        private Label _spiritPetIdentity, _spiritPetHeroLevel, _spiritPetSelectedRosterName, _spiritPetSelectedRosterLevel;
        private Label _spiritPetRarityBadge, _spiritPetRoleBadge, _spiritPetStateBadge;
        private VisualElement _hubSpiritPetFacts;
        private VisualElement _hubSpiritPetStats;
        private readonly List<Label> _hubSpiritPetStatValues = new List<Label>();
        private readonly List<VisualElement> _hubSpiritPetSkillIcons = new List<VisualElement>();
        private readonly List<Label> _hubSpiritPetSkillNames = new List<Label>();
        private readonly List<Label> _hubSpiritPetSkillLevels = new List<Label>();
        private readonly List<Label> _hubSpiritPetSkillDescriptions = new List<Label>();
        private VisualElement _characterHubBody;
        private string _renderedCharacterHubClassId;
        private string _characterHubEvidenceClassId;
        private CharacterHubMode? _activeCharacterHubPreviewMode;
        private readonly CharacterHubSelectionState _characterHubSelectionState = new CharacterHubSelectionState();

        private CharacterHubClassProfile ActiveCharacterHubProfile =>
            CharacterHubClassCatalog.Get(string.IsNullOrEmpty(_characterHubEvidenceClassId)
                ? _scene.ActiveEquipmentClassId
                : _characterHubEvidenceClassId);

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

        private Sprite GetSkillPreviewSprite(CharacterHubSkillPreview skill)
            => skill.IconCatalog == CharacterHubIconCatalog.Skill
                ? _scene.GetMap01ASkillIconSprite(skill.IconId)
                : _scene.GetMap01AHudIconSprite(skill.IconId);

        private void BindSkillPreviewIcon(VisualElement icon, CharacterHubSkillPreview skill, float size)
        {
            ApplyLgoSkillIcon(icon, size);
            var sprite = GetSkillPreviewSprite(skill);
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
        }

        private VisualElement CreateEquippedSkillSlot(int index)
        {
            var slot = new VisualElement { name = "Map01A Equipped Skill Slot " + index };
            ApplyLgoEquippedSkillSlot(slot);
            var icon = new VisualElement
            {
                name = "Map01A Equipped Skill Icon " + index,
                pickingMode = PickingMode.Ignore
            };
            _equippedSkillIcons.Add(icon);
            slot.Add(icon);
            var number = LgoLabel(index.ToString(), 16, UiGold, true);
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
            var label = LgoLabel(text, 18, UiText, true);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.Add(label);
            ApplyLgoSelectedTab(button, selected);
            if (!selected) ApplyLgoCharacterHubUnavailableControl(button);
            return button;
        }

        private Button CreateHubPathNode(int index)
        {
            var name = "Map01A Skill Node " + index;
            var node = InventoryButton(() => SelectSkillNode(index), name);
            ApplyLgoSkillNode(node);
            var icon = new VisualElement { name = name + " Icon", pickingMode = PickingMode.Ignore };
            _skillPathIcons.Add(icon);
            node.Add(icon);
            var titleLabel = LgoLabel("", 12, UiText, true);
            titleLabel.name = name + " Title";
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            titleLabel.style.marginTop = 2;
            titleLabel.style.display = DisplayStyle.None;
            titleLabel.style.whiteSpace = WhiteSpace.NoWrap;
            _skillPathTitles.Add(titleLabel);
            node.Add(titleLabel);
            var levelLabel = LgoLabel("", 10, new Color(.74f, .92f, 1f, .92f), true);
            levelLabel.name = name + " Level";
            ApplyLgoSkillLevelBadge(levelLabel);
            levelLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _skillPathLevels.Add(levelLabel);
            node.Add(levelLabel);
            ApplyHubPathNodeSelection(node, false);
            return node;
        }


        private static void ApplyHubPathNodeSelection(Button node, bool selected)
        {
            ApplyLgoCharacterHubSelectionState(node, selected);
            var title = node.Q<Label>(node.name + " Title");
            if (title != null) title.style.color = selected ? UiGold : UiText;
        }

        private static void ApplyPotentialNodeSelection(Button node, bool selected)
        {
            node.EnableInClassList(LgoCharacterHubSelectedClass, selected);
            node.style.backgroundColor = Color.clear;
            node.style.borderLeftWidth = node.style.borderRightWidth = 0;
            node.style.borderTopWidth = node.style.borderBottomWidth = 0;
            var icon = node.Q<VisualElement>(node.name + " Icon");
            if (icon != null)
            {
                icon.style.opacity = selected ? 1f : .86f;
            }
            var title = node.Q<Label>(node.name + " Title");
            if (title != null) title.style.color = selected ? UiGold : UiText;
        }

        private static VisualElement CreateHubPathConnector(string name, bool vertical = false)
        {
            var connector = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            connector.style.flexShrink = 0;
            connector.style.alignSelf = Align.Center;
            connector.style.backgroundColor = new Color(.20f, .58f, .82f, .68f);
            connector.style.width = vertical ? 2 : 40;
            connector.style.height = vertical ? 12 : 2;
            return connector;
        }

        private VisualElement CreatePotentialDataOverlay(int index, float left, float top)
        {
            var name = "Map01A Potential Node " + index;
            var node = InventoryButton(() => SelectPotentialNode(index), name);
            ApplyLgoPotentialDataOverlay(node);
            node.AddToClassList("lgo-potential-node-overlay");
            var icon = new VisualElement { name = name + " Icon", pickingMode = PickingMode.Ignore };
            ApplyLgoSkillIcon(icon, 82);
            _potentialPathIcons.Add(icon);
            node.Add(icon);
            var titleLabel = LgoLabel("", 12, UiText, true);
            titleLabel.name = name + " Title";
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _potentialPathTitles.Add(titleLabel);
            node.Add(titleLabel);
            var valueLabel = LgoLabel("", 10, new Color(.74f, .92f, 1f, .92f), true);
            valueLabel.name = name + " Value";
            valueLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _potentialPathValues.Add(valueLabel);
            node.Add(valueLabel);
            ApplyPotentialNodeSelection(node, false);
            _potentialPathNodes.Add(node);
            node.style.position = Position.Absolute;
            node.style.left = left;
            node.style.top = top;
            return node;
        }

        private VisualElement CreateSpiritPetRosterEntry(string name, string title, string level, bool selected, int lockedIndex)
        {
            var card = InventoryButton(() => ShowHubDetail(CharacterHubMode.SpiritPet), name);
            ApplyLgoSpiritPetRosterCard(card, selected);
            var art = new VisualElement
            {
                name = selected ? "Map01A Spirit Pet Selected Roster Art" : "Map01A Spirit Pet Locked Roster " + lockedIndex,
                pickingMode = PickingMode.Ignore
            };
            ApplyLgoItemIcon(art);
            art.style.width = 62;
            art.style.height = 62;
            art.style.marginTop = 0;
            art.style.marginBottom = 2;
            art.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            art.style.backgroundImage = selected && _spiritPetPortraitTexture != null
                ? new StyleBackground(_spiritPetPortraitTexture)
                : new StyleBackground(_scene.GetMap01AHudIconSprite("lock"));
            card.Add(art);
            var titleLabel = LgoLabel(title, 11, selected ? UiGold : UiSubText, true);
            titleLabel.name = name + " Name";
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(titleLabel);
            var stateLabel = LgoLabel(level, 10, selected ? new Color(.76f, 1f, .70f, .94f) : new Color(.56f, .64f, .68f, .78f));
            stateLabel.name = name + " Level";
            stateLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(stateLabel);
            ApplyLgoCharacterHubCopyRhythm(card);
            ApplyLgoSpiritPetRosterContent(art, titleLabel, stateLabel, selected);
            card.tooltip = title + " · " + level;
            if (!selected) ApplyLgoCharacterHubUnavailableControl(card);
            return card;
        }

        private UnityEngine.UIElements.ProgressBar CreateSpiritPetProgress(string name, string title, float value, Color fill)
        {
            var bar = new UnityEngine.UIElements.ProgressBar { name = name, title = title, lowValue = 0, highValue = 600, value = value };
            ApplyLgoVitalBar(bar, fill);
            ApplyLgoCharacterHubVitalBar(bar);
            bar.style.height = bar.style.minHeight = bar.style.maxHeight = 22;
            bar.style.fontSize = 15;
            bar.style.marginTop = 1;
            bar.style.marginBottom = 0;
            return bar;
        }

        private VisualElement CreateSpiritPetSkillRow(int index)
        {
            var row = InventoryRow("Map01A Spirit Pet Skill Row " + index);
            ApplyLgoSpiritPetSkillRow(row);
            var icon = new VisualElement
            {
                name = "Map01A Spirit Pet Skill Row " + index + " Icon",
                pickingMode = PickingMode.Ignore
            };
            ApplyLgoSkillIcon(icon, 56);
            icon.style.flexShrink = 0;
            icon.style.marginRight = 8;
            _hubSpiritPetSkillIcons.Add(icon);
            row.Add(icon);
            var copy = new VisualElement { name = "Map01A Spirit Pet Skill Row " + index + " Copy" };
            copy.style.flexGrow = 1;
            copy.style.minWidth = 0;
            copy.style.flexDirection = FlexDirection.Column;
            var heading = InventoryRow("Map01A Spirit Pet Skill Row " + index + " Heading");
            heading.style.marginBottom = 2;
            var skillName = LgoLabel("", 18, UiGold, true);
            skillName.name = "Map01A Spirit Pet Skill Row " + index + " Name";
            skillName.style.flexGrow = 1;
            var skillLevel = LgoLabel("", 14, UiSubText, true);
            skillLevel.name = "Map01A Spirit Pet Skill Row " + index + " Level";
            _hubSpiritPetSkillNames.Add(skillName);
            _hubSpiritPetSkillLevels.Add(skillLevel);
            heading.Add(skillName);
            heading.Add(skillLevel);
            copy.Add(heading);
            var description = LgoLabel("", 16, UiSubText);
            description.name = "Map01A Spirit Pet Skill Row " + index + " Description";
            description.style.whiteSpace = WhiteSpace.Normal;
            _hubSpiritPetSkillDescriptions.Add(description);
            copy.Add(description);
            row.Add(copy);
            return row;
        }

        private VisualElement CreateSpiritPetStatRow(int index)
        {
            var row = InventoryRow("Map01A Spirit Pet Stat Row " + index);
            ApplyLgoSpiritPetStatRow(row);
            row.Add(LgoLabel("◆", 11, new Color(.38f, .76f, 1f, 1f), true));
            var value = LgoLabel("", 18, UiText);
            value.name = "Map01A Spirit Pet Stat Value " + index;
            value.style.marginLeft = 8;
            _hubSpiritPetStatValues.Add(value);
            row.Add(value);
            return row;
        }

        private void InitializeCharacterHub(VisualElement body)
        {
            _characterHubBody = body;
            _renderedCharacterHubClassId = _scene.ActiveEquipmentClassId;
            InitializeSkillsView(body);
            InitializePotentialView(body);
            InitializeSpiritPetView(body);
            InitializeHubInspector(body);
            BindCharacterHubProfile();
            HideCharacterHubPreviewPanels();
        }

        private void RefreshCharacterHubClassProfile()
        {
            var profileId = ActiveCharacterHubProfile.Id;
            if (_characterHubBody == null || _renderedCharacterHubClassId == profileId) return;
            _renderedCharacterHubClassId = profileId;
            BindCharacterHubProfile();
            if (_activeCharacterHubPreviewMode.HasValue)
                ShowHubDetail(_activeCharacterHubPreviewMode.Value);
            else
                HideCharacterHubPreviewPanels();
        }

        internal void BindCharacterHubEvidenceClass(string classId)
        {
            var profile = CharacterHubClassCatalog.Get(classId);
            _characterHubEvidenceClassId = profile.Id;
            _renderedCharacterHubClassId = profile.Id;
            _characterHubSelectionState.SelectPotential(profile, profile.DefaultPotentialName);
            BindCharacterHubProfile();
            ShowHubDetail(CharacterHubMode.Potential);
        }

        internal void ClearCharacterHubEvidenceClass()
        {
            _characterHubEvidenceClassId = null;
            _renderedCharacterHubClassId = _scene.ActiveEquipmentClassId;
            BindCharacterHubProfile();
            if (_activeCharacterHubPreviewMode.HasValue)
                ShowHubDetail(_activeCharacterHubPreviewMode.Value);
            else
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
            rail.style.flexShrink = 0;
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
            path.style.justifyContent = Justify.SpaceAround;
            var stageCounts = new[] { 3, 3, 3 };
            var skillIndex = 0;
            for (var stageIndex = 0; stageIndex < stageCounts.Length; stageIndex++)
            {
                var stage = new VisualElement { name = "Map01A Skill Path Stage " + (stageIndex + 1) };
                stage.style.flexDirection = FlexDirection.Row;
                stage.style.flexShrink = 0;
                stage.style.height = 128;
                stage.style.alignItems = Align.Center;
                stage.style.justifyContent = Justify.Center;
                stage.style.width = new Length(100, LengthUnit.Percent);
                for (var nodeIndex = 0; nodeIndex < stageCounts[stageIndex]; nodeIndex++)
                {
                    var node = CreateHubPathNode(skillIndex);
                    _skillPathNodes.Add(node);
                    stage.Add(node);
                    skillIndex++;
                    if (nodeIndex < stageCounts[stageIndex] - 1)
                        stage.Add(CreateHubPathConnector("Map01A Skill Stage " + (stageIndex + 1) + " Connector " + (nodeIndex + 1)));
                }
                path.Add(stage);

            }
            skillArea.Add(path);
            var equippedHeading = LgoLabel("Kỹ năng đã trang bị", 20, UiSubText, true);
            equippedHeading.name = "Map01A Equipped Skill Heading";
            equippedHeading.style.unityTextAlign = TextAnchor.MiddleCenter;
            content.Add(equippedHeading);
            var equippedRow = InventoryRow("Map01A Equipped Skill Strip");
            equippedRow.style.alignItems = Align.Center;
            equippedRow.style.justifyContent = Justify.SpaceBetween;
            equippedRow.style.minHeight = 82;
            for (var equippedIndex = 0; equippedIndex < 4; equippedIndex++)
                equippedRow.Add(CreateEquippedSkillSlot(equippedIndex + 1));
            var pointsGroup = new VisualElement { name = "Map01A Skill Points Group" };
            pointsGroup.style.flexDirection = FlexDirection.Row;
            pointsGroup.style.alignItems = Align.Center;
            var pointsLabel = LgoLabel("Điểm kỹ năng", 16, UiSubText, true);
            pointsLabel.name = "Map01A Skill Points Label";
            var pointsBadge = InventoryBadge("Map01A Skill Points Badge", "12", UiGold);
            var pointsAdd = InventoryButton(() => { }, "Map01A Skill Points Add", "+");
            pointsAdd.style.flexGrow = 0;
            pointsAdd.style.flexBasis = 34;
            pointsAdd.style.minHeight = 34;
            pointsAdd.style.marginRight = 0;
            ApplyLgoCharacterHubUnavailableControl(pointsAdd);
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
            diagram.style.width = CharacterHubPotentialTopology.CanvasWidth;
            diagram.style.height = CharacterHubPotentialTopology.CanvasHeight;
            diagram.style.alignSelf = Align.Center;
            diagram.style.position = Position.Relative;
            diagram.style.flexShrink = 0;
            diagram.Add(new CharacterHubPotentialTopology(LoadLgoCharacterHubPotentialTopology()));

            var core = new VisualElement { name = "Map01A Potential Diagram Core", pickingMode = PickingMode.Ignore };
            core.style.position = Position.Absolute;
            core.style.left = (CharacterHubPotentialTopology.CanvasWidth - 168f) * .5f;
            core.style.top = 176;
            core.style.width = 168;
            core.style.height = 168;
            core.style.alignItems = Align.Center;
            core.style.justifyContent = Justify.FlexEnd;
            core.style.paddingBottom = 4;
            core.style.borderTopLeftRadius = core.style.borderTopRightRadius = 84;
            core.style.borderBottomLeftRadius = core.style.borderBottomRightRadius = 84;
            core.style.backgroundColor = Color.clear;
            core.style.borderLeftWidth = core.style.borderRightWidth = 0;
            core.style.borderTopWidth = core.style.borderBottomWidth = 0;
            diagram.Add(core);
            for (var potentialIndex = 0; potentialIndex < CharacterHubPotentialTopology.NodePositions.Length; potentialIndex++)
                diagram.Add(CreatePotentialDataOverlay(potentialIndex,
                    CharacterHubPotentialTopology.NodePositions[potentialIndex].x,
                    CharacterHubPotentialTopology.NodePositions[potentialIndex].y));
            _potentialPanel.Add(diagram);
            var footer = InventoryRow("Map01A Potential Footer");
            footer.style.justifyContent = Justify.SpaceBetween;
            _potentialRecommendation = InventoryBadge("Map01A Potential Recommendation", "", new Color(.74f, .92f, 1f, .94f));
            footer.Add(_potentialRecommendation);
            footer.Add(InventoryBadge("Map01A Potential Footer Points", "Điểm tiềm năng còn lại: 12", UiGold));
            _potentialPanel.Add(footer);
            body.Add(_potentialPanel);
        }

        private void InitializeSpiritPetView(VisualElement body)
        {
            _spiritPetPanel = CreateHubSurface("Map01A Spirit Pet Panel");
            _spiritPetPreview = new VisualElement { name = "Map01A Spirit Pet Preview Art" };
            ApplyLgoSpiritPetHeroPreview(_spiritPetPreview);
            _spiritPetPanel.Add(_spiritPetPreview);
            _spiritPetIdentity = LgoTitleLabel("", 24);
            _spiritPetIdentity.style.height = 30;
            _spiritPetIdentity.name = "Map01A Spirit Pet Identity";
            _spiritPetIdentity.style.unityTextAlign = TextAnchor.MiddleCenter;
            _spiritPetIdentity.style.flexShrink = 0;
            _spiritPetPanel.Add(_spiritPetIdentity);
            _spiritPetHeroLevel = LgoLabel("", 18, UiSubText, true);
            _spiritPetHeroLevel.name = "Map01A Spirit Pet Hero Level";
            _spiritPetHeroLevel.style.height = 22;
            _spiritPetHeroLevel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _spiritPetHeroLevel.style.flexShrink = 0;
            _spiritPetPanel.Add(_spiritPetHeroLevel);
            _spiritPetPanel.Add(CreateSpiritPetProgress("Map01A Spirit Pet Intimacy", "Thân mật 320/600", 320, new Color(.96f, .32f, .58f, 1f)));
            var growth = CreateSpiritPetProgress("Map01A Spirit Pet Growth", "Tăng trưởng 180/300", 180, new Color(.38f, .78f, .36f, 1f));
            growth.highValue = 300;
            _spiritPetPanel.Add(growth);
            var roster = InventoryRow("Map01A Spirit Pet Roster");
            ApplyLgoSpiritPetRoster(roster);
            var selectedPet = CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Selected", "", "", true, 0);
            _spiritPetSelectedRosterArt = selectedPet.Q<VisualElement>("Map01A Spirit Pet Selected Roster Art");
            _spiritPetSelectedRosterName = selectedPet.Q<Label>("Map01A Spirit Pet Card Selected Name");
            _spiritPetSelectedRosterLevel = selectedPet.Q<Label>("Map01A Spirit Pet Card Selected Level");
            roster.Add(selectedPet);
            roster.Add(CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Locked 1", "Ô Linh thú II", "Chưa thức tỉnh", false, 1));
            roster.Add(CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Locked 2", "Ô Linh thú III", "Chưa thức tỉnh", false, 2));
            roster.Add(CreateSpiritPetRosterEntry("Map01A Spirit Pet Card Locked 3", "Ô Linh thú IV", "Chưa thức tỉnh", false, 3));
            _spiritPetPanel.Add(roster);
            ApplyLgoCharacterHubCopyRhythm(_spiritPetPanel);
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
            detailHero.style.marginTop = 6;
            detailHero.style.marginBottom = 6;
            _hubDetailIcon = HubIcon("Map01A Hub Preview Detail Icon", "skill", 96);
            ApplyLgoCharacterHubHeroIconFrame(_hubDetailIcon);
            _hubDetailIcon.style.flexShrink = 0;
            _hubDetailIcon.style.marginRight = 16;
            detailHero.Add(_hubDetailIcon);
            var detailHeroCopy = new VisualElement { name = "Map01A Hub Preview Detail Hero Copy" };
            detailHeroCopy.style.flexGrow = 1;
            detailHeroCopy.style.minWidth = 0;
            detailHeroCopy.style.flexDirection = FlexDirection.Column;
            _hubDetailName = LgoTitleLabel("", 28);
            _hubDetailName.name = "Map01A Hub Preview Detail Name";
            detailHeroCopy.Add(_hubDetailName);
            _hubDetailMeta = LgoSubtitleLabel("", 16);
            _hubDetailMeta.name = "Map01A Hub Preview Detail Meta";
            _hubDetailMeta.style.marginTop = 4;
            detailHeroCopy.Add(_hubDetailMeta);
            _hubPotentialSummary = LgoLabel("", 17, UiText);
            _hubPotentialSummary.name = "Map01A Potential Summary";
            _hubPotentialSummary.style.marginTop = 6;
            _hubPotentialSummary.style.whiteSpace = WhiteSpace.Normal;
            _hubPotentialSummary.style.display = DisplayStyle.None;
            detailHeroCopy.Add(_hubPotentialSummary);
            _hubSpiritPetBadges = InventoryRow("Map01A Spirit Pet Detail Badges");
            _hubSpiritPetBadges.style.marginTop = 8;
            _hubSpiritPetBadges.style.marginBottom = 0;
            _spiritPetRarityBadge = InventoryBadge("Map01A Spirit Pet Rarity Badge", "", new Color(.88f, .62f, 1f, 1f));
            _spiritPetRoleBadge = InventoryBadge("Map01A Spirit Pet Role Badge", "", new Color(.72f, .90f, 1f, 1f));
            _spiritPetStateBadge = InventoryBadge("Map01A Spirit Pet State Badge", "", new Color(.55f, 1f, .65f, 1f));
            _hubSpiritPetBadges.Add(_spiritPetRarityBadge);
            _hubSpiritPetBadges.Add(_spiritPetRoleBadge);
            _hubSpiritPetBadges.Add(_spiritPetStateBadge);
            _hubSpiritPetBadges.style.display = DisplayStyle.None;
            detailHeroCopy.Add(_hubSpiritPetBadges);
            detailHero.Add(detailHeroCopy);
            _hubPreviewDetailPanel.Add(detailHero);
            var detailScroll = new ScrollView(ScrollViewMode.Vertical) { name = "Map01A Hub Detail Scroll" };
            RuntimeUiOverflowGuard.ApplyBoundedScroll(detailScroll, 900);
            detailScroll.style.flexGrow = 1;
            detailScroll.style.minHeight = 0;
            detailScroll.contentViewport.RegisterCallback<GeometryChangedEvent>(evt =>
                detailScroll.contentContainer.style.width = evt.newRect.width);
            _hubPreviewDetailPanel.Add(detailScroll);
            var facts = new VisualElement { name = "Map01A Hub Preview Detail Facts" };
            ApplyLgoCharacterHubInspectorFacts(facts);
            facts.style.flexShrink = 0;
            facts.style.marginTop = 0;
            facts.style.paddingTop = facts.style.paddingBottom = 4;
            _hubDetailBody = LgoLabel("", 18, UiText);
            _hubDetailBody.style.whiteSpace = WhiteSpace.Normal;
            facts.Add(_hubDetailBody);
            _hubPotentialFacts = new VisualElement { name = "Map01A Potential Detail Facts" };
            _hubPotentialFacts.style.flexDirection = FlexDirection.Column;
            _hubPotentialFacts.style.display = DisplayStyle.None;
            _hubPotentialCurrentLevel = LgoLabel("", 21, UiGold, true);
            _hubPotentialCurrentLevel.name = "Map01A Potential Current Level";
            _hubPotentialFacts.Add(_hubPotentialCurrentLevel);
            var levelDivider = LgoDivider("Map01A Potential Level Divider");
            ApplyLgoCharacterHubReadingDivider(levelDivider);
            _hubPotentialFacts.Add(levelDivider);
            var currentHeading = LgoLabel("HIỆU QUẢ HIỆN TẠI", 17, new Color(.38f, .74f, 1f, 1f), true);
            currentHeading.name = "Map01A Potential Current Effect Heading";
            _hubPotentialFacts.Add(currentHeading);
            _hubPotentialCurrentEffect = LgoLabel("", 20, UiText, true);
            _hubPotentialCurrentEffect.name = "Map01A Potential Current Effect";
            _hubPotentialCurrentEffect.style.marginTop = 6;
            _hubPotentialCurrentEffect.style.whiteSpace = WhiteSpace.Normal;
            _hubPotentialFacts.Add(_hubPotentialCurrentEffect);
            var effectDivider = LgoDivider("Map01A Potential Effect Divider");
            ApplyLgoCharacterHubReadingDivider(effectDivider);
            _hubPotentialFacts.Add(effectDivider);
            var nextHeading = LgoLabel("HIỆU QUẢ KHI CỘNG 1 ĐIỂM", 17, new Color(.50f, .96f, .58f, 1f), true);
            nextHeading.name = "Map01A Potential Next Effect Heading";
            _hubPotentialFacts.Add(nextHeading);
            _hubPotentialNextEffect = LgoLabel("", 20, new Color(.62f, 1f, .68f, 1f), true);
            _hubPotentialNextEffect.name = "Map01A Potential Next Effect";
            _hubPotentialNextEffect.style.marginTop = 6;
            _hubPotentialNextEffect.style.whiteSpace = WhiteSpace.Normal;
            _hubPotentialFacts.Add(_hubPotentialNextEffect);
            var costRow = new VisualElement { name = "Map01A Potential Cost Row" };
            costRow.style.flexDirection = FlexDirection.Row;
            costRow.style.alignItems = Align.Center;
            costRow.style.marginTop = 8;
            costRow.style.minHeight = 52;
            costRow.style.flexShrink = 0;
            ApplyLgoCharacterHubCostRow(costRow);
            var costIcon = PotentialIcon("Map01A Potential Cost Icon", "core", 38);
            costIcon.style.flexShrink = 0;
            costIcon.style.marginRight = 10;
            costRow.Add(costIcon);
            _hubPotentialCost = LgoLabel("Tiêu hao  Điểm tiềm năng ×1", 16, UiGold, true);
            _hubPotentialCost.name = "Map01A Potential Cost";
            _hubPotentialCost.style.unityTextAlign = TextAnchor.MiddleLeft;
            costRow.Add(_hubPotentialCost);
            _hubPotentialFacts.Add(costRow);
            ApplyLgoCharacterHubCopyRhythm(_hubPotentialFacts);
            facts.Add(_hubPotentialFacts);
            _hubSpiritPetFacts = new VisualElement { name = "Map01A Spirit Pet Detail Facts" };
            _hubSpiritPetFacts.style.flexDirection = FlexDirection.Column;
            _hubSpiritPetFacts.style.display = DisplayStyle.None;
            var statHeading = LgoLabel("Thuộc tính Linh Thú", 20, UiGold, true);
            statHeading.name = "Map01A Spirit Pet Stats Heading";
            _hubSpiritPetFacts.Add(statHeading);
            _hubSpiritPetStats = new VisualElement { name = "Map01A Spirit Pet Detail Stats" };
            _hubSpiritPetStats.style.flexDirection = FlexDirection.Column;
            _hubSpiritPetStats.style.flexShrink = 0;
            for (var index = 0; index < 5; index++)
                _hubSpiritPetStats.Add(CreateSpiritPetStatRow(index));
            _hubSpiritPetFacts.Add(_hubSpiritPetStats);
            var skillHeading = LgoLabel("Kỹ năng Linh Thú", 20, UiGold, true);
            skillHeading.style.flexShrink = 0;
            skillHeading.style.marginTop = 4;
            _hubSpiritPetFacts.Add(skillHeading);
            _hubSpiritPetFacts.Add(CreateSpiritPetSkillRow(0));
            _hubSpiritPetFacts.Add(CreateSpiritPetSkillRow(1));
            ApplyLgoCharacterHubCopyRhythm(_hubSpiritPetFacts);
            facts.Add(_hubSpiritPetFacts);
            detailScroll.Add(facts);
            _hubDetailStatus = LgoLabel("", 16, new Color(.76f, 1f, .70f, .94f), true);
            _hubDetailStatus.name = "Map01A Hub Detail Status";
            _hubDetailStatus.style.paddingLeft = _hubDetailStatus.style.paddingRight = 0;
            _hubDetailStatus.style.marginTop = 10;
            _hubDetailStatus.style.minHeight = 48;
            _hubDetailStatus.style.flexShrink = 0;
            _hubDetailStatus.style.whiteSpace = WhiteSpace.Normal;
            _hubDetailStatus.style.unityTextAlign = TextAnchor.MiddleLeft;
            detailScroll.Add(_hubDetailStatus);

            _hubSkillActionRow = InventoryRow("Map01A Skill Detail Actions");
            _hubSkillActionRow.style.marginTop = 6;
            _hubSkillUpgradeAction = InventoryButton(() => { }, "Map01A Skill Upgrade Action", "Nâng cấp");
            _hubSkillEquipAction = InventoryButton(() => { }, "Map01A Skill Equip Action", "Trang bị");
            foreach (var action in new[] { _hubSkillUpgradeAction, _hubSkillEquipAction })
            {
                action.style.flexGrow = 1;
                action.style.flexBasis = 0;
                action.style.marginRight = 6;
                ApplyLgoCharacterHubLockedAction(action, action == _hubSkillUpgradeAction);
                _hubSkillActionRow.Add(action);
            }
            _hubPreviewDetailPanel.Add(_hubSkillActionRow);
            _hubPotentialActionRow = InventoryRow("Map01A Potential Detail Actions");
            _hubPotentialActionRow.style.marginTop = 6;
            _potentialAddPointAction = InventoryButton(() => { }, "Map01A Potential Add Point", "Cộng 1 điểm");
            _potentialResetAction = InventoryButton(() => { }, "Map01A Potential Reset", "Đặt lại");
            foreach (var action in new[] { _potentialAddPointAction, _potentialResetAction })
            {
                action.style.flexGrow = 1;
                action.style.flexBasis = 0;
                action.style.marginRight = 6;
                ApplyLgoCharacterHubLockedAction(action, action == _potentialAddPointAction);
                _hubPotentialActionRow.Add(action);
            }
            _hubPreviewDetailPanel.Add(_hubPotentialActionRow);
            _hubSpiritPetActionRow = InventoryRow("Map01A Spirit Pet Detail Actions");
            _hubSpiritPetActionRow.style.marginTop = 6;
            _spiritPetDeployAction = InventoryButton(() => { }, "Map01A Spirit Pet Deploy Action", "Đang xuất chiến");
            _spiritPetDevelopAction = InventoryButton(() => { }, "Map01A Spirit Pet Develop Action", "Bồi dưỡng");
            foreach (var action in new[] { _spiritPetDeployAction, _spiritPetDevelopAction })
            {
                action.style.flexGrow = 1;
                action.style.flexBasis = 0;
                action.style.marginRight = 6;
                ApplyLgoCharacterHubLockedAction(action, action == _spiritPetDeployAction);
                _hubSpiritPetActionRow.Add(action);
            }
            _hubPreviewDetailPanel.Add(_hubSpiritPetActionRow);
            body.Add(_hubPreviewDetailPanel);
        }

        private void BindCharacterHubProfile()
        {
            var profile = ActiveCharacterHubProfile;
            var selectedSkillId = _characterHubSelectionState.SkillIdFor(profile);
            for (var index = 0; index < _skillPathNodes.Count; index++)
            {
                var skill = profile.Skills[index];
                var node = _skillPathNodes[index];
                node.tooltip = skill.Name + " · " + skill.Level;
                BindSkillPreviewIcon(_skillPathIcons[index], skill, 96);
                _skillPathTitles[index].text = skill.Name;
                _skillPathLevels[index].text = skill.Level;
                ApplyHubPathNodeSelection(node, skill.Id == selectedSkillId);
            }

            for (var index = 0; index < _equippedSkillIcons.Count; index++)
            {
                var skill = profile.Skills[profile.EquippedSkillIndices[index]];
                BindSkillPreviewIcon(_equippedSkillIcons[index], skill, 68);
                _equippedSkillIcons[index].tooltip = skill.Name + " · " + skill.Level;
            }

            var selectedPotentialName = _characterHubSelectionState.PotentialNameFor(profile);
            for (var index = 0; index < _potentialPathNodes.Count; index++)
            {
                var potential = profile.Potentials[index];
                var icon = _potentialPathIcons[index];
                var sprite = _scene.GetMap01APotentialIconSprite(potential.IconId);
                icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
                _potentialPathTitles[index].text = potential.Name;
                _potentialPathValues[index].text = potential.Value;
                _potentialPathNodes[index].tooltip = potential.Name + " · " + potential.Value;
                ApplyPotentialNodeSelection(_potentialPathNodes[index], potential.Name == selectedPotentialName);
            }
            _potentialRecommendation.text = profile.Recommendation;

            var pet = profile.SpiritPet;
            _spiritPetPreviewTexture = Resources.Load<Texture2D>(pet.ArtResource);
            _spiritPetPortraitTexture = Resources.Load<Texture2D>(pet.PortraitResource);
            if (_spiritPetPreviewTexture == null)
            {
                _spiritPetPreview.style.backgroundImage = StyleKeyword.None;
                _spiritPetSelectedRosterArt.style.backgroundImage = StyleKeyword.None;
            }
            else
            {
                _spiritPetPreview.style.backgroundImage = new StyleBackground(_spiritPetPreviewTexture);
                _spiritPetSelectedRosterArt.style.backgroundImage = new StyleBackground(
                    _spiritPetPortraitTexture == null ? _spiritPetPreviewTexture : _spiritPetPortraitTexture);
            }
            _spiritPetIdentity.text = pet.Name;
            _spiritPetHeroLevel.text = pet.Level + " · Đồng hành " + profile.Label;
            _spiritPetSelectedRosterName.text = pet.Name;
            _spiritPetSelectedRosterLevel.text = pet.Level;
            _spiritPetRarityBadge.text = pet.Rarity;
            _spiritPetRoleBadge.text = pet.Role;
            _spiritPetStateBadge.text = pet.State;
            for (var index = 0; index < _hubSpiritPetStatValues.Count; index++)
                _hubSpiritPetStatValues[index].text = pet.Stats[index];
            for (var index = 0; index < _hubSpiritPetSkillIcons.Count; index++)
            {
                CharacterHubSpiritPetPreview.SkillPreview skill = pet.Skills[index];
                var sprite = _scene.GetMap01ASkillIconSprite(skill.IconId);
                _hubSpiritPetSkillIcons[index].style.backgroundImage = sprite == null
                    ? StyleKeyword.None : new StyleBackground(sprite);
                _hubSpiritPetSkillNames[index].text = skill.Name;
                _hubSpiritPetSkillLevels[index].text = skill.Level;
                _hubSpiritPetSkillDescriptions[index].text = skill.Description;
            }
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
            _activeCharacterHubPreviewMode = mode;
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

        private void SelectSkillNode(int index)
        {
            var skill = ActiveCharacterHubProfile.Skills[index];
            _characterHubSelectionState.SelectSkill(ActiveCharacterHubProfile, skill.Id);
            for (var nodeIndex = 0; nodeIndex < _skillPathNodes.Count; nodeIndex++)
                ApplyHubPathNodeSelection(_skillPathNodes[nodeIndex], nodeIndex == index);
            ShowSkillDetail(skill);
        }

        private void SelectPotentialNode(int index)
        {
            var potential = ActiveCharacterHubProfile.Potentials[index];
            _characterHubSelectionState.SelectPotential(ActiveCharacterHubProfile, potential.Name);
            for (var nodeIndex = 0; nodeIndex < _potentialPathNodes.Count; nodeIndex++)
                ApplyPotentialNodeSelection(_potentialPathNodes[nodeIndex], nodeIndex == index);
            ShowPotentialDetail(potential);
        }

        private void ConfigureHubDetailMode(CharacterHubMode mode)
        {
            _hubSkillActionRow.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _hubPotentialActionRow.style.display = mode == CharacterHubMode.Potential ? DisplayStyle.Flex : DisplayStyle.None;
            _hubSpiritPetActionRow.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
            _hubSpiritPetBadges.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
            _hubDetailMeta.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _hubPotentialSummary.style.display = mode == CharacterHubMode.Potential ? DisplayStyle.Flex : DisplayStyle.None;
            _hubDetailBody.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _hubPotentialFacts.style.display = mode == CharacterHubMode.Potential ? DisplayStyle.Flex : DisplayStyle.None;
            _hubSpiritPetFacts.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
            _hubDetailStatus.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _hubDetailIcon.style.width = _hubDetailIcon.style.height = mode == CharacterHubMode.Skills
                ? 116
                : mode == CharacterHubMode.Potential ? 104 : 76;
            _hubDetailName.style.fontSize = mode == CharacterHubMode.SpiritPet ? 24 : 28;
            _hubSpiritPetBadges.style.marginTop = mode == CharacterHubMode.SpiritPet ? 4 : 8;
            var detailHero = _hubPreviewDetailPanel.Q("Map01A Hub Preview Detail Hero");
            detailHero.style.marginTop = detailHero.style.marginBottom = mode == CharacterHubMode.SpiritPet ? 2 : 6;
            _hubDetailIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        private void SetHubDetailIcon(string iconId)
        {
            var sprite = _scene.GetMap01AHudIconSprite(iconId);
            _hubDetailIcon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
        }

        private void ShowSkillDetail(CharacterHubSkillPreview skill)
        {
            ConfigureHubDetailMode(CharacterHubMode.Skills);
            var sprite = GetSkillPreviewSprite(skill);
            _hubDetailIcon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            _hubDetailIcon.style.width = _hubDetailIcon.style.height = 116;
            _hubDetailHeader.text = "CHI TIẾT KỸ NĂNG";
            _hubDetailName.text = skill.Name;
            _hubDetailMeta.text = ActiveCharacterHubProfile.Label + " · Kỹ năng chủ động · " + skill.Level;
            _hubDetailBody.text = skill.Description;
            _hubDetailStatus.text = "Nâng cấp và thay đổi bộ kỹ năng đang khóa.";
        }

        private void ShowPotentialDetail(CharacterHubPotentialPreview potential)
        {
            ConfigureHubDetailMode(CharacterHubMode.Potential);
            var sprite = _scene.GetMap01APotentialIconSprite(potential.IconId);
            _hubDetailIcon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            _hubDetailIcon.style.width = _hubDetailIcon.style.height = 124;
            _hubDetailHeader.text = "CHI TIẾT TIỀM NĂNG";
            _hubDetailName.text = potential.Name;
            _hubDetailMeta.text = string.Empty;
            _hubPotentialSummary.text = potential.Summary;
            _hubPotentialCurrentLevel.text = "Cấp hiện tại:  " + potential.Value;
            _hubPotentialCurrentEffect.text = potential.CurrentEffect;
            _hubPotentialNextEffect.text = potential.NextEffect;
            _hubDetailStatus.text = string.Empty;
        }

        private void ShowHubDetail(CharacterHubMode mode)
        {
            if (mode == CharacterHubMode.Skills)
            {
                ShowSkillDetail(_characterHubSelectionState.SkillFor(ActiveCharacterHubProfile));
            }
            else if (mode == CharacterHubMode.Potential)
            {
                var potential = _characterHubSelectionState.PotentialFor(ActiveCharacterHubProfile);
                ShowPotentialDetail(potential);
            }
            else
            {
                ConfigureHubDetailMode(CharacterHubMode.SpiritPet);
                var pet = ActiveCharacterHubProfile.SpiritPet;
                var portrait = _spiritPetPortraitTexture == null ? _spiritPetPreviewTexture : _spiritPetPortraitTexture;
                _hubDetailIcon.style.backgroundImage = portrait == null
                    ? new StyleBackground(_scene.GetMap01AHudIconSprite("crest"))
                    : new StyleBackground(portrait);
                _hubDetailIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                _hubDetailHeader.text = "CHI TIẾT LINH THÚ";
                _hubDetailName.text = pet.Name;
                _hubDetailMeta.text = ActiveCharacterHubProfile.Label + " · " + pet.Level;
                _hubDetailStatus.text = "Tính năng bồi dưỡng đang khóa.";
            }
        }

    }
}
