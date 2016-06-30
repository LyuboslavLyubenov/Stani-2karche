using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestionPanelSyncClient : MonoBehaviour
{
    public QuestionUIController QuestionUIController;
    public ClientNetworkManager ClientNetworkManager;

    // Use this for initialization
    void Start()
    {
        ClientNetworkManager.OnReceivedDataEvent += OnReceivedDataEvent;
    }

    void OnReceivedDataEvent(object sender, DataSentEventArgs args)
    {
        var message = args.Message;
        var messageParams = message.Split(':');

        if (messageParams.Length != 2)
        {
            return;
        }
       
        switch (messageParams[0])
        {
            case "LoadQuestion":

                var question = JsonUtility.FromJson<Question>(messageParams[1]);
                QuestionUIController.LoadQuestion(question);

                break;

            case "SelectAnswer":

                var clickedAnswer = messageParams[1];
                var answers = GameObject.FindGameObjectsWithTag("Answer");

                for (int i = 0; i < answers.Length; i++)
                {
                    var answerText = answers[i].GetComponentInChildren<Text>();
                    var answerButton = answers[i].GetComponent<Button>();

                    if (answerText.text == clickedAnswer)
                    {
                        answerButton.SimulateClick();
                        break;
                    }
                }

                break;
        }
    }
}
