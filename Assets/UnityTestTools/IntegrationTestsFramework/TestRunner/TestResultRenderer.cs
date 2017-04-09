using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace UnityTestTools.IntegrationTestsFramework.TestRunner
{

    using UnityTestTools.Common;

    public class TestResultRenderer
    {
        private static class Styles
        {
            public static readonly GUIStyle SucceedLabelStyle;
            public static readonly GUIStyle FailedLabelStyle;
            public static readonly GUIStyle FailedMessagesStyle;

            static Styles()
            {
                SucceedLabelStyle = new GUIStyle("label");
                SucceedLabelStyle.normal.textColor = Color.green;
                SucceedLabelStyle.fontSize = 48;

                FailedLabelStyle = new GUIStyle("label");
                FailedLabelStyle.normal.textColor = Color.red;
                FailedLabelStyle.fontSize = 32;

                FailedMessagesStyle = new GUIStyle("label");
                FailedMessagesStyle.wordWrap = false;
                FailedMessagesStyle.richText = true;
            }
        }
        private readonly Dictionary<string, List<ITestResult>> m_TestCollection = new Dictionary<string, List<ITestResult>>();

        private bool m_ShowResults;
        Vector2 m_ScrollPosition;
        private int m_FailureCount;

        public void ShowResults()
        {
            this.m_ShowResults = true;
            Cursor.visible = true;
        }

        public void AddResults(string sceneName, ITestResult result)
        {
            if (!this.m_TestCollection.ContainsKey(sceneName))
                this.m_TestCollection.Add(sceneName, new List<ITestResult>());
            this.m_TestCollection[sceneName].Add(result);
            if (result.Executed && !result.IsSuccess)
                this.m_FailureCount++;
        }

        public void Draw()
        {
            if (!this.m_ShowResults) return;
            if (this.m_TestCollection.Count == 0)
            {
                GUILayout.Label("All test succeeded", Styles.SucceedLabelStyle, GUILayout.Width(600));
            }
            else
            {
                int count = this.m_TestCollection.Sum (testGroup => testGroup.Value.Count);
                GUILayout.Label(count + " tests failed!", Styles.FailedLabelStyle);

                this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, GUILayout.ExpandWidth(true));
                var text = "";
                foreach (var testGroup in this.m_TestCollection)
                {
                    text += "<b><size=18>" + testGroup.Key + "</size></b>\n";
                    text += string.Join("\n", testGroup.Value
                        .Where(result => !result.IsSuccess)
                        .Select(result => result.Name + " " + result.ResultState + "\n" + result.Message)
                        .ToArray());
                }
                GUILayout.TextArea(text, Styles.FailedMessagesStyle);
                GUILayout.EndScrollView();
            }
            if (GUILayout.Button("Close"))
                Application.Quit();
        }

        public int FailureCount()
        {
            return this.m_FailureCount;
        }
    }

}
