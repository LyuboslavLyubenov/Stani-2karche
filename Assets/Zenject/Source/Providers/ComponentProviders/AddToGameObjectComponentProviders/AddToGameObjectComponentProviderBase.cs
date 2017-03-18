#if !NOT_UNITY3D

namespace Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Validation;

    public abstract class AddToGameObjectComponentProviderBase : IProvider
    {
        readonly object _concreteIdentifier;
        readonly Type _componentType;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArguments;

        public AddToGameObjectComponentProviderBase(
            DiContainer container, Type componentType,
            object concreteIdentifier, List<TypeValuePair> extraArguments)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            this._concreteIdentifier = concreteIdentifier;
            this._extraArguments = extraArguments;
            this._componentType = componentType;
            this._container = container;
        }

        protected DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        protected Type ComponentType
        {
            get
            {
                return this._componentType;
            }
        }

        protected object ConcreteIdentifier
        {
            get
            {
                return this._concreteIdentifier;
            }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._componentType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsNotNull(context);

            object instance;

            // We still want to make sure we can get the game object during validation
            var gameObj = this.GetGameObject(context);

            if (!this._container.IsValidating || DiContainer.CanCreateOrInjectDuringValidation(this._componentType))
            {
                instance = gameObj.AddComponent(this._componentType);
            }
            else
            {
                instance = new ValidationMarker(this._componentType);
            }

            // Note that we don't just use InstantiateComponentOnNewGameObjectExplicit here
            // because then circular references don't work
            yield return new List<object>() { instance };

            var injectArgs = new InjectArgs()
            {
                ExtraArgs = this._extraArguments.Concat(args).ToList(),
                UseAllArgs = true,
                Context = context,
                ConcreteIdentifier = this._concreteIdentifier,
            };

            this._container.InjectExplicit(instance, this._componentType, injectArgs);

            Assert.That(injectArgs.ExtraArgs.IsEmpty());
        }

        protected abstract GameObject GetGameObject(InjectContext context);
    }
}

#endif
