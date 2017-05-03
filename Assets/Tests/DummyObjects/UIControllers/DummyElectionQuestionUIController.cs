namespace Assets.Tests.DummyObjects.UIControllers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using EventArgs;

    using Interfaces;
    using Interfaces.Controllers;
    public class DummyElectionQuestionUIController : IElectionQuestionUIController
    {
        public EventHandler<AnswerEventArgs> OnAnswerClick
        {
            get; set;
        }

        public EventHandler<SimpleQuestionEventArgs> OnQuestionLoaded
        {
            get; set;
        }

        private Dictionary<string, int> answersVotes = new Dictionary<string, int>();

        public ISimpleQuestion CurrentlyLoadedQuestion
        {
            get; private set;
        }

        public string HighestVotedAnswer
        {
            get
            {
                return this.answersVotes.OrderByDescending(av => av.Value)
                    .First()
                    .Key;
            }
        }

        public void HideAnswer(int index)
        {
        }

        public DummyElectionQuestionUIController()
        {
            this.OnAnswerClick = delegate { };
            this.OnQuestionLoaded = delegate { };
        }

        public void HideAllAnswers()
        {
        }

        public void ShowAllAnswers()
        {
        }

        public void LoadQuestion(ISimpleQuestion question)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }

            this.answersVotes.Clear();
            this.CurrentlyLoadedQuestion = question;
            this.OnQuestionLoaded(this, new SimpleQuestionEventArgs(question));
        }

        public void AddVoteFor(string answer)
        {
            if (!this.answersVotes.ContainsKey(answer))
            {
                this.answersVotes.Add(answer, 0);
            }

            this.answersVotes[answer]++;
        }
    }

}