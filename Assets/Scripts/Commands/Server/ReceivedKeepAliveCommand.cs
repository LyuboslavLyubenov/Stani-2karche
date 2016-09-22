using System.Collections.Generic;

public class ReceivedKeepAliveCommand : INetworkManagerCommand
{
    ICollection<int> aliveClientsIds;

    public ReceivedKeepAliveCommand(ICollection<int> aliveClientsIds)
    {
        if (aliveClientsIds == null)
        {
            throw new System.ArgumentNullException("aliveClientsIds");
        }

        this.aliveClientsIds = aliveClientsIds;
    }

    public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
    {
        var connectionId = int.Parse(commandsOptionsValues["ConnectionId"]);
        aliveClientsIds.Add(connectionId);
    }
}
