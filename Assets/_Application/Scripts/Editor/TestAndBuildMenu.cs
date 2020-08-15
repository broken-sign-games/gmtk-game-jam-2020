using System;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace GMTK2020.Editor
{
    [InitializeOnLoad]
    public static class TestAndBuildMenu
    {
        private delegate void TestRunFinishedHandler(bool passed);

        private class TestCallbacks : IErrorCallbacks
        {
            public TestRunFinishedHandler RunFinishedCallback;

            public void OnError(string message)
            {
                Debug.LogError(message);
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                RunFinishedCallback(result.TestStatus == TestStatus.Passed);
            }

            public void RunStarted(ITestAdaptor testsToRun) { }
            public void TestFinished(ITestResultAdaptor result) { }
            public void TestStarted(ITestAdaptor test) { }
        }

        private static readonly TestCallbacks testCallbacks;
        private static readonly TestRunnerApi testApi;

        // TODO: Store this in a scriptable object
        private const string BUILD_BASE_NAME = "5-steps-ahead";

        static TestAndBuildMenu()
        {
            testApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testCallbacks = new TestCallbacks
            {
                RunFinishedCallback = CheckPlayModeTestResults
            };
            testApi.RegisterCallbacks(testCallbacks);
        }

        [MenuItem("Build/Build all _F7")]
        public static void ExportPluginEntryPoint()
        {
            Debug.Log("Starting build...");

            Debug.Log("Running EditMode tests...");
            RunTests(TestMode.EditMode);

            // Callback will continue control flow in CheckEditModeTestResults
        }

        private static void RunTests(TestMode testMode)
        {
            var modeFilter = new Filter() { testMode = testMode };

            // This will be overridden by the static constructor for PlayMode tests.
            testCallbacks.RunFinishedCallback = CheckEditModeTestResults;

            testApi.Execute(new ExecutionSettings(modeFilter));
        }

        private static void CheckEditModeTestResults(bool passed)
        {
            if (passed)
                Debug.Log("EditMode tests passed.");
            else
            {
                Debug.Log("EditMode tests failed, aborting build.");
                return;
            }

            Debug.Log("Running PlayMode tests...");
            //RunTests(TestMode.PlayMode);

            BuildAll();

            // Callback will continue control flow in CheckPlayModeTestResults
        }

        private static void CheckPlayModeTestResults(bool passed)
        {
            if (passed)
                Debug.Log("PlayMode tests passed.");
            else
            {
                Debug.Log("PlayMode tests failed, aborting build.");
                return;
            }

            BuildAll();
        }

        private static void BuildAll()
        {
            Build(BuildTarget.StandaloneWindows64);
            Build(BuildTarget.StandaloneOSX);
            Build(BuildTarget.StandaloneLinux64);
        }

        private static void Build(BuildTarget buildTarget)
        {
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray(),
                locationPathName = GetBuildPath(buildTarget),
                target = buildTarget,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }

        private static string GetBuildPath(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
            case BuildTarget.StandaloneWindows64:
                return $"Builds/{BUILD_BASE_NAME}-windows/{BUILD_BASE_NAME}.exe";
            case BuildTarget.StandaloneLinux64:
                return $"Builds/{BUILD_BASE_NAME}-linux/{BUILD_BASE_NAME}.x86_64";
            case BuildTarget.StandaloneOSX:
                return $"Builds/{BUILD_BASE_NAME}-osx.app";
            case BuildTarget.iOS:
            case BuildTarget.Android:
            case BuildTarget.WebGL:
                throw new NotImplementedException("Build target not yet supported.");
            default:
                throw new InvalidEnumArgumentException(nameof(buildTarget), (int)buildTarget, typeof(BuildTarget));
            }
        }
    }
}
