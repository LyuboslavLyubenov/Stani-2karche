using System;


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