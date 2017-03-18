namespace DTOs
{

    using System;

    using Interfaces;

    public class ExtractedQuestion
    {
        public ISimpleQuestion Question
        {
            get;
            private set;
        }

        public int SecondsForAnswerQuestion
        {
            get;
            private set;
        }

        public ExtractedQuestion(ISimpleQuestion question, int secondsForAnswerQuestion)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }

            if (secondsForAnswerQuestion <= 0)
            {
                throw new ArgumentOutOfRangeException("secondsForAnswerQuestion");
            }

            this.Question = question;
            this.SecondsForAnswerQuestion = secondsForAnswerQuestion;
        }
    }

}