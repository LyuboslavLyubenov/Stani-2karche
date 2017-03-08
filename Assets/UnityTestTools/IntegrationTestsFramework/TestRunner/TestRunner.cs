// #define IMITATE_BATCH_MODE //uncomment if you want to imitate batch mode behaviour in non-batch mode mode run

namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Assets.UnityTestTools.Assertions;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class TestRunner : MonoBehaviour
    {
        static private int TestSceneNumber = 0;
        static private readonly TestResultRenderer k_ResultRenderer = new TestResultRenderer();

        public TestComponent currentTest;
        private List<TestResult> m_ResultList = new List<TestResult>();
        private List<TestComponent> m_TestComponents;

        public bool isInitializedByRunner
        {
            get
            {
#if !IMITATE_BATCH_MODE
                if (Application.isEditor && !IsBatchMode())
                    return true;
#endif
                return false;
            }
        }

        private double m_StartTime;
        private bool m_ReadyToRun;

        private string m_TestMessages;
        private string m_Stacktrace;
        private TestState m_TestState = TestState.Running;

        private TestRunnerConfigurator m_Configurator;

        public TestRunnerCallbackList TestRunnerCallback = new TestRunnerCallbackList();
        private IntegrationTestsProvider m_TestsProvider;

        private const string k_Prefix = "IntegrationTest";
        private const string k_StartedMessage = k_Prefix + " Started";
        private const string k_FinishedMessage = k_Prefix + " Finished";
        private const string k_TimeoutMessage = k_Prefix + " Timeout";
        private const string k_FailedMessage = k_Prefix + " Failed";
        private const string k_FailedExceptionMessage = k_Prefix + " Failed with exception";
        private const string k_IgnoredMessage = k_Prefix + " Ignored";
        private const string k_InterruptedMessage = k_Prefix + " Run interrupted";


        public void Awake()
        {
            this.m_Configurator = new TestRunnerConfigurator();
            if (this.isInitializedByRunner) return;
            TestComponent.DisableAllTests();
        }

        public void Start()
        {
            if (this.isInitializedByRunner) return;

            if (this.m_Configurator.sendResultsOverNetwork)
            {
                var nrs = this.m_Configurator.ResolveNetworkConnection();
                if (nrs != null)
                    this.TestRunnerCallback.Add(nrs);
            }

            TestComponent.DestroyAllDynamicTests();
            var dynamicTestTypes = TestComponent.GetTypesWithHelpAttribute(SceneManager.GetActiveScene().name);
            foreach (var dynamicTestType in dynamicTestTypes)
                TestComponent.CreateDynamicTest(dynamicTestType);

            var tests = TestComponent.FindAllTestsOnScene();

            this.InitRunner(tests, dynamicTestTypes.Select(type => type.AssemblyQualifiedName).ToList());
        }

        public void InitRunner(List<TestComponent> tests, List<string> dynamicTestsToRun)
        {
            Application.logMessageReceived += this.LogHandler;

            // Init dynamic tests
            foreach (var typeName in dynamicTestsToRun)
            {
                var t = Type.GetType(typeName);
                if (t == null) continue;
                var scriptComponents = Resources.FindObjectsOfTypeAll(t) as MonoBehaviour[];
                if (scriptComponents.Length == 0)
                {
                    Debug.LogWarning(t + " not found. Skipping.");
                    continue;
                }
                if (scriptComponents.Length > 1) Debug.LogWarning("Multiple GameObjects refer to " + typeName);
                tests.Add(scriptComponents.First().GetComponent<TestComponent>());
            }
            // create test structure
            this.m_TestComponents = ParseListForGroups(tests).ToList();
            // create results for tests
            this.m_ResultList = this.m_TestComponents.Select(component => new TestResult(component)).ToList();
            // init test provider
            this.m_TestsProvider = new IntegrationTestsProvider(this.m_ResultList.Select(result => result.TestComponent as ITestComponent));
            this.m_ReadyToRun = true;
        }

        private static IEnumerable<TestComponent> ParseListForGroups(IEnumerable<TestComponent> tests)
        {
            var results = new HashSet<TestComponent>();
            foreach (var testResult in tests)
            {
                if (testResult.IsTestGroup())
                {
                    var childrenTestResult = testResult.gameObject.GetComponentsInChildren(typeof(TestComponent), true)
                                             .Where(t => t != testResult)
                                             .Cast<TestComponent>()
                                             .ToArray();
                    foreach (var result in childrenTestResult)
                    {
                        if (!result.IsTestGroup())
                            results.Add(result);
                    }
                    continue;
                }
                results.Add(testResult);
            }
            return results;
        }

        public void Update()
        {
            if (this.m_ReadyToRun  && Time.frameCount > 1)
            {
                this.m_ReadyToRun = false;
                this.StartCoroutine("StateMachine");
            }
        }

        public void OnDestroy()
        {
            if (this.currentTest != null)
            {
                var testResult = this.m_ResultList.Single(result => result.TestComponent == this.currentTest);
                testResult.messages += "Test run interrupted (crash?)";
                this.LogMessage(k_InterruptedMessage);
                this.FinishTest(TestResult.ResultType.Failed);
            }
            if (this.currentTest != null || (this.m_TestsProvider != null && this.m_TestsProvider.AnyTestsLeft()))
            {
                var remainingTests = this.m_TestsProvider.GetRemainingTests();
                this.TestRunnerCallback.TestRunInterrupted(remainingTests.ToList());
            }
            Application.logMessageReceived -= this.LogHandler;
        }

        private void LogHandler(string condition, string stacktrace, LogType type)
        {
            if (!condition.StartsWith(k_StartedMessage) && !condition.StartsWith(k_FinishedMessage))
            {
                var msg = condition;
                if (msg.StartsWith(k_Prefix)) msg = msg.Substring(k_Prefix.Length + 1);
                if (this.currentTest != null && msg.EndsWith("(" + this.currentTest.name + ')')) msg = msg.Substring(0, msg.LastIndexOf('('));
                this.m_TestMessages += msg + "\n";
            }
            switch (type)
            {
                case LogType.Exception:
                {
                    var exceptionType = condition.Substring(0, condition.IndexOf(':'));
                    if (this.currentTest != null && this.currentTest.IsExceptionExpected(exceptionType))
                    {
                        this.m_TestMessages += exceptionType + " was expected\n";
                        if (this.currentTest.ShouldSucceedOnException())
                        {
                            this.m_TestState = TestState.Success;
                        }
                    }
                    else
                    {
                        this.m_TestState = TestState.Exception;
                        this.m_Stacktrace = stacktrace;
                    }
                }
                    break;
                case LogType.Assert:
                case LogType.Error:
                    this.m_TestState = TestState.Failure;
                    this.m_Stacktrace = stacktrace;
                    break;
                case LogType.Log:
                    if (this.m_TestState ==  TestState.Running && condition.StartsWith(IntegrationTest.passMessage))
                    {
                        this.m_TestState = TestState.Success;
                    }
                    if (condition.StartsWith(IntegrationTest.failMessage))
                    {
                        this.m_TestState = TestState.Failure;
                    }
                    break;
            }
        }

        public IEnumerator StateMachine()
        {
            this.TestRunnerCallback.RunStarted(Application.platform.ToString(), this.m_TestComponents);
            while (true)
            {
                if (!this.m_TestsProvider.AnyTestsLeft() && this.currentTest == null)
                {
                    this.FinishTestRun();
                    yield break;
                }
                if (this.currentTest == null)
                {
                    this.StartNewTest();
                }
                if (this.currentTest != null)
                {
                    if (this.m_TestState == TestState.Running)
                    {
                        if(this.currentTest.ShouldSucceedOnAssertions())
                        {
                            var assertionsToCheck = this.currentTest.gameObject.GetComponentsInChildren<AssertionComponent>().Where(a => a.enabled).ToArray();
                            if (assertionsToCheck.Any () && assertionsToCheck.All(a => a.checksPerformed > 0))
                            {
                                IntegrationTest.Pass(this.currentTest.gameObject);
                                this.m_TestState = TestState.Success;
                            }
                        }
                        if (this.currentTest != null && Time.time > this.m_StartTime + this.currentTest.GetTimeout())
                        {
                            this.m_TestState = TestState.Timeout;
                        }
                    }

                    switch (this.m_TestState)
                    {
                        case TestState.Success:
                            this.LogMessage(k_FinishedMessage);
                            this.FinishTest(TestResult.ResultType.Success);
                            break;
                        case TestState.Failure:
                            this.LogMessage(k_FailedMessage);
                            this.FinishTest(TestResult.ResultType.Failed);
                            break;
                        case TestState.Exception:
                            this.LogMessage(k_FailedExceptionMessage);
                            this.FinishTest(TestResult.ResultType.FailedException);
                            break;
                        case TestState.Timeout:
                            this.LogMessage(k_TimeoutMessage);
                            this.FinishTest(TestResult.ResultType.Timeout);
                            break;
                        case TestState.Ignored:
                            this.LogMessage(k_IgnoredMessage);
                            this.FinishTest(TestResult.ResultType.Ignored);
                            break;
                    }
                }
                yield return null;
            }
        }

        private void LogMessage(string message)
        {
            if (this.currentTest != null)
                Debug.Log(message + " (" + this.currentTest.Name + ")", this.currentTest.gameObject);
            else
                Debug.Log(message);
        }

        private void FinishTestRun()
        {
            this.PrintResultToLog();
            this.TestRunnerCallback.RunFinished(this.m_ResultList);
            this.LoadNextLevelOrQuit();
        }

        private void PrintResultToLog()
        {
            var resultString = "";
            resultString += "Passed: " + this.m_ResultList.Count(t => t.IsSuccess);
            if (this.m_ResultList.Any(result => result.IsFailure))
            {
                resultString += " Failed: " + this.m_ResultList.Count(t => t.IsFailure);
                Debug.Log("Failed tests: " + string.Join(", ", this.m_ResultList.Where(t => t.IsFailure).Select(result => result.Name).ToArray()));
            }
            if (this.m_ResultList.Any(result => result.IsIgnored))
            {
                resultString += " Ignored: " + this.m_ResultList.Count(t => t.IsIgnored);
                Debug.Log("Ignored tests: " + string.Join(", ",
                                                          this.m_ResultList.Where(t => t.IsIgnored).Select(result => result.Name).ToArray()));
            }
            Debug.Log(resultString);
        }

        private void LoadNextLevelOrQuit()
        {
            if (this.isInitializedByRunner) return;


            TestSceneNumber += 1;
            string testScene = this.m_Configurator.GetIntegrationTestScenes(TestSceneNumber);

            if (testScene != null)
                SceneManager.LoadScene(Path.GetFileNameWithoutExtension(testScene));
            else
            {
                this.TestRunnerCallback.AllScenesFinished();
                k_ResultRenderer.ShowResults();

#if UNITY_EDITOR
                var prevScenes = this.m_Configurator.GetPreviousScenesToRestore();
                if(prevScenes!=null)
                {
                    UnityEditor.EditorBuildSettings.scenes = prevScenes;
                }
#endif

                if (this.m_Configurator.isBatchRun && this.m_Configurator.sendResultsOverNetwork)
                    Application.Quit();
            }
        }

        public void OnGUI()
        {
            k_ResultRenderer.Draw();
        }

        private void StartNewTest()
        {
            this.m_TestMessages = "";
            this.m_Stacktrace = "";
            this.m_TestState = TestState.Running;

            this.m_StartTime = Time.time;
            this.currentTest = this.m_TestsProvider.GetNextTest() as TestComponent;

            var testResult = this.m_ResultList.Single(result => result.TestComponent == this.currentTest);

            if (this.currentTest != null && this.currentTest.IsExludedOnThisPlatform())
            {
                this.m_TestState = TestState.Ignored;
                Debug.Log(this.currentTest.gameObject.name + " is excluded on this platform");
            }

            // don't ignore test if user initiated it from the runner and it's the only test that is being run
            if (this.currentTest != null
                && (this.currentTest.IsIgnored()
                    && !(this.isInitializedByRunner && this.m_ResultList.Count == 1)))
                this.m_TestState = TestState.Ignored;

            this.LogMessage(k_StartedMessage);
            this.TestRunnerCallback.TestStarted(testResult);
        }

        private void FinishTest(TestResult.ResultType result)
        {
            this.m_TestsProvider.FinishTest(this.currentTest);
            var testResult = this.m_ResultList.Single(t => t.GameObject == this.currentTest.gameObject);
            testResult.resultType = result;
            testResult.duration = Time.time - this.m_StartTime;
            testResult.messages = this.m_TestMessages;
            testResult.stacktrace = this.m_Stacktrace;
            this.TestRunnerCallback.TestFinished(testResult);
            this.currentTest = null;
            if (!testResult.IsSuccess
                && testResult.Executed
                && !testResult.IsIgnored) k_ResultRenderer.AddResults(SceneManager.GetActiveScene().name, testResult);
        }

        #region Test Runner Helpers

        public static TestRunner GetTestRunner()
        {
            TestRunner testRunnerComponent = null;
            var testRunnerComponents = Resources.FindObjectsOfTypeAll(typeof(TestRunner));

            if (testRunnerComponents.Count() > 1)
                foreach (var t in testRunnerComponents) DestroyImmediate(((TestRunner)t).gameObject);
            else if (!testRunnerComponents.Any())
                testRunnerComponent = Create().GetComponent<TestRunner>();
            else
                testRunnerComponent = testRunnerComponents.Single() as TestRunner;

            return testRunnerComponent;
        }

        private static GameObject Create()
        {
            var runner = new GameObject("TestRunner");
            runner.AddComponent<TestRunner>();
            Debug.Log("Created Test Runner");
            return runner;
        }

        private static bool IsBatchMode()
        {
#if !UNITY_METRO
            const string internalEditorUtilityClassName = "UnityEditorInternal.InternalEditorUtility, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

            var t = Type.GetType(internalEditorUtilityClassName, false);
            if (t == null) return false;

            const string inBatchModeProperty = "inBatchMode";
            var prop = t.GetProperty(inBatchModeProperty);
            return (bool)prop.GetValue(null, null);
#else   // if !UNITY_METRO
            return false;
#endif  // if !UNITY_METRO
        }

        #endregion

        enum TestState
        {
            Running,
            Success,
            Failure,
            Exception,
            Timeout,
            Ignored
        }
    }
}
