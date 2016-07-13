using UnityEngine;
using UnityEngine.UI;
using System;

public class EndGameUIController : MonoBehaviour
{
    public GameObject LeaderboardUI;
    public LeaderboardSerializer LeaderboardSerializer;

    LeaderboardUIController leaderboardUIController = null;

    void OnEnable()
    { 
        if (LeaderboardUI == null)
        {
            throw new NullReferenceException("LeaderboardUI is null on EndGameUIController obj");
        }

        if (LeaderboardSerializer == null)
        {
            throw new NullReferenceException("LeaderboardSerializer is null on EndGameUIController obj");
        }

        leaderboardUIController = LeaderboardUI.GetComponent<LeaderboardUIController>(); 
        leaderboardUIController.Populate(LeaderboardSerializer.Leaderboard);
    }

    public void SetMark(int mark)
    {
        var endScreenMark = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        endScreenMark.text = mark.ToString();
    }
}
