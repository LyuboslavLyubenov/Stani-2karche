#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.ComponentProviders.AddToGameObjectComponentProviders
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Validation;

    using UnityEngine;

    public class AddToCurrentGameObjectComponentProvider : IProvider
    {
        readonly object _concreteIdentifier;
        readonly Type _componentType;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArguments;

        public AddToCurrentGameObjectComponentProvider(
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

            Assert.That(context.ObjectType.DerivesFrom<Component>(),
                "Object '{0}' can only be injected into MonoBehaviour's since it was bound with 'FromSiblingComponent'. Attempted to inject into non-MonoBehaviour '{1}'",
                context.MemberType, context.ObjectType);

            object instance;

            if (!this._container.IsValidating || DiContainer.CanCreateOrInjectDuringValidation(this._componentType))
            {
                var gameObj = ((Component)context.ObjectInstance).gameObject;

                instance = gameObj.GetComponent(this._componentType);

                if (instance != null)
                {
                    yield return new List<object>() { instance };
                    yield break;
                }

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
    }
}

#endif
