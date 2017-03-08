namespace Assets.Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Validation;

    public class MethodProviderUntyped : IProvider
    {
        readonly DiContainer _container;
        readonly Func<InjectContext, object> _method;

        public MethodProviderUntyped(
            Func<InjectContext, object> method,
            DiContainer container)
        {
            this._container = container;
            this._method = method;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return context.MemberType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            if (this._container.IsValidating)
            {
                yield return new List<object>() { new ValidationMarker(context.MemberType) };
            }
            else
            {
                var result = this._method(context);

                if (result == null)
                {
#if !UNITY_WSA
                    Assert.That(context.MemberType.IsPrimitive,
                        "Invalid value returned from FromMethod.  Expected non-null.");
#endif
                }
                else
                {
                    Assert.That(result.GetType().DerivesFromOrEqual(context.MemberType));
                }

                yield return new List<object>() { result };
            }
        }
    }
}

