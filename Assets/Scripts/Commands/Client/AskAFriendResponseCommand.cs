public class AskAFriendResponseCommand : IOneTimeExecuteCommand
{
    public delegate void OnReceivedResponde(string username,string answer);

    OnReceivedResponde onReceivedResponde;

    public bool FinishedExecution
    {
        get;
        private set;
    }

    public AskAFriendResponseCommand(OnReceivedResponde onReceivedResponde)
    {
        if (onReceivedResponde == null)
        {
            throw new System.ArgumentNullException("onReceivedResponde");
        }
            
        this.onReceivedResponde = onReceivedResponde;
        FinishedExecution = false;
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        var username = commandsOptionsValues["Username"];
        var answer = commandsOptionsValues["Answer"];

        onReceivedResponde(username, answer);
        FinishedExecution = true;
    }
}
