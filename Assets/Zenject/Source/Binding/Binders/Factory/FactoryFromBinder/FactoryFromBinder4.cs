namespace Zenject.Source.Binding.Binders.Factory.FactoryFromBinder
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder;
    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Factories;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;

    public class FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> : FactoryFromBinderBase<TContract>
    {
        public FactoryFromBinder(BindInfo bindInfo, Type factoryType, BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, factoryType, finalizerWrapper)
        {
        }

        public ConditionBinder FromMethod(Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TContract> method)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TContract>(method));

            return this;
        }

        public ConditionBinder FromFactory<TSubFactory>()
            where TSubFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new FactoryProvider<TParam1, TParam2, TParam3, TParam4, TContract, TSubFactory>(container, new List<TypeValuePair>()));

            return this;
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TContract> FromSubContainerResolve()
        {
            return this.FromSubContainerResolve(null);
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TContract> FromSubContainerResolve(object subIdentifier)
        {
            return new FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TContract>(
                this.BindInfo, this.FactoryType, this.FinalizerWrapper, subIdentifier);
        }
    }
}
