namespace Controllers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs;

    using Interfaces;
    using Interfaces.Controllers;

    using UnityEngine;

    public class ElectionQuestionUIController : QuestionUIController, IElectionQuestionUIController
    {
        private readonly Dictionary<string, IElectionBubbleUIController> answersElectionBubbles =
            new Dictionary<string, IElectionBubbleUIController>();

        private Transform electionBubbleUIPrefab;

        public string HighestVotedAnswer
        {
            get
            {
                return this.answersElectionBubbles.OrderByDescending(aEb => aEb.Value.VoteCount)
                    .First()
                    .Key;
            }
        }

        public ElectionQuestionUIController()
        {
            this.OnQuestionLoaded += this.OnFirstQuestionLoaded;
        }

        private void OnFirstQuestionLoaded(object sender, SimpleQuestionEventArgs args)
        {
            this.electionBubbleUIPrefab = Resources.Load<Transform>("Prefabs/ElectionQuestionUI/ElectionBubbleUI");
            this.GenerateAnswerBubbles(args.Question);
            this.OnQuestionLoaded -= this.OnFirstQuestionLoaded;
            this.OnQuestionLoaded += this.OnCurrentQuestionLoaded;
        }

        private void OnCurrentQuestionLoaded(object sender, SimpleQuestionEventArgs args)
        {
            var answersBubbles = this.answersElectionBubbles.Values;
            answersBubbles.ToList().ForEach(ab => ab.ResetVotesToZero());
        }

        private void GenerateAnswerBubbles(ISimpleQuestion question)
        {
            for (int i = 0; i < question.Answers.Length; i++)
            {
                var answer = question.Answers[i];
                var answerObjIndex = this.GetAnswerIndex(answer);
                var answerObj = this.GetAnswerObj(answerObjIndex);
                var instance = Instantiate(this.electionBubbleUIPrefab, answerObj.transform);

                var rectTransform = instance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2();

                var controller = instance.GetComponent<ElectionBubbleUIController>();
                this.answersElectionBubbles.Add(answer, controller);
            }
        }

        public void AddVoteFor(string answer)
        {
            if (!this.answersElectionBubbles.ContainsKey(answer))
            {
                throw new ArgumentException("Answer doesnt exists");
            }

            this.answersElectionBubbles[answer].AddVote();
        }
    }
}