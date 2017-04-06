using VoteResultForAnswerForCurrentQuestionCollector = Network.VoteResultForAnswerForCurrentQuestionCollector;

namespace Tests.Network.VoteForAnswerForCurrentQuestionCollector
{
    using System.Linq;

    using Assets.Scripts.Interfaces.Network;

    using DTOs;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            var question = new SimpleQuestion("QuestionText", new[] { "answer1", "answer2", "answer3", "answer4" }, 0);

            Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            var gameDataIterator = new DummyGameDataIterator()
                                   {
                                       CurrentQuestion = question,
                                       CurrentMark = 2,
                                       SecondsForAnswerQuestion = 5,
                                       Loaded = true
                                   };
            
            Container.Bind<IGameDataIterator>()
                .FromInstance(gameDataIterator)
                .AsSingle();

            var mainPlayers = Enumerable.Range(1, 4);
            var server = new DummyEveryBodyVsTheTeacherServer()
                         {
                             MainPlayersConnectionIds = mainPlayers,
                             StartedGame = true
                         };
            
            Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(server)
                .AsSingle();

            Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();
            
            Container.Bind<ICollectVoteResultForAnswerForCurrentQuestion>()
                .To<VoteResultForAnswerForCurrentQuestionCollector>();
        }
    }
}