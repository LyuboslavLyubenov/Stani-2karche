using System;

public class ISimpleQuestion : ISimpleQuestion
{
    public string Text
    {
        get
        {
            return this.text;
        }
    }

    public string[] Answers
    {
        get
        {
            return this.answers;
        }
    }

    public int CorrectAnswerIndex
    {
        get
        {
            return this.correctAnswerIndex;
        }
    }

    string text;
    string[] answers;
    int correctAnswerIndex;

    public ISimpleQuestion(string text, string[] answers, int correctAnswerIndex)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Въпросът не трябва да е празен");
        }

        if (answers.Length != 4)
        {
            throw new ArgumentException("Отговорите трябва да са 4 на брои");
        }

        for (int i = 0; i < answers.Length; i++)
        {
            if (string.IsNullOrEmpty(answers[i]))
            {
                throw new ArgumentException("Отговор номер " + i + "е празен. Невъзможно е да има празен отговор.");
            }
        }
            
        if (correctAnswerIndex < 0 || correctAnswerIndex > 3)
        {
            throw new ArgumentOutOfRangeException("correctAnswerIndex", "correctAnswerIndex трябва да бъде със стойности между 0 и 3 (включително)");
        }

        this.text = text;
        this.answers = answers;
        this.correctAnswerIndex = correctAnswerIndex;
    }

    public SimpleQuestion_Serializable Serialize()
    {
        return new SimpleQuestion_Serializable(this);
    }
}

[Serializable]
public class SimpleQuestion_Serializable
{
    public string Text;
    public string[] Answers;
    public int CorrectAnswerIndex;

    public SimpleQuestion_Serializable(ISimpleQuestion question)
    {
        this.Text = question.Text;
        this.Answers = question.Answers;
        this.CorrectAnswerIndex = question.CorrectAnswerIndex;
    }

    public SimpleQuestion_Serializable()
    {
        
    }

    public ISimpleQuestion Deserialize()
    {
        return new ISimpleQuestion(this.Text, this.Answers, this.CorrectAnswerIndex);
    }
}