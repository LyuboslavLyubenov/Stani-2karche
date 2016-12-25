using System.Collections.Generic;

namespace Assets.Scripts.Commands.GameData
{

    using Assets.Scripts.Network;

    using Debug = UnityEngine.Debug;

    public class ReceivedGetRandomQuestionCommand : GameDataGetQuestionAbstractCommand
    {
        public ReceivedGetRandomQuestionCommand(LocalGameData gameData, ServerNetworkManager networkManager)
            : base(gameData, networkManager)
        {       
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

            base.GameData.GetRandomQuestion((question) =>
                {
                    var requestType = QuestionRequestType.Random;
                    base.SendQuestion(connectionId, question, requestType);    
                },
                Debug.LogException);
        
        }
    }

}
