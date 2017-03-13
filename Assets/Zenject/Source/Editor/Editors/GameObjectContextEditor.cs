namespace Assets.Zenject.Source.Editor.Editors
{

    using Assets.Zenject.Source.Install.Contexts;

    using UnityEditor;

    [CustomEditor(typeof(GameObjectContext))]
    public class GameObjectContextEditor : ContextEditor
    {
        SerializedProperty _kernel;

        public override void OnEnable()
        {
            base.OnEnable();

            this._kernel = this.serializedObject.FindProperty("_kernel");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(this._kernel);
        }
    }
}
