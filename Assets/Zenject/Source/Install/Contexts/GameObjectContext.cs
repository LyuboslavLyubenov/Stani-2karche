#if !NOT_UNITY3D


#pragma warning disable 649

namespace Zenject.Source.Install.Contexts
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.Serialization;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Runtime.Kernels;
    using Zenject.Source.Usage;
    using Zenject.Source.Util;

    public class GameObjectContext : Context
    {
        readonly List<object> _dependencyRoots = new List<object>();

        [SerializeField]
        [Tooltip("Note that this field is optional and can be ignored in most cases.  This is really only needed if you want to control the 'Script Execution Order' of your subcontainer.  In this case, define a new class that derives from MonoKernel, add it to this game object, then drag it into this field.  Then you can set a value for 'Script Execution Order' for this new class and this will control when all ITickable/IInitializable classes bound within this subcontainer get called.")]
        [FormerlySerializedAs("_facade")]
        MonoKernel _kernel;

        DiContainer _container;

        public override DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        [Inject]
        public void Construct(
            DiContainer parentContainer,
            [InjectOptional]
            InstallerExtraArgs installerExtraArgs)
        {
            Assert.IsNull(this._container);

            this._container = parentContainer.CreateSubContainer();

            this._container.LazyInstanceInjector
                .AddInstances(this.GetInjectableComponents().Cast<object>());

            foreach (var instance in this._container.LazyInstanceInjector.Instances)
            {
                if (instance is MonoKernel)
                {
                    Assert.That(ReferenceEquals(instance, this._kernel),
                        "Found MonoKernel derived class that is not hooked up to GameObjectContext.  If you use MonoKernel, you must indicate this to GameObjectContext by dragging and dropping it to the Kernel field in the inspector");
                }
            }

            this._container.IsInstalling = true;

            try
            {
                this.InstallBindings(installerExtraArgs);
            }
            finally
            {
                this._container.IsInstalling = false;
            }

            Log.Debug("GameObjectContext: Injecting into child components...");

            this._container.LazyInstanceInjector.LazyInjectAll();

            Assert.That(this._dependencyRoots.IsEmpty());
            this._dependencyRoots.AddRange(this._container.ResolveDependencyRoots());

            Log.Debug("GameObjectContext: Initialized successfully");
        }

        protected override IEnumerable<Component> GetInjectableComponents()
        {
            // We inject on all components on the root except ourself
            foreach (var component in this.GetComponents<Component>())
            {
                if (component == null)
                {
                    Log.Warn("Zenject: Found null component on game object '{0}'.  Possible missing script.", this.gameObject.name);
                    continue;
                }

                if (component.GetType().DerivesFrom<MonoInstaller>())
                {
                    // Do not inject on installers since these are always injected before they are installed
                    continue;
                }

                if (component == this)
                {
                    continue;
                }

                yield return component;
            }

            foreach (var gameObject in UnityUtil.GetDirectChildren(this.gameObject))
            {
                foreach (var component in ContextUtil.GetInjectableComponents(gameObject))
                {
                    yield return component;
                }
            }
        }

        void InstallBindings(
            InstallerExtraArgs installerExtraArgs)
        {
            this._container.DefaultParent = this.transform;

            this._container.Bind<Context>().FromInstance(this);

            if (this._kernel == null)
            {
                this._container.Bind<MonoKernel>()
                    .To<DefaultGameObjectKernel>().FromComponent(this.gameObject).AsSingle().NonLazy();
            }
            else
            {
                this._container.Bind<MonoKernel>().FromInstance(this._kernel).AsSingle().NonLazy();
            }

            this.InstallSceneBindings();

            var extraArgsMap = new Dictionary<Type, List<TypeValuePair>>();

            if (installerExtraArgs != null)
            {
                extraArgsMap.Add(
                    installerExtraArgs.InstallerType, installerExtraArgs.ExtraArgs);
            }

            this.InstallInstallers();
        }

        public class InstallerExtraArgs
        {
            public Type InstallerType;
            public List<TypeValuePair> ExtraArgs;
        }
    }
}

#endif
