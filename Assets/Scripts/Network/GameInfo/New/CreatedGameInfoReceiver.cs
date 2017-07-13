﻿using GameInfoEventArgs = EventArgs.GameInfoEventArgs;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using ICreatedGameInfoReceiver = Interfaces.Network.NetworkManager.ICreatedGameInfoReceiver;

namespace Assets.Scripts.Network.GameInfo.New
{
    using System;

    using Assets.Scripts.Commands.CreatedGameInfoReceiver;

    using UnityEngine;

    public class CreatedGameInfoReceiver : ICreatedGameInfoReceiver, IDisposable
    {        
        private const int ReceiveGameInfoTimeoutInSeconds = 5;

        private IClientNetworkManager networkManager;
        private ReceivedGameInfoCommand receivedGameInfoCommand;

        public CreatedGameInfoReceiver(IClientNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
            this.receivedGameInfoCommand = new ReceivedGameInfoCommand(ReceiveGameInfoTimeoutInSeconds);

            networkManager.CommandsManager.AddCommand(receivedGameInfoCommand);
        }

        private void StartReceivingFrom(string ipAddress, Action<GameInfoEventArgs> receivedGameInfo, Action<Exception> onError = null)
        {
            this.receivedGameInfoCommand.AllowToReceiveFrom(ipAddress, receivedGameInfo,
                () =>
                    {
                        if (onError != null)
                        {
                            onError(new TimeoutException());
                        }
                    });
        }

        public void ReceiveFrom(string ipAddress, Action<GameInfoEventArgs> receivedGameInfo, Action<Exception> onError = null)
        {
            if (this.networkManager.IsConnected)
            {
                this.networkManager.Disconnect();
            }

            var connectionResult = this.networkManager.ConnectToHost(ipAddress);
            if (connectionResult == NetworkConnectionError.NoError)
            {
                this.StartReceivingFrom(ipAddress, receivedGameInfo, onError);
            }
            else
            {
                if (onError != null)
                {
                    onError(new TimeoutException());
                }
            }
        }

        public void StopReceivingFrom(string ipAddress)
        {
            this.receivedGameInfoCommand.StopReceiveFrom(ipAddress);
        }

        public void StopReceivingFromAll()
        {
            this.receivedGameInfoCommand.StopAll();
        }

        public void Dispose()
        {
            this.networkManager.CommandsManager.RemoveCommand(this.receivedGameInfoCommand);
        }
    }
}