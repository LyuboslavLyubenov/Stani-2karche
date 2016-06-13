using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestionUIController : MonoBehaviour
{
    const int AnswersCount = 4;
   
    Text questionText;
    Text[] answersText = new Text[AnswersCount];
    Button[] answersButton = new Button[AnswersCount];
    ClientNetworkManager clientNetworkManager = null;

    void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        clientNetworkManager = GameObject.FindWithTag("MainCamera").GetComponent<ClientNetworkManager>();
        var answers = GameObject.FindGameObjectsWithTag("Answer");
        questionText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();

        for (int i = 0; i < AnswersCount; i++)
        {
            answersText[i] = answers[i].transform.GetChild(0).GetComponent<Text>();
            answersButton[i] = answers[i].GetComponent<Button>();
        }

        yield return null;
    }

    public void LoadQuestion(Question question)
    {
        var _this = this;
        questionText.text = question.Text;

        for (int i = 0; i < AnswersCount; i++)
        {
            var answer = question.Answers[i];
            answersText[i].text = question.Answers[i];
            answersButton[i].onClick.AddListener(() =>
                {
                    clientNetworkManager.SendData(answer);
                    _this.gameObject.SetActive(false);
                });
        }
    }
}
