using UnityEngine;

namespace Assets.Scripts.AnimationControllers
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.Notifications;

    public class InitializeChooseCategoryControllerAfterAnimationLoad : StateMachineBehaviour
    {
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var clientNetworkManager = GameObject.FindObjectOfType<ClientNetworkManager>();
            var remoteCategoriesReader = new RemoteAvailableCategoriesReader(clientNetworkManager, this.OnTimeout, 5);
            animator.gameObject.GetComponent<ClientChooseCategoryUIController>().Initialize(remoteCategoriesReader);
        }

        void OnTimeout()
        {
            var clientNetworkManager = GameObject.FindObjectOfType<ClientNetworkManager>();
            var notificationsController = GameObject.FindObjectOfType<NotificationsServiceController>();
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/LoadCategoriesTimeout");
            notificationsController.AddNotification(Color.red, errorMsg);
            clientNetworkManager.Disconnect();
        }

    }

}
