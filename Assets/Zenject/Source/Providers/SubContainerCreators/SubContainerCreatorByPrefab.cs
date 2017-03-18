#if !NOT_UNITY3D

namespace Zenject.Source.Providers.SubContainerCreators
{

    using System.Collections.Generic;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Injection;
    using Zenject.Source.Install.Contexts;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers.PrefabProviders;

    public class SubContainerCreatorByPrefab : ISubContainerCreator
    {
        readonly GameObjectCreationParameters _gameObjectBindInfo;
        readonly IPrefabProvider _prefabProvider;
        readonly DiContainer _container;

        public SubContainerCreatorByPrefab(
            DiContainer container, IPrefabProvider prefabProvider,
            GameObjectCreationParameters gameObjectBindInfo)
        {
            this._gameObjectBindInfo = gameObjectBindInfo;
            this._prefabProvider = prefabProvider;
            this._container = container;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args)
        {
            Assert.That(args.IsEmpty());

            var prefab = this._prefabProvider.GetPrefab();
            var gameObject = this._container.InstantiatePrefab(
                prefab, new object[0], this._gameObjectBindInfo);

            var context = gameObject.GetComponent<GameObjectContext>();

            Assert.IsNotNull(context,
                "Expected prefab with name '{0}' to container a component of type 'GameObjectContext'", prefab.name);

            return context.Container;
        }
    }
}

#endif
