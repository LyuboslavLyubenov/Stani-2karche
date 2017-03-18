#if !NOT_UNITY3D

namespace Zenject.Source.Providers.GameObjectProviders
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;

    public class EmptyGameObjectProvider : IProvider
    {
        readonly DiContainer _container;
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public EmptyGameObjectProvider(
            DiContainer container, GameObjectCreationParameters gameObjectBindInfo)
        {
            this._gameObjectBindInfo = gameObjectBindInfo;
            this._container = container;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(GameObject);
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);

            yield return new List<object>()
            {
                this._container.CreateEmptyGameObject(this._gameObjectBindInfo)
            };
        }
    }
}

#endif

