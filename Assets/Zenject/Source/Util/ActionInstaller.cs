namespace Zenject.Source.Util
{

    using System;

    using Zenject.Source.Install;
    using Zenject.Source.Main;

    public class ActionInstaller : Installer<ActionInstaller>
    {
        readonly Action<DiContainer> _installMethod;

        public ActionInstaller(Action<DiContainer> installMethod)
        {
            this._installMethod = installMethod;
        }

        public override void InstallBindings()
        {
            this._installMethod(this.Container);
        }
    }
}
