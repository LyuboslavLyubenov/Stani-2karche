namespace Zenject.Source.Binding.Binders.ConcreteBinders
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Binding.Binders.Conventions;
    using Zenject.Source.Binding.Binders.FromBinders;
    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Internal;
    using Zenject.Source.Providers;
    using Zenject.Source.Providers.Singleton;

    public class ConcreteBinderNonGeneric : FromBinderNonGeneric
    {
        public ConcreteBinderNonGeneric(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, finalizerWrapper)
        {
            this.ToSelf();
        }

        // Note that this is the default, so not necessary to call
        public FromBinderNonGeneric ToSelf()
        {
            Assert.IsEqual(this.BindInfo.ToChoice, ToChoices.Self);

            this.SubFinalizer = new ScopableBindingFinalizer(
                this.BindInfo, SingletonTypes.To, null,
                (container, type) => new TransientProvider(
                    type, container, this.BindInfo.Arguments, this.BindInfo.ConcreteIdentifier));

            return this;
        }

        public FromBinderNonGeneric To<TConcrete>()
        {
            return this.To(typeof(TConcrete));
        }

        public FromBinderNonGeneric To(params Type[] concreteTypes)
        {
            return this.To((IEnumerable<Type>)concreteTypes);
        }

        public FromBinderNonGeneric To(IEnumerable<Type> concreteTypes)
        {
            BindingUtil.AssertIsDerivedFromTypes(concreteTypes, this.BindInfo.ContractTypes, this.BindInfo.InvalidBindResponse);

            this.BindInfo.ToChoice = ToChoices.Concrete;
            this.BindInfo.ToTypes = concreteTypes.ToList();

            return this;
        }

#if !(UNITY_WSA && ENABLE_DOTNET)
        public FromBinderNonGeneric To(
            Action<ConventionSelectTypesBinder> generator)
        {
            var bindInfo = new ConventionBindInfo();

            // This is nice because it allows us to do things like Bind(all interfaces).To(specific types)
            // instead of having to do Bind(all interfaces).To(specific types that inherit from one of these interfaces)
            this.BindInfo.InvalidBindResponse = InvalidBindResponses.Skip;

            generator(new ConventionSelectTypesBinder(bindInfo));

            this.BindInfo.ToChoice = ToChoices.Concrete;
            this.BindInfo.ToTypes = bindInfo.ResolveTypes();

            return this;
        }
#endif
    }
}
