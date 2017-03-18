namespace Zenject.Source.Binding.Finalizers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers;

    public abstract class ProviderBindingFinalizer : IBindingFinalizer
    {
        public ProviderBindingFinalizer(BindInfo bindInfo)
        {
            this.BindInfo = bindInfo;
        }

        public bool CopyIntoAllSubContainers
        {
            get
            {
                return this.BindInfo.CopyIntoAllSubContainers;
            }
        }

        protected BindInfo BindInfo
        {
            get;
            private set;
        }

        public void FinalizeBinding(DiContainer container)
        {
            if (this.BindInfo.ContractTypes.IsEmpty())
            {
                // We could assert her instead but it is nice when used with things like
                // BindAllInterfaces() (and there aren't any interfaces) to allow
                // interfaces to be added later
                return;
            }

            this.OnFinalizeBinding(container);

            if (this.BindInfo.NonLazy)
            {
                container.BindRootResolve(
                    this.BindInfo.Identifier, this.BindInfo.ContractTypes.ToArray());
            }
        }

        protected abstract void OnFinalizeBinding(DiContainer container);

        protected void RegisterProvider<TContract>(
            DiContainer container, IProvider provider)
        {
            this.RegisterProvider(container, typeof(TContract), provider);
        }

        protected void RegisterProvider(
            DiContainer container, Type contractType, IProvider provider)
        {
            container.RegisterProvider(
                new BindingId(contractType, this.BindInfo.Identifier),
                this.BindInfo.Condition,
                provider);

            if (contractType.IsValueType())
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(contractType);

                // Also bind to nullable primitives
                // this is useful so that we can have optional primitive dependencies
                container.RegisterProvider(
                    new BindingId(nullableType, this.BindInfo.Identifier),
                    this.BindInfo.Condition,
                    provider);
            }
        }

        protected void RegisterProviderPerContract(
            DiContainer container, Func<DiContainer, Type, IProvider> providerFunc)
        {
            foreach (var contractType in this.BindInfo.ContractTypes)
            {
                this.RegisterProvider(container, contractType, providerFunc(container, contractType));
            }
        }

        protected void RegisterProviderForAllContracts(
            DiContainer container, IProvider provider)
        {
            foreach (var contractType in this.BindInfo.ContractTypes)
            {
                this.RegisterProvider(container, contractType, provider);
            }
        }

        protected void RegisterProvidersPerContractAndConcreteType(
            DiContainer container,
            List<Type> concreteTypes,
            Func<Type, Type, IProvider> providerFunc)
        {
            Assert.That(!this.BindInfo.ContractTypes.IsEmpty());
            Assert.That(!concreteTypes.IsEmpty());

            foreach (var contractType in this.BindInfo.ContractTypes)
            {
                foreach (var concreteType in concreteTypes)
                {
                    if (this.ValidateBindTypes(concreteType, contractType))
                    {
                        this.RegisterProvider(
                            container, contractType, providerFunc(contractType, concreteType));
                    }
                }
            }
        }

        // Returns true if the bind should continue, false to skip
        bool ValidateBindTypes(Type concreteType, Type contractType)
        {
            if (concreteType.DerivesFromOrEqual(contractType))
            {
                return true;
            }

            if (this.BindInfo.InvalidBindResponse == InvalidBindResponses.Assert)
            {
                throw Assert.CreateException(
                    "Expected type '{0}' to derive from or be equal to '{1}'", concreteType, contractType);
            }

            Assert.IsEqual(this.BindInfo.InvalidBindResponse, InvalidBindResponses.Skip);
            return false;
        }

        // Note that if multiple contract types are provided per concrete type,
        // it will re-use the same provider for each contract type
        // (each concrete type will have its own provider though)
        protected void RegisterProvidersForAllContractsPerConcreteType(
            DiContainer container,
            List<Type> concreteTypes,
            Func<DiContainer, Type, IProvider> providerFunc)
        {
            Assert.That(!this.BindInfo.ContractTypes.IsEmpty());
            Assert.That(!concreteTypes.IsEmpty());

            var providerMap = concreteTypes.ToDictionary(x => x, x => providerFunc(container, x));

            foreach (var contractType in this.BindInfo.ContractTypes)
            {
                foreach (var concreteType in concreteTypes)
                {
                    if (this.ValidateBindTypes(concreteType, contractType))
                    {
                        this.RegisterProvider(container, contractType, providerMap[concreteType]);
                    }
                }
            }
        }
    }
}

