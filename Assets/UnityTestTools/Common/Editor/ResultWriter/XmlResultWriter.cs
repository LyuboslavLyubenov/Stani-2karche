namespace Assets.UnityTestTools.Common.Editor.ResultWriter
{

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Text;

    using Assets.UnityTestTools.Common;

    using UnityEngine;

    public class XmlResultWriter
    {
        private readonly StringBuilder m_ResultWriter = new StringBuilder();
        private int m_Indend;
        private readonly string m_SuiteName;
        private readonly ITestResult[] m_Results;
        string m_Platform;

        public XmlResultWriter(string suiteName, string platform, ITestResult[] results)
        {
            this.m_SuiteName = suiteName;
            this.m_Results = results;
            this.m_Platform = platform;
        }

        private const string k_NUnitVersion = "2.6.2-Unity";

        public string GetTestResult()
        {
            this.InitializeXmlFile(this.m_SuiteName, new ResultSummarizer(this.m_Results));
            foreach (var result in this.m_Results)
            {
                this.WriteResultElement(result);
            }
            this.TerminateXmlFile();
            return this.m_ResultWriter.ToString();
        }

        private void InitializeXmlFile(string resultsName, ResultSummarizer summaryResults)
        {
            this.WriteHeader();

            DateTime now = DateTime.Now;
            var attributes = new Dictionary<string, string>
            {
                {"name", "Unity Tests"},
                {"total", summaryResults.TestsRun.ToString()},
                {"errors", summaryResults.Errors.ToString()},
                {"failures", summaryResults.Failures.ToString()},
                {"not-run", summaryResults.TestsNotRun.ToString()},
                {"inconclusive", summaryResults.Inconclusive.ToString()},
                {"ignored", summaryResults.Ignored.ToString()},
                {"skipped", summaryResults.Skipped.ToString()},
                {"invalid", summaryResults.NotRunnable.ToString()},
                {"date", now.ToString("yyyy-MM-dd")},
                {"time", now.ToString("HH:mm:ss")}
            };

            this.WriteOpeningElement("test-results", attributes);

            this.WriteEnvironment(this.m_Platform);
            this.WriteCultureInfo();
            this.WriteTestSuite(resultsName, summaryResults);
            this.WriteOpeningElement("results");
        }

        private void WriteOpeningElement(string elementName)
        {
            this.WriteOpeningElement(elementName, new Dictionary<string, string>());
        }

        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes)
        {
            this.WriteOpeningElement(elementName, attributes, false);
        }


        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes, bool closeImmediatelly)
        {
            this.WriteIndend();
            this.m_Indend++;
            this.m_ResultWriter.Append("<");
            this.m_ResultWriter.Append(elementName);
            foreach (var attribute in attributes)
            {
                this.m_ResultWriter.AppendFormat(" {0}=\"{1}\"", attribute.Key, SecurityElement.Escape(attribute.Value));
            }
            if (closeImmediatelly)
            {
                this.m_ResultWriter.Append(" /");
                this.m_Indend--;
            }
            this.m_ResultWriter.AppendLine(">");
        }

        private void WriteIndend()
        {
            for (int i = 0; i < this.m_Indend; i++)
            {
                this.m_ResultWriter.Append("  ");
            }
        }

        private void WriteClosingElement(string elementName)
        {
            this.m_Indend--;
            this.WriteIndend();
            this.m_ResultWriter.AppendLine("</" + elementName + ">");
        }

        private void WriteHeader()
        {
            this.m_ResultWriter.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            this.m_ResultWriter.AppendLine("<!--This file represents the results of running a test suite-->");
        }

        static string GetEnvironmentUserName()
        {
            return Environment.UserName;
        }

        static string GetEnvironmentMachineName()
        {
            return Environment.MachineName;
        }

        static string GetEnvironmentUserDomainName()
        {
            return Environment.UserDomainName;
        }

        static string GetEnvironmentVersion()
        {
            return Environment.Version.ToString();
        }

        static string GetEnvironmentOSVersion()
        {
            return Environment.OSVersion.ToString();
        }

        static string GetEnvironmentOSVersionPlatform()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        static string EnvironmentGetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        private void WriteEnvironment( string targetPlatform )
        {
            var attributes = new Dictionary<string, string>
            {
                {"nunit-version", k_NUnitVersion},
                {"clr-version", GetEnvironmentVersion()},
                {"os-version", GetEnvironmentOSVersion()},
                {"platform", GetEnvironmentOSVersionPlatform()},
                {"cwd", EnvironmentGetCurrentDirectory()},
                {"machine-name", GetEnvironmentMachineName()},
                {"user", GetEnvironmentUserName()},
                {"user-domain", GetEnvironmentUserDomainName()},
                {"unity-version", Application.unityVersion},
                {"unity-platform", targetPlatform}
            };
            this.WriteOpeningElement("environment", attributes, true);
        }

        private void WriteCultureInfo()
        {
            var attributes = new Dictionary<string, string>
            {
                {"current-culture", CultureInfo.CurrentCulture.ToString()},
                {"current-uiculture", CultureInfo.CurrentUICulture.ToString()}
            };
            this.WriteOpeningElement("culture-info", attributes, true);
        }

        private void WriteTestSuite(string resultsName, ResultSummarizer summaryResults)
        {
            var attributes = new Dictionary<string, string>
            {
                {"name", resultsName},
                {"type", "Assembly"},
                {"executed", "True"},
                {"result", summaryResults.Success ? "Success" : "Failure"},
                {"success", summaryResults.Success ? "True" : "False"},
                {"time", summaryResults.Duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo)}
            };
            this.WriteOpeningElement("test-suite", attributes);
        }

        private void WriteResultElement(ITestResult result)
        {
            this.StartTestElement(result);

            switch (result.ResultState)
            {
                case TestResultState.Ignored:
                case TestResultState.NotRunnable:
                case TestResultState.Skipped:
                    this.WriteReasonElement(result);
                    break;

                case TestResultState.Failure:
                case TestResultState.Error:
                case TestResultState.Cancelled:
                    this.WriteFailureElement(result);
                    break;
                case TestResultState.Success:
                case TestResultState.Inconclusive:
                    if (result.Message != null)
                        this.WriteReasonElement(result);
                    break;
            };

            this.WriteClosingElement("test-case");
        }

        private void TerminateXmlFile()
        {
            this.WriteClosingElement("results");
            this.WriteClosingElement("test-suite");
            this.WriteClosingElement("test-results");
        }

        #region Element Creation Helpers

        private void StartTestElement(ITestResult result)
        {
            var attributes = new Dictionary<string, string>
            {
                {"name", result.FullName},
                {"executed", result.Executed.ToString()}
            };
            string resultString;
            switch (result.ResultState)
            {
                case TestResultState.Cancelled:
                    resultString = TestResultState.Failure.ToString();
                    break;
                default:
                    resultString = result.ResultState.ToString();
                    break;
            }
            attributes.Add("result", resultString);
            if (result.Executed)
            {
                attributes.Add("success", result.IsSuccess.ToString());
                attributes.Add("time", result.Duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo));
            }
            this.WriteOpeningElement("test-case", attributes);
        }

        private void WriteReasonElement(ITestResult result)
        {
            this.WriteOpeningElement("reason");
            this.WriteOpeningElement("message");
            this.WriteCData(result.Message);
            this.WriteClosingElement("message");
            this.WriteClosingElement("reason");
        }

        private void WriteFailureElement(ITestResult result)
        {
            this.WriteOpeningElement("failure");
            this.WriteOpeningElement("message");
            this.WriteCData(result.Message);
            this.WriteClosingElement("message");
            this.WriteOpeningElement("stack-trace");
            if (result.StackTrace != null)
                this.WriteCData(StackTraceFilter.Filter(result.StackTrace));
            this.WriteClosingElement("stack-trace");
            this.WriteClosingElement("failure");
        }

        #endregion

        private void WriteCData(string text)
        {
            if (string.IsNullOrEmpty(text)) 
                return;
            this.m_ResultWriter.AppendFormat("<![CDATA[{0}]]>", text);
            this.m_ResultWriter.AppendLine();
        }

        public void WriteToFile(string resultDestiantion, string resultFileName)
        {
            try
            {
                var path = Path.Combine(resultDestiantion, resultFileName);
                Debug.Log("Saving results in " + path);
                File.WriteAllText(path, this.GetTestResult(), Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while opening file");
                Debug.LogException(e);
            }
        }
    }
}
