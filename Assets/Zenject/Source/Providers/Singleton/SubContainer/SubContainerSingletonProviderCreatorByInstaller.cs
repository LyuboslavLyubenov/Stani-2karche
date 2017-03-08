namespace Assets.Zenject.Source.Providers.Singleton.SubContainer
{

    using System;
    using System.Collections.Generic;

    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers.SubContainerCreators;

    public class SubContainerSingletonProviderCreatorByInstaller
    {
        readonly SingletonMarkRegistry _markRegistry;
        readonly DiContainer _container;
        readonly Dictionary<InstallerSingletonId, ISubContainerCreator> _subContainerCreators =
            new Dictionary<InstallerSingletonId, ISubContainerCreator>();

        public SubContainerSingletonProviderCreatorByInstaller(
            DiContainer container,
            SingletonMarkRegistry markRegistry)
        {
            this._markRegistry = markRegistry;
            this._container = container;
        }

        public IProvider CreateProvider(
            Type resultType, object concreteIdentifier, Type installerType, object identifier)
        {
            this._markRegistry.MarkSingleton(
                resultType, concreteIdentifier,
                SingletonTypes.ToSubContainerInstaller);

            var subContainerSingletonId = new InstallerSingletonId(
                concreteIdentifier, installerType);

            ISubContainerCreator subContainerCreator;

            if (!this._subContainerCreators.TryGetValue(subContainerSingletonId, out subContainerCreator))
            {
                subContainerCreator = new SubContainerCreatorCached(
                    new SubContainerCreatorByInstaller(
                        this._container, installerType));

                this._subContainerCreators.Add(subContainerSingletonId, subContainerCreator);
            }

            return new SubContainerDependencyProvider(
                resultType, identifier, subContainerCreator);
        }

        class InstallerSingletonId : IEquatable<InstallerSingletonId>
        {
            public readonly object ConcreteIdentifier;
            public readonly Type InstallerType;

            public InstallerSingletonId(object concreteIdentifier, Type installerType)
            {
                this.ConcreteIdentifier = concreteIdentifier;
                this.InstallerType = installerType;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 29 + (this.ConcreteIdentifier == null ? 0 : this.ConcreteIdentifier.GetHashCode());
                    hash = hash * 29 + this.InstallerType.GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object other)
            {
                if (other is InstallerSingletonId)
                {
                    InstallerSingletonId otherId = (InstallerSingletonId)other;
                    return otherId == this;
                }
                else
                {
                    return false;
                }
            }

            public bool Equals(InstallerSingletonId that)
            {
                return this == that;
            }

            public static bool operator ==(InstallerSingletonId left, InstallerSingletonId right)
            {
                return object.Equals(left.InstallerType, right.InstallerType)
                    && object.Equals(left.ConcreteIdentifier, right.ConcreteIdentifier);
            }

            public static bool operator !=(InstallerSingletonId left, InstallerSingletonId right)
            {
                return !left.Equals(right);
            }
        }
    }
}
