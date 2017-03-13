namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.Renderer
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.UnityTestTools.Common.Editor;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;

    using UnityEditor;

    using UnityEngine;

    class IntegrationTestGroupLine : IntegrationTestRendererBase
    {
        public static List<GameObject> FoldMarkers;
        private IntegrationTestRendererBase[] m_Children;

        public IntegrationTestGroupLine(GameObject gameObject) : base(gameObject)
        {
        }

        protected internal override void DrawLine(Rect rect, GUIContent label, bool isSelected, RenderingOptions options)
        {
            EditorGUI.BeginChangeCheck();
            var isClassFolded = !EditorGUI.Foldout(rect, !this.Folded, label, isSelected ? Styles.selectedFoldout : Styles.foldout);
            if (EditorGUI.EndChangeCheck()) this.Folded = isClassFolded;
        }

        private bool Folded
        {
            get { return FoldMarkers.Contains(this.m_GameObject); }

            set
            {
                if (value) FoldMarkers.Add(this.m_GameObject);
                else FoldMarkers.RemoveAll(s => s == this.m_GameObject);
            }
        }

        protected internal override void Render(int indend, RenderingOptions options)
        {
            base.Render(indend, options);
            if (!this.Folded)
                foreach (var child in this.m_Children)
                    child.Render(indend + 1, options);
        }

        protected internal override TestResult.ResultType GetResult()
        {
            bool ignored = false;
            bool success = false;
            foreach (var child in this.m_Children)
            {
                var result = child.GetResult();

                if (result == TestResult.ResultType.Failed || result == TestResult.ResultType.FailedException || result == TestResult.ResultType.Timeout)
                    return TestResult.ResultType.Failed;
                if (result == TestResult.ResultType.Success)
                    success = true;
                else if (result == TestResult.ResultType.Ignored)
                    ignored = true;
                else
                    ignored = false;
            }
            if (success) return TestResult.ResultType.Success;
            if (ignored) return TestResult.ResultType.Ignored;
            return TestResult.ResultType.NotRun;
        }

        protected internal override bool IsVisible(RenderingOptions options)
        {
            return this.m_Children.Any(c => c.IsVisible(options));
        }

        public override bool SetCurrentTest(TestComponent tc)
        {
            this.m_IsRunning = false;
            foreach (var child in this.m_Children)
                this.m_IsRunning |= child.SetCurrentTest(tc);
            return this.m_IsRunning;
        }

        public void AddChildren(IntegrationTestRendererBase[] parseTestList)
        {
            this.m_Children = parseTestList;
        }
    }
}
