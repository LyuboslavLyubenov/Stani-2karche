﻿using System;

namespace Assets.Scripts.Commands.Client
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Utils;

    public class ReceivedConnectedClientsCountCommand : INetworkManagerCommand
    {
        ValueWrapper<int> serverConnectedClientsCount;

        public ReceivedConnectedClientsCountCommand(ValueWrapper<int> serverConnectedClientsCount)
        {
            if (serverConnectedClientsCount == null)
            {
                throw new ArgumentNullException("serverConnectedClientsCount");
            }
            
            this.serverConnectedClientsCount = serverConnectedClientsCount;       
        }

        public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
        {
            var clientsCount = int.Parse(commandsOptionsValues["ConnectedClientsCount"]);
            this.serverConnectedClientsCount.Value = clientsCount;
        }
    }

}
