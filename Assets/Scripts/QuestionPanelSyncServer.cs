using UnityEngine;
using System;

public class QuestionPanelSyncServer : MonoBehaviour
{
    public BasicPlayerPlayingUIController PlayingUIController;
    public NotificationsServiceController notificationsService;

    ServerNetworkManager serverNetworkManager;

    void Start()
    {
        if (PlayingUIController == null)
        {
            if (notificationsService == null)
            {
                throw new Exception("Not found notification service");
            }
                
            notificationsService.AddNotification(Color.red, "PlayingUIController on QuestionPanelSyncServer is null");
            return;
        }

        serverNetworkManager = PlayingUIController.ServerNetworkManager;

        var questionUIController = PlayingUIController.QuestionUIController;
        questionUIController.OnAnswerClick += OnAnswerClick;
        questionUIController.OnQuestionLoaded += OnQuestionLoaded;
    }

    void OnQuestionLoaded(object sender, QuestionEventArgs args)
    {
        var questionJSON = JsonUtility.ToJson(args.Question);
        serverNetworkManager.SendAllClientsMessage("LoadQuestion:" + questionJSON);
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        serverNetworkManager.SendAllClientsMessage("SelectAnswer:" + args.Answer);
    }
}
