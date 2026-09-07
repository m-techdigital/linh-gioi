using System;

namespace LinhGioi.World
{
    public sealed class NpcDialogueSession
    {
        private readonly string[] _lines;
        private readonly string _information;
        private int _index;

        public NpcDialogueSession(string speaker, string[] lines, string informationAction = null,
            string information = null, string completionAction = "Hoàn tất")
        {
            if (string.IsNullOrWhiteSpace(speaker)) throw new ArgumentException("Speaker is required.", nameof(speaker));
            if (lines == null || lines.Length == 0) throw new ArgumentException("Dialogue needs lines.", nameof(lines));
            foreach (var line in lines)
                if (string.IsNullOrWhiteSpace(line)) throw new ArgumentException("Dialogue lines must not be empty.", nameof(lines));
            if ((informationAction == null) != (information == null) ||
                (information != null && (string.IsNullOrWhiteSpace(information) || string.IsNullOrWhiteSpace(informationAction))))
                throw new ArgumentException("Information needs both an action and non-empty text.");
            if (string.IsNullOrWhiteSpace(completionAction)) throw new ArgumentException("Completion action is required.", nameof(completionAction));
            Speaker = speaker;
            _lines = (string[])lines.Clone();
            _information = information;
            InformationAction = informationAction;
            CompletionAction = completionAction;
        }

        public string Speaker { get; }
        public bool Active { get; private set; }
        public bool Completed { get; private set; }
        public string InformationAction { get; }
        public string CompletionAction { get; }
        public bool ReadingInformation { get; private set; }
        public bool CanReadInformation => Active && !HasNext && !ReadingInformation && _information != null;
        public string Line => Active ? (ReadingInformation ? _information : _lines[_index]) : string.Empty;
        public string Progress => (Active ? _index + 1 : 0) + "/" + _lines.Length;
        public bool HasNext => Active && _index < _lines.Length - 1;

        public bool ReadInformation()
        {
            if (!CanReadInformation) return false;
            ReadingInformation = true;
            return true;
        }

        public bool Open()
        {
            if (Active) return false;
            Reset();
            Active = true;
            return true;
        }

        public bool Advance()
        {
            if (!Active) return false;
            if (ReadingInformation)
            {
                ReadingInformation = false;
                return true;
            }
            if (HasNext) _index++;
            else
            {
                Active = false;
                Completed = true;
            }
            return true;
        }

        public bool Close()
        {
            if (!Active) return false;
            Reset();
            return true;
        }

        public void Reset()
        {
            Active = false;
            Completed = false;
            ReadingInformation = false;
            _index = 0;
        }
    }
}
