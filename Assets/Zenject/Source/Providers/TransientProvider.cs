namespace Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;

    public class TransientProvider : IProvider
    {
        readonly DiContainer _container;
        readonly Type _concreteType;
        readonly object _concreteIdentifier;
        readonly List<TypeValuePair> _extraArguments;

        public TransientProvider(
            Type concreteType, DiContainer container,
            List<TypeValuePair> extraArguments, object concreteIdentifier)
        {
            this._container = container;
            this._concreteType = concreteType;
            this._concreteIdentifier = concreteIdentifier;
            this._extraArguments = extraArguments;
        }

        public TransientProvider(
            Type concreteType, DiContainer container,
            List<TypeValuePair> extraArguments)
            : this(concreteType, container, extraArguments, null)
        {
        }

        public TransientProvider(
            Type concreteType, DiContainer container)
            : this(concreteType, container, new List<TypeValuePair>())
        {
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._concreteType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsNotNull(context);

            bool autoInject = false;

            var instanceType = this.GetTypeToCreate(context.MemberType);

            var injectArgs = new InjectArgs()
            {
                ExtraArgs = this._extraArguments.Concat(args).ToList(),
                Context = context,
                ConcreteIdentifier = this._concreteIdentifier,
                UseAllArgs = false,
            };

            var instance = this._container.InstantiateExplicit(
                instanceType, autoInject, injectArgs);

            // Return before property/field/method injection to allow circular dependencies
            yield return new List<object>() { instance };

            injectArgs.UseAllArgs = true;

            this._container.InjectExplicit(instance, instanceType, injectArgs);
        }

        Type GetTypeToCreate(Type contractType)
        {
            return ProviderUtil.GetTypeToInstantiate(contractType, this._concreteType);
        }
    }
}
