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

    ISimpleQuestion_Serializable Serialize();
}

public interface ISimpleQuestion_Serializable
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

    ISimpleQuestion Deserialize();
}