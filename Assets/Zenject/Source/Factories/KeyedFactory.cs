namespace Zenject.Source.Factories
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Binding.Binders;
    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Usage;
    using Zenject.Source.Util;
    using Zenject.Source.Validation;

    public abstract class KeyedFactoryBase<TBase, TKey> : IValidatable
    {
        [Inject]
        readonly DiContainer _container = null;

        [InjectOptional]
        readonly List<ValuePair<TKey, Type>> _typePairs = null;

        Dictionary<TKey, Type> _typeMap;

        [InjectOptional]
        readonly Type _fallbackType = null;

        protected DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        protected abstract IEnumerable<Type> ProvidedTypes
        {
            get;
        }

        public ICollection<TKey> Keys
        {
            get { return this._typeMap.Keys; }
        }

        protected Dictionary<TKey, Type> TypeMap
        {
            get { return this._typeMap; }
        }

        [Inject]
        public void Initialize()
        {
            Assert.That(this._fallbackType == null || this._fallbackType.DerivesFromOrEqual<TBase>(),
                "Expected fallback type '{0}' to derive from '{1}'", this._fallbackType, typeof(TBase));

            var duplicates = this._typePairs.Select(x => x.First).GetDuplicates();

            if (!duplicates.IsEmpty())
            {
                throw Assert.CreateException(
                    "Found duplicate values in KeyedFactory: {0}", duplicates.Select(x => x.ToString()).Join(", "));
            }

            this._typeMap = this._typePairs.ToDictionary(x => x.First, x => x.Second);
            this._typePairs.Clear();
        }

        public bool HasKey(TKey key)
        {
            return this._typeMap.ContainsKey(key);
        }

        protected Type GetTypeForKey(TKey key)
        {
            Type keyedType;

            if (!this._typeMap.TryGetValue(key, out keyedType))
            {
                Assert.IsNotNull(this._fallbackType, "Could not find instance for key '{0}'", key);
                return this._fallbackType;
            }

            return keyedType;
        }

        public virtual void Validate()
        {
            foreach (var constructType in this._typeMap.Values)
            {
                this.Container.InstantiateExplicit(
                    constructType, ValidationUtil.CreateDefaultArgs(this.ProvidedTypes.ToArray()));
            }
        }

        protected static ConditionBinder AddBindingInternal<TDerived>(DiContainer container, TKey key)
            where TDerived : TBase
        {
            return container.Bind<ValuePair<TKey, Type>>()
                .FromInstance(ValuePair.New(key, typeof(TDerived)));
        }
    }

    // Zero parameters
    public class KeyedFactory<TBase, TKey> : KeyedFactoryBase<TBase, TKey>
    {
        protected override IEnumerable<Type> ProvidedTypes
        {
            get
            {
                return new Type[0];
            }
        }

        public virtual TBase Create(TKey key)
        {
            var type = this.GetTypeForKey(key);
            return (TBase)this.Container.Instantiate(type);
        }
    }

    // One parameter
    public class KeyedFactory<TBase, TKey, TParam1> : KeyedFactoryBase<TBase, TKey>
    {
        protected override IEnumerable<Type> ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1) };
            }
        }

        public virtual TBase Create(TKey key, TParam1 param1)
        {
            return (TBase)this.Container.InstantiateExplicit(
                this.GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InjectUtil.CreateTypePair(param1),
                });
        }
    }

    // Two parameters
    public class KeyedFactory<TBase, TKey, TParam1, TParam2> : KeyedFactoryBase<TBase, TKey>
    {
        protected override IEnumerable<Type> ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1), typeof(TParam2) };
            }
        }

        public virtual TBase Create(TKey key, TParam1 param1, TParam2 param2)
        {
            return (TBase)this.Container.InstantiateExplicit(
                this.GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InjectUtil.CreateTypePair(param1),
                    InjectUtil.CreateTypePair(param2),
                });
        }
    }

    // Three parameters
    public class KeyedFactory<TBase, TKey, TParam1, TParam2, TParam3> : KeyedFactoryBase<TBase, TKey>
    {
        protected override IEnumerable<Type> ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) };
            }
        }

        public virtual TBase Create(TKey key, TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return (TBase)this.Container.InstantiateExplicit(
                this.GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InjectUtil.CreateTypePair(param1),
                    InjectUtil.CreateTypePair(param2),
                    InjectUtil.CreateTypePair(param3),
                });
        }
    }

    // Four parameters
    public class KeyedFactory<TBase, TKey, TParam1, TParam2, TParam3, TParam4> : KeyedFactoryBase<TBase, TKey>
    {
        protected override IEnumerable<Type> ProvidedTypes
        {
            get
            {
                return new Type[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) };
            }
        }

        public virtual TBase Create(TKey key, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return (TBase)this.Container.InstantiateExplicit(
                this.GetTypeForKey(key),
                new List<TypeValuePair>()
                {
                    InjectUtil.CreateTypePair(param1),
                    InjectUtil.CreateTypePair(param2),
                    InjectUtil.CreateTypePair(param3),
                    InjectUtil.CreateTypePair(param4),
                });
        }
    }
}
