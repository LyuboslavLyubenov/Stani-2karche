namespace UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.Renderer
{

    using System.Collections.Generic;

    using UnityEngine;

    using UnityTestTools.Common.Editor;
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    class IntegrationTestLine : IntegrationTestRendererBase
    {
        public static List<TestResult> Results;
        protected TestResult m_Result;

        public IntegrationTestLine(GameObject gameObject, TestResult testResult) : base(gameObject)
        {
            this.m_Result = testResult;
        }

        protected internal override void DrawLine(Rect rect, GUIContent label, bool isSelected, RenderingOptions options)
        {
            if(Event.current.type != EventType.repaint)
                return;

            Styles.testName.Draw (rect, label, false, false, false, isSelected);

            if (this.m_Result.IsTimeout)
            {
                float min, max;
                Styles.testName.CalcMinMaxWidth(label, out min, out max);
                var timeoutRect = new Rect(rect);
                timeoutRect.x += min - 12;
                Styles.testName.Draw(timeoutRect, s_GUITimeoutIcon, false, false, false, isSelected);
            }
        }

        protected internal override TestResult.ResultType GetResult()
        {
            if (!this.m_Result.Executed && this.test.ignored) return TestResult.ResultType.Ignored;
            return this.m_Result.resultType;
        }

        protected internal override bool IsVisible(RenderingOptions options)
        {
            if (!string.IsNullOrEmpty(options.nameFilter) && !this.m_GameObject.name.ToLower().Contains(options.nameFilter.ToLower())) return false;
            if (!options.showSucceeded && this.m_Result.IsSuccess) return false;
            if (!options.showFailed && this.m_Result.IsFailure) return false;
            if (!options.showNotRunned && !this.m_Result.Executed) return false;
            if (!options.showIgnored && this.test.ignored) return false;
            return true;
        }

        public override bool SetCurrentTest(TestComponent tc)
        {
            this.m_IsRunning = this.test == tc;
            return this.m_IsRunning;
        }
    }
}
