namespace Assets.Zenject.Source.Binding.Binders.ConcreteBinders
{

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;

    public class ConcreteIdBinderNonGeneric : ConcreteBinderNonGeneric
    {
        public ConcreteIdBinderNonGeneric(
            BindInfo bindInfo,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, finalizerWrapper)
        {
        }

        public ConcreteBinderNonGeneric WithId(object identifier)
        {
            this.BindInfo.Identifier = identifier;
            return this;
        }
    }
}

