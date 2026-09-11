using System;
using System.Collections.Generic;

namespace LinhGioi.World
{
    public sealed class TwoDCharacterRuntimeState
    {
        private readonly string[] _modes;
        private readonly string[] _genders;
        private readonly int[] _levels;
        private readonly string[] _equipmentSlots;
        private readonly HashSet<string> _equippedSlots = new HashSet<string>(StringComparer.Ordinal);
        private int _modeIndex;
        private int _genderIndex;
        private int _levelIndex;
        private int _equipmentSlotIndex;
        private float _actionDuration;
        private float _movementHold;

        public TwoDCharacterRuntimeState(string[] modes, string[] genders, int[] levels, string[] equipmentSlots)
        {
            _modes = RequireValues(modes, nameof(modes));
            _genders = RequireValues(genders, nameof(genders));
            _levels = levels == null ? throw new ArgumentNullException(nameof(levels)) : (int[])levels.Clone();
            _equipmentSlots = RequireValues(equipmentSlots, nameof(equipmentSlots));
            if (_levels.Length == 0) throw new ArgumentException("At least one level is required", nameof(levels));
            EquipAllExcept(null);
        }

        public string Mode => _modes[_modeIndex];
        public string Gender => _genders[_genderIndex];
        public int Level => _levels[_levelIndex];
        public int[] AvailableLevels => (int[])_levels.Clone();
        public string SelectedEquipmentSlot => _equipmentSlots[_equipmentSlotIndex];
        public int EquippedSlotCount => _equippedSlots.Count;
        public string MotionState { get; private set; } = "idle";
        public bool RunEnabled { get; private set; }
        public int FacingSign { get; private set; } = 1;
        public void FaceMovement(float axis)
        {
            if (axis > .01f) FacingSign = 1;
            else if (axis < -.01f) FacingSign = -1;
        }
        public float AnimationPhase { get; private set; }
        public float ActionRemaining { get; private set; }
        public float ActionProgress => _actionDuration <= 0 ? 0 : 1f - ActionRemaining / _actionDuration;
        public bool HasActiveAction => ActionRemaining > 0;

        public void CycleMode() => _modeIndex = (_modeIndex + 1) % _modes.Length;
        public void CycleGender() => _genderIndex = (_genderIndex + 1) % _genders.Length;
        public void CycleLevel() => _levelIndex = (_levelIndex + 1) % _levels.Length;
        public void CycleEquipmentSlot() => _equipmentSlotIndex = (_equipmentSlotIndex + 1) % _equipmentSlots.Length;

        public bool IsEquipped(string slot) => _equippedSlots.Contains(slot);

        public void ToggleSelectedEquipmentSlot()
        {
            var slot = SelectedEquipmentSlot;
            if (!_equippedSlots.Remove(slot)) _equippedSlots.Add(slot);
        }

        public void EquipAllExcept(string excludedSlot)
        {
            if (excludedSlot != null && Array.IndexOf(_equipmentSlots, excludedSlot) < 0)
                throw new ArgumentException("Unknown equipment slot: " + excludedSlot, nameof(excludedSlot));
            _equippedSlots.Clear();
            foreach (var slot in _equipmentSlots)
                if (slot != excludedSlot) _equippedSlots.Add(slot);
        }

        public void SetPresentation(int genderIndex, int levelIndex, int modeIndex, int equipmentSlotIndex)
        {
            _genderIndex = RequireIndex(genderIndex, _genders.Length, nameof(genderIndex));
            _levelIndex = RequireIndex(levelIndex, _levels.Length, nameof(levelIndex));
            _modeIndex = RequireIndex(modeIndex, _modes.Length, nameof(modeIndex));
            _equipmentSlotIndex = RequireIndex(equipmentSlotIndex, _equipmentSlots.Length, nameof(equipmentSlotIndex));
        }

        public void ReleaseMovement()
        {
            _movementHold = 0;
            if (!HasActiveAction && (MotionState == "walk" || MotionState == "run")) MotionState = "idle";
        }
        public void SetRun(bool enabled)
        {
            RunEnabled = enabled;
        }

        public void HoldMovement(float seconds)
        {
            if (HasActiveAction) return;
            _movementHold = Math.Max(0, seconds);
            MotionState = _movementHold > 0 ? (RunEnabled ? "run" : "walk") : "idle";
        }

        public bool TryStartAction(string actionId, float duration)
        {
            if (string.IsNullOrEmpty(actionId)) throw new ArgumentException("Action id is required", nameof(actionId));
            if (duration <= 0) throw new ArgumentOutOfRangeException(nameof(duration));
            if (HasActiveAction) return false;
            MotionState = actionId;
            _actionDuration = duration;
            ActionRemaining = duration;
            _movementHold = 0;
            return true;
        }

        public void Advance(float seconds)
        {
            var elapsed = Math.Max(0, seconds);
            AnimationPhase += Math.Min(elapsed, .1f);
            if (ActionRemaining > 0)
            {
                ActionRemaining = Math.Max(0, ActionRemaining - elapsed);
                if (ActionRemaining <= 0) MotionState = "idle";
                return;
            }
            if (_movementHold <= 0) return;
            _movementHold = Math.Max(0, _movementHold - elapsed);
            MotionState = _movementHold > 0 ? (RunEnabled ? "run" : "walk") : "idle";
        }

        private static string[] RequireValues(string[] values, string name)
        {
            if (values == null) throw new ArgumentNullException(name);
            if (values.Length == 0) throw new ArgumentException("At least one value is required", name);
            var copy = (string[])values.Clone();
            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in copy)
                if (string.IsNullOrEmpty(value) || !unique.Add(value))
                    throw new ArgumentException("Values must be non-empty and unique", name);
            return copy;
        }

        private static int RequireIndex(int index, int length, string name)
        {
            if (index < 0 || index >= length) throw new ArgumentOutOfRangeException(name);
            return index;
        }
    }
}
