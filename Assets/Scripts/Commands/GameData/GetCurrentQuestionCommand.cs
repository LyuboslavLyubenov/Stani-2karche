namespace Commands.GameData
{

    using System.Collections.Generic;

    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using Network;

    using Debug = UnityEngine.Debug;

    public class GetCurrentQuestionCommand : GameDataGetQuestionAbstractCommand
    {
        public GetCurrentQuestionCommand(IGameDataIterator gameData, IServerNetworkManager networkManager)
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
