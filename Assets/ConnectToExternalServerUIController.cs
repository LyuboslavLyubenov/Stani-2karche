using UnityEngine;
using UnityEngine.UI;

public class ConnectToExternalServerUIController : MonoBehaviour
{
    const int ConnectionTimeoutInSeconds = 5;

    public CreatedGameInfoReceiverService GameInfoReceiverService;
    public NotificationsServiceController NotificationService;
    public BasicExamServerSelectPlayerTypeUIController SelectPlayerTypeUIController;
    public GameObject LoadingUI;
    public Text IPText;

    float elapsedTimeTryingToConnect = 0;
    string ip;
    bool connecting = false;

    public void TryToConnect(string ip)
    {
        elapsedTimeTryingToConnect = 0;
        this.ip = ip;
        connecting = true;

        GameInfoReceiverService.ReceiveFrom(ip, OnReceivedGameInfo);
        LoadingUI.SetActive(true);
    }

    public void TryToConnect()
    {
        TryToConnect(IPText.text);
    }

    void Update()
    {
        if (!connecting)
        {
            return;
        }

        elapsedTimeTryingToConnect += Time.deltaTime;

        if (elapsedTimeTryingToConnect >= ConnectionTimeoutInSeconds)
        {
            LoadingUI.SetActive(false);
            NotificationService.AddNotification(Color.red, "Няма връзка със сървъра");

            try
            {
                GameInfoReceiverService.StopReceivingFrom(this.ip);
            }
            catch (System.Exception ex)
            {
                
            }

            connecting = false;
        }
    }

    void OnReceivedGameInfo(GameInfoReceivedDataEventArgs args)
    {
        LoadingUI.SetActive(false);
        connecting = false;

        switch (args.GameInfo.GameType)
        {
            case GameType.BasicExam:
                var basicExamGameInfo = JsonUtility.FromJson<BasicExamGameInfo_Serializable>(args.JSON);
                OnConnectingToBasicExam(basicExamGameInfo);
                break;

            default:
                NotificationService.AddNotification(Color.red, "Неподържан вид игра", 10);
                break;
        }
    }

    void OnConnectingToBasicExam(BasicExamGameInfo_Serializable gameInfo)
    {
        SelectPlayerTypeUIController.gameObject.SetActive(true);
        SelectPlayerTypeUIController.Initialize(gameInfo);
    }
}
