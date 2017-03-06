using System;
using System.Collections.Generic;

using UnityEngine;
namespace Assets.Scripts.Commands.GameData
{
    using EventArgs;
    using Interfaces;
    using Network;
    using Network.NetworkManagers;

    public abstract class GameDataGetQuestionAbstractCommand : INetworkManagerCommand
    {
        public EventHandler<ServerSentQuestionEventArgs> OnSentQuestion = delegate
            {
            };

        public EventHandler<ServerSentQuestionEventArgs> OnBeforeSend = delegate
            {
            };

        protected IServerNetworkManager NetworkManager
        {
            get;
            private set;
        }

        protected IGameDataIterator GameData
        {
            get;
            private set;
        }

        protected GameDataGetQuestionAbstractCommand(IGameDataIterator gameData, IServerNetworkManager networkManager)
        {
            if (gameData == null)
            {
                throw new ArgumentNullException("gameData");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.NetworkManager = networkManager;
            this.GameData = gameData;
        }

        protected void SendQuestion(int connectionId, ISimpleQuestion question, QuestionRequestType requestType)
        {
            this.OnBeforeSend(this, new ServerSentQuestionEventArgs(question, requestType, connectionId));

            var questionJSON = JsonUtility.ToJson(question.Serialize());
            var commandData = new NetworkCommandData("GameDataQuestion");
            var requestTypeStr = Enum.GetName(typeof(QuestionRequestType), requestType);

            commandData.AddOption("QuestionJSON", questionJSON);
            commandData.AddOption("RemainingQuestionsToNextMark", this.GameData.RemainingQuestionsToNextMark.ToString());
            commandData.AddOption("SecondsForAnswerQuestion", this.GameData.SecondsForAnswerQuestion.ToString());
            commandData.AddOption("RequestType", requestTypeStr);

            this.NetworkManager.SendClientCommand(connectionId, commandData);
            this.OnSentQuestion(this, new ServerSentQuestionEventArgs(question, requestType, connectionId));
        }

        public abstract void Execute(Dictionary<string, string> commandsOptionsValues);
    }

}