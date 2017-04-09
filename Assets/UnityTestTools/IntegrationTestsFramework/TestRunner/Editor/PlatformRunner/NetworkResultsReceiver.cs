namespace UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.PlatformRunner
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    using UnityEditor;

    using UnityEditorInternal;

    using UnityEngine;

    using UnityTestTools.Common;
    using UnityTestTools.Common.Editor.ResultWriter;
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    [Serializable]
    public class NetworkResultsReceiver : EditorWindow
    {
        public static NetworkResultsReceiver Instance;

        private string m_StatusLabel;
        private TcpListener m_Listener;

        [SerializeField]
        private PlatformRunnerConfiguration m_Configuration;

        private List<ITestResult> m_TestResults = new List<ITestResult>();

        #region steering variables
        private bool m_RunFinished;
        private bool m_Repaint;

        private TimeSpan m_TestTimeout = TimeSpan.Zero;
        private DateTime m_LastMessageReceived;
        private bool m_Running;

        public TimeSpan ReceiveMessageTimeout = TimeSpan.FromSeconds(30);
        private readonly TimeSpan m_InitialConnectionTimeout = TimeSpan.FromSeconds(300);
        private bool m_TestFailed;
        #endregion

        private void AcceptCallback(TcpClient client)
        {
            this.m_Repaint = true;
            ResultDTO dto;
            try
            {
                this.m_LastMessageReceived = DateTime.Now;
                using (var stream = client.GetStream())
                {
                    var bf = new DTOFormatter();
                    dto = (ResultDTO)bf.Deserialize(stream);
                    stream.Close();
                }
                client.Close();
            }
            catch (ObjectDisposedException e)
            {
                Debug.LogException(e);
                this.m_StatusLabel = "Got disconnected";
                return;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            switch (dto.messageType)
            {
                case ResultDTO.MessageType.TestStarted:
                    this.m_StatusLabel = dto.testName;
                    this.m_TestTimeout = TimeSpan.FromSeconds(dto.testTimeout);
                    break;
                case ResultDTO.MessageType.TestFinished:
                    this.m_TestResults.Add(dto.testResult);
                    this.m_TestTimeout = TimeSpan.Zero;
                    if (dto.testResult.Executed && dto.testResult.ResultState != TestResultState.Ignored && !dto.testResult.IsSuccess)
                        this.m_TestFailed = true;
                    break;
                case ResultDTO.MessageType.RunStarted:
                    this.m_TestResults = new List<ITestResult>();
                    this.m_StatusLabel = "Run started: " + dto.loadedLevelName;
                    break;
                case ResultDTO.MessageType.RunFinished:
                    this.WriteResultsToLog(dto, this.m_TestResults);
                    if (!string.IsNullOrEmpty(this.m_Configuration.resultsDir))
                    {
                        var platform = this.m_Configuration.runInEditor ? "Editor" : this.m_Configuration.buildTarget.ToString();
                        var resultWriter = new XmlResultWriter(dto.loadedLevelName, platform, this.m_TestResults.ToArray());
                        try
                        {
                            if (!Directory.Exists(this.m_Configuration.resultsDir))
                            {
                                Directory.CreateDirectory(this.m_Configuration.resultsDir);
                            }
                            var filePath = Path.Combine(this.m_Configuration.resultsDir, dto.loadedLevelName + ".xml");
                            File.WriteAllText(filePath, resultWriter.GetTestResult());
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    break;
            case ResultDTO.MessageType.AllScenesFinished:
                this.m_Running = false;
                this.m_RunFinished = true;
                break;
            case ResultDTO.MessageType.Ping:
                    break;
            }
        }

        private void WriteResultsToLog(ResultDTO dto, List<ITestResult> list)
        {
            string result = "Run finished for: " + dto.loadedLevelName;
            var failCount = list.Count(t => t.Executed && !t.IsSuccess);
            if (failCount == 0)
                result += "\nAll tests passed";
            else
                result += "\n" + failCount + " tests failed";

            if (failCount == 0)
                Debug.Log(result);
            else
                Debug.LogWarning(result);
        }

        public void Update()
        {
            if (EditorApplication.isCompiling
                && this.m_Listener != null)
            {
                this.m_Running = false;
                this.m_Listener.Stop();
                return;
            }

            if (this.m_Running)
            {
                try
                {
                    if (this.m_Listener != null && this.m_Listener.Pending())
                    {
                        using (var client = this.m_Listener.AcceptTcpClient())
                        {
                            this.AcceptCallback(client);
                            client.Close();
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    this.m_StatusLabel = "Exception happened: " + e.Message;
                    this.Repaint();
                    Debug.LogException(e);
                }
            }

            if (this.m_Running)
            {
                var adjustedtestTimeout =  this.m_TestTimeout.Add(this.m_TestTimeout);
                var timeout = this.ReceiveMessageTimeout > adjustedtestTimeout ? this.ReceiveMessageTimeout : adjustedtestTimeout;
                if ((DateTime.Now - this.m_LastMessageReceived) > timeout)
                {
                    Debug.LogError("Timeout when waiting for test results");
                    this.m_RunFinished = true;
                }
            }
            if (this.m_RunFinished)
            {
                this.Close();
                if (InternalEditorUtility.inBatchMode)
                    EditorApplication.Exit(this.m_TestFailed ? Batch.returnCodeTestsFailed : Batch.returnCodeTestsOk);
            }
            if (this.m_Repaint) this.Repaint();
        }

        public void OnEnable()
        {
            this.minSize = new Vector2(300, 100);
            this.titleContent = new GUIContent("Test run monitor");
            Instance = this;
            this.m_StatusLabel = "Initializing...";
            if (EditorApplication.isCompiling) return;
            this.EnableServer();
        }

        private void EnableServer()
        {
            if (this.m_Configuration == null) throw new Exception("No result receiver server configuration.");

            var ipAddress = IPAddress.Any;
            if (this.m_Configuration.ipList != null && this.m_Configuration.ipList.Count == 1)
                ipAddress = IPAddress.Parse(this.m_Configuration.ipList.Single());

            var ipAddStr = Equals(ipAddress, IPAddress.Any) ? "[All interfaces]" : ipAddress.ToString();
            
            this.m_Listener = new TcpListener(ipAddress, this.m_Configuration.port);
            this.m_StatusLabel = "Waiting for connection on: " + ipAddStr + ":" + this.m_Configuration.port;
            
            try
            {
                this.m_Listener.Start(100);
            }
            catch (SocketException e)
            {
                this.m_StatusLabel = "Exception happened: " + e.Message;
                this.Repaint();
                Debug.LogException(e);
            }
            this.m_Running = true;
            this.m_LastMessageReceived = DateTime.Now + this.m_InitialConnectionTimeout;
        }

        public void OnDisable()
        {
            Instance = null;
            if (this.m_Listener != null)
                this.m_Listener.Stop();
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Status:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(this.m_StatusLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Stop"))
            {
                StopReceiver();
                if (InternalEditorUtility.inBatchMode)
                    EditorApplication.Exit(Batch.returnCodeRunError);
            }
        }

        public static void StartReceiver(PlatformRunnerConfiguration configuration)
        {
            var w = (NetworkResultsReceiver)GetWindow(typeof(NetworkResultsReceiver), false);
            w.SetConfiguration(configuration);
            if (!EditorApplication.isCompiling)
            {
                w.EnableServer();
            }
            w.Show(true);
        }

        private void SetConfiguration(PlatformRunnerConfiguration configuration)
        {
            this.m_Configuration = configuration;
        }

        public static void StopReceiver()
        {
            if (Instance == null) return;
            try{
                Instance.Close();
            }catch(Exception e){
                Debug.LogException(e);
                DestroyImmediate(Instance);
            }
        }
    }
}
