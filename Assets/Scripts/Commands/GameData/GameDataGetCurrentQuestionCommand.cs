using System.Collections.Generic;

public class GameDataGetCurrentQuestionCommand : GameDataGetQuestionAbstractCommand
{
    public GameDataGetCurrentQuestionCommand(LocalGameData gameData, ServerNetworkManager networkManager)
        : base(gameData, networkManager)
    {       
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var question = base.GameData.GetCurrentQuestion();
        var requestType = QuestionRequestType.Current;
        base.SendQuestion(connectionId, question, requestType);
    }
}
