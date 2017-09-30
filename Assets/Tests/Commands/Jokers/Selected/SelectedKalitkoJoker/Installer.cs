using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;
using Network;
using Jokers.EveryBodyVsTheTeacher.Kalitko;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{
    using Interfaces.Network;

    using Tests.DummyObjects;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            var server = new DummyEveryBodyVsTheTeacherServer
                         {
                             StartedGame = true
                         };

            Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(server)
                .AsSingle();

            var jokersData = new JokersData();
            jokersData.AddJoker<KalitkoJoker>();

            Container.Bind<JokersData>()
                .FromInstance(jokersData)
                .AsSingle();
            
            Container.Bind<SelectedKalitkoJokerCommand>()
                .FromMethod(
                    (context) =>
                        {
                            var command =
                                new SelectedKalitkoJokerCommand(
                                    context.Container.Resolve<IEveryBodyVsTheTeacherServer>(),
                                    context.Container.Resolve<JokersData>(),
                                    new DummyKalitkoJokerRouter());
                            return command;
                        });
        }
    }
}