namespace Zenject.Source.Binding.Finalizers
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.SubContainerCreators;

    public class SubContainerMethodBindingFinalizer : ProviderBindingFinalizer
    {
        readonly object _subIdentifier;
        readonly Action<DiContainer> _installMethod;

        public SubContainerMethodBindingFinalizer(
            BindInfo bindInfo, Action<DiContainer> installMethod, object subIdentifier)
            : base(bindInfo)
        {
            this._subIdentifier = subIdentifier;
            this._installMethod = installMethod;
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
                        (_, concreteType) =>
                            container.SingletonProviderCreator.CreateProviderForSubContainerMethod(
                                concreteType,
                                this.BindInfo.ConcreteIdentifier,
                                this._installMethod,
                                this._subIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    // Note: each contract/concrete pair gets its own container here
                    this.RegisterProvidersPerContractAndConcreteType(
                        container,
                        concreteTypes,
                        (contractType, concreteType) => new SubContainerDependencyProvider(
                            concreteType, this._subIdentifier,
                            new SubContainerCreatorByMethod(container, this._installMethod)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var creator = new SubContainerCreatorCached(
                        new SubContainerCreatorByMethod(container, this._installMethod));

                    // Note: each contract/concrete pair gets its own container
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) => new SubContainerDependencyProvider(
                            concreteType, this._subIdentifier, creator));
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
                        (_, contractType) => container.SingletonProviderCreator.CreateProviderForSubContainerMethod(
                            contractType,
                            this.BindInfo.ConcreteIdentifier,
                            this._installMethod,
                            this._subIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProviderPerContract(
                        container,
                        (_, contractType) => new SubContainerDependencyProvider(
                            contractType, this._subIdentifier,
                            new SubContainerCreatorByMethod(
                                container, this._installMethod)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var containerCreator = new SubContainerCreatorCached(
                        new SubContainerCreatorByMethod(container, this._installMethod));

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


