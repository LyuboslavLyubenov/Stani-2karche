namespace Assets.Tests.Utils
{
    using System;
    using System.Text;

    using DTOs;

    using Interfaces;
    
    public class QuestionGenerator
    {
        private const int StartChar = (int)'a';
        private const int EndChar = (int)'z';
        private const int QuestionTextMinLength = 5;
        private const int QuestionTextMaxLength = 10;
        private const int AnswerTextMinLength = 3;
        private const int AnswerTextMaxLength = 8;

        private Random random = new Random();

        private string GenerateStringWithRandomLetters(int length)
        {
            var questionText = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var letter = (char)this.random.Next(StartChar, EndChar + 1);
                questionText.Append(letter);
            }

            return questionText.ToString();
        }

        private string GenerateQuestionText()
        {
            var length = this.random.Next(QuestionTextMinLength, QuestionTextMaxLength);
            return this.GenerateStringWithRandomLetters(length);
        }

        private string[] GenerateAnswers()
        {
            var answers = new string[4];

            for (int i = 0; i < 4; i++)
            {
                var length = this.random.Next(AnswerTextMinLength, AnswerTextMaxLength);
                answers[i] = GenerateStringWithRandomLetters(length);
            }

            return answers;
        }

        public ISimpleQuestion GenerateQuestion()
        {
            var questionText = this.GenerateQuestionText();
            var answers = this.GenerateAnswers();
            var correctAnswerIndex = this.random.Next(answers.Length);
            return new SimpleQuestion(questionText, answers, correctAnswerIndex);
        }
    }
}
