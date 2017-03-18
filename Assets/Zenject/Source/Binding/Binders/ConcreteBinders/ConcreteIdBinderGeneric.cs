namespace Zenject.Source.Binding.Binders.ConcreteBinders
{

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;

    public class ConcreteIdBinderGeneric<TContract> : ConcreteBinderGeneric<TContract>
    {
        public ConcreteIdBinderGeneric(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, finalizerWrapper)
        {
        }

        public ConcreteBinderGeneric<TContract> WithId(object identifier)
        {
            this.BindInfo.Identifier = identifier;
            return this;
        }
    }
}

