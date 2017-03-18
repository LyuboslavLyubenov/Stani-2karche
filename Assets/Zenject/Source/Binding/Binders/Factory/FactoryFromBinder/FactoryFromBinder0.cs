namespace Zenject.Source.Binding.Binders.Factory.FactoryFromBinder
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder;
    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Factories;
    using Zenject.Source.Injection;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;

    public class FactoryFromBinder<TContract> : FactoryFromBinderBase<TContract>
    {
        public FactoryFromBinder(
            BindInfo bindInfo,
            Type factoryType,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, factoryType, finalizerWrapper)
        {
        }

        public ConditionBinder FromResolveGetter<TObj>(Func<TObj, TContract> method)
        {
            return this.FromResolveGetter<TObj>(null, method);
        }

        public ConditionBinder FromResolveGetter<TObj>(
            object subIdentifier, Func<TObj, TContract> method)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new GetterProvider<TObj, TContract>(subIdentifier, method, container));

            return this;
        }

        public ConditionBinder FromMethod(Func<DiContainer, TContract> method)
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new MethodProviderWithContainer<TContract>(method));

            return this;
        }

        public ConditionBinder FromInstance(object instance)
        {
            BindingUtil.AssertInstanceDerivesFromOrEqual(instance, this.AllParentTypes);

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new InstanceProvider(container, this.ContractType, instance));

            return this;
        }

        public ConditionBinder FromFactory<TSubFactory>()
            where TSubFactory : IFactory<TContract>
        {
            this.SubFinalizer = this.CreateFinalizer(
                (container) => new FactoryProvider<TContract, TSubFactory>(container, new List<TypeValuePair>()));

            return this;
        }

        public FactorySubContainerBinder<TContract> FromSubContainerResolve()
        {
            return this.FromSubContainerResolve(null);
        }

        public FactorySubContainerBinder<TContract> FromSubContainerResolve(object subIdentifier)
        {
            return new FactorySubContainerBinder<TContract>(
                this.BindInfo, this.FactoryType, this.FinalizerWrapper, subIdentifier);
        }

#if !NOT_UNITY3D

        public ConditionBinder FromResource(string resourcePath)
        {
            BindingUtil.AssertDerivesFromUnityObject(this.ContractType);

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new ResourceProvider(resourcePath, this.ContractType));

            return this;
        }
#endif
    }
}
