using System;

namespace Assets.Scripts.EventArgs
{

    using Assets.Scripts.Interfaces;

    using EventArgs = System.EventArgs;

    public class SimpleQuestionEventArgs : EventArgs
    {
        public SimpleQuestionEventArgs(ISimpleQuestion question)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }

            this.Question = question;
        }

        public ISimpleQuestion Question
        {
            get;
            set;
        }
    }

}