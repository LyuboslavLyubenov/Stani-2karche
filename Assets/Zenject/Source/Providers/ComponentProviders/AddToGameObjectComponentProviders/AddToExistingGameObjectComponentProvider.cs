#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Main;

    using UnityEngine;

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
