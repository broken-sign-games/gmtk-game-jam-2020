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
        private const string BUILD_BASE_NAME = "alkahest";

        static TestAndBuildMenu()
        {
            testApi = ScriptableObject.CreateInstance<TestRunnerApi>();

            testCallbacks = new TestCallbacks
            {
                RunFinishedCallback = CheckEditModeTestResults
            };
        }

        [MenuItem("Build/Build all _F7")]
        public static void ExportPluginEntryPoint()
        {
            Debug.Log("Starting build...");

            Debug.Log("Running EditMode tests...");

            testApi.RegisterCallbacks(testCallbacks);
            RunTests(TestMode.EditMode);

            // Callback will continue control flow in CheckEditModeTestResults
        }

        private static void RunTests(TestMode testMode)
        {
            var modeFilter = new Filter() { testMode = testMode };

            testApi.Execute(new ExecutionSettings(modeFilter));
        }

        private static void CheckEditModeTestResults(bool passed)
        {
            testApi.UnregisterCallbacks(testCallbacks);

            if (passed)
                Debug.Log("EditMode tests passed.");
            else
            {
                Debug.Log("EditMode tests failed, aborting build.");
                return;
            }

            // TODO: Run playmode tests in a way that doesn't set up a
            // test callbacks for test runs started from the editor.

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
