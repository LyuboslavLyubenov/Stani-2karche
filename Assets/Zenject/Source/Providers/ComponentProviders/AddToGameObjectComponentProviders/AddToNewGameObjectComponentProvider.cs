#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;

    using UnityEngine;

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
