namespace Assets.Tests.UI.EverybodyVsTheTeacher
{

    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.States.EverybodyVsTheTeacherServer;

    using Zenject;

    public class Installer : MonoInstaller {
    
        public override void InstallBindings()
        {
            var dummyNetworkManager = DummyServerNetworkManager.Instance;

            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(dummyNetworkManager)
                .AsSingle();

            this.Container.Bind<PlayersConnectingToTheServerState>().AsSingle(); 
        }
    }

}
