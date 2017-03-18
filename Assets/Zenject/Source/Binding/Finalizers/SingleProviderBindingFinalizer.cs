namespace Zenject.Source.Binding.Finalizers
{

    using System;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;

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
