namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;
    using UnityEngine.UI;

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
            var commandData = NetworkCommandData.From<StartGameRequestCommand>();
            this.networkManager.SendServerCommand(commandData);
        }
    }
}