using System;


public class QuestionEventArgs : EventArgs
{
    public QuestionEventArgs(Question question)
    {
        if (question == null)
        {
            throw new ArgumentNullException("question");
        }

        this.Question = question;
    }

    public Question Question
    {
        get;
        set;
    }
}