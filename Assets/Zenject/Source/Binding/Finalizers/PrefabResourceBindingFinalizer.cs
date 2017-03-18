#if !NOT_UNITY3D

namespace Zenject.Source.Binding.Finalizers
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.ComponentProviders;
    using Zenject.Source.Providers.GameObjectProviders;
    using Zenject.Source.Providers.PrefabCreators;
    using Zenject.Source.Providers.PrefabProviders;

    public class PrefabResourceBindingFinalizer : ProviderBindingFinalizer
    {
        readonly GameObjectCreationParameters _gameObjectBindInfo;
        readonly string _resourcePath;

        public PrefabResourceBindingFinalizer(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectBindInfo,
            string resourcePath)
            : base(bindInfo)
        {
            this._gameObjectBindInfo = gameObjectBindInfo;
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

        IProvider CreateProviderForType(
            Type contractType, IPrefabInstantiator instantiator)
        {
            if (contractType == typeof(GameObject))
            {
                return new PrefabGameObjectProvider(instantiator);
            }

            Assert.That(contractType.IsInterface() || contractType.DerivesFrom<Component>());

            return new GetFromPrefabComponentProvider(
                contractType, instantiator);
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
                        (_, concreteType) => container.SingletonProviderCreator.CreateProviderForPrefabResource(
                            this._resourcePath,
                            concreteType,
                            this._gameObjectBindInfo,
                            this.BindInfo.Arguments,
                            this.BindInfo.ConcreteIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) =>
                            this.CreateProviderForType(
                                concreteType,
                                new PrefabInstantiator(
                                    container,
                                    this._gameObjectBindInfo,
                                    this.BindInfo.Arguments,
                                    new PrefabProviderResource(this._resourcePath))));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiator(
                            container,
                            this._gameObjectBindInfo,
                            this.BindInfo.Arguments,
                            new PrefabProviderResource(this._resourcePath)));

                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) => new CachedProvider(
                            this.CreateProviderForType(concreteType, prefabCreator)));
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
                        (_, contractType) => container.SingletonProviderCreator.CreateProviderForPrefabResource(
                            this._resourcePath,
                            contractType,
                            this._gameObjectBindInfo,
                            this.BindInfo.Arguments,
                            this.BindInfo.ConcreteIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProviderPerContract(
                        container, 
                        (_, contractType) =>
                            this.CreateProviderForType(
                                contractType,
                                new PrefabInstantiator(
                                    container,
                                    this._gameObjectBindInfo,
                                    this.BindInfo.Arguments,
                                    new PrefabProviderResource(this._resourcePath))));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiator(
                            container,
                            this._gameObjectBindInfo,
                            this.BindInfo.Arguments,
                            new PrefabProviderResource(this._resourcePath)));

                    this.RegisterProviderPerContract(
                        container, 
                        (_, contractType) =>
                            new CachedProvider(
                                this.CreateProviderForType(contractType, prefabCreator)));
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
