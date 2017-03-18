namespace Zenject.Source.Binding.Finalizers
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.SubContainerCreators;

    public class SubContainerInstallerBindingFinalizer : ProviderBindingFinalizer
    {
        readonly object _subIdentifier;
        readonly Type _installerType;

        public SubContainerInstallerBindingFinalizer(
            BindInfo bindInfo, Type installerType, object subIdentifier)
            : base(bindInfo)
        {
            this._subIdentifier = subIdentifier;
            this._installerType = installerType;
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

        ISubContainerCreator CreateContainerCreator(DiContainer container)
        {
            return new SubContainerCreatorCached(
                new SubContainerCreatorByInstaller(container, this._installerType));
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
                            container.SingletonProviderCreator.CreateProviderForSubContainerInstaller(
                                concreteType, this.BindInfo.ConcreteIdentifier, this._installerType, this._subIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) =>
                            new SubContainerDependencyProvider(
                                concreteType, this._subIdentifier, this.CreateContainerCreator(container)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var containerCreator = this.CreateContainerCreator(container);

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
                        (_, contractType) => container.SingletonProviderCreator.CreateProviderForSubContainerInstaller(
                            contractType, this.BindInfo.ConcreteIdentifier, this._installerType, this._subIdentifier));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProviderPerContract(
                        container, 
                        (_, contractType) => new SubContainerDependencyProvider(
                            contractType, this._subIdentifier, this.CreateContainerCreator(container)));
                    break;
                }
                case ScopeTypes.Cached:
                {
                    var containerCreator = this.CreateContainerCreator(container);

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

