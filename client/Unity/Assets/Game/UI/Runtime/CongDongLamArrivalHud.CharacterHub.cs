using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private enum CharacterHubMode { Skills, Potential, SpiritPet }

        private VisualElement _skillsPanel, _potentialPanel, _spiritPetPanel, _hubPreviewDetailPanel;
        private Label _hubDetailHeader, _hubDetailName, _hubDetailMeta, _hubDetailBody, _hubDetailStatus;
        private VisualElement _hubDetailIcon;
        private Button _hubSkillUpgradeAction, _potentialAddPointAction, _spiritPetDevelopAction;
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

        private Button CreateHubRailControl(string name, string text, bool selected)
        {
            var button = InventoryButton(() => { }, name, text);
            ApplyLgoInventoryFilterChip(button, _touch);
            button.style.flexBasis = StyleKeyword.Auto;
            button.style.marginRight = 0;
            button.style.marginBottom = 7;
            ApplyLgoSelectedTab(button, selected);
            if (!selected) ApplyLgoDisabledAction(button);
            return button;
        }

        private Button CreateHubPathNode(string name, string title, string level, string iconId, Action action, bool selected = false)
        {
            var node = InventoryButton(action, name);
            node.style.width = 142;
            node.style.minWidth = 142;
            node.style.maxWidth = 142;
            node.style.flexBasis = 142;
            node.style.flexGrow = 0;
            node.style.height = 104;
            node.style.flexShrink = 0;
            node.style.flexDirection = FlexDirection.Column;
            node.style.alignItems = Align.Center;
            node.style.justifyContent = Justify.Center;
            node.style.borderTopLeftRadius = node.style.borderTopRightRadius = 52;
            node.style.borderBottomLeftRadius = node.style.borderBottomRightRadius = 52;
            node.Add(HubIcon(name + " Icon", iconId, 48));
            var titleLabel = LgoLabel(title, 12, UiText, true);
            titleLabel.name = name + " Title";
            titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            titleLabel.style.marginTop = 2;
            titleLabel.style.whiteSpace = WhiteSpace.NoWrap;
            node.Add(titleLabel);
            var levelLabel = LgoLabel(level, 10, new Color(.74f, .92f, 1f, .92f), true);
            levelLabel.name = name + " Level";
            levelLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            node.Add(levelLabel);
            ApplyHubPathNodeSelection(node, selected);
            return node;
        }


        private static void ApplyHubPathNodeSelection(Button node, bool selected)
        {
            ApplyLgoFrame(node,
                selected ? new Color(.025f, .18f, .30f, .96f) : new Color(.025f, .085f, .13f, .92f),
                selected ? UiGold : new Color(.28f, .55f, .70f, .78f));
            var nodeBorderWidth = selected ? 2 : 1;
            node.style.borderLeftWidth = node.style.borderRightWidth = nodeBorderWidth;
            node.style.borderTopWidth = node.style.borderBottomWidth = nodeBorderWidth;
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
            var node = CreateHubPathNode(name, title, value, iconId,
                () => SelectPotentialNode(name, title, value, iconId), selected);
            _potentialPathNodes.Add(node);
            node.style.position = Position.Absolute;
            node.style.left = left;
            node.style.top = top;
            node.style.width = 116;
            node.style.minWidth = 116;
            node.style.maxWidth = 116;
            node.style.flexBasis = 116;
            node.style.height = 116;
            node.style.borderTopLeftRadius = node.style.borderTopRightRadius = 58;
            node.style.borderBottomLeftRadius = node.style.borderBottomRightRadius = 58;
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
            ApplyLgoSelectedTab(card, selected);
            if (!selected) ApplyLgoDisabledAction(card);
            return card;
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
            content.style.flexDirection = FlexDirection.Row;
            content.style.flexGrow = 1;
            _skillsPanel.Add(content);

            var rail = new VisualElement { name = "Map01A Skills Category Rail" };
            rail.style.flexDirection = FlexDirection.Column;
            rail.style.width = 116;
            rail.style.marginRight = 12;
            rail.Add(CreateHubRailControl("Map01A Active Skills Category", "Chủ động", true));
            rail.Add(CreateHubRailControl("Map01A Passive Skills Category", "Bị động", false));
            rail.Add(CreateHubRailControl("Map01A Method Skills Category", "Tâm pháp", false));
            content.Add(rail);

            var skillArea = new VisualElement { name = "Map01A Skills Grid Area" };
            skillArea.style.flexGrow = 1;
            skillArea.style.minWidth = 0;
            var pointRow = InventoryRow("Map01A Skill Point Row");
            pointRow.style.alignItems = Align.Center;
            pointRow.style.justifyContent = Justify.SpaceBetween;
            pointRow.Add(LgoTitleLabel("Lộ trình Thiên Kiếm", 17));
            pointRow.Add(InventoryBadge("Map01A Skill Points Badge", "Điểm kỹ năng: 12", UiGold));
            skillArea.Add(pointRow);

            var path = new VisualElement { name = "Map01A Skill Progression Path" };
            path.style.flexGrow = 1;
            path.style.alignItems = Align.Center;
            var stages = new[]
            {
                new[] { ("Thiên Kiếm Quyết", "Lv.8", "skill"), ("Lăng Không Bộ", "Lv.5", "run"), ("Kiếm Vũ", "Lv.4", "attack") },
                new[] { ("Hộ Thể", "Lv.3", "skills"), ("Song Kiếm", "Lv.6", "attack"), ("Phong Trảm", "Lv.2", "skill") },
                new[] { ("Kiếm Trận", "Lv.1", "crest"), ("Ngự Kiếm", "Lv.3", "jump"), ("Vạn Kiếm", "Lv.1", "skills") }
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
                        () => SelectSkillNode(nodeName, skill.Item1, skill.Item2, skill.Item3), stageIndex == 0 && nodeIndex == 0);
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
            var equippedRow = InventoryRow("Map01A Equipped Skill Strip");
            equippedRow.style.alignItems = Align.Center;
            equippedRow.style.justifyContent = Justify.SpaceBetween;
            equippedRow.Add(InventoryBadge("Map01A Equipped Skill Summary", "Kỹ năng đã trang bị", UiSubText));
            foreach (var icon in new[] { "skill", "run", "attack", "skills" }) equippedRow.Add(HubIcon("Map01A Equipped Skill " + icon, icon, 46));
            equippedRow.Add(InventoryBadge("Map01A Equipped Skill Count", "4/4", new Color(.76f, 1f, .70f, .94f)));
            skillArea.Add(equippedRow);
            content.Add(skillArea);
            body.Add(_skillsPanel);
        }

        private void InitializePotentialView(VisualElement body)
        {
            _potentialPanel = CreateHubSurface("Map01A Potential Panel");
            var heading = InventoryRow("Map01A Potential Heading");
            heading.style.alignItems = Align.Center;
            heading.style.justifyContent = Justify.SpaceBetween;
            heading.Add(LgoTitleLabel("Kinh mạch tiềm năng", 18));
            heading.Add(InventoryBadge("Map01A Potential Remaining Points", "Điểm còn lại: 12", UiGold));
            _potentialPanel.Add(heading);

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
            core.Add(HubIcon("Map01A Potential Core Icon", "character", 80));
            var coreText = LgoLabel("TÂM MẠCH", 12, UiGold, true);
            coreText.style.unityTextAlign = TextAnchor.MiddleCenter;
            core.Add(coreText);
            diagram.Add(core);
            diagram.Add(CreatePotentialNode("Map01A Potential Node Công", "Công", "120", "attack", 267, 0));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Thủ", "Thủ", "118", "lock", 20, 132));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Sinh lực", "Sinh lực", "250", "character", 514, 132, true));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Linh lực", "Linh lực", "96", "skill", 142, 274));
            diagram.Add(CreatePotentialNode("Map01A Potential Node Nhanh nhẹn", "Nhanh nhẹn", "110", "run", 392, 274));
            _potentialPanel.Add(diagram);
            var recommendation = InventoryBadge("Map01A Potential Recommendation", "Đề xuất Võ: cân bằng Công · Sinh lực · Nhanh nhẹn", new Color(.74f, .92f, 1f, .94f));
            recommendation.style.alignSelf = Align.Center;
            _potentialPanel.Add(recommendation);
            body.Add(_potentialPanel);
        }

        private void InitializeSpiritPetView(VisualElement body)
        {
            _spiritPetPanel = CreateHubSurface("Map01A Spirit Pet Panel");
            _spiritPetPreviewTexture = Resources.Load<Texture2D>("LGOMaps/CongDongLamMap01ACharacterHub/spirit-fox-preview");
            var preview = new VisualElement { name = "Map01A Spirit Pet Preview Art" };
            ApplyLgoDetailCard(preview, 10, 8);
            preview.style.height = 382;
            preview.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            preview.style.backgroundImage = _spiritPetPreviewTexture == null ? StyleKeyword.None : new StyleBackground(_spiritPetPreviewTexture);
            _spiritPetPanel.Add(preview);
            var identity = LgoTitleLabel("Thanh Vân Hồ · Lv.20", 19);
            identity.style.unityTextAlign = TextAnchor.MiddleCenter;
            _spiritPetPanel.Add(identity);
            var growth = InventoryBadge("Map01A Spirit Pet Growth", "Thân mật 320/600  ·  Tăng trưởng 180/300", new Color(.74f, .92f, 1f, .94f));
            growth.style.alignSelf = Align.Center;
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
            ApplyLgoDetailCard(_hubPreviewDetailPanel);
            _hubDetailHeader = LgoLabel("CHI TIẾT", 14, UiSubText, true);
            _hubPreviewDetailPanel.Add(_hubDetailHeader);
            _hubDetailIcon = HubIcon("Map01A Hub Preview Detail Icon", "skill", 96);
            _hubDetailIcon.style.alignSelf = Align.Center;
            _hubDetailIcon.style.marginTop = 12;
            _hubDetailIcon.style.marginBottom = 10;
            _hubPreviewDetailPanel.Add(_hubDetailIcon);
            _hubDetailName = LgoTitleLabel("", 20);
            _hubPreviewDetailPanel.Add(_hubDetailName);
            _hubDetailMeta = LgoSubtitleLabel("", 13);
            _hubDetailMeta.style.marginTop = 4;
            _hubPreviewDetailPanel.Add(_hubDetailMeta);
            var facts = new VisualElement { name = "Map01A Hub Preview Detail Facts" };
            ApplyLgoInventoryStatsCard(facts);
            _hubDetailBody = LgoLabel("", 13, UiText);
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

            _hubSkillUpgradeAction = InventoryButton(() => { }, "Map01A Skill Upgrade Action", "Nâng cấp");
            _potentialAddPointAction = InventoryButton(() => { }, "Map01A Potential Add Point", "Cộng 1 điểm");
            _spiritPetDevelopAction = InventoryButton(() => { }, "Map01A Spirit Pet Develop Action", "Bồi dưỡng");
            foreach (var action in new[] { _hubSkillUpgradeAction, _potentialAddPointAction, _spiritPetDevelopAction })
            {
                action.style.marginTop = 12;
                action.style.flexShrink = 0;
                ApplyLgoDisabledAction(action);
                _hubPreviewDetailPanel.Add(action);
            }
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
                ? "Xem bộ kỹ năng hiện có và trạng thái trang bị"
                : mode == CharacterHubMode.Potential
                    ? "Xem hướng phát triển; chưa cộng điểm khi chưa có state chính thức"
                    : "Xem linh thú và kỹ năng đồng hành";
            ApplyHubMainTabSelection(mode);
            ShowHubDetail(mode);
        }

        private void ApplyHubMainTabSelection(CharacterHubMode? previewMode = null, bool character = false, bool storage = false)
        {
            ApplyLgoSelectedTab(_characterInfoTab, character);
            ApplyLgoSelectedTab(_bagTab, storage);
            ApplyLgoSelectedTab(_skillsTab, previewMode == CharacterHubMode.Skills);
            ApplyLgoSelectedTab(_potentialTab, previewMode == CharacterHubMode.Potential);
            ApplyLgoSelectedTab(_spiritPetTab, previewMode == CharacterHubMode.SpiritPet);
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
            _hubSkillUpgradeAction.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _potentialAddPointAction.style.display = mode == CharacterHubMode.Potential ? DisplayStyle.Flex : DisplayStyle.None;
            _spiritPetDevelopAction.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
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
            SetHubDetailIcon(iconId);
            _hubDetailHeader.text = "CHI TIẾT KỸ NĂNG";
            _hubDetailName.text = title;
            _hubDetailMeta.text = "Kỹ năng chủ động · " + level;
            _hubDetailBody.text = title == "Thiên Kiếm Quyết"
                ? "Sát thương: 320% Công\nHồi chiêu: 12 giây\nTiêu hao MP: 180\n\nBộ bốn kỹ năng hiện hành được giữ nguyên."
                : "Cấp hiện hành: " + level + "\n\nHiệu ứng chi tiết đang chờ dữ liệu kỹ năng chính thức.";
            _hubDetailStatus.text = "Nâng cấp chờ hệ thống kỹ năng chính thức.";
        }

        private void ShowPotentialDetail(string title, string value, string iconId)
        {
            ConfigureHubDetailMode(CharacterHubMode.Potential);
            SetHubDetailIcon(iconId);
            _hubDetailHeader.text = "CHI TIẾT TIỀM NĂNG";
            _hubDetailName.text = title;
            _hubDetailMeta.text = "Giá trị xem trước: " + value;
            _hubDetailBody.text = title == "Sinh lực"
                ? "Ảnh hưởng dự kiến\n• Sinh lực (HP)\n• Phòng thủ\n\nKhông thay đổi chỉ số local khi chưa có state tiến trình."
                : "Điểm đang chọn: " + title + " · " + value + "\n\nKhông thay đổi chỉ số local khi chưa có state tiến trình.";
            _hubDetailStatus.text = "Cộng điểm đang khóa an toàn.";
        }

        private void ShowHubDetail(CharacterHubMode mode)
        {
            if (mode == CharacterHubMode.Skills)
            {
                ShowSkillDetail("Thiên Kiếm Quyết", "Lv.8", "skill");
            }
            else if (mode == CharacterHubMode.Potential)
            {
                ShowPotentialDetail("Sinh lực", "250", "character");
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
                _hubDetailMeta.text = "Tinh phẩm · Hỗ trợ · Lv.20";
                _hubDetailBody.text = "Kỹ năng Linh thú\n• Thanh Vân Hộ Thể\n• Cửu Vĩ Linh Phong\n\nMàn này chưa tự ghi tăng trưởng hoặc chiến đấu vào state.";
                _hubDetailStatus.text = "Bồi dưỡng chờ hệ thống Linh thú chính thức.";
            }
        }

    }
}
