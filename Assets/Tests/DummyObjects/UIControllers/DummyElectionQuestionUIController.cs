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
        public event EventHandler<ChangedAnswersCountEventArgs> OnChangedAnswersCount = delegate { };
        public event EventHandler<AnswerEventArgs> OnHideAnswer = delegate { };
        public event EventHandler OnHideAllAnswers = delegate { };
        public event EventHandler OnShowAllAnswers = delegate { };
        public event EventHandler<AnswerEventArgs> OnDisabledAnswerInteractivity = delegate { };
        public event EventHandler OnDisableAllAnswersInteractivity = delegate { };

        private Dictionary<string, int> answersVotes = new Dictionary<string, int>();

        public EventHandler<AnswerEventArgs> OnAnswerClick
        {
            get; set;
        }

        public EventHandler<SimpleQuestionEventArgs> OnQuestionLoaded
        {
            get; set;
        }

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

        public DummyElectionQuestionUIController()
        {
            this.OnAnswerClick = delegate
                {
                };
            this.OnQuestionLoaded = delegate
                {
                };
        }

        public void ChangeAnswersCount(int count)
        {
            this.OnChangedAnswersCount(this, new ChangedAnswersCountEventArgs(count));
        }

        public void HideAnswer(string answer)
        {
            this.OnHideAnswer(this, new AnswerEventArgs(answer, null));
        }

        public void HideAnswer(int index)
        {
            var answer = this.answersVotes.ToArray()[index]
                .Key;
            this.HideAnswer(answer);
        }
        
        public void HideAllAnswers()
        {
            this.OnHideAllAnswers(this, EventArgs.Empty);
        }

        public void ShowAllAnswers()
        {
            this.OnShowAllAnswers(this, EventArgs.Empty);
        }

        public void LoadQuestion(ISimpleQuestion question)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }

            this.answersVotes.Clear();
            this.CurrentlyLoadedQuestion = question;

            for (int i = 0; i < this.CurrentlyLoadedQuestion.Answers.Length; i++)
            {
                var answer = this.CurrentlyLoadedQuestion.Answers[i];
                this.answersVotes.Add(answer, 0);
            }

            this.OnQuestionLoaded(this, new SimpleQuestionEventArgs(question));
        }

        public void DisableAnswerInteractivity(string answer)
        {
            this.OnDisabledAnswerInteractivity(this, new AnswerEventArgs(answer, null));
        }

        public void DisableAnswerInteractivity(int answerIndex)
        {
            var answer = this.answersVotes.ToArray()[answerIndex].Key;
            this.DisableAnswerInteractivity(answer);
        }

        public void DisableAllAnswersInteractivity()
        {
            this.OnDisableAllAnswersInteractivity(this, EventArgs.Empty);
        }

        public void AddVoteFor(string answer)
        {
            this.answersVotes[answer]++;
        }
    }

    public class ChangedAnswersCountEventArgs : EventArgs
    {
        public int Count
        {
            get; private set;
        }

        public ChangedAnswersCountEventArgs(int count)
        {
            this.Count = count;
        }
    }
}