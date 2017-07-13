namespace Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Interfaces.Controllers;

    using UnityEngine;

    using Zenject;

    public class HideOneMainPlayerFromScreen : MonoBehaviour
    {
        [Inject]
        private IMainPlayersContainerUIController mainPlayersContainerUiController;

        public int ConnectionId = 1;

        void Start()
        {
            this.mainPlayersContainerUiController.HideMainPlayer(this.ConnectionId);
        }
    }
}