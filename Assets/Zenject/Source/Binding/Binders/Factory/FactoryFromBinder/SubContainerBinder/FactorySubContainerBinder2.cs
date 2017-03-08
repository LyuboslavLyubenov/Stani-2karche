namespace Assets.Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder
{

    using System;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers;
    using Assets.Zenject.Source.Providers.SubContainerCreators;

    public class FactorySubContainerBinder<TParam1, TParam2, TContract>
        : FactorySubContainerBinderWithParams<TContract>
    {
        public FactorySubContainerBinder(
            BindInfo bindInfo, Type factoryType,
            BindFinalizerWrapper finalizerWrapper, object subIdentifier)
            : base(bindInfo, factoryType, finalizerWrapper, subIdentifier)
        {
        }

        public ConditionBinder ByMethod(Action<DiContainer, TParam1, TParam2> installerMethod)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByMethod<TParam1, TParam2>(
                        container, installerMethod)));

            return new ConditionBinder(this.BindInfo);
        }
    }
}

