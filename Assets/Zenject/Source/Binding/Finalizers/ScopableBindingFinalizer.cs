namespace Assets.Zenject.Source.Binding.Finalizers
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers;
    using Assets.Zenject.Source.Providers.Singleton;
    using Assets.Zenject.Source.Providers.Singleton.Standard;

    public class ScopableBindingFinalizer : ProviderBindingFinalizer
    {
        readonly SingletonTypes _singletonType;
        readonly Func<DiContainer, Type, IProvider> _providerFactory;
        readonly object _singletonSpecificId;

        public ScopableBindingFinalizer(
            BindInfo bindInfo,
            SingletonTypes singletonType, object singletonSpecificId,
            Func<DiContainer, Type, IProvider> providerFactory)
            : base(bindInfo)
        {
            this._singletonType = singletonType;
            this._providerFactory = providerFactory;
            this._singletonSpecificId = singletonSpecificId;
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
            if (concreteTypes.IsEmpty())
            {
                // This can be common when using convention based bindings
                return;
            }

            switch (this.BindInfo.Scope)
            {
                case ScopeTypes.Singleton:
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) => container.SingletonProviderCreator.CreateProviderStandard(
                            new StandardSingletonDeclaration(
                                concreteType,
                                this.BindInfo.ConcreteIdentifier,
                                this.BindInfo.Arguments,
                                this._singletonType,
                                this._singletonSpecificId),
                            this._providerFactory));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container, concreteTypes, this._providerFactory);
                    break;
                }
                case ScopeTypes.Cached:
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) =>
                            new CachedProvider(
                                this._providerFactory(container, concreteType)));
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
                        (_, contractType) => container.SingletonProviderCreator.CreateProviderStandard(
                            new StandardSingletonDeclaration(
                                contractType,
                                this.BindInfo.ConcreteIdentifier,
                                this.BindInfo.Arguments,
                                this._singletonType,
                                this._singletonSpecificId),
                            this._providerFactory));
                    break;
                }
                case ScopeTypes.Transient:
                {
                    this.RegisterProviderPerContract(container, this._providerFactory);
                    break;
                }
                case ScopeTypes.Cached:
                {
                    this.RegisterProviderPerContract(
                        container,
                        (_, contractType) =>
                            new CachedProvider(
                                this._providerFactory(container, contractType)));
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
