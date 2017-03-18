namespace Commands.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender
{

    using System;
    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    public class AudiencePlayerConnectedCommand : INetworkManagerCommand, INetworkOperationExecutedCallback
    {
        public EventHandler OnExecuted
        {
            get; set;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            throw new NotImplementedException();    
        }
    }
}
