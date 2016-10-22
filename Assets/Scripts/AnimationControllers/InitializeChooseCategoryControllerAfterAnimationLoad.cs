using UnityEngine;
using System.Collections;

public class InitializeChooseCategoryControllerAfterAnimationLoad : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var clientNetworkManager = GameObject.FindObjectOfType<ClientNetworkManager>();
        var remoteCategoriesReader = new RemoteAvailableCategoriesReader(clientNetworkManager, OnTimeout, 5);
        animator.gameObject.GetComponent<ClientChooseCategoryUIController>().Initialize(remoteCategoriesReader);
    }

    void OnTimeout()
    {
        var notificationsController = GameObject.FindObjectOfType<NotificationsServiceController>();
        var errorMsg = LanguagesManager.Instance.GetValue("Errors/LoadCategoriesTimeout");
        notificationsController.AddNotification(Color.red, errorMsg);
        //
    }

}
