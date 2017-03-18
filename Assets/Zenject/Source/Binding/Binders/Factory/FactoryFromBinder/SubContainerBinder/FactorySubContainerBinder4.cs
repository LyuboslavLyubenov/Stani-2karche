namespace Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder
{

    using System;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.SubContainerCreators;

    public class FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TContract>
        : FactorySubContainerBinderWithParams<TContract>
    {
        public FactorySubContainerBinder(
            BindInfo bindInfo, Type factoryType,
            BindFinalizerWrapper finalizerWrapper, object subIdentifier)
            : base(bindInfo, factoryType, finalizerWrapper, subIdentifier)
        {
        }

        public ConditionBinder ByMethod(Action<DiContainer, TParam1, TParam2, TParam3, TParam4> installerMethod)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByMethod<TParam1, TParam2, TParam3, TParam4>(
                        container, installerMethod)));

            return new ConditionBinder(this.BindInfo);
        }
    }
}

