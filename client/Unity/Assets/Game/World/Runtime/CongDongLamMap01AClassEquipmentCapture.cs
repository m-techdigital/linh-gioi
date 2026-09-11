using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed partial class CongDongLamMap01AArtPreview
    {
        [Serializable]
        private sealed class ClassEquipmentCaptureInfo
        {
            public string status = "PASS";
            public string classId;
            public string packId;
            public string fitStatus = "DRAFT_RUNTIME_FIT";
            public int frames;
            public int fullLoadouts;
            public int slotToggleCases;
            public int mixedLoadouts;
            public int motionCases;
            public int minVisibleSlots = 10;
            public int maxVisibleComponents;
            public string[] levels = { "1", "10", "20", "30" };
            public string[] genders = { "male", "female" };
            public string finalSnapshot;
            public List<string> errors = new List<string>();
        }

        private IEnumerator CaptureClassEquipmentReview()
        {
            Application.runInBackground = true;
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length)
                throw new ArgumentException("Missing class equipment capture directory");
            var directory = args[index + 1];
            var classId = Array.IndexOf(args, "--lgo-co-capture") >= 0 ? "co"
                : Array.IndexOf(args, "--lgo-phap-capture") >= 0 ? "phap" : "kiem";
            EnsureClassFitPreview(classId);
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            _routeX = 20.5f;
            InventoryOpen = false;
            SetClassFitPreview("male", "idle");
            yield return null;
            yield return null;

            var result = new ClassEquipmentCaptureInfo
            {
                classId = classId,
                packId = classId + "-lv1-30-equipment-runtime-v1"
            };
            yield return CaptureClassEquipmentFrame(directory, "01-male-lv1-inventory", result, true);
            InventoryOpen = false;
            var levels = new[] { 1, 10, 20, 30 };
            foreach (var gender in new[] { "male", "female" })
            {
                SetClassFitPreview(gender, "idle");
                foreach (var level in levels)
                {
                    _voState.EquipAllExcept(null);
                    foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = level;
                    RefreshVoAvatarMode();
                    yield return CaptureClassEquipmentFrame(directory, gender + "-lv" + level + "-full", result, false);
                    result.fullLoadouts++;
                }
            }

            SetClassFitPreview("male", "idle");
            foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = 1;
            for (var slotIndex = 0; slotIndex < VoEquipmentSlots.Length; slotIndex++)
            {
                var slot = VoEquipmentSlots[slotIndex];
                _voState.SetPresentation(0, 0, 2, slotIndex);
                _voState.EquipAllExcept(slot);
                RefreshVoAvatarMode();
                yield return CaptureClassEquipmentFrame(directory, "male-lv1-off-" + slot, result, false);
                result.slotToggleCases++;
            }

            foreach (var gender in new[] { "male", "female" })
            {
                SetClassFitPreview(gender, "idle");
                _voState.EquipAllExcept(null);
                for (var slotIndex = 0; slotIndex < VoEquipmentSlots.Length; slotIndex++)
                    _voEquipmentLevels[VoEquipmentSlots[slotIndex]] = levels[slotIndex % levels.Length];
                RefreshVoAvatarMode();
                yield return CaptureClassEquipmentFrame(directory, gender + "-mixed-levels", result, false);
                result.mixedLoadouts++;
            }

            SetClassFitPreview("male", "idle");
            _voState.EquipAllExcept(null);
            foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = 10;
            RefreshVoAvatarMode();
            SetVoRun(true);
            for (var phase = 0; phase < 4; phase++)
            {
                MoveOnLane(1, .09f);
                yield return CaptureClassEquipmentFrame(directory, "male-lv10-run-phase-" + phase, result, false);
                result.motionCases++;
            }
            SetVoRun(false);
            AdvanceVoAnimation(.5f);
            TriggerVoJump();
            yield return CaptureClassEquipmentFrame(directory, "male-lv10-jump-rise", result, false);
            result.motionCases++;
            AdvanceVoAnimation(.22f);
            yield return CaptureClassEquipmentFrame(directory, "male-lv10-jump-apex", result, false);
            result.motionCases++;

            result.finalSnapshot = _classFitPreview.Snapshot;
            if (result.fullLoadouts != 8 || result.slotToggleCases != 10
                || result.mixedLoadouts != 2 || result.motionCases != 6 || result.frames != 27)
                result.errors.Add("incomplete capture matrix");
            if (result.minVisibleSlots != 9 || result.maxVisibleComponents != 13)
                result.errors.Add("unexpected visible slot/component counts");
            if (result.errors.Count > 0) result.status = "FIX_REQUIRED";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), JsonUtility.ToJson(result, true));
            Application.Quit(result.status == "PASS" ? 0 : 1);
        }

        private IEnumerator CaptureClassEquipmentFrame(string directory, string name, ClassEquipmentCaptureInfo result, bool inventory)
        {
            InventoryOpen = inventory;
            _controller.RefreshForSmoke();
            Refresh();
            yield return null;
            yield return new WaitForEndOfFrame();
            var visibleSlots = _classFitPreview.VisibleSlotCount;
            var visibleComponents = _classFitPreview.VisibleComponentCount;
            result.minVisibleSlots = Mathf.Min(result.minVisibleSlots, visibleSlots);
            result.maxVisibleComponents = Mathf.Max(result.maxVisibleComponents, visibleComponents);
            if ((inventory || name.Contains("-full") || name.Contains("mixed") || name.Contains("run") || name.Contains("jump"))
                && (visibleSlots != 10 || visibleComponents != 13))
                result.errors.Add(name + ": expected 10 slots/13 components");
            if (name.Contains("-off-") && visibleSlots != 9)
                result.errors.Add(name + ": expected 9 visible slots");

            var active = RenderTexture.active;
            var image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            try
            {
                RenderTexture.active = null;
                image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                image.Apply();
                DongMonIllustratedPreview.WriteBmp(Path.Combine(directory, name + ".bmp"),
                    image.GetPixels32(), Screen.width, Screen.height);
            }
            finally
            {
                RenderTexture.active = active;
                Destroy(image);
            }
            result.frames++;
        }
    }
}
