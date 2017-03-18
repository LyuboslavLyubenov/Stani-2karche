#if !(UNITY_WSA && ENABLE_DOTNET)

namespace Zenject.Source.Binding.Binders.Conventions
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ConventionAssemblySelectionBinder
    {
        public ConventionAssemblySelectionBinder(ConventionBindInfo bindInfo)
        {
            this.BindInfo = bindInfo;
        }

        protected ConventionBindInfo BindInfo
        {
            get;
            private set;
        }

        public void FromAllAssemblies()
        {
            // Do nothing
            // This is the default
        }

        public void FromAssemblyContaining<T>()
        {
            this.FromAssembliesContaining(typeof(T));
        }

        public void FromAssembliesContaining(params Type[] types)
        {
            this.FromAssembliesContaining((IEnumerable<Type>)types);
        }

        public void FromAssembliesContaining(IEnumerable<Type> types)
        {
            this.FromAssemblies(types.Select(t => t.Assembly).Distinct());
        }

        public void FromThisAssembly()
        {
            this.FromAssemblies(Assembly.GetCallingAssembly());
        }

        public void FromAssembly(Assembly assembly)
        {
            this.FromAssemblies(assembly);
        }

        public void FromAssemblies(params Assembly[] assemblies)
        {
            this.FromAssemblies((IEnumerable<Assembly>)assemblies);
        }

        public void FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            this.BindInfo.AddAssemblyFilter(assembly => assemblies.Contains(assembly));
        }

        public void FromAssembliesWhere(Func<Assembly, bool> predicate)
        {
            this.BindInfo.AddAssemblyFilter(predicate);
        }
    }
}

#endif
