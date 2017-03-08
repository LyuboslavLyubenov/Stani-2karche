namespace Assets.Zenject.Source.Util
{

    using System;

    using Assets.Zenject.Source.Install;
    using Assets.Zenject.Source.Main;

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
