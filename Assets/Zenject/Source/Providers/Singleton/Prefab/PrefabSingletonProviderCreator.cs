#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.Singleton.Prefab
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers.ComponentProviders;
    using Assets.Zenject.Source.Providers.GameObjectProviders;
    using Assets.Zenject.Source.Providers.PrefabCreators;
    using Assets.Zenject.Source.Providers.PrefabProviders;
    using Assets.Zenject.Source.Util;

    using UnityEngine;

    public class PrefabSingletonProviderCreator
    {
        readonly SingletonMarkRegistry _markRegistry;
        readonly DiContainer _container;
        readonly Dictionary<PrefabId, IPrefabInstantiator> _prefabCreators =
            new Dictionary<PrefabId, IPrefabInstantiator>();

        public PrefabSingletonProviderCreator(
            DiContainer container,
            SingletonMarkRegistry markRegistry)
        {
            this._markRegistry = markRegistry;
            this._container = container;
        }

        public IProvider CreateProvider(
            UnityEngine.Object prefab, Type resultType, GameObjectCreationParameters gameObjectBindInfo,
            List<TypeValuePair> extraArguments, object concreteIdentifier)
        {
            IPrefabInstantiator creator;

            var prefabId = new PrefabId(concreteIdentifier, prefab);

            this._markRegistry.MarkSingleton(
                resultType, concreteIdentifier, SingletonTypes.ToPrefab);

            if (this._prefabCreators.TryGetValue(prefabId, out creator))
            {
                // TODO: Check the arguments are the same?
                Assert.That(creator.ExtraArguments.IsEmpty() && extraArguments.IsEmpty(),
                    "Ambiguous creation parameters (arguments) when using ToPrefab with AsSingle");

                Assert.IsEqual(creator.GameObjectCreationParameters, gameObjectBindInfo,
                    "Ambiguous creation parameters (game object naming/parent info) when using ToPrefab with AsSingle");
            }
            else
            {
                creator = new PrefabInstantiatorCached(
                    new PrefabInstantiator(
                        this._container, gameObjectBindInfo, extraArguments, new PrefabProvider(prefab)));

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
            public readonly UnityEngine.Object Prefab;

            public PrefabId(object concreteIdentifier, UnityEngine.Object prefab)
            {
                Assert.IsNotNull(prefab);

                this.ConcreteIdentifier = concreteIdentifier;
                this.Prefab = prefab;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 29 + (this.ConcreteIdentifier == null ? 0 : this.ConcreteIdentifier.GetHashCode());
                    hash = hash * 29 + (ZenUtilInternal.IsNull(this.Prefab) ? 0 : this.Prefab.GetHashCode());
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
                return object.Equals(left.Prefab, right.Prefab) && object.Equals(left.ConcreteIdentifier, right.ConcreteIdentifier);
            }

            public static bool operator !=(PrefabId left, PrefabId right)
            {
                return !left.Equals(right);
            }
        }
    }
}

#endif
