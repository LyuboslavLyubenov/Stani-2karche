namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.PlatformRunner
{

    using System.Collections.Generic;
    using System.IO;

    using UnityEditor;

    class PlayerSettingConfigurator
    {
        private string resourcesPath {
            get { return this.m_Temp ? k_TempPath : this.m_ProjectResourcesPath; }
        }

        private readonly string m_ProjectResourcesPath = Path.Combine("Assets", "Resources");
        const string k_TempPath = "Temp";
        private readonly bool m_Temp;

        private ResolutionDialogSetting m_DisplayResolutionDialog;
        private bool m_RunInBackground;
        private bool m_FullScreen;
        private bool m_ResizableWindow;
        private readonly List<string> m_TempFileList = new List<string>();

        public PlayerSettingConfigurator(bool saveInTempFolder)
        {
            this.m_Temp = saveInTempFolder;
        }

        public void ChangeSettingsForIntegrationTests()
        {
            this.m_DisplayResolutionDialog = PlayerSettings.displayResolutionDialog;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

            this.m_RunInBackground = PlayerSettings.runInBackground;
            PlayerSettings.runInBackground = true;

            this.m_FullScreen = PlayerSettings.defaultIsFullScreen;
            PlayerSettings.defaultIsFullScreen = false;

            this.m_ResizableWindow = PlayerSettings.resizableWindow;
            PlayerSettings.resizableWindow = true;
        }

        public void RevertSettingsChanges()
        {
            PlayerSettings.defaultIsFullScreen = this.m_FullScreen;
            PlayerSettings.runInBackground = this.m_RunInBackground;
            PlayerSettings.displayResolutionDialog = this.m_DisplayResolutionDialog;
            PlayerSettings.resizableWindow = this.m_ResizableWindow;
        }

        public void AddConfigurationFile(string fileName, string content)
        {
            var resourcesPathExists = Directory.Exists(this.resourcesPath);
            if (!resourcesPathExists) AssetDatabase.CreateFolder("Assets", "Resources");

            var filePath = Path.Combine(this.resourcesPath, fileName);
            File.WriteAllText(filePath, content);

            this.m_TempFileList.Add(filePath);
        }

        public void RemoveAllConfigurationFiles()
        {
            foreach (var filePath in this.m_TempFileList)
                AssetDatabase.DeleteAsset(filePath);
            if (Directory.Exists(this.resourcesPath)
                && Directory.GetFiles(this.resourcesPath).Length == 0)
                AssetDatabase.DeleteAsset(this.resourcesPath);
        }
    }
}
