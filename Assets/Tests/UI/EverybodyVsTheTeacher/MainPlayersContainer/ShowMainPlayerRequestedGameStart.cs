namespace Tests.UI.EverybodyVsTheTeacher.MainPlayersContainer
{

    using Interfaces.Controllers;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class ShowMainPlayerRequestedGameStart : MonoBehaviour
    {
        public int ConnectionId = 1;

        [Inject]
        private IMainPlayersContainerUIController mainPlayersContainerUiController;
        
        void Start()
        {
            this.mainPlayersContainerUiController.ShowMainPlayerRequestedGameStart(this.ConnectionId);
        }        
    }
}