using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed partial class CongDongLamMap01AArtPreview
    {
        private bool _registeredCapturing;
        [Serializable] private sealed class RegisteredEvidence
        {
            public string status = "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED";
            public string limitation = "Draft joint weights; macOS aspect simulation; visual review required";
            public int frames, width, height, actionTransitions, toggles, heldJumpRestarts, basePoseFrames;
            public float maxBindReturnError;
            public float minActorScreenHeightRatio = 999f;
            public float maxActorScreenHeightRatio;
            public int actorScreenMetricFrames;
            public bool closedFarArms;
            public bool closedBody, registeredEquipment;
            public int maxEquipmentAttachments, maxBodyVariants;
            public List<string> errors = new List<string>();
        }
        public static float RegisteredActorScreenHeightRatio(Camera camera, Bounds worldBounds, int screenHeight)
        {
            if (camera == null || screenHeight <= 0 || worldBounds.size.y <= 0f) return 0f;
            var bottom = camera.WorldToScreenPoint(new Vector3(worldBounds.center.x, worldBounds.min.y, worldBounds.center.z));
            var top = camera.WorldToScreenPoint(new Vector3(worldBounds.center.x, worldBounds.max.y, worldBounds.center.z));
            var denominator = camera.pixelHeight > 0 ? camera.pixelHeight : screenHeight;
            return Mathf.Abs(top.y - bottom.y) / denominator;
        }
        private IEnumerator CaptureRegistered()
        {
            _registeredCapturing = true;
            Application.runInBackground = true;
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (_registeredOutfit == null || index < 0 || index + 1 >= args.Length)
                throw new InvalidOperationException("Registered capture requires outfit and output path");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            var report = new RegisteredEvidence { registeredEquipment = _registeredOutfit.RegisteredEquipmentEnabled, width = Screen.width, height = Screen.height, closedBody = _registeredOutfit.ClosedBodyEnabled, closedFarArms = _registeredOutfit.ClosedFarArmsEnabled };
            yield return null;
            foreach (var gender in new[] { 0, 1 })
            {
                AdvanceVoAnimation(2); AdvanceVoAnimation(.1f);
                _voState.SetPresentation(gender, 0, 2, 0);
                _voState.EquipAllExcept(null);
                _routeX = 39;
                RefreshVoAvatarMode(); Refresh();
                var rest = _registeredOutfit.SnapshotVertices(_registeredOutfit.BindSpace);
                yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-idle");
                foreach (var action in new[] { "walk", "run", "jump", "jump_diagonal", "basic_attack", "skill" })
                {
                    VoTrainingTargetHp = 100;
                    var motionOrigin = action == "basic_attack" || action == "skill" ? 39f : 18.7f;
                    _routeX = motionOrigin;
                    var jump = action.StartsWith("jump", StringComparison.Ordinal);
                    var triggered = true;
                    if (action == "walk" || action == "run")
                    {
                        SetVoRun(action == "run");
                        for (var tick = 0; tick < 12; tick++) MoveOnLane(tick < 6 ? 1 : -1, .05f);
                        AdvanceVoAnimation(.05f);
                        // Four evenly spaced samples cover a whole stride, avoiding
                        // accidentally capturing only the near-idle zero crossing.
                        for (var sample = 0; sample < 4; sample++)
                        {
                            for (var step = 0; step < 2; step++)
                                MoveOnLane(sample < 2 ? 1 : -1, .125f / (action == "run" ? TwoDSourcePoseTimeline.RunCyclesPerSecond : 2f));
                            yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-" + action + "-phase-" + sample);
                        }
                    }
                    else
                    {
                        triggered = jump ? TriggerVoJump() : action == "skill" ? TriggerVoSkill() : TriggerVoBasicAttack();
                        if (action == "jump_diagonal") for (var tick = 0; tick < 3; tick++) MoveOnLane(1, .06f);
                        else AdvanceVoAnimation(jump ? .18f : action == "skill" ? .21f : .15f);
                    }
                    if (!triggered || VoAvatarMotionState != (jump ? "jump" : action)) report.errors.Add(VoAvatarGender + " failed " + action);
                    if ((_voAvatarRoot.localScale - Vector3.one).sqrMagnitude > .00000001f
                        || Quaternion.Angle(_voAvatarRoot.localRotation, Quaternion.identity) > .001f)
                        report.errors.Add("Root scale/rotation changed during " + action);
                    if (_registeredOutfit.VisibleLayers != 11) report.errors.Add("Expected base plus ten slots during " + action);
                    Refresh();
                    yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-" + action);
                    if (jump)
                    {
                        for (var phase = 2; phase <= 3; phase++)
                        {
                            if (action == "jump_diagonal") for (var tick = 0; tick < 3; tick++) MoveOnLane(1, .06f);
                            else AdvanceVoAnimation(.18f);
                            if (Mathf.Abs(_registeredOutfit.RollDegrees - TwoDLocomotionCurves.SomersaultDegrees(phase * .25f)) > .01f) report.errors.Add("Somersault rotation phase mismatch");
                            if (action == "jump_diagonal" && PlayerX <= motionOrigin) report.errors.Add("Missing diagonal travel");
                            Refresh();
                            yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-" + action + "-phase-" + phase);
                        }
                    }
                    if (action == "run")
                    {
                        AdvanceVoAnimation(.1f); AdvanceVoAnimation(.05f); Refresh();
                        yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-stop-start");
                        AdvanceVoAnimation(.06f); Refresh();
                        yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-stop-middle");
                        AdvanceVoAnimation(.06f); Refresh();
                        yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-stop-settled");
                    }
                    AdvanceVoAnimation(2); AdvanceVoAnimation(.1f); Refresh();
                    var after = _registeredOutfit.SnapshotVertices(_registeredOutfit.BindSpace);
                    if (rest.Length != after.Length) report.errors.Add("Mesh topology changed after " + action);
                    else
                    {
                        var error = 0f;
                        for (var v = 0; v < rest.Length; v++) error = Mathf.Max(error, Vector3.Distance(rest[v], after[v]));
                        report.maxBindReturnError = Mathf.Max(report.maxBindReturnError, error);
                        if (error > .00001f) report.errors.Add(VoAvatarGender + " return drift after " + action + ": " + error);
                    }
                    if (VoAvatarMotionState != "idle") report.errors.Add("Did not return idle after " + action);
                    report.actionTransitions++;
                    yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-after-" + action);
                }
                _routeX = 39;
                if (!TriggerVoJump()) report.errors.Add("Could not start air-turn check");
                for (var tick = 0; tick < 3; tick++) MoveOnLane(1, .06f);
                if (_voState.FacingSign != 1) report.errors.Add("Missing right facing in jump");
                yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-air-facing-right");
                for (var tick = 0; tick < 3; tick++) MoveOnLane(-1, .06f);
                if (_voState.FacingSign != -1 || VoAvatarMotionState != "jump") report.errors.Add("Air reversal cancelled jump or did not face left");
                yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-air-facing-left");
                var airPose = _registeredOutfit.SnapshotVertices(_registeredOutfit.BindSpace);
                var airProgress = _voState.ActionProgress;
                for (var slot = 0; slot < 10; slot++)
                {
                    ToggleVoEquipmentSlot();
                    if (_registeredOutfit.VisibleLayers != 10) report.errors.Add("Air slot failed to hide " + VoSelectedEquipmentSlot);
                    yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-air-off-" + VoSelectedEquipmentSlot);
                    ToggleVoEquipmentSlot();
                    if (_registeredOutfit.VisibleLayers != 11 || _voState.ActionProgress != airProgress) report.errors.Add("Equipment refresh changed action time or layer count");
                    var restored = _registeredOutfit.SnapshotVertices(_registeredOutfit.BindSpace);
                    for (var vertex = 0; vertex < airPose.Length; vertex++)
                        if (Vector3.Distance(airPose[vertex], restored[vertex]) > .00001f) { report.errors.Add("Air equip changed pose " + VoSelectedEquipmentSlot); break; }
                    report.toggles++;
                    CycleVoEquipmentSlot();
                }
                AdvanceVoAnimation(2); AdvanceVoAnimation(.1f); Refresh();
                report.actionTransitions++;
                yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-air-facing-return");
                _routeX = 29;
                var starts = VoJumpStartCount;
                SetVoJumpHeld(true);
                for (var tick = 1; tick <= 32; tick++)
                {
                    MoveOnLane(1, .05f);
                    if (VoAvatarMotionState != "jump") report.errors.Add("Held jump fell back to run before release");
                    if (tick % 8 == 0) yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-held-jump-" + tick);
                }
                SetVoJumpHeld(false);
                AdvanceVoAnimation(2); AdvanceVoAnimation(.1f); Refresh();
                var started = VoJumpStartCount - starts;
                if (started < 3 || VoAvatarMotionState != "idle") report.errors.Add("Held jump did not repeat or did not stop on release");
                report.heldJumpRestarts += started - 1;
                report.actionTransitions += started;
                yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-held-jump-released");
                for (var slot = 0; slot < 10; slot++)
                {
                    ToggleVoEquipmentSlot();
                    if (_registeredOutfit.VisibleLayers != 10) report.errors.Add("Slot failed to hide " + VoSelectedEquipmentSlot);
                    yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-off-" + VoSelectedEquipmentSlot);
                    ToggleVoEquipmentSlot();
                    if (_registeredOutfit.VisibleLayers != 11) report.errors.Add("Slot failed to restore " + VoSelectedEquipmentSlot);
                    report.toggles++;
                    CycleVoEquipmentSlot();
                }
                // Review naked base separately: equipped layers can conceal
                // deformations in the underlying body. Same rig, same map scale.
                _voState.SetPresentation(gender, 0, 1, 0);
                RefreshVoAvatarMode();
                foreach (var motion in new[] { "walk", "run", "jump" })
                {
                    _routeX = 18.7f;
                    AdvanceVoAnimation(2); AdvanceVoAnimation(.15f);
                    SetVoRun(motion == "run");
                    if (motion == "jump") TriggerVoJump();
                    for (var sample = 0; sample < 4; sample++)
                    {
                        if (motion == "jump") AdvanceVoAnimation(.15f);
                        else for (var step = 0; step < 2; step++) MoveOnLane(1, .125f / (motion == "run" ? TwoDSourcePoseTimeline.RunCyclesPerSecond : 2f));
                        if (_registeredOutfit.VisibleLayers != 1) report.errors.Add("Base diagnostic contains equipment");
                        Refresh();
                        yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-base-" + motion + "-" + sample);
                        report.basePoseFrames++;
                    }
                    AdvanceVoAnimation(2); AdvanceVoAnimation(.15f); Refresh();
                    yield return SaveRegisteredFrame(directory, report, VoAvatarGender + "-base-after-" + motion);
                    report.basePoseFrames++;
                }
            }
            _voState.SetPresentation(0, 0, 2, 0);
            _voState.EquipAllExcept(null);
            AdvanceVoAnimation(2); AdvanceVoAnimation(.1f);
            RefreshVoAvatarMode();
            for (var tile = 0; tile < 12; tile++)
            {
                _routeX = -3.3f + tile * 4.4f;
                Refresh();
                yield return SaveRegisteredFrame(directory, report, "route-surface-" + tile);
            }
            if (report.maxBindReturnError > .00001f) report.errors.Add("Deformed vertices did not return to original bind coordinates");
            if (report.errors.Count > 0) report.status = "FIX_REQUIRED";
            File.WriteAllText(Path.Combine(directory, "registered-manifest.json"), JsonUtility.ToJson(report, true));
            Application.Quit(report.errors.Count == 0 ? 0 : 1);
        }
        private IEnumerator SaveRegisteredFrame(string directory, RegisteredEvidence report, string name)
        {
            // Let HUD and the native skinning batch consume the sampled state.
            yield return null;
            yield return new WaitForEndOfFrame();
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0); texture.Apply();
            var actorRatio = RegisteredActorScreenHeightRatio(Camera.main, _registeredOutfit.VisibleWorldBounds(), Screen.height);
            if (actorRatio > 0f)
            {
                report.minActorScreenHeightRatio = Mathf.Min(report.minActorScreenHeightRatio, actorRatio);
                report.maxActorScreenHeightRatio = Mathf.Max(report.maxActorScreenHeightRatio, actorRatio);
                report.actorScreenMetricFrames++;
            }
            report.maxEquipmentAttachments = Math.Max(report.maxEquipmentAttachments, _registeredOutfit.VisibleEquipmentAttachments);
            report.maxBodyVariants = Math.Max(report.maxBodyVariants, _registeredOutfit.VisibleBodyVariants);
            var expectedFallbackCloth = _registeredOutfit.UsesEquipmentLowerBody ? 0 : 1;
            var expectedJointGarments = VoAvatarMode == "base" || !report.registeredEquipment ? 0
                : (_voState.IsEquipped("lower_garment") ? 2 : 0) + (VoAvatarGender == "female" && _voState.IsEquipped("outer_tunic") ? 2 : 0);
            if (report.registeredEquipment && _registeredOutfit.VisibleJointGarments != expectedJointGarments) report.errors.Add("Missing registered garment mesh: " + name);
            if (_registeredOutfit.VisibleBodyVariants > 1) report.errors.Add("Overlapping body occlusion variants: " + name);
            if (report.closedBody && (_registeredOutfit.VisibleRigidAttachments != 10 || _registeredOutfit.VisibleClothAttachments != expectedFallbackCloth)) report.errors.Add("Missing active closed body attachments: " + name);
            if (report.closedFarArms && _registeredOutfit.VisibleRigidAttachments != 2) report.errors.Add("Missing active closed arm attachments: " + name);
            File.WriteAllBytes(Path.Combine(directory, (++report.frames).ToString("00") + "-" + name + ".png"), texture.EncodeToPNG());
            Destroy(texture);
        }
    }
}
