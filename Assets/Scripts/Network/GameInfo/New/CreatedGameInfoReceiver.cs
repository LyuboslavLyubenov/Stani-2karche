using GameInfoEventArgs = EventArgs.GameInfoEventArgs;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using ICreatedGameInfoReceiver = Interfaces.Network.NetworkManager.ICreatedGameInfoReceiver;
using NetworkCommandData = Commands.NetworkCommandData;
using ThreadUtils = Utils.ThreadUtils;

namespace Assets.Scripts.Network.GameInfo.New
{
    using System;
    using System.Collections;

    using Assets.Scripts.Commands.CreatedGameInfoReceiver;
    using Assets.Scripts.Commands.CreatedGameInfoSender;

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
            var sendGameInfoCommand = NetworkCommandData.From<SendGameInfoCommand>();
            this.networkManager.SendServerCommand(sendGameInfoCommand);

            this.receivedGameInfoCommand.AllowToReceiveFrom(ipAddress, receivedGameInfo,
                () =>
                    {
                        if (onError != null)
                        {
                            onError(new TimeoutException());
                        }
                    });
        }

        private IEnumerator ReceiveFromCoroutine(
            string ipAddress,
            Action<GameInfoEventArgs> receivedGameInfo,
            Action<Exception> onError = null)
        {
            if (this.networkManager.IsConnected)
            {
                this.networkManager.Disconnect();
                yield return null;
            }

            this.networkManager.ConnectToHost(ipAddress);

            yield return new WaitUntil(() => this.networkManager.IsConnected);

            this.StartReceivingFrom(ipAddress, receivedGameInfo,
                (exception) =>
                    {
                        if (this.networkManager.IsConnected)
                        {
                            this.networkManager.Disconnect();
                        }
                        
                        if (onError != null)
                        {
                            onError(exception);
                        }
                    });
        }

        public void ReceiveFrom(
            string ipAddress, 
            Action<GameInfoEventArgs> receivedGameInfo, 
            Action<Exception> onError = null)
        {
            ThreadUtils.Instance.RunOnMainThread(
                this.ReceiveFromCoroutine(ipAddress, receivedGameInfo, onError));
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