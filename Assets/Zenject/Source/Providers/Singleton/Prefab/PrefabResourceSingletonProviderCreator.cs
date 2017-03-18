#if !NOT_UNITY3D

namespace Zenject.Source.Providers.Singleton.Prefab
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers.ComponentProviders;
    using Zenject.Source.Providers.GameObjectProviders;
    using Zenject.Source.Providers.PrefabCreators;
    using Zenject.Source.Providers.PrefabProviders;

    public class PrefabResourceSingletonProviderCreator
    {
        readonly SingletonMarkRegistry _markRegistry;
        readonly DiContainer _container;
        readonly Dictionary<PrefabId, IPrefabInstantiator> _prefabCreators =
            new Dictionary<PrefabId, IPrefabInstantiator>();

        public PrefabResourceSingletonProviderCreator(
            DiContainer container,
            SingletonMarkRegistry markRegistry)
        {
            this._markRegistry = markRegistry;
            this._container = container;
        }

        public IProvider CreateProvider(
            string resourcePath, Type resultType, GameObjectCreationParameters gameObjectBindInfo,
            List<TypeValuePair> extraArguments, object concreteIdentifier)
        {
            IPrefabInstantiator creator;

            this._markRegistry.MarkSingleton(
                resultType, concreteIdentifier, SingletonTypes.ToPrefabResource);

            var prefabId = new PrefabId(concreteIdentifier, resourcePath);

            if (this._prefabCreators.TryGetValue(prefabId, out creator))
            {
                // TODO: Check the arguments are the same?
                Assert.That(creator.ExtraArguments.IsEmpty() && extraArguments.IsEmpty(),
                    "Ambiguous creation parameters (arguments) when using ToPrefabResource with AsSingle");

                Assert.IsEqual(creator.GameObjectCreationParameters, gameObjectBindInfo,
                    "Ambiguous creation parameters (gameObject name/parent info) when using ToPrefabResource with AsSingle");
            }
            else
            {
                creator = new PrefabInstantiatorCached(
                    new PrefabInstantiator(
                        this._container, gameObjectBindInfo,
                        extraArguments,
                        new PrefabProviderResource(resourcePath)));

                this._prefabCreators.Add(prefabId, creator);
            }

            if (resultType == typeof(GameObject))
            {
                return new PrefabGameObjectProvider(creator);
            }

            return new GetFromPrefabComponentProvider(resultType, creator);
        }

        class PrefabId : IEquatable<PrefabId>
        {
            public readonly object ConcreteIdentifier;
            public readonly string ResourcePath;

            public PrefabId(object concreteIdentifier, string resourcePath)
            {
                Assert.IsNotNull(resourcePath);

                this.ConcreteIdentifier = concreteIdentifier;
                this.ResourcePath = resourcePath;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 29 + (this.ConcreteIdentifier == null ? 0 : this.ConcreteIdentifier.GetHashCode());
                    hash = hash * 29 + this.ResourcePath.GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object other)
            {
                if (other is PrefabId)
                {
                    PrefabId otherId = (PrefabId)other;
                    return otherId == this;
                }
                else
                {
                    return false;
                }
            }

            public bool Equals(PrefabId that)
            {
                return this == that;
            }

            public static bool operator ==(PrefabId left, PrefabId right)
            {
                return object.Equals(left.ResourcePath, right.ResourcePath)
                    && object.Equals(left.ConcreteIdentifier, right.ConcreteIdentifier);
            }

            public static bool operator !=(PrefabId left, PrefabId right)
            {
                return !left.Equals(right);
            }
        }
    }
}

#endif

