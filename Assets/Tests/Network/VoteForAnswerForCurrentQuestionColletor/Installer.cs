using VoteResultForAnswerForCurrentQuestionCollector = Network.VoteResultForAnswerForCurrentQuestionCollector;

namespace Tests.Network.VoteForAnswerForCurrentQuestionColletor
{
    using System.Linq;

    using Assets.Tests.Utils;

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
            var question = new QuestionGenerator().GenerateQuestion();

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
                             ConnectedMainPlayersConnectionIds = mainPlayers,
                             StartedGame = true
                         };
            
            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(server)
                .AsSingle();

            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance);
            
            this.Container.Bind<ICollectVoteResultForAnswerForCurrentQuestion>()
                .To<VoteResultForAnswerForCurrentQuestionCollector>();
        }
    }
}