namespace Zenject.Source.Providers.SubContainerCreators
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Injection;
    using Zenject.Source.Install;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;

    public class SubContainerCreatorByInstaller : ISubContainerCreator
    {
        readonly Type _installerType;
        readonly DiContainer _container;

        public SubContainerCreatorByInstaller(
            DiContainer container, Type installerType)
        {
            this._installerType = installerType;
            this._container = container;

            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType.Name());
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args)
        {
            var subContainer = this._container.CreateSubContainer();

            var installer = (InstallerBase)subContainer.InstantiateExplicit(
                this._installerType, args);
            installer.InstallBindings();

            subContainer.ResolveDependencyRoots();

            return subContainer;
        }
    }
}
