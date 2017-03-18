namespace Zenject.OptionalExtras.CommandsAndSignals.Command.Binders
{

    using System;

    using Zenject.OptionalExtras.CommandsAndSignals.Command.Binders.Finalizers;
    using Zenject.Source.Binding.Binders;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;

    // Five parameters

    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5> : CommandBinderBase<TCommand, Action<TParam1, TParam2, TParam3, TParam4, TParam5>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        public CommandBinder(string identifier, DiContainer container)
            : base(identifier, container)
        {
        }

        public ScopeArgBinder To<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            this.Finalizer = new CommandBindingFinalizer<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                this.BindInfo, methodGetter,
                (container) => new TransientProvider(
                    typeof(THandler), container, this.BindInfo.Arguments, this.BindInfo.ConcreteIdentifier));

            return new ScopeArgBinder(this.BindInfo);
        }

        public ScopeBinder ToResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return this.ToResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return this.ToResolveInternal<THandler>(identifier, methodGetter, false);
        }

        public ScopeBinder ToOptionalResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return this.ToOptionalResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return this.ToResolveInternal<THandler>(identifier, methodGetter, true);
        }

        public ConditionBinder ToNothing()
        {
            return this.ToMethod((p1, p2, p3, p4, p5) => {});
        }

        // AsSingle / AsCached / etc. don't make sense in this case so just return ConditionBinder
        public ConditionBinder ToMethod(Action<TParam1, TParam2, TParam3, TParam4, TParam5> action)
        {
            // Create the command class once and re-use it everywhere
            this.Finalizer = new SingleProviderBindingFinalizer(
                this.BindInfo,
                (container, _) => new CachedProvider(
                    new TransientProvider(
                        typeof(TCommand), container,
                        InjectUtil.CreateArgListExplicit(action), null)));

            return new ConditionBinder(this.BindInfo);
        }

        ScopeBinder ToResolveInternal<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter, bool optional)
        {
            this.Finalizer = new CommandBindingFinalizer<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                this.BindInfo, methodGetter,
                (container) => new ResolveProvider(typeof(THandler), container, identifier, optional));

            return new ScopeBinder(this.BindInfo);
        }
    }
}

