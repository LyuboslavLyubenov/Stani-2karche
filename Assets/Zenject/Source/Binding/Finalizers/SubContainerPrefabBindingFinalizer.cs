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

    public class SubContainerPrefabBindingFinalizer : ProviderBindingFinalizer
    {
        readonly UnityEngine.Object _prefab;
        readonly object _subIdentifier;
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public SubContainerPrefabBindingFinalizer(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectBindInfo,
            UnityEngine.Object prefab,
            object subIdentifier)
            : base(bindInfo)
        {
            this._gameObjectBindInfo = gameObjectBindInfo;
            this._prefab = prefab;
            this._subIdentifier = subIdentifier;
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
                        (_, concreteType) => container.SingletonProviderCreator.CreateProviderForSubContainerPrefab(
                            concreteType,
                            this.BindInfo.ConcreteIdentifier,
                            this._gameObjectBindInfo,
                            this._prefab,
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
                                container, new PrefabProvider(this._prefab), this._gameObjectBindInfo)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var containerCreator = new SubContainerCreatorCached(
                        new SubContainerCreatorByPrefab(
                            container, new PrefabProvider(this._prefab), this._gameObjectBindInfo));

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
                        (_, contractType) => container.SingletonProviderCreator.CreateProviderForSubContainerPrefab(
                            contractType,
                            this.BindInfo.ConcreteIdentifier,
                            this._gameObjectBindInfo,
                            this._prefab,
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
                                container, new PrefabProvider(this._prefab), this._gameObjectBindInfo)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var containerCreator = new SubContainerCreatorCached(
                        new SubContainerCreatorByPrefab(
                            container, new PrefabProvider(this._prefab), this._gameObjectBindInfo));

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
