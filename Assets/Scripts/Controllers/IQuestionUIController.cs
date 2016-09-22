using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

public interface IQuestionUIController
{
    EventHandler<AnswerEventArgs> OnAnswerClick
    {
        get;
        set;
    }

    EventHandler<QuestionEventArgs> OnQuestionLoaded
    {
        get;
        set;
    }

    void HideAnswer(int index);

    void HideAllAnswers();

    void ShowAllAnswers();

    void LoadQuestion(Question question);

}