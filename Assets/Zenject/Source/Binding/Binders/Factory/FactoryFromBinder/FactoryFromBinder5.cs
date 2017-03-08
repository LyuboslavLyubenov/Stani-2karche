namespace Assets.Zenject.Source.Binding.Binders.Factory.FactoryFromBinder
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder;
    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Factories;
    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers;

    public class FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> : FactoryFromBinderBase<TContract>
    {
        public FactoryFromBinder(BindInfo bindInfo, Type factoryType, BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, factoryType, finalizerWrapper)
        {
        }

        public ConditionBinder FromMethod(Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TContract> method)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>(method));

            return this;
        }

        public ConditionBinder FromFactory<TSubFactory>()
            where TSubFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new FactoryProvider<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TSubFactory>(container, new List<TypeValuePair>()));

            return this;
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> FromSubContainerResolve()
        {
            return this.FromSubContainerResolve(null);
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> FromSubContainerResolve(object subIdentifier)
        {
            return new FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>(
                this.BindInfo, this.FactoryType, this.FinalizerWrapper, subIdentifier);
        }
    }
}


