namespace Assets.Zenject.Source.Binding.Binders.ConcreteBinders
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Zenject.Source.Binding.Binders.Conventions;
    using Assets.Zenject.Source.Binding.Binders.FromBinders;
    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Providers;
    using Assets.Zenject.Source.Providers.Singleton;

    public class ConcreteBinderGeneric<TContract> : FromBinderGeneric<TContract>
    {
        public ConcreteBinderGeneric(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, finalizerWrapper)
        {
            this.ToSelf();
        }

        // Note that this is the default, so not necessary to call
        public FromBinderGeneric<TContract> ToSelf()
        {
            Assert.IsEqual(this.BindInfo.ToChoice, ToChoices.Self);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo, SingletonTypes.To, null,
                (container, type) => new TransientProvider(
                    type, container, this.BindInfo.Arguments, this.BindInfo.ConcreteIdentifier));

            return this;
        }

        public FromBinderGeneric<TConcrete> To<TConcrete>()
            where TConcrete : TContract
        {
            this.BindInfo.ToChoice = ToChoices.Concrete;
            this.BindInfo.ToTypes = new List<Type>()
            {
                typeof(TConcrete)
            };

            return new FromBinderGeneric<TConcrete>(
                this.BindInfo, this.FinalizerWrapper);
        }

        public FromBinderNonGeneric To(params Type[] concreteTypes)
        {
            return this.To((IEnumerable<Type>)concreteTypes);
        }

        public FromBinderNonGeneric To(IEnumerable<Type> concreteTypes)
        {
            BindingUtil.AssertIsDerivedFromTypes(
                concreteTypes, this.BindInfo.ContractTypes, this.BindInfo.InvalidBindResponse);

            this.BindInfo.ToChoice = ToChoices.Concrete;
            this.BindInfo.ToTypes = concreteTypes.ToList();

            return new FromBinderNonGeneric(
                this.BindInfo, this.FinalizerWrapper);
        }

#if !(UNITY_WSA && ENABLE_DOTNET)
        public FromBinderNonGeneric To(
            Action<ConventionSelectTypesBinder> generator)
        {
            var bindInfo = new ConventionBindInfo();

            // Automatically filter by the given contract types
            bindInfo.AddTypeFilter(
                concreteType => this.BindInfo.ContractTypes.All(contractType => concreteType.DerivesFromOrEqual(contractType)));

            generator(new ConventionSelectTypesBinder(bindInfo));
            return this.To(bindInfo.ResolveTypes());
        }
#endif
    }
}
