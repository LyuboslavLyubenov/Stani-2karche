namespace Assets.Zenject.Source.Binding.Binders.FromBinders
{

    using System;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Factories;
    using Assets.Zenject.Source.Injection;

    public class FromBinderGeneric<TContract> : FromBinder
    {
        public FromBinderGeneric(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, finalizerWrapper)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TContract), this.BindInfo.ContractTypes);
        }

        public ScopeArgBinder FromFactory<TFactory>()
            where TFactory : IFactory<TContract>
        {
            return this.FromFactoryBase<TContract, TFactory>();
        }

        public ScopeArgBinder FromFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return this.FromFactoryBase<TConcrete, TFactory>();
        }

        public ScopeArgBinder FromMethod(Func<InjectContext, TContract> method)
        {
            return this.FromMethodBase<TContract>(method);
        }

        public ScopeBinder FromResolveGetter<TObj>(Func<TObj, TContract> method)
        {
            return this.FromResolveGetter<TObj>(null, method);
        }

        public ScopeBinder FromResolveGetter<TObj>(object identifier, Func<TObj, TContract> method)
        {
            return this.FromResolveGetterBase<TObj, TContract>(identifier, method);
        }

        public ScopeBinder FromInstance(TContract instance)
        {
            return this.FromInstance(instance, false);
        }

        public ScopeBinder FromInstance(TContract instance, bool allowNull)
        {
            return this.FromInstanceBase(instance, allowNull);
        }
    }
}

