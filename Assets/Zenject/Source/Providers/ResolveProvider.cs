namespace Assets.Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Usage;

    public class ResolveProvider : IProvider
    {
        readonly object _identifier;
        readonly DiContainer _container;
        readonly Type _contractType;
        readonly bool _isOptional;

        public ResolveProvider(
            Type contractType, DiContainer container, object identifier, bool isOptional)
        {
            this._contractType = contractType;
            this._identifier = identifier;
            this._container = container;
            this._isOptional = isOptional;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._contractType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(this._contractType.DerivesFromOrEqual(context.MemberType));

            yield return this._container.ResolveAll(this.GetSubContext(context)).Cast<object>().ToList();
        }

        InjectContext GetSubContext(InjectContext parent)
        {
            var subContext = parent.CreateSubContext(this._contractType, this._identifier);

            subContext.SourceType = InjectSources.Any;
            subContext.Optional = this._isOptional;

            return subContext;
        }
    }
}
