namespace Tests.UI.ElectionQuestionUIController
{

    using Assets.Scripts.Interfaces.Controllers;
    using Interfaces;
    using UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Utils.Unity;
    using Zenject.Source.Usage;

    public class HighestVotedAnswer : ExtendedMonoBehaviour
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
                        for (int i = 0; i < this.question.Answers.Length; i++)
                        {
                            var answer = this.question.Answers[i];
                            this.electionQuestionUiController.AddVoteFor(answer);
                        }

                        this.electionQuestionUiController.AddVoteFor(this.question.CorrectAnswer);

                        if (this.electionQuestionUiController.HighestVotedAnswer == this.question.CorrectAnswer)
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