namespace UnityTestTools.Common.Editor
{

    using System.Collections.Generic;
    using System.Linq;

    using UnityEditor;

    using UnityEngine;

    using UnityTestTools.Common;
    using UnityTestTools.Common.Editor.ResultWriter;
    using UnityTestTools.IntegrationTestsFramework.TestRunner.Editor.Renderer;

    public class TestFilterSettings
    {
        public bool ShowSucceeded;
        public bool ShowFailed;
        public bool ShowIgnored;
        public bool ShowNotRun;
        
        public string FilterByName;
        public int FilterByCategory;
        
        private GUIContent _succeededBtn;
        private GUIContent _failedBtn;
        private GUIContent _ignoredBtn;
        private GUIContent _notRunBtn;
        
        public string[] AvailableCategories;
        
        private readonly string _prefsKey;
        
        public TestFilterSettings(string prefsKey)
        {
            this._prefsKey = prefsKey;
            this.Load();
            this.UpdateCounters(Enumerable.Empty<ITestResult>());
        }
            
        public void Load()
        {
            this.ShowSucceeded = EditorPrefs.GetBool(this._prefsKey + ".ShowSucceeded", true);
            this.ShowFailed = EditorPrefs.GetBool(this._prefsKey + ".ShowFailed", true);
            this.ShowIgnored = EditorPrefs.GetBool(this._prefsKey + ".ShowIgnored", true);
            this.ShowNotRun = EditorPrefs.GetBool(this._prefsKey + ".ShowNotRun", true);
            this.FilterByName = EditorPrefs.GetString(this._prefsKey + ".FilterByName", string.Empty);
            this.FilterByCategory = EditorPrefs.GetInt(this._prefsKey + ".FilterByCategory", 0);
        }
        
        public void Save()
        {
            EditorPrefs.SetBool(this._prefsKey + ".ShowSucceeded", this.ShowSucceeded);
            EditorPrefs.SetBool(this._prefsKey + ".ShowFailed", this.ShowFailed);
            EditorPrefs.SetBool(this._prefsKey + ".ShowIgnored", this.ShowIgnored);
            EditorPrefs.SetBool(this._prefsKey + ".ShowNotRun", this.ShowNotRun);
            EditorPrefs.SetString(this._prefsKey + ".FilterByName", this.FilterByName);
            EditorPrefs.SetInt(this._prefsKey + ".FilterByCategory", this.FilterByCategory);
        }
        
        public void UpdateCounters(IEnumerable<ITestResult> results)
        {
            var summary = new ResultSummarizer(results);
            
            this._succeededBtn = new GUIContent(summary.Passed.ToString(), Icons.SuccessImg, "Show tests that succeeded");
            this._failedBtn = new GUIContent((summary.Errors + summary.Failures + summary.Inconclusive).ToString(), Icons.FailImg, "Show tests that failed");
            this._ignoredBtn = new GUIContent((summary.Ignored + summary.NotRunnable).ToString(), Icons.IgnoreImg, "Show tests that are ignored");
            this._notRunBtn = new GUIContent((summary.TestsNotRun - summary.Ignored - summary.NotRunnable).ToString(), Icons.UnknownImg, "Show tests that didn't run");
        }
        
        public string[] GetSelectedCategories()
        {
            if(this.AvailableCategories == null) return new string[0];
            
            return this.AvailableCategories.Where ((c, i) => (this.FilterByCategory & (1 << i)) != 0).ToArray();
        }
        
        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            this.FilterByName = GUILayout.TextField(this.FilterByName, "ToolbarSeachTextField", GUILayout.MinWidth(100), GUILayout.MaxWidth(250), GUILayout.ExpandWidth(true));
            if(GUILayout.Button (GUIContent.none, string.IsNullOrEmpty(this.FilterByName) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton"))
                this.FilterByName = string.Empty;
            
            if (this.AvailableCategories != null && this.AvailableCategories.Length > 0)
                this.FilterByCategory = EditorGUILayout.MaskField(this.FilterByCategory, this.AvailableCategories, EditorStyles.toolbarDropDown, GUILayout.MaxWidth(90));
            
            this.ShowSucceeded = GUILayout.Toggle(this.ShowSucceeded, this._succeededBtn, EditorStyles.toolbarButton);
            this.ShowFailed = GUILayout.Toggle(this.ShowFailed, this._failedBtn, EditorStyles.toolbarButton);
            this.ShowIgnored = GUILayout.Toggle(this.ShowIgnored, this._ignoredBtn, EditorStyles.toolbarButton);
            this.ShowNotRun = GUILayout.Toggle(this.ShowNotRun, this._notRunBtn, EditorStyles.toolbarButton);
            
            if(EditorGUI.EndChangeCheck()) this.Save ();
        }
        
        public RenderingOptions BuildRenderingOptions()
        {
            var options = new RenderingOptions();
            options.showSucceeded = this.ShowSucceeded;
            options.showFailed = this.ShowFailed;
            options.showIgnored = this.ShowIgnored;
            options.showNotRunned = this.ShowNotRun;
            options.nameFilter = this.FilterByName;
            options.categories = this.GetSelectedCategories();
            return options;
        }
    }
    
}
