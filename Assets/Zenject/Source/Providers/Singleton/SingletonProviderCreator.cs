namespace Assets.Zenject.Source.Providers.Singleton
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers.Singleton.Prefab;
    using Assets.Zenject.Source.Providers.Singleton.Standard;
    using Assets.Zenject.Source.Providers.Singleton.SubContainer;

    public class SingletonProviderCreator
    {
        readonly StandardSingletonProviderCreator _standardProviderCreator;
        readonly SubContainerSingletonProviderCreatorByMethod _subContainerMethodProviderCreator;
        readonly SubContainerSingletonProviderCreatorByInstaller _subContainerInstallerProviderCreator;

#if !NOT_UNITY3D
        readonly SubContainerSingletonProviderCreatorByPrefab _subContainerPrefabProviderCreator;
        readonly SubContainerSingletonProviderCreatorByPrefabResource _subContainerPrefabResourceProviderCreator;

        readonly PrefabSingletonProviderCreator _prefabProviderCreator;
        readonly PrefabResourceSingletonProviderCreator _prefabResourceProviderCreator;
#endif
        public SingletonProviderCreator(
            DiContainer container, SingletonMarkRegistry markRegistry)
        {
            this._standardProviderCreator = new StandardSingletonProviderCreator(container, markRegistry);

            this._subContainerMethodProviderCreator = new SubContainerSingletonProviderCreatorByMethod(container, markRegistry);
            this._subContainerInstallerProviderCreator = new SubContainerSingletonProviderCreatorByInstaller(container, markRegistry);

#if !NOT_UNITY3D
            this._subContainerPrefabProviderCreator = new SubContainerSingletonProviderCreatorByPrefab(container, markRegistry);
            this._subContainerPrefabResourceProviderCreator = new SubContainerSingletonProviderCreatorByPrefabResource(container, markRegistry);

            this._prefabProviderCreator = new PrefabSingletonProviderCreator(container, markRegistry);
            this._prefabResourceProviderCreator = new PrefabResourceSingletonProviderCreator(container, markRegistry);
#endif
        }

        public IProvider CreateProviderStandard(
            StandardSingletonDeclaration dec, Func<DiContainer, Type, IProvider> providerCreator)
        {
            return this._standardProviderCreator.GetOrCreateProvider(dec, providerCreator);
        }

        public IProvider CreateProviderForSubContainerMethod(
            Type resultType, object concreteIdentifier,
            Action<DiContainer> installMethod, object identifier)
        {
            return this._subContainerMethodProviderCreator.CreateProvider(
                resultType, concreteIdentifier, installMethod, identifier);
        }

        public IProvider CreateProviderForSubContainerInstaller(
            Type resultType, object concreteIdentifier,
            Type installerType, object identifier)
        {
            return this._subContainerInstallerProviderCreator.CreateProvider(
                resultType, concreteIdentifier, installerType, identifier);
        }

#if !NOT_UNITY3D
        public IProvider CreateProviderForPrefab(
            UnityEngine.Object prefab, Type resultType, GameObjectCreationParameters gameObjectBindInfo,
            List<TypeValuePair> extraArguments, object concreteIdentifier)
        {
            return this._prefabProviderCreator.CreateProvider(
                prefab, resultType, gameObjectBindInfo,
                extraArguments, concreteIdentifier);
        }

        public IProvider CreateProviderForPrefabResource(
            string resourcePath, Type resultType, GameObjectCreationParameters gameObjectBindInfo,
            List<TypeValuePair> extraArguments, object concreteIdentifier)
        {
            return this._prefabResourceProviderCreator.CreateProvider(
                resourcePath, resultType, gameObjectBindInfo,
                extraArguments, concreteIdentifier);
        }

        public IProvider CreateProviderForSubContainerPrefab(
            Type resultType, object concreteIdentifier, GameObjectCreationParameters gameObjectBindInfo,
            UnityEngine.Object prefab, object identifier)
        {
            return this._subContainerPrefabProviderCreator.CreateProvider(
                resultType, concreteIdentifier, prefab, identifier, gameObjectBindInfo);
        }

        public IProvider CreateProviderForSubContainerPrefabResource(
            Type resultType, object concreteIdentifier, GameObjectCreationParameters gameObjectBindInfo,
            string resourcePath, object identifier)
        {
            return this._subContainerPrefabResourceProviderCreator.CreateProvider(
                resultType, concreteIdentifier, resourcePath, identifier, gameObjectBindInfo);
        }
#endif
    }
}
