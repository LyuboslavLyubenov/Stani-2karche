#if !NOT_UNITY3D

namespace Zenject.Source.Binding.Finalizers
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.PrefabProviders;
    using Zenject.Source.Providers.SubContainerCreators;

    public class SubContainerPrefabResourceBindingFinalizer : ProviderBindingFinalizer
    {
        readonly string _resourcePath;
        readonly object _subIdentifier;
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public SubContainerPrefabResourceBindingFinalizer(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectBindInfo,
            string resourcePath,
            object subIdentifier)
            : base(bindInfo)
        {
            this._gameObjectBindInfo = gameObjectBindInfo;
            this._subIdentifier = subIdentifier;
            this._resourcePath = resourcePath;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            if (this.BindInfo.ToChoice == ToChoices.Self)
            {
                Assert.IsEmpty(this.BindInfo.ToTypes);
                this.FinalizeBindingSelf(container);
            }
            else
            {
                this.FinalizeBindingConcrete(container, this.BindInfo.ToTypes);
            }
        }

        void FinalizeBindingConcrete(DiContainer container, List<Type> concreteTypes)
        {
            switch (this.BindInfo.Scope)
            {
                case ScopeTypes.Singleton:
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) => container.SingletonProviderCreator.CreateProviderForSubContainerPrefabResource(
                            concreteType,
                            this.BindInfo.ConcreteIdentifier,
                            this._gameObjectBindInfo,
                            this._resourcePath,
                            this._subIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) => new SubContainerDependencyProvider(
                            concreteType, this._subIdentifier,
                            new SubContainerCreatorByPrefab(
                                container, new PrefabProviderResource(this._resourcePath), this._gameObjectBindInfo)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var containerCreator = new SubContainerCreatorCached(
                        new SubContainerCreatorByPrefab(container, new PrefabProviderResource(this._resourcePath), this._gameObjectBindInfo));

                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) =>
                        new SubContainerDependencyProvider(
                            concreteType, this._subIdentifier, containerCreator));
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }

        void FinalizeBindingSelf(DiContainer container)
        {
            switch (this.BindInfo.Scope)
            {
                case ScopeTypes.Singleton:
                {
                    this.RegisterProviderPerContract(
                        container,
                        (_, contractType) => container.SingletonProviderCreator.CreateProviderForSubContainerPrefabResource(
                            contractType,
                            this.BindInfo.ConcreteIdentifier,
                            this._gameObjectBindInfo,
                            this._resourcePath,
                            this._subIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProviderPerContract(
                        container,
                        (_, contractType) => new SubContainerDependencyProvider(
                            contractType, this._subIdentifier,
                            new SubContainerCreatorByPrefab(
                                container, new PrefabProviderResource(this._resourcePath), this._gameObjectBindInfo)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var containerCreator = new SubContainerCreatorCached(
                        new SubContainerCreatorByPrefab(
                            container, new PrefabProviderResource(this._resourcePath), this._gameObjectBindInfo));

                    this.RegisterProviderPerContract(
                        container,
                        (_, contractType) =>
                        new SubContainerDependencyProvider(
                            contractType, this._subIdentifier, containerCreator));
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }
    }
}

#endif
