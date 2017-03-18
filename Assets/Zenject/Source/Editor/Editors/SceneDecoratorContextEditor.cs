namespace Zenject.Source.Editor.Editors
{

    using UnityEditor;

    using Zenject.Source.Install.Contexts;

    [CustomEditor(typeof(SceneDecoratorContext))]
    public class SceneDecoratorContextEditor : ContextEditor
    {
        SerializedProperty _decoratedContractNameProperty;

        public override void OnEnable()
        {
            base.OnEnable();

            this._decoratedContractNameProperty = this.serializedObject.FindProperty("_decoratedContractName");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(this._decoratedContractNameProperty);
        }
    }
}
