namespace Controllers
{

    using System;

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    public class QuestionsRemainingUIController : MonoBehaviour, IQuestionsRemainingUIController
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