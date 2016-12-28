namespace Assets.Scripts
{

    public class AnswerExtractionResult
    {
        public string[] Answers
        {
            get;
            private set;
        }

        public int CorrectAnswerIndex
        {
            get;
            private set;
        }

        public AnswerExtractionResult(string[] answers, int correctAnswerIndex)
        {
            this.Answers = answers;
            this.CorrectAnswerIndex = correctAnswerIndex;
        }
    }

}