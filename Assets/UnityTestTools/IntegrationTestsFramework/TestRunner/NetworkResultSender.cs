#if !UNITY_METRO && (UNITY_PRO_LICENSE || !(UNITY_ANDROID || UNITY_IPHONE))
#define UTT_SOCKETS_SUPPORTED
#endif

#if UTT_SOCKETS_SUPPORTED
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner
{

    using System.Collections.Generic;

    public class NetworkResultSender : ITestRunnerCallback
    {
#if UTT_SOCKETS_SUPPORTED
        private readonly TimeSpan m_ConnectionTimeout = TimeSpan.FromSeconds(5);

        private readonly string m_Ip;
        private readonly int m_Port;
#endif
        private bool m_LostConnection;

        public NetworkResultSender(string ip, int port)
        {
#if UTT_SOCKETS_SUPPORTED
            m_Ip = ip;
            m_Port = port;
#endif
        }

        private bool SendDTO(ResultDTO dto)
        {
            if (this.m_LostConnection) return false;
#if UTT_SOCKETS_SUPPORTED 
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    var result = tcpClient.BeginConnect(m_Ip, m_Port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(m_ConnectionTimeout);
                    if (!success)
                    {
                        return false;
                    }
                    try
                    {
                        tcpClient.EndConnect(result);
                    }
                    catch (SocketException)
                    {
                        m_LostConnection = true;
                        return false;
                    }

                    var bf = new DTOFormatter();
                    bf.Serialize(tcpClient.GetStream(), dto);
                    tcpClient.GetStream().Close();
                    tcpClient.Close();
                    Debug.Log("Sent " + dto.messageType);
                }
            }
            catch (SocketException e)
            {
                Debug.LogException(e);
                m_LostConnection = true;
                return false;
            }
#endif  // if UTT_SOCKETS_SUPPORTED
            return true;
        }

        public bool Ping()
        {
            var result = this.SendDTO(ResultDTO.CreatePing());
            this.m_LostConnection = false;
            return result;
        }

        public void RunStarted(string platform, List<TestComponent> testsToRun)
        {
            this.SendDTO(ResultDTO.CreateRunStarted());
        }

        public void RunFinished(List<TestResult> testResults)
        {
            this.SendDTO(ResultDTO.CreateRunFinished(testResults));
        }

        public void TestStarted(TestResult test)
        {
            this.SendDTO(ResultDTO.CreateTestStarted(test));
        }

        public void TestFinished(TestResult test)
        {
            this.SendDTO(ResultDTO.CreateTestFinished(test));
        }

        public void AllScenesFinished()
        {
            this.SendDTO (ResultDTO.CreateAllScenesFinished ());
        }

        public void TestRunInterrupted(List<ITestComponent> testsNotRun)
        {
            this.RunFinished(new List<TestResult>());
        }
    }
}
