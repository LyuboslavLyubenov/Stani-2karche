﻿namespace AnimationControllers
{

    using Controllers;

    using Localization;

    using Network;
    using Network.NetworkManagers;

    using Notifications;

    using UnityEngine;

    public class InitializeChooseCategoryControllerAfterAnimationLoad : StateMachineBehaviour
    {
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var clientNetworkManager = ClientNetworkManager.Instance;
            var remoteCategoriesReader = new RemoteAvailableCategoriesReader(clientNetworkManager, this.OnTimeout, 5);
            animator.gameObject.GetComponent<ClientChooseCategoryUIController>().Initialize(remoteCategoriesReader);
        }

        private void OnTimeout()
        {
            var clientNetworkManager = ClientNetworkManager.Instance;
            var notificationsController = GameObject.FindObjectOfType<NotificationsController>();
            var errorMsg = LanguagesManager.Instance.GetValue("Errors/LoadCategoriesTimeout");
            notificationsController.AddNotification(Color.red, errorMsg);
            clientNetworkManager.Disconnect();
        }

    }

}
