#if !(UNITY_WSA && ENABLE_DOTNET)

namespace Zenject.Source.Binding.Binders.Conventions
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Zenject.Source.Internal;

    public class ConventionFilterTypesBinder : ConventionAssemblySelectionBinder
    {
        public ConventionFilterTypesBinder(ConventionBindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public ConventionFilterTypesBinder DerivingFromOrEqual<T>()
        {
            return this.DerivingFromOrEqual(typeof(T));
        }

        public ConventionFilterTypesBinder DerivingFromOrEqual(Type parentType)
        {
            this.BindInfo.AddTypeFilter((type) => type.DerivesFromOrEqual(parentType));
            return this;
        }

        public ConventionFilterTypesBinder DerivingFrom<T>()
        {
            return this.DerivingFrom(typeof(T));
        }

        public ConventionFilterTypesBinder DerivingFrom(Type parentType)
        {
            this.BindInfo.AddTypeFilter((type) => type.DerivesFrom(parentType));
            return this;
        }

        public ConventionFilterTypesBinder WithAttribute<T>()
            where T : Attribute
        {
            return this.WithAttribute(typeof(T));
        }

        public ConventionFilterTypesBinder WithAttribute(Type attribute)
        {
            Assert.That(attribute.DerivesFrom<Attribute>());
            this.BindInfo.AddTypeFilter(t => t.HasAttribute(attribute));
            return this;
        }

        public ConventionFilterTypesBinder WithoutAttribute<T>()
            where T : Attribute
        {
            return this.WithoutAttribute(typeof(T));
        }

        public ConventionFilterTypesBinder WithoutAttribute(Type attribute)
        {
            Assert.That(attribute.DerivesFrom<Attribute>());
            this.BindInfo.AddTypeFilter(t => !t.HasAttribute(attribute));
            return this;
        }

        public ConventionFilterTypesBinder WithAttributeWhere<T>(Func<T, bool> predicate)
            where T : Attribute
        {
            this.BindInfo.AddTypeFilter(t => t.HasAttribute<T>() && t.AllAttributes<T>().All(predicate));
            return this;
        }

        public ConventionFilterTypesBinder Where(Func<Type, bool> predicate)
        {
            this.BindInfo.AddTypeFilter(predicate);
            return this;
        }

        public ConventionFilterTypesBinder InNamespace(string ns)
        {
            return this.InNamespaces(ns);
        }

        public ConventionFilterTypesBinder InNamespaces(params string[] namespaces)
        {
            return this.InNamespaces((IEnumerable<string>)namespaces);
        }

        public ConventionFilterTypesBinder InNamespaces(IEnumerable<string> namespaces)
        {
            this.BindInfo.AddTypeFilter(t => namespaces.Any(n => IsInNamespace(t, n)));
            return this;
        }

        public ConventionFilterTypesBinder WithSuffix(string suffix)
        {
            this.BindInfo.AddTypeFilter(t => t.Name.EndsWith(suffix));
            return this;
        }

        public ConventionFilterTypesBinder WithPrefix(string prefix)
        {
            this.BindInfo.AddTypeFilter(t => t.Name.StartsWith(prefix));
            return this;
        }

        public ConventionFilterTypesBinder MatchingRegex(string pattern)
        {
            return this.MatchingRegex(pattern, RegexOptions.None);
        }

        public ConventionFilterTypesBinder MatchingRegex(string pattern, RegexOptions options)
        {
            return this.MatchingRegex(new Regex(pattern, options));
        }

        public ConventionFilterTypesBinder MatchingRegex(Regex regex)
        {
            this.BindInfo.AddTypeFilter(t => regex.IsMatch(t.Name));
            return this;
        }

        static bool IsInNamespace(Type type, string requiredNs)
        {
            var actualNs = type.Namespace ?? "";

            if (requiredNs.Length > actualNs.Length)
            {
                return false;
            }

            return actualNs.StartsWith(requiredNs)
                && (actualNs.Length == requiredNs.Length || actualNs[requiredNs.Length] == '.');
        }
    }
}

#endif
