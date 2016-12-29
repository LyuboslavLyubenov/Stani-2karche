using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Network.TcpSockets;

    public class CreatedGameInfoReceiverService : MonoBehaviour
    {
        public SimpleTcpClient TcpClient;
        public SimpleTcpServer TcpServer;

        public Dictionary<string, Action<GameInfoReceivedDataEventArgs>> pendingRequests = new Dictionary<string, Action<GameInfoReceivedDataEventArgs>>();

        // Use this for initialization
        void Start()
        {
            if (!this.TcpClient.Initialized)
            {
                this.TcpClient.Initialize();
            }

            if (!this.TcpServer.Initialized)
            {
                this.TcpServer.Initialize(7774);
            }

            this.TcpServer.OnReceivedMessage += this.OnReceivedMessage;
        }

        void OnReceivedMessage(object sender, MessageEventArgs args)
        {
            var gameInfoTagIndex = args.Message.IndexOf(CreatedGameInfoSenderService.GameInfoTag);

            if (!this.pendingRequests.ContainsKey(args.IPAddress) || gameInfoTagIndex < 0)
            {
                return;
            }

            var filteredMessage = args.Message.Remove(gameInfoTagIndex, CreatedGameInfoSenderService.GameInfoTag.Length);
            var gameInfo = new GameInfoReceivedDataEventArgs(filteredMessage);

            this.pendingRequests[args.IPAddress](gameInfo);
            this.pendingRequests.Remove(args.IPAddress);
        }

        public void ReceiveFrom(string ipAddress, Action<GameInfoReceivedDataEventArgs> receivedGameInfo)
        {
            this.TcpClient.ConnectTo(ipAddress, this.TcpServer.Port, () =>
                {
                    this.TcpClient.Send(ipAddress, CreatedGameInfoSenderService.SendGameInfoCommandTag);
                    this.pendingRequests.Add(ipAddress, receivedGameInfo);
                });
        }

        public void StopReceivingFrom(string ipAddress)
        {
            if (!this.pendingRequests.ContainsKey(ipAddress))
            {
                throw new InvalidOperationException("Not listening to this ipAddress");
            }

            this.pendingRequests.Remove(ipAddress);
            this.TcpClient.DisconnectFrom(ipAddress);
            this.TcpServer.Disconnect(ipAddress);
        }
    }

}
