using DummyGameDataIterator = Tests.DummyObjects.DummyGameDataIterator;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.QuestionsRemainingCommandsSender
{
    using Assets.Scripts.Commands.UI;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils;

    using Commands;

    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenGameDataLoadedSendQuestionsRemainigToPresenter : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private IGameDataIterator iterator;

        [Inject]
        private IQuestionsRemainingCommandsSender commandsSender;

        void Start()
        {
            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentDataToClient += (sender, args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == NetworkManagerCommandUtils.GetCommandName<LoadQuestionRemainingCountCommand>())
                    {
                        IntegrationTest.Fail();
                    }
                };

            var dummyIterator = (DummyGameDataIterator)this.iterator;
            dummyIterator.Loaded = true;
            dummyIterator.ExecuteOnLoaded();

            this.CoroutineUtils.WaitForFrames(1, IntegrationTest.Pass);
        }
    }
}