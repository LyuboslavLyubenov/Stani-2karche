namespace Zenject.Source.Binding.Binders
{

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;

    public class ScopeArgBinder : ArgumentsBinder
    {
        public ScopeArgBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public ArgumentsBinder AsSingle()
        {
            return this.AsSingle(null);
        }

        public ArgumentsBinder AsSingle(object concreteIdentifier)
        {
            Assert.IsNull(this.BindInfo.ConcreteIdentifier);

            this.BindInfo.Scope = ScopeTypes.Singleton;
            this.BindInfo.ConcreteIdentifier = concreteIdentifier;
            return this;
        }

        public ArgumentsBinder AsCached()
        {
            this.BindInfo.Scope = ScopeTypes.Cached;
            return this;
        }

        // Note that this is the default so it's not necessary to call this
        public ArgumentsBinder AsTransient()
        {
            this.BindInfo.Scope = ScopeTypes.Transient;
            return this;
        }
    }
}
