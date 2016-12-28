using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Commands.Client
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;

    using EventArgs = System.EventArgs;

    public class ConnectedClientsDataCommand : IOneTimeExecuteCommand
    {
        Action<OnlineClientsData_Serializable> onReceivedOnlineClients;

        public bool FinishedExecution
        {
            get;
            private set;
        }

        public EventHandler OnFinishedExecution
        {
            get;
            set;
        }

        public ConnectedClientsDataCommand(Action<OnlineClientsData_Serializable> onReceivedOnlineClientsData)
        {
            if (onReceivedOnlineClientsData == null)
            {
                throw new ArgumentNullException("onReceivedOnlineClientsData");
            }
            
            this.onReceivedOnlineClients = onReceivedOnlineClientsData;
            this.FinishedExecution = false;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectedClientsDataJSON = commandsOptionsValues["ConnectedClientsDataJSON"];
            var onlineClientsData = JsonUtility.FromJson<OnlineClientsData_Serializable>(connectedClientsDataJSON);
            this.onReceivedOnlineClients(onlineClientsData);
            this.FinishedExecution = true;

            if (this.OnFinishedExecution != null)
            {
                this.OnFinishedExecution(this, EventArgs.Empty);
            }
        }
    }

}
