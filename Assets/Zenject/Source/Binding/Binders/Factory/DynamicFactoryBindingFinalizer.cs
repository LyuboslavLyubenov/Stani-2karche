namespace Zenject.Source.Binding.Binders.Factory
{

    using System;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Factories;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;

    public class DynamicFactoryBindingFinalizer<TContract> : ProviderBindingFinalizer
    {
        readonly Func<DiContainer, IProvider> _providerFunc;
        readonly Type _factoryType;

        public DynamicFactoryBindingFinalizer(
            BindInfo bindInfo, Type factoryType, Func<DiContainer, IProvider> providerFunc)
            : base(bindInfo)
        {
            // Note that it doesn't derive from Factory<TContract>
            // when used with To<>, so we can only check IDynamicFactory
            Assert.That(factoryType.DerivesFrom<IDynamicFactory>());

            this._factoryType = factoryType;
            this._providerFunc = providerFunc;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            var provider = this._providerFunc(container);

            this.RegisterProviderForAllContracts(
                container,
                new CachedProvider(
                    new TransientProvider(
                        this._factoryType,
                        container,
                        InjectUtil.CreateArgListExplicit(
                            provider,
                            new InjectContext(container, typeof(TContract))),
                            null)));
        }
    }
}
