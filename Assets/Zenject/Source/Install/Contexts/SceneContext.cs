#if !NOT_UNITY3D

namespace Zenject.Source.Install.Contexts
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Serialization;

    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Runtime.Kernels;
    using Zenject.Source.Util;

    public class SceneContext : Context
    {
        public static Action<DiContainer> ExtraBindingsInstallMethod;

        public static DiContainer ParentContainer;

        [FormerlySerializedAs("ParentNewObjectsUnderRoot")]
        [Tooltip("When true, objects that are created at runtime will be parented to the SceneContext")]
        [SerializeField]
        bool _parentNewObjectsUnderRoot = false;

        [Tooltip("Optional contract names for this SceneContext, allowing contexts in subsequently loaded scenes to depend on it and be parented to it, and also for previously loaded decorators to be included")]
        [SerializeField]
        List<string> _contractNames = new List<string>();

        [Tooltip("Optional contract name of a SceneContext in a previously loaded scene that this context depends on and to which it must be parented")]
        [SerializeField]
        string _parentContractName;

        [Tooltip("When false, wait until run method is explicitly called. Otherwise run on awake")]
        [SerializeField]
        bool _autoRun = true;

        DiContainer _container;
        readonly List<object> _dependencyRoots = new List<object>();

        readonly List<SceneDecoratorContext> _decoratorContexts = new List<SceneDecoratorContext>();

        bool _hasInstalled;
        bool _hasResolved;

        static bool _staticAutoRun = true;

        public override DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        public bool IsValidating
        {
            get
            {
#if UNITY_EDITOR
                return ProjectContext.Instance.Container.IsValidating;
#else
                return false;
#endif
            }
        }

        public IEnumerable<string> ContractNames
        {
            get
            {
                return this._contractNames;
            }
            set
            {
                this._contractNames.Clear();
                this._contractNames.AddRange(value);
            }
        }

        public string ParentContractName
        {
            get
            {
                return this._parentContractName;
            }
            set
            {
                this._parentContractName = value;
            }
        }

        public bool ParentNewObjectsUnderRoot
        {
            get
            {
                return this._parentNewObjectsUnderRoot;
            }
            set
            {
                this._parentNewObjectsUnderRoot = value;
            }
        }

        public void Awake()
        {
            // We always want to initialize ProjectContext as early as possible
            ProjectContext.Instance.EnsureIsInitialized();

            if (_staticAutoRun && this._autoRun)
            {
                this.Run();
            }
            else
            {
                // True should always be default
                _staticAutoRun = true;
            }
        }

#if UNITY_EDITOR
        public void Validate()
        {
            Assert.That(this.IsValidating);

            this.Install();
            this.Resolve();

            Assert.That(this._container.IsValidating);

            this._container.ValidateIValidatables();
        }
#endif

        public void Run()
        {
            Assert.That(!this.IsValidating);
            this.Install();
            this.Resolve();
        }

        IEnumerable<Scene> LoadedScenes
        {
            get
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    yield return SceneManager.GetSceneAt(i);
                }
            }
        }

        DiContainer GetParentContainer()
        {
            if (string.IsNullOrEmpty(this._parentContractName))
            {
                if (ParentContainer != null)
                {
                    var tempParentContainer = ParentContainer;

                    // Always reset after using it - it is only used to pass the reference
                    // between scenes via ZenjectSceneLoader
                    ParentContainer = null;

                    return tempParentContainer;
                }

                return ProjectContext.Instance.Container;
            }

            Assert.IsNull(ParentContainer,
                "Scene cannot have both a parent scene context name set and also an explicit parent container given");

            var sceneContexts = this.LoadedScenes
                .Where(scene => scene.isLoaded)
                .Except(this.gameObject.scene)
                .SelectMany(scene => scene.GetRootGameObjects())
                .SelectMany(root => root.GetComponentsInChildren<SceneContext>())
                .Where(sceneContext => sceneContext.ContractNames.Contains(this._parentContractName))
                .ToList();

            Assert.That(sceneContexts.Any(), () => string.Format(
                "SceneContext on object {0} of scene {1} requires contract {2}, but none of the loaded SceneContexts implements that contract.",
                this.gameObject.name,
                this.gameObject.scene.name,
                this._parentContractName));

            Assert.That(sceneContexts.Count == 1, () => string.Format(
                "SceneContext on object {0} of scene {1} requires a single implementation of contract {2}, but multiple were found.",
                this.gameObject.name,
                this.gameObject.scene.name,
                this._parentContractName));

            return sceneContexts.Single().Container;
        }

        List<SceneDecoratorContext> LookupDecoratorContexts()
        {
            if (this._contractNames.IsEmpty())
            {
                return new List<SceneDecoratorContext>();
            }

            return this.LoadedScenes
                .Where(scene => scene.isLoaded)
                .Except(this.gameObject.scene)
                .SelectMany(scene => scene.GetRootGameObjects())
                .SelectMany(root => root.GetComponentsInChildren<SceneDecoratorContext>())
                .Where(decoratorContext => this._contractNames.Contains(decoratorContext.DecoratedContractName))
                .ToList();
        }

        public void Install()
        {
#if !UNITY_EDITOR
            Assert.That(!IsValidating);
#endif

            Assert.That(!this._hasInstalled);
            this._hasInstalled = true;

            Assert.IsNull(this._container);
            this._container = this.GetParentContainer().CreateSubContainer();

            Assert.That(this._decoratorContexts.IsEmpty());
            this._decoratorContexts.AddRange(this.LookupDecoratorContexts());

            Log.Debug("SceneContext: Running installers...");

            if (this._parentNewObjectsUnderRoot)
            {
                this._container.DefaultParent = this.transform;
            }
            else
            {
                // This is necessary otherwise we inherit the project root DefaultParent
                this._container.DefaultParent = null;
            }

            // Record all the injectable components in the scene BEFORE installing the installers
            // This is nice for cases where the user calls InstantiatePrefab<>, etc. in their installer
            // so that it doesn't inject on the game object twice
            // InitialComponentsInjecter will also guarantee that any component that is injected into
            // another component has itself been injected
            this._container.LazyInstanceInjector
                .AddInstances(this.GetInjectableComponents().Cast<object>());

            foreach (var decoratorContext in this._decoratorContexts)
            {
                decoratorContext.Initialize(this._container);
            }

            this._container.IsInstalling = true;

            try
            {
                this.InstallBindings();
            }
            finally
            {
                this._container.IsInstalling = false;
            }
        }

        public void Resolve()
        {
            Log.Debug("SceneContext: Injecting components in the scene...");

            Assert.That(this._hasInstalled);
            Assert.That(!this._hasResolved);
            this._hasResolved = true;

            this._container.LazyInstanceInjector.LazyInjectAll();

            Log.Debug("SceneContext: Resolving dependency roots...");

            Assert.That(this._dependencyRoots.IsEmpty());
            this._dependencyRoots.AddRange(this._container.ResolveDependencyRoots());

            Log.Debug("SceneContext: Initialized successfully");
        }

        void InstallBindings()
        {
            this._container.Bind<Context>().FromInstance(this);
            this._container.Bind<SceneContext>().FromInstance(this);

            foreach (var decoratorContext in this._decoratorContexts)
            {
                decoratorContext.InstallDecoratorSceneBindings();
            }

            this.InstallSceneBindings();

            this._container.Bind<SceneKernel>().FromComponent(this.gameObject).AsSingle().NonLazy();

            this._container.Bind<ZenjectSceneLoader>().AsSingle();

            if (ExtraBindingsInstallMethod != null)
            {
                ExtraBindingsInstallMethod(this._container);
                // Reset extra bindings for next time we change scenes
                ExtraBindingsInstallMethod = null;
            }

            // Always install the installers last so they can be injected with
            // everything above
            foreach (var decoratorContext in this._decoratorContexts)
            {
                decoratorContext.InstallDecoratorInstallers();
            }

            this.InstallInstallers();
        }

        protected override IEnumerable<Component> GetInjectableComponents()
        {
            return ContextUtil.GetInjectableComponents(this.gameObject.scene);
        }

        // These methods can be used for cases where you need to create the SceneContext entirely in code
        // Note that if you use these methods that you have to call Run() yourself
        // This is useful because it allows you to create a SceneContext and configure it how you want
        // and add what installers you want before kicking off the Install/Resolve
        public static SceneContext Create()
        {
            return CreateComponent(
                new GameObject("SceneContext"));
        }

        public static SceneContext CreateComponent(GameObject gameObject)
        {
            _staticAutoRun = false;
            var result = gameObject.AddComponent<SceneContext>();
            Assert.That(_staticAutoRun); // Should be reset
            return result;
        }
    }
}

#endif

