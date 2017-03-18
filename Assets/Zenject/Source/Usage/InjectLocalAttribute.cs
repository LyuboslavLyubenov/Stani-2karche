namespace Zenject.Source.Usage
{

    using System;

    [AttributeUsage(AttributeTargets.Parameter
        | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectLocalAttribute : InjectAttributeBase
    {
        public InjectLocalAttribute()
        {
            this.Source = InjectSources.Local;
        }
    }
}
