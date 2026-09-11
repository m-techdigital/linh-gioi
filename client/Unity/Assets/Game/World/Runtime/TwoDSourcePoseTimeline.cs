namespace LinhGioi.World
{
    // Uses the character clock; repeated presentation refreshes cannot advance animation.
    public sealed class TwoDSourcePoseTimeline
    {
        public const float EntrySeconds = .12f;
        public const float ExitSeconds = .12f;
        private bool _moving;
        private float _loopStarts, _stopStarts = float.NegativeInfinity;

        public string Select(string motion, float seconds)
        {
            var moving = motion == "run" || motion == "walk";
            if (moving)
            {
                if (!_moving) _loopStarts = seconds + EntrySeconds;
                _moving = true;
                _stopStarts = float.NegativeInfinity;
                if (seconds < _loopStarts) return "run_start";
                return TwoDSourcePoseReview.SelectRunFrame((seconds - _loopStarts) * 1.5f, true);
            }
            if (_moving && motion == "idle") _stopStarts = seconds;
            _moving = false;
            if (motion != "idle") _stopStarts = float.NegativeInfinity;
            return seconds - _stopStarts < ExitSeconds ? "run_stop" : "idle";
        }
    }
}
