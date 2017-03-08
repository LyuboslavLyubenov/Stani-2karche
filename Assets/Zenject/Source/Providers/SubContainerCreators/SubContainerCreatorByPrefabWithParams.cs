#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.SubContainerCreators
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Install.Contexts;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers.PrefabProviders;

    public class SubContainerCreatorByPrefabWithParams : ISubContainerCreator
    {
        readonly DiContainer _container;
        readonly IPrefabProvider _prefabProvider;
        readonly Type _installerType;
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public SubContainerCreatorByPrefabWithParams(
            Type installerType, DiContainer container, IPrefabProvider prefabProvider,
            GameObjectCreationParameters gameObjectBindInfo)
        {
            this._gameObjectBindInfo = gameObjectBindInfo;
            this._prefabProvider = prefabProvider;
            this._container = container;
            this._installerType = installerType;
        }

        protected DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        DiContainer CreateTempContainer(List<TypeValuePair> args)
        {
            var tempSubContainer = this.Container.CreateSubContainer();

            foreach (var argPair in args)
            {
                tempSubContainer.Bind(argPair.Type)
                    .FromInstance(argPair.Value, true).WhenInjectedInto(this._installerType);
            }

            return tempSubContainer;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args)
        {
            Assert.That(!args.IsEmpty());

            var prefab = this._prefabProvider.GetPrefab();
            var gameObject = this.CreateTempContainer(args).InstantiatePrefab(
                prefab, new object[0], this._gameObjectBindInfo);

            var context = gameObject.GetComponent<GameObjectContext>();

            Assert.IsNotNull(context,
                "Expected prefab with name '{0}' to container a component of type 'GameObjectContext'", prefab.name);

            return context.Container;
        }
    }
}

#endif

