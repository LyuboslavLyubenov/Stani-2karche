using DummyGameDataIterator = Tests.DummyObjects.DummyGameDataIterator;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.MistakesRemainingCommandsSender
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;

    using Commands;

    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenGameStartedSendMistakesRemaining : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;
        
        [Inject]
        private IGameDataIterator iterator;
        
        [Inject]
        private int mistakesRemainingCount;

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

            var dummyIterator = (DummyGameDataIterator)this.iterator;
            dummyIterator.ExecuteOnLoaded();
        }
    }
}