namespace Interfaces.Controllers
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

        void ChangeAnswersCount(int count);

        void HideAnswer(string answer);

        void HideAnswer(int index);

        void HideAllAnswers();

        void ShowAllAnswers();

        void LoadQuestion(ISimpleQuestion question);

        void DisableAnswerInteractivity(string answer);

        void DisableAnswerInteractivity(int answerIndex);

        void DisableAllAnswersInteractivity();
    }
}