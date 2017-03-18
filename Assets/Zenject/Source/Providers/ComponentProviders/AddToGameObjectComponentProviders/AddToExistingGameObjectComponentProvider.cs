#if !NOT_UNITY3D

namespace Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Injection;
    using Zenject.Source.Main;

    public class AddToExistingGameObjectComponentProvider : AddToGameObjectComponentProviderBase
    {
        readonly GameObject _gameObject;

        public AddToExistingGameObjectComponentProvider(
            GameObject gameObject, DiContainer container, Type componentType,
            object concreteIdentifier, List<TypeValuePair> extraArguments)
            : base(container, componentType, concreteIdentifier, extraArguments)
        {
            this._gameObject = gameObject;
        }

        protected override GameObject GetGameObject(InjectContext context)
        {
            return this._gameObject;
        }
    }
}

#endif
