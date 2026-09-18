using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LinhGioi.Foundation;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.World
{
    // Evidence-only capture orchestration. Product world state/rendering stays in the core partial.
    public sealed partial class CongDongLamMap01AArtPreview
    {
        private bool CharacterSelectCaptureRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-character-select-capture") >= 0;
        private bool CharacterEntryCaptureRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-character-entry-capture") >= 0;
        private bool ServerSelectCaptureRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-server-select-capture") >= 0;
        private bool RegisterCaptureRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-register-capture") >= 0;
        private bool PasswordRecoveryCaptureRequested
            => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-password-recovery-capture") >= 0
            || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-password-recovery-verify-capture") >= 0
            || Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-password-recovery-new-password-capture") >= 0;
        private bool InventoryTabsCaptureRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-inventory-tabs-capture") >= 0;
        private bool CharacterScreenCaptureRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-character-screen-capture") >= 0;
        private bool MenuCaptureRequested => Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-menu-capture") >= 0;
        public bool IsCapturing => _registeredCapturing || _poseLoopCapturing
            || CharacterSelectCaptureRequested || CharacterEntryCaptureRequested || ServerSelectCaptureRequested || RegisterCaptureRequested || PasswordRecoveryCaptureRequested || InventoryTabsCaptureRequested || CharacterScreenCaptureRequested
            || MenuCaptureRequested || IsMapQuestCaptureForArgs(Environment.GetCommandLineArgs());

        [Serializable] private sealed class CaptureInfo
        {
            public string status = "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED";
            public string pack = "cong-dong-lam-map01a-art-draft-v1";
            public int frames, width, height, dialogueFrames;
            public string[] revisitedNpcs;
            public bool dialogueRevisitsVerified;
            public bool questWorldFramesUnobstructed = true;
            public string deviceValidation = "macOS aspect simulation only";
            public string captureScope = "map-and-legacy-wardrobe";
            public float groundY, maxFootError, parallaxDelta;
            public bool mapQuestFlowVerified = false;
            public bool functionalUiVerified = false;
            public string activeQuestId;
            public int completedQuestCount;
            public bool starterSupplies, spiritHerb, hiddenChest, combatAccepted, enemyDefeated, enemyLooted, portalUnlocked;
            public bool minimapUnlocked, healthPotionUsed, classRewardEquipped;
            public bool inventoryItemDetailVerified;
            public int healthPotionCount, manaPotionCount, playerHealth;
            public bool dialogueOpened, greetingCompleted;
            public RuntimeUiMetricsSnapshot uiMetrics;
            public RuntimeWorldPresentationMetrics worldMetrics;
            public bool voBaseVerified, voModularVerified, voWalkVerified, voSkillVerified;
            public bool voFemaleVerified, voSlotToggleVerified;
            public bool voFemaleMotionVerified, voProgressionVerified;
            public bool voRunVerified, voJumpVerified, voBasicVerified, voLienQuyenVerified;
            public bool voAttachmentLv1Verified, voAttachmentLv30FemaleVerified;
            public bool voEquipmentComponentBindingVerified;
            public bool voTenSlotMatrixVerified;
            public bool voSharedRuntimeStateVerified;
            public bool voMixedLevelMaleVerified, voMixedLevelFemaleVerified;
            public bool voMixedLevelMotionVerified, voMixedLevelToggleVerified;
            public string voMixedEquipmentSnapshot;
            public int voSkillCastCount, voSkillHitCount, voTrainingTargetHp;
        }
        private static bool HasCaptureRequestForArgs(string[] args)
        {
            if (args == null) return false;
            return IsMapQuestCaptureForArgs(args)
                || Array.IndexOf(args, "--lgo-map01a-entry-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-character-select-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-character-entry-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-server-select-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-register-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-password-recovery-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-password-recovery-verify-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-password-recovery-new-password-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-inventory-tabs-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-character-screen-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-menu-capture") >= 0;
        }

        public static bool IsMapQuestCaptureForArgs(string[] args)
            => args != null && Array.IndexOf(args, "--lgo-map01a-art-capture") >= 0;

        public static bool ShouldCaptureAuthValidationForArgs(string[] args)
            => args != null && Array.IndexOf(args, "--lgo-map01a-auth-validation-capture") >= 0;

        public static bool ShouldCaptureProductAuthStatesForArgs(string[] args)
            => args != null && Array.IndexOf(args, "--lgo-product-auth-states-capture") >= 0;

        public static bool ShouldCaptureProductAccountStatesForArgs(string[] args)
            => args != null && Array.IndexOf(args, "--lgo-product-account-states-capture") >= 0;

        public static bool QuestCaptureRequiresWorldView(int captureIndex) => captureIndex >= 10 && captureIndex <= 17;

        private IEnumerator Start()
        {
            var args = Environment.GetCommandLineArgs();
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-vo-pose-loop-capture") >= 0)
            {
                yield return CaptureSourcePoseLoop();
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-registered-capture") >= 0)
            {
                yield return CaptureRegistered();
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-entry-capture") >= 0)
            {
                yield return CaptureEntryScreen(args);
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-character-entry-capture") >= 0)
            {
                yield return CaptureCharacterEntryScreen(args);
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-character-select-capture") >= 0)
            {
                yield return CaptureCharacterSelectScreen(args);
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-server-select-capture") >= 0)
            {
                yield return CaptureServerSelectScreen(args);
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-register-capture") >= 0)
            {
                yield return CaptureRegisterScreen(args);
                yield break;
            }
            if (Application.isPlaying && (
                Array.IndexOf(args, "--lgo-map01a-password-recovery-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-password-recovery-verify-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-password-recovery-new-password-capture") >= 0))
            {
                yield return CapturePasswordRecoveryScreen(args);
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-inventory-tabs-capture") >= 0)
            {
                yield return CaptureInventoryTabs(args);
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-character-screen-capture") >= 0)
            {
                yield return CaptureCharacterScreen(args);
                yield break;
            }
            if (Application.isPlaying && Array.IndexOf(args, "--lgo-map01a-menu-capture") >= 0)
            {
                yield return CaptureMenuScreen(args);
                yield break;
            }
            if (!Application.isPlaying || !IsMapQuestCaptureForArgs(args)) yield break;
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing Map01A capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            var questOnly = Array.IndexOf(args, "--lgo-map01a-quest-only") >= 0;
            var result = new CaptureInfo { groundY = GroundY, width = Screen.width, height = Screen.height,
                captureScope = questOnly ? "map-quests-q01-q09" : "map-and-legacy-wardrobe" };
            var initial = FarOffset;
            var targets = new[] { -3.58f, -3.58f, -3.58f, 0f, 5.15f, 5.15f, 18.5f, 22.15f, 22.15f, 22.15f,
                26.15f, 30.2f, 30.2f, 34.1f, 39f, 39f, 39f, 42.15f,
                20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f,
                39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f,
                39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f,
                20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f, 20.5f,
                39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f,
                39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f, 39f };
            var names = new[] { "01-arrival-q01", "02-ha-van-dialogue", "03-q01-complete", "04-q02-grand-gate",
                "05-quan-thu-dialogue", "06-q03-complete", "07-q04-inventory-open", "08-q04-tong-phu-dialogue",
                "09-q04-starter-supplies", "10-q04-health-potion-used", "11-q05-thanh-nhi", "12-q05-spirit-herb",
                "13-q08-hidden-chest", "14-q06-lao-tran", "15-q06-combat", "16-q07-class-loot",
                "17-q07-class-item-equipped", "18-q09-portal-open",
                "19-vo-base", "20-vo-modular", "21-vo-walk", "22-vo-female-full", "23-vo-female-slot-toggle",
                "24-vo-female-walk", "25-vo-lv10-female", "26-vo-lv20-female", "27-vo-lv30-female",
                "28-vo-female-run", "29-vo-female-jump-rise", "30-vo-female-jump-apex",
                "31-vo-female-basic-windup", "32-vo-female-basic-impact",
                "33-vo-female-lien-quyen-hit", "34-vo-female-lien-quyen-finish",
                "35-vo-male-run", "36-vo-male-jump", "37-vo-male-basic", "38-vo-male-lien-quyen",
                "39-vo-male-modular-run", "40-vo-male-modular-jump", "41-vo-male-modular-basic",
                "42-vo-male-modular-lien-quyen", "43-vo-female-lv30-modular-run",
                "44-vo-female-lv30-modular-jump", "45-vo-female-lv30-modular-basic",
                "46-vo-female-lv30-modular-lien-quyen",
                "47-vo-male-lv1-off-main-weapon", "48-vo-male-lv1-off-head-hair",
                "49-vo-male-lv1-off-inner-top", "50-vo-male-lv1-off-outer-tunic",
                "51-vo-male-lv1-off-lower-garment", "52-vo-male-lv1-off-waist",
                "53-vo-male-lv1-off-arm-guard", "54-vo-male-lv1-off-boots",
                "55-vo-male-lv1-off-light-armor", "56-vo-male-lv1-off-accessory",
                "57-vo-female-lv30-off-main-weapon", "58-vo-female-lv30-off-head-hair",
                "59-vo-female-lv30-off-inner-top", "60-vo-female-lv30-off-outer-tunic",
                "61-vo-female-lv30-off-lower-garment", "62-vo-female-lv30-off-waist",
                "63-vo-female-lv30-off-arm-guard", "64-vo-female-lv30-off-boots",
                "65-vo-female-lv30-off-light-armor", "66-vo-female-lv30-off-accessory",
                "67-vo-mixed-male-idle", "68-vo-mixed-male-run", "69-vo-mixed-male-jump",
                "70-vo-mixed-male-basic", "71-vo-mixed-male-lien-quyen", "72-vo-mixed-female-idle",
                "73-vo-mixed-female-run", "74-vo-mixed-female-jump", "75-vo-mixed-female-basic",
                "76-vo-mixed-female-lien-quyen", "77-vo-mixed-outer-off", "78-vo-mixed-outer-on" };
            for (var i = 0; i < (questOnly ? 18 : targets.Length); i++)
            {
                _routeX = targets[i];
                Refresh();
                if (i == 1) result.dialogueOpened = UseCurrentRouteAction() && DialogueOpen;
                if (i == 2) { yield return CaptureNpcDialoguePages(directory, "ha-van-offer", result); result.greetingCompleted = HasMetHaVan; }
                if (i == 3) UseCurrentRouteAction();
                if (i == 4) UseCurrentRouteAction();
                if (i == 5) yield return CaptureNpcDialoguePages(directory, "quan-thu-offer", result);
                if (i == 6) UseCurrentRouteAction();
                if (i == 7) UseCurrentRouteAction();
                if (i == 8)
                {
                    yield return CaptureNpcDialoguePages(directory, "tong-phu-offer", result);
                    var document = GetComponentInChildren<UIDocument>();
                    if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for item-detail capture");
                    InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Health Potion"));
                    var detailTitle = document.rootVisualElement.Q<Label>("Map01A Inventory Detail Item Name");
                    var detailIcon = document.rootVisualElement.Q<VisualElement>("Map01A Inventory Detail Icon");
                    result.inventoryItemDetailVerified = detailTitle != null && detailTitle.text == "Bình Máu Nhỏ"
                        && detailIcon != null && detailIcon.style.backgroundImage.value.sprite == GetMap01AItemThumbnailSprite("health_potion");
                }
                if (i == 9) UseHealthPotion();
                if (i == 10) { UseCurrentRouteAction(); yield return CaptureNpcDialoguePages(directory, "thanh-nhi-offer", result); }
                if (i == 11) UseCurrentRouteAction();
                if (i == 12) UseCurrentRouteAction();
                if (i == 13) { UseCurrentRouteAction(); yield return CaptureNpcDialoguePages(directory, "lao-tran-offer", result); }
                if (i == 14)
                {
                    result.voSkillVerified = true;
                    for (var hit = 0; hit < 3; hit++)
                    {
                        result.voSkillVerified &= TriggerVoSkill();
                        AdvanceVoAnimation(.16f);
                        if (hit < 2) AdvanceVoAnimation(.5f);
                    }
                }
                if (i == 15) { AdvanceVoAnimation(.5f); UseCurrentRouteAction(); }
                if (i == 16)
                {
                    InventoryOpen = true;
                    EquipClassReward();
                }
                if (i == 17)
                {
                    InventoryOpen = false;
                    UseCurrentRouteAction();
                    result.mapQuestFlowVerified = ActiveQuestId == "COMPLETE" && CompletedQuestCount == 9
                        && HasStarterSupplies && HasHarvestedSpiritHerb && HasOpenedHiddenChest
                        && HasAcceptedCombatQuest && HasDefeatedFirstEnemy && HasLootedFirstEnemy && PortalUnlocked;
                    result.functionalUiVerified = MinimapUnlocked && HasUsedHealthPotion && HealthPotionCount == 2
                        && ManaPotionCount == 2 && PlayerHealth == 100 && HasClassRewardItem && IsClassRewardEquipped;
                    result.voSkillCastCount = VoSkillCastCount;
                    result.voSkillHitCount = VoSkillHitCount;
                    result.voTrainingTargetHp = VoTrainingTargetHp;
                }
                if (i == 18)
                {
                    CycleVoAvatarMode();
                    result.voBaseVerified = VoAvatarMode == "base" && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 1;
                }
                if (i == 19)
                {
                    CycleVoAvatarMode();
                    result.voModularVerified = VoAvatarMode == "modular"
                        && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 0
                        && _voEquipmentComponents.Values.Count(renderer => renderer.enabled) == 14
                        && _voRigParts.Values.Count(renderer => renderer.enabled) == 10;
                    result.voEquipmentComponentBindingVerified = result.voModularVerified
                        && !_voAvatarParts["lv001_male_slot_arm_guard"].enabled
                        && !_voAvatarParts["lv001_male_slot_boots"].enabled;
                }
                if (i == 20)
                {
                    MoveOnLane(1, .1f);
                    result.voWalkVerified = VoAvatarMotionState == "walk" && VoAvatarUsesAlignedPaperDollMotion
                        && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 0
                        && _voEquipmentComponents.Values.Count(renderer => renderer.enabled) == 14
                        && _voRigParts.Values.Count(renderer => renderer.enabled) == 10 && !_voMotionRenderer.enabled;
                }
                if (i == 21)
                {
                    AdvanceVoAnimation(.1f);
                    AdvanceVoAnimation(.1f);
                    CycleVoAvatarMode();
                    CycleVoAvatarGender();
                    result.voFemaleVerified = VoAvatarMode == "full" && VoAvatarGender == "female"
                        && _voAvatarParts["lv001_female_full"].enabled;
                }
                if (i == 22)
                {
                    CycleVoAvatarMode();
                    CycleVoAvatarMode();
                    CycleVoEquipmentSlot();
                    CycleVoEquipmentSlot();
                    ToggleVoEquipmentSlot();
                    result.voSlotToggleVerified = VoAvatarMode == "modular" && VoSelectedEquipmentSlot == "inner_top"
                        && VoEquippedSlotCount == 9 && !_voAvatarParts["lv001_female_slot_inner_top"].enabled;
                }
                if (i == 23)
                {
                    MoveOnLane(1, .1f);
                    result.voFemaleMotionVerified = VoAvatarMotionState == "walk" && VoAvatarUsesAlignedPaperDollMotion
                        && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 0
                        && _voEquipmentComponents.Values.Count(renderer => renderer.enabled) == 13
                        && _voRigParts.Values.Count(renderer => renderer.enabled) == 10
                        && !_voAvatarParts["lv001_female_slot_inner_top"].enabled && !_voMotionRenderer.enabled;
                }
                if (i == 24)
                {
                    AdvanceVoAnimation(.5f);
                    ToggleVoEquipmentSlot();
                    CycleVoAvatarMode();
                    CycleVoAvatarLevel();
                }
                if (i == 25) CycleVoAvatarLevel();
                if (i == 26)
                {
                    CycleVoAvatarLevel();
                    result.voProgressionVerified = VoAvatarLevel == 30
                        && _voAvatarParts["lv030_female_full"].enabled;
                }
                if (i == 27)
                {
                    CycleVoAvatarLevel();
                    VoTrainingTargetHp = 100;
                    if (_voCombatTarget != null) _voCombatTarget.color = Color.white;
                    SetVoRun(true);
                    MoveOnLane(1, .1f);
                    result.voRunVerified = VoAvatarMotionFrameId.StartsWith("female_run_", StringComparison.Ordinal);
                }
                if (i == 28)
                {
                    SetVoRun(false); AdvanceVoAnimation(.5f); TriggerVoJump();
                    result.voJumpVerified = VoAvatarMotionFrameId == "female_jump_rise";
                }
                if (i == 29)
                {
                    AdvanceVoAnimation(.22f);
                    result.voJumpVerified &= VoAvatarMotionFrameId == "female_jump_apex";
                }
                if (i == 30) { AdvanceVoAnimation(.6f); TriggerVoBasicAttack(); }
                if (i == 31)
                {
                    AdvanceVoAnimation(.14f);
                    result.voBasicVerified = VoAvatarMotionFrameId == "female_basic_impact";
                }
                if (i == 32) { AdvanceVoAnimation(.5f); TriggerVoSkill(); }
                if (i == 33)
                {
                    AdvanceVoAnimation(.3f);
                    result.voLienQuyenVerified = VoAvatarMotionFrameId == "female_lien_quyen_finish_b";
                }
                if (i == 34)
                {
                    AdvanceVoAnimation(.5f); CycleVoAvatarGender(); SetVoRun(true); MoveOnLane(1, .1f);
                    result.voRunVerified &= VoAvatarMotionFrameId.StartsWith("male_run_", StringComparison.Ordinal);
                }
                if (i == 35)
                {
                    SetVoRun(false); AdvanceVoAnimation(.5f); TriggerVoJump(); AdvanceVoAnimation(.22f);
                    result.voJumpVerified &= VoAvatarMotionFrameId == "male_jump_apex";
                }
                if (i == 36)
                {
                    AdvanceVoAnimation(.6f); TriggerVoBasicAttack(); AdvanceVoAnimation(.14f);
                    result.voBasicVerified &= VoAvatarMotionFrameId == "male_basic_impact";
                }
                if (i == 37)
                {
                    AdvanceVoAnimation(.5f); TriggerVoSkill(); AdvanceVoAnimation(.3f);
                    result.voLienQuyenVerified &= VoAvatarMotionFrameId == "male_lien_quyen_finish_b";
                    VoTrainingTargetHp = 0;
                    if (_voCombatTarget != null) _voCombatTarget.color = new Color(.28f, .28f, .28f, .75f);
                }
                if (i == 38)
                {
                    AdvanceVoAnimation(.5f); CycleVoAvatarMode(); CycleVoAvatarMode();
                    VoTrainingTargetHp = 100;
                    if (_voCombatTarget != null) _voCombatTarget.color = Color.white;
                    SetVoRun(true); MoveOnLane(1, .1f);
                    result.voAttachmentLv1Verified = VoAvatarMode == "modular" && VoAvatarUsesAlignedPaperDollMotion;
                }
                if (i == 39)
                {
                    SetVoRun(false); AdvanceVoAnimation(.5f); TriggerVoJump();
                    result.voAttachmentLv1Verified &= VoAvatarMotionState == "jump";
                }
                if (i == 40)
                {
                    AdvanceVoAnimation(.6f); TriggerVoBasicAttack();
                    result.voAttachmentLv1Verified &= VoAvatarMotionState == "basic_attack";
                }
                if (i == 41)
                {
                    AdvanceVoAnimation(.5f); TriggerVoSkill();
                    result.voAttachmentLv1Verified &= VoAvatarMotionState == "skill";
                }
                if (i == 42)
                {
                    AdvanceVoAnimation(.5f); CycleVoAvatarGender();
                    CycleVoAvatarLevel(); CycleVoAvatarLevel(); CycleVoAvatarLevel();
                    VoTrainingTargetHp = 100;
                    if (_voCombatTarget != null) _voCombatTarget.color = Color.white;
                    SetVoRun(true); MoveOnLane(1, .1f);
                    result.voAttachmentLv30FemaleVerified = VoAvatarMode == "modular" && VoAvatarLevel == 30
                        && VoAvatarGender == "female" && VoAvatarUsesAlignedPaperDollMotion;
                }
                if (i == 43)
                {
                    SetVoRun(false); AdvanceVoAnimation(.5f); TriggerVoJump();
                    result.voAttachmentLv30FemaleVerified &= VoAvatarMotionState == "jump";
                }
                if (i == 44)
                {
                    AdvanceVoAnimation(.6f); TriggerVoBasicAttack();
                    result.voAttachmentLv30FemaleVerified &= VoAvatarMotionState == "basic_attack";
                }
                if (i == 45)
                {
                    AdvanceVoAnimation(.5f); TriggerVoSkill();
                    result.voAttachmentLv30FemaleVerified &= VoAvatarMotionState == "skill";
                }
                if (i >= 46 && i <= 65)
                {
                    var matrixIndex = (i - 46) % VoEquipmentSlots.Length;
                    var selected = VoEquipmentSlots[matrixIndex];
                    _voState.SetPresentation(i < 56 ? 0 : 1, i < 56 ? 0 : 3, 2, matrixIndex);
                    _voState.EquipAllExcept(selected);
                    foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = VoAvatarLevel;
                    RefreshVoAvatarMode();
                    var prefix = "lv" + VoAvatarLevel.ToString("000") + "_" + VoAvatarGender + "_";
                    var affected = _voEquipmentComponentInfo.Count(pair => pair.Key.StartsWith(prefix, StringComparison.Ordinal)
                        && pair.Value.slot == selected);
                    var matrixValid = affected >= 1
                        && _voAvatarParts.Values.Count(renderer => renderer.enabled) == 0
                        && _voRigParts.Values.Count(renderer => renderer.enabled) == 10
                        && _voEquipmentComponents.Values.Count(renderer => renderer.enabled) == 14 - affected;
                    result.voTenSlotMatrixVerified = i == 46 ? matrixValid : result.voTenSlotMatrixVerified && matrixValid;
                    var sharedStateValid = VoAvatarMode == "modular"
                        && VoAvatarGender == (i < 56 ? "male" : "female")
                        && VoAvatarLevel == (i < 56 ? 1 : 30)
                        && VoSelectedEquipmentSlot == selected
                        && VoEquippedSlotCount == 9 && !_voState.IsEquipped(selected);
                    result.voSharedRuntimeStateVerified = i == 46 ? sharedStateValid
                        : result.voSharedRuntimeStateVerified && sharedStateValid;
                }
                if (i == 66)
                {
                    AdvanceVoAnimation(.6f);
                    VoTrainingTargetHp = 100;
                    _voState.SetPresentation(0, 0, 2, 3);
                    _voState.EquipAllExcept(null);
                    foreach (var slot in VoEquipmentSlots) _voEquipmentLevels[slot] = 1;
                    for (var step = 0; step < 3; step++) CycleVoSelectedEquipmentItemLevel();
                    result.voMixedLevelMaleVerified = VoAvatarGender == "male" && VoAvatarLevel == 1
                        && VoSelectedEquipmentSlot == "outer_tunic" && VoSelectedEquipmentItemLevel == 30
                        && _voEquipmentComponents["lv030_male_outer_tunic_center"].enabled
                        && _voEquipmentComponents["lv001_male_waist_center"].enabled;
                }
                if (i == 67)
                {
                    SetVoRun(true); MoveOnLane(1, .1f);
                    result.voMixedLevelMotionVerified = VoAvatarMotionState == "run"
                        && _voEquipmentComponents["lv030_male_outer_tunic_center"].enabled;
                }
                if (i == 68)
                {
                    SetVoRun(false); AdvanceVoAnimation(.5f); TriggerVoJump();
                    result.voMixedLevelMotionVerified &= VoAvatarMotionState == "jump"
                        && _voEquipmentComponents["lv030_male_outer_tunic_center"].enabled;
                }
                if (i == 69)
                {
                    AdvanceVoAnimation(.6f); TriggerVoBasicAttack();
                    result.voMixedLevelMotionVerified &= VoAvatarMotionState == "basic_attack";
                }
                if (i == 70)
                {
                    AdvanceVoAnimation(.5f); TriggerVoSkill();
                    result.voMixedLevelMotionVerified &= VoAvatarMotionState == "skill";
                }
                if (i == 71)
                {
                    AdvanceVoAnimation(.6f); CycleVoAvatarGender();
                    result.voMixedLevelFemaleVerified = VoAvatarGender == "female"
                        && VoSelectedEquipmentItemLevel == 30
                        && _voEquipmentComponents["lv030_female_outer_tunic_center"].enabled
                        && _voEquipmentComponents["lv001_female_waist_center"].enabled;
                }
                if (i == 72)
                {
                    SetVoRun(true); MoveOnLane(1, .1f);
                    result.voMixedLevelMotionVerified &= VoAvatarMotionState == "run";
                }
                if (i == 73)
                {
                    SetVoRun(false); AdvanceVoAnimation(.5f); TriggerVoJump();
                    result.voMixedLevelMotionVerified &= VoAvatarMotionState == "jump";
                }
                if (i == 74)
                {
                    AdvanceVoAnimation(.6f); TriggerVoBasicAttack();
                    result.voMixedLevelMotionVerified &= VoAvatarMotionState == "basic_attack";
                }
                if (i == 75)
                {
                    AdvanceVoAnimation(.5f); TriggerVoSkill();
                    result.voMixedLevelMotionVerified &= VoAvatarMotionState == "skill";
                }
                if (i == 76)
                {
                    AdvanceVoAnimation(.6f); ToggleVoEquipmentSlot();
                    result.voMixedLevelToggleVerified = !_voEquipmentComponents["lv030_female_outer_tunic_center"].enabled;
                }
                if (i == 77)
                {
                    ToggleVoEquipmentSlot();
                    result.voMixedLevelToggleVerified &= _voEquipmentComponents["lv030_female_outer_tunic_center"].enabled;
                    result.voMixedEquipmentSnapshot = VoMixedEquipmentSnapshot;
                }
                // Preserve the inventory state while scripted quest actions consume or equip items,
                // then clear the tutorial overlay so later world evidence can actually be reviewed.
                if (questOnly && QuestCaptureRequiresWorldView(i)) InventoryOpen = false;
                _controller.RefreshForSmoke();
                Refresh();
                yield return null;
                yield return new WaitForEndOfFrame();
                if (questOnly && QuestCaptureRequiresWorldView(i))
                    result.questWorldFramesUnobstructed &= !InventoryOpen && !DialogueOpen;
                result.maxFootError = Mathf.Max(result.maxFootError, Mathf.Abs(FootY - GroundY));
                WriteMapCaptureBmp(Path.Combine(directory, names[i] + ".bmp"), result.width, result.height);
                result.frames++;
            }
            if (questOnly)
            {
                var quests = CompletedQuestCount; var health = HealthPotionCount; var mana = ManaPotionCount;
                var revisited = new List<string>();
                foreach (var routeIndex in new[] { 0, 2, 4, 5, 6, 7 })
                {
                    _routeX = RouteNodeX[routeIndex]; Refresh();
                    if (!UseNpcConversation()) throw new InvalidOperationException("Cannot revisit " + CurrentRouteNodeId);
                    yield return CaptureNpcDialoguePages(directory, CurrentRouteNodeId + "-return", result);
                    revisited.Add(CurrentRouteNodeId);
                }
                result.revisitedNpcs = revisited.ToArray();
                result.dialogueRevisitsVerified = CompletedQuestCount == quests && HealthPotionCount == health
                    && ManaPotionCount == mana && ActiveQuestId == "COMPLETE";
            }
            result.activeQuestId = ActiveQuestId;
            result.completedQuestCount = CompletedQuestCount;
            result.starterSupplies = HasStarterSupplies;
            result.spiritHerb = HasHarvestedSpiritHerb;
            result.hiddenChest = HasOpenedHiddenChest;
            result.combatAccepted = HasAcceptedCombatQuest;
            result.enemyDefeated = HasDefeatedFirstEnemy;
            result.enemyLooted = HasLootedFirstEnemy;
            result.portalUnlocked = PortalUnlocked;
            result.minimapUnlocked = MinimapUnlocked;
            result.healthPotionUsed = HasUsedHealthPotion;
            result.classRewardEquipped = IsClassRewardEquipped;
            result.healthPotionCount = HealthPotionCount;
            result.manaPotionCount = ManaPotionCount;
            result.playerHealth = PlayerHealth;
            result.parallaxDelta = FarOffset - initial;
            result.uiMetrics = RuntimeUiMetrics;
            result.worldMetrics = WorldPresentationMetrics;
            var mapFailed = !result.mapQuestFlowVerified || !result.functionalUiVerified
                || !result.dialogueOpened || !result.greetingCompleted
                || !result.inventoryItemDetailVerified
                || result.voSkillCastCount != 3 || result.voSkillHitCount != 3 || result.voTrainingTargetHp != 0
                || float.IsNaN(FootY) || result.maxFootError > .001f || Mathf.Abs(result.parallaxDelta) < .01f;
            mapFailed |= questOnly && !result.dialogueRevisitsVerified;
            mapFailed |= questOnly && !result.questWorldFramesUnobstructed;
            mapFailed |= questOnly && result.uiMetrics == null;
            mapFailed |= questOnly && (result.worldMetrics == null
                || string.IsNullOrEmpty(result.worldMetrics.profileName)
                || result.worldMetrics.actorScreenHeightRatio <= 0f
                || result.worldMetrics.npcScreenHeightRatio <= 0f
                || result.worldMetrics.backgroundCoverageScale <= 0f
                || result.worldMetrics.interactionMarkerFontSize <= 0
                || result.worldMetrics.interactionMarkerCharacterSize <= 0f);
            var wardrobeFailed = !questOnly && (!result.voBaseVerified || !result.voModularVerified
                || !result.voWalkVerified || !result.voSkillVerified || !result.voFemaleVerified || !result.voSlotToggleVerified
                || !result.voFemaleMotionVerified || !result.voProgressionVerified
                || !result.voAttachmentLv1Verified || !result.voAttachmentLv30FemaleVerified
                || !result.voEquipmentComponentBindingVerified
                || !result.voTenSlotMatrixVerified
                || !result.voSharedRuntimeStateVerified
                || !result.voMixedLevelMaleVerified || !result.voMixedLevelFemaleVerified
                || !result.voMixedLevelMotionVerified || !result.voMixedLevelToggleVerified
                || string.IsNullOrEmpty(result.voMixedEquipmentSnapshot));
            if (mapFailed || wardrobeFailed) result.status = "FIX_REQUIRED";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), JsonUtility.ToJson(result, true));
            Application.Quit(result.status == "FIX_REQUIRED" ? 1 : 0);
        }


        private IEnumerator CaptureEntryScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing Map01A entry capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            var imagePath = Path.Combine(directory, "entry-login.png");
            CaptureScreenPng(imagePath);
            var captureProductAuthStates = ShouldCaptureProductAuthStatesForArgs(args);
            var captureValidation = ShouldCaptureAuthValidationForArgs(args) || captureProductAuthStates;
            var validationPath = Path.Combine(directory, "entry-login-validation.png");
            var document = captureValidation || captureProductAuthStates ? GetComponentInChildren<UIDocument>() : null;
            if ((captureValidation || captureProductAuthStates) && document == null)
                throw new InvalidOperationException("Missing Map01A UIDocument for entry auth capture");
            if (captureValidation)
            {
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Entry Login Button"));
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(validationPath);
            }

            var authenticatingPath = Path.Combine(directory, "entry-login-authenticating.png");
            var invalidPath = Path.Combine(directory, "entry-login-invalid-credentials.png");
            var successPath = Path.Combine(directory, "entry-login-success-character-select.png");
            if (captureProductAuthStates)
            {
                var root = document.rootVisualElement;
                var entryStatus = root.Q<Label>("Map01A Entry Safety Note");
                var login = root.Q<Button>("Map01A Entry Login Button");
                if (entryStatus == null || login == null)
                    throw new InvalidOperationException("Missing Map01A product auth visual-state controls");
                entryStatus.text = "Đang xác thực…";
                login.SetEnabled(false);
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(authenticatingPath);

                entryStatus.text = "Tài khoản hoặc mật khẩu không đúng.";
                login.SetEnabled(true);
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(invalidPath);

                var hud = GetComponentsInChildren<MonoBehaviour>()
                    .FirstOrDefault(component => component.GetType().FullName == "LinhGioi.UI.CongDongLamArrivalHud");
                var openCharacterSelect = hud?.GetType().GetMethod("OpenCharacterSelect", BindingFlags.Instance | BindingFlags.NonPublic);
                if (openCharacterSelect == null)
                    throw new InvalidOperationException("Missing product auth success transition capture hook");
                openCharacterSelect.Invoke(hud, null);
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(successPath);
            }
            var productFramesReady = !captureProductAuthStates
                || (File.Exists(authenticatingPath) && File.Exists(invalidPath) && File.Exists(successPath));
            var status = File.Exists(imagePath) && (!captureValidation || File.Exists(validationPath)) && productFramesReady
                ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var validationManifest = captureValidation
                ? ",\n  \"validationFrame\": \"entry-login-validation.png\""
                : string.Empty;
            var productManifest = captureProductAuthStates
                ? ",\n  \"productAuthFrames\": [\"entry-login-authenticating.png\", \"entry-login-invalid-credentials.png\", \"entry-login-success-character-select.png\"]"
                : string.Empty;
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-entry-login\",\n"
                + "  \"entryOverlayExpected\": true,\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frame\": \"entry-login.png\"" + validationManifest + productManifest + "\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }


        private IEnumerator CaptureMenuScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing Map01A menu capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            var document = GetComponentInChildren<UIDocument>();
            if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for menu capture");
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Menu Shortcut"));
            yield return null;
            yield return new WaitForEndOfFrame();
            var imagePath = Path.Combine(directory, "menu.png");
            CaptureScreenPng(imagePath);
            var status = File.Exists(imagePath) ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-menu\",\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frame\": \"menu.png\"\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }

        private IEnumerator CaptureInventoryTabs(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing Map01A inventory tab capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            if (!InventoryOpen) ToggleInventory();
            yield return null;
            var document = GetComponentInChildren<UIDocument>();
            if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for inventory tab capture");
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Character Info Main Tab"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var characterInfo = Path.Combine(directory, "character-info.png");
            CaptureScreenPng(characterInfo);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Bag Main Tab"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var bag = Path.Combine(directory, "bag.png");
            CaptureScreenPng(bag);
            var equipmentFrames = new List<string>();
            foreach (var slot in EquipmentSlotIds)
            {
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Equipment Item Tile " + slot));
                yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
                yield return new WaitForEndOfFrame();
                var frame = "item-" + slot + "-selected.png";
                CaptureScreenPng(Path.Combine(directory, frame));
                equipmentFrames.Add(frame);
            }
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Equipment Item Tile main_weapon"));
            var search = document.rootVisualElement.Q<TextField>("Map01A Inventory Search");
            if (search == null) throw new InvalidOperationException("Missing Map01A inventory search for capture");
            search.value = "binh mau";
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var bagSearch = Path.Combine(directory, "bag-search-binh-mau.png");
            CaptureScreenPng(bagSearch);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Health Potion"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var bagSearchSelected = Path.Combine(directory, "bag-search-binh-mau-selected.png");
            CaptureScreenPng(bagSearchSelected);
            search.value = string.Empty;
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Skills Main Tab"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var skillsDefault = Path.Combine(directory, "skills-default.png");
            CaptureScreenPng(skillsDefault);
            var skillNodes = document.rootVisualElement.Query<Button>(className: "lgo-skill-node").ToList();
            if (skillNodes.Count != 9) throw new InvalidOperationException("Missing Character Hub shared nine-node skill path for capture");
            InvokeHudButton(skillNodes[2]);
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var skills = Path.Combine(directory, "skills.png");
            CaptureScreenPng(skills);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Potential Main Tab"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var potential = Path.Combine(directory, "potential.png");
            var potentialDefault = Path.Combine(directory, "potential-default.png");
            CaptureScreenPng(potentialDefault);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Potential Node 0"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            CaptureScreenPng(potential);
            var hud = GetComponentsInChildren<MonoBehaviour>()
                .FirstOrDefault(component => component.GetType().FullName == "LinhGioi.UI.CongDongLamArrivalHud");
            if (hud == null) throw new InvalidOperationException("Missing Character Hub HUD for five-profile capture");
            var bindEvidenceClass = hud.GetType().GetMethod("BindCharacterHubEvidenceClass",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var clearEvidenceClass = hud.GetType().GetMethod("ClearCharacterHubEvidenceClass",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (bindEvidenceClass == null || clearEvidenceClass == null)
                throw new InvalidOperationException("Missing Character Hub five-profile evidence hooks");
            var rendererClassId = ActiveEquipmentClassId;
            var potentialClassIds = new[] { "vo", "kiem", "phap", "co", "linh" };
            var skillSelectedNodeIndex = ReadCharacterHubSkillCaptureNode(args);
            var skillClassFrames = new List<string>();
            foreach (var classId in potentialClassIds)
            {
                bindEvidenceClass.Invoke(hud, new object[] { classId });
                if (ActiveEquipmentClassId != rendererClassId)
                    throw new InvalidOperationException("Character Hub evidence binding changed renderer authority");
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Skills Main Tab"));
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Skill Node 0"));
                yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
                yield return new WaitForEndOfFrame();
                var frame = "skills-" + classId + ".png";
                CaptureScreenPng(Path.Combine(directory, frame));
                skillClassFrames.Add(frame);
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Skill Node " + skillSelectedNodeIndex));
                yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
                yield return new WaitForEndOfFrame();
                var selectedFrame = "skills-" + classId + "-selected.png";
                CaptureScreenPng(Path.Combine(directory, selectedFrame));
                skillClassFrames.Add(selectedFrame);
            }
            clearEvidenceClass.Invoke(hud, null);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Potential Main Tab"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var potentialClassFrames = new List<string>();
            foreach (var classId in potentialClassIds)
            {
                bindEvidenceClass.Invoke(hud, new object[] { classId });
                if (ActiveEquipmentClassId != rendererClassId)
                    throw new InvalidOperationException("Character Hub evidence binding changed renderer authority");
                yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
                yield return new WaitForEndOfFrame();
                var defaultFrame = "potential-" + classId + "-default.png";
                CaptureScreenPng(Path.Combine(directory, defaultFrame));
                potentialClassFrames.Add(defaultFrame);
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Potential Node 0"));
                yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
                yield return new WaitForEndOfFrame();
                var selectedFrame = "potential-" + classId + "-selected.png";
                CaptureScreenPng(Path.Combine(directory, selectedFrame));
                potentialClassFrames.Add(selectedFrame);
            }
            clearEvidenceClass.Invoke(hud, null);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Spirit Pet Main Tab"));
            yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
            yield return new WaitForEndOfFrame();
            var spiritPet = Path.Combine(directory, "spirit-pet.png");
            CaptureScreenPng(spiritPet);
            var spiritPetClassFrames = new List<string>();
            foreach (var classId in potentialClassIds)
            {
                bindEvidenceClass.Invoke(hud, new object[] { classId });
                if (ActiveEquipmentClassId != rendererClassId)
                    throw new InvalidOperationException("Character Hub evidence binding changed renderer authority");
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Spirit Pet Main Tab"));
                yield return new WaitForSecondsRealtime(CharacterHubAnimationSettleSeconds);
                yield return new WaitForEndOfFrame();
                var frame = "spirit-pet-" + classId + ".png";
                CaptureScreenPng(Path.Combine(directory, frame));
                spiritPetClassFrames.Add(frame);
            }
            clearEvidenceClass.Invoke(hud, null);
            var skillClassFramesExist = skillClassFrames.All(frame => File.Exists(Path.Combine(directory, frame)));
            var potentialClassFramesExist = potentialClassFrames.All(frame => File.Exists(Path.Combine(directory, frame)));
            var spiritPetClassFramesExist = spiritPetClassFrames.All(frame => File.Exists(Path.Combine(directory, frame)));
            var status = File.Exists(characterInfo) && File.Exists(bag) && File.Exists(bagSearch)
                && File.Exists(bagSearchSelected) && equipmentFrames.All(frame => File.Exists(Path.Combine(directory, frame)))
                && File.Exists(skillsDefault) && File.Exists(skills) && File.Exists(potentialDefault) && File.Exists(potential)
                && skillClassFramesExist && potentialClassFramesExist && File.Exists(spiritPet) && spiritPetClassFramesExist
                && RuntimeUiMetrics != null
                ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var frames = new[] { "character-info.png", "bag.png", "bag-search-binh-mau.png", "bag-search-binh-mau-selected.png",
                "skills-default.png", "skills.png" }
                .Concat(equipmentFrames)
                .Concat(skillClassFrames)
                .Concat(new[] { "potential-default.png", "potential.png" })
                .Concat(potentialClassFrames)
                .Concat(new[] { "spirit-pet.png" })
                .Concat(spiritPetClassFrames);
            var uiMetricsJson = RuntimeUiMetrics == null ? "null" : JsonUtility.ToJson(RuntimeUiMetrics);
            var evidenceAuthority = RuntimeUiMetrics == null ? "missing" : RuntimeUiMetrics.evidenceAuthority;
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-inventory-tabs\",\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"evidenceAuthority\": \"" + evidenceAuthority + "\",\n"
                + "  \"uiMetrics\": " + uiMetricsJson + ",\n"
                + "  \"skillClassProfiles\": [\"vo\", \"kiem\", \"phap\", \"co\", \"linh\"],\n"
                + "  \"skillSelectedNodeIndex\": " + skillSelectedNodeIndex + ",\n"
                + "  \"potentialClassProfiles\": [\"vo\", \"kiem\", \"phap\", \"co\", \"linh\"],\n"
                + "  \"spiritPetClassProfiles\": [\"vo\", \"kiem\", \"phap\", \"co\", \"linh\"],\n"
                + "  \"classSwitchScope\": \"character-hub-data-only-no-renderer-change\",\n"
                + "  \"frames\": [\"" + string.Join("\", \"", frames) + "\"]\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }

        private static int ReadCharacterHubSkillCaptureNode(string[] args)
        {
            const string flag = "--lgo-character-hub-skill-node";
            var index = Array.IndexOf(args, flag);
            if (index < 0) return 5;
            if (index != Array.LastIndexOf(args, flag) || index + 1 >= args.Length
                || !int.TryParse(args[index + 1], out var node) || node < 0 || node > 8)
                throw new ArgumentException("Character Hub skill capture node must be a single integer from 0 to 8");
            return node;
        }

        private IEnumerator CaptureCharacterScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length)
                throw new ArgumentException("Missing Map01A character screen capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            if (!InventoryOpen) ToggleInventory();
            var document = GetComponentInChildren<UIDocument>();
            if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for character screen capture");
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Character Info Main Tab"));
            yield return null;
            yield return new WaitForEndOfFrame();
            var imagePath = Path.Combine(directory, "character-info.png");
            CaptureScreenPng(imagePath);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Character Hero Quick Icon 2"));
            yield return null;
            yield return new WaitForEndOfFrame();
            var selectedPath = Path.Combine(directory, "character-info-selected.png");
            CaptureScreenPng(selectedPath);
            InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Inventory Detail Lock Action"));
            yield return null;
            yield return new WaitForEndOfFrame();
            var lockedPath = Path.Combine(directory, "character-info-locked.png");
            CaptureScreenPng(lockedPath);
            var iconsReady = true;
            for (var slotIndex = 0; slotIndex < VoEquipmentSlots.Length; slotIndex++)
                iconsReady &= GetMap01ACharacterEquipmentIconSprite(VoEquipmentSlots[slotIndex]) != null;
            var status = File.Exists(imagePath) && File.Exists(selectedPath) && File.Exists(lockedPath) && iconsReady
                ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED"
                : "FIX_REQUIRED";
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-character-screen\",\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"dedicatedEquipmentIcons\": " + (iconsReady ? "true" : "false") + ",\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frames\": [\"character-info.png\", \"character-info-selected.png\", \"character-info-locked.png\"]\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }


        private static void InvokeHudButton(Button button)
        {
            if (button == null) throw new InvalidOperationException("Missing Map01A HUD button for capture");
            var callback = typeof(Clickable).GetField("clicked", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(button.clickable) as Action;
            if (callback == null) throw new InvalidOperationException("Map01A HUD button has no capture callback: " + button.name);
            callback();
        }

        private static void CaptureScreenPng(string imagePath)
        {
            var image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            image.Apply();
            File.WriteAllBytes(imagePath, image.EncodeToPNG());
            UnityEngine.Object.Destroy(image);
        }

        private IEnumerator CaptureCharacterEntryScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length)
                throw new ArgumentException("Missing Map01A character entry capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            var document = GetComponentInChildren<UIDocument>();
            if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for character entry capture");
            var overlay = document.rootVisualElement.Q("Map01A Character Select Overlay");
            var enter = document.rootVisualElement.Q<Button>("Map01A Character Select Enter Game");
            if (overlay == null || enter == null)
                throw new InvalidOperationException("Missing Character Select entry controls for capture");
            yield return new WaitForEndOfFrame();
            var selectPath = Path.Combine(directory, "character-select.png");
            CaptureScreenPng(selectPath);
            var selectVisible = overlay.style.display.value == DisplayStyle.Flex;

            InvokeHudButton(enter);
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            var entryPath = Path.Combine(directory, "map01a-entry.png");
            CaptureScreenPng(entryPath);
            var entryApplied = overlay.style.display.value == DisplayStyle.None
                && Mathf.Abs(PlayerX - 18.5f) <= .001f
                && ProductEntryFacingSign == -1
                && LoadedProductRuntimeClassId == "linh"
                && ActiveEquipmentClassId == "vo"
                && CurrentRouteNodeId == "village-square";
            var status = File.Exists(selectPath) && File.Exists(entryPath) && selectVisible && entryApplied
                ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-character-entry\",\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"usesNetwork\": false,\n"
                + "  \"characterSelectOverlayExpected\": true,\n"
                + "  \"entryOverlayClosed\": " + (overlay.style.display.value == DisplayStyle.None ? "true" : "false") + ",\n"
                + "  \"runtimeClassId\": \"" + (LoadedProductRuntimeClassId ?? "") + "\",\n"
                + "  \"rendererClassId\": \"" + ActiveEquipmentClassId + "\",\n"
                + "  \"laneX\": " + PlayerX.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\n"
                + "  \"facing\": " + ProductEntryFacingSign + ",\n"
                + "  \"routeNode\": \"" + CurrentRouteNodeId + "\",\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frames\": [\"character-select.png\", \"map01a-entry.png\"]\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }

        private IEnumerator CaptureCharacterSelectScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing Map01A character select capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            var imagePath = Path.Combine(directory, "character-select.png");
            var image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            image.Apply();
            File.WriteAllBytes(imagePath, image.EncodeToPNG());
            Destroy(image);
            var status = File.Exists(imagePath) ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-character-select\",\n"
                + "  \"characterSelectOverlayExpected\": true,\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frame\": \"character-select.png\"\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }

        private IEnumerator CaptureServerSelectScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length)
                throw new ArgumentException("Missing Map01A server select capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            var imagePath = Path.Combine(directory, "server-select.png");
            CaptureScreenPng(imagePath);
            var status = File.Exists(imagePath) ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-server-select\",\n"
                + "  \"serverSelectOverlayExpected\": true,\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frame\": \"server-select.png\"\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }

        private IEnumerator CaptureRegisterScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length)
                throw new ArgumentException("Missing Map01A register capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            var imagePath = Path.Combine(directory, "register-account.png");
            CaptureScreenPng(imagePath);
            var captureValidation = ShouldCaptureAuthValidationForArgs(args);
            var validationPath = Path.Combine(directory, "register-validation.png");
            if (captureValidation)
            {
                var document = GetComponentInChildren<UIDocument>();
                if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for register validation capture");
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Register Submit"));
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(validationPath);
            }
            var captureProductAccountStates = ShouldCaptureProductAccountStatesForArgs(args);
            var loadingPath = Path.Combine(directory, "register-loading.png");
            var conflictPath = Path.Combine(directory, "register-conflict.png");
            if (captureProductAccountStates)
            {
                var document = GetComponentInChildren<UIDocument>();
                if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for register state capture");
                var root = document.rootVisualElement;
                var registerStatus = root.Q<Label>("Map01A Register Status");
                var submit = root.Q<Button>("Map01A Register Submit");
                if (registerStatus == null || submit == null)
                    throw new InvalidOperationException("Missing Map01A register state controls");
                registerStatus.text = "Đang tạo tài khoản…";
                submit.SetEnabled(false);
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(loadingPath);
                registerStatus.text = "Email này đã được đăng ký.";
                submit.SetEnabled(true);
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(conflictPath);
            }
            var productStateFramesReady = !captureProductAccountStates
                || (File.Exists(loadingPath) && File.Exists(conflictPath));
            var status = File.Exists(imagePath) && (!captureValidation || File.Exists(validationPath)) && productStateFramesReady
                ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var validationManifest = captureValidation
                ? ",\n  \"validationFrame\": \"register-validation.png\""
                : string.Empty;
            var productStateManifest = captureProductAccountStates
                ? ",\n  \"productAccountFrames\": [\"register-loading.png\", \"register-conflict.png\"]"
                : string.Empty;
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"map01a-register\",\n"
                + "  \"registerOverlayExpected\": true,\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frame\": \"register-account.png\"" + validationManifest + productStateManifest + "\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }

        private IEnumerator CapturePasswordRecoveryScreen(string[] args)
        {
            Application.runInBackground = true;
            var index = Array.IndexOf(args, "--lgo-map01a-art-dir");
            if (index < 0 || index + 1 >= args.Length)
                throw new ArgumentException("Missing Map01A password recovery capture directory");
            var directory = args[index + 1];
            Directory.CreateDirectory(directory);
            _controller.enabled = false;
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            var verifyCapture = Array.IndexOf(args, "--lgo-map01a-password-recovery-verify-capture") >= 0;
            var newPasswordCapture = Array.IndexOf(args, "--lgo-map01a-password-recovery-new-password-capture") >= 0;
            var frameName = verifyCapture ? "password-recovery-verify.png"
                : newPasswordCapture ? "password-recovery-new-password.png" : "password-recovery-request.png";
            var captureScope = verifyCapture ? "map01a-password-recovery-verify"
                : newPasswordCapture ? "map01a-password-recovery-new-password" : "map01a-password-recovery-request";
            var imagePath = Path.Combine(directory, frameName);
            CaptureScreenPng(imagePath);
            var captureValidation = !verifyCapture && !newPasswordCapture && ShouldCaptureAuthValidationForArgs(args);
            var validationPath = Path.Combine(directory, "password-recovery-validation.png");
            if (captureValidation)
            {
                var document = GetComponentInChildren<UIDocument>();
                if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for recovery validation capture");
                InvokeHudButton(document.rootVisualElement.Q<Button>("Map01A Password Recovery Submit"));
                yield return null;
                yield return new WaitForEndOfFrame();
                CaptureScreenPng(validationPath);
            }
            var captureProductAccountStates = ShouldCaptureProductAccountStatesForArgs(args);
            string[] productStateFrames = Array.Empty<string>();
            if (captureProductAccountStates)
            {
                var document = GetComponentInChildren<UIDocument>();
                if (document == null) throw new InvalidOperationException("Missing Map01A UIDocument for recovery state capture");
                var root = document.rootVisualElement;
                var recoveryStatus = root.Q<Label>("Map01A Password Recovery Status");
                if (recoveryStatus == null) throw new InvalidOperationException("Missing Map01A recovery status control");
                if (verifyCapture)
                {
                    recoveryStatus.text = "Mã xác minh không hợp lệ hoặc đã hết hạn.";
                    productStateFrames = new[] { "password-recovery-verify-invalid-expired.png" };
                    yield return null;
                    yield return new WaitForEndOfFrame();
                    CaptureScreenPng(Path.Combine(directory, productStateFrames[0]));
                }
                else if (newPasswordCapture)
                {
                    recoveryStatus.text = "Mật khẩu phải từ 8 đến 128 ký tự.";
                    productStateFrames = new[] { "password-recovery-new-password-rule.png" };
                    yield return null;
                    yield return new WaitForEndOfFrame();
                    CaptureScreenPng(Path.Combine(directory, productStateFrames[0]));
                }
                else
                {
                    var submit = root.Q<Button>("Map01A Password Recovery Submit");
                    if (submit == null) throw new InvalidOperationException("Missing Map01A recovery submit control");
                    recoveryStatus.text = "Đang gửi mã xác minh…";
                    submit.SetEnabled(false);
                    productStateFrames = new[] { "password-recovery-loading.png", "password-recovery-unavailable.png" };
                    yield return null;
                    yield return new WaitForEndOfFrame();
                    CaptureScreenPng(Path.Combine(directory, productStateFrames[0]));
                    recoveryStatus.text = "Dịch vụ gửi mã hiện chưa khả dụng.";
                    submit.SetEnabled(true);
                    yield return null;
                    yield return new WaitForEndOfFrame();
                    CaptureScreenPng(Path.Combine(directory, productStateFrames[1]));
                }
            }
            var productStateFramesReady = !captureProductAccountStates
                || productStateFrames.All(frame => File.Exists(Path.Combine(directory, frame)));
            var status = File.Exists(imagePath) && (!captureValidation || File.Exists(validationPath)) && productStateFramesReady
                ? "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED" : "FIX_REQUIRED";
            var validationManifest = captureValidation
                ? ",\n  \"validationFrame\": \"password-recovery-validation.png\""
                : string.Empty;
            var productStateManifest = captureProductAccountStates
                ? ",\n  \"productAccountFrames\": [" + string.Join(", ", productStateFrames.Select(frame => "\"" + frame + "\"")) + "]"
                : string.Empty;
            var manifest = "{\n"
                + "  \"status\": \"" + status + "\",\n"
                + "  \"captureScope\": \"" + captureScope + "\",\n"
                + "  \"passwordRecoveryOverlayExpected\": true,\n"
                + "  \"usesOsMouseOrKeyboard\": false,\n"
                + "  \"width\": " + Screen.width + ",\n"
                + "  \"height\": " + Screen.height + ",\n"
                + "  \"frame\": \"" + frameName + "\"" + validationManifest + productStateManifest + "\n"
                + "}\n";
            File.WriteAllText(Path.Combine(directory, "manifest.json"), manifest);
            Application.Quit(status == "FIX_REQUIRED" ? 1 : 0);
        }

    }
}
