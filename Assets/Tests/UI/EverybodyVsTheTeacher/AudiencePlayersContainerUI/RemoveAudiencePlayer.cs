namespace Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    using Utils.Unity;

    using Zenject.Source.Usage;

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