using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public void SetVoteCount(Dictionary<string, int> answersVoteCount, bool inProcentage)
    {
        var chancesValues = answersVoteCount.Select(a => a.Value).ToArray();
        var chancesSum = chancesValues.Sum();
        var answerIndex = 0;

        foreach (KeyValuePair<string, int> chance in answersVoteCount)
        {
            answersText[answerIndex].text = chance.Key;
          
            if (inProcentage)
            {
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
