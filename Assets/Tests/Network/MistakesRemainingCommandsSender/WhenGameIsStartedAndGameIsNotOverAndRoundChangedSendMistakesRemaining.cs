﻿using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.MistakesRemainingCommandsSender
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Utils;
    using Assets.Tests.DummyObjects;

    using Commands;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenGameIsStartedAndGameIsNotOverAndRoundChangedSendMistakesRemaining : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IRoundsSwitcher roundsSwitcher;

        [Inject]
        private IMistakesRemainingCommandsSender commandsSender;

        void Start()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;

            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadMistakesRemainingCommand>())
                    {
                        IntegrationTest.Pass();
                    }
                };

            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.StartedGame = true;
            dummyServer.IsGameOver = false;
            dummyServer.PresenterId = 1;

            var dummyRoundsSwitcher = (DummyRoundsSwitcher)this.roundsSwitcher;
            dummyRoundsSwitcher.SwitchToNextRound();
        }
    }
}