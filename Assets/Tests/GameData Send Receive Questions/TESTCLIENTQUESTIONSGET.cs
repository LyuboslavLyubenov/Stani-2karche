using UnityEngine;
using System.Collections;

public class TESTCLIENTQUESTIONSGET : MonoBehaviour
{
    public RemoteGameData GameData;
    public ClientNetworkManager NetworkManager;
    // Use this for initialization
    void Start()
    {
        NetworkManager.OnConnectedEvent += OnConnected; 
    }

    void OnConnected(object sender, System.EventArgs args)
    {
        SendRequestGetCurrentQuestion();
    }

    void SendRequestGetCurrentQuestion()
    {
        GameData.GetCurrentQuestion((question) =>
            {
                Debug.Log("Current Question received " + question.Text);
                SendRequestGetRandomQuestion();
            }, 
            (exception) =>
            {
                DebugUtils.LogException(exception);
            }
        );
    }

    void SendRequestGetRandomQuestion()
    {
        GameData.GetRandomQuestion((question) =>
            {
                Debug.Log("Random Question received " + question.Text);
                SendRequestGetNextQuestion();
            }, 
            (exception) =>
            {
                DebugUtils.LogException(exception);
            }
        );
    }

    void SendRequestGetNextQuestion()
    {
        GameData.GetNextQuestion((question) =>
            {
                Debug.Log("Next Question received " + question.Text);
            }, 
            (exception) =>
            {
                DebugUtils.LogException(exception);
            }
        );
    }
}
