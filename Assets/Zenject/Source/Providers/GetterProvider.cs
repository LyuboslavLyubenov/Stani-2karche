namespace Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Validation;

    public class GetterProvider<TObj, TResult> : IProvider
    {
        readonly DiContainer _container;
        readonly object _identifier;
        readonly Func<TObj, TResult> _method;

        public GetterProvider(
            object identifier, Func<TObj, TResult> method,
            DiContainer container)
        {
            this._container = container;
            this._identifier = identifier;
            this._method = method;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(TResult);
        }

        InjectContext GetSubContext(InjectContext parent)
        {
            var subContext = parent.CreateSubContext(
                typeof(TObj), this._identifier);

            subContext.Optional = false;

            return subContext;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(typeof(TResult).DerivesFromOrEqual(context.MemberType));

            if (this._container.IsValidating)
            {
                // All we can do is validate that the getter object can be resolved
                this._container.Resolve(typeof(TObj));

                yield return new List<object>() { new ValidationMarker(typeof(TResult)) };
            }
            else
            {
                yield return new List<object>() { this._method(
                    this._container.Resolve<TObj>(this.GetSubContext(context))) };
            }
        }
    }
}
