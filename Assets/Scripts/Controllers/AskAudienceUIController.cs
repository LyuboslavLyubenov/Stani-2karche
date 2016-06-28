using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Ask audience user interface controller.
/// </summary>
public class AskAudienceUIController : MonoBehaviour
{
    GameObject[] answers;
    Text[] answersText;
    Text[] chanceToBeCorrectText;

    void OnEnable()
    {
        answers = GameObject.FindGameObjectsWithTag("AudienceAnswerChance");
        answersText = new Text[answers.Length];
        chanceToBeCorrectText = new Text[answers.Length];   

        for (int i = 0; i < answers.Length; i++)
        {
            answersText[i] = answers[i].transform.GetChild(0).GetComponent<Text>();
            chanceToBeCorrectText[i] = answers[i].transform.GetChild(1).GetComponent<Text>();
        }
    }

    /// <summary>
    /// Sets the vote count and shows 
    /// </summary>
    /// <param name="answersVoteCount">Votes count for every answer</param>
    /// <param name="inProcentage">If set to <c>true</c> will show procentage instead of voted count.</param>
    public void SetVoteCount(Dictionary<string, int> answersVoteCount, bool inProcentage)
    {
        var votedCountValues = answersVoteCount.Select(a => a.Value).ToArray();
        //all votes count
        var chancesSum = votedCountValues.Sum();
        var answerIndex = 0;

        foreach (KeyValuePair<string, int> chance in answersVoteCount)
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
