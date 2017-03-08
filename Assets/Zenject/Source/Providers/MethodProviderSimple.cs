namespace Assets.Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;

    public class MethodProviderSimple<TReturn> : IProvider
    {
        readonly Func<TReturn> _method;

        public MethodProviderSimple(Func<TReturn> method)
        {
            this._method = method;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(TReturn);
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(typeof(TReturn).DerivesFromOrEqual(context.MemberType));

            yield return new List<object>() { this._method() };
        }
    }
}
