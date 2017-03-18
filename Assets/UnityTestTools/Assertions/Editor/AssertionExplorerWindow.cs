
#if UNITY_METRO
#warning Assertion component is not supported on Windows Store apps
#endif

namespace UnityTestTools.Assertions.Editor
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEditor;

    using UnityEngine;

    using UnityTestTools.Assertions.Comparers;
    using UnityTestTools.Common.Editor;

    [Serializable]
    public class AssertionExplorerWindow : EditorWindow
    {
        private List<AssertionComponent> m_AllAssertions = new List<AssertionComponent>();
        [SerializeField]
        private string m_FilterText = "";
        [SerializeField]
        private FilterType m_FilterType;
        [SerializeField]
        private List<string> m_FoldMarkers = new List<string>();
        [SerializeField]
        private GroupByType m_GroupBy;
        [SerializeField]
        private Vector2 m_ScrollPosition = Vector2.zero;
        private DateTime m_NextReload = DateTime.Now;
        [SerializeField]
        private static bool s_ShouldReload;
        [SerializeField]
        private ShowType m_ShowType;

        public AssertionExplorerWindow()
        {
            this.titleContent = new GUIContent("Assertion Explorer");
        }

        public void OnDidOpenScene()
        {
            this.ReloadAssertionList();
        }

        public void OnFocus()
        {
            this.ReloadAssertionList();
        }

        private void ReloadAssertionList()
        {
            this.m_NextReload = DateTime.Now.AddSeconds(1);
            s_ShouldReload = true;
        }

        public void OnHierarchyChange()
        {
            this.ReloadAssertionList();
        }

        public void OnInspectorUpdate()
        {
            if (s_ShouldReload && this.m_NextReload < DateTime.Now)
            {
                s_ShouldReload = false;
                this.m_AllAssertions = new List<AssertionComponent>((AssertionComponent[])Resources.FindObjectsOfTypeAll(typeof(AssertionComponent)));
                this.Repaint();
            }
        }

        public void OnGUI()
        {
            this.DrawMenuPanel();

            this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition);
            if (this.m_AllAssertions != null)
                this.GetResultRendere().Render(this.FilterResults(this.m_AllAssertions, this.m_FilterText.ToLower()), this.m_FoldMarkers);
            EditorGUILayout.EndScrollView();
        }

        private IEnumerable<AssertionComponent> FilterResults(List<AssertionComponent> assertionComponents, string text)
        {
            if (this.m_ShowType == ShowType.ShowDisabled)
                assertionComponents = assertionComponents.Where(c => !c.enabled).ToList();
            else if (this.m_ShowType == ShowType.ShowEnabled)
                assertionComponents = assertionComponents.Where(c => c.enabled).ToList();

            if (string.IsNullOrEmpty(text))
                return assertionComponents;

            switch (this.m_FilterType)
            {
                case FilterType.ComparerName:
                    return assertionComponents.Where(c => c.Action.GetType().Name.ToLower().Contains(text));
                case FilterType.AttachedGameObject:
                    return assertionComponents.Where(c => c.gameObject.name.ToLower().Contains(text));
                case FilterType.FirstComparedGameObjectPath:
                    return assertionComponents.Where(c => c.Action.thisPropertyPath.ToLower().Contains(text));
                case FilterType.FirstComparedGameObject:
                    return assertionComponents.Where(c => c.Action.go != null
                                                     && c.Action.go.name.ToLower().Contains(text));
                case FilterType.SecondComparedGameObjectPath:
                    return assertionComponents.Where(c =>
                                                     c.Action is ComparerBase
                                                     && (c.Action as ComparerBase).otherPropertyPath.ToLower().Contains(text));
                case FilterType.SecondComparedGameObject:
                    return assertionComponents.Where(c =>
                                                     c.Action is ComparerBase
                                                     && (c.Action as ComparerBase).other != null
                                                     && (c.Action as ComparerBase).other.name.ToLower().Contains(text));
                default:
                    return assertionComponents;
            }
        }

        private readonly IListRenderer m_GroupByComparerRenderer = new GroupByComparerRenderer();
        private readonly IListRenderer m_GroupByExecutionMethodRenderer = new GroupByExecutionMethodRenderer();
        private readonly IListRenderer m_GroupByGoRenderer = new GroupByGoRenderer();
        private readonly IListRenderer m_GroupByTestsRenderer = new GroupByTestsRenderer();
        private readonly IListRenderer m_GroupByNothingRenderer = new GroupByNothingRenderer();

        private IListRenderer GetResultRendere()
        {
            switch (this.m_GroupBy)
            {
                case GroupByType.Comparer:
                    return this.m_GroupByComparerRenderer;
                case GroupByType.ExecutionMethod:
                    return this.m_GroupByExecutionMethodRenderer;
                case GroupByType.GameObjects:
                    return this.m_GroupByGoRenderer;
                case GroupByType.Tests:
                    return this.m_GroupByTestsRenderer;
                default:
                    return this.m_GroupByNothingRenderer;
            }
        }

        private void DrawMenuPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Group by:", Styles.toolbarLabel, GUILayout.MaxWidth(60));
            this.m_GroupBy = (GroupByType)EditorGUILayout.EnumPopup(this.m_GroupBy, EditorStyles.toolbarPopup, GUILayout.MaxWidth(150));

            GUILayout.FlexibleSpace();

            this.m_ShowType = (ShowType)EditorGUILayout.EnumPopup(this.m_ShowType, EditorStyles.toolbarPopup, GUILayout.MaxWidth(100));

            EditorGUILayout.LabelField("Filter by:", Styles.toolbarLabel, GUILayout.MaxWidth(50));
            this.m_FilterType = (FilterType)EditorGUILayout.EnumPopup(this.m_FilterType, EditorStyles.toolbarPopup, GUILayout.MaxWidth(100));
            this.m_FilterText = GUILayout.TextField(this.m_FilterText, "ToolbarSeachTextField", GUILayout.MaxWidth(100));
            if (GUILayout.Button(GUIContent.none, string.IsNullOrEmpty(this.m_FilterText) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton", GUILayout.ExpandWidth(false)))
                this.m_FilterText = "";
            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("Unity Test Tools/Assertion Explorer")]
        public static AssertionExplorerWindow ShowWindow()
        {
            var w = GetWindow(typeof(AssertionExplorerWindow));
            w.Show();
            return w as AssertionExplorerWindow;
        }

        private enum FilterType
        {
            ComparerName,
            FirstComparedGameObject,
            FirstComparedGameObjectPath,
            SecondComparedGameObject,
            SecondComparedGameObjectPath,
            AttachedGameObject
        }

        private enum ShowType
        {
            ShowAll,
            ShowEnabled,
            ShowDisabled
        }

        private enum GroupByType
        {
            Nothing,
            Comparer,
            GameObjects,
            ExecutionMethod,
            Tests
        }

        public static void Reload()
        {
            s_ShouldReload = true;
        }
    }
}
