namespace Zenject.OptionalExtras.CommandsAndSignals.Command.Binders.Finalizers
{

    using System;
    using System.Linq;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.Singleton;
    using Zenject.Source.Providers.Singleton.Standard;

    // Zero Parameters

    public abstract class CommandBindingFinalizerBase<TCommand, THandler, TAction>
        : ProviderBindingFinalizer
        where TCommand : ICommand
    {
        readonly Func<DiContainer, IProvider> _handlerProviderFactory;

        public CommandBindingFinalizerBase(
            BindInfo bindInfo,
            Func<DiContainer, IProvider> handlerProviderFactory)
            : base(bindInfo)
        {
            this._handlerProviderFactory = handlerProviderFactory;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            Assert.That(this.BindInfo.ContractTypes.IsLength(1));
            Assert.IsEqual(this.BindInfo.ContractTypes.Single(), typeof(TCommand));

            // Note that the singleton here applies to the handler, not the command class
            // The command itself is always cached
            this.RegisterProvider<TCommand>(
                container,
                new CachedProvider(
                    new TransientProvider(
                        typeof(TCommand), container,
                        InjectUtil.CreateArgListExplicit(this.GetCommandAction(container)), null)));
        }

        // The returned delegate is executed every time the command is executed
        TAction GetCommandAction(DiContainer container)
        {
            var handlerProvider = this.GetHandlerProvider(container);
            var handlerInjectContext = new InjectContext(container, typeof(THandler));

            return this.GetCommandAction(handlerProvider, handlerInjectContext);
        }

        IProvider GetHandlerProvider(DiContainer container)
        {
            switch (this.BindInfo.Scope)
            {
                case ScopeTypes.Singleton:
                {
                    return container.SingletonProviderCreator.CreateProviderStandard(
                        new StandardSingletonDeclaration(
                            typeof(THandler),
                            this.BindInfo.ConcreteIdentifier,
                            this.BindInfo.Arguments,
                            SingletonTypes.To,
                            null),
                        (_, type) => this._handlerProviderFactory(container));
                }
                case ScopeTypes.Transient:
                {
                    return this._handlerProviderFactory(container);
                }
                case ScopeTypes.Cached:
                {
                    return new CachedProvider(
                        this._handlerProviderFactory(container));
                }
            }

            throw Assert.CreateException();
        }

        protected abstract TAction GetCommandAction(
            IProvider handlerProvider, InjectContext handlerContext);
    }
}

