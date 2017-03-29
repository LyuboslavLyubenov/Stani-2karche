namespace Tests.UI.ElectionQuestionUIController
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.Controllers;

    using Controllers;

    using Interfaces;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class IncreaseVoteCountForAnswer1 : ExtendedMonoBehaviour
    {
        [Inject]
        private IElectionQuestionUIController electionQuestionUiController;

        [Inject]
        private ISimpleQuestion question;
        
        void Start()
        {
            this.electionQuestionUiController.LoadQuestion(this.question);
            this.CoroutineUtils.WaitForSeconds(1f,
                () =>
                    {
                        var answersObjs = GameObject.FindGameObjectsWithTag("Answer");
                        Dictionary<string, ElectionBubbleUIController> answerObjsVoteBubbleText =
                            answersObjs.ToDictionary(
                                k => k.GetComponentInChildren<Text>().text,
                                v => v.GetComponentInChildren<ElectionBubbleUIController>());

                        this.electionQuestionUiController.AddVoteFor(this.question.CorrectAnswer);

                        if (answerObjsVoteBubbleText[this.question.CorrectAnswer].VoteCount == 1)
                        {
                            IntegrationTest.Pass();
                        }
                        else
                        {
                            IntegrationTest.Fail();
                        }
                    });
        }
    }
}