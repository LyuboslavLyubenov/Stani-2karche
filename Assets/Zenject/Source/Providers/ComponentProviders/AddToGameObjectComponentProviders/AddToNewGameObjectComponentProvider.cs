#if !NOT_UNITY3D

namespace Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;

    public class AddToNewGameObjectComponentProvider : AddToGameObjectComponentProviderBase
    {
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public AddToNewGameObjectComponentProvider(
            DiContainer container, Type componentType,
            object concreteIdentifier, List<TypeValuePair> extraArguments, GameObjectCreationParameters gameObjectBindInfo)
            : base(container, componentType, concreteIdentifier, extraArguments)
        {
            this._gameObjectBindInfo = gameObjectBindInfo;
        }

        protected override GameObject GetGameObject(InjectContext context)
        {
            if (this._gameObjectBindInfo.Name == null)
            {
                this._gameObjectBindInfo.Name = this.ConcreteIdentifier as string ?? this.ComponentType.Name();
            }

            return this.Container.CreateEmptyGameObject(this._gameObjectBindInfo);
        }
    }
}

#endif
