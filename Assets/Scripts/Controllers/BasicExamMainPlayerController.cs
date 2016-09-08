using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Basic exam controller. Used to controll UI for "Standart play mode" or "Нормално изпитване" 
/// </summary>
public class BasicExamMainPlayerController : ExtendedMonoBehaviour
{
    public GameObject LeaderboardUI;
    public GameObject LoadingUI;
    public GameObject EndGameUI;

    public ClientNetworkManager NetworkManager;
    public BasicExamPlayerTeacherDialogSwitcher DialogSwitcher;
    public NotificationsServiceController NotificationService;
    public BasicExamPlayerTutorialUIController TutorialUIController;
    public AvailableJokersUIController AvailableJokersUIController;
    public QuestionUIController QuestionUIController;

    void Start()
    { 
        QuestionUIController.OnAnswerClick += OnAnswerClick;
    }

    void ShowNotification(Color color, string message)
    {
        if (NotificationService != null)
        {
            NotificationService.AddNotification(color, message);
        }
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        var commandData = new NetworkCommandData("AnswerClicked");
        commandData.AddOption("Answer", args.Answer);
        NetworkManager.SendServerCommand(commandData);
    }

    void OnGameEnd(object sender, MarkEventArgs args)
    {
        EndGameUI.SetActive(true);
        var endGameUIController = EndGameUI.GetComponent<EndGameUIController>();
        CoroutineUtils.WaitForFrames(0, () => endGameUIController.SetMark(args.Mark));
    }
}