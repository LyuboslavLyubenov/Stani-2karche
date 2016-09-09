using System.Collections.Generic;
using UnityEngine;

public class GameDataGetRandomQuestionCommand : GameDataGetQuestionAbstractCommand
{
    public GameDataGetRandomQuestionCommand(LocalGameData gameData, ServerNetworkManager networkManager)
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
