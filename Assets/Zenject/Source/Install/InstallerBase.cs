namespace Assets.Zenject.Source.Install
{

    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Usage;

    public abstract class InstallerBase : IInstaller
    {
        [Inject]
        DiContainer _container = null;

        protected DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        public virtual bool IsEnabled
        {
            get
            {
                return true;
            }
        }

        public abstract void InstallBindings();
    }
}

