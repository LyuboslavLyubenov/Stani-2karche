namespace Zenject.Source.Factories
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Providers;
    using Zenject.Source.Usage;
    using Zenject.Source.Util;
    using Zenject.Source.Validation;

    public interface IDynamicFactory : IValidatable
    {
    }

    // Dynamic factories can be used to choose a creation method in an installer, using FactoryBinder
    public abstract class DynamicFactory<TValue> : IDynamicFactory
    {
        IProvider _provider;
        InjectContext _injectContext;

        [Inject]
        void Init(IProvider provider, InjectContext injectContext)
        {
            Assert.IsNotNull(provider);
            Assert.IsNotNull(injectContext);

            this._provider = provider;
            this._injectContext = injectContext;
        }

        protected TValue CreateInternal(List<TypeValuePair> extraArgs)
        {
            try
            {
                var result = this._provider.GetInstance(this._injectContext, extraArgs);

                Assert.That(result == null || result.GetType().DerivesFromOrEqual<TValue>());

                return (TValue)result;
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Error during construction of type '{0}' via {1}.Create method!".Fmt(typeof(TValue).Name(), this.GetType().Name()), e);
            }
        }

        public virtual void Validate()
        {
            try
            {
                this._provider.GetInstance(
                    this._injectContext, ValidationUtil.CreateDefaultArgs(this.ParamTypes.ToArray()));
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Validation for factory '{0}' failed".Fmt(this.GetType().Name()), e);
            }
        }

        protected abstract IEnumerable<Type> ParamTypes
        {
            get;
        }
    }
}
