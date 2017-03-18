namespace Exceptions
{

    using System;

    public class NoMoreQuestionsException : Exception
    {
        public NoMoreQuestionsException(string message)
            : base(message)
        {
        }

        public NoMoreQuestionsException()
        {
        
        }
    }

}