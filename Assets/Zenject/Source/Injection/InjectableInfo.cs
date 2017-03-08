namespace Assets.Zenject.Source.Injection
{

    using System;

    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Usage;

    // An injectable is a field or property with [Inject] attribute
    // Or a constructor parameter
    public class InjectableInfo
    {
        public readonly bool Optional;
        public readonly object Identifier;

        public readonly InjectSources SourceType;

        // The field name or property name from source code
        public readonly string MemberName;
        // The field type or property type from source code
        public readonly Type MemberType;

        public readonly Type ObjectType;

        // Null for constructor declared dependencies
        public readonly Action<object, object> Setter;

        public readonly object DefaultValue;

        public InjectableInfo(
            bool optional, object identifier, string memberName,
            Type memberType, Type objectType, Action<object, object> setter, object defaultValue, InjectSources sourceType)
        {
            this.Optional = optional;
            this.Setter = setter;
            this.ObjectType = objectType;
            this.MemberType = memberType;
            this.MemberName = memberName;
            this.Identifier = identifier;
            this.DefaultValue = defaultValue;
            this.SourceType = sourceType;
        }

        public InjectContext CreateInjectContext(
            DiContainer container, InjectContext currentContext, object targetInstance, object concreteIdentifier)
        {
            var context = new InjectContext();

            context.MemberType = this.MemberType;
            context.Container = container;
            context.ObjectType = this.ObjectType;
            context.ParentContext = currentContext;
            context.ObjectInstance = targetInstance;
            context.Identifier = this.Identifier;
            context.ConcreteIdentifier = concreteIdentifier;
            context.MemberName = this.MemberName;
            context.Optional = this.Optional;
            context.SourceType = this.SourceType;
            context.FallBackValue = this.DefaultValue;

            return context;
        }
    }
}
