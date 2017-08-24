namespace Assets.Tests.UI.QuestionUI
{
    using System.Collections;
    using System.Linq;

    using Assets.Tests.Utils;

    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenAnswerIsPathAndIsValidImageShowImageAsAnswer : MonoBehaviour
    {
        [Inject]
        private IQuestionUIController questionUIController;

        void Start()
        {
            this.StartCoroutine(this.Test());
        }

        private IEnumerator Test()
        {
            yield return new WaitForSeconds(1);

            var question = new QuestionGenerator().GenerateQuestion();
            question.Answers[0] = "testimages\\testimage.jpg";
            this.questionUIController.LoadQuestion(question);

            yield return new WaitForSeconds(3);

            var answerObj = GameObject.FindObjectsOfType<Button>()
                .First(b => b.GetComponentInChildren<Text>().text == question.Answers[0]);
            var textComponent = answerObj.GetComponentInChildren<Text>();
            var imageComponent = answerObj.GetComponentsInChildren<Image>(true)[1];
           
            if (textComponent.text == question.Answers[0] &&
                imageComponent.gameObject.activeSelf)
            {
                IntegrationTest.Pass();
            }
            else
            {
                IntegrationTest.Fail();
            }
        }
    }
}