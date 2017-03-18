namespace Controllers
{

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    public class QuestionsRemainingUIController : MonoBehaviour
    {
        public Text QuestionsRemaining;

        public void SetRemainingQuestions(int remainingQuestions)
        {
            if (remainingQuestions < 0)
            {
                throw new ArgumentOutOfRangeException("remainingQuestions");
            }

            this.QuestionsRemaining.text = remainingQuestions.ToString();
        }
    }

}