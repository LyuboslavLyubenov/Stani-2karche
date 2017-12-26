using Commands;
using Assets.Tests.Utils;
using Commands.Jokers;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using Interfaces;
using System.Linq;

namespace Tests.Jokers.Routers.DisableRandomAnswersRouter
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using Zenject;
    using Interfaces.Network.NetworkManager;
    using Interfaces.Network.Jokers.Routers;
    using Tests.DummyObjects;

    public class WhenActivatedSuccessfullySendAnswersToHideToConnectionId : MonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IDisableRandomAnswersRouter router;

        [Inject]
        private ISimpleQuestion question;

        void Start()
        {
            var connectionId = 2;
            var disableCount = 2;

            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyServerNetworkManager.OnSentDataToClient += (sender, args) =>
            {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == typeof(DisableRandomAnswersJokerSettingsCommand).Name.Replace("Command", "") &&
                        args.ConnectionId == connectionId &&
                        command.Options.Count == disableCount &&
                        !command.Options.ContainsValue(this.question.CorrectAnswer) &&
                        command.Options.Select(o => o.Value).All(this.question.Answers.Contains))
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
            };

            router.Activate(disableCount, connectionId);
        }
    }
}