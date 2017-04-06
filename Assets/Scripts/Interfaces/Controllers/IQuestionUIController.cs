namespace Interfaces
{

    using System;

    using EventArgs;

    public interface IQuestionUIController
    {
        EventHandler<AnswerEventArgs> OnAnswerClick
        {
            get;
            set;
        }

        EventHandler<SimpleQuestionEventArgs> OnQuestionLoaded
        {
            get;
            set;
        }

        ISimpleQuestion CurrentlyLoadedQuestion
        {
            get;
        }

        void HideAnswer(int index);

        void HideAllAnswers();

        void ShowAllAnswers();

        void LoadQuestion(ISimpleQuestion question);

    }

}