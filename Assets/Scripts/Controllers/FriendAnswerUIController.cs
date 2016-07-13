using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class FriendAnswerUIController : ExtendedMonoBehaviour
{
    public GameObject FriendUsername;
    public GameObject FriendAnswer;

    Text usernameText;
    Text answerText;

    string[] offlineFriendNames = { "Гошо", "Пешо", "г-н Кидиков" };

    void Start()
    {
        if (FriendUsername == null)
        {
            throw new NullReferenceException("FriendUsername is null on FriendAnswerUIController obj");
        }

        if (FriendAnswer == null)
        {
            throw new NullReferenceException("FriendAnswer is null on FriendAnswerUIController obj");
        }

        usernameText = FriendUsername.GetComponent<Text>();
        answerText = FriendAnswer.GetComponent<Text>();

        if (usernameText == null)
        {
            throw new Exception("FriendUsername must have Text component");
        }

        if (answerText == null)
        {
            throw new Exception("FriendAnswer msut have Text component");
        }
    }

    public void SetResponse(string answer)
    {
        var usernameIndex = UnityEngine.Random.Range(0, offlineFriendNames.Length);
        var username = offlineFriendNames[usernameIndex];

        CoroutineUtils.WaitForFrames(1, () => _SetResponse(username, answer));
    }

    public void SetResponse(string username, string answer)
    {
        CoroutineUtils.WaitForFrames(0, () => _SetResponse(username, answer));
    }

    void _SetResponse(string username, string answer)
    {
        usernameText.text = username;
        answerText.text = answer;
    }
}
