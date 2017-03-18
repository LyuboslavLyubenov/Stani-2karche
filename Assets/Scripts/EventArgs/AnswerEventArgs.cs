namespace EventArgs
{

    using System;

    public class AnswerEventArgs : EventArgs
    {
        public AnswerEventArgs(string answer, bool? isCorrect)
        {
            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentNullException("answer");
            }

            this.Answer = answer;
            this.IsCorrect = isCorrect;
        }

        public string Answer
        {
            get;
            private set;
        }

        public bool? IsCorrect
        {
            get;
            private set;
        }
    }

    [Serializable]
    public class AnswerEventArgs_Serializable
    {
        public string Answer;
        public bool? IsCorrect;
    }

}