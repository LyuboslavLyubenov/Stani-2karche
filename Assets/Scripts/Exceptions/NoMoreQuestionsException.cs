using System;

namespace Assets.Scripts.Exceptions
{

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