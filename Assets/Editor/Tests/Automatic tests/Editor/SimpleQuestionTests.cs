using System;

using NUnit.Framework;

namespace Assets.Editor.Tests.Automatic_tests.Editor
{

    using Assets.Scripts;

    public class SimpleQuestionTests
    {
    
        [Test]
        public void CreateValidSimpleQuestion()
        {
            var answers = new string[] { "otgovor1", "otgovor2", "otgovor3", "tgogoovr4" };
            var simpleQuestion = new SimpleQuestion("Valid", answers, 0);
            Assert.Pass();
        }

        [Test]
        public void InvalidAnswersCount1()
        {
            var answers = new string[] { "otgovor1", "otgovor2", "otgovor3", "tgogoovr4", "too much bro" };
            Assert.Throws<ArgumentException>(new TestDelegate(() => new SimpleQuestion("Valid", answers, 0)));
        }

        [Test]
        public void InvalidAnswersCount2()
        {
            var answers = new string[] { "too little bra" };
            Assert.Throws<ArgumentException>(new TestDelegate(() => new SimpleQuestion("Valid", answers, 0)));
        }

        [Test]
        public void EmptyQuestionText()
        {
            var answers = new string[] { "otgovor1", "otgovor2", "otgovor3", "tgogoovr4" };
            Assert.Throws<ArgumentException>(new TestDelegate(() => new SimpleQuestion("", answers, 0)));
        }

        [Test]
        public void InvalidCorrectAnswerIndex1()
        {
            var answers = new string[] { "otgovor1", "otgovor2", "otgovor3", "tgogoovr4" };
            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(() => new SimpleQuestion("Validen", answers, UnityEngine.Random.Range(5, 100))));
        }

        [Test]
        public void InvalidCorrectAnswerIndex2()
        {
            var answers = new string[] { "otgovor1", "otgovor2", "otgovor3", "tgogoovr4" };
            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(() => new SimpleQuestion("Validen", answers, -UnityEngine.Random.Range(5, 100))));
        }

        [Test]
        public void Serialization()
        {
            var answers = new string[] { "otgovor1", "otgovor2", "otgovor3", "tgogoovr4" };
            var simpleQuestion = new SimpleQuestion("Valid", answers, 0);
            var simpleQuestionSerialized = simpleQuestion.Serialize();

            Assert.AreEqual(simpleQuestion.Text, simpleQuestionSerialized.Text);
            Assert.AreEqual(simpleQuestion.CorrectAnswerIndex, simpleQuestion.CorrectAnswerIndex);

            for (int i = 0; i < simpleQuestion.Answers.Length; i++)
            {
                var answerOriginal = simpleQuestion.Answers[i];
                var answerSerialized = simpleQuestionSerialized.Answers[i];

                Assert.AreEqual(answerOriginal, answerSerialized);
            }
        }
    }

}
