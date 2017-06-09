using VoteResultForAnswerForCurrentQuestionCollector = Network.VoteResultForAnswerForCurrentQuestionCollector;

namespace Tests.Network.VoteForAnswerForCurrentQuestionColletor
{

    using System.Linq;

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

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            var secondsForAnswer = 5;

            this.Container.Bind<int>()
                .FromInstance(secondsForAnswer);

            var gameDataIterator = new DummyGameDataIterator()
                                   {
                                       CurrentQuestion = question,
                                       CurrentMark = 2,
                                       SecondsForAnswerQuestion = secondsForAnswer,
                                       Loaded = true
                                   };
            
            this.Container.Bind<IGameDataIterator>()
                .FromInstance(gameDataIterator)
                .AsSingle();

            var mainPlayers = Enumerable.Range(1, 4);
            var server = new DummyEveryBodyVsTheTeacherServer()
                         {
                             MainPlayersConnectionIds = mainPlayers,
                             StartedGame = true
                         };
            
            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(server)
                .AsSingle();

            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();
            
            this.Container.Bind<ICollectVoteResultForAnswerForCurrentQuestion>()
                .To<VoteResultForAnswerForCurrentQuestionCollector>();
        }
    }
}