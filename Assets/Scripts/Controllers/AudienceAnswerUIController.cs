namespace Assets.Scripts.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.UI;

    using Extensions;

    public class AudienceAnswerUIController : MonoBehaviour
    {
        private const int DistanceBetweenElements = 10;

        private Transform[] answers;

        private Text[] answersText;
        private Text[] chanceToBeCorrectText;

        private void DeleteOldAnswers()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                var answerChance = this.transform.GetChild(i).gameObject;
                Destroy(answerChance);
            }

            this.answers = null;
            this.answersText = null;
            this.chanceToBeCorrectText = null;
        }

        private void SetAnswersChancesCount(int count)
        {
            this.answers = new Transform[count];
            this.answersText = new Text[count];
            this.chanceToBeCorrectText = new Text[count];

            var prefab = Resources.Load<GameObject>("Prefabs/AnswerChance");
            var parentRectTransform = this.transform.GetComponent<RectTransform>();
            var parentSizeY = parentRectTransform.sizeDelta.y;
            var sizeY = (parentSizeY - (DistanceBetweenElements * count)) / count;

            for (int i = 0; i < count; i++)
            {
                var answerChance = Instantiate(prefab, parentRectTransform.transform, false);
                var rectTransform = answerChance.GetComponent<RectTransform>();
                var y = (sizeY / 2) + (i * (sizeY + DistanceBetweenElements)); 

                rectTransform.anchoredPosition = new Vector2(0, -y);
                rectTransform.sizeDelta = new Vector2(1, sizeY);

                this.answers[i] = answerChance.transform;
                this.answersText[i] = this.answers[i].GetChild(0).GetComponent<Text>();
                this.chanceToBeCorrectText[i] = this.answers[i].GetChild(1).GetComponent<Text>();
            }
        }

        public void SetVoteCount(Dictionary<string, int> answersVoteCount, bool inProcentage)
        {
            this.DeleteOldAnswers();
            this.SetAnswersChancesCount(answersVoteCount.Count);

            var answersVotes = answersVoteCount.Shuffled();
            var votedCountValues = answersVotes.Select(a => a.Value).ToArray();
            var chancesSum = votedCountValues.Sum();
            var answerIndex = 0;

            foreach (KeyValuePair<string, int> chance in answersVotes)
            {
                this.answersText[answerIndex].text = chance.Key;
          
                if (inProcentage)
                {
                    //calculate procentage
                    var chanceToBeCorrect = ((float)chance.Value / chancesSum) * 100f;
                    this.chanceToBeCorrectText[answerIndex].text = string.Format("{0:F0}%", chanceToBeCorrect);                
                }
                else
                {
                    this.chanceToBeCorrectText[answerIndex].text = chance.Value.ToString();
                }

                answerIndex++;
            }
        }
    }

}
