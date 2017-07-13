using DummyGameDataIterator = Tests.DummyObjects.DummyGameDataIterator;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.States.Server.EndGame
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using DTOs;

    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance);

            var gameDataIterator = new DummyGameDataIterator()
                                   {
                                       CurrentMark = 2,
                                       CurrentQuestion = new SimpleQuestion(
                                           "QuestionText",
                                           new[]
                                           {
                                               "Answer1",
                                               "Answer2",
                                               "Answer3",
                                               "Answer4"
                                           },
                                           0),
                                       Loaded = true
                                   };

            Container.Bind<IGameDataIterator>()
                .FromInstance(gameDataIterator)
                .AsSingle();

            Container.Bind<EndGameState>()
                .ToSelf();
        }
    }
}