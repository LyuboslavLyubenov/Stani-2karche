namespace Assets.Zenject.Source.Providers.Singleton.Standard
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Injection;

    public class StandardSingletonDeclaration
    {
        public StandardSingletonDeclaration(
            SingletonId id, List<TypeValuePair> args, SingletonTypes type, object singletonSpecificId)
        {
            this.Id = id;
            this.Type = type;
            this.SpecificId = singletonSpecificId;
            this.Arguments = args;
        }

        public StandardSingletonDeclaration(
            Type concreteType, object concreteIdentifier, List<TypeValuePair> args,
            SingletonTypes type, object singletonSpecificId)
            : this(
                new SingletonId(concreteType, concreteIdentifier), args,
                type, singletonSpecificId)
        {
        }

        public List<TypeValuePair> Arguments
        {
            get;
            private set;
        }

        public SingletonId Id
        {
            get;
            private set;
        }

        public SingletonTypes Type
        {
            get;
            private set;
        }

        public object SpecificId
        {
            get;
            private set;
        }
    }
}

