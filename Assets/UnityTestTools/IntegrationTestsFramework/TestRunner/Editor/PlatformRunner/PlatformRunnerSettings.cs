namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.PlatformRunner
{

    using Assets.UnityTestTools.Common.Editor;

    public class PlatformRunnerSettings : ProjectSettingsBase
    {
        public string resultsPath;
        public bool sendResultsOverNetwork = true;
        public int port = 0;
    }
}
