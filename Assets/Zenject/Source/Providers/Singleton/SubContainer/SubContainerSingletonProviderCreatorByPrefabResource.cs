#if !NOT_UNITY3D

namespace Zenject.Source.Providers.Singleton.SubContainer
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers.PrefabProviders;
    using Zenject.Source.Providers.SubContainerCreators;

    public class SubContainerSingletonProviderCreatorByPrefabResource
    {
        readonly SingletonMarkRegistry _markRegistry;
        readonly DiContainer _container;
        readonly Dictionary<CustomSingletonId, CreatorInfo> _subContainerCreators =
            new Dictionary<CustomSingletonId, CreatorInfo>();

        public SubContainerSingletonProviderCreatorByPrefabResource(
            DiContainer container,
            SingletonMarkRegistry markRegistry)
        {
            this._markRegistry = markRegistry;
            this._container = container;
        }

        public IProvider CreateProvider(
            Type resultType, object concreteIdentifier, string resourcePath, object identifier, GameObjectCreationParameters gameObjectBindInfo)
        {
            this._markRegistry.MarkSingleton(
                resultType, concreteIdentifier,
                SingletonTypes.ToSubContainerPrefabResource);

            var customSingletonId = new CustomSingletonId(
                concreteIdentifier, resourcePath);

            CreatorInfo creatorInfo;

            if (this._subContainerCreators.TryGetValue(customSingletonId, out creatorInfo))
            {
                Assert.IsEqual(creatorInfo.GameObjectCreationParameters, gameObjectBindInfo,
                    "Ambiguous creation parameters (game object name / parent info) when using ToSubContainerPrefab with AsSingle");
            }
            else
            {
                var creator = new SubContainerCreatorCached(
                    new SubContainerCreatorByPrefab(
                        this._container, new PrefabProviderResource(resourcePath), gameObjectBindInfo));

                creatorInfo = new CreatorInfo(
                    gameObjectBindInfo, creator);

                this._subContainerCreators.Add(customSingletonId, creatorInfo);
            }

            return new SubContainerDependencyProvider(
                resultType, identifier, creatorInfo.Creator);
        }

        class CustomSingletonId : IEquatable<CustomSingletonId>
        {
            public readonly object ConcreteIdentifier;
            public readonly string ResourcePath;

            public CustomSingletonId(object concreteIdentifier, string resourcePath)
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
                return object.Equals(left.ResourcePath, right.ResourcePath)
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
                GameObjectCreationParameters creationParameters, ISubContainerCreator creator)
            {
                this.GameObjectCreationParameters = creationParameters;
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
