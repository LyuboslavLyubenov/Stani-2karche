﻿namespace Controllers
{

    using System;
    using System.Collections;

    using DTOs;

    using UnityEngine;
    using UnityEngine.UI;

    public class LeaderboardUIController : MonoBehaviour
    {
        private const int SpaceBetweenScore = 10;
    
        public GameObject ContentPanel;

        private GameObject PlayerScorePrefab;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.PlayerScorePrefab = Resources.Load<GameObject>("Prefabs/LeaderboardPlayerScoreElement");
            
            if (this.ContentPanel == null)
            {
                throw new Exception("ContentPanel is null on LeaderboardUIController obj");
            }
        }

        private IEnumerator PopulateCoroutine(PlayerScore[] playersScore)
        {
            yield return null;

            var defaultRect = this.PlayerScorePrefab.GetComponent<RectTransform>();
            var rectHeight = defaultRect.sizeDelta.y;

            for (int i = 0; i < playersScore.Length; i++)
            {
                var playerScoreData = playersScore[i];
                var scoreObj = Instantiate(this.PlayerScorePrefab);
                var propertyInfos = playerScoreData.GetType().GetProperties();

                for (int j = 0; j < propertyInfos.Length; j++)
                {
                    var propertyName = propertyInfos[j].Name;
                    var propertyValue = propertyInfos[j].GetValue(playerScoreData, null);

                    var propObj = scoreObj.transform.Find(propertyName);

                    if (propObj == null || propertyValue == null)
                    {
                        continue;
                    }

                    propObj.GetComponent<Text>().text = propertyValue.ToString();
                }

                scoreObj.transform.SetParent(this.ContentPanel.transform, false);

                var scoreRect = scoreObj.GetComponent<RectTransform>();
                var nextY = ((rectHeight + SpaceBetweenScore) * (i + 1));
                scoreRect.anchoredPosition = new Vector2(0, -nextY);

                yield return new WaitForSeconds(0.05f);
                yield return null;
            }

            this.ResizeContentPanel();
        }

        private void ResizeContentPanel()
        {
            //get last element position
            //resize ContentPanel so its height = last element position + last element height + SpaceBetweenScore
            var lastElementIndex = this.ContentPanel.transform.childCount - 1;
            var lastElementObj = this.ContentPanel.transform.GetChild(lastElementIndex);
            var lastElementRect = lastElementObj.GetComponent<RectTransform>();
            var lastElementPos = lastElementRect.anchoredPosition;
            var contentPanelHeight = Mathf.Abs(lastElementPos.y) + lastElementRect.sizeDelta.y + SpaceBetweenScore;
            var contentPanelRect = this.ContentPanel.GetComponent<RectTransform>();

            contentPanelRect.sizeDelta = new Vector2(0, contentPanelHeight);
        }

        public void Populate(PlayerScore[] playersScore)
        {
            if (playersScore == null || playersScore.Length < 1)
            {
                return;
            }

            this.StartCoroutine(this.PopulateCoroutine(playersScore));
        }
    }
}