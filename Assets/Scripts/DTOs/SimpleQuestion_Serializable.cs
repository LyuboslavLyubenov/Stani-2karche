namespace Assets.Scripts.DTOs
{

    using System;

    using Assets.Scripts.Interfaces;

    [Serializable]
    public class SimpleQuestion_Serializable
    {
        public string Text;
        public string[] Answers;
        public int CorrectAnswerIndex;

        public SimpleQuestion_Serializable(ISimpleQuestion question)
        {
            this.Text = question.Text;
            this.Answers = question.Answers;
            this.CorrectAnswerIndex = question.CorrectAnswerIndex;
        }

        public SimpleQuestion_Serializable()
        {
        
        }

        public ISimpleQuestion Deserialize()
        {
            return new SimpleQuestion(this.Text, this.Answers, this.CorrectAnswerIndex);
        }
    }

}