using System.Collections.Generic;

namespace Assets.Scripts.Commands.GameData
{

    using Assets.Scripts.Network;

    using Debug = UnityEngine.Debug;

    public class ReceivedGetNextQuestionCommand : GameDataGetQuestionAbstractCommand
    {
        public ReceivedGetNextQuestionCommand(LocalGameData gameData, ServerNetworkManager networkManager)
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
