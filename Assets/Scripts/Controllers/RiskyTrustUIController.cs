using UnityEngine;
using System.Collections;
using System;

public class RiskyTrustUIController : MonoBehaviour
{
    public ServerNetworkManager ServerNetworkManager;
    public GameData GameData;
    public GameObject WaitingForAnswerUI;

    public void ActivateJoker()
    {
        if (ServerNetworkManager.ConnectedClientsId.Count <= 0)
        {
            throw new Exception("Жокера може да се изпозлва само когато си онлайн.");
        }

        var currentQuestion = GameData.GetCurrentQuestion();
        var currentQuestionJSON = JsonUtility.ToJson(currentQuestion);
        var friendConnectionIdIndex = UnityEngine.Random.Range(0, ServerNetworkManager.ConnectedClientsId.Count);
        var friendConnectionId = ServerNetworkManager.ConnectedClientsId[friendConnectionIdIndex];

        ServerNetworkManager.SendClientMessage(friendConnectionId, "RiskyTrust");
        ServerNetworkManager.SendClientMessage(friendConnectionId, currentQuestionJSON);

        WaitingForAnswerUI.SetActive(true);

        gameObject.SetActive(false);
    }
}
