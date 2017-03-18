namespace Zenject.Source.Binding.Binders
{

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;

    public class ScopeBinder : ConditionBinder
    {
        public ScopeBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public ConditionBinder AsSingle()
        {
            return this.AsSingle(null);
        }

        public ConditionBinder AsSingle(object concreteIdentifier)
        {
            Assert.IsNull(this.BindInfo.ConcreteIdentifier);

            this.BindInfo.Scope = ScopeTypes.Singleton;
            this.BindInfo.ConcreteIdentifier = concreteIdentifier;
            return this;
        }

        public ConditionBinder AsCached()
        {
            this.BindInfo.Scope = ScopeTypes.Cached;
            return this;
        }

        // Note that this is the default so it's not necessary to call this
        public ConditionBinder AsTransient()
        {
            this.BindInfo.Scope = ScopeTypes.Transient;
            return this;
        }
    }
}

