using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    // Presentation only: the existing catalog uses two spaces between a caption
    // and a value. Never infer numbers, units or missing gameplay data.
    public sealed class CharacterHubFactRow : VisualElement
    {
        private readonly VisualElement _marker;
        public Label Caption { get; }
        public Label Value { get; }
        public string SourceText { get; private set; } = string.Empty;

        public CharacterHubFactRow(string rowName, int fontSize, bool accent)
        {
            name = rowName;
            AddToClassList("lgo-character-hub-fact-row");
            _marker = new VisualElement { name = rowName + " Marker", pickingMode = PickingMode.Ignore };
            Caption = new Label { name = rowName + " Caption" };
            Value = new Label { name = rowName + " Value" };
            Add(_marker);
            Add(Caption);
            Add(Value);
            CongDongLamArrivalHud.ApplyLgoCharacterHubFactRow(this, _marker, Caption, Value, fontSize, accent);
        }

        public void Bind(string source)
        {
            SourceText = source ?? string.Empty;
            var split = SourceText.IndexOf("  ", StringComparison.Ordinal);
            var paired = split > 0 && !string.IsNullOrWhiteSpace(SourceText.Substring(split + 2));
            Caption.text = paired ? SourceText.Substring(0, split).TrimEnd() : SourceText;
            Value.text = paired ? SourceText.Substring(split + 2).TrimStart() : string.Empty;
            _marker.style.display = paired ? DisplayStyle.Flex : DisplayStyle.None;
            Value.style.display = paired ? DisplayStyle.Flex : DisplayStyle.None;
            style.minHeight = string.IsNullOrWhiteSpace(SourceText) ? 10 : 26;
        }
    }

    public sealed class CharacterHubFactBlock : VisualElement
    {
        private readonly CharacterHubFactRow[] _rows;
        private readonly Label _overflow;
        private bool _bound;
        public string SourceText { get; private set; } = string.Empty;
        public IReadOnlyList<CharacterHubFactRow> Rows { get; }

        public CharacterHubFactBlock(string blockName, int capacity, int fontSize, bool accent)
        {
            if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity));
            name = blockName;
            AddToClassList("lgo-character-hub-fact-block");
            style.flexShrink = 0;
            style.minWidth = 0;
            style.fontSize = fontSize;
            _rows = new CharacterHubFactRow[capacity];
            Rows = Array.AsReadOnly(_rows);
            for (var i = 0; i < capacity; i++)
            {
                _rows[i] = new CharacterHubFactRow(blockName + " Row " + i, fontSize, accent);
                _rows[i].style.display = DisplayStyle.None;
                Add(_rows[i]);
            }
            _overflow = new Label { name = blockName + " Overflow Text" };
            _overflow.style.whiteSpace = WhiteSpace.Normal;
            _overflow.style.display = DisplayStyle.None;
            _overflow.style.minWidth = 0;
            Add(_overflow);
            CongDongLamArrivalHud.ApplyLgoCharacterHubFactCopy(_overflow, fontSize, accent);
        }

        public void Bind(string source)
        {
            source = source ?? string.Empty;
            if (_bound && source == SourceText) return;
            _bound = true;
            SourceText = source;
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var overflow = lines.Length > _rows.Length;
            for (var i = 0; i < _rows.Length; i++)
            {
                var visible = !overflow && source.Length > 0 && i < lines.Length;
                _rows[i].Bind(visible ? lines[i] : string.Empty);
                _rows[i].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
            // Preserve unexpected longer content as one wrapping paragraph, rather
            // than dropping lines or changing the prebuilt hierarchy on refresh.
            _overflow.text = overflow ? source : string.Empty;
            _overflow.style.display = overflow ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
