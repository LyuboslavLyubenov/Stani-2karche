namespace UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.PlatformRunner
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using UnityEditor;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using UnityTestTools.Common.Editor;
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Object = UnityEngine.Object;

    [Serializable]
    public class PlatformRunnerSettingsWindow : EditorWindow
    {
        private BuildTarget m_BuildTarget;

        private List<string> m_IntegrationTestScenes;
        private List<string> m_OtherScenesToBuild;
        private List<string> m_AllScenesInProject;

        private Vector2 m_ScrollPositionIntegrationTests;
        private Vector2 m_ScrollPositionOtherScenes;
        private Vector2 m_ScrollPositionAllScenes;
        private readonly List<string> m_Interfaces = new List<string>();
        private readonly List<string> m_SelectedScenes = new List<string>();

        private int m_SelectedInterface;
        [SerializeField]
        private bool m_AdvancedNetworkingSettings;

        private PlatformRunnerSettings m_Settings;

        private string m_SelectedSceneInAll;
        private string m_SelectedSceneInTest;
        private string m_SelectedSceneInBuild;

        readonly GUIContent m_Label = new GUIContent("Results target directory", "Directory where the results will be saved. If no value is specified, the results will be generated in project's data folder.");
        
        void Awake()
        {
            if (this.m_OtherScenesToBuild == null)
                this.m_OtherScenesToBuild = new List<string> ();

            if (this.m_IntegrationTestScenes == null)
                this.m_IntegrationTestScenes = new List<string> ();

            this.titleContent = new GUIContent("Platform runner");
            this.m_BuildTarget = PlatformRunner.defaultBuildTarget;
            this.position.Set(this.position.xMin, this.position.yMin, 200, this.position.height);
            this.m_AllScenesInProject = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.unity", SearchOption.AllDirectories).ToList();
            this.m_AllScenesInProject.Sort();
            var currentScene = (Directory.GetCurrentDirectory() + SceneManager.GetActiveScene().path).Replace("\\", "").Replace("/", "");
            var currentScenePath = this.m_AllScenesInProject.Where(s => s.Replace("\\", "").Replace("/", "") == currentScene);
            this.m_SelectedScenes.AddRange(currentScenePath);

            this.m_Interfaces.Add("(Any)");
            this.m_Interfaces.AddRange(TestRunnerConfigurator.GetAvailableNetworkIPs());
            this.m_Interfaces.Add("127.0.0.1");

            this.LoadFromPrefereneces ();
        }

        public void OnEnable()
        {
            this.m_Settings = ProjectSettingsBase.Load<PlatformRunnerSettings>();

            // If not configured pre populate with all scenes that have test components on game objects
            // This needs to be done outsie of constructor
            if (this.m_IntegrationTestScenes.Count == 0)
                this.m_IntegrationTestScenes = GetScenesWithTestComponents (this.m_AllScenesInProject);
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
                GUIContent label;

                /* We have three lists here, The tests to run, supporting scenes to include in the build and the list of all scenes so users can
                 * pick the scenes they want to include. The motiviation here is that test scenes may require to additively load other scenes as part of the tests
                 */
                EditorGUILayout.BeginHorizontal ();

                    // Integration Tests To Run
                    EditorGUILayout.BeginVertical ();

                    label = new GUIContent("Tests:", "All Integration Test scenes that you wish to run on the platform");
                    EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(20f));

                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(this.m_SelectedSceneInTest));
                        if (GUILayout.Button("Remove Integration Test")) {
                        this.m_IntegrationTestScenes.Remove(this.m_SelectedSceneInTest);
                        this.m_SelectedSceneInTest = "";
                    }
                    EditorGUI.EndDisabledGroup();

                    this.DrawVerticalSceneList (ref this.m_IntegrationTestScenes, ref this.m_SelectedSceneInTest, ref this.m_ScrollPositionIntegrationTests);
                    EditorGUILayout.EndVertical ();
        
                    // Extra scenes to include in build
                    EditorGUILayout.BeginVertical ();
                        label = new GUIContent("Other Scenes in Build:", "If your Integration Tests additivly load any other scenes then you want to include them here so they are part of the build");
                        EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(20f));

            
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(this.m_SelectedSceneInBuild));
                    if (GUILayout.Button("Remove From Build")) {
                        this.m_OtherScenesToBuild.Remove(this.m_SelectedSceneInBuild);
                        this.m_SelectedSceneInBuild = "";
                    }
                    EditorGUI.EndDisabledGroup();

                    this.DrawVerticalSceneList (ref this.m_OtherScenesToBuild, ref this.m_SelectedSceneInBuild, ref this.m_ScrollPositionOtherScenes);
                    EditorGUILayout.EndVertical ();

                    EditorGUILayout.Separator ();

                    // All Scenes
                    EditorGUILayout.BeginVertical ();
                    label = new GUIContent("Available Scenes", "These are all the scenes within your project, please select some to run tests");
                    EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(20f));

            
                    EditorGUILayout.BeginHorizontal ();
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(this.m_SelectedSceneInAll));
                    if (GUILayout.Button("Add As Test")) {
                        if (!this.m_IntegrationTestScenes.Contains (this.m_SelectedSceneInAll) && !this.m_OtherScenesToBuild.Contains (this.m_SelectedSceneInAll)) {
                            this.m_IntegrationTestScenes.Add(this.m_SelectedSceneInAll);
                        }
                    }
            
                    if (GUILayout.Button("Add to Build")) {
                        if (!this.m_IntegrationTestScenes.Contains (this.m_SelectedSceneInAll) && !this.m_OtherScenesToBuild.Contains (this.m_SelectedSceneInAll)) {
                            this.m_OtherScenesToBuild.Add(this.m_SelectedSceneInAll);
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal ();

                    this.DrawVerticalSceneList (ref this.m_AllScenesInProject, ref this.m_SelectedSceneInAll, ref this.m_ScrollPositionAllScenes);
                    EditorGUILayout.EndVertical ();
                    
            // ButtoNetworkResultsReceiverns to edit scenes in lists
                  

                EditorGUILayout.EndHorizontal ();
                
                GUILayout.Space(3);
                
                // Select target platform
                this.m_BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build tests for", this.m_BuildTarget);

                if (PlatformRunner.defaultBuildTarget != this.m_BuildTarget)
                {
                    if (GUILayout.Button("Make default target platform"))
                    {
                    PlatformRunner.defaultBuildTarget = this.m_BuildTarget;
                    }
                }
                GUI.enabled = true;
            
                // Select various Network settings
                this.DrawSetting();
                var build = GUILayout.Button("Build and run tests");
            EditorGUILayout.EndVertical();

            if (build) 
            {
                this.BuildAndRun ();
            }
        }

        private void DrawVerticalSceneList(ref List<string> sourceList, ref string selectString, ref Vector2 scrollPosition)
        {
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, Styles.testList);
            EditorGUI.indentLevel++;
            foreach (var scenePath in sourceList)
            {
                var path = Path.GetFileNameWithoutExtension(scenePath);
                var guiContent = new GUIContent(path, scenePath);
                var rect = GUILayoutUtility.GetRect(guiContent, EditorStyles.label);
                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
                    {
                        selectString = scenePath;
                        Event.current.Use();
                    }
                }
                var style = new GUIStyle(EditorStyles.label);
 
                if (selectString == scenePath)
                    style.normal.textColor = new Color(0.3f, 0.5f, 0.85f);
                EditorGUI.LabelField(rect, guiContent, style);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();
        }

        public static List<string> GetScenesWithTestComponents(List<string> allScenes)
        {
            List<Object> results = EditorReferencesUtil.FindScenesWhichContainAsset("TestComponent.cs");    
            List<string> integrationTestScenes = new List<string>();
            
            foreach (Object obj in results) {
                string result = allScenes.FirstOrDefault(s => s.Contains(obj.name));
                if (!string.IsNullOrEmpty(result))
                    integrationTestScenes.Add(result);
            }
            
            return integrationTestScenes;
        }

        private void DrawSetting()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            this.m_Settings.resultsPath = EditorGUILayout.TextField(this.m_Label, this.m_Settings.resultsPath);
            if (GUILayout.Button("Search", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                var selectedPath = EditorUtility.SaveFolderPanel("Result files destination", this.m_Settings.resultsPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                    this.m_Settings.resultsPath = Path.GetFullPath(selectedPath);
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(this.m_Settings.resultsPath))
            {
                Uri uri;
                if (!Uri.TryCreate(this.m_Settings.resultsPath, UriKind.Absolute, out uri) || !uri.IsFile || uri.IsWellFormedOriginalString())
                {
                    EditorGUILayout.HelpBox("Invalid URI path", MessageType.Warning);
                }
            }

            this.m_Settings.sendResultsOverNetwork = EditorGUILayout.Toggle("Send results to editor", this.m_Settings.sendResultsOverNetwork);
            EditorGUI.BeginDisabledGroup(!this.m_Settings.sendResultsOverNetwork);
            this.m_AdvancedNetworkingSettings = EditorGUILayout.Foldout(this.m_AdvancedNetworkingSettings, "Advanced network settings");
            if (this.m_AdvancedNetworkingSettings)
            {
                this.m_SelectedInterface = EditorGUILayout.Popup("Network interface", this.m_SelectedInterface, this.m_Interfaces.ToArray());
                EditorGUI.BeginChangeCheck();
                this.m_Settings.port = EditorGUILayout.IntField("Network port", this.m_Settings.port);
                if (EditorGUI.EndChangeCheck())
                {
                    if (this.m_Settings.port > IPEndPoint.MaxPort)
                        this.m_Settings.port = IPEndPoint.MaxPort;
                    else if (this.m_Settings.port < IPEndPoint.MinPort)
                        this.m_Settings.port = IPEndPoint.MinPort;
                }
            }

            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                this.m_Settings.Save();
            }
        }

        private void BuildAndRun()
        {
            this.SaveToPreferences ();

            var config = new PlatformRunnerConfiguration
            {
                buildTarget = this.m_BuildTarget,
                buildScenes = this.m_OtherScenesToBuild,
                testScenes = this.m_IntegrationTestScenes,
                projectName = this.m_IntegrationTestScenes.Count > 1 ? "IntegrationTests" : Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path),
                resultsDir = this.m_Settings.resultsPath,
                sendResultsOverNetwork = this.m_Settings.sendResultsOverNetwork,
                ipList = this.m_Interfaces.Skip(1).ToList(),
                port = this.m_Settings.port
            };
            
            if (this.m_SelectedInterface > 0)
            config.ipList = new List<string> {this.m_Interfaces.ElementAt(this.m_SelectedInterface)};
            
            PlatformRunner.BuildAndRunInPlayer(config);
            this.Close ();
        }

        public void OnLostFocus() {
            this.SaveToPreferences ();
        }

        public void OnDestroy() {
            this.SaveToPreferences ();
        }

        private void SaveToPreferences()
        {
            EditorPrefs.SetString (Animator.StringToHash (Application.dataPath + "uttTestScenes").ToString (), String.Join (",",this.m_IntegrationTestScenes.ToArray()));
            EditorPrefs.SetString (Animator.StringToHash (Application.dataPath + "uttBuildScenes").ToString (), String.Join (",",this.m_OtherScenesToBuild.ToArray()));
        }
        
        private void LoadFromPrefereneces()
        {
            string storedTestScenes = EditorPrefs.GetString (Animator.StringToHash (Application.dataPath + "uttTestScenes").ToString ());
            string storedBuildScenes = EditorPrefs.GetString (Animator.StringToHash (Application.dataPath + "uttBuildScenes").ToString ());
            
            List<string> parsedTestScenes = storedTestScenes.Split (',').ToList ();
            List<string> parsedBuildScenes = storedBuildScenes.Split (',').ToList ();
            
            // Sanity check scenes actually exist
            foreach (string str in parsedTestScenes) {
                if (this.m_AllScenesInProject.Contains(str))
                    this.m_IntegrationTestScenes.Add(str);
            }
            
            foreach (string str in parsedBuildScenes) {
                if (this.m_AllScenesInProject.Contains(str))
                    this.m_OtherScenesToBuild.Add(str);
            }
        }
    }
}
