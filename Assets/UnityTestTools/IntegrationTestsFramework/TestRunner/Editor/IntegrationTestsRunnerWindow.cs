namespace UnityTestTools.IntegrationTestsFramework.TestRunner.Editor
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using UnityEditor;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using UnityTestTools.Common;
    using UnityTestTools.Common.Editor;
    using UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.Renderer;

    [Serializable]
    public class IntegrationTestsRunnerWindow : EditorWindow, IHasCustomMenu
    {
        #region GUI Contents
        private readonly GUIContent m_GUICreateNewTest = new GUIContent("Create", "Create new test");
        private readonly GUIContent m_GUIRunSelectedTests = new GUIContent("Run Selected", "Run selected test(s)");
        private readonly GUIContent m_GUIRunAllTests = new GUIContent("Run All", "Run all tests");
        private readonly GUIContent m_GUIBlockUI = new GUIContent("Block UI when running", "Block UI when running tests");
        private readonly GUIContent m_GUIPauseOnFailure = new GUIContent("Pause on test failure");
        #endregion

        #region runner steerign vars
        private static IntegrationTestsRunnerWindow s_Instance;
        [SerializeField] private List<GameObject> m_TestsToRun;
        [SerializeField] private List<string> m_DynamicTestsToRun;
        [SerializeField] private bool m_ReadyToRun;
        private bool m_IsBuilding;
        public static bool selectedInHierarchy;
        private float m_HorizontalSplitBarPosition = 200;
        private Vector2 m_TestInfoScroll, m_TestListScroll;
        private IntegrationTestRendererBase[] m_TestLines;
        private string m_CurrectSceneName;
        private TestFilterSettings m_FilterSettings;
        
        Vector2 m_resultTextSize;
        string m_resultText;
        GameObject m_lastSelectedGO;
        int m_resultTestMaxLength = 15000;

        [SerializeField] private GameObject m_SelectedLine;
        [SerializeField] private List<TestResult> m_ResultList = new List<TestResult>();
        [SerializeField] private List<GameObject> m_FoldMarkers = new List<GameObject>();

        private IntegrationTestsRunnerSettings m_Settings;

        #endregion


        static IntegrationTestsRunnerWindow()
        {
            InitBackgroundRunners();
        }

        private static void InitBackgroundRunners()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemDraw;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemDraw;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChangeUpdate;
            EditorApplication.hierarchyWindowChanged += OnHierarchyChangeUpdate;
            EditorApplication.update -= BackgroundSceneChangeWatch;
            EditorApplication.update += BackgroundSceneChangeWatch;
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
            EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
        }

        private static void OnPlaymodeStateChanged()
        {
            if (s_Instance && EditorApplication.isPlaying  == EditorApplication.isPlayingOrWillChangePlaymode)
                s_Instance.RebuildTestList();
        }

        public void OnDestroy()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemDraw;
            EditorApplication.update -= BackgroundSceneChangeWatch;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChangeUpdate;
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;

            TestComponent.DestroyAllDynamicTests();
        }

        private static void BackgroundSceneChangeWatch()
        {
            if (!s_Instance) return;
            var currentScene = SceneManager.GetActiveScene().path;
            if (s_Instance.m_CurrectSceneName != null && s_Instance.m_CurrectSceneName == currentScene) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            TestComponent.DestroyAllDynamicTests();
            s_Instance.m_CurrectSceneName = currentScene;
            s_Instance.m_ResultList.Clear();
            s_Instance.RebuildTestList();
        }

        public void OnEnable()
        {
            this.titleContent = new GUIContent("Integration Tests");
            s_Instance = this;

            this.m_Settings = ProjectSettingsBase.Load<IntegrationTestsRunnerSettings>();
            this.m_FilterSettings = new TestFilterSettings("UnityTest.IntegrationTestsRunnerWindow");

            InitBackgroundRunners();
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !this.m_ReadyToRun) this.RebuildTestList();
        }

        public void OnSelectionChange()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode
                || Selection.objects == null
                || Selection.objects.Length == 0) return;

            if (Selection.gameObjects.Length == 1)
            {
                var go = Selection.gameObjects.Single();
                var temp = go.transform;
                while (temp != null)
                {
                    var tc = temp.GetComponent<TestComponent>();
                    if (tc != null) break;
                    temp = temp.parent;
                }

                if (temp != null)
                {
                    SelectInHierarchy(temp.gameObject);
                    Selection.activeGameObject = temp.gameObject;
                    this.m_SelectedLine = temp.gameObject;
                }
            }
        }

        public static void OnHierarchyChangeUpdate()
        {
            if (!s_Instance || s_Instance.m_TestLines == null || EditorApplication.isPlayingOrWillChangePlaymode) return;

            // create a test runner if it doesn't exist
            TestRunner.GetTestRunner();

            // make tests are not places under a go that is not a test itself
            foreach (var test in TestComponent.FindAllTestsOnScene())
            {
                if (test.gameObject.transform.parent != null && test.gameObject.transform.parent.gameObject.GetComponent<TestComponent>() == null)
                {
                    test.gameObject.transform.parent = null;
                    Debug.LogWarning("Tests need to be on top of the hierarchy or directly under another test.");
                }
            }
            if (selectedInHierarchy) selectedInHierarchy = false;
            else s_Instance.RebuildTestList();
        }
        
        public static TestResult GetResultForTest(TestComponent tc)
        {
            if(!s_Instance) return new TestResult(tc);
            return s_Instance.m_ResultList.FirstOrDefault(r => r.GameObject == tc.gameObject);
        }

        public static void OnHierarchyWindowItemDraw(int id, Rect rect)
        {
            var o = EditorUtility.InstanceIDToObject(id);
            if (o is GameObject)
            {
                var go = o as GameObject;

                if (Event.current.type == EventType.MouseDown
                    && Event.current.button == 0
                    && rect.Contains(Event.current.mousePosition))
                {
                    var temp = go.transform;
                    while (temp != null)
                    {
                        var c = temp.GetComponent<TestComponent>();
                        if (c != null) break;
                        temp = temp.parent;
                    }
                    if (temp != null) SelectInHierarchy(temp.gameObject);
                }
            }
        }

        private static void SelectInHierarchy(GameObject gameObject)
        {
            if (!s_Instance) return;
            if (gameObject == s_Instance.m_SelectedLine && gameObject.activeInHierarchy) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!gameObject.activeSelf)
            {
                selectedInHierarchy = true;
                gameObject.SetActive(true);
            }

            var tests = TestComponent.FindAllTestsOnScene();
            var skipList = gameObject.GetComponentsInChildren(typeof(TestComponent), true).ToList();
            tests.RemoveAll(skipList.Contains);
            foreach (var test in tests)
            {
                var enable = test.GetComponentsInChildren(typeof(TestComponent), true).Any(c => c.gameObject == gameObject);
                if (test.gameObject.activeSelf != enable) test.gameObject.SetActive(enable);
            }
        }

        private void RunTests(IList<ITestComponent> tests)
        {
            if (!tests.Any() || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            FocusWindowIfItsOpen(this.GetType());

            var testComponents = tests.Where(t => t is TestComponent).Cast<TestComponent>().ToList();
            var dynaminTests = testComponents.Where(t => t.dynamic).ToList();
            this.m_DynamicTestsToRun = dynaminTests.Select(c => c.dynamicTypeName).ToList();
            testComponents.RemoveAll(dynaminTests.Contains);

            this.m_TestsToRun = testComponents.Select( tc => tc.gameObject ).ToList();

            this.m_ReadyToRun = true;
            TestComponent.DisableAllTests();

            EditorApplication.isPlaying = true;
        }

        public void Update()
        {
            if (this.m_ReadyToRun && EditorApplication.isPlaying)
            {
                this.m_ReadyToRun = false;
                var testRunner = TestRunner.GetTestRunner();
                testRunner.TestRunnerCallback.Add(new RunnerCallback(this));
                var testComponents = this.m_TestsToRun.Select(go => go.GetComponent<TestComponent>()).ToList();
                testRunner.InitRunner(testComponents, this.m_DynamicTestsToRun);
            }
        }
        
        private void RebuildTestList()
        {
            this.m_TestLines = null;
            if (!TestComponent.AnyTestsOnScene() 
                && !TestComponent.AnyDynamicTestForCurrentScene()) return;

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var dynamicTestsOnScene = TestComponent.FindAllDynamicTestsOnScene();
                var dynamicTestTypes = TestComponent.GetTypesWithHelpAttribute(SceneManager.GetActiveScene().path);

                foreach (var dynamicTestType in dynamicTestTypes)
                {
                    var existingTests = dynamicTestsOnScene.Where(component => component.dynamicTypeName == dynamicTestType.AssemblyQualifiedName);
                    if (existingTests.Any())
                    {
                        var testComponent = existingTests.Single();
                        foreach (var c in testComponent.gameObject.GetComponents<Component>())
                        {
                            var type = Type.GetType(testComponent.dynamicTypeName);
                            if (c is TestComponent || c is Transform || type.IsInstanceOfType(c)) continue;
                            DestroyImmediate(c);
                        }
                        dynamicTestsOnScene.Remove(existingTests.Single());
                        continue;
                    }
                    TestComponent.CreateDynamicTest(dynamicTestType);
                }

                foreach (var testComponent in dynamicTestsOnScene)
                    DestroyImmediate(testComponent.gameObject);
            }

            var topTestList = TestComponent.FindAllTopTestsOnScene();

            var newResultList = new List<TestResult>();
            this.m_TestLines = this.ParseTestList(topTestList, newResultList);

            var oldDynamicResults = this.m_ResultList.Where(result => result.dynamicTest);
            foreach (var oldResult in this.m_ResultList)
            {
                var result = newResultList.Find(r => r.Id == oldResult.Id);
                if (result == null) continue;
                result.Update(oldResult);
            }
            newResultList.AddRange(oldDynamicResults.Where(r => !newResultList.Contains(r)));
            this.m_ResultList = newResultList;

            IntegrationTestRendererBase.RunTest = this.RunTests;
            IntegrationTestGroupLine.FoldMarkers = this.m_FoldMarkers;
            IntegrationTestLine.Results = this.m_ResultList;
            
            this.m_FilterSettings.UpdateCounters(this.m_ResultList.Cast<ITestResult>());

            this.m_FoldMarkers.RemoveAll(o => o == null);

            selectedInHierarchy = true;
            this.Repaint();
        }


        private IntegrationTestRendererBase[] ParseTestList(List<TestComponent> testList, List<TestResult> results)
        {
            var tempList = new List<IntegrationTestRendererBase>();
            foreach (var testObject in testList)
            {
                if (!testObject.IsTestGroup())
                {
                    var result = new TestResult(testObject);
                    if (results != null)
                        results.Add(result);
                    tempList.Add(new IntegrationTestLine(testObject.gameObject, result));
                    continue;
                }
                var group = new IntegrationTestGroupLine(testObject.gameObject);
                var children = testObject.gameObject.GetComponentsInChildren(typeof(TestComponent), true).Cast<TestComponent>().ToList();
                children = children.Where(c => c.gameObject.transform.parent == testObject.gameObject.transform).ToList();
                group.AddChildren(this.ParseTestList(children, results));
                tempList.Add(group);
            }
            tempList.Sort();
            return tempList.ToArray();
        }

        public void OnGUI()
        {
            if (BuildPipeline.isBuildingPlayer)
            {
                this.m_IsBuilding = true;
            }
            else if (this.m_IsBuilding)
            {
                this.m_IsBuilding = false;
                this.Repaint();
            }

            this.PrintHeadPanel();

            EditorGUILayout.BeginVertical(Styles.testList);
            this.m_TestListScroll = EditorGUILayout.BeginScrollView(this.m_TestListScroll);
            bool repaint = this.PrintTestList(this.m_TestLines);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            this.RenderDetails();

            if (repaint) this.Repaint();
        }

        public void PrintHeadPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (GUILayout.Button(this.m_GUIRunAllTests, EditorStyles.toolbarButton))
            {
                this.RunTests(TestComponent.FindAllTestsOnScene().Cast<ITestComponent>().ToList());
            }
            EditorGUI.BeginDisabledGroup(!Selection.gameObjects.Any (t => t.GetComponent(typeof(ITestComponent))));
            if (GUILayout.Button(this.m_GUIRunSelectedTests, EditorStyles.toolbarButton))
            {
                this.RunTests(Selection.gameObjects.Select(t => t.GetComponent(typeof(TestComponent))).Cast<ITestComponent>().ToList());
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(this.m_GUICreateNewTest, EditorStyles.toolbarButton))
            {
                var test = TestComponent.CreateTest();
                if (Selection.gameObjects.Length == 1
                    && Selection.activeGameObject != null
                    && Selection.activeGameObject.GetComponent<TestComponent>())
                {
                    test.transform.parent = Selection.activeGameObject.transform.parent;
                }
                Selection.activeGameObject = test;
                this.RebuildTestList();
            }
            EditorGUI.EndDisabledGroup();
            
            GUILayout.FlexibleSpace ();
            
            this.m_FilterSettings.OnGUI ();
            
            EditorGUILayout.EndHorizontal ();
        }
        
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(this.m_GUIBlockUI, this.m_Settings.blockUIWhenRunning, this.m_Settings.ToggleBlockUIWhenRunning);
            menu.AddItem(this.m_GUIPauseOnFailure, this.m_Settings.pauseOnTestFailure, this.m_Settings.TogglePauseOnTestFailure);
        }
        
        private bool PrintTestList(IntegrationTestRendererBase[] renderedLines)
        {
            if (renderedLines == null) return false;

            var filter = this.m_FilterSettings.BuildRenderingOptions();

            bool repaint = false;
            foreach (var renderedLine in renderedLines)
            {
                repaint |= renderedLine.Render(filter);
            }
            return repaint;
        }

        private void RenderDetails()
        {
            var ctrlId = GUIUtility.GetControlID(FocusType.Passive);

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.y = rect.height + rect.y - 1;
            rect.height = 3;

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            var e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (GUIUtility.hotControl == 0 && rect.Contains(e.mousePosition))
                        GUIUtility.hotControl = ctrlId;
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == ctrlId)
                    {
                        this.m_HorizontalSplitBarPosition -= e.delta.y;
                        if (this.m_HorizontalSplitBarPosition < 20) this.m_HorizontalSplitBarPosition = 20;
                        this.Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == ctrlId)
                        GUIUtility.hotControl = 0;
                    break;
            }

            this.m_TestInfoScroll = EditorGUILayout.BeginScrollView(this.m_TestInfoScroll, GUILayout.MinHeight(this.m_HorizontalSplitBarPosition));

            if (this.m_SelectedLine != null)
                this.UpdateResultText(this.m_SelectedLine);

            EditorGUILayout.SelectableLabel(this.m_resultText, Styles.info, 
                                            GUILayout.ExpandHeight(true), 
                                            GUILayout.ExpandWidth(true), 
                                            GUILayout.MinWidth(this.m_resultTextSize.x), 
                                            GUILayout.MinHeight(this.m_resultTextSize.y));
            EditorGUILayout.EndScrollView();
        }

        private void UpdateResultText(GameObject go)
        {
            if(go == this.m_lastSelectedGO) return;
            this.m_lastSelectedGO = go;
            var result = this.m_ResultList.Find(r => r.GameObject == go);
            if (result == null)
            {
                this.m_resultText = string.Empty;
                this.m_resultTextSize = Styles.info.CalcSize(new GUIContent(string.Empty));
                return;
            }
            var sb = new StringBuilder(result.Name.Trim());
            if (!string.IsNullOrEmpty(result.messages))
            {
                sb.Append("\n---\n");
                sb.Append(result.messages.Trim());
            }
            if (!string.IsNullOrEmpty(result.stacktrace))
            {
                sb.Append("\n---\n");
                sb.Append(result.stacktrace.Trim());
            }
            if(sb.Length>this.m_resultTestMaxLength)
            {
                sb.Length = this.m_resultTestMaxLength;
                sb.AppendFormat("...\n\n---MESSAGE TRUNCATED AT {0} CHARACTERS---", this.m_resultTestMaxLength);
            }
            this.m_resultText = sb.ToString().Trim();
            this.m_resultTextSize = Styles.info.CalcSize(new GUIContent(this.m_resultText));
        }

        public void OnInspectorUpdate()
        {
            if (focusedWindow != this) this.Repaint();
        }

        private void SetCurrentTest(TestComponent tc)
        {
            foreach (var line in this.m_TestLines)
                line.SetCurrentTest(tc);
        }

        class RunnerCallback : ITestRunnerCallback
        {
            private readonly IntegrationTestsRunnerWindow m_Window;
            private int m_TestNumber;
            private int m_CurrentTestNumber;

            private readonly bool m_ConsoleErrorOnPauseValue;
            private readonly bool m_RunInBackground;
            private TestComponent m_CurrentTest;

            public RunnerCallback(IntegrationTestsRunnerWindow window)
            {
                this.m_Window = window;

                this.m_ConsoleErrorOnPauseValue = GuiHelper.GetConsoleErrorPause();
                GuiHelper.SetConsoleErrorPause(false);
                this.m_RunInBackground = PlayerSettings.runInBackground;
                PlayerSettings.runInBackground = true;
            }

            public void RunStarted(string platform, List<TestComponent> testsToRun)
            {
                EditorApplication.update += this.OnEditorUpdate;
                this.m_TestNumber = testsToRun.Count;
                foreach (var test in testsToRun)
                {
                    var result = this.m_Window.m_ResultList.Find(r => r.TestComponent == test);
                    if (result != null) result.Reset();
                }
            }

            public void RunFinished(List<TestResult> testResults)
            {
                this.m_Window.SetCurrentTest(null);
                this.m_CurrentTest = null;
                EditorApplication.update -= this.OnEditorUpdate;
                EditorApplication.isPlaying = false;
                EditorUtility.ClearProgressBar();
                GuiHelper.SetConsoleErrorPause(this.m_ConsoleErrorOnPauseValue);
                PlayerSettings.runInBackground = this.m_RunInBackground;
            }

            public void AllScenesFinished()
            {

            }

            public void TestStarted(TestResult test)
            {
                this.m_Window.SetCurrentTest(test.TestComponent);
                this.m_CurrentTest = test.TestComponent;
            }


            public void TestFinished(TestResult test)
            {
                this.m_CurrentTestNumber++;

                var result = this.m_Window.m_ResultList.Find(r => r.Id == test.Id);
                if (result != null)
                    result.Update(test);
                else
                    this.m_Window.m_ResultList.Add(test);
                    
                if(test.IsFailure && this.m_Window.m_Settings.pauseOnTestFailure)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.isPaused = true;
                }
            }

            public void TestRunInterrupted(List<ITestComponent> testsNotRun)
            {
                Debug.Log("Test run interrupted");
                this.RunFinished(new List<TestResult>());
            }

            private void OnEditorUpdate()
            {
                if(!EditorApplication.isPlaying) 
                {
                    this.TestRunInterrupted(null);
                    return;
                }

                if (this.m_Window.m_Settings.blockUIWhenRunning 
                    && this.m_CurrentTest != null 
                    && !EditorApplication.isPaused 
                    && EditorUtility.DisplayCancelableProgressBar("Integration Test Runner",
                                                                  "Running " + this.m_CurrentTest.Name,
                                                                  (float)this.m_CurrentTestNumber / this.m_TestNumber))
                {
                    this.TestRunInterrupted(null);
                }
            }
        }

        [MenuItem("Unity Test Tools/Integration Test Runner %#&t")]
        public static IntegrationTestsRunnerWindow ShowWindow()
        {
            var w = GetWindow(typeof(IntegrationTestsRunnerWindow));
            w.Show();
            return w as IntegrationTestsRunnerWindow;
        }
    }
}
