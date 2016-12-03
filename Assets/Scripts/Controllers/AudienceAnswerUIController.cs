using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class AudienceAnswerUIController : MonoBehaviour
{
    const int DistanceBetweenElements = 10;

    Transform[] answers;
    Text[] answersText;
    Text[] chanceToBeCorrectText;

    void DeleteOldAnswers()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var answerChance = transform.GetChild(i).gameObject;
            Destroy(answerChance);
        }

        answers = null;
        answersText = null;
        chanceToBeCorrectText = null;
    }

    void SetAnswersChancesCount(int count)
    {
        answers = new Transform[count];
        answersText = new Text[count];
        chanceToBeCorrectText = new Text[count];

        var prefab = Resources.Load<GameObject>("Prefabs/AnswerChance");
        var parentRectTransform = transform.GetComponent<RectTransform>();
        var parentSizeY = parentRectTransform.sizeDelta.y;
        var sizeY = (parentSizeY - (DistanceBetweenElements * count)) / count;

        for (int i = 0; i < count; i++)
        {
            var answerChance = (GameObject)Instantiate(prefab, parentRectTransform.transform, false);
            var rectTransform = answerChance.GetComponent<RectTransform>();
            var y = (sizeY / 2) + (i * (sizeY + DistanceBetweenElements)); 

            rectTransform.anchoredPosition = new Vector2(0, -y);
            rectTransform.sizeDelta = new Vector2(1, sizeY);

            answers[i] = answerChance.transform;
            answersText[i] = answers[i].GetChild(0).GetComponent<Text>();
            chanceToBeCorrectText[i] = answers[i].GetChild(1).GetComponent<Text>();
        }
    }

    public void SetVoteCount(Dictionary<string, int> answersVoteCount, bool inProcentage)
    {
        DeleteOldAnswers();
        SetAnswersChancesCount(answersVoteCount.Count);

        var answersVotes = answersVoteCount.Shuffled();
        var votedCountValues = answersVotes.Select(a => a.Value).ToArray();
        var chancesSum = votedCountValues.Sum();
        var answerIndex = 0;

        foreach (KeyValuePair<string, int> chance in answersVotes)
        {
            answersText[answerIndex].text = chance.Key;
          
            if (inProcentage)
            {
                //calculate procentage
                var chanceToBeCorrect = ((float)chance.Value / chancesSum) * 100f;
                chanceToBeCorrectText[answerIndex].text = string.Format("{0:F0}%", chanceToBeCorrect);                
            }
            else
            {
                chanceToBeCorrectText[answerIndex].text = chance.Value.ToString();
            }

            answerIndex++;
        }
    }
}
