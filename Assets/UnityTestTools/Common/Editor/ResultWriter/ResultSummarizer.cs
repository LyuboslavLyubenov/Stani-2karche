// ****************************************************************
// Based on nUnit 2.6.2 (http://www.nunit.org/)
// ****************************************************************

namespace UnityTestTools.Common.Editor.ResultWriter
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for ResultSummarizer.
    /// </summary>
    public class ResultSummarizer
    {
        private int m_ErrorCount;
        private int m_FailureCount;
        private int m_IgnoreCount;
        private int m_InconclusiveCount;
        private int m_NotRunnable;
        private int m_ResultCount;
        private int m_SkipCount;
        private int m_SuccessCount;
        private int m_TestsRun;

        private TimeSpan m_Duration;

        public ResultSummarizer(IEnumerable<ITestResult> results)
        {
            foreach (var result in results)
                this.Summarize(result);
        }

        public bool Success
        {
            get { return this.m_FailureCount == 0; }
        }

        /// <summary>
        /// Returns the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int ResultCount
        {
            get { return this.m_ResultCount; }
        }

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as ResultCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int TestsRun
        {
            get { return this.m_TestsRun; }
        }

        /// <summary>
        /// Returns the number of tests that passed
        /// </summary>
        public int Passed
        {
            get { return this.m_SuccessCount; }
        }

        /// <summary>
        /// Returns the number of test cases that had an error.
        /// </summary>
        public int Errors
        {
            get { return this.m_ErrorCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Failures
        {
            get { return this.m_FailureCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Inconclusive
        {
            get { return this.m_InconclusiveCount; }
        }

        /// <summary>
        /// Returns the number of test cases that were not runnable
        /// due to errors in the signature of the class or method.
        /// Such tests are also counted as Errors.
        /// </summary>
        public int NotRunnable
        {
            get { return this.m_NotRunnable; }
        }

        /// <summary>
        /// Returns the number of test cases that were skipped.
        /// </summary>
        public int Skipped
        {
            get { return this.m_SkipCount; }
        }

        public int Ignored
        {
            get { return this.m_IgnoreCount; }
        }

        public double Duration
        {
            get { return this.m_Duration.TotalSeconds; }
        }

        public int TestsNotRun
        {
            get { return this.m_SkipCount + this.m_IgnoreCount + this.m_NotRunnable; }
        }

        public void Summarize(ITestResult result)
        {
            this.m_Duration += TimeSpan.FromSeconds(result.Duration);
            this.m_ResultCount++;
            
            if(!result.Executed)
            {
                if(result.IsIgnored)
                {
                    this.m_IgnoreCount++;
                    return;
                }
                
                this.m_SkipCount++;
                return;
            }
            
            switch (result.ResultState)
            {
                case TestResultState.Success:
                    this.m_SuccessCount++;
                    this.m_TestsRun++;
                    break;
                case TestResultState.Failure:
                    this.m_FailureCount++;
                    this.m_TestsRun++;
                    break;
                case TestResultState.Error:
                case TestResultState.Cancelled:
                    this.m_ErrorCount++;
                    this.m_TestsRun++;
                    break;
                case TestResultState.Inconclusive:
                    this.m_InconclusiveCount++;
                    this.m_TestsRun++;
                    break;
                case TestResultState.NotRunnable:
                    this.m_NotRunnable++;
                    // errorCount++;
                    break;
                case TestResultState.Ignored:
                    this.m_IgnoreCount++;
                    break;
                default:
                    this.m_SkipCount++;
                    break;
            }
        }
    }
}
