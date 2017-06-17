namespace Controllers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs;

    using Interfaces;
    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    public class ElectionQuestionUIController : QuestionUIController, IElectionQuestionUIController
    {
        private readonly Dictionary<Text, ElectionBubbleUIController> answersElectionBubbles = new Dictionary<Text, ElectionBubbleUIController>();

        private Transform electionBubbleUIPrefab;

        public string HighestVotedAnswer
        {
            get
            {
                return this.answersElectionBubbles.OrderByDescending(aEb => aEb.Value.VoteCount)
                    .First()
                    .Key
                    .text;
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
            var answersBubbles = this.answersElectionBubbles.Values.ToArray();

            for (int i = 0; i < answersBubbles.Length; i++)
            {
                var answerBubble = answersBubbles[i];
                answerBubble.ResetVotesToZero();
            } 
        }

        private void GenerateAnswerBubbles(ISimpleQuestion question)
        {
            for (int i = 0; i < question.Answers.Length; i++)
            {
                var answer = question.Answers[i];
                var answerObjIndex = this.GetAnswerIndex(answer);
                var answerObj = this.GetAnswerObj(answerObjIndex);
                var answerTextComponent = answerObj.GetComponentInChildren<Text>();
                var instance = Instantiate(this.electionBubbleUIPrefab, answerObj.transform);

                var rectTransform = instance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2();

                var controller = instance.GetComponent<ElectionBubbleUIController>();
                this.answersElectionBubbles.Add(answerTextComponent, controller);
            }
        }

        public void AddVoteFor(string answer)
        {
            if (this.answersElectionBubbles.All(a => a.Key.text != answer))
            {
                throw new ArgumentException("Answer doesnt exists");
            }

            var electionBubble = this.answersElectionBubbles.First(a => a.Key.text == answer).Value;
            electionBubble.AddVote();
        }
    }
}