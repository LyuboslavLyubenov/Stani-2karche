namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner
{

    using System;

    using Assets.UnityTestTools.Common;

    using UnityEngine;

    [Serializable]
    public class TestResult : ITestResult, IComparable<TestResult>
    {
        private readonly GameObject m_Go;
        private string m_Name;
        public ResultType resultType = ResultType.NotRun;
        public double duration;
        public string messages;
        public string stacktrace;
        public string id;
        public bool dynamicTest;

        public TestComponent TestComponent;

        public GameObject GameObject
        {
            get { return this.m_Go; }
        }

        public TestResult(TestComponent testComponent)
        {
            this.TestComponent = testComponent;
            this.m_Go = testComponent.gameObject;
            this.id = testComponent.gameObject.GetInstanceID().ToString();
            this.dynamicTest = testComponent.dynamic;

            if (this.m_Go != null) this.m_Name = this.m_Go.name;

            if (this.dynamicTest)
                this.id = testComponent.dynamicTypeName;
        }

        public void Update(TestResult oldResult)
        {
            this.resultType = oldResult.resultType;
            this.duration = oldResult.duration;
            this.messages = oldResult.messages;
            this.stacktrace = oldResult.stacktrace;
        }

        public enum ResultType
        {
            Success,
            Failed,
            Timeout,
            NotRun,
            FailedException,
            Ignored
        }

        public void Reset()
        {
            this.resultType = ResultType.NotRun;
            this.duration = 0f;
            this.messages = "";
            this.stacktrace = "";
        }

        #region ITestResult implementation
        public TestResultState ResultState {
            get
            {
                switch (this.resultType)
                {
                    case ResultType.Success: return TestResultState.Success;
                    case ResultType.Failed: return TestResultState.Failure;
                    case ResultType.FailedException: return TestResultState.Error;
                    case ResultType.Ignored: return TestResultState.Ignored;
                    case ResultType.NotRun: return TestResultState.Skipped;
                    case ResultType.Timeout: return TestResultState.Cancelled;
                    default: throw new Exception();
                }
            }
        }
        public string Message { get { return this.messages; } }
        public string Logs { get { return null; } }
        public bool Executed { get { return this.resultType != ResultType.NotRun; } }
        public string Name { get { if (this.m_Go != null) this.m_Name = this.m_Go.name; return this.m_Name; } }
        public string Id { get { return this.id; } }
        public bool IsSuccess { get { return this.resultType == ResultType.Success; } }
        public bool IsTimeout { get { return this.resultType == ResultType.Timeout; } }
        public double Duration { get { return this.duration; } }
        public string StackTrace { get { return this.stacktrace; } }
        public string FullName {
            get
            {
                var fullName = this.Name;
                if (this.m_Go != null)
                {
                    var tempGo = this.m_Go.transform.parent;
                    while (tempGo != null)
                    {
                        fullName = tempGo.name + "." + fullName;
                        tempGo = tempGo.transform.parent;
                    }
                }
                return fullName;
            }
        }

        public bool IsIgnored { get { return this.resultType == ResultType.Ignored; } }
        public bool IsFailure
        {
            get
            {
                return this.resultType == ResultType.Failed
                       || this.resultType == ResultType.FailedException
                       || this.resultType == ResultType.Timeout;
            }
        }
        #endregion

        #region IComparable, GetHashCode and Equals implementation
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public int CompareTo(TestResult other)
        {
            var result = this.Name.CompareTo(other.Name);
            if (result == 0)
                result = this.m_Go.GetInstanceID().CompareTo(other.m_Go.GetInstanceID());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is TestResult)
                return this.GetHashCode() == obj.GetHashCode();
            return base.Equals(obj);
        }
        #endregion
    }
}
