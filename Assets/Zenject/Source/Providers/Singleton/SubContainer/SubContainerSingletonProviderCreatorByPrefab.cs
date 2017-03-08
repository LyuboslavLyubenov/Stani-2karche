#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Providers.Singleton.SubContainer
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers.PrefabProviders;
    using Assets.Zenject.Source.Providers.SubContainerCreators;
    using Assets.Zenject.Source.Util;

    public class SubContainerSingletonProviderCreatorByPrefab
    {
        readonly SingletonMarkRegistry _markRegistry;
        readonly DiContainer _container;
        readonly Dictionary<CustomSingletonId, CreatorInfo> _subContainerCreators =
            new Dictionary<CustomSingletonId, CreatorInfo>();

        public SubContainerSingletonProviderCreatorByPrefab(
            DiContainer container,
            SingletonMarkRegistry markRegistry)
        {
            this._markRegistry = markRegistry;
            this._container = container;
        }

        public IProvider CreateProvider(
            Type resultType, object concreteIdentifier, UnityEngine.Object prefab, object identifier,
            GameObjectCreationParameters gameObjectBindInfo)
        {
            this._markRegistry.MarkSingleton(
                resultType, concreteIdentifier,
                SingletonTypes.ToSubContainerPrefab);

            var customSingletonId = new CustomSingletonId(
                concreteIdentifier, prefab);

            CreatorInfo creatorInfo;

            if (this._subContainerCreators.TryGetValue(customSingletonId, out creatorInfo))
            {
                Assert.IsEqual(creatorInfo.GameObjectCreationParameters, gameObjectBindInfo,
                    "Ambiguous creation parameters (game object name/parent info) when using ToSubContainerPrefab with AsSingle");
            }
            else
            {
                var creator = new SubContainerCreatorCached(
                    new SubContainerCreatorByPrefab(this._container, new PrefabProvider(prefab), gameObjectBindInfo));

                creatorInfo = new CreatorInfo(gameObjectBindInfo, creator);

                this._subContainerCreators.Add(customSingletonId, creatorInfo);
            }

            return new SubContainerDependencyProvider(
                resultType, identifier, creatorInfo.Creator);
        }

        class CustomSingletonId : IEquatable<CustomSingletonId>
        {
            public readonly object ConcreteIdentifier;
            public readonly UnityEngine.Object Prefab;

            public CustomSingletonId(object concreteIdentifier, UnityEngine.Object prefab)
            {
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
                if (other is CustomSingletonId)
                {
                    CustomSingletonId otherId = (CustomSingletonId)other;
                    return otherId == this;
                }
                else
                {
                    return false;
                }
            }

            public bool Equals(CustomSingletonId that)
            {
                return this == that;
            }

            public static bool operator ==(CustomSingletonId left, CustomSingletonId right)
            {
                return object.Equals(left.Prefab, right.Prefab)
                    && object.Equals(left.ConcreteIdentifier, right.ConcreteIdentifier);
            }

            public static bool operator !=(CustomSingletonId left, CustomSingletonId right)
            {
                return !left.Equals(right);
            }
        }

        class CreatorInfo
        {
            public CreatorInfo(
                GameObjectCreationParameters gameObjectBindInfo, ISubContainerCreator creator)
            {
                this.GameObjectCreationParameters = gameObjectBindInfo;
                this.Creator = creator;
            }

            public GameObjectCreationParameters GameObjectCreationParameters
            {
                get;
                private set;
            }

            public ISubContainerCreator Creator
            {
                get;
                private set;
            }
        }
    }
}

#endif
