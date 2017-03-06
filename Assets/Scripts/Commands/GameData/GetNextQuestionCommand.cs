using System.Collections.Generic;

namespace Assets.Scripts.Commands.GameData
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.IO;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    using Debug = UnityEngine.Debug;

    public class GetNextQuestionCommand : GameDataGetQuestionAbstractCommand
    {
        public GetNextQuestionCommand(IGameDataIterator gameData, IServerNetworkManager networkManager)
            : base(gameData, networkManager)
        {     
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

            base.GameData.GetNextQuestion((question) =>
                {
                    var requestType = QuestionRequestType.Next;
                    base.SendQuestion(connectionId, question, requestType);    
                }, 
                Debug.LogException);
        }
    }

}
