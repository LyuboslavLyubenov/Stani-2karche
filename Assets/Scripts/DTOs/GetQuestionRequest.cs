namespace DTOs
{

    using System;

    using Interfaces;

    public class GetQuestionRequest
    {
        public delegate void OnSuccessfullyLoadedDelegate(ISimpleQuestion question);

        public delegate void OnExceptionDelegate(Exception exception);

        public OnSuccessfullyLoadedDelegate OnLoaded
        {
            get;
            private set;
        }

        public OnExceptionDelegate OnException
        {
            get;
            private set;
        }

        public GetQuestionRequest(OnSuccessfullyLoadedDelegate onSuccessfullyLoaded, OnExceptionDelegate onException)
        {
            if (onSuccessfullyLoaded == null)
            {
                throw new ArgumentNullException("onSuccessfullyLoaded");
            }

            this.OnLoaded = onSuccessfullyLoaded;
            this.OnException = onException;
        }
    }

}