namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner.Editor
{

    using Assets.UnityTestTools.Common.Editor;

    public class IntegrationTestsRunnerSettings : ProjectSettingsBase
    {
        public bool blockUIWhenRunning = true;
        public bool pauseOnTestFailure;
        
        public void ToggleBlockUIWhenRunning ()
        {
            this.blockUIWhenRunning = !this.blockUIWhenRunning;
            this.Save ();
        }
        
        public void TogglePauseOnTestFailure()
        {
            this.pauseOnTestFailure = !this.pauseOnTestFailure;
            this.Save ();
        }
    }
}
