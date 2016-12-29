using System.Collections.Generic;

namespace Assets.Scripts.Commands.GameData
{

    using Assets.Scripts.IO;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    using Debug = UnityEngine.Debug;

    public class GetCurrentQuestionCommand : GameDataGetQuestionAbstractCommand
    {
        public GetCurrentQuestionCommand(GameDataIterator gameData, ServerNetworkManager networkManager)
            : base(gameData, networkManager)
        {       
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);

            base.GameData.GetCurrentQuestion((question) =>
                {
                    var requestType = QuestionRequestType.Current;
                    base.SendQuestion(connectionId, question, requestType);
                }, 
                Debug.LogException);
        }
    }

}
