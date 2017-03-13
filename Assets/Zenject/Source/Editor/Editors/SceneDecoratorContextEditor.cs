namespace Assets.Zenject.Source.Editor.Editors
{

    using Assets.Zenject.Source.Install.Contexts;

    using UnityEditor;

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
