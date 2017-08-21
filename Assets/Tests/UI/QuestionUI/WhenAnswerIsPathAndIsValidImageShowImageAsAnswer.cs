using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.UI.QuestionUI
{

    using System.Collections;
    using System.Reflection;

    using Assets.Tests.Utils;
    
    using Interfaces;
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
            question.Answers[0] = "images\\testimage.jpg";
            this.questionUIController.LoadQuestion(question);

            yield return new WaitForSeconds(3);

            var answerObj = GameObject.FindObjectOfType<Button>();
            var imageComponent = answerObj.transform.GetChild(0)
                .GetComponent<Image>();

            if (imageComponent != null)
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