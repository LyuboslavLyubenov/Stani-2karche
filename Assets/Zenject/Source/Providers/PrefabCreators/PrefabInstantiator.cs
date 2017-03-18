#if !NOT_UNITY3D

namespace Zenject.Source.Providers.PrefabCreators
{

    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers.PrefabProviders;

    public class PrefabInstantiator : IPrefabInstantiator
    {
        readonly IPrefabProvider _prefabProvider;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArguments;
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public PrefabInstantiator(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
            List<TypeValuePair> extraArguments,
            IPrefabProvider prefabProvider)
        {
            this._prefabProvider = prefabProvider;
            this._extraArguments = extraArguments;
            this._container = container;
            this._gameObjectBindInfo = gameObjectBindInfo;
        }

        public GameObjectCreationParameters GameObjectCreationParameters
        {
            get
            {
                return this._gameObjectBindInfo;
            }
        }

        public List<TypeValuePair> ExtraArguments
        {
            get
            {
                return this._extraArguments;
            }
        }

        public UnityEngine.Object GetPrefab()
        {
            return this._prefabProvider.GetPrefab();
        }

        public IEnumerator<GameObject> Instantiate(List<TypeValuePair> args)
        {
            var gameObject = this._container.CreateAndParentPrefab(this.GetPrefab(), this._gameObjectBindInfo);
            Assert.IsNotNull(gameObject);

            // Return it before inject so we can do circular dependencies
            yield return gameObject;

            this._container.InjectGameObjectExplicit(
                gameObject, true, this._extraArguments.Concat(args).ToList());
        }
    }
}

#endif
