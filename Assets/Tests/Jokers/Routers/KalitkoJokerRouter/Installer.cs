namespace Tests.Jokers.Routers.KalitkoJokerRouter
{

    using DTOs;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .To<DummyEveryBodyVsTheTeacherServer>()
                .AsSingle();

            var question = new SimpleQuestion("QuestionText", new[] { "Answer1", "Answer2", "Answer3", "Answer4" }, 0);
            var gameDataIterator = new DummyGameDataIterator()
            {
                CurrentQuestion = question,
                Loaded = true
            };

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();

            this.Container.Bind<IGameDataIterator>()
                .FromInstance(gameDataIterator)
                .AsSingle();
        }
    }

}