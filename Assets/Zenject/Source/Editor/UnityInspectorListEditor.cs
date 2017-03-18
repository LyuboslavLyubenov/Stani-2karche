namespace Zenject.Source.Editor
{

    using System.Collections.Generic;
    using System.Linq;

    using UnityEditor;

    using UnityEditorInternal;

    using UnityEngine;

    using Zenject.Source.Internal;

    public abstract class UnityInspectorListEditor : UnityEditor.Editor
    {
        List<ReorderableList> _installersLists;
        List<SerializedProperty> _installersProperties;

        protected abstract string[] PropertyDisplayNames
        {
            get;
        }

        protected abstract string[] PropertyNames
        {
            get;
        }

        protected abstract string[] PropertyDescriptions
        {
            get;
        }

        public virtual void OnEnable()
        {
            this._installersProperties = new List<SerializedProperty>();
            this._installersLists = new List<ReorderableList>();

            var descriptions = this.PropertyDescriptions;
            var names = this.PropertyNames;
            var displayNames = this.PropertyDisplayNames;

            Assert.IsEqual(descriptions.Length, names.Length);

            var infos = Enumerable.Range(0, names.Length).Select(i => new { Name = names[i], DisplayName = displayNames[i], Description = descriptions[i] }).ToList();

            foreach (var info in infos)
            {
                var installersProperty = this.serializedObject.FindProperty(info.Name);
                this._installersProperties.Add(installersProperty);

                ReorderableList installersList = new ReorderableList(this.serializedObject, installersProperty, true, true, true, true);
                this._installersLists.Add(installersList);

                var closedName = info.DisplayName;
                var closedDesc = info.Description;

                installersList.drawHeaderCallback += rect =>
                {
                    GUI.Label(rect,
                    new GUIContent(closedName, closedDesc));
                };
                installersList.drawElementCallback += (rect, index, active, focused) =>
                {
                    rect.width -= 40;
                    rect.x += 20;
                    EditorGUI.PropertyField(rect, installersProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
                };
            }
        }

        public sealed override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            this.OnGui();

            this.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnGui()
        {
            if (Application.isPlaying)
            {
                GUI.enabled = false;
            }

            foreach (var list in this._installersLists)
            {
                list.DoLayoutList();
            }

            GUI.enabled = true;
        }
    }
}

