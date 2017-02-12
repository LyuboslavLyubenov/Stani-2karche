namespace Assets.Tests.UI.EverybodyVsTheTeacher
{

    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.Interfaces;

    using Zenject;

    public class Installer : MonoInstaller {
    
        public override void InstallBindings()
        {
            var dummyNetworkManager = DummyServerNetworkManager.Instance;

            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(dummyNetworkManager)
                .AsSingle();

            this.Container.Bind<IEverybodyVsTheTeacherServer>()
                .To<EveryBodyVsTheTeacherServer>()
                .FromGameObject();
        }
    }

}
