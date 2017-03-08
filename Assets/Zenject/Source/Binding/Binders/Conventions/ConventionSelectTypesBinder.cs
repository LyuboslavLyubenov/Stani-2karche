#if !(UNITY_WSA && ENABLE_DOTNET)

namespace Assets.Zenject.Source.Binding.Binders.Conventions
{
    public class ConventionSelectTypesBinder
    {
        readonly ConventionBindInfo _bindInfo;

        public ConventionSelectTypesBinder(ConventionBindInfo bindInfo)
        {
            this._bindInfo = bindInfo;
        }

        ConventionFilterTypesBinder CreateNextBinder()
        {
            return new ConventionFilterTypesBinder(this._bindInfo);
        }

        public ConventionFilterTypesBinder AllTypes()
        {
            // Do nothing (this is the default)
            return this.CreateNextBinder();
        }

        public ConventionFilterTypesBinder AllClasses()
        {
            this._bindInfo.AddTypeFilter(t => t.IsClass);
            return this.CreateNextBinder();
        }

        public ConventionFilterTypesBinder AllNonAbstractClasses()
        {
            this._bindInfo.AddTypeFilter(t => t.IsClass && !t.IsAbstract);
            return this.CreateNextBinder();
        }

        public ConventionFilterTypesBinder AllAbstractClasses()
        {
            this._bindInfo.AddTypeFilter(t => t.IsClass && t.IsAbstract);
            return this.CreateNextBinder();
        }

        public ConventionFilterTypesBinder AllInterfaces()
        {
            this._bindInfo.AddTypeFilter(t => t.IsInterface);
            return this.CreateNextBinder();
        }
    }
}

#endif
