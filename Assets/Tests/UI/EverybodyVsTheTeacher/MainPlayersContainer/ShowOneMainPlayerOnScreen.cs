namespace Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Interfaces.Controllers;

    using UnityEngine;
    using Zenject.Source.Usage;

    public class ShowOneMainPlayerOnScreen : MonoBehaviour
    {
        [Inject]
        private IMainPlayersContainerUIController mainPlayersContainerUiController;

        public int ConnectionId = 1;
        public string Username = "Ivan";

        void Start()
        {
            this.mainPlayersContainerUiController.ShowMainPlayer(1, "Ivan");
        }
    }
}
