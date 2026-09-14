using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private enum ServerSelectReturnTarget
        {
            Entry,
            CharacterSelect
        }

        private VisualElement _serverSelectOverlay;
        private Label _serverSelectStatus;
        private bool _serverSelectOpen;
        private ServerSelectReturnTarget _serverSelectReturnTarget = ServerSelectReturnTarget.Entry;

        private void BuildServerSelect()
        {
            _serverSelectOverlay = new VisualElement { name = "Map01A Server Select Overlay" };
            ApplyLgoServerSelectPanel(_serverSelectOverlay);

            var title = LgoTitleLabel("CHỌN MÁY CHỦ", 27, TextAnchor.MiddleCenter);
            title.name = "Map01A Server Select Title";
            title.style.whiteSpace = WhiteSpace.NoWrap;
            _serverSelectOverlay.Add(title);

            var subtitle = LgoSubtitleLabel("Chọn nơi bắt đầu hành trình", 14, TextAnchor.MiddleCenter);
            subtitle.name = "Map01A Server Select Subtitle";
            subtitle.style.marginTop = 2;
            _serverSelectOverlay.Add(subtitle);

            var server = new Button(SelectCurrentServer)
            {
                name = "Map01A Server Select Card",
                text = string.Empty
            };
            ApplyLgoServerSelectCard(server);
            var icon = CreateLgoEntryIcon("Map01A Server Select Icon", _scene.GetMap01AHudIconSprite("server"), 48);
            icon.style.marginRight = 18;
            server.Add(icon);

            var identity = new VisualElement { name = "Map01A Server Select Identity", pickingMode = PickingMode.Ignore };
            identity.style.flexGrow = 1;
            var serverName = LgoTitleLabel("S1 · Đông Lâm", 22);
            serverName.name = "Map01A Server Select Name";
            serverName.style.whiteSpace = WhiteSpace.NoWrap;
            identity.Add(serverName);
            var current = LgoSubtitleLabel("Máy chủ hiện tại", 13);
            current.name = "Map01A Server Select Current";
            current.style.marginTop = 4;
            identity.Add(current);
            server.Add(identity);

            var stateDot = new VisualElement { name = "Map01A Server Select State Dot", pickingMode = PickingMode.Ignore };
            stateDot.style.width = stateDot.style.height = 14;
            stateDot.style.marginRight = 8;
            stateDot.style.backgroundColor = new Color(.52f, 1f, .18f, 1f);
            stateDot.style.borderTopLeftRadius = stateDot.style.borderTopRightRadius = 7;
            stateDot.style.borderBottomLeftRadius = stateDot.style.borderBottomRightRadius = 7;
            server.Add(stateDot);
            var serverState = LgoLabel("Mượt", 17, new Color(.62f, 1f, .30f, 1f), true);
            serverState.name = "Map01A Server Select State";
            serverState.style.whiteSpace = WhiteSpace.NoWrap;
            server.Add(serverState);
            _serverSelectOverlay.Add(server);

            _serverSelectStatus = LgoSubtitleLabel("Sẵn sàng kết nối.", 14, TextAnchor.MiddleCenter);
            _serverSelectStatus.name = "Map01A Server Select Status";
            _serverSelectStatus.style.minHeight = 28;
            _serverSelectStatus.style.marginBottom = 12;
            _serverSelectOverlay.Add(_serverSelectStatus);

            var actions = new VisualElement { name = "Map01A Server Select Actions" };
            actions.style.flexDirection = FlexDirection.Row;
            var back = new Button(() => CloseServerSelect(false))
            {
                name = "Map01A Server Select Back",
                text = "Quay lại"
            };
            var confirm = new Button(() => CloseServerSelect(true))
            {
                name = "Map01A Server Select Confirm",
                text = "Xác nhận"
            };
            ApplyLgoServerSelectAction(back, false);
            ApplyLgoServerSelectAction(confirm, true);
            back.style.marginRight = 12;
            actions.Add(back);
            actions.Add(confirm);
            _serverSelectOverlay.Add(actions);

            _entryPanel.Add(_serverSelectOverlay);
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-server-select-capture") >= 0)
                OpenServerSelect(ServerSelectReturnTarget.Entry);
            else
            {
                _serverSelectOverlay.style.display = DisplayStyle.None;
                _entryControlCard.style.display = DisplayStyle.Flex;
            }
        }

        private void SelectCurrentServer()
        {
            if (_serverSelectStatus != null)
                _serverSelectStatus.text = "S1 · Đông Lâm đã được chọn.";
        }

        private void OpenServerSelect(ServerSelectReturnTarget returnTarget)
        {
            _serverSelectReturnTarget = returnTarget;
            _serverSelectOpen = true;
            _registerOpen = false;
            _entryOpen = true;
            _characterSelectOpen = false;
            UpdateCharacterSelectScreen();
            UpdateEntryScreen();
            UpdateServerSelectScreen();
        }

        private void CloseServerSelect(bool confirm)
        {
            var returnTarget = _serverSelectReturnTarget;
            _serverSelectOpen = false;
            if (returnTarget == ServerSelectReturnTarget.CharacterSelect)
            {
                _entryOpen = false;
                _characterSelectOpen = true;
                if (confirm) SetCharacterSelectStatus("Máy chủ đã chọn · S1 Đông Lâm.");
            }
            else
            {
                _entryOpen = true;
                _characterSelectOpen = false;
                if (confirm && _entryStatus != null)
                    _entryStatus.text = "Máy chủ đã chọn · S1 Đông Lâm.";
            }
            UpdateServerSelectScreen();
            UpdateCharacterSelectScreen();
            UpdateEntryScreen();
        }

        private void UpdateServerSelectScreen()
        {
            if (_serverSelectOverlay == null) return;
            _serverSelectOverlay.style.display = _serverSelectOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateEntryControlCardVisibility();
            UpdateHudShellVisibility();
        }
    }
}
