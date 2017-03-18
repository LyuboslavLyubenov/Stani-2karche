#if !NOT_UNITY3D

namespace Zenject.Source.Install.Contexts
{

    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.Serialization;

    using Zenject.Source.Internal;
    using Zenject.Source.Main;

    public class SceneDecoratorContext : Context
    {
        [FormerlySerializedAs("SceneName")]
        [SerializeField]
        string _decoratedContractName = null;

        DiContainer _container;

        public string DecoratedContractName
        {
            get
            {
                return this._decoratedContractName;
            }
        }

        public override DiContainer Container
        {
            get
            {
                Assert.IsNotNull(this._container);
                return this._container;
            }
        }

        public void Initialize(DiContainer container)
        {
            Assert.IsNull(this._container);
            this._container = container;

            container.LazyInstanceInjector
                .AddInstances(this.GetInjectableComponents().Cast<object>());
        }

        public void InstallDecoratorSceneBindings()
        {
            this._container.Bind<SceneDecoratorContext>().FromInstance(this);
            this.InstallSceneBindings();
        }

        public void InstallDecoratorInstallers()
        {
            this.InstallInstallers();
        }

        protected override IEnumerable<Component> GetInjectableComponents()
        {
            return ContextUtil.GetInjectableComponents(this.gameObject.scene);
        }
    }
}

#endif
