using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed partial class CongDongLamMap01AArtPreview
    {
        private bool _poseLoopCapturing;
        [Serializable] private sealed class PoseLoopFrame
        {
            public int frame;
            public float seconds, animationPhase;
            public string motion, pose, registeredPose;
            public float registeredRunCycle;
            public Vector3[] registeredJoints;
        }
        [Serializable] private sealed class PoseLoopEvidence
        {
            public string status = "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED";
            public string limitation = "Fixed-step run comparison; live input and held-jump are covered by the registered capture separately";
            public bool registeredComparison;
            public int fps = 30;
            public List<PoseLoopFrame> frames = new List<PoseLoopFrame>();
        }

        private Vector3[] CaptureRegisteredJointPoints()
        {
            if (_registeredOutfit == null) return Array.Empty<Vector3>();
            var points = new List<Vector3>();
            foreach (var part in new[] { "head", "left-upper-arm", "left-forearm-hand", "right-upper-arm", "right-forearm-hand", "left-thigh", "left-shin-foot", "left-foot", "right-thigh", "right-shin-foot", "right-foot" })
                points.Add(Camera.main.WorldToScreenPoint(_registeredOutfit.JointWorldPosition("male", part)));
            foreach (var side in new[] { "left", "right" })
                points.Add(Camera.main.WorldToScreenPoint(_registeredOutfit.HandWorldPosition("male", side)));
            return points.ToArray();
        }

        private IEnumerator CaptureSourcePoseLoop()
        {
            if (_sourcePoseReview == null) throw new InvalidOperationException("Pose loop capture requires review pack");
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing pose loop output directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _poseLoopCapturing = true;
            Application.runInBackground = true;
            var previousRate = Time.captureFramerate;
            Time.captureFramerate = 30;
            var evidence = new PoseLoopEvidence { registeredComparison = _registeredOutfit != null };
            var image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            try
            {
                _routeX = 18.7f;
                AdvanceVoAnimation(2);
                _voState.SetPresentation(0, 0, Array.IndexOf(args, "--lgo-vo-pose-loop-base") >= 0 ? 1 : 2, 0);
                SetVoRun(true);
                yield return null;
                var phaseOrigin = _voState.AnimationPhase;
                for (var frame = 0; frame < 180; frame++)
                {
                    // Three complete four-pose loops, then recovery and a somersault.
                    if (frame >= 15 && frame < 79) MoveOnLane(1, 1f / 30);
                    else
                    {
                        if (frame == 90) TriggerVoJump();
                        AdvanceVoAnimation(1f / 30);
                    }
                    Refresh();
                    yield return new WaitForEndOfFrame();
                    if (Mathf.Abs(_voState.AnimationPhase - phaseOrigin - (frame + 1) / 30f) > .001f)
                        evidence.status = "FIX_REQUIRED_ANIMATION_CLOCK";
                    if (frame >= 15 && frame < 79 && VoAvatarMotionState != "run")
                        evidence.status = "FIX_REQUIRED_RUN_STATE";
                    if (_registeredOutfit != null && (VoAvatarMotionState == "run" || VoAvatarMotionState == "idle") && _sourcePoseReview.HasTransitions
                        && _registeredOutfit.CurrentRunPose != _sourcePoseReview.CurrentFrame)
                        evidence.status = "FIX_REQUIRED_REGISTERED_SOURCE_PHASE";
                    var active = RenderTexture.active;
                    try
                    {
                        RenderTexture.active = null;
                        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                        image.Apply();
                        DongMonIllustratedPreview.WriteBmp(Path.Combine(directory, frame.ToString("D4") + ".bmp"), image.GetPixels32(), Screen.width, Screen.height);
                    }
                    finally { RenderTexture.active = active; }
                    evidence.frames.Add(new PoseLoopFrame { frame = frame, seconds = frame / 30f,
                        animationPhase = _voState.AnimationPhase, motion = VoAvatarMotionState, pose = _sourcePoseReview.CurrentFrame, registeredPose = _registeredOutfit?.CurrentRunPose,
                        registeredRunCycle = _registeredOutfit == null ? 0 : _registeredOutfit.RunCycle,
                        registeredJoints = CaptureRegisteredJointPoints() });
                    yield return null;
                }
                var poses = new HashSet<string>();
                foreach (var frame in evidence.frames) poses.Add(frame.pose);
                foreach (var required in new[] { "idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck" })
                    if (!poses.Contains(required)) evidence.status = "FIX_REQUIRED_MISSING_POSE_" + required;
                if (_sourcePoseReview.HasTransitions && (!poses.Contains("run_start") || !poses.Contains("run_stop")))
                    evidence.status = "FIX_REQUIRED_ENTRY_EXIT";
                File.WriteAllText(Path.Combine(directory, "pose-loop.json"), JsonUtility.ToJson(evidence, true));
            }
            finally
            {
                Time.captureFramerate = previousRate;
                _poseLoopCapturing = false;
                Destroy(image);
            }
            Application.Quit(evidence.status == "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" ? 0 : 1);
        }
    }
}
