using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FriendAnswerUIController : MonoBehaviour
{
    public GameObject FriendUsername;
    public GameObject FriendAnswer;

    Text usernameText;
    Text answerText;

    string[] offlineFriendNames = { "Гошо", "Пешо", "Баба яга с мотиката", "г-н Кидиков" };

    void Start()
    {
        usernameText = FriendUsername.GetComponent<Text>();
        answerText = FriendAnswer.GetComponent<Text>();
    }

    public void SetResponse(string answer)
    {
        var usernameIndex = Random.Range(0, offlineFriendNames.Length);
        var username = offlineFriendNames[usernameIndex];

        StartCoroutine(SetResponseCoroutine(username, answer));
    }

    public void SetResponse(string username, string answer)
    {
        StartCoroutine(SetResponseCoroutine(username, answer));
    }

    IEnumerator SetResponseCoroutine(string username, string answer)
    {
        yield return null;

        usernameText.text = username;
        answerText.text = answer;
    }
}
