namespace Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    using Zenject.Source.Usage;

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