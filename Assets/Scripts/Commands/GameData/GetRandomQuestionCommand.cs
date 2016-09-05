using System.Collections.Generic;

public class GameDataGetRandomQuestionCommand : GameDataGetQuestionAbstractCommand
{
    public GameDataGetRandomQuestionCommand(LocalGameData gameData, ServerNetworkManager networkManager)
        : base(gameData, networkManager)
    {       
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var question = base.GameData.GetRandomQuestion();
        var requestType = QuestionRequestType.Random;
        base.SendQuestion(connectionId, question, requestType);
    }
}
