﻿namespace Controllers.EveryBodyVsTheTeacher
{

    using Commands;
    using Commands.EveryBodyVsTheTeacher;
    using Commands.EveryBodyVsTheTeacher.PlayersConnectingState;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using Zenject.Source.Usage;

    public class StartGameButtonUIController : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(this.SendRequestToStartTheGame);
        }

        private void SendRequestToStartTheGame()
        {
            var commandData = NetworkCommandData.From<MainPlayerRequestedGameStartCommand>();
            this.networkManager.SendServerCommand(commandData);
        }
    }
}