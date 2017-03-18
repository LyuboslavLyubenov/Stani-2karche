namespace Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Zenject.Source.Factories;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Validation;

    public class UntypedFactoryProvider : IProvider
    {
        readonly List<TypeValuePair> _factoryArgs;
        readonly DiContainer _container;
        readonly Type _factoryType;
        readonly Type _concreteType;
        readonly MethodInfo _createMethod;

        public UntypedFactoryProvider(
            Type factoryType, DiContainer container, List<TypeValuePair> factoryArgs)
        {
            Assert.That(factoryType.DerivesFrom<IFactory>());

            this._concreteType = this.LookupConcreteType(factoryType);
            this._factoryType = factoryType;
            this._container = container;
            this._factoryArgs = factoryArgs;

            this._createMethod = factoryType
                .DeclaredInstanceMethods().Where(x => x.Name == "Create").Single();

            Assert.That(this._createMethod.ReturnType == this._concreteType);
        }

        Type LookupConcreteType(Type factoryType)
        {
            // We assume here that the concrete type is the last generic argument to the IFactory class
            return factoryType.Interfaces().Where(x => x.Interfaces().OnlyOrDefault() == typeof(IFactory))
                .Single().GenericArguments().Last();
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._concreteType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            // Do this even when validating in case it has its own dependencies
            var factory = this._container.InstantiateExplicit(this._factoryType, this._factoryArgs);

            if (this._container.IsValidating)
            {
                // In case users define a custom IFactory that needs to be validated
                if (factory is IValidatable)
                {
                    ((IValidatable)factory).Validate();
                }

                // We assume here that we are creating a user-defined factory so there's
                // nothing else we can validate here
                yield return new List<object>() { new ValidationMarker(this._concreteType) };
            }
            else
            {
                var result = this._createMethod.Invoke(factory, args.Select(x => x.Value).ToArray());

                yield return new List<object>() { result };
            }
        }
    }
}
