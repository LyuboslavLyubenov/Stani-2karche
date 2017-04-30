namespace Assets.Tests.Utils.UnityTimer
{
    using Assets.Scripts.Interfaces.Utils;

    using Controllers;
    
    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IUnityTimer>()
                .To<UnityTimer>()
                .FromGameObject();
        }
    }
}