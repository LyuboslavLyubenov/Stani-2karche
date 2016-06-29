using UnityEngine;
using UnityEngine.UI;

public class EndGameUIController : MonoBehaviour
{
    public GameObject LeaderboardUI;
    public LeaderboardSerializer leaderboardSerializer;

    LeaderboardUIController leaderboardUIController = null;

    void OnEnable()
    {
        if (LeaderboardUI != null)
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
