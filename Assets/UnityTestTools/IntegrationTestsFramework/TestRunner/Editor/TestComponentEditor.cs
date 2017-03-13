namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner.Editor
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;

    using UnityEditor;
    using UnityEditor.SceneManagement;

    using UnityEngine;

    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TestComponent))]
    public class TestComponentEditor : Editor
    {
        private SerializedProperty m_ExpectException;
        private SerializedProperty m_ExpectedExceptionList;
        private SerializedProperty m_Ignored;
        private SerializedProperty m_SucceedAssertions;
        private SerializedProperty m_SucceedWhenExceptionIsThrown;
        private SerializedProperty m_Timeout;

        #region GUI Contens

        private readonly GUIContent m_GUIExpectException = new GUIContent("Expect exception", "Should the test expect an exception");
        private readonly GUIContent m_GUIExpectExceptionList = new GUIContent("Expected exception list", "A comma separated list of exception types which will not fail the test when thrown");
        private readonly GUIContent m_GUIIgnore = new GUIContent("Ignore", "Ignore the tests in runs");
        private readonly GUIContent m_GUIIncludePlatforms = new GUIContent("Included platforms", "Platform on which the test should run");
        private readonly GUIContent m_GUISuccedOnAssertions = new GUIContent("Succeed on assertions", "Succeed after all assertions are executed");
        private readonly GUIContent m_GUISucceedWhenExceptionIsThrown = new GUIContent("Succeed when exception is thrown", "Should the test succeed when an expected exception is thrown");
        private readonly GUIContent m_GUITestName = new GUIContent("Test name", "Name of the test (is equal to the GameObject name)");
        private readonly GUIContent m_GUITimeout = new GUIContent("Timeout", "Number of seconds after which the test will timeout");

        #endregion

        public void OnEnable()
        {
            this.m_Timeout = this.serializedObject.FindProperty("timeout");
            this.m_Ignored = this.serializedObject.FindProperty("ignored");
            this.m_SucceedAssertions = this.serializedObject.FindProperty("succeedAfterAllAssertionsAreExecuted");
            this.m_ExpectException = this.serializedObject.FindProperty("expectException");
            this.m_ExpectedExceptionList = this.serializedObject.FindProperty("expectedExceptionList");
            this.m_SucceedWhenExceptionIsThrown = this.serializedObject.FindProperty("succeedWhenExceptionIsThrown");
        }

        public override void OnInspectorGUI()
        {
            var component = (TestComponent)this.target;

            if (component.dynamic)
            {
                if(GUILayout.Button("Reload dynamic tests"))
                {
                    TestComponent.DestroyAllDynamicTests();
                    Selection.objects = new Object[0];
                    IntegrationTestsRunnerWindow.selectedInHierarchy = false;
                    GUIUtility.ExitGUI();
                    return;
                }
                EditorGUILayout.HelpBox("This is a test generated from code. No changes in the component will be persisted.", MessageType.Info);
            }

            if (component.IsTestGroup())
            {
                EditorGUI.BeginChangeCheck();
                var newGroupName = EditorGUILayout.TextField(this.m_GUITestName, component.name);
                if (EditorGUI.EndChangeCheck()) component.name = newGroupName;

                this.serializedObject.ApplyModifiedProperties();
                return;
            }

            this.serializedObject.Update();

            EditorGUI.BeginDisabledGroup(this.serializedObject.isEditingMultipleObjects);

            EditorGUI.BeginChangeCheck();
            var newName = EditorGUILayout.TextField(this.m_GUITestName, component.name);
            if (EditorGUI.EndChangeCheck()) component.name = newName;

            if (component.platformsToIgnore == null)
            {
                component.platformsToIgnore = this.GetListOfIgnoredPlatforms(Enum.GetNames(typeof(TestComponent.IncludedPlatforms)), (int)component.includedPlatforms);
            }

            var enumList = Enum.GetNames(typeof(RuntimePlatform));
            var flags = this.GetFlagList(enumList, component.platformsToIgnore);
            flags = EditorGUILayout.MaskField(this.m_GUIIncludePlatforms, flags, enumList, EditorStyles.popup);
            var newList = this.GetListOfIgnoredPlatforms(enumList, flags);
            if (!component.dynamic)
                component.platformsToIgnore = newList;
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(this.m_Timeout, this.m_GUITimeout);
            EditorGUILayout.PropertyField(this.m_Ignored, this.m_GUIIgnore);
            EditorGUILayout.PropertyField(this.m_SucceedAssertions, this.m_GUISuccedOnAssertions);
            EditorGUILayout.PropertyField(this.m_ExpectException, this.m_GUIExpectException);

            EditorGUI.BeginDisabledGroup(!this.m_ExpectException.boolValue);
            EditorGUILayout.PropertyField(this.m_ExpectedExceptionList, this.m_GUIExpectExceptionList);
            EditorGUILayout.PropertyField(this.m_SucceedWhenExceptionIsThrown, this.m_GUISucceedWhenExceptionIsThrown);
            EditorGUI.EndDisabledGroup();

            if (!component.dynamic)
                this.serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private string[] GetListOfIgnoredPlatforms(string[] enumList, int flags)
        {
            var notSelectedPlatforms = new List<string>();
            for (int i = 0; i < enumList.Length; i++)
            {
                var sel = (flags & (1 << i)) != 0;
                if (!sel) notSelectedPlatforms.Add(enumList[i]);
            }
            return notSelectedPlatforms.ToArray();
        }

        private int GetFlagList(string[] enumList, string[] platformsToIgnore)
        {
            int flags = ~0;
            for (int i = 0; i < enumList.Length; i++)
                if (platformsToIgnore != null && platformsToIgnore.Any(s => s == enumList[i]))
                    flags &= ~(1 << i);
            return flags;
        }
    }
}
