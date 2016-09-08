using System.Collections.Generic;

public class ServerReceivedAnswerClicked : INetworkManagerCommand
{
    Question currentQuestion;
    ServerNetworkManager networkManager;

    public ServerReceivedAnswerClicked(Question currentQuestion, ServerNetworkManager networkManager)
    {
        if (currentQuestion == null)
        {
            throw new System.ArgumentNullException("currentQuestion");
        }
            
        if (networkManager == null)
        {
            throw new System.ArgumentNullException("networkManager");
        }
            
        this.currentQuestion = currentQuestion;
        this.networkManager = networkManager;
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ClientToSendConnectionId"]);
        var answerClicked = commandsOptionsValues["Answer"];
        var correctAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
        var commandData = new NetworkCommandData(COMMANDS_NAMES.SelectedAnswer);

        commandData.AddOption("IsCorrect", (answerClicked == correctAnswer).ToString());

        networkManager.SendClientCommand(connectionId, commandData);
    }
}
