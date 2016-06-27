using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject PlayerScorePrefab;
    public GameObject ContentPanel;

    const int SpaceBetweenScore = 10;


    public void Populate(IList<PlayerScore> playersScore)
    {
        StartCoroutine(PopulateCoroutine(playersScore));
    }

    IEnumerator PopulateCoroutine(IList<PlayerScore> playersScore)
    {
        yield return null;

        var defaultRect = PlayerScorePrefab.GetComponent<RectTransform>();
        var height = defaultRect.sizeDelta.y;

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
            score.transform.SetParent(ContentPanel.transform, false);

            var nextY = ((scoreRect.sizeDelta.y + SpaceBetweenScore) * (i + 1));
            scoreRect.anchoredPosition = new Vector2(0, -nextY);
                
            yield return new WaitForEndOfFrame();
        }

        var lastElementIndex = ContentPanel.transform.childCount - 1;
        var lastElementObj = ContentPanel.transform.GetChild(lastElementIndex);
        var lastElementRect = lastElementObj.GetComponent<RectTransform>();
        var lastElementPos = lastElementRect.anchoredPosition;
        var contentPanelHeight = Mathf.Abs(lastElementPos.y) + lastElementRect.sizeDelta.y + SpaceBetweenScore;
        var contentPanelRect = ContentPanel.GetComponent<RectTransform>();

        contentPanelRect.sizeDelta = new Vector2(0, contentPanelHeight);
    }
}