namespace EventArgs
{

    using System;

    using Interfaces;

    using Network;

    using EventArgs = System.EventArgs;

    public class ServerSentQuestionEventArgs : EventArgs
    {
        public ISimpleQuestion Question
        {
            get;
            private set;
        }

        public QuestionRequestType QuestionType
        {
            get;
            private set;
        }

        public int ClientId
        {
            get;
            private set;
        }

        public ServerSentQuestionEventArgs(ISimpleQuestion question, QuestionRequestType questionType, int clientId)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }

            if (clientId <= 0)
            {
                throw new ArgumentOutOfRangeException("clientId");
            }
            
            this.Question = question;
            this.QuestionType = questionType;
            this.ClientId = clientId;
        }
    }

}
