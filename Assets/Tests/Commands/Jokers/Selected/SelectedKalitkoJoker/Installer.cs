using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using Assets.Scripts.Interfaces.Network;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;
    using Assets.Zenject.Source.Usage;

    public class Installer : MonoInstaller
    {

        public override void InstallBindings()
        {
            var server = new DummyEveryBodyVsTheTeacherServer();
            server.StartedGame = true;

            Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(server)
                .AsSingle();

            Container.Bind<int>()
                .FromInstance(5)
                .WhenInjectedInto<SelectedKalitkoJokerCommand>();

            Container.Bind<SelectedKalitkoJokerCommand>()
                .ToSelf();
        }
    }
}