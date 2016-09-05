using System.Collections.Generic;

public class GameDataGetNextQuestionCommand : GameDataGetQuestionAbstractCommand
{
    public GameDataGetNextQuestionCommand(LocalGameData gameData, ServerNetworkManager networkManager)
        : base(gameData, networkManager)
    {       
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        var question = base.GameData.GetNextQuestion();
        var requestType = QuestionRequestType.Next;
        base.SendQuestion(connectionId, question, requestType);
    }
}
