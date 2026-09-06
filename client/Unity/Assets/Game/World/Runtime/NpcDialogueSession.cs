using System;

namespace LinhGioi.World
{
    public sealed class NpcDialogueSession
    {
        private readonly string[] _lines;
        private int _index;

        public NpcDialogueSession(string speaker, string[] lines)
        {
            if (string.IsNullOrWhiteSpace(speaker)) throw new ArgumentException("Speaker is required.", nameof(speaker));
            if (lines == null || lines.Length == 0) throw new ArgumentException("Dialogue needs lines.", nameof(lines));
            foreach (var line in lines)
                if (string.IsNullOrWhiteSpace(line)) throw new ArgumentException("Dialogue lines must not be empty.", nameof(lines));
            Speaker = speaker;
            _lines = (string[])lines.Clone();
        }

        public string Speaker { get; }
        public bool Active { get; private set; }
        public bool Completed { get; private set; }
        public string Line => Active ? _lines[_index] : string.Empty;
        public string Progress => (Active ? _index + 1 : 0) + "/" + _lines.Length;
        public bool HasNext => Active && _index < _lines.Length - 1;

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
            _index = 0;
        }
    }
}
