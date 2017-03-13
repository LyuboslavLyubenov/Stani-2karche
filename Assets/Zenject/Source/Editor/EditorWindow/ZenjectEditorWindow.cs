namespace Assets.Zenject.Source.Editor.EditorWindow
{

    using System;

    using Assets.Zenject.Source.Install.Contexts;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Runtime;
    using Assets.Zenject.Source.Usage;

    using UnityEditor;

    public abstract class ZenjectEditorWindow : EditorWindow
    {
        [Inject]
        [NonSerialized]
        TickableManager _tickableManager = null;

        [Inject]
        [NonSerialized]
        InitializableManager _initializableManager = null;

        [Inject]
        [NonSerialized]
        DisposableManager _disposableManager = null;

        [Inject]
        [NonSerialized]
        GuiRenderableManager _guiRenderableManager = null;

        [NonSerialized]
        DiContainer _container;

        protected DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        public virtual void OnEnable()
        {
            this._container = new DiContainer(StaticContext.Container);

            // Make sure we don't create any game objects since editor windows don't have a scene
            this._container.AssertOnNewGameObjects = true;

            this._container.Bind<TickableManager>().AsSingle();
            this._container.Bind<InitializableManager>().AsSingle();
            this._container.Bind<DisposableManager>().AsSingle();
            this._container.Bind<GuiRenderableManager>().AsSingle();

            this.InstallBindings();

            this._container.Inject(this);

            this._initializableManager.Initialize();
        }

        public virtual void OnDisable()
        {
            if (this._disposableManager != null)
            {
                this._disposableManager.Dispose();
                this._disposableManager = null;
            }
        }

        public virtual void Update()
        {
            if (this._tickableManager != null)
            {
                this._tickableManager.Update();
            }

            // We might also consider only calling Repaint when changes occur
            this.Repaint();
        }

        public virtual void OnGUI()
        {
            if (this._guiRenderableManager != null)
            {
                this._guiRenderableManager.OnGui();
            }
        }

        public abstract void InstallBindings();
    }
}
