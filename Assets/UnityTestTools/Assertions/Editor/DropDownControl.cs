namespace Assets.UnityTestTools.Assertions.Editor
{

    using System;

    using UnityEditor;

    using UnityEngine;

    [Serializable]
    internal class DropDownControl<T>
    {
        private readonly GUILayoutOption[] m_ButtonLayoutOptions = { GUILayout.ExpandWidth(true) };
        public Func<T, string> convertForButtonLabel = s => s.ToString();
        public Func<T, string> convertForGUIContent = s => s.ToString();
        public Func<T[], bool> ignoreConvertForGUIContent = t => t.Length <= 40;
        public Action<T> printContextMenu = null;
        public string tooltip = "";

        private object m_SelectedValue;


        public void Draw(T selected, T[] options, Action<T> onValueSelected)
        {
            this.Draw(null,
                 selected,
                 options,
                 onValueSelected);
        }

        public void Draw(string label, T selected, T[] options, Action<T> onValueSelected)
        {
            this.Draw(label, selected, () => options, onValueSelected);
        }

        public void Draw(string label, T selected, Func<T[]> loadOptions, Action<T> onValueSelected)
        {
            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.BeginHorizontal();
            var guiContent = new GUIContent(label);
            var labelSize = EditorStyles.label.CalcSize(guiContent);

            if (!string.IsNullOrEmpty(label))
                GUILayout.Label(label, EditorStyles.label, GUILayout.Width(labelSize.x));

            if (GUILayout.Button(new GUIContent(this.convertForButtonLabel(selected), this.tooltip),
                                 EditorStyles.popup, this.m_ButtonLayoutOptions))
            {
                if (Event.current.button == 0)
                {
                    this.PrintMenu(loadOptions());
                }
                else if (this.printContextMenu != null && Event.current.button == 1)
                    this.printContextMenu(selected);
            }

            if (this.m_SelectedValue != null)
            {
                onValueSelected((T)this.m_SelectedValue);
                this.m_SelectedValue = null;
            }
            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();
        }

        public void PrintMenu(T[] options)
        {
            var menu = new GenericMenu();
            foreach (var s in options)
            {
                var localS = s;
                menu.AddItem(new GUIContent((this.ignoreConvertForGUIContent(options) ? localS.ToString() : this.convertForGUIContent(localS))),
                             false,
                             () => { this.m_SelectedValue = localS; }
                             );
            }
            menu.ShowAsContext();
        }
    }
}
