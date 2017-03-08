namespace Assets.Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;

    public class InstanceProvider : IProvider
    {
        readonly object _instance;
        readonly Type _instanceType;
        readonly LazyInstanceInjector _lazyInjector;

        public InstanceProvider(
            DiContainer container, Type instanceType, object instance)
        {
            this._instanceType = instanceType;
            this._instance = instance;
            this._lazyInjector = container.LazyInstanceInjector;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._instanceType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(this._instanceType.DerivesFromOrEqual(context.MemberType));

            this._lazyInjector.OnInstanceResolved(this._instance);

            yield return new List<object>() { this._instance };
        }
    }
}
