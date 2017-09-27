using IAskPlayerQuestionJokerRouter = Interfaces.Network.Jokers.Routers.IAskPlayerQuestionJokerRouter;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using SelectedElectionJokerCommand = Commands.Jokers.Selected.SelectedElectionJokerCommand;
using Network;
using Jokers;

namespace Assets.Scripts.Commands.Jokers.Selected
{
    using System;

    using Assets.Scripts.Utils;

    public class SelectedHelpFromFriendElectionJoker : SelectedElectionJokerCommand
    {
        private readonly IAskPlayerQuestionJokerRouter askPlayerQuestionJoker;

        private readonly IServerNetworkManager networkManager;

        private readonly IGameDataIterator gameDataIterator;

        public SelectedHelpFromFriendElectionJoker(
            IEveryBodyVsTheTeacherServer server,
            JokersData jokersData,
            IAskPlayerQuestionJokerRouter askPlayerQuestionJoker,
            IServerNetworkManager networkManager,
            IGameDataIterator gameDataIterator,
            int selectThisJokerTimeoutInSeconds = MinTimeTimeoutInSeconds)
            : base(server, jokersData, typeof(HelpFromFriendJoker), selectThisJokerTimeoutInSeconds)
        {
            if (askPlayerQuestionJoker == null)
            {
                throw new ArgumentNullException("askPlayerQuestionJoker");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            this.askPlayerQuestionJoker = askPlayerQuestionJoker;
            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;
        }

        protected override void ActivateRouter()
        {
            var friendConnectionId = this.networkManager.ConnectedClientsConnectionId.GetRandomElement();
            this.askPlayerQuestionJoker.Activate(this.server.PresenterId, friendConnectionId, this.gameDataIterator.SecondsForAnswerQuestion);
        }
    }
}
