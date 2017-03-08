namespace Assets.Zenject.Source.Binding.Binders.Factory.FactoryFromBinder.SubContainerBinder
{

    using System;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Binding.Finalizers;
    using Assets.Zenject.Source.Install;
    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Providers;
    using Assets.Zenject.Source.Providers.SubContainerCreators;

    public class FactorySubContainerBinderBase<TContract>
    {
        readonly BindFinalizerWrapper _finalizerWrapper;

        public FactorySubContainerBinderBase(
            BindInfo bindInfo, Type factoryType,
            BindFinalizerWrapper finalizerWrapper, object subIdentifier)
        {
            this.SubIdentifier = subIdentifier;
            this.BindInfo = bindInfo;
            this.FactoryType = factoryType;

            this._finalizerWrapper = finalizerWrapper;

            // Reset so we get errors if we end here
            finalizerWrapper.SubFinalizer = null;
        }

        protected Type FactoryType
        {
            get;
            private set;
        }

        protected BindInfo BindInfo
        {
            get;
            private set;
        }

        protected object SubIdentifier
        {
            get;
            private set;
        }

        protected Type ContractType
        {
            get
            {
                return typeof(TContract);
            }
        }

        protected IBindingFinalizer SubFinalizer
        {
            set
            {
                this._finalizerWrapper.SubFinalizer = value;
            }
        }

        protected IBindingFinalizer CreateFinalizer(Func<DiContainer, IProvider> providerFunc)
        {
            return new DynamicFactoryBindingFinalizer<TContract>(
                this.BindInfo, this.FactoryType, providerFunc);
        }

        public ConditionBinder ByInstaller<TInstaller>()
            where TInstaller : InstallerBase
        {
            return this.ByInstaller(typeof(TInstaller));
        }

        public ConditionBinder ByInstaller(Type installerType)
        {
            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType.Name());

            this.SubFinalizer = this.CreateFinalizer(
                (container) => new SubContainerDependencyProvider(
                    this.ContractType, this.SubIdentifier,
                    new SubContainerCreatorByInstaller(
                        container, installerType)));

            return new ConditionBinder(this.BindInfo);
        }
    }
}
