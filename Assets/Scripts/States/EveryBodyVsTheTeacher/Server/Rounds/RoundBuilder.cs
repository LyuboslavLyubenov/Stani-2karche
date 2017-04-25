using ICollectVoteResultForAnswerForCurrentQuestion = Interfaces.Network.ICollectVoteResultForAnswerForCurrentQuestion;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using JokersData = Network.JokersData;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds
{
    public abstract class RoundBuilder
    {
        public IServerNetworkManager ServerNetworkManager
        {
            get;
            set;
        }

        public IEveryBodyVsTheTeacherServer Server
        {
            get;
            set;
        }

        public IGameDataIterator GameDataIterator
        {
            get;
            set;
        }
        
        public ICollectVoteResultForAnswerForCurrentQuestion CurrentQuestionAnswersCollector
        {
            get;
            set;
        }

        public JokersData JokersData
        {
            get;
            set;
        }
    }
}
