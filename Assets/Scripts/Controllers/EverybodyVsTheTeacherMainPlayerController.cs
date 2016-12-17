using UnityEngine;
using System;

public class EverybodyVsTheTeacherMainPlayerController : ExtendedMonoBehaviour
{
    public GameObject LoadingUI;
    public GameObject UnableToConnectUI;

    public ClientNetworkManager NetworkManager;
    public NotificationsServiceController NotificationController;
    public AvailableJokersUIController JokersUI;
    public SecondsRemainingUIController SecondsRemainingUIController;
    public MarkPanelController MarkUIController;
    public SurrenderConfirmUIController SurrenderConfirmUIController;
    public ClientChooseCategoryUIController ChooseCategoryUIController;
    public UnableToConnectUIController UnableToConnectUIController;
    public QuestionUIController QuestionUIController;

    void Start()
    {
        LoadControllers();
        AttachEventHandlers();
        ConnectToServer();

        LoadingUI.SetActive(true);
        ConnectToServer();
    }

    void OnConnectedToServer(object sender, EventArgs args)
    {
        LoadingUI.SetActive(false);
        UnableToConnectUI.SetActive(false);
    }

    void OnDisconnectedFromServer(object sender, EventArgs args)
    {
        UnableToConnectUI.SetActive(true);
    }

    void OnFoundServerIP(string ip)
    {
        UnableToConnectUIController.ServerIP = ip;

        try
        {
            NetworkManager.ConnectToHost(ip);
        }
        catch
        {
            OnFoundServerIPError();
        }
    }

    void OnFoundServerIPError()
    {
        CoroutineUtils.WaitForSeconds(1f, ConnectToServer);
    }

    void LoadControllers()
    {  
        
    }

    void AttachEventHandlers()
    {
        NetworkManager.OnConnectedEvent += OnConnectedToServer;
        NetworkManager.OnDisconnectedEvent += OnDisconnectedFromServer;
    }

    void ConnectToServer()
    {
        NetworkManagerUtils.Instance.GetServerIp(OnFoundServerIP, OnFoundServerIPError);
    }
}