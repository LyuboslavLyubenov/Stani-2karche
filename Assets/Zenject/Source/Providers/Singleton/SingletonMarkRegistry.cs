namespace Assets.Zenject.Source.Providers.Singleton
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Internal;

    public class SingletonMarkRegistry
    {
        readonly Dictionary<SingletonId, SingletonTypes> _singletonTypes = new Dictionary<SingletonId, SingletonTypes>();

        public SingletonTypes? TryGetSingletonType<T>()
        {
            return this.TryGetSingletonType(typeof(T));
        }

        public SingletonTypes? TryGetSingletonType(Type type)
        {
            return this.TryGetSingletonType(type, null);
        }

        public SingletonTypes? TryGetSingletonType(Type type, object concreteIdentifier)
        {
            return this.TryGetSingletonType(new SingletonId(type, concreteIdentifier));
        }

        public SingletonTypes? TryGetSingletonType(SingletonId id)
        {
            SingletonTypes type;

            if (this._singletonTypes.TryGetValue(id, out type))
            {
                return type;
            }

            return null;
        }

        public void MarkSingleton(
            Type type, object concreteIdentifier, SingletonTypes singletonType)
        {
            this.MarkSingleton(new SingletonId(type, concreteIdentifier), singletonType);
        }

        public void MarkSingleton(SingletonId id, SingletonTypes type)
        {
            SingletonTypes existingType;

            if (this._singletonTypes.TryGetValue(id, out existingType))
            {
                if (existingType != type)
                {
                    throw Assert.CreateException(
                        "Cannot use both '{0}' and '{1}' for the same type/concreteIdentifier!", existingType, type);
                }
            }
            else
            {
                this._singletonTypes.Add(id, type);
            }
        }
    }
}
