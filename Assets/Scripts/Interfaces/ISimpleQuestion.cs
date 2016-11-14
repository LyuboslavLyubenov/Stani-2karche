using System;

public interface ISimpleQuestion
{
    string Text
    {
        get;
    }

    string[] Answers
    {
        get;
    }

    int CorrectAnswerIndex
    {
        get;
    }

    string CorrectAnswer
    {
        get;
    }

    SimpleQuestion_Serializable Serialize();
}