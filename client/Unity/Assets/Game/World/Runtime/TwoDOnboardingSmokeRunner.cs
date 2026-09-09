using System;
using System.IO;
using UnityEngine;

namespace LinhGioi.World
{
    public static class TwoDOnboardingSmokeRunner
    {
        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_2D_ONBOARDING_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-2d-onboarding-smoke") return true;
            return false;
        }

        public static void RunFromCommandLine()
        {
            var resultPath = GetArg("--lgo-2d-result") ?? Path.Combine(Application.persistentDataPath, "lgo-2d-onboarding-smoke-result.json");
            var result = new TwoDOnboardingSmokeResult
            {
                status = "STARTED",
                startedAtUtc = DateTimeOffset.UtcNow.ToString("O"),
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                resultPath = resultPath
            };
            var exitCode = 99;

            try
            {
                var state = new TwoDOnboardingState();
                state.Reset();
                Require(state.Step == TwoDOnboardingStep.FindGateKeeper, "initial step mismatch");
                Require(state.AvailableAction == TwoDOnboardingAction.None, "initial action should be none");

                state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
                Require(state.Step == TwoDOnboardingStep.FindGateKeeper, "training stone should be locked before gate dialogue");
                Require(state.AvailableAction == TwoDOnboardingAction.None, "stone action available too early");

                state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
                Require(state.AvailableAction == TwoDOnboardingAction.Talk, "gate keeper talk action missing");
                Require(state.TryUseAction(), "talk action failed");
                Require(state.DialogueOpen, "gate dialogue did not open");
                Require(state.DialogueLine.Contains("Chào mừng đến Linh Thành"), "gate dialogue copy drifted");

                Require(state.TryUseAction(), "continue action failed");
                Require(state.Step == TwoDOnboardingStep.GoToTrainingStone, "did not advance to training stone objective");
                Require(!state.DialogueOpen, "dialogue should close before training objective");

                state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
                Require(state.AvailableAction == TwoDOnboardingAction.Train, "training action missing");
                Require(state.TryUseAction(), "training action failed");
                Require(state.Step == TwoDOnboardingStep.LearnJump, "training should unlock jump lesson");
                Require(state.TryUseJump(), "jump lesson failed");
                Require(state.Step == TwoDOnboardingStep.LearnDash, "jump should unlock dash lesson");
                Require(state.TryUseDash(), "dash lesson failed");
                Require(state.Step == TwoDOnboardingStep.LearnClassSkill, "dash should unlock class skill lesson");
                Require(state.TryUseClassSkill(), "class skill lesson failed");
                Require(state.Step == TwoDOnboardingStep.Complete, "2D onboarding did not complete");
                Require(state.ObjectiveText.Contains("Hoàn tất nhập môn"), "completion copy drifted");

                result.finalStep = state.Step.ToString();
                result.finalObjective = state.ObjectiveText;
                result.finalFeedback = state.FeedbackText;
                result.status = "PASS";
                exitCode = 0;
            }
            catch (Exception exception)
            {
                result.status = "FAIL";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 14;
            }
            finally
            {
                result.finishedAtUtc = DateTimeOffset.UtcNow.ToString("O");
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log($"[LinhGioi] 2D onboarding smoke status={result.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void WriteResult(string path, TwoDOnboardingSmokeResult result)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, JsonUtility.ToJson(result, true));
        }

        private static void Quit(int exitCode)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(exitCode);
#else
            Application.Quit(exitCode);
#endif
        }

        [Serializable]
        private sealed class TwoDOnboardingSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string finalStep;
            public string finalObjective;
            public string finalFeedback;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
