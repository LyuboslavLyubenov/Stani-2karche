using GameInfoEventArgs = EventArgs.GameInfoEventArgs;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using TimerUtils = Utils.TimerUtils;

namespace Assets.Scripts.Commands.CreatedGameInfoReceiver
{
    using System;
    using System.Collections.Generic;

    using DTOs;

    using UnityEngine;

    public class ReceivedGameInfoCommand : INetworkManagerCommand
    {
        private readonly int timeoutInSeconds;

        private Dictionary<string, ReceiveGameInfoRequest> requests = new Dictionary<string, ReceiveGameInfoRequest>();

        public ReceivedGameInfoCommand(int timeoutInSeconds)
        {
            if (timeoutInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.timeoutInSeconds = timeoutInSeconds;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var gameInfoJSON = commandsOptionsValues["GameInfoJSON"];
            var gameInfo = JsonUtility.FromJson<CreatedGameInfo_DTO>(gameInfoJSON);
            var serverExternalIp = gameInfo.ServerInfo.ExternalIpAddress;
            var serverLocalIp = gameInfo.ServerInfo.LocalIPAddress;
            
            if (!this.requests.ContainsKey(serverExternalIp) || !this.requests.ContainsKey(serverLocalIp))
            {
                return;
            }

            var request = 
                this.requests.ContainsKey(serverExternalIp) 
                ? 
                this.requests[serverExternalIp] : this.requests[serverLocalIp];

            var gameInfoEventArgs = new GameInfoEventArgs(gameInfoJSON);
            request.OnSuccess(gameInfoEventArgs);
            requests.Remove(request.Ip);
        }

        private void OnRequestTimeout(string ip, Action onTimeout = null)
        {
            if (!this.requests.ContainsKey(ip))
            {
                return;
            }

            this.requests.Remove(ip);

            if (onTimeout != null)
            {
                onTimeout();
            }
        }
        
        public void AllowToReceiveFrom(string ip, Action<GameInfoEventArgs> onReceivedSuccessfully, Action onTimeout = null)
        {
            if (this.requests.ContainsKey(ip))
            {
                throw new InvalidOperationException();
            }

            var request = new ReceiveGameInfoRequest(ip, onReceivedSuccessfully);
            this.requests.Add(ip, request);

            var timer = TimerUtils.ExecuteAfter(this.timeoutInSeconds, () => this.OnRequestTimeout(ip, onTimeout));
            timer.AutoDispose = true;
            timer.RunOnUnityThread = true;
            timer.Start();
        }

        public void StopReceiveFrom(string ip)
        {
            if (this.requests.ContainsKey(ip))
            {
                throw new InvalidOperationException();
            }

            this.requests.Remove(ip);
        }

        public void StopAll()
        {
            this.requests.Clear();
        }
    }
}