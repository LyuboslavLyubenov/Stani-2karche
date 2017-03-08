namespace Assets.Zenject.Source.Binding.Finalizers
{

    using System;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers;

    public class SingleProviderBindingFinalizer : ProviderBindingFinalizer
    {
        readonly Func<DiContainer, Type, IProvider> _providerFactory;

        public SingleProviderBindingFinalizer(
            BindInfo bindInfo, Func<DiContainer, Type, IProvider> providerFactory)
            : base(bindInfo)
        {
            this._providerFactory = providerFactory;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            if (this.BindInfo.ToChoice == ToChoices.Self)
            {
                Assert.IsEmpty(this.BindInfo.ToTypes);

                this.RegisterProviderPerContract(container, this._providerFactory);
            }
            else
            {
                // Empty sometimes when using convention based bindings
                if (!this.BindInfo.ToTypes.IsEmpty())
                {
                    this.RegisterProvidersForAllContractsPerConcreteType(
                        container, this.BindInfo.ToTypes, this._providerFactory);
                }
            }
        }
    }
}
