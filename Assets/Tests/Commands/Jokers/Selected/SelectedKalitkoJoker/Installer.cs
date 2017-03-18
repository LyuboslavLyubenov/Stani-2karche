using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using Interfaces.Network;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

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