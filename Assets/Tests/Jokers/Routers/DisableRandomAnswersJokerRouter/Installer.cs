using Zenject;
using Interfaces.Network.Jokers.Routers;
using Tests.DummyObjects;
using Assets.Tests.Utils;
using Interfaces.GameData;
using Interfaces.Network.NetworkManager;
using DisableRandomAnswersJokerRouterClass = Jokers.Routers.DisableRandomAnswersJokerRouter;
using Utils;

namespace Tests.Jokers.Routers.DisableRandomAnswersJokerRouter
{
    using System;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IDisableRandomAnswersRouter>()
                .To<DisableRandomAnswersJokerRouterClass>()
                .AsTransient();

            Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();
            
            var question = new QuestionGenerator().GenerateQuestion();

            Container.Bind<Interfaces.ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();

            var gameDataIterator = new DummyGameDataIterator();
            gameDataIterator.CurrentQuestion = question;

            Container.Bind<IGameDataIterator>()
                .FromInstance(gameDataIterator)
                .AsTransient();

            Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance)
                .AsSingle();
        }
    }
}

