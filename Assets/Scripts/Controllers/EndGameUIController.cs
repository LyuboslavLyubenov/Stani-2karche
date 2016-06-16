using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameUIController : MonoBehaviour
{
    public GameObject LeaderboardUI;

    LeaderboardUIController leaderboardUIController = null;

    void OnEnable()
    {
        var leaderboardSerializer = GameObject.FindWithTag("MainCamera").GetComponent<LeaderboardSerializer>();

        if (leaderboardUIController == null)
        {
            leaderboardUIController = LeaderboardUI.GetComponent<LeaderboardUIController>();    
        }

        leaderboardUIController.Populate(leaderboardSerializer.Leaderboard);
    }

    public void SetMark(int mark)
    {
        var endScreenMark = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        endScreenMark.text = mark.ToString();
    }
}
