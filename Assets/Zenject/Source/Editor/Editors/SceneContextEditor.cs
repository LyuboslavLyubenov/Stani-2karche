namespace Zenject.Source.Editor.Editors
{

    using UnityEditor;

    using Zenject.Source.Install.Contexts;

    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : ContextEditor
    {
        SerializedProperty _contractNameProperty;
        SerializedProperty _parentContractNameProperty;
        SerializedProperty _parentNewObjectsUnderRootProperty;
        SerializedProperty _autoRun;

        public override void OnEnable()
        {
            base.OnEnable();

            this._contractNameProperty = this.serializedObject.FindProperty("_contractNames");
            this._parentContractNameProperty = this.serializedObject.FindProperty("_parentContractName");
            this._parentNewObjectsUnderRootProperty = this.serializedObject.FindProperty("_parentNewObjectsUnderRoot");
            this._autoRun = this.serializedObject.FindProperty("_autoRun");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(this._contractNameProperty, true);
            EditorGUILayout.PropertyField(this._parentContractNameProperty);
            EditorGUILayout.PropertyField(this._parentNewObjectsUnderRootProperty);
            EditorGUILayout.PropertyField(this._autoRun);
        }
    }
}

