namespace Tests.UI.ElectionQuestionUIController
{
    using System.Collections.Generic;
    using System.Linq;

    using Controllers;

    using DTOs;

    using Interfaces;
    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils;
    using Utils.Unity;

    using Zenject.Source.Usage;

    public class ResetsVotesOnNextQuestion : ExtendedMonoBehaviour
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
                        for (int i = 0; i < 4; i++)
                        {
                            var answer = this.question.Answers.GetRandomElement();
                            this.electionQuestionUiController.AddVoteFor(answer);
                        }

                        this.CoroutineUtils.WaitForFrames(1,
                            () =>
                                {
                                    var newQuestion = new SimpleQuestion("QuestionText2", new[] { "Answer1", "Answer2", "Answer3", "Answer4" }, 0);
                                    this.electionQuestionUiController.LoadQuestion(newQuestion);

                                    this.CoroutineUtils.WaitForSeconds(1f,
                                        () =>
                                        {
                                            var answersObjs = GameObject.FindGameObjectsWithTag("Answer");
                                            Dictionary<string, ElectionBubbleUIController> answerObjsVoteBubbleText =
                                                answersObjs.ToDictionary(
                                                    k => k.GetComponentInChildren<Text>().text,
                                                    v => v.GetComponentInChildren<ElectionBubbleUIController>());

                                            for (int i = 0; i < newQuestion.Answers.Length; i++)
                                            {
                                                var answer = newQuestion.Answers[i];

                                                if (answerObjsVoteBubbleText[answer].VoteCount != 0)
                                                {
                                                    IntegrationTest.Fail();
                                                    return;
                                                }
                                            }

                                            IntegrationTest.Pass();
                                        });
                                });
                    });
        }
    }
}