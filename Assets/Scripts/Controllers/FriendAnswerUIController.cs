using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FriendAnswerUIController : MonoBehaviour
{
    public GameObject FriendUsername;
    public GameObject FriendAnswer;

    Text usernameText;
    Text answerText;

    void Start()
    {
        usernameText = FriendUsername.GetComponent<Text>();
        answerText = FriendAnswer.GetComponent<Text>();
    }

    public void SetResponse(string answer)
    {
        StartCoroutine(SetResponse_(answer));
    }

    IEnumerator SetResponse_(string answer)
    {
        yield return null;

        usernameText.text = "Иван";
        answerText.text = answer;
    }
}
