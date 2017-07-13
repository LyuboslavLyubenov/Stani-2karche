namespace Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using Interfaces.Controllers;

    using Utils.Unity;

    using Zenject;

    public class RemoveAudiencePlayer : ExtendedMonoBehaviour
    {
        public int ConnectionId = 1;
        public float AfterTimeInSeconds = 1f;

        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUIController;
        
        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.AfterTimeInSeconds,
                () =>
                    {
                        this.audiencePlayersContainerUIController.HideAudiencePlayer(ConnectionId);
                    });
        }
    }
}