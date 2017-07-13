namespace Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using Interfaces.Controllers;

    using Utils.Unity;

    using Zenject;

    public class AddAudiencePlayer : ExtendedMonoBehaviour
    {
        public int ConnectionId = 1;

        public string Username = "Ivan";

        public float AfterTimeInSeconds = 1f;

        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUIController;

        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(this.AfterTimeInSeconds,
                () =>
                    {
                        this.audiencePlayersContainerUIController.ShowAudiencePlayer(this.ConnectionId, this.Username);
                    });
        }
    }
}