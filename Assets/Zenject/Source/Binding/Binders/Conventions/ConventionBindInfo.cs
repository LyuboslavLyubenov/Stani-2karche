#if !(UNITY_WSA && ENABLE_DOTNET)

namespace Assets.Zenject.Source.Binding.Binders.Conventions
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ConventionBindInfo
    {
        readonly List<Func<Type, bool>> _typeFilters = new List<Func<Type, bool>>();
        readonly List<Func<Assembly, bool>> _assemblyFilters = new List<Func<Assembly, bool>>();

        static Dictionary<Assembly, Type[]> _assemblyTypeCache = new Dictionary<Assembly, Type[]>();

        public void AddAssemblyFilter(Func<Assembly, bool> predicate)
        {
            this._assemblyFilters.Add(predicate);
        }

        public void AddTypeFilter(Func<Type, bool> predicate)
        {
            this._typeFilters.Add(predicate);
        }

        IEnumerable<Assembly> GetAllAssemblies()
        {
            // This seems fast enough that it's not worth caching
            // We also want to allow dynamically loading assemblies
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        bool ShouldIncludeAssembly(Assembly assembly)
        {
            return this._assemblyFilters.All(predicate => predicate(assembly));
        }

        bool ShouldIncludeType(Type type)
        {
            return this._typeFilters.All(predicate => predicate(type));
        }

        Type[] GetTypes(Assembly assembly)
        {
            Type[] types;

            // This is much faster than calling assembly.GetTypes() every time
            if (!_assemblyTypeCache.TryGetValue(assembly, out types))
            {
                types = assembly.GetTypes();
                _assemblyTypeCache[assembly] = types;
            }

            return types;
        }

        public List<Type> ResolveTypes()
        {
            return this.GetAllAssemblies()
                .Where(this.ShouldIncludeAssembly)
                .SelectMany(assembly => this.GetTypes(assembly))
                .Where(this.ShouldIncludeType).ToList();
        }
    }
}

#endif
