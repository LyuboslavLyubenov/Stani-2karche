using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Network
{
    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;

    public class QuestionsRemainingUICommandsSender : IQuestionsRemainingCommandsSender
    {
        private readonly IEveryBodyVsTheTeacherServer server;

        private readonly IServerNetworkManager networkManager;
        private readonly IGameDataIterator gameDataIterator;

        private readonly PresenterConnectingCommand presenterConnectingCommand;
        
        public QuestionsRemainingUICommandsSender(
            IServerNetworkManager networkManager,
            IEveryBodyVsTheTeacherServer server, 
            IGameDataIterator gameDataIterator)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            this.networkManager = networkManager;
            this.server = server;
            this.gameDataIterator = gameDataIterator;

            this.presenterConnectingCommand = new PresenterConnectingCommand(this.OnPresenterConnecting);
            this.networkManager.CommandsManager.AddCommand(presenterConnectingCommand);
            this.gameDataIterator.OnNextQuestionLoaded += OnNextQuestionLoaded;
        }

        private void OnPresenterConnecting(int connectionId)
        {
            this.SendQuestionsRemainingCountToPresenter();
        }

        private void OnNextQuestionLoaded(object sender, EventArgs args)
        {
            this.SendQuestionsRemainingCountToPresenter();
        }

        private void SendQuestionsRemainingCountToPresenter()
        {
            var questionsRemainingCountCommand = NetworkCommandData.From<LoadQuestionRemainingCountCommand>();
            questionsRemainingCountCommand.AddOption("Count", this.gameDataIterator.RemainingQuestionsToNextMark.ToString());
            this.networkManager.SendClientCommand(this.server.PresenterId, questionsRemainingCountCommand);
        }

        public void Dispose()
        {
            if (this.networkManager.CommandsManager.Exists(this.presenterConnectingCommand))
            {
                this.networkManager.CommandsManager.RemoveCommand(this.presenterConnectingCommand);
            }
        }
    }
}