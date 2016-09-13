using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LeaderboardUIController : MonoBehaviour
{
    const int SpaceBetweenScore = 10;
    
    public GameObject ContentPanel;

    GameObject PlayerScorePrefab;

    void Start()
    {
        PlayerScorePrefab = Resources.Load<GameObject>("Prefabs/LeaderboardPlayerScoreElement");

        if (ContentPanel == null)
        {
            throw new Exception("ContentPanel is null on LeaderboardUIController obj");
        }
    }

    public void Populate(PlayerScore[] playersScore)
    {
        StartCoroutine(PopulateCoroutine(playersScore));
    }

    IEnumerator PopulateCoroutine(PlayerScore[] playersScore)
    {
        yield return null;

        var defaultRect = PlayerScorePrefab.GetComponent<RectTransform>();
        var height = defaultRect.sizeDelta.y;

        for (int i = 0; i < playersScore.Length; i++)
        {
            var playerScoreData = playersScore[i];
            var scoreObj = Instantiate(PlayerScorePrefab);
            var propertyInfos = playerScoreData.GetType().GetProperties();

            for (int j = 0; j < propertyInfos.Length; j++)
            {
                var propertyName = propertyInfos[j].Name;
                var propertyValue = propertyInfos[j].GetValue(playerScoreData, null);

                var propObj = scoreObj.transform.FindChild(propertyName);

                if (propObj == null || propertyValue == null)
                {
                    continue;
                }

                propObj.GetComponent<Text>().text = propertyValue.ToString();
            }

            scoreObj.transform.SetParent(ContentPanel.transform, false);

            var scoreRect = scoreObj.GetComponent<RectTransform>();
            var nextY = ((scoreRect.sizeDelta.y + SpaceBetweenScore) * (i + 1));
            scoreRect.anchoredPosition = new Vector2(0, -nextY);
                
            yield return new WaitForSeconds(0.05f);
            yield return null;
        }

        ResizeContentPanel();
    }

    void ResizeContentPanel()
    {
        //get last element position
        //resize ContentPanel so its height = last element position + last element height + SpaceBetweenScore
        var lastElementIndex = ContentPanel.transform.childCount - 1;
        var lastElementObj = ContentPanel.transform.GetChild(lastElementIndex);
        var lastElementRect = lastElementObj.GetComponent<RectTransform>();
        var lastElementPos = lastElementRect.anchoredPosition;
        var contentPanelHeight = Mathf.Abs(lastElementPos.y) + lastElementRect.sizeDelta.y + SpaceBetweenScore;
        var contentPanelRect = ContentPanel.GetComponent<RectTransform>();

        contentPanelRect.sizeDelta = new Vector2(0, contentPanelHeight);
    }
}