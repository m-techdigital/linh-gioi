namespace LinhGioi.World
{
    // Uses the character clock; repeated presentation refreshes cannot advance animation.
    public sealed class TwoDSourcePoseTimeline
    {
        public const float RunCyclesPerSecond = 1.5f;
        public float Cycle { get; private set; }
        public float EntryWeight { get; private set; }
        public float ExitWeight { get; private set; }
        public const float EntrySeconds = .12f;
        public const float ExitSeconds = .12f;
        private bool _moving;
        private float _stepSeconds;
        public void Advance(float seconds) => _stepSeconds = UnityEngine.Mathf.Max(0, seconds);
        private float _loopStarts, _stopStarts = float.NegativeInfinity;

        public string Select(string motion, float seconds)
        {
            var moving = motion == "run" || motion == "walk";
            if (moving)
            {
                if (!_moving) _loopStarts = seconds + EntrySeconds;
                _moving = true;
                _stopStarts = float.NegativeInfinity;
                EntryWeight = UnityEngine.Mathf.Clamp01(1 + (seconds - _loopStarts) / EntrySeconds);
                Cycle = UnityEngine.Mathf.Max(0, seconds - _loopStarts) * RunCyclesPerSecond;
                if (seconds < _loopStarts) return "run_start";
                return TwoDSourcePoseReview.SelectRunFrame(Cycle, true);
            }
            if (motion == "idle")
            {
                if (_stepSeconds >= ExitSeconds) _stopStarts = float.NegativeInfinity;
                else if (_moving) _stopStarts = seconds;
            }
            _moving = false;
            if (motion != "idle") _stopStarts = float.NegativeInfinity;
            ExitWeight = UnityEngine.Mathf.Clamp01((seconds - _stopStarts) / ExitSeconds);
            return seconds - _stopStarts < ExitSeconds ? "run_stop" : "idle";
        }
    }
}
