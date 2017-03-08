namespace Assets.Zenject.Source.Binding.Binders.FromBinders
{

    using System;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Factories;
    using Assets.Zenject.Source.Injection;

    public class FromBinderNonGeneric : FromBinder
    {
        public FromBinderNonGeneric(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, finalizerWrapper)
        {
        }

        public ScopeArgBinder FromFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
        {
            return this.FromFactoryBase<TConcrete, TFactory>();
        }

        public ScopeArgBinder FromMethod<TConcrete>(Func<InjectContext, TConcrete> method)
        {
            return this.FromMethodBase<TConcrete>(method);
        }

        public ScopeBinder FromResolveGetter<TObj, TContract>(Func<TObj, TContract> method)
        {
            return this.FromResolveGetter<TObj, TContract>(null, method);
        }

        public ScopeBinder FromResolveGetter<TObj, TContract>(object identifier, Func<TObj, TContract> method)
        {
            return this.FromResolveGetterBase<TObj, TContract>(identifier, method);
        }

        public ScopeBinder FromInstance(object instance)
        {
            return this.FromInstance(instance, false);
        }

        public ScopeBinder FromInstance(object instance, bool allowNull)
        {
            return this.FromInstanceBase(instance, allowNull);
        }
    }
}

