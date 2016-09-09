using System.Collections.Generic;
using UnityEngine;

public class GameDataGetCurrentQuestionCommand : GameDataGetQuestionAbstractCommand
{
    public GameDataGetCurrentQuestionCommand(LocalGameData gameData, ServerNetworkManager networkManager)
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
