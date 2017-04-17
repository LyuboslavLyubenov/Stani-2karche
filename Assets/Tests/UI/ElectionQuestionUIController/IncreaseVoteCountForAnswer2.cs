namespace Tests.UI.ElectionQuestionUIController
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Utils;

    using Controllers;

    using Interfaces;
    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils;
    using Utils.Unity;

    using Zenject.Source.Usage;

    public class IncreaseVoteCountForAnswer2 : ExtendedMonoBehaviour
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
                        
                        var correctAnswerVotedCount = 0;

                        for (int i = 0; i < 5; i++)
                        {
                            var answer = this.question.Answers.GetRandomElement();

                            if (answer == this.question.CorrectAnswer)
                            {
                                correctAnswerVotedCount++;
                            }

                            this.electionQuestionUiController.AddVoteFor(answer);
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            this.electionQuestionUiController.AddVoteFor(this.question.CorrectAnswer);
                            correctAnswerVotedCount++;
                        }

                        if (answerObjsVoteBubbleText[this.question.CorrectAnswer].VoteCount == correctAnswerVotedCount)
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