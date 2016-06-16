using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject PlayerScorePrefab;
    public GameObject ContentPanel;

    void OnEnable()
    {
        //TODO: DECOUPLE
        var leaderboardSerializer = GameObject.FindWithTag("MainCamera").GetComponent<LeaderboardSerializer>();
        Populate(leaderboardSerializer.Leaderboard);
    }

    public void Populate(IList<PlayerScore> playersScore)
    {
        StartCoroutine(PopulateCoroutine(playersScore));
    }

    IEnumerator PopulateCoroutine(IList<PlayerScore> playersScore)
    {
        yield return null;

        for (int i = 0; i < playersScore.Count; i++)
        {
            var playerScoreData = playersScore[i];
            var score = Instantiate(PlayerScorePrefab);
            var scoreRect = score.GetComponent<RectTransform>();
            var playerNameTextObj = score.transform.GetChild(0);
            var scoreTextObj = score.transform.GetChild(1);

            //SET TEXT
            playerNameTextObj.GetComponent<Text>().text = playerScoreData.PlayerName;
            scoreTextObj.GetComponent<Text>().text = playerScoreData.Score.ToString();

            //SET POSITION AND PARENT
            score.transform.SetParent(ContentPanel.transform);

            scoreRect.anchoredPosition = new Vector2(0, -60 + (-90 * i));
            scoreRect.sizeDelta = PlayerScorePrefab.GetComponent<RectTransform>().sizeDelta;
                
            yield return null;
        }

        var lastElementIndex = ContentPanel.transform.childCount - 1;
        var lastElementObj = ContentPanel.transform.GetChild(lastElementIndex);
        var lastElementPos = lastElementObj.GetComponent<RectTransform>().anchoredPosition;
        var contentPanelHeight = Mathf.Abs(lastElementPos.y) + 60;
        var contentPanelRect = ContentPanel.GetComponent<RectTransform>();

        contentPanelRect.sizeDelta = new Vector2(0, contentPanelHeight);
    }
}