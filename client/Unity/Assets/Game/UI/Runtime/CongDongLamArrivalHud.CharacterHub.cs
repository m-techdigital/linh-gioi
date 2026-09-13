using System;
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
            return button;
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
            pointRow.Add(LgoLabel("Điểm kỹ năng: 12", 14, UiGold, true));
            skillArea.Add(pointRow);
            var grid = new VisualElement { name = "Map01A Skill Grid" };
            grid.style.flexDirection = FlexDirection.Row;
            grid.style.flexWrap = Wrap.Wrap;
            foreach (var skill in new[]
            {
                ("Thiên Kiếm Quyết", "Lv.8", "skill"), ("Lăng Không Bộ", "Lv.5", "run"),
                ("Kiếm Vũ", "Lv.4", "attack"), ("Hộ Thể", "Lv.3", "skills"),
                ("Song Kiếm", "Lv.6", "attack"), ("Phong Trảm", "Lv.2", "skill"),
                ("Kiếm Trận", "Lv.1", "crest"), ("Ngự Kiếm", "Lv.3", "jump"),
                ("Vạn Kiếm", "Lv.1", "skills")
            })
                grid.Add(CreateHubTile("Map01A Skill Card " + skill.Item1, skill.Item1, skill.Item2, skill.Item3,
                    () => ShowHubDetail(CharacterHubMode.Skills)));
            skillArea.Add(grid);
            var equipped = InventoryBadge("Map01A Equipped Skill Summary", "Đã trang bị 4/4 kỹ năng chủ động", new Color(.76f, 1f, .70f, .94f));
            equipped.style.marginTop = 8;
            skillArea.Add(equipped);
            content.Add(skillArea);
            body.Add(_skillsPanel);
        }

        private void InitializePotentialView(VisualElement body)
        {
            _potentialPanel = CreateHubSurface("Map01A Potential Panel");
            _potentialPanel.Add(LgoTitleLabel("Kinh mạch tiềm năng", 19));
            var intro = LgoSubtitleLabel("Chọn một thuộc tính để xem hiệu quả. Cộng điểm chờ state tiến trình chính thức.", 12);
            intro.style.marginBottom = 12;
            _potentialPanel.Add(intro);
            var nodes = new VisualElement { name = "Map01A Potential Node Grid" };
            nodes.style.flexDirection = FlexDirection.Row;
            nodes.style.flexWrap = Wrap.Wrap;
            foreach (var node in new[]
            {
                ("Công", "120", "attack"), ("Thủ", "118", "lock"), ("Sinh lực", "250", "character"),
                ("Linh lực", "96", "skill"), ("Nhanh nhẹn", "110", "run")
            })
            {
                var card = CreateHubTile("Map01A Potential Node " + node.Item1, node.Item1, node.Item2, node.Item3,
                    () => ShowHubDetail(CharacterHubMode.Potential));
                card.style.flexBasis = new Length(30.5f, LengthUnit.Percent);
                nodes.Add(card);
            }
            _potentialPanel.Add(nodes);
            var remaining = InventoryBadge("Map01A Potential Remaining Points", "Điểm tiềm năng còn lại: 12", UiGold);
            remaining.style.marginTop = 12;
            _potentialPanel.Add(remaining);
            body.Add(_potentialPanel);
        }

        private void InitializeSpiritPetView(VisualElement body)
        {
            _spiritPetPanel = CreateHubSurface("Map01A Spirit Pet Panel");
            var preview = new VisualElement { name = "Map01A Spirit Pet Preview Art" };
            ApplyLgoDetailCard(preview, 10, 8);
            preview.style.height = 390;
            preview.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            var texture = Resources.Load<Texture2D>("LGOMaps/CongDongLamMap01ACharacterHub/spirit-fox-preview");
            preview.style.backgroundImage = texture == null ? StyleKeyword.None : new StyleBackground(texture);
            _spiritPetPanel.Add(preview);
            var identity = LgoTitleLabel("Thanh Vân Hồ · Lv.20", 19);
            identity.style.unityTextAlign = TextAnchor.MiddleCenter;
            _spiritPetPanel.Add(identity);
            var growth = InventoryBadge("Map01A Spirit Pet Growth", "Thân mật 320/600  ·  Tăng trưởng 180/300", new Color(.74f, .92f, 1f, .94f));
            growth.style.alignSelf = Align.Center;
            _spiritPetPanel.Add(growth);
            var roster = InventoryRow("Map01A Spirit Pet Roster");
            roster.style.marginTop = 10;
            foreach (var pet in new[] { "Thanh Vân Hồ", "Xích Diệm Khuyển", "Trúc Linh", "Lam Vũ Điểu" })
            {
                var card = CreateHubTile("Map01A Spirit Pet Card " + pet, pet, pet == "Thanh Vân Hồ" ? "Đang chọn" : "Chưa thức tỉnh", "crest",
                    () => ShowHubDetail(CharacterHubMode.SpiritPet));
                card.style.flexBasis = new Length(23, LengthUnit.Percent);
                card.style.height = 92;
                roster.Add(card);
            }
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

        private void ShowHubDetail(CharacterHubMode mode)
        {
            var iconId = mode == CharacterHubMode.Skills ? "skill" : mode == CharacterHubMode.Potential ? "character" : "crest";
            var sprite = _scene.GetMap01AHudIconSprite(iconId);
            _hubDetailIcon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            _hubSkillUpgradeAction.style.display = mode == CharacterHubMode.Skills ? DisplayStyle.Flex : DisplayStyle.None;
            _potentialAddPointAction.style.display = mode == CharacterHubMode.Potential ? DisplayStyle.Flex : DisplayStyle.None;
            _spiritPetDevelopAction.style.display = mode == CharacterHubMode.SpiritPet ? DisplayStyle.Flex : DisplayStyle.None;
            if (mode == CharacterHubMode.Skills)
            {
                _hubDetailHeader.text = "CHI TIẾT KỸ NĂNG";
                _hubDetailName.text = "Thiên Kiếm Quyết";
                _hubDetailMeta.text = "Kỹ năng chủ động · Lv.8/10";
                _hubDetailBody.text = "Sát thương: 320% Công\nHồi chiêu: 12 giây\nTiêu hao MP: 180\n\nBộ bốn kỹ năng hiện hành được giữ nguyên.";
                _hubDetailStatus.text = "Nâng cấp chờ hệ thống kỹ năng chính thức.";
            }
            else if (mode == CharacterHubMode.Potential)
            {
                _hubDetailHeader.text = "CHI TIẾT TIỀM NĂNG";
                _hubDetailName.text = "Sinh lực";
                _hubDetailMeta.text = "Giá trị xem trước: 250";
                _hubDetailBody.text = "Ảnh hưởng dự kiến\n• Sinh lực (HP)\n• Phòng thủ\n\nKhông thay đổi chỉ số local khi chưa có state tiến trình.";
                _hubDetailStatus.text = "Cộng điểm đang khóa an toàn.";
            }
            else
            {
                _hubDetailHeader.text = "CHI TIẾT LINH THÚ";
                _hubDetailName.text = "Thanh Vân Hồ";
                _hubDetailMeta.text = "Tinh phẩm · Hỗ trợ · Lv.20";
                _hubDetailBody.text = "Kỹ năng Linh thú\n• Thanh Vân Hộ Thể\n• Cửu Vĩ Linh Phong\n\nMàn này chưa tự ghi tăng trưởng hoặc chiến đấu vào state.";
                _hubDetailStatus.text = "Bồi dưỡng chờ hệ thống Linh thú chính thức.";
            }
        }
    }
}
