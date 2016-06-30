using UnityEngine;

public class QuestionPanelSyncServer : MonoBehaviour
{
    public PlayingUIController PlayingUIController;
   
    ServerNetworkManager serverNetworkManager;

    void Start()
    {
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
