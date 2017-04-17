namespace States.EveryBodyVsTheTeacher.Server
{

    using Commands.Jokers.Selected;

    using Interfaces.Commands.Jokers.Selected;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Jokers.Routers;

    using Zenject.Source.Install;

    public class FirstRoundInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .FromResolve();

            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromResolve();

            this.Container.Bind<IGameDataIterator>()
                .FromResolve();

            var networkManager = this.Container.Resolve<IServerNetworkManager>();
            var server = this.Container.Resolve<IEveryBodyVsTheTeacherServer>();
            var gameDataIterator = this.Container.Resolve<IGameDataIterator>();
            var kalitkoJokerRouter = new KalitkoJokerRouter(networkManager, server, gameDataIterator);
            var trustRandomPersonJokerRouter = new TrustRandomPersonJokerRouter(networkManager, server, gameDataIterator);
            var disableRandomAnswersJokerRouter = new DisableRandomAnswersJokerRouter(networkManager);

            var selectedKalitkoJokerCommand = new SelectedKalitkoJokerCommand(server, kalitkoJokerRouter);
            var selectedTrustRandomPersonJokerCommand = new SelectedTrustRandomPersonJokerCommand(server, trustRandomPersonJokerRouter);
            var selectedConsultWithTeacherJokerCommand = new SelectedConsultWithTeacherJokerCommand(server, disableRandomAnswersJokerRouter);

            var commands = new IElectionJokerCommand[]
                           {
                               selectedKalitkoJokerCommand,
                               selectedTrustRandomPersonJokerCommand,
                               selectedConsultWithTeacherJokerCommand
                           };

            this.Container.Bind<IElectionJokerCommand[]>()
                .FromInstance(commands)
                .WhenInjectedInto<FirstRoundState>();
        }
    }

}